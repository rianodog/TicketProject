using AutoMapper;
using Azure.Core;
using MediatR;
using System.Text.Json;
using TicketProject.Models.Dto;
using TicketProject.Models.Dto.ButTicket;
using TicketProject.Models.Dto.BuyTicketHandlerAsync;
using TicketProject.Models.Dto.TicketService;
using TicketProject.Services.Interfaces;
using static TicketProject.Models.Enums;

namespace TicketProject.Commands.Handlers
{
    public class BuyTicketHandlerAsync : IRequestHandler<BuyTicketCommand, bool>
    {
        private readonly IRedisService _redisService;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IErrorHandler<BuyTicketHandlerAsync> _errorHandler;
        private readonly IMapper _mapper;
        private readonly string _exchange = "ticket_exchange";
        private readonly string _routekey = "ticket_routekey";
        private readonly string _buyTicketLuaScript = @"
                    -- KEYS[1]: Campaign key
                    -- ARGV[1]: BuyList JSON
                    -- ARGV[2]: Timestamp

                    local campaignKey = KEYS[1]
                    local buyListTable = cjson.decode(ARGV[1])
                    local timestamp = ARGV[2]

                    local campaignJson = redis.call('JSON.GET', campaignKey)
                    if not campaignJson then
                        return cjson.encode({ Status = 'Error', Result = 'Cmapaign_NotFound' })
                    end
                    local campaign = cjson.decode(campaignJson)

                    for _, buyItem in ipairs(buyListTable) do

                        local typeName = buyItem.TypeName
                        local quantity = buyItem.Quantity
                        local ticketFound = false
                        

                        for i, ticket in ipairs(campaign.TicketContents) do
                            if ticket.TypeName == typeName then
                                ticketFound = true
                                if ticket.QuantityAvailable < quantity then
                                    return cjson.encode({ Status = 'Sold_out'})
                                end
                                break
                            end
                        end
                        if not ticketFound then
                            return cjson.encode({ Status = 'Error', Result = 'TicketType_NotFound' })
                        end
                    end

                    for _, buyItem in ipairs(buyListTable) do
                        local typeName = buyItem.TypeName
                        local quantity = buyItem.Quantity
                        for i, ticket in ipairs(campaign.TicketContents) do
                            if ticket.TypeName == typeName then
                                campaign.TicketContents[i].QuantityAvailable = campaign.TicketContents[i].QuantityAvailable - quantity
                                campaign.TicketContents[i].QuantitySold = campaign.TicketContents[i].QuantitySold + quantity
                                campaign.TicketContents[i].UpdateAt = timestamp
                                break
                            end
                        end
                    end

                    local updatedCampaignJson = cjson.encode(campaign)

                    -- 為了排除Lua內只有Table導致Json解析後空陣列會變為物件的問題，將Tickets和OrderItems強制設為空陣列
                    updatedCampaignJson = string.gsub(updatedCampaignJson, 'Tickets\"":{}', 'Tickets\"":[]')
                    updatedCampaignJson = string.gsub(updatedCampaignJson, 'OrderItems\"":{}', 'OrderItems\"":[]')
                    
                    redis.log(redis.LOG_DEBUG, cjson.encode({ Status = 'Success', Result = updatedCampaignJson }))

                    redis.call('JSON.SET', campaignKey, '.', updatedCampaignJson)

                    -- redis.call('JSON.SET', campaignKey, '.TicketContents[*].Tickets', '[]')
                    -- redis.call('JSON.SET', campaignKey, '.TicketContents[*].OrderItems', '[]')
    
                    return cjson.encode({ Status = 'Success', Result = updatedCampaignJson })

                ";

        public BuyTicketHandlerAsync(IRedisService redisService,
            IRabbitMQService rabbitMQService,
            IErrorHandler<BuyTicketHandlerAsync> errorHandler,
            IMapper mapper)
        {
            _redisService = redisService;
            _rabbitMQService = rabbitMQService;
            _errorHandler = errorHandler;
            _mapper = mapper;
        }

        public async Task<bool> Handle(BuyTicketCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var luaResult = await ExcuteLuaSctipt(request);

                // 以下區塊統計消耗的時間低到可忽略不計
                //var stopwatch = Stopwatch.StartNew();
                if (luaResult.Status == ResultStatus.Success)
                {
                    var updatedCampaign = JsonSerializer.Deserialize<CampaignDto>(luaResult.Result);
                    var updateTicketContentDtos = new List<UpdateTicketContentDto>();

                    var order = new OrderDto
                    {
                        UserId = int.Parse(request.UserId),
                        OrderDate = DateTime.UtcNow,
                        OrderItems = []
                    };

                    // 構建訂單和更新 DTO
                    if (!HandleBuyList(request, updatedCampaign!.TicketContents.ToList(), ref order, ref updateTicketContentDtos))
                        return false;

                    var insertDataDto = new InsertDataDto
                    {
                        Order = _mapper.Map<OrderDto>(order),
                        UpdateTicketContentDtos = updateTicketContentDtos
                    };
                    // 將資料整理插入Queue待批次處理
                    await _rabbitMQService.PublishMessage(_exchange, JsonSerializer.Serialize(insertDataDto), _routekey);
                    //stopwatch.Stop();
                    //_errorHandler.Debug($"{stopwatch.ElapsedMilliseconds}");
                    return true;
                }
                else if (luaResult.Status == ResultStatus.Sold_out)
                    return false;
                else
                    throw new Exception($"Lua Script Error: {luaResult.Result}");
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        private async Task<LuaResultDto> ExcuteLuaSctipt(BuyTicketCommand request)
        {
            var keys = new[] { $"Campaign:Id:{request.CampaignId}" };
            var buyListJson = JsonSerializer.Serialize(request.BuyList);
            var args = new[] { buyListJson, DateTime.UtcNow.ToString("o") };
            var lusResultJson = await _redisService.ExecuteLuaScriptAsync(_buyTicketLuaScript, keys, args);
            return JsonSerializer.Deserialize<LuaResultDto>(lusResultJson)!;
        }

        private static bool HandleBuyList(BuyTicketCommand request, List<TicketContentDto> ticketContents,
                ref OrderDto order, ref List<UpdateTicketContentDto> updateTicketContentDtos)
        {
            foreach (var item in request.BuyList)
            {
                var ticketContent = ticketContents.First(t => t.TypeName == item.TypeName);

                // 此處已經在 Lua 腳本中扣減了數量，所以直接更新 Order 和 UpdateTicketContentDtos
                updateTicketContentDtos.Add(new UpdateTicketContentDto
                {
                    TicketContentDto = ticketContent,
                    Quantity = item.Quantity
                });

                order.TotalAmount += ticketContent.Price * item.Quantity;

                order.OrderItems.Add(new OrderItemDto
                {
                    TicketContentId = ticketContent.TicketContentId,
                    Quantity = item.Quantity,
                    Ticket = new TicketDto
                    {
                        UserId = Convert.ToInt32(request.UserId),
                        TicketContentId = ticketContent.TicketContentId,
                    }
                });
            }
            return true;
        }
    }
}

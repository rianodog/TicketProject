using EFCore.BulkExtensions;
using TicketProject.DAL.Interfaces;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;

namespace TicketProject.DAL.Implement
{
    /// <summary>
    /// 實作訂單相關資料操作的資料存取物件。
    /// </summary>
    public class OrderWriteDao : IOrderWriteDao
    {
        private readonly WriteTicketDbContext _dbContext;
        private readonly IErrorHandler<OrderWriteDao> _errorHandler;
        private readonly int batchSize = 1000;

        /// <summary>
        /// 初始化 <see cref="OrderWriteDao"/> 類別的新執行個體。
        /// </summary>
        /// <param name="writeTicketDbContext">寫入資料庫的上下文。</param>
        /// <param name="errorHandler">錯誤處理器。</param>
        public OrderWriteDao(WriteTicketDbContext writeTicketDbContext, IErrorHandler<OrderWriteDao> errorHandler)
        {
            _dbContext = writeTicketDbContext;
            _errorHandler = errorHandler;
        }

        /// <summary>
        /// 批次建立訂單，並包含相關的訂單項目和票券，
        /// 使用 BulkInsertAsync 並分批插入以提升效能，
        /// 資料分批插入是為了提高插入效能和穩定性，減少交易的持續時間，控制記憶體使用
        /// </summary>
        /// <param name="orders">要建立的訂單集合。</param>
        /// <returns>執行緒任務。</returns>
        public async Task CreateOrdersAsync(ICollection<Order> orders)
        {
            try
            {
                // 禁用變更追蹤以提升效能
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

                // 將訂單分成多個批次並插入
                await BulkInsertWithBatchesAsync(orders, async orderBatch =>
                {
                    await _dbContext.BulkInsertAsync(orderBatch, new BulkConfig
                    {
                        // 遇到子物件與它的子物件在超過一層關係後的關聯出現錯誤
                        // 發現有人回復類似的issues 作者的建議竟然是不要使用 因為它是外部PR...
                        // https://github.com/borisdj/EFCore.BulkExtensions/issues/919
                        // 新版本有針對這個API進行更新 但實際使用還是有遇到問題
                        // 決定與其將三層分開插入 不如最後將未關聯成功的欄位進行Update
                        IncludeGraph = true, // 包含相關的物件，例如 OrderItems
                        SetOutputIdentity = true // 讓自動產生的 Id 回傳，供相關物件使用
                    });

                    // 設置 Ticket 的 OrderItemId
                    foreach (var order in orderBatch)
                    {
                        foreach (var orderItem in order.OrderItems)
                        {
                            orderItem.Ticket!.OrderItemId = orderItem.OrderItemId;
                        }
                    }

                    await _dbContext.BulkUpdateAsync(orderBatch.SelectMany(o => o.OrderItems).Select(o => o.Ticket!));

                });
            }
            catch (Exception e)
            {
                // 處理例外狀況
                _errorHandler.HandleError(e);
                throw;
            }
            finally
            {
                // 恢復變更追蹤
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        /// <summary>
        /// 建立訂單項目。
        /// </summary>
        /// <param name="orderItems">要建立的訂單項目清單。</param>
        /// <returns>建立後的訂單項目清單。</returns>
        public async Task<List<OrderItem>> CreateOrderItemAsync(List<OrderItem> orderItems)
        {
            try
            {
                await _dbContext.BulkInsertAsync(orderItems, new BulkConfig
                {
                    SetOutputIdentity = true // 確保回寫 OrderItemId
                });
                return orderItems;
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
                throw;
            }
        }

        /// <summary>
        /// 將集合分批插入資料庫。
        /// </summary>
        /// <typeparam name="T">實體類型。</typeparam>
        /// <param name="entities">要插入的實體集合。</param>
        /// <param name="insertAction">批次插入動作。</param>
        /// <returns>執行緒任務。</returns>
        private async Task BulkInsertWithBatchesAsync<T>(ICollection<T> entities, Func<List<T>, Task> insertAction)
        {
            var batches = entities
                .Select((entity, index) => new { entity, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x => x.entity).ToList())
                .ToList();

            // 將所有資料分堆，並呼叫傳入的委派進行插入
            foreach (var batch in batches)
            {
                await insertAction(batch);
            }
        }
    }
}

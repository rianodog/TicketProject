using AutoMapper;
using TicketProject.Commands;
using TicketProject.Models.Dto;
using TicketProject.Models.Dto.CreateCampaignCommand;
using TicketProject.Models.Entity;
using static TicketProject.Models.Enums;

namespace TicketProject.AutoMapper
{
    public class DtoMappingProfile : Profile
    {
        public DtoMappingProfile()
        {
            CreateMap<Campaign, CampaignDto>()
                .AfterMap((src, dest) =>
                {
                    foreach (var ticketContent in dest.TicketContents)
                    {
                        ticketContent.Tickets ??= [];
                        ticketContent.OrderItems ??= [];
                    }
                })
                .ReverseMap()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Campaign, CreateCampaignCommand>()
                .ReverseMap()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<TicketContent, ValidTicketContentDto>()
                .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => Enum.Parse<TicketType>(src.TypeName)))
                .ReverseMap()
                .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.TypeName.ToString()))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<TicketContent, TicketContentDto>()
                .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => Enum.Parse<TicketType>(src.TypeName)))
                .ReverseMap()
                .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.TypeName))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Order, OrderDto>()
                .ReverseMap()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<OrderItem, OrderItemDto>()
                .ReverseMap()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Ticket, TicketDto>()
                .ReverseMap()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<User, UserDto>()
                .ReverseMap()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}

using AutoMapper;
using TicketProject.Commands;
using TicketProject.Models.Dto;
using TicketProject.Models.Entity;

/// <summary>  
/// AutoMapper 配置檔案，用於命令和實體之間的映射。  
/// </summary>  
public class CommandMappingProfile : Profile
{
    /// <summary>  
    /// 初始化 <see cref="CommandMappingProfile"/> 類別的新執行個體。  
    /// </summary>  
    public CommandMappingProfile()
    {
        // CreateEventHandler  
        CreateMap<CreateEventCommand, Event>()
            .ForMember(dest => dest.Tickets, opt => opt.Ignore())
            .ReverseMap()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<CreateTicketDto, Ticket>()
            .ReverseMap()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // RegisterHandler  
        CreateMap<RegisterCommand, User>()
            .ReverseMap()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}

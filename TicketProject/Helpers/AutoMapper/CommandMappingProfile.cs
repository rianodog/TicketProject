using AutoMapper;
using TicketProject.Commands;
using TicketProject.Models.Dto;
using TicketProject.Models.Entity;

/// <summary>  
/// AutoMapper �t�m�ɮסA�Ω�R�O�M���餧�����M�g�C  
/// </summary>  
public class CommandMappingProfile : Profile
{
    /// <summary>  
    /// ��l�� <see cref="CommandMappingProfile"/> ���O���s�������C  
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

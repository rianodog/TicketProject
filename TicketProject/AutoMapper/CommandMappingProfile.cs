using AutoMapper;
using TicketProject.Commands;
using TicketProject.Models.Dto;
using TicketProject.Models.Entity;
using TicketProject.Querys;
using static TicketProject.Models.Enums;

/// <summary>  
/// AutoMapper �t�m�ɮסA�Ω�R�O�M���餧�����M�g�C  
/// </summary>  
namespace TicketProject.AutoMapper
{
    public class CommandMappingProfile : Profile
    {
        /// <summary>  
        /// ��l�� <see cref="CommandMappingProfile"/> ���O���s�������C  
        /// </summary>  
        public CommandMappingProfile()
        {
            // CreateCampaignHandler  
            CreateMap<CreateCampaignCommand, Campaign>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City.ToString()))
                .ReverseMap()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => Enum.Parse<City>(src.City)))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // GetCampaignsHandller
            CreateMap<GetCampaignsQuery, Campaign>()
                .ReverseMap()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // RegisterHandler  
            CreateMap<RegisterCommand, User>()
                .ReverseMap()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
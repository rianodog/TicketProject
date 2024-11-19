using AutoMapper;
using TicketProject.Commands;
using TicketProject.Models.Dto;
using TicketProject.Models.Entity;
using TicketProject.Querys;
using static TicketProject.Models.Enums;

/// <summary>  
/// AutoMapper 配置檔案，用於命令和實體之間的映射。  
/// </summary>  
namespace TicketProject.AutoMapper
{
    public class CommandMappingProfile : Profile
    {
        /// <summary>  
        /// 初始化 <see cref="CommandMappingProfile"/> 類別的新執行個體。  
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
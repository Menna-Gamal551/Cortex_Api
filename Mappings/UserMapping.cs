
using AutoMapper;
using Cortex_Api.Data.Models;
using Cortex_Api.Data.Models.DTOs;

namespace Cortex_Api.Mappings
{
    public class UserMapping : Profile
    {
        public UserMapping ()
        {
            // Entity -> DTO
            CreateMap<User, UserDto>();

            // DTO -> Entity
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ForMember(dest => dest.device, opt => opt.Ignore())
                .ForMember(dest => dest.AccountMetaDatas, opt => opt.Ignore())
                .ForMember(dest => dest.Auditlogs, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityEvents, opt => opt.Ignore());
        }
    }
}


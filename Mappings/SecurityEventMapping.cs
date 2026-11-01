
using AutoMapper;
using Cortex_Api.Data.Models;
using Cortex_Api.Data.Models.DTOs;

namespace Cortex_Api.Mappings
{
    public class SecurityEventMappingProfile : Profile
    {
        public SecurityEventMappingProfile()
        {
            // Entity -> DTO
            CreateMap<SecurityEvent, SecurityEventDto>();

            // DTO -> Entity
            CreateMap<SecurityEventDto, SecurityEvent>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.user, opt => opt.Ignore())
                .ForMember(dest => dest.DeviceId, opt => opt.Ignore())
                .ForMember(dest => dest.device, opt => opt.Ignore());
        }
    }
}


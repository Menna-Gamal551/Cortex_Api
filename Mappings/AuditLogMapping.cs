
using AutoMapper;
using Cortex_Api.Data.Models;
using Cortex_Api.Data.Models.DTOs;

namespace Cortex_Api.Mappings
{
    public class AuditlogMapping : Profile
    {
        public AuditlogMapping()
        {
            // Entity -> DTO
            CreateMap<Auditlog, AuditlogDto>()
                .ForMember(
                    dest => dest.OccurredAt,
                    opt => opt.MapFrom(src => src.OccuredAt)
                );

            // DTO -> Entity
            CreateMap<AuditlogDto, Auditlog>()
                .ForMember(
                    dest => dest.OccuredAt,
                    opt => opt.MapFrom(src => src.OccurredAt)
                )
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.user, opt => opt.Ignore())
                .ForMember(dest => dest.DeviceId, opt => opt.Ignore())
                .ForMember(dest => dest.device, opt => opt.Ignore());
        }
    }
}


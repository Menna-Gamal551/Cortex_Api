
using AutoMapper;
using Cortex_Api.Data.Models;
using Cortex_Api.Data.Models.DTOs;

namespace Cortex_Api.Mappings
{
    public class AccountMetaDataMappingProfile : Profile
    {
        public AccountMetaDataMappingProfile()
        {
            // Entity -> Response DTO
            CreateMap<AccountMetaData, AccountMetaDataDto>();

            // Create DTO -> Entity
            CreateMap<CreateAccountMetaDataDto, AccountMetaData>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.user, opt => opt.Ignore())
                .ForMember(dest => dest.BreachResults, opt => opt.Ignore());

            // Update DTO -> Existing Entity
            CreateMap<UpdateAccountMetaDataDto, AccountMetaData>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.user, opt => opt.Ignore())
                .ForMember(dest => dest.BreachResults, opt => opt.Ignore());
        }
    }
}


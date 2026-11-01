using AutoMapper;
using Cortex_Api.Data.Models;
using Cortex_Api.Data.Models.DTOs.DeviceDTOs;

namespace Cortex_Api.Mappings
{
    public class DeviceMapping : Profile
    {
        public DeviceMapping()
        {
            CreateMap<Device, DeviceResponseDto>().ReverseMap();
        }

    }
}

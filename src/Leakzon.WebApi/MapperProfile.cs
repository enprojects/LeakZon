using AutoMapper;
using Leakzon.WebApi.ViewModels;
using Leakzone.Backend.Accessors.DbModels;
using Leakzone.Backend.Managers.Models;

namespace Leakzon.WebApi
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<SensorHourlyConsumption, SensorHourlyConsumptionViewModel>();
            CreateMap<HourlyConsumptionRequestViewModel, HourlyConsumptionRequest>();
            CreateMap<SensorInfo, SensorInfoViewModel>();
            CreateMap<SensorInfo, SensorReadingDb>()
                .ReverseMap();
        }
    }
}
using AutoMapper;
using SPlus.DTO;
using SPlus.Model.Domain;
using System.Linq;

namespace SPlus.UseCases
{
    public class StrategyMapper : Profile
    {
        public StrategyMapper()
        {
            CreateMap<Strategy, CAStrategyDTO>();
            CreateMap<CAStrategyDTO, Strategy>()
                .ForPath(a => a.YearString, s => { s.MapFrom(dt => dt.Years != null && dt.Years.Any() ? string.Join(",", dt.Years) : ""); });
            CreateMap<Strategy, CAStrategyThemeDTO>();
            CreateMap<Strategy, CAStrategyPerspectiveDTO>();
            CreateMap<Strategy, CAStrategyObjectiveDTO>();
            CreateMap<Strategy, StrategyObjectiveDTO>();

            CreateMap<Strategy, CAStrategyWeightDTO>();
            CreateMap<Strategy, CAStrategyObjectiveListDTO>();

        }

    }
}

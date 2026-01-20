using AutoMapper;
using SPlus.DTO;
using SPlus.Model.Domain;

namespace SPlus.UseCases
{
    public class ThemeMapper : Profile
    {
        public ThemeMapper()
        {
            CreateMap<Theme, CAThemeDTO>();
            CreateMap<Theme, CAThemeWeightDTO>();
            CreateMap<CAThemeDTO, Theme>()
                .ForMember(a => a.StrategyID, s => { s.MapFrom(dt => dt.Strategy.ID); })
                .ForPath(dto => dto.Strategy, opt => opt.Ignore());
            CreateMap<Theme, CAThemeStrategicObjectiveDTO>();

            CreateMap<Theme, ThemeDTO>();
            CreateMap<Theme, ThemeStrategicObjectiveDetailsDTO>();

            CreateMap<Theme, ThemeObjectiveDTO>();


        }

    }
}

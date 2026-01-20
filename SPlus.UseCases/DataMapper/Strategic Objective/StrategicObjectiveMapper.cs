using AutoMapper;
using SPlus.DTO;
using SPlus.Model.Domain;

namespace SPlus.UseCases
{
    public class StrategicObjectiveMapper : Profile
    {
        public StrategicObjectiveMapper()
        {
            CreateMap<StrategicObjective, CAStrategicObjectiveDTO>();
            CreateMap<StrategicObjective, CAStrategicObjectiveWeightDTO>();
            CreateMap<StrategicObjective, ObjectiveWeightDTO>();


            CreateMap<StrategicObjective, CAStrategicObjectiveKPIsDTO>();
            CreateMap<StrategicObjective, StrategicObjectiveWithLEDDTO>();
            CreateMap<StrategicObjective, CAStrategicObjectiveDTO>();

            CreateMap<StrategicObjective, StrategicObjectiveListDTO>();
            CreateMap<StrategicObjective, StrategicObjectiveChildDTO>();
            CreateMap<StrategicObjective, CAStrategicObjectiveDivisionalWeightDTO>();
            CreateMap<StrategicObjective, StrategicObjectiveWithCardDTO>();
            CreateMap<CAStrategicObjectiveDTO, StrategicObjective>().ForMember(a => a.ThemeID, s => { s.MapFrom(dt => dt.Theme.ID); })
                .ForPath(dto => dto.Theme, opt => opt.Ignore()); ;

            CreateMap<StrategicObjective, StrategyPerformanceSODTO>()
                 .ForPath(dto => dto.KPIByStatuses, opt => opt.Ignore())
             .ForPath(dto => dto.DivisionalObjectives, opt => opt.Ignore());

            CreateMap<StrategicObjective, StrategicObjectiveDashboardDTO>();

        }

    }
}

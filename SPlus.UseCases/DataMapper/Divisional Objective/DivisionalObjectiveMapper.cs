using AutoMapper;
using SPlus.DTO;
using SPlus.Model.Domain;

namespace SPlus.UseCases
{
    public class DivisionalObjectiveMapper : Profile
    {
        public DivisionalObjectiveMapper()
        {
            CreateMap<DivisionalObjective, CADivisionalObjectiveDTO>();

            CreateMap<CADivisionalObjectiveDTO, DivisionalObjective>()
                .ForMember(a => a.OrgStructureId, s => { s.MapFrom(dt => dt.OrgStructure.ID); })
                .ForMember(a => a.OrgStructure, opt => opt.Ignore())
                .ForMember(a => a.StrategicObjectiveID, s => { s.MapFrom(dt => dt.StrategicObjective.ID); })
                .ForMember(a => a.StrategicObjective, opt => opt.Ignore());


            CreateMap<WeightDTO, DivisionalObjective>();

            CreateMap<DivisionalObjective, CAODivisionalObjectiveKPIDTO>();

            CreateMap<DivisionalObjective, DivisionalObjectiveWithLEDDTO>();

            CreateMap<DivisionalObjective, DivisionalObjectiveWithCardDTO>();


            CreateMap<DivisionalObjective, DivisionalObjectiveDTO>();

        }
    }
}

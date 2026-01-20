using AutoMapper;
using SPlus.DTO;
using SPlus.Model.Domain;

namespace SPlus.UseCases
{
    public class PerspectiveMapper : Profile
    {
        public PerspectiveMapper()
        {
            CreateMap<Perspective, CASingularPerspectiveDTO>();
            CreateMap<Perspective, CAPerspectiveDTO>();
            CreateMap<CAPerspectiveDTO, Perspective>()
                .ForMember(a => a.StrategyID, s => { s.MapFrom(dt => dt.Strategy.ID); })
                .ForPath(dto => dto.Strategy, opt => opt.Ignore());

            CreateMap<Perspective, OperationalDepartmentSectorDTO>();


            CreateMap<Perspective, KPIRelationPerspectiveDTO>();




        }

    }
}

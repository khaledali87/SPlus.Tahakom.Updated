using AutoMapper;
using SPlus.DTO;
using SPlus.Model.Domain;
using System.Collections.Generic;
using System.Linq;

namespace SPlus.UseCases
{
    public class OrgStructureMapper : Profile
    {
        public OrgStructureMapper()
        {
            #region OrgStructure
            CreateMap<OrgStructure, OrgStructureDTO>()
                .ForPath(a => a.Years, s => { s.MapFrom(dt => !string.IsNullOrEmpty(dt.YearString) ? dt.YearString.Split(',').ToList() : new List<string>()); });
            CreateMap<OrgStructure, CAOrgStructureWeightDTO>();
            CreateMap<OrgStructureDTO, OrgStructure>()
                .ForPath(a => a.GroupID, s => { s.MapFrom(dt => dt.Group.ID); })
                .ForPath(a => a.Manager, s => { s.MapFrom(dt => dt.ManagerModel.UserName); })
                .ForPath(a => a.YearString, s => { s.MapFrom(dt => dt.Years != null && dt.Years.Any() ? string.Join(",", dt.Years) : ""); })
                .ForPath(dto => dto.Group, opt => opt.Ignore())
                .ForPath(dto => dto.ManagerModel, opt => opt.Ignore());

            CreateMap<WeightDTO, OrgStructure>();

            CreateMap<OrgStructure, CADepartmentKPIsDTO>();
            CreateMap<OrgStructure, CADivisionKPIsDTO>()
                .ForMember(a => a.Departments, s => { s.MapFrom(dt => dt.Childrens); });

            CreateMap<Strategy, OperationalStrategyDTO>();
            CreateMap<OrgStructure, OperationalStrategyDivisionDTO>()
               .ForMember(a => a.Departments, s => { s.MapFrom(dt => dt.Childrens); });
            CreateMap<OrgStructure, OperationalStrategyDepartmentDTO>();

            CreateMap<OrgStructure, OperationalDetailsDTO>();



            CreateMap<OrgStructure, CAOrgStructureDivisionalObjectiveDTO>();

            CreateMap<OrgStructure, CAOrgWeightDTO>();
            CreateMap<DivisionalObjective, CADivisionalObjectiveWeightDTO>();

            CreateMap<DivisionalObjective, ObjectiveWeightDTO>();

            CreateMap<OrgStructure, OrgStructureMapDTO>();
            CreateMap<OrgStructure, OrgStructureWeightDTO>();


            CreateMap<OrgStructure, OrgStructureCardDTO>()
            .ForPath(a => a.Departments, s => { s.MapFrom(dt => dt.Childrens); });



            CreateMap<OrgStructure, DepartmentOrgStructureCardDTO>()
           .ForPath(a => a.KPIs, s => { s.MapFrom(dt => dt.DivisionalObjective.SelectMany(k => k.KPIs)); });

            CreateMap<OrgStructure, DepartmentOrgStructureCardDTO>()
        .ForPath(a => a.KPIs, s => { s.MapFrom(dt => dt.KPIList); });
            ;

            CreateMap<OrgStructure, BalanceScoreDepratmentDetailsDTO>()
            .ForPath(a => a.KPIs, s => { s.MapFrom(dt => dt.DivisionalObjective.SelectMany(k => k.KPIs)); });
            CreateMap<OrgStructure, BalanceScoreDepratmentDetailsDTO>()
           .ForPath(a => a.KPIs, s => { s.MapFrom(dt => dt.KPIList); });

            #endregion
        }
    }
}

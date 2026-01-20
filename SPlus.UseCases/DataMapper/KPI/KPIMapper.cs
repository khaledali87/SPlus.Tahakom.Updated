using AutoMapper;
using SPlus.DTO;
using SPlus.Model.Domain;

namespace SPlus.UseCases
{
    public class KPIMapper : Profile
    {
        public KPIMapper()
        {
            CreateMap<KPI, CAKPIListingDTO>();
            CreateMap<KPI, CAKPIWeightDTO>();

            #region Create KPI

            CreateMap<KPI, CAKPIDTO>()
               .ForPath(a => a.KPIType, s => { s.MapFrom(dt => dt.KPIType); })
               .ForPath(a => a.ChampionModel, s => { s.MapFrom(dt => dt.ChampionModel ?? new User()); })
               .ForPath(a => a.OwnerModel, s => { s.MapFrom(dt => dt.OwnerModel ?? new User()); });

            CreateMap<CAKPIDTO, KPI>()
                .ForPath(a => a.KPITypeID, s => { s.MapFrom(dt => dt.KPIType.KPITypeID); })
                .ForPath(a => a.Champion, s => { s.MapFrom(dt => dt.ChampionModel.UserName); })
                .ForPath(a => a.Owner, s => { s.MapFrom(dt => dt.OwnerModel.UserName); })
                .ForPath(dto => dto.StrategicObjective, opt => opt.Ignore())
                .ForPath(dto => dto.DivisionalObjective, opt => opt.Ignore())
                .ForPath(a => a.StrategicObjectiveID, s => { s.MapFrom(dt => dt.StrategicObjective.ID); })
                .ForPath(a => a.DivisionalObjectiveID, s => { s.MapFrom(dt => dt.DivisionalObjective.ID); })
                      .ForPath(a => a.OrgStructureID, s => { s.MapFrom(dt => dt.OrgStructure.ID); })
                          .ForPath(dto => dto.OrgStructure, opt => opt.Ignore())
                .ForPath(dto => dto.Perspective, opt => opt.Ignore())
                .ForPath(dto => dto.KPIType, opt => opt.Ignore())
                .ForPath(dto => dto.ChampionModel, opt => opt.Ignore())
                .ForPath(dto => dto.OwnerModel, opt => opt.Ignore());



            CreateMap<KPI, CreateKPIFormDTO>()
              .ForPath(a => a.KPIType, s => { s.MapFrom(dt => dt.KPIType); })
              .ForPath(a => a.ChampionModel, s => { s.MapFrom(dt => dt.ChampionModel ?? new User()); })
              .ForPath(a => a.OwnerModel, s => { s.MapFrom(dt => dt.OwnerModel ?? new User()); });

            CreateMap<CreateKPIFormDTO, KPI>()
                .ForPath(a => a.KPITypeID, s => { s.MapFrom(dt => dt.KPIType.KPITypeID); })
                .ForPath(a => a.Champion, s => { s.MapFrom(dt => dt.ChampionModel.UserName); })
                .ForPath(a => a.Owner, s => { s.MapFrom(dt => dt.OwnerModel.UserName); })
                .ForPath(dto => dto.StrategicObjective, opt => opt.Ignore())
                .ForPath(dto => dto.DivisionalObjective, opt => opt.Ignore())
                .ForPath(a => a.StrategicObjectiveID, s => { s.MapFrom(dt => dt.StrategicObjective.ID); })
                .ForPath(a => a.DivisionalObjectiveID, s => { s.MapFrom(dt => dt.DivisionalObjective.ID); })
                      .ForPath(a => a.OrgStructureID, s => { s.MapFrom(dt => dt.OrgStructure.ID); })
                          .ForPath(dto => dto.OrgStructure, opt => opt.Ignore())
                .ForPath(dto => dto.Perspective, opt => opt.Ignore())
                .ForPath(dto => dto.KPIType, opt => opt.Ignore())
                .ForPath(dto => dto.ChampionModel, opt => opt.Ignore())
                .ForPath(dto => dto.OwnerModel, opt => opt.Ignore());



            #endregion

            #region Read KPI 

            CreateMap<KPIAffect, KPIAffectDTO>();
            CreateMap<KPIAffectDTO, KPIAffect>();


            CreateMap<KPI, KPIDetailsDTO>()
                .ForPath(a => a.KPIType, s => { s.MapFrom(dt => dt.KPIType); })
                .ForPath(a => a.ChampionModel, s => { s.MapFrom(dt => dt.ChampionModel ?? new User()); })
                .ForPath(a => a.OwnerModel, s => { s.MapFrom(dt => dt.OwnerModel ?? new User()); });

            CreateMap<KPIDetailsDTO, KPI>()
                .ForPath(a => a.KPITypeID, s => { s.MapFrom(dt => dt.KPIType.KPITypeID); })
                .ForPath(a => a.StrategicObjectiveID, s => { s.MapFrom(dt => dt.StrategicObjective.ID); })
                .ForPath(a => a.DivisionalObjectiveID, s => { s.MapFrom(dt => dt.DivisionalObjective.ID); })
                    .ForPath(a => a.OrgStructureID, s => { s.MapFrom(dt => dt.OrgStructure.ID); })
                .ForPath(a => a.Champion, s => { s.MapFrom(dt => dt.ChampionModel.UserName); })
                .ForPath(a => a.Owner, s => { s.MapFrom(dt => dt.OwnerModel.UserName); })
                .ForPath(dto => dto.StrategicObjective, opt => opt.Ignore())
                 .ForPath(dto => dto.DivisionalObjective, opt => opt.Ignore())
                .ForPath(dto => dto.KPIType, opt => opt.Ignore())
                .ForPath(dto => dto.ChampionModel, opt => opt.Ignore())
                .ForPath(dto => dto.OwnerModel, opt => opt.Ignore())
                     .ForPath(dto => dto.OrgStructure, opt => opt.Ignore())
                ;

            CreateMap<KPI, KPICardDTO>();
            CreateMap<KPI, KPILEDDTO>();

            CreateMap<WeightDTO, KPIsWeightDTO>();
            CreateMap<KPIsWeightDTO, WeightDTO>();


            CreateMap<KPIChangeRequestFormDTO, KPI>()

                .ForPath(a => a.Champion, s => { s.MapFrom(dt => dt.ChampionModel.UserName); })
                .ForPath(a => a.Owner, s => { s.MapFrom(dt => dt.OwnerModel.UserName); })
                .ForPath(dto => dto.StrategicObjective, opt => opt.Ignore())

                .ForPath(dto => dto.KPIType, opt => opt.Ignore())
                .ForPath(dto => dto.ChampionModel, opt => opt.Ignore())
                .ForPath(dto => dto.OwnerModel, opt => opt.Ignore())
              ;



            CreateMap<KPI, KPIReleationKPIDTO>();

            #endregion

        }

    }
}

using AutoMapper;
using SPlus.DTO;
using SPlus.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using SPlus.Helper;
namespace SPlus.UseCases
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Attachement
            CreateMap<Attachment, AttachmentDTO>();
            CreateMap<Attachment, KPIAttachmentDTO>();
            CreateMap<Attachment, ActivityAttachmentDTO>();
            CreateMap<AttachmentDTO, Attachment>();
            #endregion 

            #region Lookup
            CreateMap<LookupValue, LookupValueDTO>();
            CreateMap<LookupValueDTO, LookupValue>();

            CreateMap<Lookup, LookupDTO>()
                  .ForMember(a => a.Name, s => { s.MapFrom(dt => dt.Title); })
                  .ForMember(a => a.Items, s => { s.MapFrom(dt => dt.LookupValues); });
            CreateMap<LookupDTO, Lookup>()
                  .ForMember(a => a.Title, s => { s.MapFrom(dt => dt.Name); })
                  .ForMember(a => a.LookupValues, s => { s.MapFrom(dt => dt.Items); });

            #endregion


            CreateMap<Request, RequestDTO>()
                .ForPath(a => a.RequestedBy, s => { s.MapFrom(dt => dt.CreatedBy); }); ;

            CreateMap<RequestStep, RequestStepDTO>()
                .ForPath(a => a.ActionByModel, s => { s.MapFrom(dt => dt.ActionByModel); });

            CreateMap<Request, RequestDetailsDTO>()
           .ForPath(a => a.Form, s => { s.MapFrom(dt => dt.Form); })
           .ForPath(a => a.Steps, s => { s.MapFrom(dt => dt.RequestSteps); });

            #region Workflow

            CreateMap<Workflow, SingularWorkflowDTO>();
            CreateMap<SingularWorkflowDTO, Workflow>();

            CreateMap<WorkflowStep, WorkflowStepDTO>();
            CreateMap<WorkflowStepDTO, WorkflowStep>();

            CreateMap<Workflow, WorkflowDTO>();
            CreateMap<WorkflowDTO, Workflow>();

            #endregion

            #region KPI Type
            CreateMap<ReminderConfiguration, ReminderConfigurationDTO>();
            CreateMap<ReminderConfigurationDTO, ReminderConfiguration>();

            CreateMap<Status, StatusDTO>()
                  .ForMember(a => a.Max, s => { s.MapFrom(dt => dt.KPIThresholds.Select(y => y.Max).FirstOrDefault()); })
                  .ForMember(a => a.Min, s => { s.MapFrom(dt => dt.KPIThresholds.Select(y => y.Min).FirstOrDefault()); })
                  .ForMember(a => a.MaxOperator, s => { s.MapFrom(dt => dt.KPIThresholds.Select(y => y.Operator).FirstOrDefault()); })
                  .ForMember(a => a.MinOperator, s => { s.MapFrom(dt => dt.KPIThresholds.Select(y => y.MinOperator).FirstOrDefault()); });

            CreateMap<StatusDTO, Status>();

            CreateMap<StatusDTO, KPIThreshold>()
                .ForMember(a => a.Max, s => { s.MapFrom(dt => dt.Max); })
                .ForMember(a => a.Min, s => { s.MapFrom(dt => dt.Min); })
                .ForMember(a => a.MinOperator, s => { s.MapFrom(dt => dt.MinOperator); })
                .ForMember(a => a.Operator, s => { s.MapFrom(dt => dt.MaxOperator); })
                .ForMember(a => a.Code, s => { s.MapFrom(dt => dt.Code); });

            CreateMap<KPIThreshold, StatusDTO>()
                .ForMember(a => a.Max, s => { s.MapFrom(dt => dt.Max); })
                .ForMember(a => a.Min, s => { s.MapFrom(dt => dt.Min); })
                .ForMember(a => a.MinOperator, s => { s.MapFrom(dt => dt.MinOperator); })
                .ForMember(a => a.MaxOperator, s => { s.MapFrom(dt => dt.Operator); })
                .ForMember(a => a.Code, s => { s.MapFrom(dt => dt.Code); });

            CreateMap<KPIType, SingularKPITypeDTO>()
              .ForMember(a => a.Status, s => { s.MapFrom(dt => dt.KPIThresholds.Select(y => y.Status)); });
            CreateMap<SingularKPITypeDTO, KPIType>()
                .ForMember(a => a.KPIThresholds, s => { s.MapFrom(dt => dt.Status); });

            CreateMap<KPIType, KPITypeDTO>()
                .ForMember(a => a.Status, s => { s.MapFrom(dt => dt.KPIThresholds.Select(y => y.Status)); });
            CreateMap<KPITypeDTO, KPIType>()
                .ForMember(a => a.KPIThresholds, s => { s.MapFrom(dt => dt.Status); })
                .ForMember(a => a.ReminderConfiguration, s => { s.MapFrom(dt => dt.ReminderConfiguration); });

            CreateMap<Status, ListingStatusDTO>();
            #endregion

            #region System Performance Threshold

            CreateMap<SystemPerformanceThreshold, SystemPerformanceThresholdDTO>()
               .ForMember(a => a.ArabicName, s => { s.MapFrom(dt => dt.Status.ArabicName); })
               .ForMember(a => a.EnglishName, s => { s.MapFrom(dt => dt.Status.EnglishName); })
               .ForMember(a => a.Color, s => { s.MapFrom(dt => dt.Status.Color); })
               .ForMember(a => a.Order, s => { s.MapFrom(dt => dt.Status.Order); });

            CreateMap<SystemPerformanceThresholdDTO, SystemPerformanceThreshold>();
            CreateMap<UpdateSystemPerformanceThresholdDTO, SystemPerformanceThreshold>();

            CreateMap<Status, SystemPerformanceThresholdDTO>()
               .ForMember(a => a.ArabicName, s => { s.MapFrom(dt => dt.ArabicName); })
               .ForMember(a => a.EnglishName, s => { s.MapFrom(dt => dt.EnglishName); })
               .ForMember(a => a.Color, s => { s.MapFrom(dt => dt.Color); })
               .ForMember(a => a.Order, s => { s.MapFrom(dt => dt.Order); })
               .ForPath(a => a.Order, s => { s.MapFrom(dt => dt.Order); });

            #endregion

            #region Notification
            CreateMap<NotificationParameter, NotificationParameterDTO>();
            CreateMap<NotificationParameterDTO, NotificationParameter>();


            CreateMap<NotificationConfiguration, NotificationConfigurationDTO>();
            CreateMap<NotificationConfigurationDTO, NotificationConfiguration>();

            CreateMap<Notification, NotificationDTO>();
            CreateMap<NotificationDTO, Notification>();

            #endregion

            #region Permission

            CreateMap<Role, RoleDTO>()
                .ForMember(a => a.Role, s => { s.MapFrom(dt => dt.Role1); });
            CreateMap<RoleDTO, Role>()
               .ForMember(a => a.Role1, s => { s.MapFrom(dt => dt.Role); });

            CreateMap<MatrixDTO, Matrix>()
                .ForMember(a => a.ResourceID, s => { s.MapFrom(dt => dt.Resource.ID); })
                .ForMember(a => a.RoleID, s => { s.MapFrom(dt => dt.Role.ID); })
                .ForPath(dto => dto.Resource, opt => opt.Ignore())
                .ForPath(dto => dto.Role, opt => opt.Ignore());

            CreateMap<Resource, ResourceDTO>();
            CreateMap<ResourceDTO, Resource>();

            CreateMap<Matrix, MatrixDTO>();

            #endregion

            #region Users And Groups

            CreateMap<Group, GroupDTO>();
            CreateMap<GroupDTO, Group>();

            CreateMap<Group, GroupListDTO>();

            CreateMap<Group, SingularGroupDTO>()
                .ForMember(a => a.Users, s => { s.MapFrom(dt => dt.UsersGroups.Select(y => y.User).ToList()); });

            CreateMap<SingularGroupDTO, Group>()
             .ForMember(a => a.UsersGroups, s => { s.MapFrom(dt => dt.Users); })
                .AfterMap((a, s) =>
                {
                    foreach (var src in a.Users)
                    {
                        foreach (var dst in s.UsersGroups.Where(w => w.UserName == src.UserName))
                        {
                            dst.UserName = src.UserName;
                            dst.GroupID = a.ID;

                        }
                    }
                });

            CreateMap<User, UserListDTO>();

            CreateMap<User, SingularUserDTO>();

            CreateMap<SingularUserDTO, User>();

            CreateMap<User, UserDTO>()
                .ForMember(a => a.Groups, s => { s.MapFrom(dt => dt.UsersGroups.Select(y => y.Group).ToList()); });

            CreateMap<UserDTO, User>()
                .ForMember(a => a.UsersGroups, s => { s.MapFrom(dt => dt.Groups ?? new List<GroupDTO>()); })
                .AfterMap((a, s) =>
                {
                    foreach (var src in a.Groups ?? new List<GroupDTO>())
                    {
                        foreach (var dst in s.UsersGroups.Where(w => w.GroupID == src.ID))
                        {
                            dst.GroupID = src.ID;
                            dst.UserName = a.UserName;

                        }
                    }
                });

            CreateMap<UsersGroup, UserDTO>()
                .ForMember(a => a.UserName, s => { s.MapFrom(dt => dt.UserName); });

            CreateMap<UserDTO, UsersGroup>()
                .ForMember(a => a.UserName, s => { s.MapFrom(dt => dt.UserName); });

            CreateMap<UsersGroup, GroupDTO>()
              .ForMember(a => a.ID, s => { s.MapFrom(dt => dt.GroupID); });


            CreateMap<SingularGroupDTO, UsersGroup>()
             .ForMember(a => a.GroupID, s => { s.MapFrom(dt => dt.ID); })
             .ForMember(a => a.ID, opt => opt.Ignore())
             .ForPath(dto => dto.Group, opt => opt.Ignore())
             .ForPath(dto => dto.User, opt => opt.Ignore());

            CreateMap<SingularUserDTO, UsersGroup>()
                .ForMember(a => a.UserName, s => { s.MapFrom(dt => dt.UserName); })
                .ForMember(a => a.ID, opt => opt.Ignore())
                .ForPath(dto => dto.Group, opt => opt.Ignore())
                .ForPath(dto => dto.User, opt => opt.Ignore());

            CreateMap<GroupDTO, UsersGroup>()
                .ForMember(a => a.GroupID, s => { s.MapFrom(dt => dt.ID); })
                .ForMember(a => a.ID, opt => opt.Ignore())
                .ForPath(dto => dto.Group, opt => opt.Ignore())
                .ForPath(dto => dto.User, opt => opt.Ignore());


            CreateMap<UserDTO, UsersGroup>()
                .ForMember(a => a.UserName, s => { s.MapFrom(dt => dt.UserName); })
                .ForMember(a => a.ID, opt => opt.Ignore())
                .ForPath(dto => dto.Group, opt => opt.Ignore())
                .ForPath(dto => dto.User, opt => opt.Ignore());

            #endregion

            #region Delegation 

            CreateMap<Delegation, DelegationDTO>()
                .ForPath(a => a.CreatedBy, s => { s.MapFrom(dt => dt.CreatedByModel); })
                .ForPath(a => a.ModifiedBy, s => { s.MapFrom(dt => dt.ModifiedByModel); })
                .ForPath(a => a.ToUser, s => { s.MapFrom(dt => dt.DelegatedUser); })
                .ForPath(a => a.FromUser, s => { s.MapFrom(dt => dt.DelegatorUser); });

            CreateMap<DelegationDTO, Delegation>()
                .ForPath(a => a.CreatedBy, s => { s.MapFrom(dt => dt.CreatedBy.UserName); })
                .ForPath(a => a.ModifiedBy, s => { s.MapFrom(dt => dt.ModifiedBy.UserName); })
                .ForPath(a => a.ToUser, s => { s.MapFrom(dt => dt.ToUser.UserName); })
                .ForPath(a => a.FromUser, s => { s.MapFrom(dt => dt.FromUser.UserName); });

            CreateMap<Delegation, ActiveDelegationDTO>()
                .ForPath(a => a.FromUser, s => { s.MapFrom(dt => dt.DelegatorUser); });

            #endregion

            #region Parameter

            CreateMap<Parameter, ParameterDTO>();
            CreateMap<ParameterDTO, Parameter>();

            #endregion

            #region KPI

            CreateMap<KPIComment, KPICommentDTO>();
            CreateMap<KPICommentDTO, KPIComment>();

            //CreateMap<KPICommentPostDTO, KPIComment>();

            CreateMap<KPIMeasure, KPIMeasureDTO>();
            CreateMap<KPIMeasureDTO, KPIMeasure>();

            CreateMap<KPI, SimpleKPIDTO>();
            CreateMap<KPI, KPIWeightDTO>();


            CreateMap<KPI, KPIDTO>()
                .ForMember(a => a.Years, s => { s.MapFrom(dt => dt.Years); })
                .ForPath(a => a.KPIType, s => { s.MapFrom(dt => dt.KPIType ?? new KPIType()); })
                .ForPath(a => a.ChampionModel, s => { s.MapFrom(dt => dt.ChampionModel ?? new User()); })
                .ForPath(a => a.OwnerModel, s => { s.MapFrom(dt => dt.OwnerModel ?? new User()); });

            CreateMap<KPIDTO, KPI>()
                .ForMember(a => a.Years, s => { s.MapFrom(dt => dt.Years); })
                .ForPath(a => a.Champion, s => { s.MapFrom(dt => dt.ChampionModel.UserName); })
                .ForPath(a => a.Owner, s => { s.MapFrom(dt => dt.OwnerModel.UserName); });

            CreateMap<KPI, SingularKPIDTO>()
                .ForPath(a => a.KPIType, s => { s.MapFrom(dt => dt.KPIType); })
               .ForPath(a => a.ChampionModel, s => { s.MapFrom(dt => dt.ChampionModel ?? new User()); })
               .ForPath(a => a.OwnerModel, s => { s.MapFrom(dt => dt.OwnerModel ?? new User()); });
            ;

            CreateMap<SingularKPIDTO, KPI>()
                .ForPath(a => a.KPITypeID, s => { s.MapFrom(dt => dt.KPIType.KPITypeID); })
                .ForPath(a => a.StrategicObjectiveID, s => { s.MapFrom(dt => dt.StrategicObjective.ID); })

                .ForPath(a => a.Champion, s => { s.MapFrom(dt => dt.ChampionModel.UserName); })
                .ForPath(a => a.Owner, s => { s.MapFrom(dt => dt.OwnerModel.UserName); })
                .ForPath(dto => dto.StrategicObjective, opt => opt.Ignore())
                .ForPath(dto => dto.KPIType, opt => opt.Ignore())
                .ForPath(dto => dto.ChampionModel, opt => opt.Ignore())
                .ForPath(dto => dto.OwnerModel, opt => opt.Ignore());

            CreateMap<UpdateKPIDTO, KPI>()
                .ForPath(a => a.StrategicObjectiveID, s => { s.MapFrom(dt => dt.StrategicObjective.ID); })
                 .ForPath(a => a.DivisionalObjectiveID, s => { s.MapFrom(dt => dt.DivisionalObjective.ID); })
                .ForPath(a => a.Champion, s => { s.MapFrom(dt => dt.ChampionModel.UserName); })
                .ForPath(a => a.Owner, s => { s.MapFrom(dt => dt.OwnerModel.UserName); })
                .ForPath(dto => dto.StrategicObjective, opt => opt.Ignore())
                .ForPath(dto => dto.KPIType, opt => opt.Ignore())
                .ForPath(dto => dto.ChampionModel, opt => opt.Ignore())
                .ForPath(dto => dto.OwnerModel, opt => opt.Ignore());

            CreateMap<KPI, UpdateKPIDTO>()
                .ForPath(a => a.ChampionModel, s => { s.MapFrom(dt => dt.ChampionModel ?? new User()); })
                .ForPath(a => a.OwnerModel, s => { s.MapFrom(dt => dt.OwnerModel ?? new User()); });


            CreateMap<KPI, KPIUpdateWeightDTO>();
            CreateMap<KPIUpdateWeightDTO, KPI>();
            CreateMap<WeightDTO, KPI>();
            CreateMap<KPI, DashboardKPIDetailsDTO>()
                .ForPath(a => a.ChampionModel, s => { s.MapFrom(dt => dt.ChampionModel ?? new User()); })
                .ForPath(a => a.OwnerModel, s => { s.MapFrom(dt => dt.OwnerModel ?? new User()); });
            #endregion

            #region Strategic Objective
            //CreateMap<StrategicObjective, StrategicObjectiveDTO>();
            //CreateMap<StrategicObjectiveDTO, StrategicObjective>();




            //CreateMap<StrategicObjective, StrategicObjectiveListDTO>();
            //CreateMap<StrategicObjectiveListDTO, StrategicObjective>();

            //CreateMap<StrategicObjective, UpdateStrategicObjectiveDTO>();
            //CreateMap<StrategicObjective, WeightDTO>();


            //CreateMap<UpdateStrategicObjectiveDTO, StrategicObjective>()
            //    .ForPath(a => a.PerspectiveID, s => { s.MapFrom(dt => dt.Perspective.ID); })
            //    .ForPath(a => a.ThemeID, s => { s.MapFrom(dt => dt.Theme.ID); })
            //    .ForPath(dto => dto.Perspective, opt => opt.Ignore())
            //    .ForPath(dto => dto.Theme, opt => opt.Ignore());

            CreateMap<StrategicObjective, SingularStrategicObjectiveDTO>();

            //CreateMap<SingularStrategicObjectiveDTO, StrategicObjective>()
            //    .ForPath(a => a.PerspectiveID, s => { s.MapFrom(dt => dt.Perspective.ID); })
            //    .ForPath(a => a.ThemeID, s => { s.MapFrom(dt => dt.Theme.ID); })
            //    .ForPath(dto => dto.Perspective, opt => opt.Ignore())
            //    .ForPath(dto => dto.Theme, opt => opt.Ignore());

            CreateMap<WeightDTO, StrategicObjective>();
            CreateMap<StrategicObjective, SimpleStrategicObjectiveDTO>();
            CreateMap<StrategicObjective, StrategicObjectiveWeightDTO>();
            #endregion

            #region Theme
            CreateMap<Theme, ThemeDTO>();
            CreateMap<ThemeDTO, Theme>();

            CreateMap<Theme, ThemeListDTO>();
            CreateMap<ThemeListDTO, Theme>();

            CreateMap<Theme, SingularThemeDTO>();
            CreateMap<SingularThemeDTO, Theme>();

            CreateMap<Theme, ThemeWeightDTO>();
            CreateMap<WeightDTO, Theme>();

            #endregion

            #region BalanceScoreCard

            CreateMap<Theme, BalanceScoreCardTheme>();
            CreateMap<BalanceScoreCardTheme, Theme>();

            //CreateMap<Perspective, BalanceScoreCardPerspective>();
            //CreateMap<BalanceScoreCardPerspective, Perspective>();

            CreateMap<StrategicObjective, BalanceScoreCardStrategicObjective>();
            CreateMap<BalanceScoreCardStrategicObjective, StrategicObjective>();

            CreateMap<BalanceScoreCardKPIListDTO, KPI>();
            CreateMap<KPI, BalanceScoreCardKPIListDTO>();


            #endregion

            #region Projects
            CreateMap<Project, OperationalLevelDTO>()
                .ForMember(a => a.ManagerModel, s => { s.MapFrom(dt => dt.Manager); });

            #endregion


        }

    }
}

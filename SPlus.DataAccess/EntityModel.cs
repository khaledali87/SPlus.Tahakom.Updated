using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using SPlus.Model.Domain;
using System.Configuration;

namespace SPlus.DataAccess
{
    public partial class EntityModel : DbContext
    {
        public EntityModel()
            : base(ConfigurationManager.AppSettings["ConnectionString"].ToString())
        {
            Configuration.ProxyCreationEnabled = false;

        }

        #region DBSets
        public virtual DbSet<Attachment> Attachements { get; set; }
        public virtual DbSet<Holiday> Holidays { get; set; }
        public virtual DbSet<AuditTrail> AuditTrails { get; set; }
        public virtual DbSet<Model.Domain.Configuration> Configurations { get; set; }
        public virtual DbSet<Delegation> Delegations { get; set; }
        public virtual DbSet<EmailConfiguration> EmailConfigurations { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Strategy> Stratgies { get; set; }
        public virtual DbSet<KPI> KPIs { get; set; }
        public virtual DbSet<KPIComment> KPIComments { get; set; }
        public virtual DbSet<KPIMeasure> KPIMeasures { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<KPIThreshold> KPIThresholds { get; set; }
        public virtual DbSet<KPIType> KPITypes { get; set; }
        public virtual DbSet<Lookup> Lookups { get; set; }
        public virtual DbSet<LookupValue> LookupValues { get; set; }
        public virtual DbSet<Matrix> Matrices { get; set; }
        public virtual DbSet<NotificationConfiguration> NotificationConfigurations { get; set; }
        public virtual DbSet<NotificationParameter> NotificationParameters { get; set; }
        public virtual DbSet<NotificationReceiver> NotificationReceivers { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<OrgStructure> OrgStructures { get; set; }
        public virtual DbSet<ReminderConfiguration> ReminderConfigurations { get; set; }
        public virtual DbSet<Resource> Resources { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<StrategicObjective> StrategicObjectives { get; set; }
        public virtual DbSet<DivisionalObjective> DivisionalObjective { get; set; }
        public virtual DbSet<SYNC_Job_Logging> SYNC_Job_Logging { get; set; }
        public virtual DbSet<Theme> Themes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersGroup> UsersGroups { get; set; }
        public virtual DbSet<WFReminderRegistry> WFReminderRegistries { get; set; }
        public virtual DbSet<WFRequest> WFRequests { get; set; }
        public virtual DbSet<WFRequestStep> WFRequestSteps { get; set; }
        public virtual DbSet<Workflow> Workflows { get; set; }
        public virtual DbSet<WorkflowsSnapshot> WorkflowsSnapshots { get; set; }
        public virtual DbSet<WorkflowStep> WorkflowSteps { get; set; }
        public virtual DbSet<WorkflowStepsSnapshot> WorkflowStepsSnapshots { get; set; }
        public virtual DbSet<WorkflowTask> WorkflowTasks { get; set; }
        public virtual DbSet<SystemPerformanceThreshold> SystemPerformanceThresholds { get; set; }
        public virtual DbSet<BaseWorkflow> BaseWorkflow { get; set; }
        public virtual DbSet<AttachmentContent> AttachmentContents { get; set; }
        public virtual DbSet<WFFormUpdateKPI> WFFormUpdateKPIs { get; set; }
        public virtual DbSet<Parameter> Parameters { get; set; }
        public virtual DbSet<ParameterValue> ParameterValues { get; set; }
        public virtual DbSet<Request> Request { get; set; }
        public virtual DbSet<RequestStep> RequestSteps { get; set; }
        public virtual DbSet<KPIHistory> KPIHistories { get; set; }
        public virtual DbSet<KPIAffect> KPIAffects { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<TwoFactorAuth> TwoFactorAuth { get; set; }
        public virtual DbSet<UserActivity> UserActivities { get; set; }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {


            #region LookUp

            modelBuilder.Entity<Lookup>()
            .HasMany(e => e.LookupValues)
            .WithOptional(e => e.Lookup)
            .HasForeignKey(e => e.LookupID);

            #endregion

            #region Notification Configuration

            modelBuilder.Entity<NotificationConfiguration>()
            .HasMany(e => e.NotificationReceivers)
            .WithRequired(e => e.NotificationConfiguration)
            .HasForeignKey(e => e.TemplateID)
            .WillCascadeOnDelete(true);

            modelBuilder.Entity<NotificationConfiguration>()
                .HasMany(e => e.NotificationParameters)
                .WithRequired(e => e.NotificationConfiguration)
                .HasForeignKey(e => e.TemplateID)
                .WillCascadeOnDelete(true);

            #endregion

            #region Attachment
            modelBuilder.Entity<Attachment>()
                .HasRequired(a => a.Content)
                .WithRequiredPrincipal(a => a.Attachment)
                .Map(m => m.MapKey())
                .WillCascadeOnDelete(true);

            #endregion

            #region Notification

            #endregion

            #region Permission
            modelBuilder.Entity<Group>()
                .HasMany(a => a.Matrices)
                .WithRequired(a => a.Group)
                .HasForeignKey(a => a.GroupID)
                .WillCascadeOnDelete(true);
            #endregion

            #region User/Group


            modelBuilder.Entity<User>()
             .HasMany(e => e.ChampionKPIs)
             .WithRequired(e => e.ChampionModel)
             .HasForeignKey(e => e.Champion)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.OwnerKPIs)
                .WithRequired(e => e.OwnerModel)
                .HasForeignKey(e => e.Owner)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UsersGroups)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Group>()
              .HasMany(e => e.UsersGroups)
              .WithRequired(e => e.Group)
              .WillCascadeOnDelete(true);

            modelBuilder.Entity<User>()
            .HasMany(e => e.Delegated)
            .WithRequired(e => e.DelegatedUser)
            .HasForeignKey(e => e.ToUser)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
              .HasMany(e => e.Delegatees)
              .WithRequired(e => e.DelegatorUser)
              .HasForeignKey(e => e.FromUser)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
              .HasMany(e => e.CreatedBy)
              .WithRequired(e => e.CreatedByModel)
              .HasForeignKey(e => e.CreatedBy)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
              .HasMany(e => e.ModifiedBy)
              .WithRequired(e => e.ModifiedByModel)
              .HasForeignKey(e => e.ModifiedBy)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
              .HasMany(e => e.WFRequestSteps)
              .WithOptional(e => e.ActionByModel)
              .HasForeignKey(e => e.ActionBy)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
              .HasMany(e => e.RequestSteps)
              .WithOptional(e => e.ActionByModel)
              .HasForeignKey(e => e.ActionBy)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
             .HasMany(e => e.Managers)
             .WithRequired(e => e.ManagerModel)
             .HasForeignKey(e => e.Manager)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<Group>()
             .HasMany(e => e.OrgStructures)
             .WithOptional(e => e.Group)
             .HasForeignKey(e => e.GroupID)
             .WillCascadeOnDelete(false);


            #endregion

            #region Request 

            modelBuilder.Entity<Request>()
            .HasMany(e => e.RequestSteps)
            .WithRequired(e => e.Request)
            .HasForeignKey(e => e.RequestID)
            .WillCascadeOnDelete(true);


            modelBuilder.Entity<WFRequest>()
               .HasMany(e => e.WFRequestSteps)
               .WithRequired(e => e.WFRequest)
               .HasForeignKey(e => e.RequestID)
               .WillCascadeOnDelete(true);

            modelBuilder.Entity<WFRequest>()
                .HasRequired(e => e.WFFormUpdateKPI)
                .WithRequiredPrincipal(e => e.WFRequest)
                .Map(m => m.MapKey())
                .WillCascadeOnDelete(true);

            #endregion

            #region Workflow

            modelBuilder.Entity<Workflow>()
               .HasMany(e => e.WorkflowSteps)
               .WithRequired(e => e.Workflow)
               .HasForeignKey(e => e.WorkflowID)
               .WillCascadeOnDelete(true);

            modelBuilder.Entity<BaseWorkflow>()
               .HasMany(e => e.Workflows)
               .WithRequired(e => e.BaseWorkflow)
               .HasForeignKey(e => e.BaseWorkflowID)
               .WillCascadeOnDelete(true);

            #endregion

            #region KPIType

            modelBuilder.Entity<Status>()
               .HasMany(e => e.KPIThresholds)
               .WithRequired(e => e.Status)
               .HasForeignKey(e => e.Code)
               .WillCascadeOnDelete(true);

            modelBuilder.Entity<KPIType>()
                .HasMany(e => e.KPIThresholds)
                .WithRequired(e => e.KPIType)
                .HasForeignKey(e => e.KPITypeID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Status>()
            .HasRequired(e => e.SystemPerformanceThreshold)
            .WithRequiredPrincipal(e => e.Status)
            .WillCascadeOnDelete(true);

            modelBuilder.Entity<KPIType>()
                .HasMany(e => e.Workflows)
                .WithOptional(e => e.KPIType)
                .HasForeignKey(e => e.KPITypeID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<KPIType>()
                .HasMany(e => e.KPIs)
                .WithRequired(e => e.KPIType)
                .HasForeignKey(e => e.KPITypeID)
                .WillCascadeOnDelete(true);


            modelBuilder.Entity<KPIType>()
                .HasRequired(e => e.ReminderConfiguration)
                .WithRequiredPrincipal(e => e.KPIType)
                .Map(m => m.MapKey())
                .WillCascadeOnDelete(true);

            #endregion

            #region KPIMeasure

            modelBuilder.Entity<KPIMeasure>()
                .Property(e => e.AccumulutiveOutOfTarget)
                .HasPrecision(18, 6);

            modelBuilder.Entity<KPIMeasure>()
                .Property(e => e.AccumulutiveTarget)
                .HasPrecision(18, 6);

            modelBuilder.Entity<KPIMeasure>()
                .Property(e => e.AccumulutiveValue)
                .HasPrecision(18, 6);


            modelBuilder.Entity<KPIMeasure>()
                .Property(e => e.Target)
                .HasPrecision(18, 6);

            modelBuilder.Entity<KPIMeasure>()
                .Property(e => e.Value)
                .HasPrecision(18, 6);

            modelBuilder.Entity<KPIMeasure>()
                .Property(e => e.OutOfTarget)
                .HasPrecision(18, 6);

            modelBuilder.Entity<KPIMeasure>()
                .HasMany(e => e.ParameterValues)
                .WithRequired(e => e.Measure)
                .HasForeignKey(e => e.MeasureID)
                .WillCascadeOnDelete(true);

            #endregion

            #region KPI

            modelBuilder.Entity<KPI>()
              .Property(e => e.Baseline)
              .HasPrecision(18, 6);

            modelBuilder.Entity<KPI>()
                .Property(e => e.Weight)
                .HasPrecision(18, 6);

            modelBuilder.Entity<KPI>()
                .Property(e => e.BusinessUnitWeight)
                .HasPrecision(18, 6);

            modelBuilder.Entity<KPI>()
                .HasMany(e => e.KPIComments)
                .WithRequired(e => e.KPI)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<KPI>()
                .HasMany(e => e.KPIMeasures)
                .WithRequired(e => e.KPI)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<KPI>()
                .HasMany(e => e.Parameters)
                .WithRequired(e => e.KPI)
                .HasForeignKey(e => e.KPIID)
                .WillCascadeOnDelete(true);

            #endregion

            #region Strategic Objective

            modelBuilder.Entity<StrategicObjective>()
               .Property(e => e.Weight)
               .HasPrecision(18, 0);

            modelBuilder.Entity<StrategicObjective>()
                .HasMany(e => e.KPIs)
                .WithOptional(e => e.StrategicObjective)
                .HasForeignKey(e => e.StrategicObjectiveID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<StrategicObjective>()
                .HasMany(e => e.DivisionalObjectives)
                .WithRequired(e => e.StrategicObjective)
                .HasForeignKey(e => e.StrategicObjectiveID)
                .WillCascadeOnDelete(false);




            #endregion

            #region Strategy

            modelBuilder.Entity<Strategy>()
                .HasMany(e => e.Themes)
                .WithRequired(e => e.Strategy)
                .HasForeignKey(e => e.StrategyID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Strategy>()
                .HasMany(e => e.Perspectives)
                .WithRequired(e => e.Strategy)
                .HasForeignKey(e => e.StrategyID)
                .WillCascadeOnDelete(false);

            #endregion

            #region Theme

            modelBuilder.Entity<Theme>()
               .Property(e => e.Weight)
               .HasPrecision(18, 0);

            modelBuilder.Entity<Theme>()
                .HasMany(e => e.StrategicObjectives)
                .WithRequired(e => e.Theme)
                .HasForeignKey(e => e.ThemeID)
                .WillCascadeOnDelete(true);

            #endregion                                                       

            #region Perspective

            modelBuilder.Entity<Perspective>()
                .HasMany(e => e.KPIs)
                .WithOptional(e => e.Perspective)
                .HasForeignKey(e => e.PerspectiveID)
                .WillCascadeOnDelete(false);

            #endregion                                                       

            #region Org Structure

            modelBuilder.Entity<OrgStructure>()
                .HasMany(e => e.DivisionalObjective)
                .WithOptional(e => e.OrgStructure)
                .HasForeignKey(e => e.OrgStructureId)
                .WillCascadeOnDelete(true);



            #endregion

            #region Divisional Objective

            modelBuilder.Entity<DivisionalObjective>()
               .Property(e => e.Weight)
               .HasPrecision(18, 0);

            modelBuilder.Entity<DivisionalObjective>()
                .HasMany(e => e.KPIs)
                .WithOptional(e => e.DivisionalObjective)
                .HasForeignKey(e => e.DivisionalObjectiveID)
                .WillCascadeOnDelete(true);


            #endregion
        }
    }
}

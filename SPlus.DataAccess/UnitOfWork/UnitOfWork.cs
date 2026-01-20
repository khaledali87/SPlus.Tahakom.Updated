using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.Model.Domain;

namespace SPlus.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EntityModel Context;
        public IRepository<KPI> KPI { get; private set; }
        public IRepository<KPIComment> KPIComment { get; private set; }
        public IRepository<StrategicObjective> StrategicObjective { get; private set; }
        public IRepository<DivisionalObjective> DivisionalObjective { get;  set; }
        public IRepository<Theme> Theme { get; private set; }
        public IRepository<Perspective> Perspective { get; private set; }
        public IRepository<Workflow> Workflow { get; private set; }
        public IUserRepository User { get; set; }
        public IRepository<OrgStructure> OrgStructure { get; set; }
        public IRepository<Notification> Notification { get; set; }
        public IRepository<NotificationConfiguration> NotificationConfiguration { get; set; }
        public IRepository<KPIType> KPIType { get; set; }
        public IRepository<Lookup> Handshake { get; set; }
        public IRepository<SystemPerformanceThreshold> SystemPerformanceThreshold { get; set; }
        public IRepository<WorkflowStep> WorkflowStep { get; set; }
        public IRepository<WFFormUpdateKPI> WFFormUpdateKPI { get; set;}
        public IRepository<Delegation> Delegation { get; set; }
        public IRepository<Group> Group { get; set; }
        public IRepository<AuditTrail> AuditTrail { get; set; }
        public IAttachmentRepository Attachment { get; set; }
        public IRepository<KPIMeasure> KPIMeasure { get; set; }
        public IRepository<UsersGroup> UsersGroup { get; set; }
        public IRepository<NotificationReceiver> NotificationReceiver { get; set; }
        public IRepository<Parameter> Parameter { get; set; }
        public IRepository<ParameterValue> ParameterValue { get; set; }
        public IRepository<Resource> Resource { get; set; }
        public IRepository<Configuration> Configuration { get; set; }
        public IRepository<EmailConfiguration> EmailConfiguration { get; set; }
        public IRepository<Status> Status { get; set; }
        public IRepository<ReminderConfiguration> ReminderConfiguration { get; set; }
        public IRepository<Matrix> Matrix { get; set; }
        public IRepository<Request> Request { get; set; }
        public IRepository<Strategy> Strategy { get; set; }
        public IRepository<RequestStep> RequestStep { get; set; }
        public IRepository<WFReminderRegistry> WFReminderRegistry { get; set; }
        public IRepository<KPIThreshold> KPIThreshold { get; set; }
        public IRepository<Session> Session { get; set; }

        public IRepository<KPIHistory> KPIHistory { get; set; }
        public IRepository<KPIAffect> KPIAffect { get; set; }
        public IRepository<TwoFactorAuth> TwoFactorAuth { get; set; }
        public IRepository<UserActivity> UserActivity { get; set; }

        public UnitOfWork()
        {
            Context = new EntityModel();

            KPI = new Repository<KPI>(Context);
            Perspective = new Repository<Perspective>(Context);
            StrategicObjective = new Repository<StrategicObjective>(Context);
            Theme = new Repository<Theme>(Context);
            Workflow = new Repository<Workflow>(Context);
            Request = new Repository<Request>(Context);
            RequestStep = new Repository<RequestStep>(Context);
            KPIComment = new Repository<KPIComment>(Context);
            User = new UserDataAccess(Context);
            OrgStructure = new Repository<OrgStructure>(Context);
            Notification = new Repository<Notification>(Context);
            NotificationConfiguration = new Repository<NotificationConfiguration>(Context);
            KPIType = new Repository<KPIType>(Context);
            Handshake = new Repository<Lookup>(Context);
            SystemPerformanceThreshold = new Repository<SystemPerformanceThreshold>(Context);
            WorkflowStep = new Repository<WorkflowStep>(Context);
            WFFormUpdateKPI = new Repository<WFFormUpdateKPI>(Context);
            Delegation = new Repository<Delegation>(Context);
            Group = new Repository<Group>(Context);          
            AuditTrail = new Repository<AuditTrail>(Context);
            Attachment = new AttachmentDataAccess(Context);
            KPIMeasure = new Repository<KPIMeasure>(Context);
            UsersGroup = new Repository<UsersGroup>(Context);
            NotificationReceiver = new Repository<NotificationReceiver>(Context);
            Parameter = new Repository<Parameter>(Context);
            ParameterValue = new Repository<ParameterValue>(Context);
            Resource = new Repository<Resource>(Context);
            Configuration = new Repository<Configuration>(Context);
            EmailConfiguration = new Repository<EmailConfiguration>(Context);
            Status = new Repository<Status>(Context);
            Matrix = new Repository<Matrix>(Context);
            ReminderConfiguration = new Repository<ReminderConfiguration>(Context);
            Strategy = new Repository<Strategy>(Context);   
            WFReminderRegistry = new Repository<WFReminderRegistry>(Context);
            KPIThreshold = new Repository<KPIThreshold>(Context);
            DivisionalObjective = new Repository<DivisionalObjective>(Context);
            KPIHistory = new Repository<KPIHistory>(Context);
            KPIAffect = new Repository<KPIAffect>(Context);
            Session = new Repository<Session>(Context);
            TwoFactorAuth = new Repository<TwoFactorAuth>(Context);
            UserActivity = new Repository<UserActivity>(Context);
        }
        public int Complete()
        {
            return Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}

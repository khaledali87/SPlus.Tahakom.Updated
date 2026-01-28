using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.Model.Domain;

namespace SPlus.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<KPI> KPI { get; }
        IRepository<Holiday> Holiday { get; }
        IRepository<StrategicObjective> StrategicObjective { get; }

        IRepository<DivisionalObjective> DivisionalObjective { get; }
        IRepository<Theme> Theme { get; }
        IRepository<Perspective> Perspective { get; }
        IRepository<Workflow> Workflow { get; }
        IRepository<Request> Request { get; }
        IRepository<RequestStep> RequestStep { get; }
        IRepository<KPIComment> KPIComment { get; }
        IUserRepository User { get; }
        IRepository<OrgStructure> OrgStructure { get; }
        IRepository<Notification> Notification { get; }
        IRepository<NotificationConfiguration> NotificationConfiguration { get; }
        IRepository<KPIType> KPIType { get; }
        IRepository<KPIThreshold> KPIThreshold { get; }
        IRepository<WorkflowStep> WorkflowStep { get; }
        IRepository<Lookup> Handshake { get; }
        IRepository<SystemPerformanceThreshold> SystemPerformanceThreshold { get; }
        IRepository<WFFormUpdateKPI> WFFormUpdateKPI { get; }
        IRepository<Delegation> Delegation { get; }
        IRepository<Group> Group { get; }
        IRepository<AuditTrail> AuditTrail { get; }
        IAttachmentRepository Attachment { get; }
        IRepository<KPIMeasure> KPIMeasure { get; }
        IRepository<UsersGroup> UsersGroup { get; }
        IRepository<NotificationReceiver> NotificationReceiver { get; }
        IRepository<Parameter> Parameter { get; }
        IRepository<ParameterValue> ParameterValue { get; }
        IRepository<Resource> Resource { get; }
        IRepository<Configuration> Configuration { get; }
        IRepository<EmailConfiguration> EmailConfiguration { get; }
        IRepository<Status> Status { get; }
        IRepository<Matrix> Matrix { get; }
        IRepository<Strategy> Strategy { get; }  
        IRepository<ReminderConfiguration> ReminderConfiguration { get; }
        IRepository<WFReminderRegistry> WFReminderRegistry { get; }
        IRepository<KPIHistory> KPIHistory { get; }
        IRepository<KPIAffect> KPIAffect { get; }
        IRepository<Session> Session { get; }
        IRepository<TwoFactorAuth> TwoFactorAuth { get; }
        IRepository<UserActivity> UserActivity { get; }
        int Complete();


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.DTO;
using StructureMap;
using SPlus.DataAccess;
using SPlus.Model.Domain;
using System.Data.Entity;
using System.Reflection;
using SPlus.Helper;
using model = SPlus.Model;
using System.Net.Mail;
using System.Net;
using Z.EntityFramework.Plus;
using SPlus.Model;
using Newtonsoft.Json;

namespace SPlus.BLL
{
    public class NotificationConfigurationBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();

        private readonly IUnitOfWorkFactory _factory;
        public NotificationConfigurationBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }
      
        #region Read
        public List<NotificationConfiguration> Read()
        {
            using (var dataAccess = _factory.Create())
            {
                List<NotificationConfiguration> notificationConfiguration = dataAccess.NotificationConfiguration.Query()
                    .Include(a => a.NotificationParameters)
                    .Include(a => a.NotificationReceivers)
                    .ToList();
                return notificationConfiguration;
            }
        }

        public NotificationConfiguration ReadByID(int ID)
        {

            using (var dataAccess = _factory.Create())
            {
                return dataAccess.NotificationConfiguration.Query()
                    .Include(a => a.NotificationParameters)
                    .Include(a => a.NotificationReceivers)
                    .Where(s => s.ID == ID).SingleOrDefault();
            }
        }
        public NotificationConfiguration GetSingleNotificationConfiguration(int ID)
        { 

            using (var dataAccess = _factory.Create())
            {
                return dataAccess.NotificationConfiguration.Query()
                    .Include(a => a.NotificationReceivers)
                    .Where(s => s.ID == ID).SingleOrDefault();
            }
        }
        public NotificationConfiguration GetSingle(int ID)
        {
            using (var dataAccess = _factory.Create())
            {
                return dataAccess.NotificationConfiguration.Get(ID);
            }
        }
        #endregion

        #region Update
        public NotificationConfiguration Update(NotificationConfiguration notificationConfiguration)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                var current = dataAccess.NotificationConfiguration.Query().Where(w => w.ID == notificationConfiguration.ID).FirstOrDefault();


                notificationConfiguration.ActionType = current.ActionType;
                #region Receivers Commented
                //List<PropertyInfo> receiversProperties = new List<PropertyInfo>();
                //receiversProperties.Add(typeof(NotificationReceiver).GetProperty("TemplateID"));
                //receiversProperties.Add(typeof(NotificationReceiver).GetProperty("Receiver"));
                //receiversProperties.Add(typeof(NotificationReceiver).GetProperty("IsCC"));

                //List<NotificationReceiver> newreceivers = notificationConfiguration.NotificationReceivers.ToList();
                //context.NotificationReceivers.ContextListMapper(newreceivers, receiversProperties);

                //List<NotificationReceiver> receivers = new List<NotificationReceiver>();
                //foreach (var receiver in notificationConfiguration.NotificationReceivers)
                //{
                //    NotificationReceiver rec = new NotificationReceiver();
                //    rec = context.NotificationReceivers.Where(w => w.ID == receiver.ID).FirstOrDefault();
                //    if (rec != null)
                //        receivers.Add(rec);
                //}
                //var receiverstodelete = context.NotificationReceivers.Where(w => !receivers.Select(s => s.ID).Contains(w.ID)).ToList();
                //foreach (var receiver in receiverstodelete)
                //{
                //    dataAccess.NotificationReceiver.Delete(receiver);
                //}

                //foreach (var receiver in notificationConfiguration.NotificationReceivers)
                //{
                //    if (receiver.ID == 0)
                //        context.NotificationReceivers.Add(receiver);
                //}
                #endregion

                dataAccess.NotificationConfiguration.Save(notificationConfiguration);
                result = dataAccess.Complete();
                return notificationConfiguration;
            }
        }

        #endregion

        private Configuration GetConfiguration(string key)
        {
            using (var dataAccess = _factory.Create())
            {
                return dataAccess.Configuration.Query().Where(w => w.Key == key).FirstOrDefault();
            }
        }
        #region Actions

        public void SendNotificationWorkflow(int LevelID, int MeasureID, int requestID, model.enumNotificationEventType eventType,  LevelTypeEnum Level, string NotificationType = "Reminder")
        {
            using (var dataAccess = _factory.Create())
            {
                List<RequestStep> _RequestSteps = dataAccess.Request.Query().IncludeOptimized(a => a.RequestSteps).SelectMany(a => a.RequestSteps).Where(a => !a.IsCancelled).ToList();
                List<NotificationConfiguration> _Templates = Read();
                NotificationConfiguration _template = null;

                Notification _Notification = new Notification();
                List<Notification> ReturnNotifications = new List<Notification>();
                Notification ReturnNotification;
                EmailTemplate _EmailTemplate = new EmailTemplate();
                if (_Templates.Count > 0)
                {
                    _template = new NotificationConfiguration();
                    _template = _Templates.Where(a => a.ActionType == (int)eventType).FirstOrDefault();
                }
                if (_template != null)
                {
                    if(Level == LevelTypeEnum.KPI )
                        _template = ResolveNotificationsForKPI(LevelID, MeasureID, requestID, _template, _RequestSteps);
                    

                    List<string> CC = new List<string>();
                    foreach (NotificationReceiver reciever in _template.NotificationReceivers.Where(a => a.IsCC))
                    {
                        foreach (User user in reciever.Users)
                        {
                            if (user != null)
                            {
                                CC.Add(user.Email);
                            }
                        }
                    }

                    NotificationConfiguration tempTemplate = null;
                    foreach (NotificationReceiver reciever in _template.NotificationReceivers)
                    {
                        foreach (User user in reciever.Users)
                        {
                            try
                            {
                                tempTemplate = new NotificationConfiguration();
                                //Handle User Parameter
                                if (_template.NotificationEnglish.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.NotificationEnglish = _template.NotificationEnglish.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.NotificationEnglish = _template.NotificationEnglish;

                                if (_template.NotificationArabic.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.NotificationArabic = _template.NotificationArabic.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.NotificationArabic = _template.NotificationArabic;

                                if (_template.EmailSubject.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.EmailSubject = _template.EmailSubject.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.EmailSubject = _template.EmailSubject;

                                if (_template.EmailBody.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.EmailBody = _template.EmailBody.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.EmailBody = _template.EmailBody;

                                if (!reciever.IsCC)
                                {
                                    // Send Push Notification
                                    ReturnNotification = new Notification();
                                    _Notification = new Notification();
                                    _Notification.EnglishName = tempTemplate.NotificationEnglish;
                                    _Notification.ArabicName = tempTemplate.NotificationArabic;
                                    _Notification.NotificationType = NotificationType;
                                    _Notification.Created = DateTime.Now;
                                    _Notification.Modified = DateTime.Now;
                                    _Notification.RelatedItemID = LevelID;
                                    if (Level == LevelTypeEnum.KPI)
                                        _Notification.ItemType = LevelTypeEnum.KPI.ToString();                                                                      

                                    _Notification.Status = (int)model.NotificationEnums.Pending;
                                    _Notification.AssignedTo = user.UserName;
                                    dataAccess.Notification.Save(_Notification);
                                    int result = dataAccess.Complete();
                                    if (result != 0)
                                        ReturnNotifications.Add(_Notification);
                                }
                                // Send Email
                                if (!reciever.IsCC)
                                {
                                    _EmailTemplate = new EmailTemplate();
                                    _EmailTemplate.To = user.Email;
                                    _EmailTemplate.CC = CC;
                                    _EmailTemplate.Body = tempTemplate.EmailBody;
                                    _EmailTemplate.Subject = tempTemplate.EmailSubject;

                                    var emailConfigurations = dataAccess.EmailConfiguration.GetAll().FirstOrDefault();
                                    #region get EmailTemplate From Configuration
                                    var EmailTemplate = GetConfiguration("EmailTemplate");
                                    string EmailBody = _EmailTemplate.Body;
                                    if (EmailTemplate != null)
                                    {
                                        EmailBody = EmailTemplate.Value.Replace("{emailbody}", _EmailTemplate.Body);
                                    }
                                    #endregion
                                    SendEmail(_EmailTemplate.To, _EmailTemplate.CC, _EmailTemplate.Subject, EmailBody, emailConfigurations);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                continue;
                            }

                        }
                    }
                }

            }
        }

        public Task<int> SendNotificationForLevel(int LevelID, LevelTypeEnum level, int requestID, enumNotificationEventType eventType, string NotificationType = "Reminder")
        {
            using (var dataAccess = _factory.Create())
            {
                List<RequestStep> _RequestSteps = dataAccess.Request.Query().IncludeOptimized(a => a.RequestSteps).SelectMany(a => a.RequestSteps).Where(a => !a.IsCancelled).ToList();
                List<NotificationConfiguration> _Templates = Read();
                NotificationConfiguration _template = null;

                Notification _Notification = new Notification();
                List<Notification> ReturnNotifications = new List<Notification>();
                Notification ReturnNotification;
                EmailTemplate _EmailTemplate = new EmailTemplate();

                Request request = dataAccess.Request.Query().Where(w => w.ID == requestID).FirstOrDefault();
                Workflow workflow = dataAccess.Workflow.Query().Where(w => w.WorkflowID == request.WorkflowID).FirstOrDefault();

                if (_Templates.Count > 0)
                {
                    _template = new NotificationConfiguration();
                    _template = _Templates.Where(a => a.ActionType == (int)eventType).FirstOrDefault();
                }
                if (_template != null)
                {
                    _template = ResolveNotificationsForLevel(LevelID, level, requestID, _template, _RequestSteps, eventType);

                    List<string> CC = new List<string>();
                    foreach (NotificationReceiver reciever in _template.NotificationReceivers.Where(a => a.IsCC))
                    {
                        foreach (User user in reciever.Users)
                        {
                            if (user != null)
                            {
                                CC.Add(user.Email);
                            }
                        }
                    }

                    NotificationConfiguration tempTemplate = null;
                    foreach (NotificationReceiver reciever in _template.NotificationReceivers)
                    {
                        foreach (User user in reciever.Users)
                        {
                            try
                            {
                                tempTemplate = new NotificationConfiguration();
                                //Handle User Parameter
                                if (_template.NotificationEnglish.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.NotificationEnglish = _template.NotificationEnglish.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.NotificationEnglish = _template.NotificationEnglish;

                                if (_template.NotificationArabic.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.NotificationArabic = _template.NotificationArabic.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.NotificationArabic = _template.NotificationArabic;

                                if (_template.EmailSubject.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.EmailSubject = _template.EmailSubject.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.EmailSubject = _template.EmailSubject;

                                if (_template.EmailBody.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.EmailBody = _template.EmailBody.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.EmailBody = _template.EmailBody;

                                if (!reciever.IsCC)
                                {
                                    // Send Push Notification
                                    ReturnNotification = new Notification();
                                    _Notification = new Notification();
                                    _Notification.EnglishName = tempTemplate.NotificationEnglish;
                                    _Notification.ArabicName = tempTemplate.NotificationArabic;
                                    _Notification.NotificationType = NotificationType;
                                    _Notification.Created = DateTime.Now;
                                    _Notification.Modified = DateTime.Now;
                                    _Notification.RelatedItemID = requestID;
                                    _Notification.Status = (int)model.NotificationEnums.Pending;
                                    _Notification.AssignedTo = user.UserName;
                                    _Notification.ItemType = $"{level}-{Enum.GetName(typeof(EnumWFBaseWorkflows), workflow.BaseWorkflowID)}";
                                    dataAccess.Notification.Save(_Notification);
                                    int result = dataAccess.Complete();
                                    if (result != 0)
                                        ReturnNotifications.Add(_Notification);
                                }
                                // Send Email
                                if (!reciever.IsCC)
                                {
                                    _EmailTemplate = new EmailTemplate();
                                    _EmailTemplate.To = user.Email;
                                    _EmailTemplate.CC = CC;
                                    _EmailTemplate.Body = tempTemplate.EmailBody;
                                    _EmailTemplate.Subject = tempTemplate.EmailSubject;

                                    var emailConfigurations = dataAccess.EmailConfiguration.GetAll().FirstOrDefault();
                                    #region get EmailTemplate From Configuration
                                    var EmailTemplate = GetConfiguration("EmailTemplate");
                                    string EmailBody = _EmailTemplate.Body;
                                    if (EmailTemplate != null)
                                    {
                                        EmailBody = EmailTemplate.Value.Replace("{emailbody}", _EmailTemplate.Body);
                                    }
                                    #endregion
                                    SendEmail(_EmailTemplate.To, _EmailTemplate.CC, _EmailTemplate.Subject, EmailBody, emailConfigurations);
                                }
                            }
                            catch (System.Exception)
                            {
                                continue;
                            }

                        }
                    }
                }

            }

            return Task.FromResult((int)eventType);
        }
        public NotificationConfiguration ResolveNotificationsForLevel(int LevelID, LevelTypeEnum level, int RequestID, NotificationConfiguration _template, List<RequestStep> requestSteps, enumNotificationEventType eventType)
        {
            using (var dataAccess = _factory.Create())
            {
                List<Group> Groups = dataAccess.Group.Query()
                   .IncludeOptimized(a => a.UsersGroups)
                   .IncludeOptimizedByPath("UsersGroups.User")
                   .ToList();
                List<User> Users = dataAccess.User.Query().IncludeOptimized(e => e.UsersGroups)
                    .IncludeOptimizedByPath("UsersGroups.Group").Where(w => !w.Deleted).ToList();

                Request request = dataAccess.Request.Query().Where(w => w.ID == RequestID).FirstOrDefault();

                Workflow workflow = dataAccess.Workflow.Query().Where(w => w.WorkflowID == request.WorkflowID).FirstOrDefault();
                List<Lookup> lookups = dataAccess.Handshake.Query().Include(a => a.LookupValues).ToList();
                List<User> _TempReceivers = new List<User>();
                List<WorkflowStep> steps = dataAccess.WorkflowStep.Query().ToList();
               
                KPI kpi = null;
              

                //if (level == LevelTypeEnum.Initiative)
                //{
                //    initiative = dataAccess.Initiative.Query().Include(a => a.ManagerModel).Include(a => a.Department.OwnerModel).Where(w => w.ID == LevelID).FirstOrDefault();
                //}
                if (level == LevelTypeEnum.KPI)
                {
                    kpi = dataAccess.KPI.Query().IncludeOptimized(a => a.ChampionModel).IncludeOptimized(a => a.OwnerModel).Where(w => w.ID == LevelID).FirstOrDefault();
                }

                #region Resolve Recievers
                foreach (NotificationReceiver _Reciever in _template.NotificationReceivers)
                {

                    if (!_Reciever.IsGroup)
                    {
                        //HandleReceiver(KPI:Champion)
                        if (_Reciever.Receiver == model.NotificationReceivers.Requester.ToString())
                        {
                            //if (level == LevelTypeEnum.Initiative)
                            //{
                            //    CreateInitiativeFormDTO form = JsonConvert.DeserializeObject<CreateInitiativeFormDTO>(request.Form);
                            //    _Reciever.Users.Add(Users.Where(w => w.UserName.ToLower() == form.CreatedBy?.ToLower()).FirstOrDefault());
                            //}
                            if (level == LevelTypeEnum.KPI)
                            {
                                CreateKPIFormDTO form = JsonConvert.DeserializeObject<CreateKPIFormDTO>(request.Form);
                                _Reciever.Users.Add(Users.Where(w => w.UserName.ToLower() == form.CreatedBy?.ToLower()).FirstOrDefault());
                            }
                        }
                        else if (_Reciever.Receiver == model.NotificationReceivers.Owner.ToString())
                        {
                            if ( workflow.BaseWorkflowID == (int)EnumWFBaseWorkflows.CreateKPI)
                            {
                                //if (level == LevelTypeEnum.Initiative)
                                //{
                                //    CreateInitiativeFormDTO form = JsonConvert.DeserializeObject<CreateInitiativeFormDTO>(request.Form);
                                //    _Reciever.Users.Add(Users.Where(w => w.UserName.ToLower() == form.OwnerModel?.UserName?.ToLower()).FirstOrDefault());
                                //}
                                if (level == LevelTypeEnum.KPI)
                                {
                                    CreateKPIFormDTO form = JsonConvert.DeserializeObject<CreateKPIFormDTO>(request.Form);
                                    _Reciever.Users.Add(Users.Where(w => w.UserName.ToLower() == form.OwnerModel?.UserName?.ToLower()).FirstOrDefault());
                                }
                            }
                            if (workflow.BaseWorkflowID == (int)EnumWFBaseWorkflows.KPIChangeRequest)
                            {
                                _Reciever.Users.Add(kpi.OwnerModel);
                            }
                        }
                        else if (_Reciever.Receiver == "NextApprovers")
                        {
                            List<RequestStep> wfsteps = requestSteps.Where(a => a.RequestID == RequestID && a.Status == (int)model.EnumWFStatuses.New).ToList();
                            foreach (var step in wfsteps)
                            {
                                if (step != null)
                                {
                                    if (step.IsGroup)
                                    {
                                        List<User> users = Groups.Where(a => a.Title.ToLower() == step.Approver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                        foreach (User user in users)
                                        {
                                            _Reciever.Users.Add(user);
                                        }
                                    }
                                    else
                                    {
                                        User user = Users.Where(w => w.UserName == step.Approver.ToLower()).FirstOrDefault();
                                        _Reciever.Users.Add(user);
                                    }
                                }
                            }
                        }
                        else if (_Reciever.Receiver == "AllApprovers")
                        {
                            List<RequestStep> wfsteps = requestSteps.Where(a => a.RequestID == RequestID).Skip(1).ToList();
                            foreach (var step in wfsteps)
                            {
                                if (step != null)
                                {
                                    if (step.IsGroup)
                                    {
                                        List<User> users = Groups.Where(a => a.Title.ToLower() == step.Approver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                        foreach (User user in users)
                                        {
                                            _Reciever.Users.Add(user);
                                        }
                                    }
                                    else
                                    {
                                        User user = Users.Where(w => w.UserName == step.Approver.ToLower()).FirstOrDefault();
                                        _Reciever.Users.Add(user);
                                    }
                                }
                            }
                        }
                        else if (_Reciever.Receiver == "OrgStructureOwner")
                        {
                            //if (initiative != null)
                            //{
                            //    User user = Users.Where(w => w.UserName.ToLower() == initiative.Department?.Owner?.ToLower()).FirstOrDefault();
                            //    _Reciever.Users.Add(user);
                            //}
                        }
                        else if (_Reciever.Receiver == "AllPreviousApprovers")
                        {
                            List<RequestStep> wfsteps = requestSteps.Where(a => a.RequestID == RequestID && a.Status == (int)EnumWFStatuses.Approved).ToList();
                            foreach (var step in wfsteps)
                            {
                                if (step != null)
                                {
                                    if (step.IsGroup)
                                    {
                                        List<User> users = Groups.Where(a => a.Title.ToLower() == step.Approver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                        foreach (User user in users)
                                        {
                                            _Reciever.Users.Add(user);
                                        }
                                    }
                                    else
                                    {
                                        User user = Users.Where(w => w.UserName == step.Approver.ToLower()).FirstOrDefault();
                                        _Reciever.Users.Add(user);
                                    }
                                }
                            }
                        }
                        else if (_Reciever.Receiver == "CurrentApprover")
                        {
                            RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && a.Status == (int)model.EnumWFStatuses.Pending).FirstOrDefault();
                            if (step != null)
                            {
                                if (step.IsGroup)
                                {
                                    List<User> users = Users.Where(w => w.UsersGroups.Any(a => a.Group.Title.ToLower() == step.Approver.ToLower())).ToList();
                                    //List<User> users = Groups.Where(a => a.Title.ToLower() == step.Approver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                    foreach (User user in users)
                                    {
                                        _Reciever.Users.Add(user);
                                    }
                                }
                                else
                                {
                                    User user = Users.Where(w => w.UserName == step.Approver.ToLower()).FirstOrDefault();
                                    _Reciever.Users.Add(user);
                                }
                            }
                        }
                        else if (_Reciever.Receiver == "NextApprover")
                        {
                            RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && a.Status == (int)model.EnumWFStatuses.Pending).FirstOrDefault();
                            if (step != null)
                            {
                                if (step.IsGroup)
                                {
                                    List<User> users = Groups.Where(a => a.Title.ToLower() == step.Approver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                    foreach (User user in users)
                                    {
                                        _Reciever.Users.Add(user);
                                    }
                                }
                                else
                                {
                                    User user = Users.Where(w => w.UserName == step.Approver.ToLower()).FirstOrDefault();
                                    _Reciever.Users.Add(user);
                                }
                            }
                        }
                        else
                        {
                            int count = Groups.Where(a => a.Title.ToLower() == _Reciever.Receiver.ToLower()).Count();
                            if (count > 0)
                            {
                                List<User> _groupUsers = Groups.Where(a => a.Title.ToLower() == _Reciever.Receiver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                _Reciever.Users.AddRange(_groupUsers);
                            }
                        }
                    }
                    else
                    {
                        List<User> users = Groups.Where(a => a.Title.ToLower() == _Reciever.Receiver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                        foreach (User user in users)
                        {
                            _Reciever.Users.Add(user);
                        }
                    }

                    _Reciever.Users.AddRange(_TempReceivers);

                }
                #endregion

                #region Resolve Parametrs
                foreach (NotificationParameter _Parameter in _template.NotificationParameters)
                {
                    if (_Parameter.Title.ToLower() == "[level]")
                    {
                        if (level == LevelTypeEnum.KPI)
                            _Parameter.Value = LevelTypeEnum.KPI.ToString();
                        //else if (level == LevelTypeEnum.Initiative)
                        //    _Parameter.Value = LevelTypeEnum.Initiative.ToString();
                    }
                    if (_Parameter.Title.ToLower() == "[levelar]")
                    {
                        if (level == LevelTypeEnum.KPI)
                            _Parameter.Value = "مؤشر الأداء";
                        //else if (level == LevelTypeEnum.Initiative)
                        //    _Parameter.Value = "مبادرة";
                    }
                    if (_Parameter.Title.ToLower() == "[levelname]")
                    {
                        if ( workflow.BaseWorkflowID == (int)EnumWFBaseWorkflows.CreateKPI)
                        {
                            //if (level == LevelTypeEnum.Initiative)
                            //{
                            //    CreateInitiativeFormDTO form = JsonConvert.DeserializeObject<CreateInitiativeFormDTO>(request.Form);
                            //    _Parameter.Value = form.EnglishName;

                            //}
                            if (level == LevelTypeEnum.KPI)
                            {
                                CreateKPIFormDTO form = JsonConvert.DeserializeObject<CreateKPIFormDTO>(request.Form);
                                _Parameter.Value = form.EnglishName;
                            }
                        }
                        if (workflow.BaseWorkflowID == (int)EnumWFBaseWorkflows.KPIChangeRequest)
                        {
                            if (level == LevelTypeEnum.KPI)
                            {
                                _Parameter.Value = kpi.EnglishName;
                            }
                        }
                    }
                    if (_Parameter.Title.ToLower() == "[levelnamear]")
                    {
                        if ( workflow.BaseWorkflowID == (int)EnumWFBaseWorkflows.CreateKPI)
                        {
                            //if (level == LevelTypeEnum.Initiative)
                            //{
                            //    CreateInitiativeFormDTO form = JsonConvert.DeserializeObject<CreateInitiativeFormDTO>(request.Form);
                            //    _Parameter.Value = form.ArabicName;

                            //}
                            if (level == LevelTypeEnum.KPI)
                            {
                                CreateKPIFormDTO form = JsonConvert.DeserializeObject<CreateKPIFormDTO>(request.Form);
                                _Parameter.Value = form.ArabicName;
                            }
                        }
                        if (workflow.BaseWorkflowID == (int)EnumWFBaseWorkflows.KPIChangeRequest)
                        {
                            if (level == LevelTypeEnum.KPI)
                            {
                                _Parameter.Value = kpi.ArabicName;
                            }
                        }
                    }
                    if (_Parameter.Title.ToLower() == "[requester]")
                    {
                        var definition = new { CreatedBy = string.Empty };
                        var CreatedBy = JsonConvert.DeserializeAnonymousType(request.Form, definition).CreatedBy;
                        _Parameter.Value = Users.Where(w => w.UserName.ToLower() == CreatedBy?.ToLower()).FirstOrDefault().DisplayName;
                    }

                    if (_Parameter.Title.ToLower() == "[workflow]")
                    {
                        if (workflow != null)
                        {
                            //if (workflow.BaseWorkflowID == (int)EnumWFBaseWorkflows.CreateInitiative)
                            //{
                            //    _Parameter.Value = $"Create {level}";
                            //}
                            if (workflow.BaseWorkflowID == (int)EnumWFBaseWorkflows.CreateKPI)
                            {
                                _Parameter.Value = $"Add new {level}";
                            }
                            if (workflow.BaseWorkflowID == (int)EnumWFBaseWorkflows.KPIChangeRequest)
                            {
                                _Parameter.Value = $"{level} Change Request";
                            }
                        }
                    }
                    if (_Parameter.Title.ToLower() == "[workflowar]")
                    {
                        if (workflow != null)
                        {
                            //string levelAr = level == LevelTypeEnum.Initiative ? "مبادرة" : (level == LevelTypeEnum.KPI ? "مؤشر الأداء" : null);
                            string levelAr =  (level == LevelTypeEnum.KPI ? "مؤشر الأداء" : null);
                            if (levelAr != null)
                            {
                                if ( workflow.BaseWorkflowID == (int)EnumWFBaseWorkflows.CreateKPI)
                                {
                                    _Parameter.Value = $"اضافة {levelAr}";
                                }
                                if (workflow.BaseWorkflowID == (int)EnumWFBaseWorkflows.KPIChangeRequest)
                                {
                                    _Parameter.Value = $"اجراء تغيير على {levelAr}";
                                }
                            }
                        }
                    }
                    if (_Parameter.Title.ToLower() == "[action]")
                    {
                        RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && (a.Status == (int)model.EnumWFStatuses.Approved || a.Status == (int)model.EnumWFStatuses.Return || a.Status == (int)model.EnumWFStatuses.Rejected || a.Status == (int)model.EnumWFStatuses.NA)).LastOrDefault();
                        var statuses = lookups.Where(w => w.Title == "WorkflowStatus").SelectMany(s => s.LookupValues).ToList();
                        if (step != null)
                        {
                            _Parameter.Value = statuses.Where(w => w.Value == step.Status.ToString()).Select(s => s.English).FirstOrDefault();
                        }
                    }
                    if (_Parameter.Title.ToLower() == "[actionar]")
                    {
                        RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && (a.Status == (int)model.EnumWFStatuses.Approved || a.Status == (int)model.EnumWFStatuses.Return || a.Status == (int)model.EnumWFStatuses.Rejected || a.Status == (int)model.EnumWFStatuses.NA)).LastOrDefault();
                        var statuses = lookups.Where(w => w.Title == "WorkflowStatus").SelectMany(s => s.LookupValues).ToList();
                        if (step != null)
                        {
                            _Parameter.Value = statuses.Where(w => w.Value == step.Status.ToString()).Select(s => s.Arabic).FirstOrDefault();
                            if (step.Status == (int)model.EnumWFStatuses.Approved)
                                _Parameter.Value += "";
                        }
                    }
                    if (_Parameter.Title.ToLower() == "[stepapproverrole]")
                    {
                        RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && (a.Status == (int)model.EnumWFStatuses.Approved || a.Status == (int)model.EnumWFStatuses.Return || a.Status == (int)model.EnumWFStatuses.Rejected || a.Status == (int)model.EnumWFStatuses.NA)).LastOrDefault();
                        if (step != null)
                        {
                            if (step.EnglishName.ToLower() == WFApproversEnum.Requester.ToString().ToLower())
                                _Parameter.Value = step.Approver?.MapUser()?.DisplayName;
                            else
                                _Parameter.Value = step.EnglishName;
                        }
                    }
                    if (_Parameter.Title.ToLower() == "[stepapproverrolear]")
                    {
                        RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && (a.Status == (int)model.EnumWFStatuses.Approved || a.Status == (int)model.EnumWFStatuses.Return || a.Status == (int)model.EnumWFStatuses.Rejected || a.Status == (int)model.EnumWFStatuses.NA)).LastOrDefault();
                        if (step != null)
                        {
                            if (step.EnglishName.ToLower() == WFApproversEnum.Requester.ToString().ToLower())
                                _Parameter.Value = step.Approver?.MapUser()?.DisplayName;
                            else
                                _Parameter.Value = step.ArabicName;
                        }
                    }
                    if (_Parameter.Title.ToLower() == "[stepapproverdisplayname]")
                    {
                        RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && (a.Status == (int)model.EnumWFStatuses.Approved || a.Status == (int)model.EnumWFStatuses.Return || a.Status == (int)model.EnumWFStatuses.Rejected || a.Status == (int)model.EnumWFStatuses.NA)).LastOrDefault();
                        if (step != null)
                            if (step.IsGroup)
                            {
                                //var groupUsers = Groups.Where(w => w.Title.ToLower() == step.Approver.ToLower()).FirstOrDefault()?.UsersGroups.Select(s => s.User.DisplayName).ToList();
                                //_Parameter.Value = Users.Where(w => w.UserName == step.Approver).FirstOrDefault().DisplayName;
                            }
                            else
                                _Parameter.Value = Users.Where(w => w.UserName == step.Approver).FirstOrDefault().DisplayName;
                    }
                    if (_Parameter.Title.ToLower() == "[requesturl]")
                    {
                        RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && (a.Status == (int)model.EnumWFStatuses.Approved || a.Status == (int)model.EnumWFStatuses.Return || a.Status == (int)model.EnumWFStatuses.Rejected || a.Status == (int)model.EnumWFStatuses.NA)).LastOrDefault();
                        string FormURL = dataAccess.Configuration.Query().Where(w => w.Key == "LevelDetailURL").FirstOrDefault()?.Value;
                        string TaskCenterURL = dataAccess.Configuration.Query().Where(w => w.Key == "TaskCenterURL").FirstOrDefault()?.Value;
                        //if (workflow.BaseWorkflowID == (int)EnumWFBaseWorkflows.CreateInitiative)
                        //{
                        //    if (FormURL != null && FormURL.Contains("{ItemID}"))
                        //    {
                        //        if (level == LevelTypeEnum.Initiative)
                        //        {
                        //            CreateInitiativeFormDTO form = JsonConvert.DeserializeObject<CreateInitiativeFormDTO>(request.Form);
                        //            FormURL = FormURL.Replace("{ItemID}", request.ID.ToString());
                        //        }
                        //    }
                        //    if (FormURL != null && FormURL.Contains("{item}"))
                        //    {
                        //        if (level == LevelTypeEnum.Initiative)
                        //        {
                        //            FormURL = FormURL.Replace("{item}", $"add-new-{level}");
                        //        }
                        //    }
                        //    _Parameter.Value = FormURL;
                        //}
                        if (workflow.BaseWorkflowID == (int)EnumWFBaseWorkflows.CreateKPI)
                        {
                            //if (FormURL != null && FormURL.Contains("{ItemID}"))
                            //{
                            //    if (level == LevelTypeEnum.KPI)
                            //    {
                            //        FormURL = FormURL.Replace("{ItemID}", request.ID.ToString());
                            //    }
                            //}
                            //if (FormURL != null && FormURL.Contains("{item}"))
                            //{
                            //    if (level == LevelTypeEnum.KPI)
                            //    {
                            //        FormURL = FormURL.Replace("{item}", $"add-new-{level}");
                            //    }
                            //}
                            _Parameter.Value = TaskCenterURL;
                        }
                        if (workflow.BaseWorkflowID == (int)EnumWFBaseWorkflows.KPIChangeRequest)
                        {
                            if (FormURL != null && FormURL.Contains("{ItemID}"))
                            {
                                FormURL = FormURL.Replace("{ItemID}", request.ID.ToString());
                            }
                            if (FormURL != null && FormURL.Contains("{item}"))
                            {
                                FormURL = FormURL.Replace("{item}", $"{level}-change-request");
                            }
                            _Parameter.Value = FormURL;
                        }
                    }
                }
                _template = ReplaceText(_template);
                #endregion
            }
            return _template;

        }


        public Task<int> SendNotificationForMitigiationPlan( LevelTypeEnum level, int requestID, enumNotificationEventType eventType, string userName,string NotificationType = "Reminder")
        {
            using (var dataAccess = _factory.Create())
            {
                List<RequestStep> _RequestSteps = dataAccess.Request.Query().IncludeOptimized(a => a.RequestSteps).SelectMany(a => a.RequestSteps).Where(a => !a.IsCancelled).ToList();
                List<NotificationConfiguration> _Templates = Read();
                NotificationConfiguration _template = null;

                Notification _Notification = new Notification();
                List<Notification> ReturnNotifications = new List<Notification>();
                Notification ReturnNotification;
                EmailTemplate _EmailTemplate = new EmailTemplate();

                Request request = dataAccess.Request.Query().Where(w => w.ID == requestID).FirstOrDefault();
                Workflow workflow = dataAccess.Workflow.Query().Where(w => w.WorkflowID == request.WorkflowID).FirstOrDefault();

                if (_Templates.Count > 0)
                {
                    _template = new NotificationConfiguration();
                    _template = _Templates.Where(a => a.ActionType == (int)eventType).FirstOrDefault();
                }
                if (_template != null)
                {
                    _template = ResolveNotificationsForMitigiationPlan( level, requestID, _template, _RequestSteps, eventType, userName);

                    List<string> CC = new List<string>();
                    foreach (NotificationReceiver reciever in _template.NotificationReceivers.Where(a => a.IsCC))
                    {
                        foreach (User user in reciever.Users)
                        {
                            if (user != null)
                            {
                                CC.Add(user.Email);
                            }
                        }
                    }

                    NotificationConfiguration tempTemplate = null;
                    foreach (NotificationReceiver reciever in _template.NotificationReceivers)
                    {
                        foreach (User user in reciever.Users)
                        {
                            try
                            {
                                tempTemplate = new NotificationConfiguration();
                                //Handle User Parameter
                                if (_template.NotificationEnglish.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.NotificationEnglish = _template.NotificationEnglish.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.NotificationEnglish = _template.NotificationEnglish;

                                if (_template.NotificationArabic.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.NotificationArabic = _template.NotificationArabic.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.NotificationArabic = _template.NotificationArabic;

                                if (_template.EmailSubject.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.EmailSubject = _template.EmailSubject.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.EmailSubject = _template.EmailSubject;

                                if (_template.EmailBody.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.EmailBody = _template.EmailBody.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.EmailBody = _template.EmailBody;

                                if (!reciever.IsCC)
                                {
                                    // Send Push Notification
                                    ReturnNotification = new Notification();
                                    _Notification = new Notification();
                                    _Notification.EnglishName = tempTemplate.NotificationEnglish;
                                    _Notification.ArabicName = tempTemplate.NotificationArabic;
                                    _Notification.NotificationType = NotificationType;
                                    _Notification.Created = DateTime.Now;
                                    _Notification.Modified = DateTime.Now;
                                    _Notification.RelatedItemID = requestID;
                                    _Notification.Status = (int)model.NotificationEnums.Pending;
                                    _Notification.AssignedTo = user.UserName;
                                    _Notification.ItemType = $"{level}-{Enum.GetName(typeof(EnumWFBaseWorkflows), workflow.BaseWorkflowID)}";
                                    dataAccess.Notification.Save(_Notification);
                                    int result = dataAccess.Complete();
                                    if (result != 0)
                                        ReturnNotifications.Add(_Notification);
                                }
                                // Send Email
                                if (!reciever.IsCC)
                                {
                                    _EmailTemplate = new EmailTemplate();
                                    _EmailTemplate.To = user.Email;
                                    _EmailTemplate.CC = CC;
                                    _EmailTemplate.Body = tempTemplate.EmailBody;
                                    _EmailTemplate.Subject = tempTemplate.EmailSubject;

                                    var emailConfigurations = dataAccess.EmailConfiguration.GetAll().FirstOrDefault();
                                    #region get EmailTemplate From Configuration
                                    var EmailTemplate = GetConfiguration("EmailTemplate");
                                    string EmailBody = _EmailTemplate.Body;
                                    if (EmailTemplate != null)
                                    {
                                        EmailBody = EmailTemplate.Value.Replace("{emailbody}", _EmailTemplate.Body);
                                    }
                                    #endregion
                                    SendEmail(_EmailTemplate.To, _EmailTemplate.CC, _EmailTemplate.Subject, EmailBody, emailConfigurations);
                                }
                            }
                            catch (System.Exception)
                            {
                                continue;
                            }

                        }
                    }
                }

            }

            return Task.FromResult((int)eventType);
        }
        public NotificationConfiguration ResolveNotificationsForMitigiationPlan(LevelTypeEnum level, int RequestID, NotificationConfiguration _template, List<RequestStep> requestSteps, enumNotificationEventType eventType,string userName)
        {
            using (var dataAccess = _factory.Create())
            {
                List<Group> Groups = dataAccess.Group.Query()
                   .IncludeOptimized(a => a.UsersGroups)
                   .IncludeOptimizedByPath("UsersGroups.User")
                   .ToList();
                List<User> Users = dataAccess.User.Query().IncludeOptimized(e => e.UsersGroups)
                    .IncludeOptimizedByPath("UsersGroups.Group").Where(w => !w.Deleted).ToList();

                Request request = dataAccess.Request.Query().Where(w => w.ID == RequestID).FirstOrDefault();

                Workflow workflow = dataAccess.Workflow.Query().Where(w => w.WorkflowID == request.WorkflowID).FirstOrDefault();
                List<Lookup> lookups = dataAccess.Handshake.Query().Include(a => a.LookupValues).ToList();
                List<User> _TempReceivers = new List<User>();
                List<WorkflowStep> steps = dataAccess.WorkflowStep.Query().ToList();

               


                //if (level == LevelTypeEnum.Initiative)
                //{
                //    initiative = dataAccess.Initiative.Query().Include(a => a.ManagerModel).Include(a => a.Department.OwnerModel).Where(w => w.ID == LevelID).FirstOrDefault();
                //}
                //if (level == LevelTypeEnum.KPI)
                //{
                //    kpi = dataAccess.KPI.Query().IncludeOptimized(a => a.ChampionModel).IncludeOptimized(a => a.OwnerModel).Where(w => w.ID == LevelID).FirstOrDefault();
                //}

                #region Resolve Recievers
                foreach (NotificationReceiver _Reciever in _template.NotificationReceivers)
                {

                    if (!_Reciever.IsGroup)
                    {
                        //HandleReceiver(KPI:Champion)
                        if (_Reciever.Receiver == model.NotificationReceivers.Requester.ToString())
                        {
                            //if (level == LevelTypeEnum.Initiative)
                            //{
                            //    CreateInitiativeFormDTO form = JsonConvert.DeserializeObject<CreateInitiativeFormDTO>(request.Form);
                            //    _Reciever.Users.Add(Users.Where(w => w.UserName.ToLower() == form.CreatedBy?.ToLower()).FirstOrDefault());
                            //}
                            if (level == LevelTypeEnum.MitigationAction)
                            {
                                MitigationActionRequestFormDTO form = JsonConvert.DeserializeObject<MitigationActionRequestFormDTO>(request.Form);
                                _Reciever.Users.Add(Users.Where(w => w.UserName.ToLower() == form.CreatedBy?.ToLower()).FirstOrDefault());
                            }
                        }
                    
                        else if (_Reciever.Receiver == "NextApprovers")
                        {
                            List<RequestStep> wfsteps = requestSteps.Where(a => a.RequestID == RequestID && a.Status == (int)model.EnumWFStatuses.New).ToList();
                            foreach (var step in wfsteps)
                            {
                                if (step != null)
                                {
                                    if (step.IsGroup)
                                    {
                                        List<User> users = Groups.Where(a => a.Title.ToLower() == step.Approver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                        foreach (User user in users)
                                        {
                                            _Reciever.Users.Add(user);
                                        }
                                    }
                                    else
                                    {
                                        User user = Users.Where(w => w.UserName == step.Approver.ToLower()).FirstOrDefault();
                                        _Reciever.Users.Add(user);
                                    }
                                }
                            }
                        }
                        else if (_Reciever.Receiver == "AllApprovers")
                        {
                            List<RequestStep> wfsteps = requestSteps.Where(a => a.RequestID == RequestID).Skip(1).ToList();
                            foreach (var step in wfsteps)
                            {
                                if (step != null)
                                {
                                    if (step.IsGroup)
                                    {
                                        List<User> users = Groups.Where(a => a.Title.ToLower() == step.Approver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                        foreach (User user in users)
                                        {
                                            _Reciever.Users.Add(user);
                                        }
                                    }
                                    else
                                    {
                                        User user = Users.Where(w => w.UserName == step.Approver.ToLower()).FirstOrDefault();
                                        _Reciever.Users.Add(user);
                                    }
                                }
                            }
                        }
                        else if (_Reciever.Receiver == "OrgStructureOwner")
                        {
                            //if (initiative != null)
                            //{
                            //    User user = Users.Where(w => w.UserName.ToLower() == initiative.Department?.Owner?.ToLower()).FirstOrDefault();
                            //    _Reciever.Users.Add(user);
                            //}
                        }
                        else if (_Reciever.Receiver == "AllPreviousApprovers")
                        {
                            List<RequestStep> wfsteps = requestSteps.Where(a => a.RequestID == RequestID && a.Status == (int)EnumWFStatuses.Approved).ToList();
                            foreach (var step in wfsteps)
                            {
                                if (step != null)
                                {
                                    if (step.IsGroup)
                                    {
                                        List<User> users = Groups.Where(a => a.Title.ToLower() == step.Approver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                        foreach (User user in users)
                                        {
                                            _Reciever.Users.Add(user);
                                        }
                                    }
                                    else
                                    {
                                        User user = Users.Where(w => w.UserName == step.Approver.ToLower()).FirstOrDefault();
                                        _Reciever.Users.Add(user);
                                    }
                                }
                            }
                        }
                        else if (_Reciever.Receiver == "CurrentApprover")
                        {
                            RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && a.Status == (int)model.EnumWFStatuses.Pending).FirstOrDefault();
                            if (step != null)
                            {
                                if (step.IsGroup)
                                {
                                    List<User> users = Users.Where(w => w.UsersGroups.Any(a => a.Group.Title.ToLower() == step.Approver.ToLower())).ToList();
                                    //List<User> users = Groups.Where(a => a.Title.ToLower() == step.Approver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                    foreach (User user in users)
                                    {
                                        _Reciever.Users.Add(user);
                                    }
                                }
                                else
                                {
                                    User user = Users.Where(w => w.UserName == step.Approver.ToLower()).FirstOrDefault();
                                    _Reciever.Users.Add(user);
                                }
                            }
                        }
                        else if (_Reciever.Receiver == "NextApprover")
                        {
                            RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && a.Status == (int)model.EnumWFStatuses.Pending).FirstOrDefault();
                            if (step != null)
                            {
                                if (step.IsGroup)
                                {
                                    List<User> users = Groups.Where(a => a.Title.ToLower() == step.Approver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                    foreach (User user in users)
                                    {
                                        _Reciever.Users.Add(user);
                                    }
                                }
                                else
                                {
                                    User user = Users.Where(w => w.UserName == step.Approver.ToLower()).FirstOrDefault();
                                    _Reciever.Users.Add(user);
                                }
                            }
                        }
                        else
                        {
                            int count = Groups.Where(a => a.Title.ToLower() == _Reciever.Receiver.ToLower()).Count();
                            if (count > 0)
                            {
                                List<User> _groupUsers = Groups.Where(a => a.Title.ToLower() == _Reciever.Receiver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                _Reciever.Users.AddRange(_groupUsers);
                            }
                        }
                    }
                    else
                    {
                        List<User> users = Groups.Where(a => a.Title.ToLower() == _Reciever.Receiver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                        foreach (User user in users)
                        {
                            _Reciever.Users.Add(user);
                        }
                    }

                    _Reciever.Users.AddRange(_TempReceivers);

                }
                #endregion

                #region Resolve Parametrs
                foreach (NotificationParameter _Parameter in _template.NotificationParameters)
                {
                    
                    if (_Parameter.Title.ToLower() == "[requester]")
                    {
                        var definition = new { CreatedBy = string.Empty };
                        var CreatedBy = JsonConvert.DeserializeAnonymousType(request.Form, definition).CreatedBy;
                        _Parameter.Value = Users.Where(w => w.UserName.ToLower() == CreatedBy?.ToLower()).FirstOrDefault().DisplayName;
                    }

               
                    if (_Parameter.Title.ToLower() == "[action]")
                    {
                        RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && (a.Status == (int)model.EnumWFStatuses.Approved || a.Status == (int)model.EnumWFStatuses.Return || a.Status == (int)model.EnumWFStatuses.Rejected || a.Status == (int)model.EnumWFStatuses.NA)).LastOrDefault();
                        var statuses = lookups.Where(w => w.Title == "WorkflowStatus").SelectMany(s => s.LookupValues).ToList();
                        if (step != null)
                        {
                            _Parameter.Value = statuses.Where(w => w.Value == step.Status.ToString()).Select(s => s.English).FirstOrDefault();
                        }
                    }
                    if (_Parameter.Title.ToLower() == "[actionar]")
                    {
                        RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && (a.Status == (int)model.EnumWFStatuses.Approved || a.Status == (int)model.EnumWFStatuses.Return || a.Status == (int)model.EnumWFStatuses.Rejected || a.Status == (int)model.EnumWFStatuses.NA)).LastOrDefault();
                        var statuses = lookups.Where(w => w.Title == "WorkflowStatus").SelectMany(s => s.LookupValues).ToList();
                        if (step != null)
                        {
                            _Parameter.Value = statuses.Where(w => w.Value == step.Status.ToString()).Select(s => s.Arabic).FirstOrDefault();
                            if (step.Status == (int)model.EnumWFStatuses.Approved)
                                _Parameter.Value += "";
                        }
                    }


                    if (_Parameter.Title.ToLower() == "[actionby]")
                    {
                        _Parameter.Value = Users.Where(w => w.UserName.ToLower() == userName.ToLower()).FirstOrDefault().DisplayName;
                    }


                    if (_Parameter.Title.ToLower() == "[requesturl]")
                    {
                        RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && (a.Status == (int)model.EnumWFStatuses.Approved || a.Status == (int)model.EnumWFStatuses.Return || a.Status == (int)model.EnumWFStatuses.Rejected || a.Status == (int)model.EnumWFStatuses.NA)).LastOrDefault();
                        string FormURL = dataAccess.Configuration.Query().Where(w => w.Key == "MitigationActionURL").FirstOrDefault()?.Value;
                     
                      
                        if (workflow.BaseWorkflowID == (int)EnumWFBaseWorkflows.MitigationAction)
                        {
                            if (FormURL != null && FormURL.Contains("{ItemID}"))
                            {
                                FormURL = FormURL.Replace("{ItemID}", request.ID.ToString());
                            }
                            if (FormURL != null && FormURL.Contains("{item}"))
                            {
                                FormURL = FormURL.Replace("{item}", $"{level}-change-request");
                            }
                            _Parameter.Value = FormURL;
                        }
                    }
                }
                _template = ReplaceText(_template);
                #endregion
            }
            return _template;

        }



        #region KPI
        public NotificationConfiguration ResolveNotificationsForKPI(int KPIID, int MeasureID, int RequestID, NotificationConfiguration _template, List<RequestStep> requestSteps)
        {
            using (var dataAccess = _factory.Create())
            {
                KPI _KPI = dataAccess.KPI.Query().IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimizedByPath("KPIType.KPIThresholds")
                    .IncludeOptimizedByPath("KPIType.KPIThresholds.Status")
                    .IncludeOptimizedByPath("KPIType.ReminderConfiguration").Where(w => w.ID == KPIID).FirstOrDefault();
                KPIMeasure _Measure = _KPI.KPIMeasures.Where(w => w.ID == MeasureID).FirstOrDefault();
                int KPITypeID = _KPI.KPITypeID;
                List<Group> Groups = dataAccess.Group.Query()
                    .IncludeOptimized(a => a.UsersGroups)
                    .IncludeOptimizedByPath("UsersGroups.User")
                    .ToList();
                List<User> Users = dataAccess.User.Query().IncludeOptimized(e => e.UsersGroups)
                    .IncludeOptimizedByPath("UsersGroups.Group").Where(w => !w.Deleted).ToList();
                ReminderConfiguration _Reminder = _KPI.KPIType.ReminderConfiguration;
                List<User> _TempReceivers = new List<User>();
                //WFFormUpdateKPI form = dataAccess.WFFormUpdateKPI.Query().IncludeOptimized(a => a.WFRequest).Where(w => w.WFRequest.ID == RequestID).FirstOrDefault();
                List<WorkflowStep> steps = dataAccess.WorkflowStep.Query().ToList();

                #region Resolve Recievers
                foreach (NotificationReceiver _Reciever in _template.NotificationReceivers)
                {
                    if (!_Reciever.IsGroup)
                    {
                        //HandleReceiver(KPI:Champion)
                        if (_Reciever.Receiver == model.NotificationReceivers.Champion.ToString())
                            _Reciever.Users.Add(_KPI.ChampionModel);
                        else if (_Reciever.Receiver == model.NotificationReceivers.Owner.ToString())
                        {
                            if (_KPI.OwnerModel != null)
                                _Reciever.Users.Add(_KPI.OwnerModel);
                        }
                        else if (_Reciever.Receiver == "NextApprovers")
                        {
                            List<RequestStep> wfsteps = requestSteps.Where(a => a.RequestID == RequestID && a.Status == (int)model.EnumWFStatuses.New).ToList();
                            foreach (var step in wfsteps)
                            {
                                if (step != null)
                                {
                                    if (step.IsGroup)
                                    {
                                        List<User> users = Groups.Where(a => a.Title.ToLower() == step.Approver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                        foreach (User user in users)
                                        {
                                            _Reciever.Users.Add(user);
                                        }
                                    }
                                    else
                                    {
                                        User user = Users.Where(w => w.UserName == step.Approver.ToLower()).FirstOrDefault();
                                        _Reciever.Users.Add(user);
                                    }
                                }
                            }

                        }
                        else if (_Reciever.Receiver == "AllApprovers")
                        {
                            List<RequestStep> wfsteps = requestSteps.Where(a => a.RequestID == RequestID).Skip(1).ToList();
                            foreach (var step in wfsteps)
                            {
                                if (step != null)
                                {
                                    if (step.IsGroup)
                                    {
                                        List<User> users = Groups.Where(a => a.Title.ToLower() == step.Approver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                        foreach (User user in users)
                                        {
                                            _Reciever.Users.Add(user);
                                        }
                                    }
                                    else
                                    {
                                        User user = Users.Where(w => w.UserName == step.Approver.ToLower()).FirstOrDefault();
                                        _Reciever.Users.Add(user);
                                    }
                                }
                            }

                        }
                        else if (_Reciever.Receiver == "CurrentApprover")
                        {
                            RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && a.Status == (int)model.EnumWFStatuses.Pending).FirstOrDefault();
                            if (step != null)
                            {
                                if (step.IsGroup)
                                {
                                    List<User> users = Users.Where(w => w.UsersGroups.Any(a => a.Group.Title.ToLower() == step.Approver.ToLower())).ToList();
                                    //List<User> users = Groups.Where(a => a.Title.ToLower() == step.Approver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                    foreach (User user in users)
                                    {
                                        _Reciever.Users.Add(user);
                                    }
                                }
                                else
                                {
                                    User user = Users.Where(w => w.UserName == step.Approver.ToLower()).FirstOrDefault();
                                    _Reciever.Users.Add(user);
                                }
                            }
                        }
                        else if (_Reciever.Receiver == "NextApprover")
                        {
                            RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && a.Status == (int)model.EnumWFStatuses.Pending).FirstOrDefault();
                            if (step != null)
                            {
                                if (step.IsGroup)
                                {
                                    List<User> users = Groups.Where(a => a.Title.ToLower() == step.Approver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                    foreach (User user in users)
                                    {
                                        _Reciever.Users.Add(user);
                                    }
                                }
                                else
                                {
                                    User user = Users.Where(w => w.UserName == step.Approver.ToLower()).FirstOrDefault();
                                    _Reciever.Users.Add(user);
                                }
                            }
                        }
                        else
                        {
                            int count = Groups.Where(a => a.Title.ToLower() == _Reciever.Receiver.ToLower()).Count();
                            if (count > 0)
                            {
                                List<User> _groupUsers = Groups.Where(a => a.Title.ToLower() == _Reciever.Receiver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                                _Reciever.Users.AddRange(_groupUsers);
                            }
                        }
                    }
                    else
                    {
                        List<User> users = Groups.Where(a => a.Title.ToLower() == _Reciever.Receiver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                        foreach (User user in users)
                        {
                            _Reciever.Users.Add(user);
                        }
                    }

                    _Reciever.Users.AddRange(_TempReceivers);

                }
                #endregion

                #region Resolve Parametrs
                foreach (NotificationParameter _Parameter in _template.NotificationParameters)
                {
                    if (_Parameter.Title.ToLower() == "[levelname]")
                        _Parameter.Value = _KPI.EnglishName;

                   

                    if (_Parameter.Title.ToLower() == "[levelnamear]")
                        _Parameter.Value = _KPI.ArabicName;

                    if (_Parameter.Title.ToLower() == "[measuredate]" && _Measure != null)
                        _Parameter.Value = _Measure.DueDate.Date.ToString("dd-MMM-yyyy");

                    if (_Parameter.Title.ToLower() == "[championdisplayname]")
                        _Parameter.Value = _KPI.Champion;

                    if (_Parameter.Title.ToLower() == "[beforereminderdays]" && _Reminder != null)
                    {
                        _Parameter.Value = _Reminder.BeforeReminder.ToString();
                    }
                    if (_Parameter.Title.ToLower() == "[stepapproverrole]")
                    {
                        RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && (a.Status == (int)model.EnumWFStatuses.Approved || a.Status == (int)model.EnumWFStatuses.Rejected || a.Status == (int)model.EnumWFStatuses.NA || a.Status == (int)model.EnumWFStatuses.Return)).LastOrDefault();
                        if (step != null)
                        {
                            _Parameter.Value = step.EnglishName;
                        }
                    }

                    if (_Parameter.Title.ToLower() == "[comments]")
                    {
                        RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && ( a.Status == (int)model.EnumWFStatuses.Rejected )).LastOrDefault();
                        if (step != null)
                        {
                            _Parameter.Value = step.Comments;
                        }
                    }

                    if (_Parameter.Title.ToLower() == "[stepapproverrolear]")
                    {
                        RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && (a.Status == (int)model.EnumWFStatuses.Approved || a.Status == (int)model.EnumWFStatuses.Rejected || a.Status == (int)model.EnumWFStatuses.NA || a.Status == (int)model.EnumWFStatuses.Return)).LastOrDefault();
                        if (step != null)
                        {
                            _Parameter.Value = step.ArabicName;
                        }
                    }
                    if (_Parameter.Title.ToLower() == "[stepapproverdisplayname]")
                    {
                        RequestStep step = requestSteps.Where(a => a.RequestID == RequestID && (a.Status == (int)model.EnumWFStatuses.Approved || a.Status == (int)model.EnumWFStatuses.Rejected || a.Status == (int)model.EnumWFStatuses.NA || a.Status == (int)model.EnumWFStatuses.Return)).LastOrDefault();
                        if (step != null)
                            if (step.IsGroup)
                            {
                                //var groupUsers = Groups.Where(w => w.Title.ToLower() == step.Approver.ToLower()).FirstOrDefault()?.UsersGroups.Select(s => s.User.DisplayName).ToList();
                                //_Parameter.Value = Users.Where(w => w.UserName == step.Approver).FirstOrDefault().DisplayName;
                            }
                            else
                                _Parameter.Value = Users.Where(w => w.UserName == step.Approver).FirstOrDefault().DisplayName;
                    }
                    if (_Parameter.Title.ToLower() == "[levelurl]")
                    {
                        string URL = dataAccess.Configuration.Query().Where(w => w.Key == "LevelDetailsURL").FirstOrDefault()?.Value;
                        if (URL != null && URL.Contains("{level}"))
                        {
                            URL = URL.Replace("{level}", "kpi");
                        }
                        if (URL != null && URL.Contains("{ItemID}"))
                        {
                            URL = URL.Replace("{ItemID}", _KPI.ID.ToString());
                        }
                        _Parameter.Value = URL;
                    }

                    if (_Parameter.Title.ToLower() == "[crurl]")
                    {
                        string URL = dataAccess.Configuration.Query().Where(w => w.Key == "CRDetailsURL").FirstOrDefault()?.Value;
                        if (URL != null && URL.Contains("{level}"))
                        {
                            URL = URL.Replace("{level}", "kpi");
                        }
                        if (URL != null && URL.Contains("{ItemID}"))
                        {
                            URL = URL.Replace("{ItemID}", RequestID.ToString());
                        }
                        _Parameter.Value = URL;
                    }

                    if (_Parameter.Title.ToLower() == "[myrequesturl]")
                    {
                        string URL = dataAccess.Configuration.Query().Where(w => w.Key == "MyRequestURL").FirstOrDefault()?.Value;
                        if (URL != null && URL.Contains("{level}"))
                        {
                            URL = URL.Replace("{level}", "kpi");
                        }
                        if (URL != null && URL.Contains("{ItemID}"))
                        {
                            URL = URL.Replace("{ItemID}", RequestID.ToString());
                        }
                        _Parameter.Value = URL;
                    }

                    if (_Parameter.Title.ToLower() == "[myapprovalurl]")
                    {
                        string URL = dataAccess.Configuration.Query().Where(w => w.Key == "MyApprovalURL").FirstOrDefault()?.Value;
                        if (URL != null && URL.Contains("{level}"))
                        {
                            URL = URL.Replace("{level}", "activity");
                        }
                        if (URL != null && URL.Contains("{ItemID}"))
                        {
                            URL = URL.Replace("{ItemID}", RequestID.ToString());
                        }
                        _Parameter.Value = URL;
                    }

                }
                _template = ReplaceText(_template);
                #endregion
            }
            return _template;

        }
        #endregion

        #region Delegation
        public void SendNotificationForDelegation(int DelegationID, model.enumNotificationEventType eventType)
        {
            using (var dataAccess = _factory.Create())
            {
                string NotificationType = string.Empty;
                NotificationType = "Delegation";
                List<NotificationConfiguration> _Templates = Read();
                NotificationConfiguration _template = null;
                Notification _Notification = new Notification();
                List<Notification> ReturnNotifications = new List<Notification>();
                Notification ReturnNotification;
                EmailTemplate _EmailTemplate = new EmailTemplate();
                if (_Templates.Count > 0)
                {
                    _template = _Templates.Where(a => a.ActionType == (int)eventType).FirstOrDefault();
                }
                if (_template != null)
                {
                    _template = ResolveNotificationForDelegation(DelegationID, _template);

                    List<string> CC = new List<string>();
                    foreach (NotificationReceiver receiver in _template.NotificationReceivers.Where(a => a.IsCC))
                    {
                        foreach (User user in receiver.Users)
                        {
                            CC.Add(user.Email);
                        }
                    }
                    NotificationConfiguration tempTemplate = null;
                    foreach (NotificationReceiver receiver in _template.NotificationReceivers)
                    {
                        foreach (User user in receiver.Users)
                        {
                            try
                            {
                                tempTemplate = new NotificationConfiguration();
                                //Handle User Parameter
                                if (_template.NotificationEnglish.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.NotificationEnglish = _template.NotificationEnglish.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.NotificationEnglish = _template.NotificationEnglish;

                                if (_template.NotificationArabic.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.NotificationArabic = _template.NotificationArabic.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.NotificationArabic = _template.NotificationArabic;

                                if (_template.EmailSubject.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.EmailSubject = _template.EmailSubject.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.EmailSubject = _template.EmailSubject;

                                if (_template.EmailBody.Contains("[ReceiverDisplayName]"))
                                    tempTemplate.EmailBody = _template.EmailBody.Replace("[ReceiverDisplayName]", user.DisplayName);
                                else
                                    tempTemplate.EmailBody = _template.EmailBody;

                                if (!receiver.IsCC)
                                {
                                    // Send Push Notification
                                    ReturnNotification = new Notification();
                                    _Notification = new Notification();
                                    _Notification.EnglishName = tempTemplate.NotificationEnglish;
                                    _Notification.ArabicName = tempTemplate.NotificationArabic;
                                    _Notification.NotificationType = NotificationType;
                                    _Notification.ItemType = "Delegation";
                                    _Notification.Created = DateTime.Now;
                                    _Notification.Modified = DateTime.Now;
                                    _Notification.RelatedItemID = DelegationID;
                                    _Notification.Status = (int)model.NotificationEnums.Pending;
                                    _Notification.AssignedTo = user.UserName;
                                    dataAccess.Notification.Save(_Notification);
                                    int result = dataAccess.Complete();
                                    if (result != 0)
                                        ReturnNotifications.Add(_Notification);
                                }
                                // Send Email
                                if (!receiver.IsCC)
                                {
                                    _EmailTemplate = new EmailTemplate();
                                    _EmailTemplate.To = user.Email;
                                    _EmailTemplate.CC = CC;
                                    _EmailTemplate.Body = tempTemplate.EmailBody;
                                    _EmailTemplate.Subject = tempTemplate.EmailSubject;

                                    var emailConfigurations = dataAccess.EmailConfiguration.GetAll().FirstOrDefault();
                                    #region get EmailTemplate From Configuration
                                    var EmailTemplate = GetConfiguration("EmailTemplate");
                                    string EmailBody = _EmailTemplate.Body;
                                    if (EmailTemplate != null)
                                    {
                                        EmailBody = EmailTemplate.Value.Replace("{emailbody}", _EmailTemplate.Body);
                                    }
                                    #endregion
                                    SendEmail(_EmailTemplate.To, _EmailTemplate.CC, _EmailTemplate.Subject, EmailBody, emailConfigurations);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                continue;
                            }

                        }
                    }
                }
            }
        }
        public NotificationConfiguration ResolveNotificationForDelegation(int DelegationID, NotificationConfiguration _template)
        {
            using (var dataAccess = _factory.Create())
            {
                Delegation delegation = dataAccess.Delegation.Query()
                    .IncludeOptimized(a => a.CreatedByModel)
                    .IncludeOptimized(a => a.ModifiedByModel)
                    .IncludeOptimized(a => a.DelegatedUser)
                    .IncludeOptimized(a => a.DelegatorUser)
                    .Where(s => s.ID == DelegationID).SingleOrDefault();
                List<Group> Groups = dataAccess.Group.Query()
                  .IncludeOptimized(a => a.UsersGroups)
                  .IncludeOptimizedByPath("UsersGroups.User")
                  .ToList();
                List<User> Users = dataAccess.User.GetAll().Where(w => !w.Deleted).ToList();
                List<User> _TempReceivers = new List<User>();

                #region Resolve Recievers
                foreach (NotificationReceiver _Reciever in _template.NotificationReceivers)
                {
                    if (_Reciever.Receiver == "ToDelegationUser")
                        _Reciever.Users.Add(delegation.DelegatedUser);
                    else if (_Reciever.Receiver == "FromDelegationUser")
                        _Reciever.Users.Add(delegation.DelegatorUser);
                    else
                    {
                        List<User> users = Groups.Where(a => a.Title.ToLower() == _Reciever.Receiver.ToLower()).SelectMany(s => s.UsersGroups.Select(x => x.User)).ToList();
                        foreach (User user in users)
                        {
                            _Reciever.Users.Add(user);
                        }
                    }
                }
                #endregion
                #region Resolve Parametrs
                foreach (NotificationParameter _Parameter in _template.NotificationParameters)
                {
                    if (_Parameter.Title.ToLower() == "[delegationdatefrom]")
                        _Parameter.Value = delegation.FromDate.Value.Date.ToString("dd-MMM-yyyy");

                    if (_Parameter.Title.ToLower() == "[delegationdateto]")
                        _Parameter.Value = delegation.ToDate.Value.Date.ToString("dd-MMM-yyyy");

                    if (_Parameter.Title.ToLower() == "[delegationfromdisplayname]")
                        _Parameter.Value = delegation.DelegatorUser.DisplayName;
                    if (_Parameter.Title.ToLower() == "[delegationtodisplayname]")
                        _Parameter.Value = delegation.DelegatedUser.DisplayName;

                    if (_Parameter.Title.ToLower() == "[delegationurl]")
                    {
                        string URL = dataAccess.Configuration.Query().Where(w => w.Key == "SPlusDelegationURL").FirstOrDefault()?.Value;
                        if (URL != null && URL.Contains("{ItemID}"))
                        {
                            URL = URL.Replace("{ItemID}", delegation.ID.ToString());
                        }
                        _Parameter.Value = URL;
                    }

                }
                _template = ReplaceText(_template);
                #endregion
            }
            return _template;
        }
        #endregion

        
        public NotificationConfiguration ReplaceText(NotificationConfiguration _template)
        {
            foreach (NotificationParameter _Parameter in _template.NotificationParameters)
            {
                _template.NotificationEnglish = _template.NotificationEnglish.Replace(_Parameter.Title, _Parameter.Value);
                _template.NotificationArabic = _template.NotificationArabic.Replace(_Parameter.Title, _Parameter.Value);  ////This was the issue in the notification test
                _template.EmailSubject = _template.EmailSubject.Replace(_Parameter.Title, _Parameter.Value);
                _template.EmailBody = _template.EmailBody.Replace(_Parameter.Title, _Parameter.Value);
            }
            return _template;
        }
        public void SendEmail(string To, List<string> CC, string Subject, string Body, EmailConfiguration emailConfigurations)
        {
            try
            {

                if (emailConfigurations != null)
                {
                    MailMessage message = new MailMessage();
                    //message.Body += eNemailBody;
                    //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
                    //  message.Body,
                    //  null, "text/html");
                    var From = emailConfigurations.SenderEmail;
                    var SMTP = emailConfigurations.SmtpServer;
                    var Port = emailConfigurations.SmtpPort;
                    var credentialsEmail = emailConfigurations.SenderEmail;
                    var enableSsl = emailConfigurations.EnableSsl;
                    var credentialsPassword = emailConfigurations.SenderPassword;

                    if (!string.IsNullOrWhiteSpace(To) && !string.IsNullOrWhiteSpace(From) && !string.IsNullOrWhiteSpace(SMTP) && !string.IsNullOrWhiteSpace(Port.ToString()) && !string.IsNullOrWhiteSpace(credentialsEmail) && !string.IsNullOrWhiteSpace(credentialsPassword))
                    {

                        using (MailMessage mm = new MailMessage(From, To))
                        {
                            mm.Subject = Subject;
                            mm.Body = Body;
                            mm.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = SMTP;
                            smtp.EnableSsl = enableSsl;
                            NetworkCredential NetworkCred = new NetworkCredential(credentialsEmail, Encryption.Decrypt(credentialsPassword));
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = NetworkCred;
                            smtp.Port = Port;
                            foreach (string ccmail in CC)
                            {
                                if (!string.IsNullOrWhiteSpace(ccmail))
                                    mm.CC.Add(ccmail);
                            }

                            smtp.Send(mm);

                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
            }

        }
        #endregion 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using SPlus.DataAccess;
using SPlus.Model.Domain;
using System.Data.Entity;
using model = SPlus.Model;
using Z.EntityFramework.Plus;

namespace SPlus.BLL
{
    public class KPITypeBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();


        private readonly IUnitOfWorkFactory _factory;
        public KPITypeBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }

        #region Create
        public KPIType Create(KPIType kpiType)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                List<KPIType> types = dataAccess.KPIType.GetAll().ToList();
                List<Workflow> workflows = dataAccess.Workflow.Query().Include(a => a.BaseWorkflow).ToList();

                int kpitypeid;
                int workflowid;


                if (types.Count() != 0)
                    kpitypeid = types.Max(m => m.KPITypeID) + 1;
                else
                    kpitypeid = 1;

                if (workflows.Count() != 0)
                    workflowid = workflows.Max(m => m.WorkflowID) + 1;
                else
                    workflowid = 1;
                kpiType.ReminderConfiguration.KPITypeID = kpitypeid;
                kpiType.KPITypeID = kpitypeid;
                foreach (var workflow in kpiType.Workflows)
                {
                    workflow.KPITypeID = kpitypeid;
                    workflow.Name = kpiType.EnglishName + "-" + Enum.GetName(typeof(model.EnumWFBaseWorkflows), workflow.BaseWorkflowID);
                    workflow.WorkflowID = workflowid;
                    workflow.Created = DateTime.Now;
                    workflow.Modified = DateTime.Now;
                    foreach (var step in workflow.WorkflowSteps)
                    {
                        step.Created = DateTime.Now;
                        step.Modified = DateTime.Now;
                        if (string.IsNullOrWhiteSpace(step.ArabicName))
                            step.ArabicName = step.EnglishName;
                        step.WorkflowID = workflowid;
                    }
                    workflowid++;
                }

                dataAccess.KPIType.Save(kpiType);
                result = dataAccess.Complete();
                Resource resource = new Resource();
                resource = new Resource
                {
                    Code = "Type",
                    IsScreen = false,
                    Name = kpiType.EnglishName,
                    PropertyName = "KPITypeID",
                    PropertyValue = kpiType.KPITypeID,
                    LevelID = (int)model.LevelTypeEnum.KPI
                };
                dataAccess.Resource.Save(resource);
                result = dataAccess.Complete();
                if (kpiType.Attachment != null)
                {
                    IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
                    List<Attachment> Attachments = new List<Attachment>();

                    Attachment attach = new Attachment();
                    attach = SourceAttachments.Where(w => w.ID == kpiType.Attachment.ID).FirstOrDefault();
                    if (attach != null)
                    {
                        attach.RelatedItemID = kpiType.KPITypeID;
                        attach.Type = typeof(KPIType).Name;
                        Attachments.Add(attach);
                        dataAccess.Attachment.Save(attach);
                    }
                }
                dataAccess.Complete();
                dataAccess.Dispose();
                return kpiType;
            }
        }
       
        #endregion

        #region Read
        public List<KPIType> Read()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPIType> kpiTypes = dataAccess.KPIType.Query()
                    .Include(a => a.ReminderConfiguration)
                    .Include(a => a.KPIThresholds.Select(s => s.Status))
                    .Include(a => a.KPIs)                    
                    .Include(a => a.Workflows.Select(s => s.WorkflowSteps));

                Status na = dataAccess.Status.Query().ToList().Where(a=>a.Code == "NA").FirstOrDefault();
                //Status fin = dataAccess.Status.Query().ToList().Where(a => a.Code == "FIN").FirstOrDefault();


                foreach (var kPIType in kpiTypes)
                {
                    KPIThreshold th = new KPIThreshold();
                    th.Status = na;

                    kPIType.KPIThresholds.Add(th);

                    //KPIThreshold fi = new KPIThreshold();
                    //fi.Status = fin;

                    //kPIType.KPIThresholds.Add(fi);

                    if (kPIType.KPIs.Count() > 0)
                    {
                        kPIType.IsDeletable = false;
                    }
                    else
                    {
                        kPIType.IsDeletable = true;
                    }
                }
                return kpiTypes.MapAttachment(dataAccess).ToList();
            }
        }
        public KPIType ReadByIDForKPIDetails(int id)
        {
            using (var dataAccess = _factory.Create())
            {
                KPIType kpiType = dataAccess.KPIType.Query()
                    .Include(a => a.ReminderConfiguration)
                    .Include(a => a.KPIThresholds.Select(s => s.Status))
                    .Include(a => a.Workflows.Select(s => s.WorkflowSteps))
                    .Include(a => a.KPIs)
                    .Where(s => s.KPITypeID == id).SingleOrDefault();

                Status na = dataAccess.Status.Query().ToList().Where(a => a.Code == "NA").FirstOrDefault();
                //Status fin = dataAccess.Status.Query().ToList().Where(a => a.Code == "FIN").FirstOrDefault();

                KPIThreshold th = new KPIThreshold();
                th.Status = na;

                kpiType.KPIThresholds.Add(th);  
                
                //KPIThreshold fi = new KPIThreshold();
                //fi.Status = fin;

                //kpiType.KPIThresholds.Add(fi);

                if (kpiType.KPIs.Count() > 0)
                {
                    kpiType.IsDeletable = false;
                }
                else
                {
                    kpiType.IsDeletable = true;
                }
                return kpiType.MapAttachment(dataAccess);
            }
        }
        public KPIType ReadByID(int id)
        {
            using (var dataAccess = _factory.Create())
            {
                KPIType kpiType = dataAccess.KPIType.Query()
                    .Include(a => a.ReminderConfiguration)
                    .Include(a => a.KPIThresholds.Select(s => s.Status))
                    .Include(a => a.Workflows.Select(s => s.WorkflowSteps))
                    .Include(a => a.KPIs)
                    .Where(s => s.KPITypeID == id).SingleOrDefault();

                if (kpiType.KPIs.Count() > 0)
                {
                    kpiType.IsDeletable = false;
                }
                else
                {
                    kpiType.IsDeletable = true;
                }
                return kpiType.MapAttachment(dataAccess);
            }
        }
        public List<Status> ReadStatuses()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<Status> statuses = dataAccess.Status.GetAll();
                return statuses.ToList();
            }
        }


        #endregion

        #region Update

        public KPIType Update(KPIType kpiType)
        {

            int result;
            List<Workflow> workflows = new List<Workflow>();
            List<WorkflowStep> workflowSteps = new List<WorkflowStep>();
            List<KPIThreshold> thresholds = new List<KPIThreshold>();
            KPIType type = null;
            using (var dataAccess = _factory.Create())
            {
                type = dataAccess.KPIType.Query()
                    .IncludeOptimized(a => a.Workflows)
                    .IncludeOptimized(a => a.KPIThresholds)
                    .IncludeOptimizedByPath("Workflows.WorkflowSteps")
                    .IncludeOptimized(a => a.ReminderConfiguration)
                    .Where(w => w.KPITypeID == kpiType.KPITypeID).FirstOrDefault();
                if (type.Workflows != null && type.Workflows.Count() > 0)
                {
                    workflows = type.Workflows.ToList();
                    workflowSteps = type.Workflows.SelectMany(s => s.WorkflowSteps.Where(w => w.WorkflowID == s.WorkflowID)).ToList();
                }
                foreach (var workflowStep in workflowSteps)
                {
                    if (!kpiType.Workflows.Any(e => e.WorkflowSteps.Select(s => s.ID).Contains(workflowStep.ID)))
                    {
                        dataAccess.WorkflowStep.Delete(workflowStep);
                    }

                }
                //workflowSteps = new List<WorkflowStep>();
                //workflowSteps = dataAccess.WorkflowStep.GetAll().ToList();
                foreach (var workflow in kpiType.Workflows)
                {
                    foreach (var step in workflow.WorkflowSteps)
                    {
                        if (workflowSteps.Any(e => e.WorkflowID == step.WorkflowID))
                        {
                            step.Created = DateTime.Now;
                            step.Modified = DateTime.Now;
                            if (string.IsNullOrWhiteSpace(step.ArabicName))
                                step.ArabicName = step.EnglishName;
                            dataAccess.WorkflowStep.Save(step);
                            continue;
                        }

                        var stepentity = workflowSteps.FirstOrDefault(e => e.ID == step.ID);
                        if (stepentity != null)
                        {
                            stepentity.Created = DateTime.Now;
                            stepentity.Modified = DateTime.Now;
                            if (string.IsNullOrWhiteSpace(step.ArabicName))
                                step.ArabicName = step.EnglishName;
                            stepentity.WorkflowID = workflow.WorkflowID;
                        }
                        else
                        {
                            stepentity = step;
                            stepentity.Modified = DateTime.Now;
                            if (string.IsNullOrWhiteSpace(step.ArabicName))
                                step.ArabicName = step.EnglishName;
                            stepentity.WorkflowID = workflow.WorkflowID;
                            dataAccess.WorkflowStep.Save(stepentity);
                        }
                    }
                }

                if (kpiType.Workflows != null && kpiType.Workflows.Count() > 0)
                {
                    foreach (var workflow in kpiType.Workflows)
                    {
                        if (!workflows.Any(e => e.WorkflowID == workflow.WorkflowID))
                        {
                            workflow.KPITypeID = null;
                        }
                    }
                    //workflows = new List<Workflow>();
                    //workflows = dataAccess.Workflow.GetAll().ToList();
                    foreach (var workflow in kpiType.Workflows)
                    {
                        if (workflows.Any(e => e.KPITypeID == workflow.KPITypeID))
                        {
                            workflow.Name = kpiType.EnglishName + "-" + Enum.GetName(typeof(model.EnumWFBaseWorkflows), workflow.BaseWorkflowID);
                            dataAccess.Workflow.Save(workflow);
                            continue;
                        }

                        var entity = workflows.FirstOrDefault(e => e.WorkflowID == workflow.WorkflowID);
                        if (entity != null)
                        {
                            entity.KPITypeID = kpiType.KPITypeID;
                        }
                    }
                }
                type.ReminderConfiguration.BeforeReminder = kpiType.ReminderConfiguration.BeforeReminder;
                type.ReminderConfiguration.FirstReminder = kpiType.ReminderConfiguration.FirstReminder;
                type.ReminderConfiguration.SecondReminder = kpiType.ReminderConfiguration.SecondReminder;
                dataAccess.ReminderConfiguration.Save(type.ReminderConfiguration); // Added Because the reminder weren't update from the KPI type object 

                foreach (var threshold in kpiType.KPIThresholds)
                {
                    var current = type.KPIThresholds.Where(w => w.KPITypeID == type.KPITypeID && w.Code == threshold.Code).FirstOrDefault();
                    current.Max = threshold.Max;
                    current.Min = threshold.Min;
                    current.MinOperator = threshold.MinOperator;
                    current.Operator = threshold.Operator;
                    dataAccess.KPIThreshold.Save(current);
                }

                dataAccess.KPIType.Save(kpiType);
                result = dataAccess.Complete();

                Resource resource = dataAccess.Resource.GetAll().Where(w => !w.IsScreen && w.PropertyValue == kpiType.KPITypeID && w.LevelID == (int)model.LevelTypeEnum.KPI).FirstOrDefault();
                if (resource != null)
                {
                    resource.Name = kpiType.EnglishName;
                }
                else
                {
                    resource = new Resource
                    {
                        Code = "Type",
                        IsScreen = false,
                        Name = kpiType.EnglishName,
                        PropertyName = "KPITypeID",
                        PropertyValue = kpiType.KPITypeID,
                        LevelID = (int)model.LevelTypeEnum.KPI
                    };
                }
                dataAccess.Resource.Save(resource);
                result = dataAccess.Complete();

                if (kpiType.Attachment != null)
                {
                    IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.GetAll().ToList();
                    var attachmentstodelete = type.MapAttachment(dataAccess).Attachment;

                    List<Attachment> Attachments = new List<Attachment>();

                    Attachment attach = new Attachment();
                    attach = SourceAttachments.Where(w => w.ID == kpiType.Attachment.ID).FirstOrDefault();
                    if (attach != null)
                    {
                        attach.RelatedItemID = kpiType.KPITypeID;
                        attach.Type = typeof(KPIType).Name;
                        Attachments.Add(attach);
                        dataAccess.Attachment.Save(attach);
                    }
                    if (attachmentstodelete != null)
                    {
                        if (!Attachments.Select(s => s.ID).Contains(attachmentstodelete.ID))
                        {
                            if (attach != null)
                            {
                                Task.Run(async () => { await DeleteAttachment(attachmentstodelete); });
                            }
                        }
                    }
                    result = dataAccess.Complete();
                }
                return kpiType;
            }
        }

        #endregion

        #region Delete
        private async Task<bool> DeleteAttachment(Attachment Attachment)
        {
            int result = 0;

            using (var dataAccess = _factory.Create())
            {
                List<Attachment> Attachments = await dataAccess.Attachment.Query().IncludeOptimized(a => a.Content).ToListAsync();
                Attachments = Attachments.Where(s => Attachment.ID == s.ID).ToList();
                foreach (var attach in Attachments)
                {
                    dataAccess.Attachment.Delete(attach);
                    result = dataAccess.Complete();
                }
                if (result >= 1)
                    return true;
                else
                    return false;
            }
        }
        public bool Delete(int id)
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                KPIType kpiType = dataAccess.KPIType.Query().Include(a => a.ReminderConfiguration).Where(w => w.KPITypeID == id).FirstOrDefault();
                if (kpiType != null)
                {
                    kpiType = kpiType.MapAttachment(dataAccess);
                    if (kpiType.Attachment != null)
                        Task.Run(() => DeleteAttachment(kpiType.Attachment));
                    dataAccess.KPIType.Delete(kpiType);
                    result = dataAccess.Complete();
                }
                if (result >= 1)
                {
                    Resource resource = dataAccess.Resource.Query().Where(w => !w.IsScreen && w.PropertyValue == id).FirstOrDefault();
                    if (resource != null)
                    {
                        dataAccess.Resource.Delete(resource);
                        result = dataAccess.Complete();
                    }
                    if (result >= 1)
                        return true;
                    else return false;
                }
                else
                    return false;
            }
        }

        #endregion
    }
}

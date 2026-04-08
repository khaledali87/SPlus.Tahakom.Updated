using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SPlus.BLL;
using SPlus.DTO;
using SPlus.Model;
using SPlus.Model.Domain;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPlus.UseCases
{
    public class TaskCenterUseCases : LoggingUseCases
    {

        Container _Container = IOC.InitializeContainer();
        private readonly KPIBLL KPIBLL;
        private readonly RequestBLL RequestBLL;
        private readonly UserBLL UserBLL;
        public TaskCenterUseCases()
        {
            KPIBLL = _Container.GetInstance<KPIBLL>();
            RequestBLL = _Container.GetInstance<RequestBLL>();
            UserBLL = _Container.GetInstance<UserBLL>();
        }

        #region Update KPI List
        public List<UpdateListDTO> GetKPIUpdates(string UserName)
        {
            List<KPI> kpis = KPIBLL.TaskCenterKPIs(UserName);
            var Updates = GetRequireUpdateKPIs(kpis.Where(a => a.CanUpdate).ToList());

            return Updates;
        }
        private List<UpdateListDTO> GetRequireUpdateKPIs(List<KPI> kpis)
        {
            List<UpdateListDTO> list = new List<UpdateListDTO>();
            foreach (var item in kpis)
            {
                UpdateListDTO listitem = new UpdateListDTO();
                listitem.ID = item.ID;
                listitem.Type = (int)LevelTypeEnum.KPI;
                listitem.EnglishName = item.EnglishName;
                listitem.ArabicName = item.ArabicName;
                listitem.DueDate = item.KPIMeasures.Where(a => a.IsActive).FirstOrDefault().DueDate;
                listitem.Status = item.KPIMeasures.Where(a => a.IsActive).FirstOrDefault().AccumulutiveStatus ?? "NA";
                listitem.UnitOfMeasure = item.UnitOfMeasure;
                listitem.DataSource = item.DataSource;
                listitem.EnglishUnitDetails = item.EnglishUnitDetails;
                listitem.ArabicUnitDetails = item.ArabicUnitDetails;
                listitem.IsLocked = item.IsLocked;
                listitem.Parameters = AutoMapper.Mapper.Map<List<ParameterDTO>>(item.Parameters);
                listitem.Measure = AutoMapper.Mapper.Map<List<KPIMeasureDTO>>(item.KPIMeasures.Where(a => a.AllowUpdate).ToList());
                listitem.ChampionModel = AutoMapper.Mapper.Map<UserListDTO>(item.ChampionModel);
                listitem.OwnerModel = AutoMapper.Mapper.Map<UserListDTO>(item.OwnerModel);
                //if (!list.Any(a => a.ID == listitem.ID))
                list.Add(listitem);
            }
            return list.OrderByDescending(o => o.DueDate).ToList();
        }
        #endregion

        #region My Requests
        public TaskCenterDTO<List<MyRequestDTO>> MyRequests(string UserName, string type)
        {
            TaskCenterDTO<List<MyRequestDTO>> taskCenter = new TaskCenterDTO<List<MyRequestDTO>>();
           var myTasks = RequestBLL.GetMyRequests(UserName);
          //  var myTasks = RequestBLL.GetRequests_new(UserName,false);

            
            List<KPI> kpis = KPIBLL.Read_LessData(UserName, null);
            List<User> users = UserBLL.Read();
            taskCenter = MyRequestList(myTasks, kpis, users, type);
            return taskCenter;

        }

    
        private TaskCenterDTO<List<MyRequestDTO>> MyRequestList(List<RequestStep> RequestSteps, List<KPI> kpis, List<User> users, string baseworkflow)
        {
            TaskCenterDTO<List<MyRequestDTO>> taskCenter = new TaskCenterDTO<List<MyRequestDTO>>();
            List<MyRequestDTO> MyRequestList = new List<MyRequestDTO>();
            //  MyRequestDTO myRequest = null;
            AttachmentBLL attachement = new AttachmentBLL();

            TaskCentreWFTypeEnum taskcentertype = (TaskCentreWFTypeEnum)Enum.Parse(typeof(TaskCentreWFTypeEnum), baseworkflow, true);

            var definition = new { BaseWorkflow = 0, Type = 0, ID = 0 , BaseWorkflowID = 0 };

            //var kpiIndex = kpis
            //    .SelectMany(k => k.KPIMeasures.Select(m => new { m.ID, KPI = k, Measure = m }))
            //    .ToDictionary(x => x.ID, x => x);





            List<MyRequestDTO> ApprovalList_Mitigationtmp = new List<MyRequestDTO>();
            List<MyRequestDTO> ApprovalList_WorkProctmp = new List<MyRequestDTO>();
            List<MyRequestDTO> ApprovalList_ReUpdatetmp = new List<MyRequestDTO>();

            var steps = RequestSteps.GroupBy(s => s.RequestID).Select(s => s.LastOrDefault()).ToList();

            foreach (var step in steps)
            {
                try
                {
                    if(step.RequestID == 4971)
                    {

                    }
                    var parsed = JsonConvert.DeserializeAnonymousType(step.Request.Form, definition);
                    int type = parsed.Type;
                    int? baseWF = parsed.BaseWorkflow;
                    int baseWFID = parsed.BaseWorkflowID;
                    MyRequestDTO myRequest = new MyRequestDTO
                    {
                        RequestID = step.RequestID,
                        BaseWorkflowID = baseWF ?? baseWFID,
                        Attachment = Mapper.Map<AttachmentDTO>(step?.Request?.Attachment)
                    };

                    // dynamic form = null;

                    UpdateKPIForm updateKPIForm = null;
                    KPIChangeRequestFormDTO kpiChangeRequestFormDTO = null;
                    CreateKPIFormDTO createKPIFormDTO = null;
                    MitigationActionRequestFormDTO mitigationActionRequestFormDTO = null;
                    // Deserialize ONCE based on workflow
                    switch (baseWF)
                    {
                        case (int)EnumWFBaseWorkflows.Update:
                        case 0:
                            if (type == (int)LevelTypeEnum.KPI)
                                updateKPIForm = JsonConvert.DeserializeObject<UpdateKPIForm>(step.Request.Form);
                            break;
                        case (int)EnumWFBaseWorkflows.ReUpdate:
                            if (type == (int)LevelTypeEnum.KPI)
                                updateKPIForm = JsonConvert.DeserializeObject<UpdateKPIForm>(step.Request.Form);

                            break;
                        case (int)EnumWFBaseWorkflows.KPIChangeRequest:
                            kpiChangeRequestFormDTO = JsonConvert.DeserializeObject<KPIChangeRequestFormDTO>(step.Request.Form);
                            break;

                        case (int)EnumWFBaseWorkflows.CreateKPI:
                            createKPIFormDTO = JsonConvert.DeserializeObject<CreateKPIFormDTO>(step.Request.Form);
                            break;

                        case (int)EnumWFBaseWorkflows.MitigationAction:
                            mitigationActionRequestFormDTO = JsonConvert.DeserializeObject<MitigationActionRequestFormDTO>(step.Request.Form);
                            break;
                    }


                    if (type == (int)LevelTypeEnum.KPI &&
                      (baseWF == (int)EnumWFBaseWorkflows.ReUpdate || baseWFID == (int)EnumWFBaseWorkflows.ReUpdate))
                    {
                        var pendingStep = RequestSteps
                      .Where(x => x.RequestID == step.RequestID
                               && x.Status == (int)EnumWFStatuses.Pending
                               && x.IsCancelled != true)
                                           .OrderByDescending(x => x.Order)
                                              .FirstOrDefault();
                        var previousStep = pendingStep != null ? RequestSteps
                      .Where(x => x.RequestID == pendingStep.RequestID
                               && x.Order < pendingStep.Order
                               && x.IsCancelled != true)
                      .OrderByDescending(x => x.Order)
                      .FirstOrDefault(): RequestSteps
                      .Where(x => x.RequestID == step.RequestID
                               && x.Order < step.Order
                               && x.IsCancelled != true)
                      .OrderByDescending(x => x.Order)
                      .FirstOrDefault();


                        int itemID = updateKPIForm.RelatedID;



                        var kpi = KPIBLL.GetKPIByMeasureID_All(itemID);
                        var measure = kpi.KPIMeasures.FirstOrDefault(x => x.ID == itemID);
                        //if (!kpiIndex.TryGetValue(itemID, out var row))
                        //    continue;

                        myRequest.ID = kpi.ID;
                        myRequest.EnglishName = kpi.EnglishName;
                        myRequest.ArabicName = kpi.ArabicName;
                        myRequest.Type = (int)LevelTypeEnum.KPI;
                        myRequest.Created = step.Request.Created;

                        myRequest.AccumulutiveTarget = measure?.AccumulutiveTarget;
                        myRequest.AccumulutiveValue = measure?.AccumulutiveValue;
                        myRequest.DueDate = measure?.DueDate;

                        myRequest.BaseWorkflowID = baseWFID;

                        myRequest.OldActualValue = updateKPIForm.OldValue;
                        myRequest.ActualValue = updateKPIForm.Value;

                        myRequest.ReviewedBy = previousStep.ActionBy != null ? Mapper.Map<UserListDTO>(users.Where(w => w.UserName.ToLower() == previousStep?.ActionBy.ToLower()).FirstOrDefault()) : null;
                        myRequest.CreatedBy = Mapper.Map<UserListDTO>(users.Where(w => w.UserName.ToLower() == step.Request.CreatedBy.ToLower()).FirstOrDefault());
                        myRequest.Status = pendingStep != null ? pendingStep.Status : step.Status != 0 ? step.Status : step.Request.Status;

                        ApprovalList_ReUpdatetmp.Add(myRequest);

                        if (taskcentertype == TaskCentreWFTypeEnum.ReUpdate || taskcentertype == TaskCentreWFTypeEnum.All)
                            MyRequestList.Add(myRequest);
                    }
                    //
                    // ******** Work Procedure (KPI Change Request / Create KPI) ********
                    //
                    if (type == (int)LevelTypeEnum.KPI && baseWF == (int)EnumWFBaseWorkflows.KPIChangeRequest)
                    {

                        KPI kpi = new KPI();

                        kpi = kpis.Where(w => w.ID == kpiChangeRequestFormDTO.ID).FirstOrDefault();
                        if (kpi is null)
                            continue;

                        myRequest.Created = kpiChangeRequestFormDTO.Created;
                        myRequest.CreatedBy = Mapper.Map<UserListDTO>(users.Where(w => w.UserName.ToLower() == kpiChangeRequestFormDTO.CreatedBy.ToLower()).FirstOrDefault());
                        myRequest.EnglishName = kpi.EnglishName;
                        myRequest.ArabicName = kpi.ArabicName;
                        myRequest.Type = (int)LevelTypeEnum.KPI;
                        myRequest.Status = step.Request.Status;
                        ApprovalList_WorkProctmp.Add(myRequest);

                        if (taskcentertype == TaskCentreWFTypeEnum.WorkProcedure || taskcentertype == TaskCentreWFTypeEnum.All)
                            MyRequestList.Add(myRequest);

                    }

                    if (type == (int)LevelTypeEnum.CreateKPI && baseWF == (int)EnumWFBaseWorkflows.CreateKPI)
                    {



                        myRequest.Created = createKPIFormDTO.Created;
                        myRequest.CreatedBy = Mapper.Map<UserListDTO>(users.Where(w => w.UserName.ToLower() == createKPIFormDTO.CreatedBy.ToLower()).FirstOrDefault());
                        myRequest.EnglishName = createKPIFormDTO.EnglishName;
                        myRequest.ArabicName = createKPIFormDTO.ArabicName;
                        myRequest.Type = (int)LevelTypeEnum.KPI;
                        myRequest.Status = step.Request.Status;
                        if (createKPIFormDTO.DivisionalObjective != null)
                        {
                            myRequest.BaseWorkflowID = (int)EnumWFBaseWorkflows.DepartmentalKPI;
                        }
                        else
                        {
                            myRequest.BaseWorkflowID = (int)EnumWFBaseWorkflows.StrategicKPI;
                        }


                        ApprovalList_WorkProctmp.Add(myRequest);

                        if (taskcentertype == TaskCentreWFTypeEnum.WorkProcedure || taskcentertype == TaskCentreWFTypeEnum.All)
                            MyRequestList.Add(myRequest);
                    }

                    //
                    // ******** Mitigation ********
                    //
                    if (type == (int)LevelTypeEnum.MitigationAction &&
                        baseWF == (int)EnumWFBaseWorkflows.MitigationAction)
                    {




                        myRequest.Created = mitigationActionRequestFormDTO.Created;
                        myRequest.CreatedBy = Mapper.Map<UserListDTO>(users.Where(w => w.UserName.ToLower() == mitigationActionRequestFormDTO.CreatedBy.ToLower()).FirstOrDefault());
                        myRequest.EnglishName = mitigationActionRequestFormDTO.KPI.EnglishName;
                        myRequest.ArabicName = mitigationActionRequestFormDTO.KPI.ArabicName;
                        myRequest.Type = (int)LevelTypeEnum.MitigationAction;
                        myRequest.Status = step.Request.Status;
                        myRequest.OrgStructure = mitigationActionRequestFormDTO.OrgStructure;
                        myRequest.OwnerModel = mitigationActionRequestFormDTO.KPI.OwnerModel;


                        ApprovalList_Mitigationtmp.Add(myRequest);

                        if (taskcentertype == TaskCentreWFTypeEnum.MitigationActions || taskcentertype == TaskCentreWFTypeEnum.All)
                            MyRequestList.Add(myRequest);
                    }
                }

                catch
                {
                    var rId = step.RequestID;
                    continue;
                }

            }


            taskCenter.WorkProcedureCount = ApprovalList_WorkProctmp.Count;
            taskCenter.ReUpdateCount = ApprovalList_ReUpdatetmp.Count;

            taskCenter.MitigationActionsCount = ApprovalList_Mitigationtmp.Count;
            taskCenter.Data = MyRequestList.OrderByDescending(o => o.Created).ToList();
            return taskCenter;
        }

        //AS
        private TaskCenterDTO<List<MyRequestDTO>> MyRequestList1(List<RequestStep> RequestSteps, List<KPI> kpis, List<User> users, string baseworkflow)
        {
            TaskCenterDTO<List<MyRequestDTO>> taskCenter = new TaskCenterDTO<List<MyRequestDTO>>();
            List<MyRequestDTO> MyRequestList = new List<MyRequestDTO>();
            MyRequestDTO myRequest = null;
            AttachmentBLL attachement = new AttachmentBLL();

            TaskCentreWFTypeEnum taskcentertype = (TaskCentreWFTypeEnum)Enum.Parse(typeof(TaskCentreWFTypeEnum), baseworkflow, true);

            var definition = new { BaseWorkflow = 0, Type = 0, ID = 0 };

            //taskCenter.WorkProcedureCount = RequestSteps.Where(w => JsonConvert.DeserializeAnonymousType(w.Request.Form, definition).BaseWorkflow == (int)EnumWFBaseWorkflows.KPIChangeRequest ||
            //(JsonConvert.DeserializeAnonymousType(w.Request.Form, definition).BaseWorkflow == (int)EnumWFBaseWorkflows.CreateKPI)).Count();
            //taskCenter.MitigationActionsCount = RequestSteps.Where(w => JsonConvert.DeserializeAnonymousType(w.Request.Form, definition).BaseWorkflow == (int)EnumWFBaseWorkflows.MitigationAction).Count();
            //taskCenter.KPICount = RequestSteps.Where(w => JsonConvert.DeserializeAnonymousType(w.Request.Form, definition).BaseWorkflow == 0).Count();


            List<MyRequestDTO> ApprovalList_Mitigationtmp = new List<MyRequestDTO>();
            List<MyRequestDTO> ApprovalList_WorkProctmp = new List<MyRequestDTO>();

            foreach (var step in RequestSteps)
            {
                myRequest = new MyRequestDTO();
                int Type = JsonConvert.DeserializeAnonymousType(step.Request.Form, definition).Type;
                int BaseWorkflow = JsonConvert.DeserializeAnonymousType(step.Request.Form, definition).BaseWorkflow;

                myRequest.RequestID = step.RequestID;
                myRequest.Attachment = Mapper.Map<AttachmentDTO>(step?.Request?.Attachment);
                myRequest.BaseWorkflowID = BaseWorkflow;
                if (taskcentertype == TaskCentreWFTypeEnum.WorkProcedure || taskcentertype == TaskCentreWFTypeEnum.All)
                {
                    if (Type == (int)LevelTypeEnum.KPI && BaseWorkflow == (int)EnumWFBaseWorkflows.KPIChangeRequest)
                    {
                        KPI kpi = new KPI();
                        KPIChangeRequestFormDTO form = JsonConvert.DeserializeObject<KPIChangeRequestFormDTO>(step.Request.Form);
                        kpi = kpis.Where(w => w.ID == form.ID).FirstOrDefault();
                        if (kpi is null)
                            continue;

                        myRequest.Created = form.Created;
                        myRequest.CreatedBy = Mapper.Map<UserListDTO>(users.Where(w => w.UserName.ToLower() == form.CreatedBy.ToLower()).FirstOrDefault());
                        myRequest.EnglishName = kpi.EnglishName;
                        myRequest.ArabicName = kpi.ArabicName;
                        myRequest.Type = (int)LevelTypeEnum.KPI;
                        myRequest.Status = step.Request.Status;

                        MyRequestList.Add(myRequest);
                    }

                    if (Type == (int)LevelTypeEnum.CreateKPI && BaseWorkflow == (int)EnumWFBaseWorkflows.CreateKPI)
                    {
                        CreateKPIFormDTO form = JsonConvert.DeserializeObject<CreateKPIFormDTO>(step.Request.Form);

                        myRequest.Created = form.Created;
                        myRequest.CreatedBy = Mapper.Map<UserListDTO>(users.Where(w => w.UserName.ToLower() == form.CreatedBy.ToLower()).FirstOrDefault());
                        myRequest.EnglishName = form.EnglishName;
                        myRequest.ArabicName = form.ArabicName;
                        myRequest.Type = (int)LevelTypeEnum.KPI;
                        myRequest.Status = step.Request.Status;
                        if (form.DivisionalObjective != null)
                        {
                            myRequest.BaseWorkflowID = (int)EnumWFBaseWorkflows.DepartmentalKPI;
                        }
                        else
                        {
                            myRequest.BaseWorkflowID = (int)EnumWFBaseWorkflows.StrategicKPI;
                        }

                        MyRequestList.Add(myRequest);
                    }
                }

                //WorkProcedure count
                if (Type == (int)LevelTypeEnum.KPI && BaseWorkflow == (int)EnumWFBaseWorkflows.KPIChangeRequest)
                {
                    KPI kpi = new KPI();
                    KPIChangeRequestFormDTO form = JsonConvert.DeserializeObject<KPIChangeRequestFormDTO>(step.Request.Form);
                    kpi = kpis.Where(w => w.ID == form.ID).FirstOrDefault();
                    if (kpi is null)
                        continue;

                    myRequest.Created = form.Created;
                    myRequest.CreatedBy = Mapper.Map<UserListDTO>(users.Where(w => w.UserName.ToLower() == form.CreatedBy.ToLower()).FirstOrDefault());
                    myRequest.EnglishName = kpi.EnglishName;
                    myRequest.ArabicName = kpi.ArabicName;
                    myRequest.Type = (int)LevelTypeEnum.KPI;
                    myRequest.Status = step.Request.Status;

                    ApprovalList_WorkProctmp.Add(myRequest);
                }

                if (Type == (int)LevelTypeEnum.CreateKPI && BaseWorkflow == (int)EnumWFBaseWorkflows.CreateKPI)
                {
                    CreateKPIFormDTO form = JsonConvert.DeserializeObject<CreateKPIFormDTO>(step.Request.Form);

                    myRequest.Created = form.Created;
                    myRequest.CreatedBy = Mapper.Map<UserListDTO>(users.Where(w => w.UserName.ToLower() == form.CreatedBy.ToLower()).FirstOrDefault());
                    myRequest.EnglishName = form.EnglishName;
                    myRequest.ArabicName = form.ArabicName;
                    myRequest.Type = (int)LevelTypeEnum.KPI;
                    myRequest.Status = step.Request.Status;
                    if (form.DivisionalObjective != null)
                    {
                        myRequest.BaseWorkflowID = (int)EnumWFBaseWorkflows.DepartmentalKPI;
                    }
                    else
                    {
                        myRequest.BaseWorkflowID = (int)EnumWFBaseWorkflows.StrategicKPI;
                    }

                    ApprovalList_WorkProctmp.Add(myRequest);
                }


                if (taskcentertype == TaskCentreWFTypeEnum.MitigationActions || taskcentertype == TaskCentreWFTypeEnum.All)
                {

                    if (Type == (int)LevelTypeEnum.MitigationAction && BaseWorkflow == (int)EnumWFBaseWorkflows.MitigationAction)
                    {

                        MitigationActionRequestFormDTO form = JsonConvert.DeserializeObject<MitigationActionRequestFormDTO>(step.Request.Form);


                        myRequest.Created = form.Created;
                        myRequest.CreatedBy = Mapper.Map<UserListDTO>(users.Where(w => w.UserName.ToLower() == form.CreatedBy.ToLower()).FirstOrDefault());
                        myRequest.EnglishName = form.KPI.EnglishName;
                        myRequest.ArabicName = form.KPI.ArabicName;
                        myRequest.Type = (int)LevelTypeEnum.MitigationAction;
                        myRequest.Status = step.Request.Status;
                        myRequest.OrgStructure = form.OrgStructure;
                        myRequest.OwnerModel = form.KPI.OwnerModel;
                        MyRequestList.Add(myRequest);
                    }
                }

                //Mitigation count
                if (Type == (int)LevelTypeEnum.MitigationAction && BaseWorkflow == (int)EnumWFBaseWorkflows.MitigationAction)
                {

                    MitigationActionRequestFormDTO form = JsonConvert.DeserializeObject<MitigationActionRequestFormDTO>(step.Request.Form);


                    myRequest.Created = form.Created;
                    myRequest.CreatedBy = Mapper.Map<UserListDTO>(users.Where(w => w.UserName.ToLower() == form.CreatedBy.ToLower()).FirstOrDefault());
                    myRequest.EnglishName = form.KPI.EnglishName;
                    myRequest.ArabicName = form.KPI.ArabicName;
                    myRequest.Type = (int)LevelTypeEnum.MitigationAction;
                    myRequest.Status = step.Request.Status;
                    myRequest.OrgStructure = form.OrgStructure;
                    myRequest.OwnerModel = form.KPI.OwnerModel;
                    ApprovalList_Mitigationtmp.Add(myRequest);
                }


            }


            taskCenter.WorkProcedureCount = ApprovalList_WorkProctmp.Count;


            taskCenter.MitigationActionsCount = ApprovalList_Mitigationtmp.Count;
            taskCenter.Data = MyRequestList.OrderByDescending(o => o.Created).ToList();
            return taskCenter;
        }
        #endregion

        #region Approvals
        public TaskCenterListDTO<List<ApprovalDTO>> GetApprovalList(string UserName, string type)
        {
            TaskCenterListDTO<List<ApprovalDTO>> taskCenter = new TaskCenterListDTO<List<ApprovalDTO>>();
            // var myTasks1 = RequestBLL.GetMyTasks(UserName, false);
            List<User> users = UserBLL.Read();


            var myTasks = RequestBLL.GetActiveRequests(UserName, false);
            List<KPI> kpis = KPIBLL.TaskCenterKPIs(UserName);
            taskCenter = GetApprovalList(myTasks, kpis, type, users);
            return taskCenter;

        }
        private TaskCenterListDTO<List<ApprovalDTO>> GetApprovalList(List<RequestStep> RequestSteps, List<KPI> kpis, string baseworkflow,List<User> users)
        {
            TaskCenterListDTO<List<ApprovalDTO>> taskCenter = new TaskCenterListDTO<List<ApprovalDTO>>();
            List<ApprovalDTO> ApprovalList = new List<ApprovalDTO>();
            List<ApprovalDTO> ApprovalList_Kpitmp = new List<ApprovalDTO>();
            List<ApprovalDTO> ApprovalList_Mitigationtmp = new List<ApprovalDTO>();
            List<ApprovalDTO> ApprovalList_WorkProctmp = new List<ApprovalDTO>();
            List<ApprovalDTO> ApprovalList_ReUpdatetmp = new List<ApprovalDTO>();
            // ApprovalDTO approval = null;
            AttachmentBLL attachement = new AttachmentBLL();
            TaskCentreWFTypeEnum taskcentertype = (TaskCentreWFTypeEnum)Enum.Parse(typeof(TaskCentreWFTypeEnum), baseworkflow, true);
           
            var definition = new { BaseWorkflow = 0, Type = 0, ID = 0 , BaseWorkflowID = 0 };

            var kpiIndex = kpis
                .SelectMany(k => k.KPIMeasures.Select(m => new { m.ID, KPI = k, Measure = m }))
                .ToDictionary(x => x.ID, x => x);


            var steps = RequestSteps.Where(x => x.CanApprove).ToList();

            foreach (var step in steps)
            {
                //approval = new ApprovalDTO();
                var parsed = JsonConvert.DeserializeAnonymousType(step.Request.Form, definition);
                int type = parsed.Type;
                int baseWF = parsed.BaseWorkflow;
                int baseWFID = parsed.BaseWorkflowID;
                ApprovalDTO approval = new ApprovalDTO
                {
                    RequestID = step.RequestID,
                    BaseWorkflowID = baseWF,
                    Type = type,
                Attachments = Mapper.Map<List<AttachmentDTO>>(step?.Request?.Attachments)
                };

                // dynamic form = null;

              


                UpdateKPIForm updateKPIForm = null;
                KPIChangeRequestFormDTO kpiChangeRequestFormDTO = null;
                CreateKPIFormDTO createKPIFormDTO = null;
                MitigationActionRequestFormDTO mitigationActionRequestFormDTO = null;
                // Deserialize ONCE based on workflow
                switch (baseWF)
                {
                    case (int)EnumWFBaseWorkflows.Update:
                    case (int)EnumWFBaseWorkflows.ReUpdate:
                    case 0:
                        if (type == (int)LevelTypeEnum.KPI)
                            updateKPIForm = JsonConvert.DeserializeObject<UpdateKPIForm>(step.Request.Form);
                        break;

                    case (int)EnumWFBaseWorkflows.KPIChangeRequest:
                        kpiChangeRequestFormDTO = JsonConvert.DeserializeObject<KPIChangeRequestFormDTO>(step.Request.Form);
                        break;

                    case (int)EnumWFBaseWorkflows.CreateKPI:
                        createKPIFormDTO = JsonConvert.DeserializeObject<CreateKPIFormDTO>(step.Request.Form);
                        break;

                    case (int)EnumWFBaseWorkflows.MitigationAction:
                        mitigationActionRequestFormDTO = JsonConvert.DeserializeObject<MitigationActionRequestFormDTO>(step.Request.Form);
                        break;
                }
                //
                // ******** KPI ********
                //
                if (type == (int)LevelTypeEnum.KPI &&
                   (baseWF == 0 || baseWF == (int)EnumWFBaseWorkflows.Update))
                {
                    int itemID = updateKPIForm.RelatedID;

                    if (!kpiIndex.TryGetValue(itemID, out var row))
                        continue;

                    approval.ID = row.KPI.ID;
                    approval.EnglishName = row.KPI.EnglishName;
                    approval.ArabicName = row.KPI.ArabicName;
                    approval.DueDate = row.Measure.DueDate;
                    approval.Status = row.KPI.KPIMeasures.FirstOrDefault(a => a.IsActive)?.AccumulutiveStatus ?? "NA";

                    ApprovalList_Kpitmp.Add(approval);

                    if (taskcentertype == TaskCentreWFTypeEnum.KPI || taskcentertype == TaskCentreWFTypeEnum.All)
                        ApprovalList.Add(approval);
                }

                if (type == (int)LevelTypeEnum.KPI &&
                   (baseWF == (int)EnumWFBaseWorkflows.ReUpdate || baseWFID == (int)EnumWFBaseWorkflows.ReUpdate))
                {

                    var previousStep = RequestSteps
                  .Where(x => x.RequestID == step.RequestID
                           && x.Order < step.Order
                           && x.IsCancelled != true)
                  .OrderByDescending(x => x.Order)
                  .FirstOrDefault();

                    int itemID = updateKPIForm.RelatedID;

                    var kpi = kpis.FirstOrDefault(x => x.KPIMeasures.Any(m => m.ID == itemID));
                    var measure = kpi.KPIMeasures.FirstOrDefault(x => x.ID == itemID);

                    approval.ID = kpi.ID;
                    approval.EnglishName = kpi.EnglishName;
                    approval.ArabicName = kpi.ArabicName;
                    approval.DueDate = measure?.DueDate;
                    approval.Created = step?.Request.Created;

                    approval.DueDate = measure.DueDate;
                    approval.BaseWorkflowID = baseWFID;

                    approval.AccumulutiveTarget = measure.AccumulutiveTarget;
                    approval.AccumulutiveValue = measure.AccumulutiveValue;

                    approval.OldActualValue = updateKPIForm.OldValue;
                    approval.ActualValue = updateKPIForm.Value;
                    approval.ReviewedBy = Mapper.Map<UserListDTO>(users.FirstOrDefault(w => w.UserName.ToLower() == previousStep.ActionBy.ToLower()));
                    approval.CreatedBy = Mapper.Map<UserListDTO>(users.FirstOrDefault(w => w.UserName.ToLower() == step?.Request?.CreatedBy.ToLower()));



                    approval.Status = measure?.AccumulutiveStatus ?? "NA";

                    ApprovalList_ReUpdatetmp.Add(approval);

                    if (taskcentertype == TaskCentreWFTypeEnum.ReUpdate || 
                        taskcentertype == TaskCentreWFTypeEnum.All)
                        ApprovalList.Add(approval);
                }

                //
                // ******** Work Procedure (KPI Change Request / Create KPI) ********
                //
                if (type == (int)LevelTypeEnum.KPI && baseWF == (int)EnumWFBaseWorkflows.KPIChangeRequest)
                {
                    approval.ID = kpiChangeRequestFormDTO.ID;
                    approval.EnglishName = kpiChangeRequestFormDTO.EnglishName;
                    approval.ArabicName = kpiChangeRequestFormDTO.ArabicName;
                    approval.Created = kpiChangeRequestFormDTO.Created;
                    approval.Status = step.Status.ToString();
                    approval.CreatedBy = Mapper.Map<UserListDTO>(kpiChangeRequestFormDTO.CreatedBy.MapUser());

                    ApprovalList_WorkProctmp.Add(approval);

                    if (taskcentertype == TaskCentreWFTypeEnum.WorkProcedure || taskcentertype == TaskCentreWFTypeEnum.All)
                        ApprovalList.Add(approval);
                }

                if (type == (int)LevelTypeEnum.CreateKPI && baseWF == (int)EnumWFBaseWorkflows.CreateKPI)
                {
                    approval.ID = createKPIFormDTO.ID;
                    approval.EnglishName = createKPIFormDTO.EnglishName;
                    approval.ArabicName = createKPIFormDTO.ArabicName;
                    approval.Created = createKPIFormDTO.Created;
                    approval.DueDate = createKPIFormDTO.KPIMeasures.OrderBy(w => w.DueDate).Select(x => x.DueDate).LastOrDefault();
                    approval.Status = step.Status.ToString();
                    approval.CreatedBy = Mapper.Map<UserListDTO>(createKPIFormDTO.CreatedBy.MapUser());

                    approval.BaseWorkflowID =
                        createKPIFormDTO.DivisionalObjective != null ?
                        (int)EnumWFBaseWorkflows.DepartmentalKPI :
                        (int)EnumWFBaseWorkflows.StrategicKPI;

                    ApprovalList_WorkProctmp.Add(approval);

                    if (taskcentertype == TaskCentreWFTypeEnum.WorkProcedure || taskcentertype == TaskCentreWFTypeEnum.All)
                        ApprovalList.Add(approval);
                }

                //
                // ******** Mitigation ********
                //
                if (type == (int)LevelTypeEnum.MitigationAction &&
                    baseWF == (int)EnumWFBaseWorkflows.MitigationAction)
                {
                    approval.ID = mitigationActionRequestFormDTO.ID;
                    approval.EnglishName = mitigationActionRequestFormDTO.KPI.EnglishName;
                    approval.ArabicName = mitigationActionRequestFormDTO.KPI.ArabicName;
                    approval.OwnerModel = mitigationActionRequestFormDTO.KPI.OwnerModel;
                    approval.Status = step.Status.ToString();
                    approval.Created = mitigationActionRequestFormDTO.Created;
                    approval.OrgStructure = mitigationActionRequestFormDTO.OrgStructure;
                    approval.CreatedBy = Mapper.Map<UserListDTO>(mitigationActionRequestFormDTO.CreatedBy.MapUser());

                    ApprovalList_Mitigationtmp.Add(approval);

                    if (taskcentertype == TaskCentreWFTypeEnum.MitigationActions || taskcentertype == TaskCentreWFTypeEnum.All)
                        ApprovalList.Add(approval);
                }


                
            }





            //
            // ******** Final counts (always returned) ********
            //
            taskCenter.KPICount = ApprovalList_Kpitmp.Count;
            taskCenter.WorkProcedureCount = ApprovalList_WorkProctmp.Count;
            taskCenter.MitigationActionsCount = ApprovalList_Mitigationtmp.Count;
            taskCenter.ReUpdateCount = ApprovalList_ReUpdatetmp.Count;
            taskCenter.AllCount = ApprovalList_Kpitmp.Count + ApprovalList_WorkProctmp.Count + ApprovalList_ReUpdatetmp.Count + ApprovalList_Mitigationtmp.Count;

            //
            // ******** sort & return ********
            //
            taskCenter.Data = ApprovalList
                .OrderByDescending(o => o.Created.HasValue ? o.Created : o.DueDate)
                .ToList();




            return taskCenter;
        }


        //ASS
        private TaskCenterListDTO<List<ApprovalDTO>> GetApprovalList1(List<RequestStep> RequestSteps, List<KPI> kpis, string baseworkflow)
        {
            TaskCenterListDTO<List<ApprovalDTO>> taskCenter = new TaskCenterListDTO<List<ApprovalDTO>>();
            List<ApprovalDTO> ApprovalList = new List<ApprovalDTO>();
            List<ApprovalDTO> ApprovalList_Kpitmp = new List<ApprovalDTO>();
            List<ApprovalDTO> ApprovalList_Mitigationtmp = new List<ApprovalDTO>();
            List<ApprovalDTO> ApprovalList_WorkProctmp = new List<ApprovalDTO>();
            ApprovalDTO approval = null;
            AttachmentBLL attachement = new AttachmentBLL();
            TaskCentreWFTypeEnum taskcentertype = (TaskCentreWFTypeEnum)Enum.Parse(typeof(TaskCentreWFTypeEnum), baseworkflow, true);
            var definition = new { BaseWorkflow = 0, Type = 0, ID = 0 };

            //taskCenter.KPICount = RequestSteps.Where(w => JsonConvert.DeserializeAnonymousType(w.Request.Form, definition).BaseWorkflow == 0).Count();
            //taskCenter.WorkProcedureCount = RequestSteps.Where(w => JsonConvert.DeserializeAnonymousType(w.Request.Form, definition).BaseWorkflow == (int)EnumWFBaseWorkflows.KPIChangeRequest ||
            //(JsonConvert.DeserializeAnonymousType(w.Request.Form, definition).BaseWorkflow == (int)EnumWFBaseWorkflows.CreateKPI)).Count();
            //taskCenter.MitigationActionsCount = RequestSteps.Where(w => JsonConvert.DeserializeAnonymousType(w.Request.Form, definition).BaseWorkflow == (int)EnumWFBaseWorkflows.MitigationAction).Count();
            //taskCenter.AllCount = taskCenter.KPICount;



            foreach (var step in RequestSteps)
            {
                approval = new ApprovalDTO();
                int Type = JsonConvert.DeserializeAnonymousType(step.Request.Form, definition).Type;
                int BaseWorkflow = JsonConvert.DeserializeAnonymousType(step.Request.Form, definition).BaseWorkflow;

                approval.RequestID = step.RequestID;
                approval.Type = Type;
                approval.Attachments = Mapper.Map<List<AttachmentDTO>>(step?.Request?.Attachments);
                approval.BaseWorkflowID = BaseWorkflow;

                if (taskcentertype == TaskCentreWFTypeEnum.KPI || taskcentertype == TaskCentreWFTypeEnum.All)
                {
                    if ((Type == (int)LevelTypeEnum.KPI && BaseWorkflow == 0) || (Type == (int)LevelTypeEnum.KPI && BaseWorkflow == (int)EnumWFBaseWorkflows.Update))
                    {
                        KPIMeasure measure;
                        KPI kpi = new KPI();
                        measure = new KPIMeasure();
                        UpdateKPIForm form = JsonConvert.DeserializeObject<UpdateKPIForm>(step.Request.Form);
                        int itemID = form.RelatedID;
                        kpi = kpis.Where(s => s.KPIMeasures.Any(w => w.ID == itemID)).FirstOrDefault();
                        if (kpi is null)
                            continue;
                        measure = kpi.KPIMeasures.Where(w => w.ID == itemID).FirstOrDefault();
                        approval.ID = kpi.ID;
                        approval.EnglishName = kpi.EnglishName;
                        approval.ArabicName = kpi.ArabicName;
                        approval.DueDate = measure.DueDate;
                        approval.Status = kpi.KPIMeasures.Where(a => a.IsActive).FirstOrDefault().AccumulutiveStatus ?? "NA";
                        ApprovalList.Add(approval);
                    }
                }
                //Count  KPI
                if ((Type == (int)LevelTypeEnum.KPI && BaseWorkflow == 0) || (Type == (int)LevelTypeEnum.KPI && BaseWorkflow == (int)EnumWFBaseWorkflows.Update))
                {
                    KPIMeasure measure;
                    KPI kpi = new KPI();
                    measure = new KPIMeasure();
                    UpdateKPIForm form = JsonConvert.DeserializeObject<UpdateKPIForm>(step.Request.Form);
                    int itemID = form.RelatedID;
                    kpi = kpis.Where(s => s.KPIMeasures.Any(w => w.ID == itemID)).FirstOrDefault();
                    if (kpi is null)
                        continue;
                    measure = kpi.KPIMeasures.Where(w => w.ID == itemID).FirstOrDefault();
                    approval.ID = kpi.ID;
                    approval.EnglishName = kpi.EnglishName;
                    approval.ArabicName = kpi.ArabicName;
                    approval.DueDate = measure.DueDate;
                    approval.Status = kpi.KPIMeasures.Where(a => a.IsActive).FirstOrDefault().AccumulutiveStatus ?? "NA";
                    ApprovalList_Kpitmp.Add(approval);
                }


                if (taskcentertype == TaskCentreWFTypeEnum.WorkProcedure || taskcentertype == TaskCentreWFTypeEnum.All)
                {
                    if (Type == (int)LevelTypeEnum.KPI && BaseWorkflow == (int)EnumWFBaseWorkflows.KPIChangeRequest)
                    {
                        KPIChangeRequestFormDTO form = JsonConvert.DeserializeObject<KPIChangeRequestFormDTO>(step.Request.Form);
                        int itemID = form.ID;
                        approval.ID = form.ID;
                        approval.EnglishName = form.EnglishName;
                        approval.ArabicName = form.ArabicName;
                        approval.Status = step.Status.ToString();
                        approval.Created = form.Created;
                        approval.CreatedBy = AutoMapper.Mapper.Map<UserListDTO>(form.CreatedBy.MapUser());

                        ApprovalList.Add(approval);
                    }
                    if (Type == (int)LevelTypeEnum.CreateKPI && BaseWorkflow == (int)EnumWFBaseWorkflows.CreateKPI)
                    {
                        CreateKPIFormDTO form = JsonConvert.DeserializeObject<CreateKPIFormDTO>(step.Request.Form);
                        int itemID = form.ID;

                        approval.ID = form.ID;
                        approval.EnglishName = form.EnglishName;
                        approval.ArabicName = form.ArabicName;
                        approval.Status = step.Status.ToString();
                        approval.DueDate = form.KPIMeasures.OrderBy(w => w.DueDate).Select(s => s.DueDate).LastOrDefault();
                        approval.Created = form.Created;
                        approval.CreatedBy = AutoMapper.Mapper.Map<UserListDTO>(form.CreatedBy.MapUser());

                        if (form.DivisionalObjective != null)
                        {
                            approval.BaseWorkflowID = (int)EnumWFBaseWorkflows.DepartmentalKPI;
                        }
                        else
                        {
                            approval.BaseWorkflowID = (int)EnumWFBaseWorkflows.StrategicKPI;
                        }
                        ApprovalList.Add(approval);
                    }
                }

                //Count Work Proc
                if (Type == (int)LevelTypeEnum.KPI && BaseWorkflow == (int)EnumWFBaseWorkflows.KPIChangeRequest)
                {
                    KPIChangeRequestFormDTO form = JsonConvert.DeserializeObject<KPIChangeRequestFormDTO>(step.Request.Form);
                    int itemID = form.ID;
                    approval.ID = form.ID;
                    approval.EnglishName = form.EnglishName;
                    approval.ArabicName = form.ArabicName;
                    approval.Status = step.Status.ToString();
                    approval.Created = form.Created;
                    approval.CreatedBy = AutoMapper.Mapper.Map<UserListDTO>(form.CreatedBy.MapUser());

                    ApprovalList_WorkProctmp.Add(approval);
                }
                if (Type == (int)LevelTypeEnum.CreateKPI && BaseWorkflow == (int)EnumWFBaseWorkflows.CreateKPI)
                {
                    CreateKPIFormDTO form = JsonConvert.DeserializeObject<CreateKPIFormDTO>(step.Request.Form);
                    int itemID = form.ID;

                    approval.ID = form.ID;
                    approval.EnglishName = form.EnglishName;
                    approval.ArabicName = form.ArabicName;
                    approval.Status = step.Status.ToString();
                    approval.DueDate = form.KPIMeasures.OrderBy(w => w.DueDate).Select(s => s.DueDate).LastOrDefault();
                    approval.Created = form.Created;
                    approval.CreatedBy = AutoMapper.Mapper.Map<UserListDTO>(form.CreatedBy.MapUser());

                    if (form.DivisionalObjective != null)
                    {
                        approval.BaseWorkflowID = (int)EnumWFBaseWorkflows.DepartmentalKPI;
                    }
                    else
                    {
                        approval.BaseWorkflowID = (int)EnumWFBaseWorkflows.StrategicKPI;
                    }
                    ApprovalList_WorkProctmp.Add(approval);
                }

                if (taskcentertype == TaskCentreWFTypeEnum.MitigationActions || taskcentertype == TaskCentreWFTypeEnum.All)
                {
                    if (Type == (int)LevelTypeEnum.MitigationAction && BaseWorkflow == (int)EnumWFBaseWorkflows.MitigationAction)
                    {
                        MitigationActionRequestFormDTO form = JsonConvert.DeserializeObject<MitigationActionRequestFormDTO>(step.Request.Form);
                        int itemID = form.ID;
                        approval.ID = form.ID;
                        approval.EnglishName = form.KPI.EnglishName;
                        approval.ArabicName = form.KPI.ArabicName;

                        approval.OwnerModel = form.KPI.OwnerModel;


                        approval.Status = step.Status.ToString();
                        approval.Created = form.Created;
                        approval.OrgStructure = form.OrgStructure;
                        approval.CreatedBy = AutoMapper.Mapper.Map<UserListDTO>(form.CreatedBy.MapUser());

                        ApprovalList.Add(approval);
                    }

                }

                //Count Mitigation 
                if (Type == (int)LevelTypeEnum.MitigationAction && BaseWorkflow == (int)EnumWFBaseWorkflows.MitigationAction)
                {
                    MitigationActionRequestFormDTO form = JsonConvert.DeserializeObject<MitigationActionRequestFormDTO>(step.Request.Form);
                    int itemID = form.ID;
                    approval.ID = form.ID;
                    approval.EnglishName = form.KPI.EnglishName;
                    approval.ArabicName = form.KPI.ArabicName;

                    approval.OwnerModel = form.KPI.OwnerModel;


                    approval.Status = step.Status.ToString();
                    approval.Created = form.Created;
                    approval.OrgStructure = form.OrgStructure;
                    approval.CreatedBy = AutoMapper.Mapper.Map<UserListDTO>(form.CreatedBy.MapUser());

                    ApprovalList_Mitigationtmp.Add(approval);
                }
            }






            taskCenter.WorkProcedureCount = ApprovalList_WorkProctmp.Count;
            taskCenter.MitigationActionsCount = ApprovalList_Mitigationtmp.Count;
            taskCenter.AllCount = ApprovalList_Kpitmp.Count;
            taskCenter.KPICount = ApprovalList_Kpitmp.Count;
            taskCenter.Data = ApprovalList.OrderByDescending(o => o.Created.HasValue ? o.Created : o.DueDate).ToList();
            return taskCenter;
        }

        #endregion

        #region Commented
        //public TaskCenterDTO GetTaskCenter(string UserName)
        //{
        //    TaskCenterDTO taskCenter = new TaskCenterDTO();
        //    var myTasks = RequestBLL.GetMyTasks(UserName);
        //    List<KPI> kpis = KPIBLL.TaskCenterKPIs(UserName);            
        //    taskCenter.Updates = GetRequireUpdateKPIs(kpis);
        //    taskCenter.Approvals = GetApprovalList(myTasks, kpis);
        //    return taskCenter;

        //}
        //private List<ApprovalDTO> GetApprovalList(List<RequestStep> WFRequestSteps, List<KPI> kpis)
        //{
        //    List<RequestStepDTO> RequestSteps = AutoMapper.Mapper.Map<List<RequestStepDTO>>(WFRequestSteps);
        //    List<ApprovalDTO> ApprovalList = new List<ApprovalDTO>();
        //    List<KPIDTO> DTOkpis = AutoMapper.Mapper.Map<List<KPIDTO>>(kpis);
        //    ApprovalDTO approval = null;
        //    AttachmentBLL attachement = new AttachmentBLL();
        //    KPI kpi;
        //    KPIMeasure measure;
        //    foreach (var step in WFRequestSteps)
        //    {
        //        approval = new ApprovalDTO();
        //        approval.RequestID = step.RequestID;
        //        approval.Attachments = AutoMapper.Mapper.Map<List<AttachmentDTO>>(step?.Request?.Attachments);
        //        var data = (JObject)JsonConvert.DeserializeObject(step.Request.Form);
        //        int Type = data.SelectToken("Type").Value<int>();

        //        approval.Type = Type;
        //        if (Type == (int)LevelTypeEnum.KPI)
        //        {
        //            kpi = new KPI();
        //            measure = new KPIMeasure();
        //            UpdateKPIForm form = JsonConvert.DeserializeObject<UpdateKPIForm>(step.Request.Form);
        //            int measureID = form.RelatedID;
        //            kpi = kpis.Where(s => s.KPIMeasures.Any(w => w.ID == measureID)).FirstOrDefault();
        //            if (kpi == null)
        //                continue;
        //            measure = kpi.KPIMeasures.Where(w => w.ID == measureID).FirstOrDefault();
        //            approval.ID = kpi.ID;
        //            approval.EnglishName = kpi.EnglishName;
        //            approval.ArabicName = kpi.ArabicName;
        //            approval.DueDate = measure.DueDate;
        //            approval.Status = kpi.KPIMeasures.Where(a => a.IsActive).FirstOrDefault().AccumulutiveStatus ?? "NA";
        //        }
        //        else
        //        {
        //            continue;
        //        }
        //        ApprovalList.Add(approval);
        //    }

        //    return ApprovalList;
        //}

        #endregion

    }
}

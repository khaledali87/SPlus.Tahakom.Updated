using AutoMapper;
using Dangl.Calculator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SPlus.BLL;
using SPlus.DTO;
using SPlus.Helper;
using SPlus.Model;
using SPlus.Model.Domain;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
namespace SPlus.UseCases
{
    public class RequestUseCases : LoggingUseCases
    {

        Container _Container = IOC.InitializeContainer();
        private readonly RequestBLL RequestBLL;
        private readonly KPIBLL KPIBLL;
        private readonly KPITypeBLL KPITypeBLL;
        private readonly NotificationConfigurationBLL NotificationConfigurationBLL;
        private readonly UserBLL UserBLL;
        private readonly ParameterBLL ParameterBLL;
        private readonly AttachmentBLL AttachmentBLL;
        private readonly OrgStructureBLL OrgStructureBLL;
        private readonly bool IncludeFullSync = Convert.ToBoolean(ConfigurationManager.AppSettings["IncludeFullSync"]);

        WeightUseCases _WeightUseCases = new WeightUseCases();
        public RequestUseCases()
        {
            RequestBLL = _Container.GetInstance<RequestBLL>();
            KPIBLL = _Container.GetInstance<KPIBLL>();
            KPITypeBLL = _Container.GetInstance<KPITypeBLL>();
            NotificationConfigurationBLL = _Container.GetInstance<NotificationConfigurationBLL>();
            UserBLL = _Container.GetInstance<UserBLL>();
            ParameterBLL = _Container.GetInstance<ParameterBLL>();
            AttachmentBLL = _Container.GetInstance<AttachmentBLL>();
            OrgStructureBLL = _Container.GetInstance<OrgStructureBLL>();

        }



        #region Forms

        #region WFFormUpdateKPI

        public List<RequestDTO> GetUpdateKPIRequestsByKPIID(int kpiID, string userName)
        {

            KPI kpi = KPIBLL.ReadByID(kpiID, userName);
            List<Request> final = new List<Request>();
            List<RequestDTO> dtos = new List<RequestDTO>();
            if (kpi != null)
            {
                if (kpi.KPIType.Workflows.Where(a => a.BaseWorkflowID == (int)EnumWFBaseWorkflows.Update).Count() > 0)
                {

                    int workflowID = kpi.KPIType.Workflows.Where(a => a.BaseWorkflowID == (int)EnumWFBaseWorkflows.Update).FirstOrDefault().WorkflowID;
                    if (kpi != null)
                    {
                        List<Request> requests = RequestBLL.GetRequests(userName).Where(a => a.WorkflowID == workflowID || a.WorkflowID == 103).ToList();
                        foreach (Request request in requests)
                        {
                            var data = (JObject)JsonConvert.DeserializeObject(request.Form);
                            int Type = data.SelectToken("Type").Value<int>();
                            if (Type == (int)LevelTypeEnum.KPI)
                            {
                                UpdateKPIForm Form = JsonConvert.DeserializeObject<UpdateKPIForm>(request.Form);
                                if (kpi.KPIMeasures.Any(a => a.ID == Form.RelatedID))
                                {
                                    final.Add(request);
                                }
                            }
                        }

                        foreach (Request request in final)
                        {
                            RequestDTO dto = new RequestDTO();
                            dto = AutoMapper.Mapper.Map<RequestDTO>(request);
                            dto.RequestedBy = request.CreatedBy;
                            UpdateKPIForm form = JsonConvert.DeserializeObject<UpdateKPIForm>(request.Form);
                            if (form.Attachment != null && form.Attachments == null)
                            {
                                form.Attachments = new List<Attachment>();
                                form.Attachments.Add(form.Attachment);
                            }
                          
                            form.Value = form.Value.FormatDecimal();
                            form.Target = kpi.KPIMeasures.Where(a => a.ID == form.RelatedID).Select(s => s.Target).FirstOrDefault();
                            form.DueDate = kpi.KPIMeasures.Where(a => a.ID == form.RelatedID).Select(s => s.DueDate).FirstOrDefault();
                            dto.Form = form;

                            dtos.Add(dto);

                        }

                        return dtos.GroupBy(x=> ((UpdateKPIForm)x.Form).RelatedID)
                                   .Select(g=> g.OrderByDescending(x=> x.ID).FirstOrDefault())
                                   .OrderBy(a => ((UpdateKPIForm)a.Form).RelatedID)
                                   .ToList();
                    }
                }
            }
            return new List<RequestDTO>();
        }


        #endregion

        #endregion

        #region Request

        public RequestDetailsDTO GetRequestByID(int requestID, string userName)
        {
            var Request = RequestBLL.GetRequestByID(requestID, userName);
            RequestDetailsDTO request = AutoMapper.Mapper.Map<RequestDetailsDTO>(Request);
            dynamic form = JsonConvert.DeserializeObject(Request.Form);
            request.Form = form;


            List<WFHistoryDTO> WFHistoryDTO = new List<WFHistoryDTO>();
            //Add Creator as Save draft history
            //var CreatedByProperty = "CreatedByModel";
            //var propertyInfo = request.Form.GetType().GetProperty(CreatedByProperty);
            //var value = propertyInfo.GetValue(request.Form, null);

            var definition = new { IsDraft = false, CreatedByModel = new UserDTO(), Created = string.Empty, RequestId = 0 };



            var RequestWithReturnStep = RequestBLL.GetRequestByIDWithReturn(requestID, userName);
            RequestDetailsDTO requestWithReturnStep = AutoMapper.Mapper.Map<RequestDetailsDTO>(RequestWithReturnStep);
            WFHistoryDTO.Add(new DTO.WFHistoryDTO
            {
                ActionByModel = JsonConvert.DeserializeAnonymousType(Request.Form, definition).CreatedByModel,
                ActionDate = Convert.ToDateTime(JsonConvert.DeserializeAnonymousType(Request.Form, definition).Created).Date.ToString("dd/MM/yyyy"),
                ActionTime = Convert.ToDateTime(JsonConvert.DeserializeAnonymousType(Request.Form, definition).Created).ToString("HH:mm"),
                // EnglishStatus = "Saved as draft",
                //ArabicStatus = "حفظ كمسودة",
                Status = (int)EnumWFStatuses.SaveAsDraft,
                Comment = string.Empty,
                ArabicName = "منشئ الطلب",
                EnglishName = "Requester",

            });

            if (requestWithReturnStep.Steps != null && requestWithReturnStep.Steps.Count() > 0)
            {

                foreach (var item in requestWithReturnStep.Steps.Where(a => a.Status != 0 && a.Status != 1))
                {
                    WFHistoryDTO.Add(new DTO.WFHistoryDTO
                    {
                        ActionByModel = item.ActionByModel,
                        ActionDate = item.Modified.Date.ToString("dd/MM/yyyy"),
                        ActionTime = item.Modified.ToString("HH:mm"),
                        Status = item.Order == 1 && (JsonConvert.DeserializeAnonymousType(Request.Form, definition).IsDraft == false || Request.ID > 0) ?
                                    (int)EnumWFStatuses.Submitted
                                    : Request.WorkflowID == Convert.ToInt32(ConfigurationManager.AppSettings["MitigationActionWorkflowID"])
                                    ? item.Order == 3 && item.Status == (int)EnumWFStatuses.Approved
                                    ? (int)EnumWFStatuses.Closed
                                    : item.Status
                                    : item.Status,
                        Comment = item.Comments,
                        ArabicName = item.ArabicName, //item.Status == 5 ? "منشئ الطلب" : item.ArabicName,
                        EnglishName = item.EnglishName, //item.Status == 5 ? "Requester" : item.EnglishName
                        ClosedDate = item.Order == 3 && Request.WorkflowID == Convert.ToInt32(ConfigurationManager.AppSettings["MitigationActionWorkflowID"]) && request.Status == (int)EnumWFStatuses.Completed ? item.Modified.Date.ToString("dd/MM/yyyy") : ""
                    });
                }

            }

            request.WFHistory = WFHistoryDTO;
            return request;

        }



        public List<RequestDetailsDTO> ReadSubmitedChangeRequest()
        {
            List<RequestDetailsDTO> allRequests = new List<RequestDetailsDTO>();
            var requests = RequestBLL.GetAllRequests().Where(a => a.WorkflowID == Convert.ToInt32(ConfigurationManager.AppSettings["KPIChangeRequestWorkflowID"])).ToList();
            allRequests = AutoMapper.Mapper.Map<List<RequestDetailsDTO>>(requests);
            foreach (var item in requests)
            {
                dynamic form = JsonConvert.DeserializeObject(item.Form);
                allRequests.Where(a => a.ID == item.ID).FirstOrDefault().Form = form;
            }


            return allRequests;

        }


        public List<RequestDetailsDTO> ReportChangeRequests(string userName)
        {
            List<RequestDetailsDTO> allRequests = new List<RequestDetailsDTO>();
            var requests = RequestBLL.GetAllRequests().Where(a => a.WorkflowID == Convert.ToInt32(ConfigurationManager.AppSettings["KPIChangeRequestWorkflowID"])).ToList();
            List<Group> UserGroups = UserBLL.ReadGroupByUserName(userName.ToLower());
            if (!UserGroups.Any(a => a.Title?.ToLower() == Constants._CPMSupervisorGroup?.ToLower() || a.Title?.ToLower() == Constants._CPMDirectorGroup?.ToLower() || a.Title?.ToLower() == Constants._AdminGroup?.ToLower()))
            {
                var definition = new { CreatedBy = string.Empty };
                requests = requests.Where(a => JsonConvert.DeserializeAnonymousType(a.Form, definition).CreatedBy?.ToLower() == userName?.ToLower()).ToList();
            }
            allRequests = AutoMapper.Mapper.Map<List<RequestDetailsDTO>>(requests);
            foreach (var item in requests)
            {
                dynamic form = JsonConvert.DeserializeObject(item.Form);
                allRequests.Where(a => a.ID == item.ID).FirstOrDefault().Form = form;
            }



            return allRequests;

        }

        public RequestDetailsDTO GetRequestByIDUpdateKPIForm(int requestID, string userName)
        {
            var Request = RequestBLL.GetRequestByID(requestID, userName);
            var data = (JObject)JsonConvert.DeserializeObject(Request.Form);
            int Type = data.SelectToken("Type").Value<int>();
            int RelatedID = data.SelectToken("RelatedID").Value<int>();
            int BaseWorkflowID = data.SelectToken("BaseWorkflowID").Value<int>();

            List<WFHistoryDTO> list = new List<WFHistoryDTO>();
            if (Type == (int)LevelTypeEnum.KPI)
            {
               
                var Requests = RequestBLL.GetRequestsByFormRelatedIDCapital(RelatedID, Type);

                var mainRequest = Requests.Any(x => x.Status == (int)EnumWFStatuses.Pending) ? 
                                           Requests.Where(x=> x.Status == (int)EnumWFStatuses.Pending)
                                          .OrderByDescending(x=> x.ID)
                                          .FirstOrDefault() : Request;

                UpdateKPIForm form = JsonConvert.DeserializeObject<UpdateKPIForm>(mainRequest.Form);

                KPI kpi = KPIBLL.GetKPIByMeasureID(form.RelatedID, userName);
                KPIMeasure measure = kpi.KPIMeasures.FirstOrDefault(m => m.ID == form.RelatedID);
                RequestDetailsDTO requestDTO = AutoMapper.Mapper.Map<RequestDetailsDTO>(mainRequest);

                if (kpi != null)
                {
                    //if (kpi.KPIMeasures.Where(a => a.Status != "NA").Count() > 0)
                    //    form.OldValue = kpi.KPIMeasures.Where(a => a.Status != "NA").FirstOrDefault().Value.Value;
                    //else
                    //    form.OldValue = kpi.Baseline;

                    if (kpi.KPIMeasures.Min(x => x.ID) == form.RelatedID)
                        form.OldValue = kpi.Baseline;
                    else
                        form.OldValue = kpi.KPIMeasures.Where(w => w.ID < form.RelatedID && w.Status != "NA").LastOrDefault()?.Value ?? default;

                    form.Target = measure.Target;
                    form.Value = form.Value.FormatDecimal();//kpi.KPIMeasures.Where(a => a.ID == form.RelatedID).FirstOrDefault().Value.Value.FormatDecimal();
                    form.DueDate = measure.DueDate;
                    form.UnitOfMeasure = kpi.UnitOfMeasure;
                    form.EnglishUnitDetails = kpi.EnglishUnitDetails;
                    form.ArabicUnitDetails = kpi.ArabicUnitDetails;

                }



                requestDTO.Form = form;
                requestDTO.Related = new List<RequestDetailsDTO>();

                foreach (var req in Requests.Where(r=> r.ID != mainRequest.ID))
                {
                    RequestDetailsDTO related = AutoMapper.Mapper.Map<RequestDetailsDTO>(req);

                    UpdateKPIForm relatedForm = JsonConvert.DeserializeObject<UpdateKPIForm>(req.Form);

                    if (kpi != null)
                    {
                        //if (kpi.KPIMeasures.Where(a => a.Status != "NA").Count() > 0)
                        //    form.OldValue = kpi.KPIMeasures.Where(a => a.Status != "NA").FirstOrDefault().Value.Value;
                        //else
                        //    form.OldValue = kpi.Baseline;

                        var step = related.Steps.OrderBy(x => x.ID).FirstOrDefault(x=> x.Status == (int)EnumWFStatuses.Pending);

                        if (kpi.KPIMeasures.Min(x => x.ID) == form.RelatedID)
                            relatedForm.OldValue = kpi.Baseline;
                        else
                            relatedForm.OldValue = kpi.KPIMeasures.Where(w => w.ID < form.RelatedID && w.Status != "NA").LastOrDefault()?.Value ?? default;

                        relatedForm.Target = measure.Target;
                        relatedForm.Value = relatedForm.Value.FormatDecimal();//kpi.KPIMeasures.Where(a => a.ID == form.RelatedID).FirstOrDefault().Value.Value.FormatDecimal();
                        relatedForm.DueDate = measure.DueDate;
                        relatedForm.UnitOfMeasure = kpi.UnitOfMeasure;
                        relatedForm.EnglishUnitDetails = kpi.EnglishUnitDetails;
                        relatedForm.ArabicUnitDetails = kpi.ArabicUnitDetails;
                        relatedForm.AccumulutiveTarget = measure.AccumulutiveTarget;
                        relatedForm.AccumulutiveValue = measure.AccumulutiveValue;
                        relatedForm.ActionByModel = step?.ActionByModel;
                        relatedForm.ActionDate = step?.Modified;
                        related.Steps = new List<RequestStepDTO>();
                    }

                    related.Form = relatedForm;
                    
                    requestDTO.Related.Add(related);
                }

               

                return requestDTO;
            }
            else
            {
                return new RequestDetailsDTO();
            }
        }


        public void CorrectValue()
        {

            var requests = RequestBLL.GetRequest_All();
            CreateException("ahmad", "ahmad", requests.Count.ToString(), "");
            int count = 0;
            foreach (var request in requests)
            {

                CreateException("ahmad", "form", request.Form, "");
                UpdateKPIForm form = JsonConvert.DeserializeObject<UpdateKPIForm>(request.Form);

                if (form.BaseWorkflow == 0 || form.BaseWorkflow == 1)
                {
                    UpdateKPIForm updateKPIForm = form;

                    //Update KPI Parameters And Parameters Value Tables
                    //if (form.Parameters != null && form.Parameters.Count() > 0)
                    // ParameterBLL.UpdateParameterValues(form.RelatedID, form.Parameters);

                    CreateException("ahmad", "form", "Start", "");
                    KPIBLL.UpdateKPIPeriod_Script(updateKPIForm);
                    count = count + 1;
                }
            }
            CreateException("ahmad", "count", count + "", "");
        }


        public int DoAction(ActionDTO Action, string[] Credential)
        {

            string UserName;
            string DelegationUserName;
            if (Credential.Count() != 3)
            {
                //UserName = Credential[0];
                if (Credential[2] != "")
                    UserName = Credential[2];
                else
                    UserName = Credential[0];
                DelegationUserName = Credential[0];
            }
            else
            {
                if (Credential[2] != "")
                    UserName = Credential[2];
                else
                    UserName = Credential[0];
                DelegationUserName = Credential[0];
            }

            var result = RequestBLL.DoAction(Action.RequestID, Action.Action, Action.Comments, UserName, DelegationUserName, Action.QualityType);
            if (result > 0)
            {
                var request = RequestBLL.GetRequestByID(Action.RequestID, UserName);

                var workflow = RequestBLL.GetWorkflow(request.WorkflowID);
                switch (result)
                {
                    case (int)EnumWFStatuses.Approved:
                        switch (workflow.BaseWorkflowID)
                        {
                            case (int)EnumWFBaseWorkflows.Update:
                                {
                                    UpdateKPIForm form = JsonConvert.DeserializeObject<UpdateKPIForm>(request.Form);
                                    UpdateKPIForm updateKPIForm = form;
                                    KPI kpi = KPIBLL.GetKPIByMeasureID(form.RelatedID, UserName);
                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationWorkflow(kpi.ID, form.RelatedID, request.ID, enumNotificationEventType.Approved, LevelTypeEnum.KPI));
                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationWorkflow(kpi.ID, form.RelatedID, request.ID, enumNotificationEventType.Submit, LevelTypeEnum.KPI));
                                    break;
                                }

                            case (int)EnumWFBaseWorkflows.KPIChangeRequest:
                                {
                                    KPIChangeRequestFormDTO form = JsonConvert.DeserializeObject<KPIChangeRequestFormDTO>(request.Form);
                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForLevel(form.ID, LevelTypeEnum.KPI, request.ID, enumNotificationEventType.ChangeRequestAction));
                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForLevel(form.ID, LevelTypeEnum.KPI, request.ID, enumNotificationEventType.ChangeRequestSubmit));
                                    break;
                                }

                            case (int)EnumWFBaseWorkflows.CreateKPI:
                                {
                                    CreateKPIFormDTO form = JsonConvert.DeserializeObject<CreateKPIFormDTO>(request.Form);
                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForLevel(0, LevelTypeEnum.KPI, request.ID, enumNotificationEventType.CreateLevelAction));
                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForLevel(0, LevelTypeEnum.KPI, request.ID, enumNotificationEventType.CreateLevelSubmit));
                                    break;
                                }
                            case (int)EnumWFBaseWorkflows.MitigationAction:
                                {
                                    MitigationActionRequestFormDTO form = JsonConvert.DeserializeObject<MitigationActionRequestFormDTO>(request.Form);
                                    form.Reasons = Action.Reasons;
                                    form.RequestID = Action.RequestID;
                                    RequestBLL.UpdateMitigationActionRequest(form);
                                    if (Action.Attachments != null)
                                    {
                                        List<Attachment> attachments = AutoMapper.Mapper.Map<List<Attachment>>(Action.Attachments);
                                        AttachmentBLL.CreateRequestAttachment(attachments, Action.RequestID, false);
                                    }
                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForMitigiationPlan(LevelTypeEnum.MitigationAction, request.ID, enumNotificationEventType.MitigationPlanResponse, UserName));

                                    break;
                                }
                        }
                        break;

                    case (int)EnumWFStatuses.Completed:
                        switch (workflow.BaseWorkflowID)
                        {
                            case (int)EnumWFBaseWorkflows.Update:
                            case (int)EnumWFBaseWorkflows.ReUpdate:
                                {
                                    UpdateKPIForm form = JsonConvert.DeserializeObject<UpdateKPIForm>(request.Form);
                                    UpdateKPIForm updateKPIForm = form;
                                    KPI kpi = KPIBLL.GetKPIByMeasureID(form.RelatedID, UserName);

                                    if (form.IsSkipped)
                                    {
                                        KPIBLL.SetMeasureIsSkipped(form.RelatedID, form.IsSkipped);
                                    }
                                    else
                                    {
                                        //Update KPI Parameters And Parameters Value Tables
                                        if (form.Parameters != null && form.Parameters.Count() > 0)
                                            ParameterBLL.UpdateParameterValues(form.RelatedID, form.Parameters);
                                        KPIBLL.UpdateKPIPeriod(updateKPIForm);
                                    }
                                  
                                    kpi = KPIBLL.GetKPIByMeasureID(form.RelatedID, UserName);
                                    var updatedMeasure = kpi.KPIMeasures.Where(w => w.ID == form.RelatedID).FirstOrDefault();
                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationWorkflow(kpi.ID, form.RelatedID, request.ID, enumNotificationEventType.Completed, LevelTypeEnum.KPI));
                                    break;
                                }
                            case (int)EnumWFBaseWorkflows.KPIChangeRequest:
                                {
                                    KPIChangeRequestFormDTO form = JsonConvert.DeserializeObject<KPIChangeRequestFormDTO>(request.Form);
                                    KPI kpi = Mapper.Map<KPI>(form);


                                    kpi = KPIBLL.KPIChangeRequest(kpi, UserName, form.ChangeType);

                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForLevel(kpi.ID, LevelTypeEnum.KPI, request.ID, enumNotificationEventType.ChangeRequestFinalApproval));

                                }
                                break;

                            case (int)EnumWFBaseWorkflows.MitigationAction:
                                {
                                    //MitigationActionRequestFormDTO form = JsonConvert.DeserializeObject<MitigationActionRequestFormDTO>(request.Form);
                                    // form.Reasons = Action.Reasons;
                                    //form.RequestID = Action.RequestID;
                                    //  RequestBLL.UpdateMitigationActionRequest(form);
                                    if (Action.Attachments != null)
                                    {
                                        List<Attachment> attachments = AutoMapper.Mapper.Map<List<Attachment>>(Action.Attachments);
                                        AttachmentBLL.CreateRequestAttachment(attachments, Action.RequestID, false);
                                    }
                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForMitigiationPlan(LevelTypeEnum.MitigationAction, request.ID, enumNotificationEventType.CreateMitigationPlanAction, UserName));

                                }
                                break;

                            case (int)EnumWFBaseWorkflows.CreateKPI:
                                {
                                    CreateKPIFormDTO form = JsonConvert.DeserializeObject<CreateKPIFormDTO>(request.Form);

                                    KPI kpi = Mapper.Map<KPI>(form);


                                    kpi = KPIBLL.Create(kpi, UserName);

                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForLevel(kpi.ID, LevelTypeEnum.KPI, request.ID, enumNotificationEventType.CreateKPIFinalApproval));

                                }
                                break;
                        }
                        break;
                    case (int)EnumWFStatuses.Rejected:
                        switch (workflow.BaseWorkflowID)
                        {
                            case (int)EnumWFBaseWorkflows.Update:
                                {
                                    UpdateKPIForm form = JsonConvert.DeserializeObject<UpdateKPIForm>(request.Form);
                                    UpdateKPIForm updateKPIForm = form;
                                    KPI kpi = KPIBLL.GetKPIByMeasureID(form.RelatedID, UserName);
                                    KPIBLL.UpdateMeasureAllowUpdate(updateKPIForm.RelatedID, true);
                                    kpi = KPIBLL.GetKPIByMeasureID(form.RelatedID, UserName);
                                    KPIBLL.Update_ManualLock(kpi);
                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationWorkflow(kpi.ID, form.RelatedID, request.ID, enumNotificationEventType.Rejected, LevelTypeEnum.KPI));
                                    break;
                                }
                            case (int)EnumWFBaseWorkflows.KPIChangeRequest:
                                {
                                    KPIChangeRequestFormDTO form = JsonConvert.DeserializeObject<KPIChangeRequestFormDTO>(request.Form);
                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForLevel(form.ID, LevelTypeEnum.KPI, request.ID, enumNotificationEventType.ChangeRequestAction));
                                    break;
                                }

                            case (int)EnumWFBaseWorkflows.MitigationAction:
                                {
                                    MitigationActionRequestFormDTO form = JsonConvert.DeserializeObject<MitigationActionRequestFormDTO>(request.Form);
                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForMitigiationPlan(LevelTypeEnum.MitigationAction, request.ID, enumNotificationEventType.CreateMitigationPlanAction, UserName));
                                    break;
                                }
                            case (int)EnumWFBaseWorkflows.CreateKPI:
                                {
                                    CreateKPIFormDTO form = JsonConvert.DeserializeObject<CreateKPIFormDTO>(request.Form);
                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForLevel(form.ID, LevelTypeEnum.KPI, request.ID, enumNotificationEventType.CreateLevelAction));
                                    break;
                                }


                        }
                        break;
                    case (int)EnumWFStatuses.Return:
                        switch (workflow.BaseWorkflowID)
                        {

                            case (int)EnumWFBaseWorkflows.KPIChangeRequest:
                                {
                                    KPIChangeRequestFormDTO form = JsonConvert.DeserializeObject<KPIChangeRequestFormDTO>(request.Form);
                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForLevel(form.ID, LevelTypeEnum.KPI, request.ID, enumNotificationEventType.ChangeRequestAction));
                                    break;
                                }

                            case (int)EnumWFBaseWorkflows.MitigationAction:
                                {
                                    MitigationActionRequestFormDTO form = JsonConvert.DeserializeObject<MitigationActionRequestFormDTO>(request.Form);

                                    form.Reasons = Action.Reasons;
                                    if (!string.IsNullOrWhiteSpace(Action.Reasons))
                                    {
                                        // RequestBLL.UpdateMitigationActionRequest(form);
                                        if (Action.Attachments != null)
                                        {
                                            List<Attachment> attachments = AutoMapper.Mapper.Map<List<Attachment>>(Action.Attachments);
                                            AttachmentBLL.CreateRequestAttachment(attachments, Action.RequestID, false);
                                        }
                                    }

                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForMitigiationPlan(LevelTypeEnum.MitigationAction, request.ID, enumNotificationEventType.MitigationPlanReturn, UserName));
                                    break;
                                }

                            case (int)EnumWFBaseWorkflows.CreateKPI:
                                {
                                    CreateKPIFormDTO form = JsonConvert.DeserializeObject<CreateKPIFormDTO>(request.Form);
                                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForLevel(form.ID, LevelTypeEnum.KPI, request.ID, enumNotificationEventType.CreateLevelAction));
                                    break;
                                }
                        }
                        break;
                }
            }

            return result;
        }

        #endregion

        public bool UpdateKPIValue(List<SaveWFFormUpdateKPIDTO> forms, string[] Credential)
        {

            string UserName;
            string DelegationUserName;
            if (Credential.Count() != 4)
            {
                //UserName = Credential[0];
                if (Credential[2] != "")
                    UserName = Credential[2];
                else
                    UserName = Credential[0];
                DelegationUserName = Credential[0];
            }
            else
            {
                if (Credential[2] != "")
                    UserName = Credential[2];
                else
                    UserName = Credential[0];
                DelegationUserName = Credential[0];
            }

            KPI kpi = KPIBLL.GetKPIByMeasureID(forms.FirstOrDefault().RelatedID, UserName);

            List<KPIType> types = KPITypeBLL.Read();
            int requestID = 0;
            int currentRequestID = 0;
            if (kpi.Champion.ToLower() == UserName.ToLower() || kpi.Champion.ToLower() == DelegationUserName.ToLower())
            {
                if (kpi != null)
                {
                    if (kpi.KPIType != null && kpi.KPIType.Workflows.Count() > 0)
                    {
                        int workflowID = kpi.KPIType.Workflows.Where(a => a.BaseWorkflowID == (int)EnumWFBaseWorkflows.Update).FirstOrDefault().WorkflowID;
                        List<Group> groups = UserBLL.ReadGroup();
                        forms = forms.OrderBy(o => o.RelatedID).ToList();
                        foreach (SaveWFFormUpdateKPIDTO form in forms)
                        {
                            

                            string formula = kpi.Formula;


                            #region Calculate Semi-Auto Value
                            if (kpi.DataSource == "semi-auto" || kpi.DataSource == "auto")
                            {
                                //kpi.Parameters= kpi.Parameters.ToList().Sort((a, b) => b.ParameterName.Length.CompareTo(a.ParameterName.Length));
                                if (kpi.Parameters != null && kpi.Parameters.Count() > 0 && form.Parameters != null && form.Parameters.Count() > 0)
                                {
                                    form.Parameters.Sort((a, b) => b.ParameterName.Length.CompareTo(a.ParameterName.Length));
                                    foreach (var parameter in form.Parameters)
                                    {
                                        if (parameter.IsConstant)
                                        {
                                            formula = formula.Replace(parameter.ParameterName, kpi.Parameters.Where(a => a.ParameterName == parameter.ParameterName).FirstOrDefault()?.ConstantValue.ToString());
                                        }
                                        else
                                            formula = formula.Replace(parameter.ParameterName, parameter.Value.ToString());
                                    }
                                }
                                else
                                {
                                    throw new System.Exception("This KPI Does not have Parameters While it's Semi-Auto.");
                                }
                                
                                form.Value = CalculateFormula(formula);
                                //form.Value = KPIBLL.CalculatePeriodActual(kpi.KPIMeasures.FirstOrDefault(f => f.ID == form.RelatedID), kpi, true);
                            }

                            #endregion

                            form.Type = (int)LevelTypeEnum.KPI;
                            //form.BaseWorkflowID = (int)EnumWFBaseWorkflows.Update;

                            form.Value = form.Value.TrimDecimal();
                            string formPayload = JsonConvert.SerializeObject(form);

                            List<Request> currentRequests = RequestBLL.GetRequestsByFormRelatedID(form.RelatedID, LevelTypeEnum.KPI, EnumWFBaseWorkflows.Update); //RequestBLL.GetRequestsByFormRelatedID(form.RelatedID, (int)LevelTypeEnum.KPI);
                            if (currentRequests != null && currentRequests.Count() > 0)
                            {
                                currentRequestID = currentRequests.LastOrDefault().ID;
                                KPIBLL.UpdateMeasureAllowUpdate(form.RelatedID, false);
                                requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, true);
                                AttachmentBLL.DeleteOldRequestAttachment(requestID);
                            }

                            else
                            {
                                Request request = new Request();
                                if (form.RelatedID == forms.FirstOrDefault().RelatedID)
                                {
                                    //First measure
                                    KPIBLL.UpdateMeasureAllowUpdate(form.RelatedID, false);
                                    var currentKPI = KPIBLL.GetKPIByMeasureID(form.RelatedID, UserName);
                                    bool IsActive = true;
                                    if (currentKPI != null)
                                    {
                                        var KPIRequests = RequestBLL.GetLevelPendingRequest(currentKPI.KPIMeasures.Select(s => s.ID).ToList(), LevelTypeEnum.KPI);
                                        if (KPIRequests.Any())
                                        {
                                            IsActive = false;
                                            requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, IsActive, KPIRequests.LastOrDefault().ID);
                                        }
                                        else
                                        {
                                            requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, IsActive);
                                        }
                                    }
                                    else
                                    {
                                        requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, IsActive);
                                    }
                                }
                                else
                                {
                                    currentRequestID = 0;
                                    KPIBLL.UpdateMeasureAllowUpdate(form.RelatedID, false);
                                    var currentKPI = KPIBLL.GetKPIByMeasureID(form.RelatedID, UserName);
                                    bool IsActive = true;
                                    if (currentKPI != null)
                                    {
                                        var KPIRequests = RequestBLL.GetLevelPendingRequest(currentKPI.KPIMeasures.Select(s => s.ID).ToList(), LevelTypeEnum.KPI);
                                        if (KPIRequests.Any())
                                        {
                                            IsActive = false;
                                            requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, IsActive, KPIRequests.LastOrDefault().ID);
                                        }
                                        else
                                        {
                                            requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, IsActive);
                                        }
                                    }
                                    else
                                    {
                                        requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, false, requestID);
                                    }
                                }

                            }






                            // Request request = new Request();

                            //form.Value = form.Value.TrimDecimal();

                            //if (form.RelatedID == forms.FirstOrDefault().RelatedID)
                            //{
                            //    //First measure
                            //    KPIBLL.UpdateMeasureAllowUpdate(form.RelatedID, false);
                            //    requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, true);
                            //}
                            //else
                            //{
                            //    KPIBLL.UpdateMeasureAllowUpdate(form.RelatedID, false);
                            //    requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, false, requestID);
                            //}
                            if (form.Attachments != null)
                            {
                                //Attachment attachment = AutoMapper.Mapper.Map<List<Attachment>>(form.Attachment);
                                //AttachmentBLL.CreateRequestAttachment(attachment, requestID);
                                List<Attachment> attachments = AutoMapper.Mapper.Map<List<Attachment>>(form.Attachments);
                                AttachmentBLL.CreateRequestAttachment_List(attachments, requestID);
                            }
                        }

                        ValidateRequestSequance(kpi);
                    }
                    else
                    {
                        //Reflect to measure immediatly as no workflow available
                        foreach (SaveWFFormUpdateKPIDTO form in forms)
                        {
                            form.Type = (int)LevelTypeEnum.KPI;

                            // var Form = AutoMapper.Mapper.Map<UpdateKPIForm>(form);
                            UpdateKPIForm updateKPIForm = new UpdateKPIForm();
                            updateKPIForm.RelatedID = form.RelatedID;
                            updateKPIForm.OldValue = form.OldValue;
                            updateKPIForm.Target = form.Target;
                            updateKPIForm.Value = form.Value;
                            KPIBLL.UpdateKPIPeriod(updateKPIForm);
                        }
                    }
                    Task.Run(() => NotificationConfigurationBLL.SendNotificationWorkflow(kpi.ID, 0, requestID, enumNotificationEventType.Submit, LevelTypeEnum.KPI));
                }
            }
            else
            {
                throw new System.Exception("Unauthorized user");
            }

            return true;
        }

        public bool ReUpdateKPIValue(List<SaveWFFormUpdateKPIDTO> forms, string[] Credential)
        {

            string UserName;
            string DelegationUserName;
            if (Credential.Count() != 4)
            {
                //UserName = Credential[0];
                if (Credential[2] != "")
                    UserName = Credential[2];
                else
                    UserName = Credential[0];
                DelegationUserName = Credential[0];
            }
            else
            {
                if (Credential[2] != "")
                    UserName = Credential[2];
                else
                    UserName = Credential[0];
                DelegationUserName = Credential[0];
            }

            KPI kpi = KPIBLL.GetKPIByMeasureID(forms.FirstOrDefault().RelatedID, UserName);

            List<KPIType> types = KPITypeBLL.Read();
            int requestID = 0;
            int currentRequestID = 0;
            
                if (kpi != null)
                {
                    if (kpi.KPIType != null)
                    {
                        int workflowID = 103;
                        List<Group> groups = UserBLL.ReadGroup();
                        var form = forms.FirstOrDefault();
                        
                            string formula = kpi.Formula;


                            #region Calculate Semi-Auto Value
                            if (kpi.DataSource == "semi-auto" || kpi.DataSource == "auto")
                            {
                                //kpi.Parameters= kpi.Parameters.ToList().Sort((a, b) => b.ParameterName.Length.CompareTo(a.ParameterName.Length));
                                if (kpi.Parameters != null && kpi.Parameters.Count() > 0 && form.Parameters != null && form.Parameters.Count() > 0)
                                {
                                    form.Parameters.Sort((a, b) => b.ParameterName.Length.CompareTo(a.ParameterName.Length));
                                    foreach (var parameter in form.Parameters)
                                    {
                                        if (parameter.IsConstant)
                                        {
                                            formula = formula.Replace(parameter.ParameterName, kpi.Parameters.Where(a => a.ParameterName == parameter.ParameterName).FirstOrDefault()?.ConstantValue.ToString());
                                        }
                                        else
                                            formula = formula.Replace(parameter.ParameterName, parameter.Value.ToString());
                                    }
                                }
                                else
                                {
                                    throw new System.Exception("This KPI Does not have Parameters While it's Semi-Auto.");
                                }
                                form.Value = CalculateFormula(formula);
                                //form.Value = KPIBLL.CalculatePeriodActual(kpi.KPIMeasures.FirstOrDefault(f => f.ID == form.RelatedID), kpi, true);
                            }

                            #endregion

                            form.Type = (int)LevelTypeEnum.KPI;
                            form.BaseWorkflowID = (int)EnumWFBaseWorkflows.ReUpdate;
                            form.IsReUpdate = true; 
                            form.Value = form.Value.TrimDecimal();
                            string formPayload = JsonConvert.SerializeObject(form);

                            
                                Request request = new Request();
                                if (form.RelatedID == forms.FirstOrDefault().RelatedID)
                                {
                                    //First measure
                                    KPIBLL.UpdateMeasureAllowUpdate(form.RelatedID, false);
                                    var currentKPI = KPIBLL.GetKPIByMeasureID(form.RelatedID, UserName);
                                    bool IsActive = true;
                                    if (currentKPI != null)
                                    {
                                        var KPIRequests = RequestBLL.GetLevelPendingRequest(currentKPI.KPIMeasures.Select(s => s.ID).ToList(), LevelTypeEnum.KPI);
                                        if (KPIRequests.Any())
                                        {
                                            IsActive = false;
                                            requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, IsActive, KPIRequests.LastOrDefault().ID);
                                        }
                                        else
                                        {
                                            requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, IsActive);
                                        }
                                    }
                                    else
                                    {
                                        requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, IsActive);
                                    }
                                }
                                else
                                {
                                    currentRequestID = 0;
                                    KPIBLL.UpdateMeasureAllowUpdate(form.RelatedID, false);
                                    var currentKPI = KPIBLL.GetKPIByMeasureID(form.RelatedID, UserName);
                                    bool IsActive = true;
                                    if (currentKPI != null)
                                    {
                                        var KPIRequests = RequestBLL.GetLevelPendingRequest(currentKPI.KPIMeasures.Select(s => s.ID).ToList(), LevelTypeEnum.KPI);
                                        if (KPIRequests.Any())
                                        {
                                            IsActive = false;
                                            requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, IsActive, KPIRequests.LastOrDefault().ID);
                                        }
                                        else
                                        {
                                            requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, IsActive);
                                        }
                                    }
                                    else
                                    {
                                        requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, false, requestID);
                                    }
                                }







                            // Request request = new Request();

                            //form.Value = form.Value.TrimDecimal();

                            //if (form.RelatedID == forms.FirstOrDefault().RelatedID)
                            //{
                            //    //First measure
                            //    KPIBLL.UpdateMeasureAllowUpdate(form.RelatedID, false);
                            //    requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, true);
                            //}
                            //else
                            //{
                            //    KPIBLL.UpdateMeasureAllowUpdate(form.RelatedID, false);
                            //    requestID = RequestBLL.Submit(formPayload, kpi, currentRequestID, groups, UserName, DelegationUserName, workflowID, (int)LevelTypeEnum.KPI, false, requestID);
                            //}
                            if (form.Attachments != null)
                            {
                                //Attachment attachment = AutoMapper.Mapper.Map<List<Attachment>>(form.Attachment);
                                //AttachmentBLL.CreateRequestAttachment(attachment, requestID);
                                List<Attachment> attachments = AutoMapper.Mapper.Map<List<Attachment>>(form.Attachments);
                                AttachmentBLL.CreateRequestAttachment_List(attachments, requestID);
                            }
                       

                        ValidateRequestSequance(kpi);
                    }
                    else
                    {
                        //Reflect to measure immediatly as no workflow available
                        SaveWFFormUpdateKPIDTO form = forms.FirstOrDefault();
                        
                            form.Type = (int)LevelTypeEnum.KPI;

                            // var Form = AutoMapper.Mapper.Map<UpdateKPIForm>(form);
                            UpdateKPIForm updateKPIForm = new UpdateKPIForm();
                            updateKPIForm.RelatedID = form.RelatedID;
                            updateKPIForm.OldValue = form.OldValue;
                            updateKPIForm.Target = form.Target;
                            updateKPIForm.Value = form.Value;
                            KPIBLL.UpdateKPIPeriod(updateKPIForm);
                        
                    }
                    Task.Run(() => NotificationConfigurationBLL.SendNotificationWorkflow(kpi.ID, 0, requestID, enumNotificationEventType.Submit, LevelTypeEnum.KPI));
                }
            

            return true;
        }


        public void ValidateRequestSequance(KPI kpi)
        {
            var KPIRequests = RequestBLL.GetLevelPendingRequest(kpi.KPIMeasures.Select(s => s.ID).ToList(), LevelTypeEnum.KPI);
            var definition = new { RelatedID = 0, Type = 0 };
            KPIRequests.ForEach(a => a.RelatedItemId = JsonConvert.DeserializeAnonymousType(a.Form, definition).RelatedID);
            KPIRequests = KPIRequests.OrderBy(a => a.RelatedItemId).ToList();
            int index = 0;
            foreach (var item in KPIRequests)
            {
                item.RelatedRequestID = index;
                index = item.ID;


            }
            RequestBLL.UpdateKPISequanceRequest(KPIRequests);

        }

        public void ValidateRequestSequance(int kpiId)
        {
            KPI kpi = KPIBLL.ReadForCalculation(null).FirstOrDefault(a=>a.ID==kpiId);
            var KPIRequests = RequestBLL.GetLevelPendingRequest(kpi.KPIMeasures.Select(s => s.ID).ToList(), LevelTypeEnum.KPI);
            var definition = new { RelatedID = 0, Type = 0 };
            KPIRequests.ForEach(a => a.RelatedItemId = JsonConvert.DeserializeAnonymousType(a.Form, definition).RelatedID);
            KPIRequests = KPIRequests.OrderBy(a => a.RelatedItemId).ToList();
            int index = 0;
            foreach (var item in KPIRequests)
            {
                item.RelatedRequestID = index;
                index = item.ID;


            }
            RequestBLL.UpdateKPISequanceRequest(KPIRequests);

        }

        public bool KPIChangeRequest(KPIChangeRequestFormDTO form, string[] Credential)
        {

            decimal? SummedTargets = 0;
            decimal? AverageTargets = 0;
            int PeriodCount = 0;
            foreach (var measure in form.KPIMeasures)
            {
                SummedTargets += measure.Target;
                PeriodCount++;
                AverageTargets = SummedTargets / PeriodCount;
                //if (measure.Target == form.Baseline || SummedTargets == form.Baseline || AverageTargets == form.Baseline)
                //{
                //    throw new System.Exception("TargetEqualBaseline");
                //}
            }

            string UserName;
            string DelegationUserName;
            if (Credential.Count() != 3)
            {
                //UserName = Credential[0].ToLower();
                if (Credential[2] != "")
                    UserName = Credential[2];
                else
                    UserName = Credential[0];
                DelegationUserName = Credential[0].ToLower();

            }
            else
            {
                if (Credential[2] != "")
                    UserName = Credential[2].ToLower();
                else
                    UserName = Credential[0].ToLower();
                DelegationUserName = Credential[0].ToLower();
            }
            int WorkflowID = RequestBLL.GetWorkflowByBaseWorkflow((int)EnumWFBaseWorkflows.KPIChangeRequest).WorkflowID;
            List<Group> Groups = UserBLL.ReadGroup();
            KPI kpi = KPIBLL.ReadByID(form.ID, UserName);

            if (kpi.Champion.ToLower() == UserName.ToLower())
            {

                // For old data
                form.OldKPI = Mapper.Map<KPIDetailsDTO>(kpi);
                List<CAKPIWeightDTO> oldweight = new List<CAKPIWeightDTO>();
                if (!kpi.DivisionalObjectiveID.HasValue)
                    oldweight = _WeightUseCases.WeightByObjectiveId(kpi.StrategicObjectiveID.Value, "strategic").KPIs;
                else
                    oldweight = _WeightUseCases.WeightByObjectiveId(kpi.DivisionalObjectiveID.Value, "operational").KPIs;
                form.OldKPIWeight = oldweight;
                // End

                form.CreatedBy = UserName;
                form.CreatedByModel = Mapper.Map<UserListDTO>(UserName.MapUser());
                form.Created = DateTime.Now;
                form.Type = (int)LevelTypeEnum.KPI;
                form.BaseWorkflow = (int)EnumWFBaseWorkflows.KPIChangeRequest;
                int Action = (int)(form.IsDraft ? EnumRequestAction.Save : EnumRequestAction.Submit);
                string FormPayload = JsonConvert.SerializeObject(form);
                bool isNew = form.RequestID == 0;
                int requestID = RequestBLL.Submit(FormPayload, kpi, form.RequestID, Groups, UserName, DelegationUserName, WorkflowID, (int)LevelTypeEnum.KPI, true, RequestAction: Action);

                if (form.IsDraft == false)
                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForLevel(kpi.ID, LevelTypeEnum.KPI, requestID, enumNotificationEventType.ChangeRequestSubmit));


                if (form.Attachments != null)
                {
                    List<Attachment> attachment = AutoMapper.Mapper.Map<List<Attachment>>(form.Attachments);
                    AttachmentBLL.CreateRequestAttachment(attachment, requestID, isNew);
                }

                return requestID > 0;
            }
            else
                return false;
        }


        public bool MitigationAction(MitigationActionRequestFormDTO form, string[] Credential)
        {
            string UserName;
            string DelegationUserName;
            if (Credential.Count() != 3)
            {
                //UserName = Credential[0].ToLower();
                if (Credential[2] != "")
                    UserName = Credential[2];
                else
                    UserName = Credential[0];
                DelegationUserName = Credential[0].ToLower();
            }
            else
            {
                if (Credential[2] != "")
                    UserName = Credential[2].ToLower();
                else
                    UserName = Credential[0].ToLower();
                DelegationUserName = Credential[0].ToLower();
            }
            int WorkflowID = RequestBLL.GetWorkflowByBaseWorkflow((int)EnumWFBaseWorkflows.MitigationAction).WorkflowID;
            List<Group> Groups = UserBLL.ReadGroup();

            //  OrgStructure orgStructured = OrgStructureBLL.ReadByID(form.OrgStructure.ID);


            KPI kpi = KPIBLL.SingleKPI(form.KPI.ID);
            if (Groups.Where(a => a.Title == Constants.AddMitigationActionGroup && a.UsersGroups.Any(u => u.UserName.ToLower() == UserName.ToLower())).Count() > 0)
            {

                form.CreatedBy = UserName;
                form.CreatedByModel = Mapper.Map<UserListDTO>(UserName.MapUser());
                form.Created = DateTime.Now;
                form.Type = (int)LevelTypeEnum.MitigationAction;
                form.BaseWorkflow = (int)EnumWFBaseWorkflows.MitigationAction;
                int Action = (int)(form.IsDraft ? EnumRequestAction.Save : EnumRequestAction.Submit);
                string FormPayload = JsonConvert.SerializeObject(form);
                bool isNew = form.RequestID == 0;
                int requestID = RequestBLL.Submit(FormPayload, kpi, form.RequestID, Groups, UserName, DelegationUserName, WorkflowID, (int)LevelTypeEnum.MitigationAction, true, RequestAction: Action);
                if (form.IsDraft == false)
                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForMitigiationPlan(LevelTypeEnum.MitigationAction, requestID, enumNotificationEventType.CreateMitigationPlan, UserName));
                //Task.Run(() => NotificationConfigurationBLL.SendNotificationForLevel(kpi.ID, LevelTypeEnum.KPI, requestID, enumNotificationEventType.ChangeRequestSubmitSMO));
                if (form.Attachments != null)
                {
                    List<Attachment> attachment = AutoMapper.Mapper.Map<List<Attachment>>(form.Attachments);
                    AttachmentBLL.CreateRequestAttachment(attachment, requestID, isNew);
                }
                return requestID > 0;
            }
            else
                return false;
        }


        public bool CreateKPIRequest(CreateKPIFormDTO form, string[] Credential)
        {

            decimal? SummedTargets = 0;
            decimal? AverageTargets = 0;
            int PeriodCount = 0;
            foreach (var measure in form.KPIMeasures)
            {
                SummedTargets += measure.Target;
                PeriodCount++;
                AverageTargets = SummedTargets / PeriodCount;
                //if (measure.Target == form.Baseline || SummedTargets == form.Baseline || AverageTargets == form.Baseline)
                //{
                //    throw new System.Exception("TargetEqualBaseline");
                //}
            }

            string UserName;
            string DelegationUserName;
            if (Credential.Count() != 3)
            {
                //UserName = Credential[0].ToLower();
                if (Credential[2] != "")
                    UserName = Credential[2].ToLower();
                else
                    UserName = Credential[0].ToLower();
                DelegationUserName = Credential[0].ToLower();
            }
            else
            {
                if (Credential[2] != "")
                    UserName = Credential[2].ToLower();
                else
                    UserName = Credential[0].ToLower();
                DelegationUserName = Credential[0].ToLower();
            }
            int WorkflowID = RequestBLL.GetWorkflowByBaseWorkflow((int)EnumWFBaseWorkflows.CreateKPI).WorkflowID;
            List<Group> Groups = UserBLL.ReadGroup();

            form.CreatedBy = UserName;
            form.CreatedByModel = Mapper.Map<UserListDTO>(UserName.MapUser());
            form.Created = DateTime.Now;
            form.Type = (int)LevelTypeEnum.CreateKPI;
            form.BaseWorkflow = (int)EnumWFBaseWorkflows.CreateKPI;
            int Action = form.IsDraft ? (int)EnumRequestAction.Save : (int)EnumRequestAction.Submit;
            string FormPayload = JsonConvert.SerializeObject(form);
            bool isNew = form.RequestID == 0;

            int requestID = RequestBLL.Submit(FormPayload, null, form.RequestID, Groups, UserName, DelegationUserName, WorkflowID, (int)LevelTypeEnum.CreateKPI, true, RequestAction: Action);
            if (form.Attachments != null)
            {
                List<Attachment> attachment = AutoMapper.Mapper.Map<List<Attachment>>(form.Attachments);
                AttachmentBLL.CreateRequestAttachment(attachment, requestID, isNew);
            }
            if (form.IsDraft == false)
                Task.Run(() => NotificationConfigurationBLL.SendNotificationForLevel(0, LevelTypeEnum.KPI, requestID, enumNotificationEventType.CreateLevelSubmit));

            return requestID > 0;
        }

        #region Hangfire
        public void KPIApproveDailyReminder()
        {
            var kpis = KPIBLL.KPIUpdateReminder();
            var steps = RequestBLL.GetPendingSteps().Where(w => w.Request.IsActive).ToList();
            foreach (var step in steps)
            {
                var data = (JObject)JsonConvert.DeserializeObject(step.Request.Form);
                int Type = data.SelectToken("Type").Value<int>();
                if (Type == (int)LevelTypeEnum.KPI)
                {
                    UpdateKPIForm form = JsonConvert.DeserializeObject<UpdateKPIForm>(step.Request.Form);
                    var kpi = kpis.Where(w => w.KPIMeasures.Any(a => a.ID == form.RelatedID)).FirstOrDefault();
                    if (kpi != null)
                    {
                        if (KPIBLL.IsInGracePeriod(kpi))
                        {
                            Task.Run(() => NotificationConfigurationBLL.SendNotificationWorkflow(kpi.ID, form.RelatedID, step.Request.ID, enumNotificationEventType.ApprovalDailyReminder, LevelTypeEnum.KPI));
                        }
                    }
                }
            }
        }
        public void SyncKPIHistory()
        {
            var reult = KPIBLL.SyncKPIHistory();

        }
        public void FullSyncKPIHistory()
        {
            if (IncludeFullSync)
                KPIBLL.TSyncKPIHistory();
        }

        #endregion

        public decimal FormulateKPIFormula(List<ParameterDTO> parameters, string userName)
        {



            decimal result = 0;
            int KPIID = parameters.FirstOrDefault().KPIID;
            KPI kpi = KPIBLL.ReadByID(KPIID, userName);
            string formula = kpi.Formula;
            foreach (var parameter in parameters.OrderByDescending(p => p.ParameterName.Length))
            {
                if (parameter.IsConstant)
                {
                    parameter.ParameterName = kpi.Parameters.Where(w => w.ID == parameter.ID).Select(s => s.ParameterName).FirstOrDefault();
                    formula = formula.Replace(parameter.ParameterName, kpi.Parameters.Where(a => a.ParameterName == parameter.ParameterName).FirstOrDefault()?.ConstantValue.ToString());
                }
                else
                {




                    // parameter.ParameterName = kpi.Parameters.Where(w => w.ID == parameter.ID).Select(s => s.ParameterName).FirstOrDefault();
                    formula = System.Text.RegularExpressions.Regex.Replace(formula,
                 $@"\b{System.Text.RegularExpressions.Regex.Escape(parameter.ParameterName)}\b",
                 parameter.Value.ToString());
                    //System.Text.RegularExpressions.Regex.Replace(formula, $@"\b{System.Text.RegularExpressions.Regex.Escape(parameter.ParameterName)}\b", parameter.Value.ToString());
                    //formula.Replace(parameter.ParameterName, parameter.Value.ToString());

                }
            }
            result = CalculateFormula(formula);
            return result;

        }

        private decimal CalculateFormula(string formula, int recursionCounter = 0)
        {

            CalculationResult calculationResult = new CalculationResult();
            if (recursionCounter <= 10)
            {
                calculationResult = Calculator.Calculate(formula);
                if (!calculationResult.IsValid)
                {
                    formula = FixFormula(formula);
                    recursionCounter++;
                    return CalculateFormula(formula, recursionCounter);
                }
                else
                    return Convert.ToDecimal(calculationResult.Result);
            }
            else
            {
                throw new System.Exception("KPI Formula is Invalid");
            }
            if (recursionCounter <= 10)
            {
                recursionCounter++;
                CalculateFormula(formula, recursionCounter);
            }
            else
            {
                throw new System.Exception("KPI Formula is Invalid");
            }
        }

        public bool ValidateKPIFormula(ValidateParameterDTO expression)
        {
            decimal result = 0;
            string formula = expression.Formula;
            Random rnd = new Random();

            expression.Parameters.Sort((a, b) => b.ParameterName.Length.CompareTo(a.ParameterName.Length));
            foreach (var parameter in expression.Parameters)
            {
                if (parameter.ParameterName != null)
                    formula = System.Text.RegularExpressions.Regex.Replace(formula, $@"\b{System.Text.RegularExpressions.Regex.Escape(parameter.ParameterName)}\b", rnd.Next(1, 10).ToString());
            }
            bool isValid = CalculateFormulaDummyVar(formula);
            return isValid;

        }

        private bool CalculateFormulaDummyVar(string formula, int recursionCounter = 0)
        {

            CalculationResult calculationResult = new CalculationResult();
            if (recursionCounter <= 10)
            {
                calculationResult = Calculator.Calculate(formula);
                if (!calculationResult.IsValid)
                {
                    formula = FixFormula(formula);
                    recursionCounter++;
                    return CalculateFormulaDummyVar(formula, recursionCounter);
                }
                else
                    return calculationResult.IsValid;
            }
            else
                return false;

        }

        private string FixFormula(string formula)
        {
            if (!string.IsNullOrWhiteSpace(formula))
            {
                formula = formula.Replace("%", "");
                return formula;
            }
            throw new System.Exception("KPI Formula is Invalid");
        }

    }
}

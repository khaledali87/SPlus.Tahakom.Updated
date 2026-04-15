using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using SPlus.Model;
using SPlus.DataAccess;
using SPlus.Model.Domain;
using System.Data.Entity;
using Newtonsoft.Json;
using Z.EntityFramework.Plus;
using System.CodeDom;
using SPlus.DTO;
using System.Configuration;
using System.Linq.Expressions;
using System.Text.Json.Nodes;

namespace SPlus.BLL
{
    public class RequestBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();


        private readonly IUnitOfWorkFactory _factory;
        public RequestBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }

        //#region Read
        //public List<WFRequestStep> GetUserPendingTasks(string userName)
        //{
        //    using (var dataAccess = _factory.Create())
        //    {
        //        User user = dataAccess.User.Query()
        //            .Include(a => a.UsersGroups.Select(s => s.Group))
        //            .Where(s => s.UserName.ToLower() == userName.ToLower()).SingleOrDefault();
        //        //We need to Trim KPIs by user and delegations 
        //        List<KPI> kpis = dataAccess.KPI.Query()
        //            .Include(a => a.KPIMeasures).ToList();
        //        //kpis.SecureListObj(dataAccess, userName);

        //        List<WFFormUpdateKPI> forms = dataAccess.WFFormUpdateKPI.Query().Include(a => a.WFRequest.WFRequestSteps).ToList();
        //        //List <WFRequestStep> pendingSteps = new List<WFRequestStep>();
        //        //List<WFFormUpdateKPI> tempforms = new List<WFFormUpdateKPI>();


        //        //Get List of requests with steps and forms 
        //        List<WFRequest> Requests = dataAccess.Request.Query().Include(a => a.WFRequestSteps).Include(a => a.WFFormUpdateKPI).ToList();

        //        //Get Allow Requests
        //        Requests = AllowedRequests(Requests, user);
        //        //Set the can approve steps in a list
        //        List<WFRequestStep> requestSteps = Requests.Where(a=>a.CanApprove).SelectMany(s => s.WFRequestSteps).ToList();


        //        foreach (var step in requestSteps)
        //        {
        //            step.WFRequest.WFFormUpdateKPI.MapAttachment(dataAccess);                    
        //        }

        //        return requestSteps;
        //    }
        //}


        public List<RequestStep> GetMyRequests(string userName)
        {
            var definition = new { IsDraft = false, CreatedBy = string.Empty, BaseWorkflow = 0 };
            var requests = GetRequests(userName,false , a => a.CreatedBy.ToLower() == userName.ToLower())
                          .Where(a => a.CreatedBy.ToLower() == userName.ToLower() ||  JsonConvert.DeserializeAnonymousType(a.Form, definition).CreatedBy?.ToLower() == userName.ToLower()).ToList();
           
            var steps = requests.SelectMany(r => r.RequestSteps)
                                .Where(r => !r.IsCancelled)
                                .isAllowedToDoActionList(userName.MapUserWithGroups())
                                //.Where(a => a.CanApprove)
                                .ToList();


            var DraftRequest = requests.Where(a => JsonConvert.DeserializeAnonymousType(a.Form, definition).IsDraft && ( 
                                        a.Status == (int)EnumWFStatuses.Return || 
                                        a.Status == (int)EnumWFStatuses.Rejected || 
                                        a.Status == (int)EnumWFStatuses.Completed || 
                                        a.Status == (int)EnumWFStatuses.Pending)).ToList();
            RequestStep requestStep;
            foreach (var request in DraftRequest)
            {
                requestStep = new RequestStep();
                requestStep.RequestID = request.ID;
                requestStep.Request = new Request();
                requestStep.Request = request;
                if (!steps.Any(a => a.RequestID == request.ID))
                    steps.Add(requestStep);
            }

            return steps;
        }



        public List<Request> GetLevelPendingRequest(List<int> RelatedIDs, LevelTypeEnum Level)
        {
            using (var dataAccess = _factory.Create())
            {
                var definition = new { RelatedID = 0, Type = 0 };
                List<Request> requests = dataAccess.Request.Query(w => w.Status == (int)EnumWFStatuses.Pending && RelatedIDs.Contains(w.RelatedID ?? 0)).IncludeOptimized(a => a.RequestSteps)
                    .ToList().Where(a => RelatedIDs.Contains(JsonConvert.DeserializeAnonymousType(a.Form, definition).RelatedID) && JsonConvert.DeserializeAnonymousType(a.Form, definition).Type == (int)Level).ToList();
                return requests.OrderBy(a => a.ID).ToList();
            }
        }

        public void UpdateKPISequanceRequest(List<Request> requests)
        {
            using (var dataAccess = _factory.Create())
            {
                foreach (var item in requests)
                {
                    var request = dataAccess.Request.Query().FirstOrDefault(a => a.ID == item.ID);
                    if (request != null)
                    {
                        request.RelatedRequestID = item.RelatedRequestID;

                        dataAccess.Request.Save(request);

                    }

                }
                dataAccess.Complete();
            }
        }


        public List<RequestStep> GetMyTasks(string userName,bool includeAttachment)
        {
            var requests = GetRequests(userName, includeAttachment).Where(a => a.IsActive).ToList();
            return requests.SelectMany(r => r.RequestSteps).Where(r => !r.IsCancelled).isAllowedToDoActionList(userName.MapUserWithGroups()).Where(a => a.CanApprove).ToList();
        }

        public List<RequestStep> GetAllPendingAndNewTasks()
        {
            var requests = GetAllRequests().ToList();
            return requests.SelectMany(r => r.RequestSteps).Where(r => !r.IsCancelled).Where(a => a.Status == (int)EnumWFStatuses.Pending || a.Status == (int)EnumWFStatuses.New).ToList();
        }
        public List<WorkflowStep> GetWorklowSteps()
        {
            using (var dataAccess = _factory.Create())
            {
                return dataAccess.WorkflowStep.GetAll().ToList();
            }
        }

        public List<Request> GetRequests_old(string userName, bool includActtchment = true)
        {
            using (var dataAccess = _factory.Create())
            {
                List<Request> requests = dataAccess.Request.Query().Include(a => a.RequestSteps).ToList();
                requests.MapUsers();
                foreach (Request request in requests)
                {


                    if (request.RelatedRequestID != 0 && requests.Where(a => a.ID == request.RelatedRequestID).FirstOrDefault().Status == (int)EnumWFStatuses.Completed)
                        request.RequestSteps.Where(r => !r.IsCancelled).isAllowedToDoActionList(userName.MapUserWithGroups());
                    else if (request.RelatedRequestID == 0)
                        request.RequestSteps.Where(r => !r.IsCancelled).isAllowedToDoActionList(userName.MapUserWithGroups());

                    if (request.RequestSteps.Where(r => !r.IsCancelled).Any(a => a.CanApprove))
                        request.CanApprove = true;

                }

                //requests = requests.MapAttachment(dataAccess).ToList();
                if (includActtchment)
                    requests = requests.MapAttachments(dataAccess).ToList();
                return requests.OrderBy(a => a.ID).ToList();
            }
        }

        public List<Request> GetRequests(string userName, bool includActtchment = true, Expression<Func<Request, bool>> predicate = null)
        {
            var user = userName.MapUserWithGroups();
            using (var dataAccess = _factory.Create())
            {
              // 1️⃣ Load all requests with steps(cannot filter IsActive)
                var requestsQuery = dataAccess.Request.Query()
                .Include(r => r.RequestSteps);

                if(predicate != null)
                {
                    requestsQuery = requestsQuery.Where(predicate);
                }

                var requests = requestsQuery.ToList();

                // 2️⃣ Map users to requests (if needed)
                requests.MapUsers();
                if (includActtchment)
                    requests = requests.MapAttachments(dataAccess).ToList();
                // 3️⃣ Prepare dictionary for RelatedRequest lookup
                var requestDict = requests.ToDictionary(r => r.ID);
                foreach (var request in requests)
                {
                    // Only consider steps that are not cancelled
                    var steps = request.RequestSteps.Where(s => !s.IsCancelled);
                        // If this request is related to another request

                        if(request.WorkflowID == 103)
                        {
                             steps = steps.isAllowedToDoActionList(user);
                        }
                        else if (request.RelatedRequestID != 0 && 
                            requestDict.TryGetValue(request.RelatedRequestID, out var relatedRequest) &&
                            (relatedRequest.Status == (int)EnumWFStatuses.Completed || relatedRequest.Status == (int)EnumWFStatuses.Rejected))
                        {
                            steps = steps.isAllowedToDoActionList(user);
                        }
                        else if (request.RelatedRequestID == 0)
                        {
                            steps = steps.isAllowedToDoActionList(user);
                        }
                    
                   

                    

                    // Mark request as approvable if any of its steps can be approved
                    request.CanApprove = steps.Any(s => s.CanApprove);
                }

                //requests = requests.MapAttachment(dataAccess).ToList();
                if (includActtchment)
                    requests = requests.MapAttachments(dataAccess).ToList();
                return requests.OrderBy(a => a.ID).ToList();
            }
        }



        public List<RequestStep> GetActiveRequests(string userName, bool includeAttachment = true)
        {
            var user = userName.MapUserWithGroups(); // get user + groups once

            using (var dataAccess = _factory.Create())
            {

               
                var requestsQuery = dataAccess.Request.Query()
                    .Where(r => r.IsActive || r.WorkflowID == 103)
                .Include(r => r.RequestSteps);

            

                var requests = requestsQuery.ToList();

                //  Map users to requests (if needed)
                requests.MapUsers();
                if (includeAttachment)
                    requests = requests.MapAttachments(dataAccess).ToList();
                // Prepare dictionary for RelatedRequest lookup
                var requestDict = requests.ToDictionary(r => r.ID);

                // 4️⃣ Process each request and compute CanApprove for its steps
                foreach (var request in requests)
                {
                    // Only consider steps that are not cancelled
                    var steps = request.RequestSteps.Where(s => !s.IsCancelled);

                    if(request.ID == 5007)
                    {

                    }

                    if(request.WorkflowID == 103)
                    {
                        steps = steps.isAllowedToDoActionList(user);
                    }
                    // If this request is related to another request
                    else if (request.RelatedRequestID != 0 &&
                        requestDict.TryGetValue(request.RelatedRequestID, out var relatedRequest) &&
                        relatedRequest.Status == (int)EnumWFStatuses.Completed)
                    {
                        steps = steps.isAllowedToDoActionList(user);
                    }
                    else if (request.RelatedRequestID == 0)
                    {
                        steps = steps.isAllowedToDoActionList(user);
                    }
                  

                    // Mark request as approvable if any of its steps can be approved
                    request.CanApprove = steps.Any(s => s.CanApprove);
                }
            
            // 5️⃣ Only return steps from requests that are active
            return requests
                .SelectMany(r => r.RequestSteps)
                .Where(s => !s.IsCancelled)
                .ToList();
        }}

     


        public List<Request> GetRequests(LevelTypeEnum Type, EnumWFBaseWorkflows baseWorkflow)
        {
            using (var dataAccess = _factory.Create())
            {
                var definition = new { Type = 0, BaseWorkflow = 0 };
                List<Request> requests = dataAccess.Request.Query()
                    .IncludeOptimized(a => a.RequestSteps).ToList()
                    .Where(w => JsonConvert.DeserializeAnonymousType(w.Form, definition).Type == (int)Type && (JsonConvert.DeserializeAnonymousType(w.Form, definition).BaseWorkflow == (int)baseWorkflow || JsonConvert.DeserializeAnonymousType(w.Form, definition).BaseWorkflow == 0)).ToList();
                requests.MapUsers();

                foreach (var request in requests)
                {
                    if (JsonConvert.DeserializeAnonymousType(request.Form, definition).Type == (int)LevelTypeEnum.KPI && (JsonConvert.DeserializeAnonymousType(request.Form, definition).BaseWorkflow == (int)baseWorkflow || JsonConvert.DeserializeAnonymousType(request.Form, definition).BaseWorkflow == 0))
                    {
                        request.UpdateKPIForm = JsonConvert.DeserializeObject<UpdateKPIForm>(request.Form);
                    }
                }

                return requests.OrderBy(a => a.ID).ToList();
            }
        }
        //public List<Request> GetAllRequests()
        //{
        //    using (var dataAccess = _factory.Create())
        //    {
        //        List<Request> requests = dataAccess.Request.Query().IncludeOptimized(a => a.RequestSteps).Where(a => a.RequestSteps.Any(r => !r.IsCancelled)).ToList();
        //        requests.MapUsers();

        //        requests = requests.MapAttachment(dataAccess).ToList();
        //        requests = requests.MapAttachments(dataAccess).ToList();
        //        return requests.OrderBy(a => a.ID).ToList();
        //    }
        //}

        public List<Request> GetAllRequests(Expression<Func<Request, bool>> predicate = null)
        {
            using (var dataAccess = _factory.Create())
            {
                IQueryable<Request> query = dataAccess.Request.Query().IncludeOptimized(a => a.RequestSteps);

                if(predicate != null)
                {
                    query = query.Where(predicate);
                }

                List<Request>  requests = query.Where(a => a.RequestSteps.Any(r => !r.IsCancelled)).ToList();
                
                
                
                requests.MapUsers();

                requests = requests.MapAttachment(dataAccess).ToList();
                requests = requests.MapAttachments(dataAccess).ToList();
                return requests.OrderBy(a => a.ID).ToList();
            }
        }

        public List<Request> ReadCRDashboardSubmitedChangeRequest()
        {
            var requests = GetAllRequests().Where(a => a.WorkflowID == Convert.ToInt32(ConfigurationManager.AppSettings["KPIChangeRequestWorkflowID"])
               && (a.Status == (int)EnumWFStatuses.Rejected || a.Status == (int)EnumWFStatuses.Completed || a.Status == (int)EnumWFStatuses.Pending)
                ).ToList();

            return requests;
        }





        public Request GetRequestByID(int id, string userName)
        {
            using (var dataAccess = _factory.Create())
            {
                Request request = dataAccess.Request.Query().Include(a => a.RequestSteps.Select(s => s.ActionByModel)).Where(a => a.ID == id).FirstOrDefault();
                request.RequestSteps = request.RequestSteps.Where(r => !r.IsCancelled).ToList();
                request.RequestSteps.Where(r => !r.IsCancelled).isAllowedToDoActionList(userName.MapUserWithGroups());
                request.CanApprove = request.RequestSteps.Where(w => w.Status == (int)EnumWFStatuses.Pending).Any(a => a.CanApprove);

                request.RequestSteps.Where(r => !r.IsCancelled).MapUsers();

                request = request.MapAttachment(dataAccess);
                request = request.MapAttachments(dataAccess);
                return request;
            }
        }


        public List<Request> GetRequest_All()
        {
            using (var dataAccess = _factory.Create())
            {
                List<Request> request = dataAccess.Request.Query().ToList();

                return request;
            }
        }


        public Request GetRequestByIDWithReturn(int id, string userName)
        {
            using (var dataAccess = _factory.Create())
            {
                Request request = dataAccess.Request.Query().Include(a => a.RequestSteps.Select(s => s.ActionByModel)).Where(a => a.ID == id).FirstOrDefault();

                request.RequestSteps.isAllowedToDoActionList(userName.MapUserWithGroups());
                request.RequestSteps.MapUsers();
                return request;
            }
        }
        public List<Request> GetRequestsByFormRelatedID(int relatedID, int type)
        {
            using (var dataAccess = _factory.Create())
            {
                var definition = new { RelatedID = 0, type = 0 };
                List<Request> requests = dataAccess.Request.Query().Where(x => x.RelatedID == relatedID).Include(a => a.RequestSteps).Where(r => !r.RequestSteps.Any(rr => !rr.IsCancelled))
                    .ToList().Where(a => JsonConvert.DeserializeAnonymousType(a.Form, definition).RelatedID == relatedID && JsonConvert.DeserializeAnonymousType(a.Form, definition).type == type).ToList();
                return requests.OrderBy(a => a.ID).ToList();
            }
        }

        public List<Request> GetRequestsByFormRelatedIDCapital(int relatedID, int type)
        {
            using (var dataAccess = _factory.Create())
            {
                var definition = new { RelatedID = 0, Type = 0 };
                List<Request> requests = dataAccess.Request.Query()
                    .Where(x=> x.RelatedID == relatedID)
                    .AsNoTracking()
                    .Include(a => a.RequestSteps)
                    .ToList();



                   requests = requests.Where(a => JsonConvert.DeserializeObject<UpdateKPIForm>(a.Form).RelatedID == relatedID && JsonConvert.DeserializeObject<UpdateKPIForm>(a.Form).Type == type).ToList();
                return requests.OrderBy(a => a.ID).ToList();
            }
        }

        public List<Request> GetRequestsByFormRelatedID(int relatedID, LevelTypeEnum Level, EnumWFBaseWorkflows enumWFBaseWorkflows)
        {
            using (var dataAccess = _factory.Create())
            {
                var definition = new { RelatedID = 0, type = 0, BaseWorkflowID = 0 };
                List<Request> requests = dataAccess.Request.Query().Where(x => x.RelatedID == relatedID).IncludeOptimized(a => a.RequestSteps).Where(r => !r.RequestSteps.Any(rr => rr.IsCancelled)).ToList();
                requests = requests.Where(a => JsonConvert.DeserializeAnonymousType(a.Form, definition).RelatedID == relatedID && JsonConvert.DeserializeAnonymousType(a.Form, definition).type == (int)Level).ToList();
                return requests.OrderBy(a => a.ID).ToList();
            }
        }
        public List<Request> GetRequestsByFormRelatedIDs(List<int> relatedIDs)
        {
            using (var dataAccess = _factory.Create())
            {
                var definition = new { Type = 0, BaseWorkflowID = 0 };
                List<Request> requests = dataAccess.Request.Query().Where(r =>  relatedIDs.Any(x=> x== r.RelatedID)).Include(a => a.RequestSteps)
                    //.Where(r => !r.RequestSteps.Any(rr => !rr.IsCancelled))
                    .ToList();

                foreach (var request in requests)
                {
                    if (JsonConvert.DeserializeAnonymousType(request.Form, definition).Type == (int)LevelTypeEnum.KPI && (JsonConvert.DeserializeAnonymousType(request.Form, definition).BaseWorkflowID == (int)EnumWFBaseWorkflows.Update || JsonConvert.DeserializeAnonymousType(request.Form, definition).BaseWorkflowID == 0))
                    {
                        request.UpdateKPIForm = JsonConvert.DeserializeObject<UpdateKPIForm>(request.Form);
                    }
                }
                requests = requests.Where(a => relatedIDs.Any(id => id == a.UpdateKPIForm?.RelatedID)).ToList();
                return requests.OrderBy(a => a.ID).ToList();
            }
        }

        public bool DeleteRequest(int id)
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                Request req = dataAccess.Request.Get(id);
                if (req != null)
                {

                    dataAccess.Request.Delete(req);
                    result = dataAccess.Complete();
                    dataAccess.Dispose();
                }
                if (result >= 1)
                    return true;
                else
                    return false;
            }
        }




        public int Submit(string form, object obj, int currentRequestID, List<Group> groups, string userName, string DelegationUserName, int workflowID, int Level, bool isActive = false, int relatedRequestIID = 0, int RequestAction = 0)
        {
            using (var dataAccess = _factory.Create())
            {

                //   switch (Level)
                //   {
                //case (int)LevelTypeEnum.KPI:
                //    {
                Workflow workflow = dataAccess.Workflow.Query().Include(ws => ws.WorkflowSteps).Where(w => w.WorkflowID == workflowID).SingleOrDefault();
                Request Request = null;

                if (workflow is null || workflow.WorkflowSteps is null || workflow.WorkflowSteps.Count() == 0)
                    return 0;
                else
                {
                    if (currentRequestID == 0)
                        Request = new Request();
                    else
                    {
                        Request = GetRequestByID(currentRequestID, userName);
                        List<RequestStep> steps = dataAccess.RequestStep.Query().Where(a => a.RequestID == Request.ID).ToList();
                        //foreach (RequestStep step in steps)
                        //{
                        //    dataAccess.RequestStep.Delete(step);
                        //}
                        if (Request.Status == (int)EnumWFStatuses.Return)
                        {
                            foreach (RequestStep step in steps)
                            {
                                step.IsCancelled = true;
                                dataAccess.RequestStep.Save(step);
                            }
                        }
                        else
                        {
                            foreach (RequestStep step in steps)
                            {
                                step.IsCancelled = false;
                                dataAccess.RequestStep.Delete(step);
                            }
                        }
                        //Delete Steps
                    }

                    Request.Created = DateTime.Now;
                    Request.Modified = DateTime.Now;
                    Request.CreatedBy = userName;
                    Request.WorkflowID = workflow.WorkflowID;
                    Request.IsActive = isActive;
                    Request.Form = form;
                    Request.RelatedRequestID = relatedRequestIID;
                    Request.Status = workflowID == 104 ? (int)EnumWFStatuses.Completed : RequestAction != (int)EnumRequestAction.Save ? (int)EnumWFStatuses.Pending : (int)EnumWFStatuses.New; //(int)EnumWFStatuses.Pending;
                    dataAccess.Request.Save(Request);
                    dataAccess.Complete();
                    Request = GetRequestByID(Request.ID, userName);
                    if (RequestAction != (int)EnumRequestAction.Save)
                    {
                        foreach (WorkflowStep wfStep in workflow.WorkflowSteps.OrderBy(a => a.Order))
                        {
                            RequestStep step = new RequestStep();

                            step.RequestID = Request.ID;
                            step.EnglishName = wfStep.EnglishName;
                            step.ArabicName = wfStep.ArabicName;
                            step.Order = wfStep.Order;
                            step.Created = DateTime.Now;
                            step.Modified = DateTime.Now;
                            if (wfStep.Order == 1)
                            {
                                step.Status = workflowID == 104 ? (int)EnumWFStatuses.Approved : (int)EnumWFStatuses.NA;
                                step.ActionBy = DelegationUserName != null ? DelegationUserName.ToLower() : null;
                            }
                            else if (wfStep.Order == 2)
                                step.Status = (int)EnumWFStatuses.Pending;
                            else
                                step.Status = (int)EnumWFStatuses.New;

                            step.IsGroup = wfStep.IsGroup;
                            if (wfStep.IsGroup)
                                step.Approver = groups.Where(a => a.ID == wfStep.Approver).FirstOrDefault().Title;
                            else
                            {
                                string approver = Enum.GetName(typeof(WFApproversEnum), wfStep.Approver);
                                step.Approver = GetApprover(approver, workflow.BaseWorkflowID, obj);
                            }
                            dataAccess.RequestStep.Save(step);

                        }
                    }
                }

                dataAccess.Complete();
                return Request.ID;
                //  }
                //  default:
                //  return 0;
                //}

            }
        }

        public int DoAction(int requestID, int ActionID, string Comments, string userName, string DelegationUserName, int QualityType)
        {
            using (var dataAccess = _factory.Create())
            {
                List<Request> requests = dataAccess.Request.Query().Include(a => a.RequestSteps).ToList();
                Request request = requests.FirstOrDefault(s => s.ID == requestID);

                var workflow = dataAccess.Workflow.Query().Where(a => a.WorkflowID == request.WorkflowID).FirstOrDefault();
                RequestStep step = request.RequestSteps.Where(a => !a.IsCancelled && a.RequestID == requestID && a.Status == (int)EnumWFStatuses.Pending).FirstOrDefault();
                Request nextRequest = null;
                int stepActionID = ActionID;
                if (step != null)
                {
                    bool isValid;
                    if (step.IsGroup)
                    {
                        User user = dataAccess.User.Query().Include(a => a.UsersGroups.Select(s => s.Group)).Where(w => w.UserName.ToLower() == userName.ToLower()).FirstOrDefault();
                        isValid = user.UsersGroups.Any(a => a.Group.Title.ToLower() == step.Approver.ToLower());
                    }
                    else
                    {
                        isValid = userName.ToLower() == step.Approver.ToLower();
                    }

                    if (isValid)
                    {
                        step.ActionBy = DelegationUserName.ToLower();
                        step.Comments = Comments;
                        step.Modified = DateTime.Now;
                        step.QualityType = QualityType;

                        if (ActionID == (int)EnumWFStatuses.Approved)
                        {
                            if (request.RequestSteps.Where(a => !a.IsCancelled).Max(a => a.Order) == step.Order)
                            {
                                //Final Approval
                                request.Status = (int)EnumWFStatuses.Completed;
                                ActionID = (int)EnumWFStatuses.Completed;
                                request.IsActive = false;
                                if(request.WorkflowID != 103)
                                {
                                    nextRequest = requests.SingleOrDefault(s => s.RelatedRequestID == requestID);
                                    if (nextRequest != null)
                                        nextRequest.IsActive = true;
                                }
                               
                            }
                            else
                            {
                                //Approval
                                RequestStep nextStep = request.RequestSteps.Where(a => !a.IsCancelled && a.Order == step.Order + 1).FirstOrDefault();
                                if (nextStep is null)
                                    nextStep = request.RequestSteps.Where(a => !a.IsCancelled && a.Order == step.Order + 2).FirstOrDefault();
                                nextStep.Modified = DateTime.Now;
                                nextStep.Status = (int)EnumWFStatuses.Pending;
                            }
                        }

                        else if (ActionID == (int)EnumWFStatuses.Return)
                        {
                            //Return
                            request.Modified = DateTime.Now;
                            request.Status = (int)EnumWFStatuses.Return;

                            if (workflow != null)
                            {
                                switch (workflow.BaseWorkflowID)
                                {
                                    case (int)EnumWFBaseWorkflows.MitigationAction:
                                        {
                                            if (request.RequestSteps.Where(a => !a.IsCancelled).Max(a => a.Order) == step.Order)
                                            {
                                                RequestStep currentStep = request.RequestSteps.Where(a => !a.IsCancelled && a.Order == step.Order).FirstOrDefault();
                                                currentStep.Modified = DateTime.Now;
                                                currentStep.Status = (int)EnumWFStatuses.New;


                                                RequestStep prevStep = request.RequestSteps.Where(a => !a.IsCancelled && a.Order == step.Order - 1).FirstOrDefault();
                                                prevStep.Status = (int)EnumWFStatuses.Pending;


                                            }


                                            break;
                                        }

                                }
                            }
                        }

                        else
                        {
                            //Rejection
                            request.Modified = DateTime.Now;
                            request.Status = (int)EnumWFStatuses.Rejected;
                        }
                        step.Status = stepActionID;
                        dataAccess.Request.Save(request);
                        if (nextRequest != null)
                            dataAccess.Request.Save(nextRequest);
                        dataAccess.Complete();

                    }
                    else
                    {
                        ActionID = 0;

                        throw new System.Exception("Unauthorized user");
                    }

                    dataAccess.Dispose();
                }
                else ActionID = 0;
            }
            return ActionID;
        }

        private string GetApprover(string approver, int baseWorkflowID, object obj)
        {
            switch (baseWorkflowID)
            {
                case (int)EnumWFBaseWorkflows.Update:
                    if (obj.GetType() == typeof(KPI))
                    {

                        KPI kpi = (KPI)obj;

                        if (kpi.GetType().GetProperties().Where(w => w.Name?.ToLower() == approver?.ToLower()).FirstOrDefault() != null)
                        {
                            approver = kpi.GetType().GetProperties()
                                .Where(w => w.Name?.ToLower() == approver?.ToLower())
                                .Select(s => s.GetValue(kpi))
                                .FirstOrDefault()?.ToString();
                        }
                    }
                    break;
                case (int)EnumWFBaseWorkflows.KPIChangeRequest:
                    if (obj.GetType() == typeof(KPI))
                    {

                        KPI kpi = (KPI)obj;

                        if (kpi.GetType().GetProperties().Where(w => w.Name?.ToLower() == approver?.ToLower()).FirstOrDefault() != null)
                        {
                            approver = kpi.GetType().GetProperties()
                                .Where(w => w.Name?.ToLower() == approver?.ToLower())
                                .Select(s => s.GetValue(kpi))
                                .FirstOrDefault()?.ToString();
                        }
                    }
                    break;

                case (int)EnumWFBaseWorkflows.MitigationAction:
                    //if (obj.GetType() == typeof(OrgStructure))
                    //{

                    //    OrgStructure orgStructure = (OrgStructure)obj;

                    //    if (orgStructure.GetType().GetProperties().Where(w => w.Name?.ToLower() == approver?.ToLower()).FirstOrDefault() != null)
                    //    {
                    //        approver = orgStructure.GetType().GetProperties()
                    //            .Where(w => w.Name?.ToLower() == approver?.ToLower())
                    //            .Select(s => s.GetValue(orgStructure))
                    //            .FirstOrDefault()?.ToString();
                    //    }
                    //}
                    if (obj.GetType() == typeof(KPI))
                    {

                        KPI kpi = (KPI)obj;

                        if (kpi.GetType().GetProperties().Where(w => w.Name?.ToLower() == approver?.ToLower()).FirstOrDefault() != null)
                        {
                            approver = kpi.GetType().GetProperties()
                                .Where(w => w.Name?.ToLower() == approver?.ToLower())
                                .Select(s => s.GetValue(kpi))
                                .FirstOrDefault()?.ToString();
                        }
                    }
                    break;





            }
            return approver;
        }


        public Workflow GetWorkflow(int id)
        {
            using (var dataAccess = _factory.Create())
            {
                return dataAccess.Workflow.Query().Where(a => a.WorkflowID == id).FirstOrDefault();
            }
        }



        public Workflow GetWorkflowByBaseWorkflow(int id)
        {
            using (var dataAccess = _factory.Create())
            {
                return dataAccess.Workflow.Query().Where(a => a.BaseWorkflowID == id).FirstOrDefault();
            }
        }

        public List<Workflow> GetWorkflowByBaseWorkflowList(int id)
        {
            using (var dataAccess = _factory.Create())
            {
                return dataAccess.Workflow.Query().Where(a => a.BaseWorkflowID == id).ToList();
            }
        }

        public void UpdateMitigationActionRequest(MitigationActionRequestFormDTO form)
        {
            using (var dataAccess = _factory.Create())
            {
                Request request = dataAccess.Request.Query().Where(a => a.ID == form.RequestID).FirstOrDefault();
                if (request != null)
                {
                    string FormPayload = JsonConvert.SerializeObject(form);
                    request.Form = FormPayload;
                    dataAccess.Request.Save(request);
                    dataAccess.Complete();
                    dataAccess.Dispose();
                }
            }
        }
        public void UpdateRequestApprovers(KPI kpi)
        {
            using (var dataAccess = _factory.Create())
            {
                var relatedIds = dataAccess.KPIMeasure.Query(w => w.KPIID == kpi.ID).Select(s => s.ID).ToList();
                var requests = GetRequestsByFormRelatedIDs(relatedIds);
                var pendingRequests = requests.Where(w => w.Status == (int)EnumWFStatuses.Pending).ToList();

                foreach (var request in pendingRequests)
                {
                    var Workflow = dataAccess.Workflow.Query(a => a.WorkflowID == request.WorkflowID).Include(a => a.WorkflowSteps).FirstOrDefault();
                    if (Workflow != null)
                    {
                        foreach (var wfstep in Workflow.WorkflowSteps)
                        {
                            var step = request.RequestSteps.Where(a => !a.IsCancelled && a.Order == wfstep.Order && (a.Status == (int)EnumWFStatuses.Pending || a.Status == (int)EnumWFStatuses.New)).FirstOrDefault();
                            if (step != null)
                            {
                                if (!wfstep.IsGroup && wfstep.Approver == (int)WFApproversEnum.Owner)
                                {
                                    var dbstep = dataAccess.RequestStep.Query(w => w.ID == step.ID).FirstOrDefault();
                                    switch (wfstep.Approver)
                                    {
                                        case (int)WFApproversEnum.Owner:
                                            {
                                                step.Approver = kpi.Owner;
                                                break;
                                            }
                                        case (int)WFApproversEnum.Champion:
                                            {
                                                step.Approver = kpi.Champion;
                                                break;
                                            }
                                        case (int)WFApproversEnum.Sponsor:
                                            {
                                                break;
                                            }
                                        default:
                                            break;
                                    }
                                    dataAccess.RequestStep.Save(step);
                                    dataAccess.Complete();
                                }

                            }
                        }
                    }
                }
            }
        }

        //public void UpdateMitigationReturnOneStep(MitigationActionRequestFormDTO form)
        //{
        //    using (var dataAccess = _factory.Create())
        //    {
        //        List<RequestStep> requests = dataAccess.RequestStep.Query().Where(a => a.RequestID == form.RequestID).ToList();
        //        if (requests != null)
        //        {

        //            RequestStep requestStep = requests.Where(a => a.Status == (int)EnumWFStatuses.Pending).FirstOrDefault();

        //            if (requestStep != null)
        //            {
        //                requestStep.Status ==

        //            }

        //            foreach (var item in requests)
        //            {
        //                if (item.Order == 3)
        //                {
        //                    item.Status = (int)EnumWFStatuses.New;
        //                }
        //                else if
        //            }


        //            string FormPayload = JsonConvert.SerializeObject(form);
        //            request.Form = FormPayload;
        //            dataAccess.Request.Save(request);
        //            dataAccess.Complete();
        //            dataAccess.Dispose();
        //        }
        //    }
        //}

        #region Hangfire

        public List<RequestStep> GetPendingSteps()
        {
            using (var dataAccess = _factory.Create())
            {
                return dataAccess.RequestStep.Query().IncludeOptimized(a => a.Request).Where(w => !w.IsCancelled && w.Status == (int)EnumWFStatuses.Pending).ToList();
            }
        }
        #endregion
    }
}

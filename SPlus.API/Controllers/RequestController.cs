using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SPlus.Model;
using SPlus.UseCases;
using System.Web.Http.Cors;
using SPlus.API.Attribute;
using SPlus.DTO;
using model = SPlus.Model;
using System.Threading.Tasks;
using StructureMap;
using SPlus.Helper;

namespace SPlus.API.Controllers
{
   [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RequestController : ApiController
    {
        Container _Container = IOC.InitializeContainer();

        private readonly RequestUseCases RequestUseCases;
        private readonly NotificationConfigurationUseCases NotificationConfigurationUseCases;
        private readonly ParameterUseCases ParameterUseCases;
        private readonly KPIUseCases KPIUseCases;
        private readonly TaskCenterUseCases TaskCenterUseCases;

        public RequestController()
        {
            RequestUseCases = _Container.GetInstance<RequestUseCases>();
            NotificationConfigurationUseCases = _Container.GetInstance<NotificationConfigurationUseCases>();
            ParameterUseCases = _Container.GetInstance<ParameterUseCases>();
            KPIUseCases = _Container.GetInstance<KPIUseCases>();
            TaskCenterUseCases = _Container.GetInstance<TaskCenterUseCases>();
        }
        #region Create

        [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("UpdateKPIValue")]
        public ResultWrapper<bool> UpdateKPIValue(List<SaveWFFormUpdateKPIDTO> WFFormUpdateKPIs)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                var Credential = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = RequestUseCases.UpdateKPIValue(WFFormUpdateKPIs, Credential);
                    Task.Run(async () => { await RequestUseCases.Log(Token, "Submit", AuditTrailActionENums.Workflow); });

                }

                catch (Exception ex)
                {
                    result.Data = false;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("ReUpdateKPIValue")]
        public ResultWrapper<bool> ReUpdateKPIValue(List<SaveWFFormUpdateKPIDTO> WFFormUpdateKPIs)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                var Credential = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = RequestUseCases.ReUpdateKPIValue(WFFormUpdateKPIs, Credential);
                    Task.Run(async () => { await RequestUseCases.Log(Token, "Submit", AuditTrailActionENums.Workflow); });

                }

                catch (Exception ex)
                {
                    result.Data = false;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        [OperationContract]
        [DataFormatingInvoker]
        //[BasicAuthenticationInvoker]
        [HttpGet]
        [Route("FixSequance/{id}")]
        public ResultWrapper<bool> FixSequance(int id)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            
            //if (Request.Headers.TryGetValues("Token", out Token))
            {
               // var Credential = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                     RequestUseCases.ValidateRequestSequance(id);
                    //Task.Run(async () => { await RequestUseCases.Log(Token, "Submit", AuditTrailActionENums.Workflow); });

                }

                catch (Exception ex)
                {
                    result.Data = false;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


        [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("KPIChangeRequest")]
        public ResultWrapper<bool> KPIChangeRequest(KPIChangeRequestFormDTO kPIChangeRequestForm)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            string Language = string.Empty;
            if (Request.Headers.TryGetValues("Language", out var LanguageFromHeader))
                Language = LanguageFromHeader.FirstOrDefault().ToString();
            else
                Language = "en";
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                var Credential = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = RequestUseCases.KPIChangeRequest(kPIChangeRequestForm, Credential);
                    Task.Run(async () => { await RequestUseCases.Log(Token, "Submit", AuditTrailActionENums.Workflow); });
                }
                catch (Exception ex)
                {
                    result.Data = false;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                    {
                        if (ex.Message == "TargetEqualBaseline")
                        {
                            if (Language == "en")
                            {
                                result.StatusMessage = "Target, Sum Of Targets, or Average of Targets can not be equal to baseline.";
                            }
                            else
                            {
                                result.StatusMessage = "لا يمكن أن يكون الهدف أو مجموع الأهداف أو متوسط ​​الأهداف مساوياً لخط الأساس.";
                            }
                        }
                        else
                        {
                            result.StatusMessage = ex.Message;
                        }
                    }
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }




        [OperationContract]  
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("CreateKPI")]
        public ResultWrapper<bool> CreateKPI(CreateKPIFormDTO createKPIForm)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            IEnumerable<string> LanguageFromHeader;
            string Language = string.Empty;
            if (Request.Headers.TryGetValues("Language", out LanguageFromHeader))
                Language = LanguageFromHeader.FirstOrDefault().ToString();
            else 
                Language = "en";
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                var Credential = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = RequestUseCases.CreateKPIRequest(createKPIForm, Credential);
                    Task.Run(async () => { await RequestUseCases.Log(Token, "Submit", AuditTrailActionENums.Workflow); });
                }
                catch (Exception ex)
                {
                    result.Data = false;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                    {
                        if (ex.Message == "TargetEqualBaseline")
                        {
                            if (Language == "en")
                            {
                                result.StatusMessage = "Target, Sum Of Targets, or Average of Targets can not be equal to baseline.";
                            }
                            else
                            {
                                result.StatusMessage = "لا يمكن أن يكون الهدف أو مجموع الأهداف أو متوسط ​​الأهداف مساوياً لخط الأساس.";
                            }
                        }
                        else
                        {
                            result.StatusMessage = ex.Message;
                        }
                    }
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }



         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("MitigationAction")]
        public ResultWrapper<bool> MitigationAction(MitigationActionRequestFormDTO mitigationAction)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                var Credential = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = RequestUseCases.MitigationAction(mitigationAction, Credential);
                    Task.Run(async () => { await RequestUseCases.Log(Token, "Submit", AuditTrailActionENums.Workflow); });
                }
                catch (Exception ex)
                {
                    result.Data = false;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        #endregion

        #region Read
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Request/KPI/{id}")]
        public ResultWrapper<List<RequestDTO>> ReadByKPIID(int id)
        {

            ResultWrapper<List<RequestDTO>> result = new ResultWrapper<List<RequestDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = RequestUseCases.GetUpdateKPIRequestsByKPIID(id, userName);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = new List<RequestDTO>();
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    result.StatusMessage = ex.Message;
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Request/{id}")]
        public ResultWrapper<RequestDetailsDTO> ReadByID(int id)
        {
            ResultWrapper<RequestDetailsDTO> result = new ResultWrapper<RequestDetailsDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = RequestUseCases.GetRequestByID(id, userName);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = new RequestDetailsDTO();
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    result.StatusMessage = ex.Message;
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }



         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Request/KPI/ChangeRequest")]
        public ResultWrapper<List<RequestDetailsDTO>> ReadSubmitedChangeRequest()
        {
            ResultWrapper< List<RequestDetailsDTO>> result = new ResultWrapper<List<RequestDetailsDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = RequestUseCases.ReadSubmitedChangeRequest();
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = new List<RequestDetailsDTO>();
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    result.StatusMessage = ex.Message;
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Report/Request/KPI/ChangeRequest")]
        public ResultWrapper<List<RequestDetailsDTO>> ReportChangeRequests()
        {
            ResultWrapper<List<RequestDetailsDTO>> result = new ResultWrapper<List<RequestDetailsDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = RequestUseCases.ReportChangeRequests(userName);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = new List<RequestDetailsDTO>();
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    result.StatusMessage = ex.Message;
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("UpdateRequest/{id}")]
        public ResultWrapper<RequestDetailsDTO> ReadUpdateKPIRequestByID(int id)
        {
            ResultWrapper<RequestDetailsDTO> result = new ResultWrapper<RequestDetailsDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = RequestUseCases.GetRequestByIDUpdateKPIForm(id, userName);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = new RequestDetailsDTO();
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    result.StatusMessage = ex.Message;
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }




        // [OperationContract]  [DataFormatingInvoker]
        //[BasicAuthenticationInvoker]
        ////[BasicAuthenticationInvoker()]
        //[HttpGet]
        //[Route("TaskCenter")]
        //public ResultWrapper<TaskCenterDTO> GetTaskCenter()
        //{
        //    ResultWrapper<TaskCenterDTO> result = new ResultWrapper<TaskCenterDTO>();
        //    IEnumerable<string> Token;
        //    if (Request.Headers.TryGetValues("Token", out Token))
        //    {
        //        string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
        //        try
        //        {
        //            var res = TaskCenterUseCases.GetTaskCenter(userName);
        //            result.Data = res;
        //        }
        //        catch (Exception ex)
        //        {
        //            result.Data = new TaskCenterDTO();
        //            result.ErrorCode = "0000";
        //            result.StatusCode = "fail";
        //            result.StatusMessage = ex.Message;
        //            if (ex.InnerException == null)
        //                RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
        //            else if (ex.InnerException.InnerException == null)
        //                RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
        //            else
        //                RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
        //        }
        //    }
        //    return result;
        //}





        // [OperationContract]  [DataFormatingInvoker]
        //[BasicAuthenticationInvoker]
        //[HttpGet]
        //[Route("TaskCenter/MyRequest/{type}")]
        //public ResultWrapper<TaskCenterListDTO<List<MyRequestDTO>>> GetTaskCenterMyRequests(string type)
        //{
        //    ResultWrapper<TaskCenterListDTO<List<MyRequestDTO>>> result = new ResultWrapper<TaskCenterListDTO<List<MyRequestDTO>>>();
        //    IEnumerable<string> Token;
        //    if (Request.Headers.TryGetValues("Token", out Token))
        //    {
        //        string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
        //        try
        //        {
        //            var res = TaskCenterUseCases.MyRequests(userName, type);
        //            result.Data = res;
        //        }
        //        catch (Exception ex)
        //        {
        //            result.Data = new TaskCenterListDTO<List<MyRequestDTO>>();
        //            result.ErrorCode = "0000";
        //            result.StatusCode = "fail";
        //            result.StatusMessage = ex.Message;
        //            if (ex.InnerException == null)
        //                RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
        //            else if (ex.InnerException.InnerException == null)
        //                RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
        //            else
        //                RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
        //        }
        //    }
        //    return result;
        //}


         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("TaskCenter/UpdateKPI")]
        public ResultWrapper<List<UpdateListDTO>> GetTaskCenterUpdateKPIs()
        {
            ResultWrapper<List<UpdateListDTO>> result = new ResultWrapper<List<UpdateListDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = TaskCenterUseCases.GetKPIUpdates(userName);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = new List<UpdateListDTO>();
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    result.StatusMessage = ex.Message;
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }



         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("TaskCenter/Approval/{type}")]
        public ResultWrapper<TaskCenterListDTO<List<ApprovalDTO>>> GetTaskCenterApprovals(string type)
        {
            ResultWrapper<TaskCenterListDTO<List<ApprovalDTO>>> result = new ResultWrapper<TaskCenterListDTO<List<ApprovalDTO>>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = TaskCenterUseCases.GetApprovalList(userName, type);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = new TaskCenterListDTO<List<ApprovalDTO>>();
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    result.StatusMessage = ex.Message;
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


        [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("TaskCenter/MyRequest/{type}")]
        public ResultWrapper<TaskCenterDTO<List<MyRequestDTO>>> GetTaskCenterMyRequests(string type)
        {
            ResultWrapper<TaskCenterDTO<List<MyRequestDTO>>> result = new ResultWrapper<TaskCenterDTO<List<MyRequestDTO>>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = TaskCenterUseCases.MyRequests(userName, type);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = new TaskCenterDTO<List<MyRequestDTO>>();
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    result.StatusMessage = ex.Message;
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


        #endregion

        #region Action
        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        [Route("Request/DoAction")]
        public ResultWrapper<int> DoAction(ActionDTO Action)
        {
            ResultWrapper<int> result = new ResultWrapper<int>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                var Credential = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    Action.Action = RequestUseCases.DoAction(Action, Credential);

                    Task.Run(async () => { await RequestUseCases.Log(Token, "DoAction", AuditTrailActionENums.Workflow); });
                    result.Data = Action.Action;
                }
                catch (Exception ex)
                {
                    result.Data = 0;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }



        [OperationContract]
        //[DataFormatingInvoker]
      //  [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Request/CorrectValue")]
        public ResultWrapper<bool> CorrectValue()
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
           // if (Request.Headers.TryGetValues("Token", out Token))
            {
                //var Credential = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                   RequestUseCases.CorrectValue( );

             
                    result.Data = true;
                }
                catch (Exception ex)
                {
                    result.Data = false;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        RequestUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }



        [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [BasicAuthenticationInvoker]
        [Route("CalculateParameterValue")]
        public ResultWrapper<decimal> CalculateParameterValue(List<ParameterDTO> parameters)
        {

            ResultWrapper<decimal> result = new ResultWrapper<decimal>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = RequestUseCases.FormulateKPIFormula(parameters, userName);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = default;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";

                    if (ex.InnerException == null)
                        KPIUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        KPIUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        KPIUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [BasicAuthenticationInvoker]
        [Route("ValidateFormula")]
        public ResultWrapper<bool> ValidateFormula(ValidateParameterDTO expression)
        {

            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = RequestUseCases.ValidateKPIFormula(expression);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = false;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                        result.StatusMessage = ex.Message;
                    else
                        result.StatusCode = "An error has occured";

                    if (ex.InnerException == null)
                        KPIUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        KPIUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        KPIUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }
        #endregion


    }
}

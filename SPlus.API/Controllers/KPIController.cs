using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SPlus.Model;
using SPlus.UseCases;
using SPlus.API.Attribute;
using System.Web.Http.Cors;
using SPlus.Helper;
using System.Threading.Tasks;
using SPlus.DTO;
using System.Data.Entity.Validation;
using model = SPlus.Model;
using StructureMap;
using AutoMapper;

namespace SPlus.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class KPIController : ApiController
    {
        Container _Container = IOC.InitializeContainer();
        private readonly KPIUseCases KPIUseCases;
        public KPIController()
        {
            KPIUseCases = _Container.GetInstance<KPIUseCases>();
        }

        #region Create

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("KPI")]
        public ResultWrapper<CAKPIDTO> Create(CAKPIDTO _KPI)
        {
            ResultWrapper<CAKPIDTO> result = new ResultWrapper<CAKPIDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                var Language = Request.Headers.TryGetValues("Language", out var HeaderLanguage) ? HeaderLanguage.FirstOrDefault().ToString() : "en";
                try
                {
                    string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());

                    result.Data = KPIUseCases.Create(_KPI, userName);

                    Task.Run(async () => { await KPIUseCases.Log(Token, result.Data, AuditTrailActionENums.Create); });
                }
                catch (DbEntityValidationException ex)
                {
                    Exception raise = ex;
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting  
                            // the current instance as InnerException  
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    if (ex.InnerException == null)
                        KPIUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        KPIUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        KPIUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    throw raise;
                }
                catch (Exception ex)
                {
                    result.Data = null;// new KPI();
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

                            if (ex.InnerException == null)
                                result.StatusMessage = ex.Message;
                            else if (ex.InnerException.InnerException == null)
                                result.StatusMessage = ex.InnerException.Message;
                            else
                                result.StatusMessage = ex.InnerException.InnerException.Message;
                        }
                    }
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


        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("KPI/Comment")]
        public ResultWrapper<KPICommentDTO> CreateComment(KPICommentDTO _KPIComment)
        {
            ResultWrapper<KPICommentDTO> result = new ResultWrapper<KPICommentDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                var Credential = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    string CurrentUserName;
                    string DelegationrUserName;
                    if (Credential.Count() != 3)
                    {
                        //CurrentUserName = Credential[0];
                        if (Credential[2] != "")
                            CurrentUserName = Credential[2];
                        else
                            CurrentUserName = Credential[0];
                        DelegationrUserName = Credential[0];
                    }
                    else
                    {
                        if (Credential[2] != "")
                            CurrentUserName = Credential[2];
                        else
                            CurrentUserName = Credential[0];
                        DelegationrUserName = Credential[0];
                    }
                    result.Data = KPIUseCases.CreateComment(_KPIComment, DelegationrUserName);
                    Task.Run(async () => { await KPIUseCases.Log(Token, result.Data, AuditTrailActionENums.Create); });
                }
                catch (Exception ex)
                {
                    result.Data = new KPICommentDTO();
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



        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker()]
        [HttpPost]
        [Route("KPI/KPIAffect/{Delete}")]
        public ResultWrapper<List<KPIAffectDTO>> ConnectWithAffect(List<KPIAffectDTO> _KPIAffectDTO, bool Delete)
        {
            ResultWrapper<List<KPIAffectDTO>> result = new ResultWrapper<List<KPIAffectDTO>>();
            try
            {
                result.Data = KPIUseCases.ConnectWithAffect(_KPIAffectDTO, Delete);
            }
            catch (Exception ex)
            {
                result.Data = new List<KPIAffectDTO>();
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
            return result;
        }



        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("KPI/UpdateKPIPeriod_FromMigration")]
        public ResultWrapper<bool> UpdateKPIPeriod_FromMigration()
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                var Credential = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    string CurrentUserName;
                    string DelegationrUserName;
                    if (Credential.Count() != 3)
                    {
                        //CurrentUserName = Credential[0];
                        if (Credential[2] != "")
                            CurrentUserName = Credential[2];
                        else
                            CurrentUserName = Credential[0];
                        DelegationrUserName = Credential[0];
                    }
                    else
                    {
                        if (Credential[2] != "")
                            CurrentUserName = Credential[2];
                        else
                            CurrentUserName = Credential[0];
                        DelegationrUserName = Credential[0];
                    }
                    result.Data = KPIUseCases.UpdateKPIPeriod_FromMigration();
                    Task.Run(async () => { await KPIUseCases.Log(Token, result.Data, AuditTrailActionENums.Create); });
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

        #region Read


        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("KPI/KPIAffect")]
        public ResultWrapper<List<KPIAffectDTO>> ReadKPIAffect()
        {
            ResultWrapper<List<KPIAffectDTO>> result = new ResultWrapper<List<KPIAffectDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = KPIUseCases.ReadWithAffect();
                    result.Data = res;

                }
                catch (Exception ex)
                {
                    result.Data = null;
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



        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("KPI")]
        public ResultWrapper<List<CAKPIListingDTO>> Read()
        {
            ResultWrapper<List<CAKPIListingDTO>> result = new ResultWrapper<List<CAKPIListingDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = KPIUseCases.Read(userName);
                    result.Data = res;

                }
                catch (Exception ex)
                {
                    result.Data = null;
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



        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("KPI/Divisional")]
        public ResultWrapper<List<CAKPIListingDTO>> ReadDivisional(int? Year = null)
        {
            ResultWrapper<List<CAKPIListingDTO>> result = new ResultWrapper<List<CAKPIListingDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = KPIUseCases.ReadByType(userName, "divisional", Year);
                    result.Data = res;

                }
                catch (Exception ex)
                {
                    result.Data = null;
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


        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("KPI/Strategic")]
        public ResultWrapper<List<CAKPIListingDTO>> ReadStrategic(int? Year = null)
        {
            ResultWrapper<List<CAKPIListingDTO>> result = new ResultWrapper<List<CAKPIListingDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = KPIUseCases.ReadByType(userName, "strategic", Year);
                    result.Data = res;

                }
                catch (Exception ex)
                {
                    result.Data = null;
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

        [OperationContract]
        [BasicAuthenticationInvoker]
        [DataFormatingInvoker]
        [HttpGet]
        [Route("KPI/{id}")]
        public ResultWrapper<KPIDetailsDTO> ReadByID(int id)
        {
            ResultWrapper<KPIDetailsDTO> result = new ResultWrapper<KPIDetailsDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = KPIUseCases.ReadByID(id, userName);
                    result.Data = res;

                }
                catch (Exception ex)
                {
                    result.Data = null;
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




        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("KPI/ChageRequestKPIs")]
        public ResultWrapper<List<KPIDetailsDTO>> ReadChangeRequestKpis(int? Year = null)
        {
            ResultWrapper<List<KPIDetailsDTO>> result = new ResultWrapper<List<KPIDetailsDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = KPIUseCases.ReadChangeRequestKpis(userName, Year);
                    result.Data = res;

                }
                catch (Exception ex)
                {
                    result.Data = null;
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


        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("KPI/MitigationAction")]
        public ResultWrapper<List<CAKPIListingDTO>> ReadMitigationActionKpis(int? Year = null)
        {
            ResultWrapper<List<CAKPIListingDTO>> result = new ResultWrapper<List<CAKPIListingDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = KPIUseCases.ReadMitigationActionKpis(Year);
                    result.Data = res;

                }
                catch (Exception ex)
                {
                    result.Data = null;
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

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Report/KPIs")]
        public ResultWrapper<List<KPIDetailsDTO>> ReadReportKPIs()
        {
            ResultWrapper<List<KPIDetailsDTO>> result = new ResultWrapper<List<KPIDetailsDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = KPIUseCases.ReadReportKPIs(userName);
                    result.Data = res;

                }
                catch (Exception ex)
                {
                    result.Data = null;
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



        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("KPI/Measures")]
        public ResultWrapper<List<KPIMeasureDTO>> ReadMeasures()
        {

            ResultWrapper<List<KPIMeasureDTO>> result = new ResultWrapper<List<KPIMeasureDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    // var res = KPIBLLNew.ReadMeasure();
                    //var x = AutoMapper.Mapper.Map<KPIDTO>(res[0]..FirstOrDefault());
                    //List<KPIMeasureDTO> KPIs = AutoMapper.Mapper.Map<List<KPIMeasureDTO>>(res);
                    //result.Data = KPIs;
                }
                catch (Exception ex)
                {
                    result.Data = null;
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

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("KPI/Comment/{KPIID}")]
        public ResultWrapper<List<KPICommentDTO>> ReadComments(int KPIID)
        {

            ResultWrapper<List<KPICommentDTO>> result = new ResultWrapper<List<KPICommentDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = KPIUseCases.ReadComments(KPIID, userName);
                    result.Data = res;

                }
                catch (Exception ex)
                {
                    result.Data = null;
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

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("KPI/KPICardByObjective/{id}")]
        public ResultWrapper<List<KPICardDTO>> KPICardByObjectiveID(int id, bool onlyMine = false, int? Year = null)
        {
            ResultWrapper<List<KPICardDTO>> result = new ResultWrapper<List<KPICardDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = KPIUseCases.KPICardByObjectiveID(id, userName, onlyMine, Year);
                    result.Data = res;
                }
                catch (AutoMapperMappingException ex)
                {
                    result.StatusMessage = ex.Message;
                }
                catch (Exception ex)
                {
                    result.Data = null;
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



        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("KPI/KPICardByDivisionalObjective/{id}")]
        public ResultWrapper<List<KPICardDTO>> KPICardByDivisionalObjective(int id, bool onlyMine = false, int? Year = null)
        {
            ResultWrapper<List<KPICardDTO>> result = new ResultWrapper<List<KPICardDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = KPIUseCases.KPICardByDivisionalObjective(id, userName, onlyMine, Year);
                    result.Data = res;
                }
                catch (AutoMapperMappingException ex)
                {
                    result.StatusMessage = ex.Message;
                }
                catch (Exception ex)
                {
                    result.Data = null;
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

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("KPI/ValidYears")]
        public ResultWrapper<List<int>> GetValidYears()
        {
            ResultWrapper<List<int>> result = new ResultWrapper<List<int>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = KPIUseCases.GetValidYears(userName);
                    result.Data = res;
                }
                catch (AutoMapperMappingException ex)
                {
                    result.StatusMessage = ex.Message;
                }
                catch (Exception ex)
                {
                    result.Data = new List<int>() { DateTime.Now.Year };
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

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("RangeYears")]
        public ResultWrapper<List<int>> GetRangeYears()
        {
            ResultWrapper<List<int>> result = new ResultWrapper<List<int>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    List<int> YearRange = new List<int>();
                    int currentYear = DateTime.Now.Year;
                    for (int i = currentYear - 5; i <= currentYear + 5; i++)
                    {
                        YearRange.Add(i);
                    }
                    YearRange.Sort();
                    result.Data = YearRange;
                }
                catch (AutoMapperMappingException ex)
                {
                    result.StatusMessage = ex.Message;
                }
                catch (Exception ex)
                {
                    result.Data = new List<int>() { DateTime.Now.Year };
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

        #region Update

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        [Route("KPI")]
        public ResultWrapper<CAKPIDTO> Update(CAKPIDTO _KPI)
        {
            ResultWrapper<CAKPIDTO> result = new ResultWrapper<CAKPIDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                var Credential = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                var Language = Request.Headers.TryGetValues("Language", out var HeaderLanguage) ? HeaderLanguage.FirstOrDefault().ToString() : "en";
                try
                {
                    string CurrentUserName;
                    string DelegationrUserName;
                    if (Credential.Count() != 3)
                    {
                        //CurrentUserName = Credential[0];
                        if (Credential[2] != "")
                            CurrentUserName = Credential[2];
                        else
                            CurrentUserName = Credential[0];
                        DelegationrUserName = Credential[0];
                    }
                    else
                    {
                        if (Credential[2] != "")
                            CurrentUserName = Credential[2];
                        else
                            CurrentUserName = Credential[0];
                        DelegationrUserName = Credential[0];
                    }
                    result.Data = KPIUseCases.Update(_KPI, DelegationrUserName);
                    Task.Run(async () => { await KPIUseCases.Log(Token, _KPI, AuditTrailActionENums.Update); });
                }
                catch (Exception ex)
                {
                    result.Data = null;// new KPI();
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
                        KPIUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        KPIUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        KPIUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;

        }

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        [Route("KPI/Comment")]
        public ResultWrapper<KPICommentDTO> UpdateComment(KPICommentDTO KPIComment)
        {
            ResultWrapper<KPICommentDTO> result = new ResultWrapper<KPICommentDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = KPIUseCases.UpdateComment(KPIComment, userName);
                    result.Data = res;


                    Task.Run(async () => { await KPIUseCases.Log(Token, res, AuditTrailActionENums.Update); });

                }
                catch (Exception ex)
                {
                    result.Data = null;// new KPI();
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

        #region Delete

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpDelete]
        [Route("KPI/{id}")]
        public ResultWrapper<bool> Delete(int id)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = KPIUseCases.Delete(id);


                    Task.Run(async () => { await KPIUseCases.Log(Token, "KPI", AuditTrailActionENums.Delete, id); });
                }
                catch (Exception ex)
                {
                    result.Data = false;// new bool();
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

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpDelete]
        [Route("KPI/Comment/{id}")]
        public ResultWrapper<bool> DeleteComment(int id)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = KPIUseCases.DeleteComment(id);


                    Task.Run(async () => { await KPIUseCases.Log(Token, "KPIComment", AuditTrailActionENums.Delete, id); });
                }
                catch (Exception ex)
                {
                    result.Data = false;// new bool();
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

        #region Processes



        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        [Route("KPI/UnlockKPI/{id}")]
        public ResultWrapper<bool> UnlockKPI(int id)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            try
            {
                result.Data = KPIUseCases.UnlockKPI(id);
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
                // _ExceptionBLL.AddSharePointException("Exception", ex.Message, "KPI", "UnlockKPI");
            }
            return result;
        }

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        [Route("KPI/lockKPI/{id}")]
        public ResultWrapper<bool> lockKPI(int id)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            try
            {
                result.Data = KPIUseCases.LockKPI(id);
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
                // _ExceptionBLL.AddSharePointException("Exception", ex.Message, "KPI", "LockKPI");
            }
            return result;
        }

        #endregion


    }
}

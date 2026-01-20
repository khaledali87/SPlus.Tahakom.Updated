using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SPlus.API.Attribute;
using System.Web.Http.Cors;
using SPlus.Helper;
using SPlus.DTO;
using SPlus.Model;
using System.Threading.Tasks;
using SPlus.UseCases;
using StructureMap;
using SPlus.DTO.Delegation;

namespace SPlus.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DelegationController : ApiController
    {

        Container _Container = IOC.InitializeContainer();
        private readonly DelegationUseCases DelegationUseCases;
        public DelegationController()
        {
            DelegationUseCases = _Container.GetInstance<DelegationUseCases>();
        }
        #region Create

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        public ResultWrapper<DelegationDTO> Create(DelegationDTO delegation)
        {
            ResultWrapper<DelegationDTO> result = new ResultWrapper<DelegationDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                var Credentials = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    delegation.FromUser = new UserDTO();
                    delegation.FromUser.UserName = Credentials[0].ToLower();
                    result.Data = DelegationUseCases.Create(Credentials[0], delegation);

                    Task.Run(async () => { await DelegationUseCases.Log(Token, result.Data, AuditTrailActionENums.Create); });
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
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        [Route("Delegation/ActivateDelegation/{id}")]
        public ResultWrapper<ActivatedDelegationDTO> ActivateDelegation(int id)
        {
            ResultWrapper<ActivatedDelegationDTO> result = new ResultWrapper<ActivatedDelegationDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                //string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                var Credentials = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                string SSOToken=string.Empty;
                if (Request.Headers.Contains("ssotoken"))
                {
                    SSOToken = Request.Headers.GetValues("ssotoken").FirstOrDefault() + "";
                }
                try
                {
                    var res = DelegationUseCases.ActivateDelegation(id, Token.FirstOrDefault().ToString(), SSOToken);
                    result.Data = res;

                    Task.Run(async () => { await DelegationUseCases.Log(Token, res, AuditTrailActionENums.Create); });

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
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
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
        [Route("Delegation/ActiveDelegation")]
        public ResultWrapper<List<ActiveDelegationDTO>> GetActiveDelegations()
        {
            IEnumerable<string> Token;
            ResultWrapper<List<ActiveDelegationDTO>> result = new ResultWrapper<List<ActiveDelegationDTO>>();
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                var Credentials = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());

                try
                {
                    var res = DelegationUseCases.GetActiveDelegations(Credentials[0]);
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
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;

        }


        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        public ResultWrapper<SystemDelegationDTO> Read()
        {
            IEnumerable<string> Token;
            ResultWrapper<SystemDelegationDTO> result = new ResultWrapper<SystemDelegationDTO>();
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                var Credentials = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());

                try
                {
                    var res = DelegationUseCases.Read(Credentials[0]);
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
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;

        }

        [OperationContract]
        [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        public ResultWrapper<DelegationDTO> ReadByID(int id)
        {
            ResultWrapper<DelegationDTO> result = new ResultWrapper<DelegationDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = DelegationUseCases.ReadByID(id);
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
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
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
        public ResultWrapper<DelegationDTO> Update(DelegationDTO delegation)
        {
            ResultWrapper<DelegationDTO> result = new ResultWrapper<DelegationDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                var Credentials = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    delegation.FromUser = new UserDTO();
                    delegation.FromUser.UserName = Credentials[0].ToLower();
                    var res = DelegationUseCases.Update(delegation);
                    result.Data = res;

                    Task.Run(async () => { await DelegationUseCases.Log(Token, res, AuditTrailActionENums.Update); });

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
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
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
        public ResultWrapper<bool> Delete(int id)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = DelegationUseCases.Delete(id);

                    Task.Run(async () => { await DelegationUseCases.Log(Token, "Delegation", AuditTrailActionENums.Delete, id); });

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
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        DelegationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        #endregion
    }
}

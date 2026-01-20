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
using SPlus.DTO;
using SPlus.Helper;
using System.Threading.Tasks;
using StructureMap;
using DocumentFormat.OpenXml.Bibliography;

namespace SPlus.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class StrategyController : ApiController
    {
        Container _Container = IOC.InitializeContainer();
        private readonly StrategyUseCases StrategyUseCases;
        public StrategyController()
        {
            StrategyUseCases = _Container.GetInstance<StrategyUseCases>();
        }

        #region Create

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        public ResultWrapper<CAStrategyDTO> Create(CAStrategyDTO strategy)
        {
            ResultWrapper<CAStrategyDTO> result = new ResultWrapper<CAStrategyDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = StrategyUseCases.Create(strategy);
                    Task.Run(async () => { await StrategyUseCases.Log(Token, result.Data, AuditTrailActionENums.Create); });
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
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        #endregion

        #region Read

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        public ResultWrapper<List<CAStrategyDTO>> Read(int? Year = null)
        {
            ResultWrapper<List<CAStrategyDTO>> result = new ResultWrapper<List<CAStrategyDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string[] Credintials = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategyUseCases.Read(Year);
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
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }
         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Strategy/Themes")]
        public ResultWrapper<List<CAStrategyThemeDTO>> ReadWithTheme(int? Year = null)
        {
            ResultWrapper<List<CAStrategyThemeDTO>> result = new ResultWrapper<List<CAStrategyThemeDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string[] Credintials = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategyUseCases.ReadWithTheme(Year);
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
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Strategy/Perspectives")]
        public ResultWrapper<List<CAStrategyPerspectiveDTO>> ReadWithPerspective(int? Year = null)
        {
            ResultWrapper<List<CAStrategyPerspectiveDTO>> result = new ResultWrapper<List<CAStrategyPerspectiveDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string[] Credintials = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategyUseCases.ReadWithPerspective(Year);
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
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Strategy/Objectives")]
        public ResultWrapper<List<CAStrategyObjectiveDTO>> ReadWithObjectives(bool withOperational = false, int? Year = null)
        {
            ResultWrapper<List<CAStrategyObjectiveDTO>> result = new ResultWrapper<List<CAStrategyObjectiveDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string[] Credintials = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategyUseCases.ReadWithObjective(Year);
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
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }



         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Strategy/DivisionalObjectives")]
        public ResultWrapper<List<CAOrgStructureDivisionalObjectiveDTO>> ReadWithDivisionalObjectives(int? Year = null)
        {
            ResultWrapper<List<CAOrgStructureDivisionalObjectiveDTO>> result = new ResultWrapper<List<CAOrgStructureDivisionalObjectiveDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string[] Credintials = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategyUseCases.ReadWithDivisionalObjectives(Year);
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
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }







         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Strategy/StrategicObjectivesList")]
        public ResultWrapper<List<CAStrategyObjectiveListDTO>> ReadWithObjectivesList(int? Year = null)
        {
            ResultWrapper<List<CAStrategyObjectiveListDTO>> result = new ResultWrapper<List<CAStrategyObjectiveListDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string[] Credintials = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategyUseCases.ReadWithObjectiveList(Year);
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
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        public ResultWrapper<CAStrategyDTO> ReadByID(int id)
        {
            ResultWrapper<CAStrategyDTO> result = new ResultWrapper<CAStrategyDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategyUseCases.ReadByID(id);
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
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("StrategyMap/{StrategyID}")]
        public ResultWrapper<StrategyMapDTO> StrategyMap(int StrategyID, int? Year = null)
        {
            ResultWrapper<StrategyMapDTO> result = new ResultWrapper<StrategyMapDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategyUseCases.StrategyMap(StrategyID, userName, Year);
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
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }
        #endregion

        #region Update

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        public ResultWrapper<CAStrategyDTO> Update(CAStrategyDTO strategy)
        {
            ResultWrapper<CAStrategyDTO> result = new ResultWrapper<CAStrategyDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = StrategyUseCases.Update(strategy);
                    result.Data = res;

                    Task.Run(async () => { await StrategyUseCases.Log(Token, res, AuditTrailActionENums.Update); });

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
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }
        #endregion

        #region Delete

         [OperationContract]  [DataFormatingInvoker]
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
                    result.Data = StrategyUseCases.Delete(id);

                    Task.Run(async () => { await StrategyUseCases.Log(Token, "Strategy", AuditTrailActionENums.Delete, id); });

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
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        StrategyUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        #endregion
    }
}

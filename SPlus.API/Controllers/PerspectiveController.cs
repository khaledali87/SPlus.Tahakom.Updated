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

namespace SPlus.API.Controllers
{
   [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PerspectiveController : ApiController
    {

        Container _Container = IOC.InitializeContainer();
        private readonly PerspectiveUseCases PerspectiveUseCases;
        public PerspectiveController()
        {
            PerspectiveUseCases = _Container.GetInstance<PerspectiveUseCases>();
        }
        #region Create

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPost]
        public ResultWrapper<CAPerspectiveDTO> Create(CAPerspectiveDTO _Perspective)
        {
            ResultWrapper<CAPerspectiveDTO> result = new ResultWrapper<CAPerspectiveDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = PerspectiveUseCases.Create(_Perspective);
                    if (result.Data.ID == -1 && result.Data.EnglishName == "Order Must be Unique")
                    {
                        result.Data = null;
                        result.ErrorCode = "0000";
                        result.StatusCode = "fail";
                        result.StatusMessage = "Order Must be Unique";
                    }
                    Task.Run(async () => { await PerspectiveUseCases.Log(Token, result.Data, AuditTrailActionENums.Create); });

                }
                catch (Exception ex)
                {
                    result.Data = null;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    if (Constants._Error)
                    {
                        if (ex.InnerException == null)
                            result.StatusMessage = ex.Message;
                        else if (ex.InnerException.InnerException == null)
                            result.StatusMessage = ex.InnerException.Message;
                        else
                            result.StatusMessage = ex.InnerException.InnerException.Message;
                    }
                    else
                        result.StatusCode = "An error has occured";
                    if (ex.InnerException == null)
                        PerspectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        PerspectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        PerspectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        #endregion

        #region Read

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        public ResultWrapper<List<CAPerspectiveDTO>> Read()
        {
            ResultWrapper<List<CAPerspectiveDTO>> result = new ResultWrapper<List<CAPerspectiveDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string[] Credintials = Encryption.GetCredentialsFromSecurityToken(Token.FirstOrDefault().ToString());
                try
                {
                    var res = PerspectiveUseCases.Read();
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
                        PerspectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        PerspectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        PerspectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        public ResultWrapper<CAPerspectiveDTO> ReadByID(int id)
        {
            ResultWrapper<CAPerspectiveDTO> result = new ResultWrapper<CAPerspectiveDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = PerspectiveUseCases.ReadByID(id);
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
                        PerspectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        PerspectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        PerspectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        #endregion

        #region Update

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        public ResultWrapper<CAPerspectiveDTO> Update(CAPerspectiveDTO Perspective)
        {
            ResultWrapper<CAPerspectiveDTO> result = new ResultWrapper<CAPerspectiveDTO>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = PerspectiveUseCases.Update(Perspective);
                    result.Data = res;
                    if (result.Data.ID == -1 && result.Data.EnglishName == "Order Must be Unique")
                    {
                        result.Data = null;
                        result.ErrorCode = "0000";
                        result.StatusCode = "fail";
                        result.StatusMessage = "Order Must be Unique";
                    }
                    Task.Run(async () => { await PerspectiveUseCases.Log(Token, res, AuditTrailActionENums.Update); });

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
                        PerspectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        PerspectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        PerspectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
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
                    result.Data = PerspectiveUseCases.Delete(id);

                    Task.Run(async () => { await PerspectiveUseCases.Log(Token, "Perspective", AuditTrailActionENums.Delete,id); });

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
                        PerspectiveUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        PerspectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        PerspectiveUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        #endregion
    }
}

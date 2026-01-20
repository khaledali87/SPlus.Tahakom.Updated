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
using StructureMap;
using SPlus.Helper;

namespace SPlus.API.Controllers
{
   [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class NotificationController : ApiController
    {
        Container _Container = IOC.InitializeContainer();
        private readonly NotificationUseCases NotificationUseCases;
        private readonly NotificationConfigurationUseCases NotificationConfigurationUseCases;
        public NotificationController()
        {
            NotificationUseCases = _Container.GetInstance<NotificationUseCases>();
            NotificationConfigurationUseCases = _Container.GetInstance<NotificationConfigurationUseCases>();

        }
        #region New 


         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        public ResultWrapper<List<NotificationDTO>> Read()
        {
            ResultWrapper<List<NotificationDTO>> result = new ResultWrapper<List<NotificationDTO>>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    var res = NotificationUseCases.GetAllNotifications(userName);
                    result.Data = res;
                }
                catch (Exception ex)
                {
                    result.Data = null;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    result.StatusMessage = ex.Message;
                    if (ex.InnerException == null)
                        NotificationUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        NotificationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        NotificationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Notification/Count")]
        public ResultWrapper<int> NotificationsCount()
        {
            ResultWrapper<int> result = new ResultWrapper<int>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = NotificationUseCases.GetAllNotifications(userName).Count();
                }
                catch (Exception ex)
                {
                    result.Data = 0;
                    result.ErrorCode = "0000";
                    result.StatusCode = "fail";
                    result.StatusMessage = ex.Message;
                    if (ex.InnerException == null)
                        NotificationUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        NotificationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        NotificationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("NotificationConfiguration")]
        public ResultWrapper<List<NotificationConfigutrationListDTO>> NotificationConfiguration()
        {
            ResultWrapper<List<NotificationConfigutrationListDTO>> result = new ResultWrapper<List<NotificationConfigutrationListDTO>>();
            try
            {
                var res = NotificationConfigurationUseCases.Read();
                result.Data = res;
            }
            catch (Exception ex)
            {
                result.Data = null;// new bool();
                result.ErrorCode = "0000";
                result.StatusCode = "fail";
                if (Constants._Error)
                    result.StatusMessage = ex.Message;
                else
                    result.StatusCode = "An error has occured";
                if (ex.InnerException == null)
                    NotificationConfigurationUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else if (ex.InnerException.InnerException == null)
                    NotificationConfigurationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else
                    NotificationConfigurationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }


         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("NotificationConfiguration/{id}")]
        public ResultWrapper<NotificationConfigurationDTO> NotificationConfiguration(int id)
        {
            ResultWrapper<NotificationConfigurationDTO> result = new ResultWrapper<NotificationConfigurationDTO>();
            try
            {
                var res = NotificationConfigurationUseCases.ReadByID(id);
                result.Data = res;
            } 
            catch (Exception ex)
            {
                result.Data = null;// new bool();
                result.ErrorCode = "0000";
                result.StatusCode = "fail";
                if (Constants._Error)
                    result.StatusMessage = ex.Message;
                else
                    result.StatusCode = "An error has occured";
                if (ex.InnerException == null)
                    NotificationConfigurationUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else if (ex.InnerException.InnerException == null)
                    NotificationConfigurationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else
                    NotificationConfigurationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        [Route("NotificationConfiguration")]
        public ResultWrapper<NotificationConfigurationDTO> NotificationConfiguration(NotificationConfigurationDTO _NotificationConfigurationDTO)
        {
            ResultWrapper<NotificationConfigurationDTO> result = new ResultWrapper<NotificationConfigurationDTO>();
            try
            {
                var res = NotificationConfigurationUseCases.Update(_NotificationConfigurationDTO);
                result.Data = res;
            }
            catch (Exception ex)
            {
                result.Data = null;// new bool();
                result.ErrorCode = "0000";
                result.StatusCode = "fail";
                if (Constants._Error)
                    result.StatusMessage = ex.Message;
                else
                    result.StatusCode = "An error has occured";
                if (ex.InnerException == null)
                    NotificationConfigurationUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else if (ex.InnerException.InnerException == null)
                    NotificationConfigurationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else
                    NotificationConfigurationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }


         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpGet]
        [Route("Notification/IsAvailable")]
        public ResultWrapper<bool> IsAvailable()
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = NotificationUseCases.IsAvailable(userName);
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
                        NotificationUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        NotificationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        NotificationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }


         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker]
        [HttpPut]
        [Route("Notification/{id}")]
        public ResultWrapper<bool> UpdateNotification(int id)
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = NotificationUseCases.UpdateNotification(id);
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
                        NotificationUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        NotificationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        NotificationUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.Cors;
using SPlus.Model;
using SPlus.UseCases;
using SPlus.API.Attribute;
using System.Threading;
using SPlus.DTO;
using StructureMap;
using SPlus.Helper;
using SPlus.DTO.Central_Administrartion.Permissions.User;

namespace SPlus.API.Controllers
{
   [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {

        Container _Container = IOC.InitializeContainer();
        private readonly LoginUseCases LoginUseCases;
        public LoginController()
        {
            LoginUseCases = _Container.GetInstance<LoginUseCases>();
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker(CheckSecurityToken = false)]
        [HttpPost]
        public ResultWrapper<LoginDTO> Login(UserDTO _user)
        {
            ResultWrapper<LoginDTO> result = new ResultWrapper<LoginDTO>();
            try
            {
                
                result.Data = LoginUseCases.LoginUser(_user);

                if (result.Data.Token == "NOT Authorized")
                {
                    result.ErrorCode = "401";
                    result.StatusCode = "fail";
                    result.StatusMessage = "Username or password is incorrect";
                }
                else if (result.Data.Token == "NOT Authenticated")
                {
                    result.ErrorCode = "404";
                    result.StatusCode = "fail";
                    result.StatusMessage = "Username or password is incorrect";
                }
                else if (result.Data.Token == "NOT Authenticated and NOT Authorized")
                {
                    result.ErrorCode = "401";
                    result.StatusCode = "fail";
                    result.StatusMessage = "Username or password is incorrect";
                }
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
                    LoginUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else if (ex.InnerException.InnerException == null)
                    LoginUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else
                    LoginUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker(CheckSecurityToken = false)]
        [HttpPost]
        [Route("Login/SSO")]
        public ResultWrapper<LoginDTO> LoginSSO(UserSSOLoginDTO data)
        {
            ResultWrapper<LoginDTO> result = new ResultWrapper<LoginDTO>();
            try
            {
                result.Data = LoginUseCases.LoginUserSSO(data.ssoToken);

                if (result.Data.Token == "NOT Authorized")
                {
                    result.ErrorCode = "401";
                    result.StatusCode = "fail";
                }
                else if (result.Data.Token == "NOT Authenticated")
                {
                    result.ErrorCode = "404";
                    result.StatusCode = "fail";
                }
                else if (result.Data.Token == "NOT Authenticated and NOT Authorized")
                {
                    result.ErrorCode = "401";
                    result.StatusCode = "fail";
                }
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
                    LoginUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else if (ex.InnerException.InnerException == null)
                    LoginUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else
                    LoginUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker(CheckSecurityToken = false)]
        [HttpPost]
        [Route("Login/Validate")]
        public ResultWrapper<LoginDTO> ValidateLogin(UserDTO _user)
        {
            ResultWrapper<LoginDTO> result = new ResultWrapper<LoginDTO>();
            try
            {

                result.Data = LoginUseCases.ValidateLogin(_user);

                if (result.Data.Token == "NOT Authorized")
                {
                    result.ErrorCode = "401";
                    result.StatusCode = "fail";
                    result.StatusMessage = "Username or password is incorrect";
                }
                else if (result.Data.Token == "NOT Authenticated")
                {
                    result.ErrorCode = "404";
                    result.StatusCode = "fail";
                    result.StatusMessage = "Username or password is incorrect";
                }
                else if (result.Data.Token == "NOT Authenticated and NOT Authorized")
                {
                    result.ErrorCode = "401";
                    result.StatusCode = "fail";
                    result.StatusMessage = "Username or password is incorrect";
                }
                 else if (result.Data.Token == "Code Is Expired")
                {
                    result.ErrorCode = "600";
                    result.StatusCode = "fail";
                    result.StatusMessage = "Code Is Expired";
                }

                else if (result.Data.Token == "User Is Blocked")
                {
                    result.ErrorCode = "600";
                    result.StatusCode = "fail";
                    result.StatusMessage = "User Is Blocked";
                }
                else if (result.Data.Token == "Invalid Code")
                {
                    result.ErrorCode = "601";
                    result.StatusCode = "fail";
                    result.StatusMessage = "Invalid Code";
                }
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
                    LoginUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else if (ex.InnerException.InnerException == null)
                    LoginUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                else
                    LoginUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }


         [OperationContract]  [DataFormatingInvoker]
        [BasicAuthenticationInvoker(CheckSecurityToken = false)]
        [HttpPost]
        [Route("logout")]
        public ResultWrapper<bool> Logout()
        {
            ResultWrapper<bool> result = new ResultWrapper<bool>();
            IEnumerable<string> Token;
            if (Request.Headers.TryGetValues("Token", out Token))
            {
                string userName = Encryption.GetCurrentUser(Token.FirstOrDefault().ToString());
                try
                {
                    result.Data = LoginUseCases.DeleteSession(userName);

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
                        LoginUseCases.CreateException("Exception", GetType().Name, ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else if (ex.InnerException.InnerException == null)
                        LoginUseCases.CreateException("Exception", GetType().Name, ex.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    else
                        LoginUseCases.CreateException("Exception", GetType().Name, ex.InnerException.InnerException.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return result;
        }

    }
}

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using SPlus.UseCases;
using SPlus.Helper;
using System.Configuration;
using SPlus.Model.Domain;

namespace SPlus.API.Attribute
{
    public class BasicAuthenticationInvoker : ActionFilterAttribute
    {
        #region Private Fields
        //ActiveDirectoryBLL _ActiveDirectoryBLL;
        LoginUseCases _LoginUseCases = new LoginUseCases();
        UserUseCases _UserUseCases = new UserUseCases();
        private HttpRequestMessage request;

        #endregion Private Fields       
        #region IOperationBehavior Members

        private bool checkSecurityToken = true;
        public bool CheckSecurityToken
        {
            get
            {
                return checkSecurityToken;
            }
            set
            {
                checkSecurityToken = value;
            }
        }

        #endregion IOperationInvoker Members

        private bool Authenticate()
        {
            try
            {
                if (CheckSecurityToken)
                {
                    _LoginUseCases = new LoginUseCases();
                    _UserUseCases = new UserUseCases();
                    string[] credentialsString = HttpContext.Current.Request.Headers.GetValues(Constants.hdr_Token);
                    string token = credentialsString == null ? "" : credentialsString[0];
                    bool ssoEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["ssoEnabled"]);

                    string[] userinfo = Encryption.GetCredentialsFromSecurityToken(token);

                    //string[] serviceCredentials = Authentication.GetServiceCredentials(HttpContext.Current.Request.Headers);

                    //if (serviceCredentials != null && serviceCredentials.Length > 1)
                    //{
                    var username = userinfo[0];
                    var password = userinfo[1];
                    var sessionID = Guid.Parse(userinfo[3]);
                    if (!ssoEnabled && this.CheckSecurityToken && !_LoginUseCases.Login(username, password, sessionID))
                    {
                        return false;
                    }
                    else
                    {
                        string ssoToken = HttpContext.Current.Request.Headers.GetValues(Constants.hdr_SSOToken)?.FirstOrDefault();
                        if (ssoEnabled && this.CheckSecurityToken && !_LoginUseCases.LoginSSO(ssoToken, sessionID))
                            return false;
                    }

                    Guid sessionid = Guid.Parse(userinfo[3]);
                    SPlus.Model.Domain.UserActivity userActivity = null;
                    if (!_LoginUseCases.ValidateSession(username, sessionid, out userActivity))
                        return false;
                    else
                        _LoginUseCases.UpdateSession(userActivity);

                    return true;
                }
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (!Authenticate())
            {
                actionExecutedContext.Response = new HttpResponseMessage();
                actionExecutedContext.Response.StatusCode = HttpStatusCode.Unauthorized;
                actionExecutedContext.Response.Content = null;
            }
        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            request = actionContext.Request;
            actionContext.Response = null;
        }

        private bool ValidateUser(string username, string password)
        {
            return true;
        }


    }
}
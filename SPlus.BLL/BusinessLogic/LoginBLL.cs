using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using SPlus.DataAccess;
using SPlus.Model.Domain;
using System.Data.Entity;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using SPlus.DTO;
using System.IdentityModel.Tokens.Jwt;

namespace SPlus.BLL
{
    public class LoginBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();
        private static DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        static readonly bool isLocal = Convert.ToBoolean(ConfigurationManager.AppSettings["isLocal"]);
        private readonly IUnitOfWorkFactory _factory;

        public LoginBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }

        public bool Login(string username, string password)
        {

            bool valid = false;
            if (!isLocal)
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
                {
                    valid = context.ValidateCredentials(username, password);
                }

                if (valid)
                {
                    using (var dataAccess = _factory.Create())
                    {
                        User user = dataAccess.User.Query().Where(a => a.UserName.ToLower() == username.ToLower() && !a.Deleted).FirstOrDefault();
                        if (user != null)
                            valid = true;
                        else
                            valid = false;
                    }
                }
            }
            else
            {
                valid = true;
            }
            return valid;
        }

        public bool ValidateSession(string username, Guid sessionid)
        {

            using (var dataAccess = _factory.Create())
            {
                Session session = dataAccess.Session.Query().Where(a => a.UserName.ToLower() == username.ToLower() && a.SessionID == sessionid).ToList().LastOrDefault();
                if (session != null)
                {
                    TimeSpan ts = DateTime.Now - session.SessionTime;
                    double minutesDifference = ts.TotalMinutes;
                    if (minutesDifference > Helper.Constants.sessionTimeOutMinutes)
                        return false;
                    else
                        return true;

                }
                else
                    return false;
            }
        }

        public void CreateSession(string username, Guid sessionid)
        {
            Session newSession = new Session();
            using (var dataAccess = _factory.Create())
            {
                var Sessions = dataAccess.Session.Query().Where(a => a.UserName.ToLower() == username.ToLower()).ToList();
                if (Sessions != null && Sessions.Count > 0)
                {
                    foreach (var session in Sessions)
                        dataAccess.Session.Delete(session);

                }
                newSession.UserName = username;
                newSession.SessionID = sessionid;
                newSession.SessionTime = DateTime.Now;
                dataAccess.Session.Save(newSession);
                dataAccess.Complete();
                dataAccess.Dispose();
            }
        }

        public void UpdateSession(string username, Guid sessionid)
        {
            using (var dataAccess = _factory.Create())
            {
                var session = dataAccess.Session.Query().Where(a => a.UserName.ToLower() == username.ToLower() && a.SessionID == sessionid).ToList().LastOrDefault();
                if (session != null)
                {
                    session.SessionTime = DateTime.Now;
                    dataAccess.Session.Save(session);
                    dataAccess.Complete();
                    dataAccess.Dispose();
                }

            }
        }

        public bool DeleteSession(string username)
        {
            bool successed = false;
            using (var dataAccess = _factory.Create())
            {
                var sessions = dataAccess.Session.Query().Where(a => a.UserName.ToLower() == username.ToLower()).ToList();
                if (sessions != null && sessions.Count > 0)
                {
                    foreach (var session in sessions)
                        dataAccess.Session.Delete(session);
                }
                dataAccess.Complete();
                dataAccess.Dispose();
                successed = true;
                return successed;
            }
        }


        public void AddTwoFactorAuth(string username, int codeExpirationMinutes,int smsCode, UserDTO CurrentUser,string token)
        {
            using (var dataAccess = _factory.Create())
            {
                var twoFactorAuth = dataAccess.TwoFactorAuth.Query().Where(a => a.UserName.ToLower() == username.ToLower()).OrderByDescending(l => l.Created).FirstOrDefault();
                if (twoFactorAuth != null )
                {
                    //exists
                    twoFactorAuth.Created = DateTime.Now;

                    twoFactorAuth.Expired = DateTime.Now.AddMinutes(codeExpirationMinutes);
                    twoFactorAuth.SMSAuthCode = smsCode;
                    twoFactorAuth.MobileNumber = CurrentUser.PhoneNumber;
                    twoFactorAuth.UserName = username;
                    twoFactorAuth.Counter = 1;
                    twoFactorAuth.Token = token;
                    dataAccess.TwoFactorAuth.Save(twoFactorAuth);
                }
                else
                {
                    TwoFactorAuth newtwoFactorAuth = new TwoFactorAuth();
                    newtwoFactorAuth.Created = DateTime.Now;

                    newtwoFactorAuth.Expired = DateTime.Now.AddMinutes(codeExpirationMinutes);
                    newtwoFactorAuth.SMSAuthCode = smsCode;
                    newtwoFactorAuth.MobileNumber = CurrentUser.PhoneNumber;
                    newtwoFactorAuth.UserName = username;
                    newtwoFactorAuth.Counter = 1;
                    newtwoFactorAuth.Token = token;
                    dataAccess.TwoFactorAuth.Save(newtwoFactorAuth);
                }
                dataAccess.Complete();
                dataAccess.Dispose();
                
              
            }
        }

        public TwoFactorAuth GetTwoFactorAuth(string username)
        {
            using (var dataAccess = _factory.Create())
            {
                var twoFactorAuth = dataAccess.TwoFactorAuth.Query().Where(a => a.UserName.ToLower() == username.ToLower()).OrderByDescending(l => l.Created).FirstOrDefault();
                if (twoFactorAuth != null)
                {
                    return new TwoFactorAuth()
                    {
                        UserName = twoFactorAuth.UserName,
                        Counter = twoFactorAuth.Counter,
                        Created = twoFactorAuth.Created,
                        Expired = twoFactorAuth.Expired,
                        MobileNumber = twoFactorAuth.MobileNumber,
                        SMSAuthCode = twoFactorAuth.SMSAuthCode,
                        Token=twoFactorAuth.Token
                    };
                }
                else
                {
                    return null;
                }
             


            }
        }


        public void ClearSuccessLogins(string username)
        {
            using (var dataAccess = _factory.Create())
            {
                var list = dataAccess.TwoFactorAuth.Query().Where(l => l.UserName == username).ToList();
                if (list != null && list.Count > 0)
                {
                    foreach (var logged in list)
                        dataAccess.TwoFactorAuth.Delete(logged);
                }
                dataAccess.Complete();
                dataAccess.Dispose();
            }
        }
        public void IncrementLoginCounter(string username)
        {
            using (var dataAccess = _factory.Create())
            {
                var login = dataAccess.TwoFactorAuth.Query().Where(l => l.UserName == username).OrderByDescending(l => l.Created).FirstOrDefault();
                if (login != null)
                {
                    login.Counter++;
                }
                dataAccess.Complete();
                dataAccess.Dispose();
            }
        }

        public bool LoginSSO(string ssoToken, out string _username)
        {
            _username = "";
            bool valid = false;
            if (!isLocal)
            {
                try
                {
                    //hdahamsheh: check config to check SSO service, and expiry date
                    var handler = new JwtSecurityTokenHandler();
                    var finalTokenResult = handler.ReadToken(ssoToken) as JwtSecurityToken;
                    _username = finalTokenResult.Claims.First(claim => claim.Type == "preferred_username").Value;
                    long expiryDateTicks = long.Parse(finalTokenResult.Claims.First(claim => claim.Type == "exp").Value);

                    DateTime tokenExpiryDate = start.AddSeconds(expiryDateTicks).ToLocalTime();

                    bool checkSSOToken = Convert.ToBoolean(ConfigurationManager.AppSettings["checkSSOToken"]);
                    bool checkSSOTokenExpiry = Convert.ToBoolean(ConfigurationManager.AppSettings["checkSSOTokenExpiry"]);

                    if (checkSSOToken)
                    {
                        //validate token
                    }
                    //if (checkSSOTokenExpiry && DateTime.Now > tokenExpiryDate)
                    //{
                    //    return false;
                    //}

                    string username = _username.Substring(0, _username.IndexOf('@'));
                    _username = username;
                    valid = true;

                    if (valid)
                    {
                        using (var dataAccess = _factory.Create())
                        {
                            User user = dataAccess.User.Query().Where(a => a.UserName.ToLower() == username.ToLower() && !a.Deleted).FirstOrDefault();
                            if (user != null)
                                valid = true;
                            else
                                valid = false;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception(ex.ToString());
                }
            }
            else
            {
                valid = true;
            }
            return valid;
        }
    }
}

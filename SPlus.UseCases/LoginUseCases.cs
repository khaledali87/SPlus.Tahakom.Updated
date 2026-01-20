using SPlus.BLL;
using SPlus.DTO;
using SPlus.Helper;
using SPlus.Model.Domain;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SPlus.UseCases
{
    public class LoginUseCases : LoggingUseCases
    {

        Container _Container = IOC.InitializeContainer();
        private readonly LoginBLL LoginBLL;
        private readonly UserActivityBLL UserActivityBLL;
        private readonly UserBLL UserBLL;
        public LoginUseCases()
        {
            LoginBLL = _Container.GetInstance<LoginBLL>();
            UserActivityBLL = _Container.GetInstance<UserActivityBLL>();
            UserBLL = _Container.GetInstance<UserBLL>();
        }

        public bool Login(string username, string password, Guid sessionID)
        {
            bool IsLogged = LoginBLL.Login(username, password);
            IsLogged = UserActivityBLL.CheckUserSession(username, sessionID);

            return IsLogged;
        }
        public LoginDTO LoginUser(UserDTO user)
        {
            LoginDTO User = new LoginDTO();
            UserBLL _UserBLL = new UserBLL();
            bool IsValid = true;
            bool IsAuthorized = false;
            string password = Encryption.DecryptPassword(user.Password);


            IsValid = LoginBLL.Login(user.UserName, password);

            if (IsValid)
            {
                string AdminGroup = Constants._AdminGroup;                
                List<UserDTO> Users = AutoMapper.Mapper.Map<List<UserDTO>>(_UserBLL.Read());
                List<ResourceDTO> resources = AutoMapper.Mapper.Map<List<ResourceDTO>>(_UserBLL.ReadResources());

                if (Users.Where(a => a.UserName.ToLower() == user.UserName.ToLower()).Count() > 0)
                {
                    IsAuthorized = true;
                }

                if (IsAuthorized)
                {
                    Guid UserSessionID = Guid.NewGuid();
                    User.Token = Encryption.GenerateSecurityToken(user.UserName, password, UserSessionID);

                    string USERNAME = user.UserName.Substring(user.UserName.LastIndexOf("\\") + 1);

                    UserActivityBLL.CreateUserSession(USERNAME, UserSessionID);
                    UserDTO CurrentUser = AutoMapper.Mapper.Map<UserDTO>(_UserBLL.ReadByUserName(USERNAME));
                    List<MatrixDTO> matrices = CurrentUser.Groups.SelectMany(s => s.Matrices).ToList();
                    User.UserData = CurrentUser;
                    List<string> Screens = new List<string>();
                    if (CurrentUser.Groups.Select(s => s.Title.ToLower()).Any(a => a == AdminGroup.ToLower()))
                    {
                        Screens = resources.Where(w => w.IsScreen).Select(s => s.Code).ToList();
                        User.UserData.isAdmin = true;

                    }
                    foreach (var matrix in matrices)
                    {
                        if (matrix.Resource.IsScreen)
                        {
                            if (!Screens.Contains(matrix.Resource.Code))
                            {
                                Screens.Add(matrix.Resource.Code);
                            }
                        }
                    }

                    User.Screens = Screens;
                }
                else
                {
                    User.Token = "NOT Authorized";
                    User.UserData = null;
                }
            }
            else
            { User.Token = "NOT Authenticated"; User.UserData = null; }
            return User;
        }
        public bool LoginSSO(string ssoToken, Guid sessionID)
        {
            bool IsLogged = LoginBLL.LoginSSO(ssoToken, out string username);
            IsLogged = UserActivityBLL.CheckUserSession(username, sessionID);

            return IsLogged;
        }
        public LoginDTO LoginUserSSO(string tokenSSO)
        {
            LoginDTO User = new LoginDTO();
            UserBLL _UserBLL = new UserBLL();
            bool IsValid = true;
            bool IsAuthorized = false;
            IsValid = LoginBLL.LoginSSO(tokenSSO, out string username);

            if (IsValid)
            {
                string AdminGroup = ConfigurationManager.AppSettings["AdminGroup"].ToString();
                List<UserDTO> Users = AutoMapper.Mapper.Map<List<UserDTO>>(_UserBLL.Read());
                List<ResourceDTO> resources = AutoMapper.Mapper.Map<List<ResourceDTO>>(_UserBLL.ReadResources());

                if (Users.Where(a => a.UserName.ToLower() == username.ToLower()).Count() > 0)
                {
                    IsAuthorized = true;
                }

                if (IsAuthorized)
                {
                    Guid UserSessionID = new Guid();
                    User.Token = Encryption.GenerateSecurityTokenSSO(username, UserSessionID);
                    User.SSOToken = tokenSSO;



                    string USERNAME = username.Substring(username.LastIndexOf("\\") + 1);

                    UserActivityBLL.CreateUserSession(USERNAME, UserSessionID);
                    UserDTO CurrentUser = AutoMapper.Mapper.Map<UserDTO>(_UserBLL.ReadByUserName(USERNAME));
                    List<MatrixDTO> matrices = CurrentUser.Groups.SelectMany(s => s.Matrices).ToList();
                    User.UserData = CurrentUser;
                    List<string> Screens = new List<string>();
                    if (CurrentUser.Groups.Select(s => s.Title.ToLower()).Any(a => a == AdminGroup.ToLower()))
                    {
                        Screens = resources.Where(w => w.IsScreen).Select(s => s.Code).ToList();
                        User.UserData.isAdmin = true;

                    }

                    //List<string> Forms = new List<string>();
                    //if (CurrentUser.Groups.Select(s => s.Title.ToLower()).Any(a => a == AdminGroup.ToLower()))
                    //{
                    //    Forms = resources.Where(w => w.IsForm).Select(s => s.Code).ToList();
                    //    User.UserData.isAdmin = true;

                    //}


                    foreach (var matrix in matrices)
                    {
                        if (matrix.Resource.IsScreen)
                        {
                            if (!Screens.Contains(matrix.Resource.Code))
                            {
                                Screens.Add(matrix.Resource.Code);
                            }
                        }
                        //if (matrix.Resource.IsForm)
                        //{
                        //    if (!Forms.Contains(matrix.Resource.Code))
                        //    {
                        //        Forms.Add(matrix.Resource.Code);
                        //    }
                        //}
                    }

                    User.Screens = Screens;
                   // User.Forms = Forms;

                }
                else
                {
                    User.Token = "NOT Authorized";
                    User.UserData = new UserDTO() { UserName = username };
                }
            }
            else
            { User.Token = "NOT Authenticated"; User.UserData = null; }
            return User;
        }
        public void UpdateSession(UserActivity userActivity)
        {
            UserActivityBLL.UpdateSession(userActivity);
        }
        public bool ValidateSession(string username, Guid sessionid, out UserActivity userActivity)
        {
            return UserActivityBLL.ValidateSession(username, sessionid, out userActivity);
        }
        public bool DeleteSession(string username)
        {
            return UserActivityBLL.DeleteSession(username);
        }
        public LoginDTO ValidateLogin(UserDTO user)
        {
            LoginDTO User = new LoginDTO();

            TwoFactorAuth login = LoginBLL.GetTwoFactorAuth(user.UserName);
            bool logged = true;
            if (login != null)
            {
                if (login.Expired < DateTime.Now)
                {
                    LoginBLL.ClearSuccessLogins(user.UserName);
                    User.Token = "Code Is Expired"; User.UserData = null;
                    logged = false;
                    return User;
                }

                int.TryParse(Constants.constTwoFactorAuthAttemptsCount, out int allowedAttempts);
                if (login.Counter > allowedAttempts)
                {
                    LoginBLL.ClearSuccessLogins(user.UserName);
                    User.Token = "User Is Blocked"; User.UserData = null;
                    logged = false;
                    return User;
                }

                if (user.SMSAuthCode != login.SMSAuthCode)
                {
                    LoginBLL.IncrementLoginCounter(user.UserName);
                    User.Token = "Invalid Code"; User.UserData = null;
                    logged = false;

                }
                if (logged)
                {
                    string AdminGroup = Constants._AdminGroup;
                    List<UserDTO> Users = AutoMapper.Mapper.Map<List<UserDTO>>(UserBLL.Read());
                    List<ResourceDTO> resources = AutoMapper.Mapper.Map<List<ResourceDTO>>(UserBLL.ReadResources());

                    User.Token = login.Token;
                    User.IsTwoFactorEnabled = true;
                    string USERNAME = user.UserName.Substring(user.UserName.LastIndexOf("\\") + 1);

                    UserDTO CurrentUser = AutoMapper.Mapper.Map<UserDTO>(UserBLL.ReadByUserName(USERNAME));
                    List<MatrixDTO> matrices = CurrentUser.Groups.SelectMany(s => s.Matrices).ToList();
                    User.UserData = CurrentUser;
                    List<string> Screens = new List<string>();
                    if (CurrentUser.Groups.Select(s => s.Title.ToLower()).Any(a => a == AdminGroup.ToLower()))
                    {
                        Screens = resources.Where(w => w.IsScreen).Select(s => s.Code).ToList();
                        User.UserData.isAdmin = true;

                    }


                    foreach (var matrix in matrices)
                    {
                        if (matrix.Resource.IsScreen)
                        {
                            if (!Screens.Contains(matrix.Resource.Code))
                            {
                                Screens.Add(matrix.Resource.Code);
                            }
                        }
                    }

                    User.Screens = Screens;


                    LoginBLL.ClearSuccessLogins(user.UserName);
                }

            }
            else
            { User.Token = "NOT Authenticated"; User.UserData = null; }
            return User;
        }
        public int GenerateSMSAuthCode()
        {
            Random rnd = new Random();
            int rndNumber = rnd.Next(1000, 9999);
            return rndNumber;
        }
        public void SendSMS(string PhoneNumber, int code)
        {

            string messageBody = code.ToString();
            Utilities.SendSMS(PhoneNumber, messageBody);

        }



    }
}

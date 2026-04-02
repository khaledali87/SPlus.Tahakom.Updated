using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using SPlus.DataAccess;
using SPlus.Model.Domain;
using Z.EntityFramework.Plus;


namespace SPlus.BLL
{
    public class UserActivityBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();
        private readonly IUnitOfWorkFactory _factory;
        public UserActivityBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }

        #region Public
        public UserActivity CreateUserSession(string userName, Guid sessionid)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                var current = ReadByUserName(userName);
                UserActivity userActivity = new UserActivity();
                if (current is null)
                {
                    userActivity.Created = DateTime.Now;
                    userActivity.Modified = DateTime.Now;
                    userActivity.LastActivity = DateTime.Now;
                    userActivity.UserName = userName.ToLower();
                    userActivity.SessionID = sessionid;
                }
                else
                {
                    userActivity.Created = DateTime.Now;
                    userActivity.Modified = DateTime.Now;
                    userActivity.LastActivity = DateTime.Now;
                    userActivity.UserName = userName.ToLower();
                    userActivity.Id = current.Id;
                    userActivity.SessionID = sessionid;
                }
                dataAccess.UserActivity.Save(userActivity);
                result = dataAccess.Complete();

                dataAccess.Dispose();

                var res = ReadByUserName(userActivity.UserName, sessionid);
                return res;
            }
        }

        //public UserActivity CreateUserSession(string userName, Guid sessionid)
        //{
        //    int result;
        //    using (var dataAccess = _factory.Create())
        //    {
        //        //var current = ReadByUserName(userName);
        //        UserActivity userActivity = new UserActivity();
        //        //if (current is null)
        //        //{
        //            userActivity.Created = DateTime.Now;
        //            userActivity.Modified = DateTime.Now;
        //            userActivity.LastActivity = DateTime.Now;
        //            userActivity.UserName = userName.ToLower();
        //            userActivity.SessionID = sessionid;
        //        //}
        //        //else
        //        //{
        //        //    userActivity.Created = DateTime.Now;
        //        //    userActivity.Modified = DateTime.Now;
        //        //    userActivity.LastActivity = DateTime.Now;
        //        //    userActivity.UserName = userName.ToLower();
        //        //    userActivity.Id = current.Id;
        //        //    userActivity.SessionID = sessionid;
        //        //}
        //        dataAccess.UserActivity.Save(userActivity);
        //        result = dataAccess.Complete();

        //        dataAccess.Dispose();

        //        var res = ReadByUserName(userActivity.UserName, sessionid);
        //        return res;
        //    }
        //}


        public bool CheckUserSession(string userName, Guid sessionID)
        {
            UserActivity userActivity = ReadByUserName(userName, sessionID);
            int SessionTime = GetUserSessionTime();
            if(userActivity != null)
            {
                if (DateTime.Now > userActivity.LastActivity.AddMinutes(SessionTime))
                {
                    DeleteSession(userName);
                    return false;
                }
                else
                {
                    UpdateSession(userActivity);
                    return true;
                }
            }
            return false;
            
        }
        public bool ValidateSession(string username, Guid SessionID, out UserActivity userActivity)
        {

            using (var dataAccess = _factory.Create())
            {
                UserActivity session = dataAccess.UserActivity.Query().Where(a => a.UserName.ToLower() == username.ToLower() && a.SessionID == SessionID).ToList().LastOrDefault();
                if (session != null)
                {
                    userActivity = session;
                    TimeSpan ts = DateTime.Now - session.LastActivity;
                    double minutesDifference = ts.TotalMinutes;
                    if (minutesDifference > GetUserSessionTime())
                    {
                        DeleteSession(username);
                        return false;
                    }
                    else
                        return true;
                }
                else
                {
                    userActivity = null;
                    return false;
                }
            }
        }
        public bool DeleteSession(string userName)
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                var current = dataAccess.UserActivity.Query(w => w.UserName == userName);

                if (current.Count() == 1)
                {
                    UserActivity userActivity = current.FirstOrDefault();
                    if (userActivity != null)
                    {
                        dataAccess.UserActivity.Delete(userActivity);
                        result = dataAccess.Complete();
                        if (result > 0)
                            return true;
                        return false;
                    }
                }
                else
                {
                    List<UserActivity> userActivities = current.ToList();
                    foreach (var userActivity in userActivities)
                    {
                        if (userActivity != null)
                        {
                            dataAccess.UserActivity.Delete(userActivity);
                        }
                    }
                    result = dataAccess.Complete();
                    if (result > 0)
                        return true;
                    return false;
                }

                return false;
            }
        }
        #endregion

        private UserActivity ReadByUserName(string userName, Guid sessionID)
        {
            using (var dataAccess = _factory.Create())
            {
                UserActivity userActivity = dataAccess.UserActivity.Query(w => w.UserName == userName && w.SessionID == sessionID).FirstOrDefault();
                return userActivity;
            }
        }
        private UserActivity ReadByUserName(string userName)
        {
            using (var dataAccess = _factory.Create())
            {
                UserActivity userActivity = dataAccess.UserActivity.Query(w => w.UserName == userName).FirstOrDefault();
                return userActivity;
            }
        }
        private int GetUserSessionTime()
        {
            int SessionTimer;
            using (var dataAccess = _factory.Create())
            {
                Model.Domain.Configuration configuration = dataAccess.Configuration.Query(w => w.Key == "UserSessionTimeByMinutes").FirstOrDefault();
                if (configuration is null)
                {
                    SessionTimer = 5;
                }
                else
                {
                    SessionTimer = int.Parse(configuration.Value) == 0 ? 15 : int.Parse(configuration.Value);
                }
            }
            return SessionTimer;
        }
        public UserActivity UpdateSession(UserActivity userActivity)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                UserActivity current = dataAccess.UserActivity.Query(w => w.UserName == userActivity.UserName && w.SessionID == userActivity.SessionID).FirstOrDefault();

                current.LastActivity = DateTime.Now;
                current.Modified = DateTime.Now;
                dataAccess.UserActivity.Save(current);
                result = dataAccess.Complete();

                dataAccess.Dispose();
                return current;
            }
        }
    }
}

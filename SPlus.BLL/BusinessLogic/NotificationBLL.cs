using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.DTO;
using StructureMap;

using SPlus.DataAccess;
using SPlus.Model.Domain;

namespace SPlus.BLL
{
    public class NotificationBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();

        private readonly IUnitOfWorkFactory _factory;
        public NotificationBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }
        
        #region Read
        public List<Notification> GetAllNotifications(string user)
        {
            using (var dataAccess = _factory.Create())
            {
                //List<Notification> notification = dataAccess.Notification.GetAll().Where(w => w.AssignedTo?.ToLower() == user?.ToLower()).OrderByDescending(o => o.ID).Take(250).ToList();

                var normalizedUser = user?.ToLower();

                List<Notification> notifications = dataAccess.Notification.Query()
                    .Where(w =>w.AssignedTo!=null&&w.AssignedTo.ToLower()== normalizedUser)
                    .OrderByDescending(o => o.ID)
                    .Take(250)
                    .ToList();


                return notifications;



            }
        }

        public Notification ReadByID(int ID)
        {

            using (var dataAccess = _factory.Create())
            {
                return dataAccess.Notification.Get(ID);
            }
        }
        public bool IsAvailable(string user)
        {
            return GetAllNotifications(user).Where(w => w.AssignedTo?.ToLower() == user && w.Status == (int)Model.NotificationEnums.Pending).Count() > 0;
        }
        #endregion

        #region Update
        public Notification UpdateNotification(Notification notification)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                var context = dataAccess.Notification.Query().Where(w => w.RelatedItemID == notification.RelatedItemID && w.Status == (int)Model.NotificationEnums.Pending).FirstOrDefault();
                if (context != null)
                {
                    context.Modified = DateTime.Now;
                    context.Status = (int)Model.NotificationEnums.Closed;
                    dataAccess.Notification.Save(context);
                    dataAccess.Complete();

                }
                return notification;
            }
        }

        public bool UpdateNotification(int id)
        {
            using (var dataAccess = _factory.Create())
            {
                Notification notification = dataAccess.Notification.Get(id);
                if (notification != null) 
                {
                    notification.Modified = DateTime.Now;
                    notification.Status = (int)Model.NotificationEnums.Closed;
                    dataAccess.Notification.Save(notification);
                    dataAccess.Complete();
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Delete

        #endregion
    }
}

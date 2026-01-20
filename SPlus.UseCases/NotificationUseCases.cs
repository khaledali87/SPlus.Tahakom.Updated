using SPlus.BLL;
using SPlus.DTO;
using SPlus.Model.Domain;
using StructureMap;
using System.Collections.Generic;
using System.Linq;
namespace SPlus.UseCases
{
    public class NotificationUseCases : LoggingUseCases
    {

        Container _Container = IOC.InitializeContainer();
        private readonly NotificationBLL NotificationBLL;
        public NotificationUseCases()
        {
            NotificationBLL = _Container.GetInstance<NotificationBLL>();
        }

        #region Read
        public List<NotificationDTO> GetAllNotifications(string user)
        {
            return AutoMapper.Mapper.Map<List<NotificationDTO>>(NotificationBLL.GetAllNotifications(user));
        }

        public Notification ReadByID(int ID)
        {
            return NotificationBLL.ReadByID(ID);
        }
        public bool IsAvailable(string user)
        {
            return GetAllNotifications(user).Where(w => w.AssignedTo?.ToLower() == user && w.Status == Model.NotificationEnums.Pending.ToString()).Count() > 0;
        }
        #endregion

        #region Update
        public Notification UpdateNotification(Notification notification)
        {
            return NotificationBLL.UpdateNotification(notification);
        }

        public bool UpdateNotification(int id)
        {
            return NotificationBLL.UpdateNotification(id);
        }
        #endregion

        #region Delete

        #endregion
    }
}

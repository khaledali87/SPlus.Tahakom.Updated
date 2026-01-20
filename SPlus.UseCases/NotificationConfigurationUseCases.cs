using SPlus.BLL;
using SPlus.DTO;
using SPlus.Model.Domain;
using StructureMap;
using System.Collections.Generic;

namespace SPlus.UseCases
{
    public class NotificationConfigurationUseCases : LoggingUseCases
    {

        Container _Container = IOC.InitializeContainer();
        private readonly NotificationConfigurationBLL NotificationConfigurationBLL;
        public NotificationConfigurationUseCases()
        {
            NotificationConfigurationBLL = _Container.GetInstance<NotificationConfigurationBLL>();
        }


        #region Read
        public List<NotificationConfigutrationListDTO> Read()
        {
            var res = NotificationConfigurationBLL.Read();
            return AutoMapper.Mapper.Map<List<NotificationConfigutrationListDTO>>(res);
        }

        public NotificationConfigurationDTO ReadByID(int ID)
        {
            var res = NotificationConfigurationBLL.ReadByID(ID);
            return AutoMapper.Mapper.Map<NotificationConfigurationDTO>(res);

        }

        #endregion

        #region Update
        public NotificationConfigurationDTO Update(NotificationConfigurationDTO notificationConfiguration)
        {
            var notificationConfigurationmapped = AutoMapper.Mapper.Map<NotificationConfiguration>(notificationConfiguration);

            var res = NotificationConfigurationBLL.Update(notificationConfigurationmapped);
            return AutoMapper.Mapper.Map<NotificationConfigurationDTO>(res);
        }

        #endregion

    }
}

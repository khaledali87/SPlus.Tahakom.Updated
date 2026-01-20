using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.DTO;
using StructureMap;
using SPlus.DataAccess;
using SPlus.Model.Domain;
using AutoMapper;
using System.Data.Entity;
using Z.EntityFramework.Plus;

namespace SPlus.BLL
{
    public class ReminderRegistryBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();


        private readonly IUnitOfWorkFactory _factory;
        public ReminderRegistryBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }

        #region Create

        public WFReminderRegistry Create(WFReminderRegistry reminderRegistry)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                dataAccess.WFReminderRegistry.Save(reminderRegistry);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return reminderRegistry;
            }
        }
        #endregion

        #region Read
        public List<WFReminderRegistry> Read()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<WFReminderRegistry> WFReminderRegistries = dataAccess.WFReminderRegistry.Query();
                return WFReminderRegistries.ToList();
            }
        }
        #endregion


    }
}

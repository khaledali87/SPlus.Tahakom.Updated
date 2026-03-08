using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;

using SPlus.DataAccess;
using SPlus.Model.Domain;
using System.Runtime.Remoting.Contexts;

namespace SPlus.BLL
{
    public class HolidayBLL
    {
        Container _Container = IOC.InitializeContainer();


        private readonly IUnitOfWorkFactory _factory;
        public HolidayBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }



        #region Read
        public List<Holiday> Read()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<Holiday> holidays   = dataAccess.Holiday.Query();

                return holidays.ToList();
            }
        }

        public List<DateTime> HolidayDays()
        {
            using (var dataAccess = _factory.Create())
            {
                var holidayDays = dataAccess.Holiday.Query(h => h.IsActive == true)
                   .AsEnumerable()
                   .SelectMany(h => Enumerable.Range(0, (h.EndDate - h.StartDate).Days + 1)
                       .Select(d => h.StartDate.AddDays(d).Date))
                   .Distinct()
                   .ToList();

                return holidayDays.ToList();
            }
        }

       


        public Holiday ReadById(int id)
        {
            using (var dataAccess = _factory.Create())
            {
                Holiday holiday = dataAccess.Holiday.Query()
                    .Where(a => a.ID == id)
                    .FirstOrDefault();

                return holiday;
            }
        }

        public Holiday Create(Holiday holiday)
        {
            using (var dataAccess = _factory.Create())
            {
                holiday.Created = DateTime.Now; 
                holiday.Modified = DateTime.Now;    
                dataAccess.Holiday.Save(holiday);
                dataAccess.Complete();  
                return holiday;
            }
        }

        public Holiday Update(Holiday holiday)
        {
            using (var dataAccess = _factory.Create())
            {
                holiday.Modified = DateTime.Now;

                dataAccess.Holiday.Save(holiday);
                dataAccess.Complete();
                
                return holiday;
            }
        }

        public bool UpdateStatus(int id , bool active)
        {
            using (var dataAccess = _factory.Create())
            {
                var holiday = dataAccess.Holiday.Query().Where(a => a.ID == id).FirstOrDefault();
                
                holiday.IsActive = active;

                dataAccess.Holiday.Save(holiday);
                dataAccess.Complete();

                return dataAccess.Complete() > 0; 
            }
        }

        public bool Delete(int id)
        {
            using (var dataAccess = _factory.Create())
            {
                Holiday holiday = dataAccess.Holiday.Query()
                   .Where(a => a.ID == id)
                   .FirstOrDefault();

                dataAccess.Holiday.Delete(holiday);
                return dataAccess.Complete() > 0;
            }
        }

        #endregion


    }

}

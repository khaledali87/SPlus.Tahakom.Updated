using SPlus.BLL;
using SPlus.DTO;
using SPlus.Model.Domain;
using StructureMap;
using StructureMap.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPlus.UseCases
{
    public class KPITypeUseCases : LoggingUseCases
    {
        Container _Container = IOC.InitializeContainer();
        private readonly KPITypeBLL KPITypeBLL;
        public KPITypeUseCases()
        {
            KPITypeBLL = _Container.GetInstance<KPITypeBLL>();
        }

        #region Create
        public KPITypeDTO Create(KPITypeDTO kpiType)
        {
            if (string.IsNullOrWhiteSpace(kpiType.ArabicName))
                kpiType.ArabicName = kpiType.EnglishName;

            var res = AutoMapper.Mapper.Map<KPIType>(kpiType);
            var result = KPITypeBLL.Create(res);
            var final = KPITypeBLL.ReadByID(result.KPITypeID);
            return AutoMapper.Mapper.Map<KPITypeDTO>(final);

        }

        #endregion

        #region Read
        public List<ListingStatusDTO> GetStatuses()
        {
            return AutoMapper.Mapper.Map<List<ListingStatusDTO>>(KPITypeBLL.ReadStatuses());
        }
        public List<KPITypeDTO> Read()
        {
            return AutoMapper.Mapper.Map<List<KPITypeDTO>>(KPITypeBLL.Read());

        }
        public List<SingularKPITypeDTO> ReadBasic()
        {
            return AutoMapper.Mapper.Map<List<SingularKPITypeDTO>>(KPITypeBLL.Read());
        }
        public KPITypeDTO ReadByID(int id)
        {
            return AutoMapper.Mapper.Map<KPITypeDTO>(KPITypeBLL.ReadByID(id));
        }

        #endregion

        #region Update

        public KPITypeDTO Update(KPITypeDTO kpiType)
        {
            var res = AutoMapper.Mapper.Map<KPIType>(kpiType);
            var result = KPITypeBLL.Update(res);
            var final = KPITypeBLL.ReadByID(result.KPITypeID);
            return AutoMapper.Mapper.Map<KPITypeDTO>(final);
        }

        #endregion

        #region Delete
        public bool Delete(int id)
        {
            bool IsDeletable = KPITypeBLL.ReadByID(id).IsDeletable;
            if (!IsDeletable)
            {
                throw new System.Exception("The KPIType Has Childrens/Child");
            }
            else
            {
                return KPITypeBLL.Delete(id);
            }
        }

        #endregion
    }

    public class HolidayUseCases : LoggingUseCases
    {
        Container _Container = IOC.InitializeContainer();
        private readonly HolidayBLL HolidayBLL;
        public HolidayUseCases()
        {
            HolidayBLL = _Container.GetInstance<HolidayBLL>();
        }

        #region Create
        public Holiday Create(Holiday holiday)
        {
            DateTime newStart = holiday.StartDate.Date;
            DateTime newEnd = holiday.EndDate.Date.AddDays(1).AddTicks(-1); // end of day

            bool isOverlapping = HolidayBLL.Read().Any(x =>
                            newStart <= x.EndDate &&
                            newEnd >= x.StartDate
                        );

            if (isOverlapping)
            {
                throw new System.Exception($"The date range ({newStart:yyyy-MM-dd} to {newEnd:yyyy-MM-dd}) overlaps with an existing holidays.");
            }

            return HolidayBLL.Create(holiday);
        }

        #endregion

        #region Read
        public List<Holiday> Read()
        {
            return HolidayBLL.Read();

        }

        public List<DateTime> HolidayDays()
        {
            return HolidayBLL.HolidayDays();

        }

        public Holiday Read(int id)
        {
            return HolidayBLL.ReadById(id);
        }

        #endregion

        #region Update

        public Holiday Update(Holiday holiday)
        {
            DateTime newStart = holiday.StartDate.Date;
            DateTime newEnd = holiday.EndDate.Date.AddDays(1).AddTicks(-1); // end of day
            bool isOverlapping = HolidayBLL.Read().Where(x=> x.ID != holiday.ID).Any(x =>
                            newStart <= x.EndDate &&
                            newEnd >= x.StartDate
                        );

            if (isOverlapping)
            {
                throw new System.Exception($"The date range ({newStart:yyyy-MM-dd} to {newEnd:yyyy-MM-dd}) overlaps with an existing holidays.");
            }


            return HolidayBLL.Update(holiday); ;
        }

        public bool UpdateStatus(int id , bool active)
        {
            return HolidayBLL.UpdateStatus(id, active); ;
        }

        #endregion

        #region Delete
        public bool Delete(int id)
        {
           return HolidayBLL.Delete(id);
        }

        #endregion
    }
}

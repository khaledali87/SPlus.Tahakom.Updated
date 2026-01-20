using SPlus.BLL;
using SPlus.DTO;
using SPlus.Model.Domain;
using StructureMap;
using System.Collections.Generic;

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
}

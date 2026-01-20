using SPlus.BLL;
using SPlus.DTO;
using SPlus.Model.Domain;
using StructureMap;
using System.Collections.Generic;
using System.Linq;

namespace SPlus.UseCases
{
    public class PerspectiveUseCases : LoggingUseCases
    {

        Container _Container = IOC.InitializeContainer();
        private readonly PerspectiveBLL PerspectiveBLL;
        public PerspectiveUseCases()
        {
            PerspectiveBLL = _Container.GetInstance<PerspectiveBLL>();
        }


        #region Create

        public CAPerspectiveDTO Create(CAPerspectiveDTO perspective)
        {
            Perspective Perspective = AutoMapper.Mapper.Map<Perspective>(perspective);
            var allPerspective = PerspectiveBLL.Read();
            if (allPerspective.Any(a => a.Order == perspective.Order))
            {
                //throw new System.Exception("Order Must be Unique");
                CAPerspectiveDTO data = new CAPerspectiveDTO();
                data.ID = -1;
                data.EnglishName = "Order Must be Unique";
                return data;
            }
            else
            {
                var result = PerspectiveBLL.Create(Perspective);
                return AutoMapper.Mapper.Map<CAPerspectiveDTO>(PerspectiveBLL.ReadByID(result.ID));
            }
        }

        #endregion

        #region Read
        public List<CAPerspectiveDTO> Read()
        {
            var res = PerspectiveBLL.Read();
            List<CAPerspectiveDTO> Perspectives = AutoMapper.Mapper.Map<List<CAPerspectiveDTO>>(res);
            return Perspectives;
        }

        public CAPerspectiveDTO ReadByID(int ID)
        {
            var res = PerspectiveBLL.ReadByID(ID);
            CAPerspectiveDTO Perspective = AutoMapper.Mapper.Map<CAPerspectiveDTO>(res);
            return Perspective;
        }

        #endregion

        #region Update
        public CAPerspectiveDTO Update(CAPerspectiveDTO perspective)
        {
            Perspective Perspective = AutoMapper.Mapper.Map<Perspective>(perspective);
            var allPerspective = PerspectiveBLL.Read();
            if (allPerspective.Any(a => a.Order == perspective.Order && a.ID != perspective.ID))
            {
                CAPerspectiveDTO data = new CAPerspectiveDTO();
                data.ID = -1;
                data.EnglishName = "Order Must be Unique";
                return data;
            }
            else
            {
                var res = PerspectiveBLL.Update(Perspective);
                return AutoMapper.Mapper.Map<CAPerspectiveDTO>(res);
            }
        }


        #endregion

        #region Delete
        public bool Delete(int id)
        {
            return PerspectiveBLL.Delete(id);
        }

        #endregion
    }
}

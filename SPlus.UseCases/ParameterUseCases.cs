using SPlus.BLL;
using SPlus.DTO;
using SPlus.Model.Domain;
using StructureMap;
using System.Collections.Generic;
namespace SPlus.UseCases
{
    public class ParameterUseCases : LoggingUseCases
    {

        Container _Container = IOC.InitializeContainer();
        private readonly ParameterBLL ParameterBLL;
        public ParameterUseCases()
        {
            ParameterBLL = _Container.GetInstance<ParameterBLL>();
        }


        #region Create



        #endregion

        #region Read

        #endregion

        #region Update

        public bool UpdateParameterValues(List<WFFormUpdateKPIDTO> forms)
        {
            // var param = AutoMapper.Mapper.Map<List<Parameter>>(parameters);

            var updateforms = AutoMapper.Mapper.Map<List<WFFormUpdateKPI>>(forms);
            //return ParameterBLL.UpdateParameterValues(updateforms, param);
            return false;
        }
        #endregion

        #region Delete

        #endregion
    }
}

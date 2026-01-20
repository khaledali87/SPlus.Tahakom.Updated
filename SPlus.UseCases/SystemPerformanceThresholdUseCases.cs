using SPlus.BLL;
using SPlus.DTO;
using SPlus.Model.Domain;
using StructureMap;
using System.Collections.Generic;

namespace SPlus.UseCases
{
    public class SystemPerformanceThresholdUseCases : LoggingUseCases
    {
        Container _Container = IOC.InitializeContainer();
        private readonly SystemPerformanceThresholdBLL SystemPerformanceThresholdBLL;
        public SystemPerformanceThresholdUseCases()
        {
            SystemPerformanceThresholdBLL = _Container.GetInstance<SystemPerformanceThresholdBLL>();
        }
        #region Read
        public List<SystemPerformanceThresholdDTO> SystemPerformanceThreshold()
        {
            var res = SystemPerformanceThresholdBLL.SystemPerformanceThreshold();
            List<SystemPerformanceThresholdDTO> thresholds = AutoMapper.Mapper.Map<List<SystemPerformanceThresholdDTO>>(res);
            return thresholds;
        }
        public List<SystemPerformanceThresholdDTO> SystemPerformanceThresholdNA()
        {
            var res = SystemPerformanceThresholdBLL.SystemPerformanceThresholdNA();
            List<SystemPerformanceThresholdDTO> thresholds = AutoMapper.Mapper.Map<List<SystemPerformanceThresholdDTO>>(res);
            return thresholds;
        }
        #endregion

        #region Update
        public List<UpdateSystemPerformanceThresholdDTO> UpdateSystemPerformanceThreshold(List<UpdateSystemPerformanceThresholdDTO> systemPerformanceThresholds)
        {
            List<SystemPerformanceThreshold> KPIPerformanceThresholds = AutoMapper.Mapper.Map<List<SystemPerformanceThreshold>>(systemPerformanceThresholds);
            var res = SystemPerformanceThresholdBLL.UpdateSystemPerformanceThreshold(KPIPerformanceThresholds);
            return AutoMapper.Mapper.Map<List<UpdateSystemPerformanceThresholdDTO>>(res);

        }
        #endregion
    }
}

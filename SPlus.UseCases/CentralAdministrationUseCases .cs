using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using SPlus.DataAccess;
using SPlus.DataAccess.Model;
using System.Data.Entity;
using System.Reflection;
using SPlus.BLL.BusinessLogic;
using SPlus.DTO;
namespace SPlus.UseCases
{
    public class CentralAdministrationUseCases : LoggingUseCases
    {
        KPIBLL KPIBLL = new KPIBLL();
        StrategicObjectiveBLL StrategicObjectiveBLL = new StrategicObjectiveBLL();
        ThemeBLL ThemeBLL = new ThemeBLL();
        PerspectiveBLL PerspectiveBLL = new PerspectiveBLL();
        UserBLL UserBLL = new UserBLL();
        OrgStructureBLL OrgStructureBLL = new OrgStructureBLL();
        KPITypeBLL KPITypeBLL = new KPITypeBLL();
        NotificationConfigurationBLL NotificationConfigurationBLL = new NotificationConfigurationBLL();
        #region Create
        public int Create(SingularKPIDTO kpi)
        {
           KPI KPI = AutoMapper.Mapper.Map<DataAccess.Model.KPI>(kpi);
            return  KPIBLL.Create(KPI);
        }
        public int CreateComment(KPICommentDTO kpiComment)
        {
            DataAccess.Model.KPIComment KPIComment = AutoMapper.Mapper.Map<DataAccess.Model.KPIComment>(kpiComment);
           return KPIBLL.CreateComment(KPIComment);
        }

        #endregion

        #region Read

        public List<KPIListDTO> ReadCentralAdministrationKPI(string username)
        {
            var res = KPIBLL.Read(username); 
            List<KPIListDTO> KPIs = AutoMapper.Mapper.Map<List<KPIListDTO>>(res);
            return KPIs;

        }
        public SingularKPIDTO ReadByID(int id)
        {
            var res = KPIBLL.ReadByID(id);
            SingularKPIDTO KPI = AutoMapper.Mapper.Map<SingularKPIDTO>(res);
            return KPI;
        }

        public SingularKPIDTO ReadByID(int id,string username)
        {
            var res = KPIBLL.ReadByID(id, username);
            SingularKPIDTO KPI = AutoMapper.Mapper.Map<SingularKPIDTO>(res);
            return KPI;
        }
        public int GetKPIIDByRequestID(int RequestID)
        {
            return KPIBLL.GetKPIByRequestID(RequestID).ID;
         
        }
        public List<SystemPerformanceThresholdDTO> SystemPerformanceThreshold()
        {
            var res = KPIBLL.SystemPerformanceThreshold();
            //var x = AutoMapper.Mapper.Map<KPITypeDTO>(res[0]);
            List<SystemPerformanceThresholdDTO> thresholds = AutoMapper.Mapper.Map<List<SystemPerformanceThresholdDTO>>(res);
            return thresholds;
        }
        #endregion

        #region Update

        public UpdateKPIDTO Update(UpdateKPIDTO kpi)
        {
            DataAccess.Model.KPI KPI = AutoMapper.Mapper.Map<DataAccess.Model.KPI>(kpi);
            var res = KPIBLL.Update(KPI);
            return AutoMapper.Mapper.Map<UpdateKPIDTO>(res);
             
        }
        public List<KPIUpdateWeightDTO> Update(List<KPIUpdateWeightDTO> kpis)
        {
            List<DataAccess.Model.KPI> KPIs = AutoMapper.Mapper.Map<List<DataAccess.Model.KPI>>(kpis);
            var res = KPIBLL.Update(KPIs);
            return AutoMapper.Mapper.Map<List<KPIUpdateWeightDTO>>(res);
        }
        public KPICommentDTO UpdateComment(KPICommentDTO kpiComment)
        {
            DataAccess.Model.KPIComment _KPIComment = AutoMapper.Mapper.Map<DataAccess.Model.KPIComment>(kpiComment);
            var res = KPIBLL.UpdateComment(_KPIComment);
            return AutoMapper.Mapper.Map<KPICommentDTO>(res);
        }
        public List<UpdateSystemPerformanceThresholdDTO> UpdateSystemPerformanceThreshold(List<UpdateSystemPerformanceThresholdDTO> systemPerformanceThresholds)
        {
            List<DataAccess.Model.SystemPerformanceThreshold> KPIPerformanceThresholds = AutoMapper.Mapper.Map<List<DataAccess.Model.SystemPerformanceThreshold>>(systemPerformanceThresholds);
            var res = KPIBLL.UpdateSystemPerformanceThreshold(KPIPerformanceThresholds);
            return AutoMapper.Mapper.Map<List<UpdateSystemPerformanceThresholdDTO>>(res);
            
        }

        #endregion

        #region Delete

      
        public bool Delete(int id)
        {
            return KPIBLL.Delete(id);
        }
        public bool DeleteComment(int id)
        {
            return KPIBLL.DeleteComment(id);
        }

        #endregion

    }
}

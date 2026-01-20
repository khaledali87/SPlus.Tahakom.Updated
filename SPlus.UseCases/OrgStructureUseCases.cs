using AutoMapper;
using SPlus.BLL;
using SPlus.DTO;
using SPlus.Helper;
using SPlus.Model.Domain;
using StructureMap;
using System.Collections.Generic;
using System.Linq;

namespace SPlus.UseCases
{
    public class OrgStructureUseCases : LoggingUseCases
    {
        Container _Container = IOC.InitializeContainer();
        private readonly OrgStructureBLL OrgStructureBLL;
        private readonly SystemPerformanceThresholdBLL SystemPerformanceThresholdBLL;
        private readonly KPIBLL KPIBLL;
        private readonly HandshakeBLL HandshakeBLL;
        private readonly KPITypeBLL KPITypeBLL;
        private readonly StrategyBLL StrategyBLL;
        private readonly UserBLL UserBLL;

        public OrgStructureUseCases()
        {
            OrgStructureBLL = _Container.GetInstance<OrgStructureBLL>();
            SystemPerformanceThresholdBLL = _Container.GetInstance<SystemPerformanceThresholdBLL>();
            KPIBLL = _Container.GetInstance<KPIBLL>();
            HandshakeBLL = _Container.GetInstance<HandshakeBLL>();
            StrategyBLL = _Container.GetInstance<StrategyBLL>();
            KPITypeBLL = _Container.GetInstance<KPITypeBLL>();
            UserBLL = _Container.GetInstance<UserBLL>();

        }


        #region Create
        public OrgStructureDTO Create(OrgStructureDTO orgStructure, string UserName)
        {
            var res = AutoMapper.Mapper.Map<OrgStructure>(orgStructure);
            return AutoMapper.Mapper.Map<OrgStructureDTO>(OrgStructureBLL.Create(res, UserName));
        }
        #endregion

        #region Read

        public List<OrgStructureDTO> Read(string UserName, int? Year)
        {

            var res = OrgStructureBLL.Read(UserName,Year);
            if (Year.HasValue)
                res = res.Where(w => w.Years.Any(a => a == Year.Value.ToString())).ToList();

            List<OrgStructureDTO> orgStructureDTOs = AutoMapper.Mapper.Map<List<OrgStructureDTO>>(res);

            foreach (var item in orgStructureDTOs)
            {
                if (item.ID == Constants.CorporateDepartmentID || item.ID == Constants.CorporateSectortID)
                {
                    item.IsCorporate = true;
                }
            }
            return orgStructureDTOs;


        }
        public List<OrgStructureDTO> ReadReadByManger(string userName, int? Year)
        {
            var UserGroups = UserBLL.ReadGroupByUserName(userName);
            var res = OrgStructureBLL.Read(userName,Year).Where(a => a.Manager.ToLower() == userName.ToLower() || UserGroups.Any(group => group.Title.Equals(Constants._AdminGroup, System.StringComparison.OrdinalIgnoreCase)));

            if (Year.HasValue)
                res = res.Where(w => w.Years.Any(a => a == Year.Value.ToString())).ToList();

            List<OrgStructureDTO> orgStructureDTOs = AutoMapper.Mapper.Map<List<OrgStructureDTO>>(res);

            foreach (var item in orgStructureDTOs)
            {
                if (item.ID == Constants.CorporateDepartmentID || item.ID == Constants.CorporateSectortID)
                {
                    item.IsCorporate = true;
                }
            }
            return orgStructureDTOs;


        }
        public List<CADepartmentKPIsDTO> ReadCA(int id, string userName)
        {

            var res = OrgStructureBLL.ReadDivisionalObjectiveByOrgId(id, userName);

            var kpis = KPIBLL.Read(null).Where(a => a.DivisionalObjectiveID.HasValue && !a.StrategicObjectiveID.HasValue && a.DivisionalObjectiveID > 0);

            foreach (var division in res)
            {


                division.KPIs = kpis.Where(a => a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID == division.ID).ToList();
                foreach (var item in division.KPIs)
                {
                    // no need to delete from CA
                    item.IsDeletable = false;
                    item.IsEditable = false;

                    if (KPIBLL.IsInGracePeriod(item))
                        item.AllowLock = false;
                    else
                        item.AllowLock = true;

                }



            }


            return AutoMapper.Mapper.Map<List<CADepartmentKPIsDTO>>(res);
        }
        public OrgStructureDTO ReadByID(int ID, string UserName)
        {
            var res = OrgStructureBLL.ReadByID(ID, UserName);
            return AutoMapper.Mapper.Map<OrgStructureDTO>(res);
        }
        public List<OrgStructureMapDTO> OrgStructureMap(string UserName, int? Year)
        {
            List<OrgStructureMapDTO> orgStructureMapDTOList = new List<OrgStructureMapDTO>();
            List<OrgStructure> orgStructures = OrgStructureBLL.ReadOrgWithDivisional(UserName,Year);

            if (Year.HasValue)
                orgStructures = orgStructures.Where(w => w.Years.Any(a => a == Year.Value.ToString())).ToList();

            if (orgStructures != null && orgStructures.Count > 0)
            {
                var systemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold();
                List<KPI> kpis = KPIBLL.Read(UserName,Year).Where(a => a.DivisionalObjectiveID.HasValue && !a.StrategicObjectiveID.HasValue).ToList();
                List<KPI> Calculationkpis = KPIBLL.ReadForCalculation(Year);
                List<Status> Statuses = KPITypeBLL.ReadStatuses();


                foreach (var org in orgStructures)
                {
                    OrgStructureMapDTO orgStructureMapDTO = new OrgStructureMapDTO();

                    orgStructureMapDTO = Mapper.Map<OrgStructureMapDTO>(org);
                    orgStructureMapDTO.DivisionalObjectives = Mapper.Map<List<DivisionalObjectiveWithLEDDTO>>(org.DivisionalObjective);

                    foreach (var divisionalObjective in orgStructureMapDTO.DivisionalObjectives)
                    {
                        var SOKPIs = kpis.Where(w => w.DivisionalObjectiveID.HasValue && w.DivisionalObjectiveID.Value == divisionalObjective.ID).ToList();

                        #region Performance
                        var SOKPIsForCalculation = Calculationkpis.Where(w => w.DivisionalObjectiveID.HasValue && w.DivisionalObjectiveID.Value == divisionalObjective.ID).ToList();
                        decimal? SOPerformance = KPIBLL.CalculateKPIPerformace(SOKPIsForCalculation);
                        divisionalObjective.Performance = SOPerformance ?? 0;
                        if (SOPerformance.HasValue)
                        {
                            divisionalObjective.Status = systemPerformances
                                    .Where(w => w.Max == null || SystemPerformanceThresholdBLL.CalculateSystemOperation(SOPerformance.Value, (int)w.Max, true, w.MaxOperator))
                                    .Where(w => w.Min == null || SystemPerformanceThresholdBLL.CalculateSystemOperation(SOPerformance.Value, (int)w.Min, false, w.MinOperator))
                                    .Select(s => s.Code).FirstOrDefault();
                        }
                        else
                        {
                            divisionalObjective.Status = "NA";
                        }
                        #endregion



                    }

                    orgStructureMapDTOList.Add(orgStructureMapDTO);
                }
            }
            return orgStructureMapDTOList;
        }

        #endregion

        #region Update
        public OrgStructureDTO Update(OrgStructureDTO OrgStructureDTO, string UserName)
        {
            var org = AutoMapper.Mapper.Map<OrgStructure>(OrgStructureDTO);
            var res = OrgStructureBLL.Update(org, UserName);
            return AutoMapper.Mapper.Map<OrgStructureDTO>(res);
        }
        #endregion

        #region Delete
        public bool Delete(int id)
        {
            return OrgStructureBLL.Delete(id);
        }
        #endregion


    }
}

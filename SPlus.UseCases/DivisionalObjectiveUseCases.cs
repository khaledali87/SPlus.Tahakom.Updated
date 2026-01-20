using SPlus.BLL;
using SPlus.DTO;
using SPlus.Model.Domain;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPlus.UseCases
{
    public class DivisionalObjectiveUseCases : LoggingUseCases
    {
        Container _Container = IOC.InitializeContainer();
        private readonly DivisionalObjectiveBLL DivisionalObjectiveBLL;
        private readonly KPIBLL KPIBLL;
        private readonly OrgStructureBLL OrgStructureBLL;
        private readonly KPITypeBLL KPITypeBLL;
        private readonly SystemPerformanceThresholdBLL SystemPerformanceThresholdBLL;
        public DivisionalObjectiveUseCases()
        {
            DivisionalObjectiveBLL = _Container.GetInstance<DivisionalObjectiveBLL>();
            KPIBLL = _Container.GetInstance<KPIBLL>();
            KPITypeBLL = _Container.GetInstance<KPITypeBLL>();
            OrgStructureBLL = _Container.GetInstance<OrgStructureBLL>();
            SystemPerformanceThresholdBLL = _Container.GetInstance<SystemPerformanceThresholdBLL>();
        }
        #region Create

        public CADivisionalObjectiveDTO Create(CADivisionalObjectiveDTO divisionalObjective)
        {
            if (string.IsNullOrWhiteSpace(divisionalObjective.ArabicName))
                divisionalObjective.ArabicName = divisionalObjective.EnglishName;

            var allData = DivisionalObjectiveBLL.Read();
            if (allData.Any(a => a.Order == divisionalObjective.Order))
            {
                CADivisionalObjectiveDTO data = new CADivisionalObjectiveDTO();
                data.ID = -1;
                data.EnglishName = "Order Must be Unique";
                return data;
            }
            else
            {

                DivisionalObjective res = AutoMapper.Mapper.Map<DivisionalObjective>(divisionalObjective);
                var result = DivisionalObjectiveBLL.Create(res);


                return AutoMapper.Mapper.Map<CADivisionalObjectiveDTO>(result);
            }
        }
        #endregion

        #region Read
        public List<CADivisionalObjectiveDTO> Read()
        {
            List<CADivisionalObjectiveDTO> DivisionalcObjectives = AutoMapper.Mapper.Map<List<CADivisionalObjectiveDTO>>(DivisionalObjectiveBLL.Read());
            var kpis = KPIBLL.ReadForCalculation(null);
            List<SystemPerformanceThreshold> systemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold();

            foreach (var item in DivisionalcObjectives)
            {

                var strategicObjectiveKPIs = kpis.Where(a => a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID == item.ID && a.KPIType.IsDepartmental == true).ToList();

                #region Performance

                decimal? StrategicObjectivePerformance = KPIBLL.CalculateKPIPerformace(strategicObjectiveKPIs);
                item.Performance = StrategicObjectivePerformance.HasValue ? StrategicObjectivePerformance.Value : 0;
                if (StrategicObjectivePerformance.HasValue)
                {
                    item.Status = systemPerformances
                            .Where(w => w.Max != null ? SystemPerformanceThresholdBLL.CalculateSystemOperation(StrategicObjectivePerformance.Value, (int)w.Max, true, w.MaxOperator) : true)
                            .Where(w => w.Min != null ? SystemPerformanceThresholdBLL.CalculateSystemOperation(StrategicObjectivePerformance.Value, (int)w.Min, false, w.MinOperator) : true)
                            .Select(s => s.Code).FirstOrDefault();
                }
                else
                {
                    item.Status = "NA";
                }
                if (item.Status == null)
                    item.Status = "NA";


                #endregion
            }

            return DivisionalcObjectives.ToList();
        }






        public List<CAODivisionalObjectiveKPIDTO> ReadByStrategicObjectivesIDWithKPIs(int id, int? Year)
        {
            var res = DivisionalObjectiveBLL.ReadByStrategicObjectivesIDWithKPIs(id);
            if (Year.HasValue)
                res.ForEach(f => f.KPIs = f.KPIs.Where(w => w.StartDate.Year == Year.Value).ToList());
            List<CAODivisionalObjectiveKPIDTO> DivisionalcObjectives = AutoMapper.Mapper.Map<List<CAODivisionalObjectiveKPIDTO>>(res);

            return DivisionalcObjectives.ToList();
        }
        public CADivisionalObjectiveDTO ReadByID(int id, string UserName)
        {
            var res = DivisionalObjectiveBLL.ReadByID(id);

            var orgStructures = OrgStructureBLL.Read(UserName,null);
            if (res.OrgStructure != null && res.OrgStructure.ParentID.HasValue)
            {
                res.OrgStructure = OrgStructureBLL.BuildUpwardTree(res.OrgStructure, orgStructures);
            }

            CADivisionalObjectiveDTO DivisionalObjective = AutoMapper.Mapper.Map<CADivisionalObjectiveDTO>(res);
            return DivisionalObjective;
        }
        public List<CADivisionalObjectiveDTO> ReportDivisionalObjective(string UserName, string filterDate)
        {
            List<CADivisionalObjectiveDTO> DivisionalcObjectives = AutoMapper.Mapper.Map<List<CADivisionalObjectiveDTO>>(DivisionalObjectiveBLL.Read());
            var kpis = KPIBLL.ReadForCalculation(null);
            var orgStructures = OrgStructureBLL.Read(UserName,null);
            orgStructures = OrgStructureBLL.BuildTreeWithParents(orgStructures);
            List<SystemPerformanceThreshold> systemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold();

            if (!string.IsNullOrEmpty(filterDate))
            {
                if (DateTime.TryParse(filterDate, out DateTime FilterDate) && FilterDate != DateTime.MinValue)
                {
                    kpis = kpis.Where(w => w.KPIMeasures.Any(a => FilterDate.Date.Month == a.DueDate.Date.Month && FilterDate.Date.Year == a.DueDate.Date.Year)).ToList();
                    kpis.ForEach(f =>
                        f.KPIMeasures = f.KPIMeasures.Where(a => FilterDate.Date.Month == a.DueDate.Date.Month && FilterDate.Date.Year == a.DueDate.Date.Year).ToList()
                        );
                }
                KPIBLL.MapKPIProperties(kpis,false);
            }


            foreach (var item in DivisionalcObjectives)
            {

                var strategicObjectiveKPIs = kpis.Where(a => a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID == item.ID && a.KPIType.IsDepartmental == true).ToList();

                #region Performance

                decimal? StrategicObjectivePerformance = KPIBLL.CalculateKPIPerformace(strategicObjectiveKPIs);
                item.Performance = StrategicObjectivePerformance.HasValue ? StrategicObjectivePerformance.Value : 0;
                if (StrategicObjectivePerformance.HasValue)
                {
                    item.Status = systemPerformances
                            .Where(w => w.Max != null ? SystemPerformanceThresholdBLL.CalculateSystemOperation(StrategicObjectivePerformance.Value, (int)w.Max, true, w.MaxOperator) : true)
                            .Where(w => w.Min != null ? SystemPerformanceThresholdBLL.CalculateSystemOperation(StrategicObjectivePerformance.Value, (int)w.Min, false, w.MinOperator) : true)
                            .Select(s => s.Code).FirstOrDefault();
                }
                else
                {
                    item.Status = "NA";
                }
                if (item.Status == null)
                    item.Status = "NA";


                #endregion
            }

            return DivisionalcObjectives.ToList();
        }
        public DivisionalObjectiveWithCardDTO DivisionalObjectiveDetails(int ID, string userName, int? Year)
        {
            DivisionalObjectiveWithCardDTO finalDivisionalObjective = null;
            DivisionalObjective devisionalObjective = DivisionalObjectiveBLL.ReadByID(ID);
            List<KPI> CalculationKPIs = KPIBLL.ReadForCalculation(Year);
            var KPIs = KPIBLL.Read(userName,Year).Where(w => w.DivisionalObjectiveID.HasValue && w.DivisionalObjectiveID.Value == devisionalObjective.ID && w.KPIType.IsDepartmental == true).ToList();

            if (Year.HasValue)
            {
                KPIs = KPIs.Where(w => w.StartDate.Year == Year.Value).ToList();
                CalculationKPIs = CalculationKPIs.Where(w => w.StartDate.Year == Year.Value).ToList();
            }

            devisionalObjective.KPIs = KPIs;

            List<Status> Statuses = KPITypeBLL.ReadStatuses();
            List<SystemPerformanceThreshold> systemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold();

            if (devisionalObjective != null)
            {

                #region Performance
                var devisionalObjectiveKPIs = CalculationKPIs.Where(w => w.DivisionalObjectiveID.HasValue && w.DivisionalObjectiveID.Value == devisionalObjective.ID && w.KPIType.IsDepartmental == true).ToList();
                decimal? devisionalObjectivePerformance = KPIBLL.CalculateKPIPerformace(devisionalObjectiveKPIs);
                devisionalObjective.Performance = devisionalObjectivePerformance.HasValue ? devisionalObjectivePerformance.Value : 0;
                if (devisionalObjectivePerformance.HasValue)
                {
                    devisionalObjective.Status = systemPerformances
                            .Where(w => w.Max != null ? SystemPerformanceThresholdBLL.CalculateSystemOperation(devisionalObjectivePerformance.Value, (int)w.Max, true, w.MaxOperator) : true)
                            .Where(w => w.Min != null ? SystemPerformanceThresholdBLL.CalculateSystemOperation(devisionalObjectivePerformance.Value, (int)w.Min, false, w.MinOperator) : true)
                            .Select(s => s.Code).FirstOrDefault();
                }
                else
                {
                    devisionalObjective.Status = "NA";
                }
                if (devisionalObjective.Status == null)
                    devisionalObjective.Status = "NA";
                #endregion



                finalDivisionalObjective = AutoMapper.Mapper.Map<DivisionalObjectiveWithCardDTO>(devisionalObjective);

                #region ChartData
                finalDivisionalObjective.ChartData = new ChartDTO
                {
                    Count = devisionalObjective.KPIs.Count(),
                    ByStatus = Statuses.Select(f => new ByStatusDTO
                    {
                        Status = f.Code,
                        Count = devisionalObjective.KPIs.Where(w => w.Status == f.Code).Count(),
                    }).ToList(),
                };
                #endregion
                return finalDivisionalObjective;
            }
            return finalDivisionalObjective;
        }

        #endregion

        #region Update
        public CADivisionalObjectiveDTO Update(CADivisionalObjectiveDTO divisionalObjectiveDTO)
        {
            if (string.IsNullOrWhiteSpace(divisionalObjectiveDTO.ArabicName))
                divisionalObjectiveDTO.ArabicName = divisionalObjectiveDTO.EnglishName;

            var allData = DivisionalObjectiveBLL.Read();
            if (allData.Any(a => a.Order == divisionalObjectiveDTO.Order && a.ID != divisionalObjectiveDTO.ID))
            {
                CADivisionalObjectiveDTO data = new CADivisionalObjectiveDTO();
                data.ID = -1;
                data.EnglishName = "Order Must be Unique";
                return data;
            }
            else
            {

                DivisionalObjective divisionalObjective = AutoMapper.Mapper.Map<DivisionalObjective>(divisionalObjectiveDTO);
                var result = DivisionalObjectiveBLL.Update(divisionalObjective);
                return AutoMapper.Mapper.Map<CADivisionalObjectiveDTO>(result);
            }
        }
        #endregion

        #region Delete
        public bool Delete(int id)
        {
            return DivisionalObjectiveBLL.Delete(id);
        }
        #endregion



    }
}

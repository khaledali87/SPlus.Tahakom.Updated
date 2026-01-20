using AutoMapper;
using SPlus.BLL;
using SPlus.DTO;
using SPlus.Model.Domain;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPlus.UseCases
{
    public class StrategicObjectiveUseCases : LoggingUseCases
    {


        Container _Container = IOC.InitializeContainer();
        private readonly StrategicObjectiveBLL StrategicObjectiveBLL;
        private readonly KPIBLL KPIBLL;
        private readonly KPITypeBLL KPITypeBLL;
        private readonly ThemeBLL ThemeBLL;
        private readonly SystemPerformanceThresholdBLL SystemPerformanceThresholdBLL;
        public StrategicObjectiveUseCases()
        {
            StrategicObjectiveBLL = _Container.GetInstance<StrategicObjectiveBLL>();
            KPIBLL = _Container.GetInstance<KPIBLL>();
            ThemeBLL = _Container.GetInstance<ThemeBLL>();
            SystemPerformanceThresholdBLL = _Container.GetInstance<SystemPerformanceThresholdBLL>();
            KPITypeBLL = _Container.GetInstance<KPITypeBLL>();
        }



        #region Create

        public CAStrategicObjectiveDTO Create(CAStrategicObjectiveDTO strategicObjective)
        {
            StrategicObjective res = AutoMapper.Mapper.Map<StrategicObjective>(strategicObjective);

            var allData = StrategicObjectiveBLL.Read();
            if (allData.Any(a => a.Order == strategicObjective.Order))
            {
                CAStrategicObjectiveDTO data = new CAStrategicObjectiveDTO();
                data.ID = -1;
                data.EnglishName = "Order Must be Unique";
                return data;
            }
            else
            {

                var result = StrategicObjectiveBLL.Create(res);
                return AutoMapper.Mapper.Map<CAStrategicObjectiveDTO>(result);
            }
        }
        #endregion

        #region Read
        public List<CAStrategicObjectiveDTO> Read()
        {
            List<CAStrategicObjectiveDTO> StrategicObjectives = AutoMapper.Mapper.Map<List<CAStrategicObjectiveDTO>>(StrategicObjectiveBLL.Read());

            var kpis = KPIBLL.ReadForCalculation(null);
            List<SystemPerformanceThreshold> systemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold();
            foreach (var item in StrategicObjectives)
            {

                var strategicObjectiveKPIs = kpis.Where(a => a.StrategicObjectiveID == item.ID && !a.DivisionalObjectiveID.HasValue && a.KPIType.IsDepartmental == false).ToList();

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

            return StrategicObjectives.ToList();
        }
        public List<SimpleStrategicObjectiveDTO> StrategicObjectiveKPIs(string userName, int? Year)
        {

            var res = StrategicObjectiveBLL.ReadChild(userName);
            var kpis = KPIBLL.Read(userName,Year);

            if (Year.HasValue)
            {
                kpis = kpis.Where(w => w.StartDate.Year == Year.Value).ToList();
            }
            foreach (var o in res)
            {
                o.KPIs = kpis.Where(a => a.StrategicObjectiveID == o.ID).ToList();
            }
            foreach (var strategicObjective in res)//Need KPIType And Measures and KPIs
            {
                foreach (var item in strategicObjective.KPIs)
                {
                    item.IsDeletable = true;
                    if (KPIBLL.IsInGracePeriod(item))
                        item.AllowLock = false;
                    else
                        item.AllowLock = true;
                }

            }


            List<SimpleStrategicObjectiveDTO> StrategicObjective = AutoMapper.Mapper.Map<List<SimpleStrategicObjectiveDTO>>(res);
            return StrategicObjective;
        }
        public List<CAStrategicObjectiveDTO> ReportStrategicObjectives(string filterDate)
        {
            List<CAStrategicObjectiveDTO> StrategicObjectives = AutoMapper.Mapper.Map<List<CAStrategicObjectiveDTO>>(StrategicObjectiveBLL.Read());
            var kpis = KPIBLL.ReadForCalculation(null);

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

            List<SystemPerformanceThreshold> systemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold();
            foreach (var item in StrategicObjectives)
            {
                var strategicObjectiveKPIs = kpis.Where(a => a.StrategicObjectiveID == item.ID && !a.DivisionalObjectiveID.HasValue && a.KPIType.IsDepartmental == false).ToList();

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
            return StrategicObjectives;
        }
        public List<CAStrategicObjectiveKPIsDTO> ReadWithStrategicalKPIsByThemeID(int id, string username, int? Year)
        {
            var res = StrategicObjectiveBLL.ReadByThemeID(id);
            var kpis = KPIBLL.Read(username,Year).Where(a => !a.DivisionalObjectiveID.HasValue && a.StrategicObjectiveID.HasValue && a.StrategicObjectiveID > 0);

            if (Year.HasValue)
            {
                kpis = kpis.Where(w => w.StartDate.Year == Year.Value).ToList();
            }
            foreach (var strategicObjective in res)
            {
                strategicObjective.KPIs = kpis.Where(a => a.StrategicObjectiveID == strategicObjective.ID).ToList();
                foreach (var item in strategicObjective.KPIs)
                {
                    if (KPIBLL.IsInGracePeriod(item))
                        item.AllowLock = false;
                    else
                        item.AllowLock = true;

                }

            }

            List<CAStrategicObjectiveKPIsDTO> StrategicObjective = AutoMapper.Mapper.Map<List<CAStrategicObjectiveKPIsDTO>>(res);
            return StrategicObjective;
        }

        public CAStrategicObjectiveDTO ReadByID(int id)
        {
            var res = StrategicObjectiveBLL.ReadByID(id);
            CAStrategicObjectiveDTO StrategicObjective = AutoMapper.Mapper.Map<CAStrategicObjectiveDTO>(res);
            return StrategicObjective;
        }
        public StrategicObjectiveWithCardDTO StrategicObjectiveDetails(int ID, string userName, int? Year)
        {
            StrategicObjectiveWithCardDTO finalStrategicObjective = null;
            StrategicObjective strategicObjective = StrategicObjectiveBLL.ReadByID(ID);
            List<Status> Statuses = KPITypeBLL.ReadStatuses();
            List<KPI> kpis = KPIBLL.Read(userName,Year);
            
            List<KPI> CalculationKPIs = KPIBLL.ReadForCalculation(Year).ToList();
            if (Year.HasValue)
            {
                kpis = KPIBLL.FilterByYear(kpis, Year.Value);
                CalculationKPIs = KPIBLL.FilterByYear(CalculationKPIs, Year.Value);
            }
            List<SystemPerformanceThreshold> systemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold();
            strategicObjective.KPIs = kpis.Where(a => a.StrategicObjectiveID == ID && a.DivisionalObjectiveID.HasValue == false).ToList();
            strategicObjective.Theme = ThemeBLL.ReadByID(strategicObjective.Theme.ID);

            if (strategicObjective != null)
            {
                foreach (var div in strategicObjective.DivisionalObjectives)
                {
                    div.KPIs = kpis.Where(a => a.DivisionalObjectiveID == div.ID).ToList();
                }

                #region Performance
                var strategicObjectiveKPIs = CalculationKPIs.Where(w => w.StrategicObjectiveID == strategicObjective.ID && w.KPIType.IsDepartmental == false).ToList();
                decimal? StrategicObjectivePerformance = KPIBLL.CalculateKPIPerformace(strategicObjectiveKPIs);
                strategicObjective.Performance = StrategicObjectivePerformance.HasValue ? StrategicObjectivePerformance.Value : 0;
                if (StrategicObjectivePerformance.HasValue)
                {
                    strategicObjective.Status = systemPerformances
                            .Where(w => w.Max != null ? SystemPerformanceThresholdBLL.CalculateSystemOperation(StrategicObjectivePerformance.Value, (int)w.Max, true, w.MaxOperator) : true)
                            .Where(w => w.Min != null ? SystemPerformanceThresholdBLL.CalculateSystemOperation(StrategicObjectivePerformance.Value, (int)w.Min, false, w.MinOperator) : true)
                            .Select(s => s.Code).FirstOrDefault();
                }
                else
                {
                    strategicObjective.Status = "NA";
                }
                if (strategicObjective.Status == null)
                    strategicObjective.Status = "NA";

                foreach (var objective in strategicObjective.DivisionalObjectives)
                {
                    var divisionalObjectiveKPIs = CalculationKPIs.Where(w => w.DivisionalObjectiveID == objective.ID && w.KPIType.IsDepartmental == true).ToList();
                    decimal? divisionalObjectivePerformance = KPIBLL.CalculateKPIPerformace(divisionalObjectiveKPIs);
                    objective.Performance = divisionalObjectivePerformance.HasValue ? divisionalObjectivePerformance.Value : 0;
                    if (divisionalObjectivePerformance.HasValue)
                    {
                        objective.Status = systemPerformances
                                .Where(w => w.Max != null ? SystemPerformanceThresholdBLL.CalculateSystemOperation(divisionalObjectivePerformance.Value, (int)w.Max, true, w.MaxOperator) : true)
                                .Where(w => w.Min != null ? SystemPerformanceThresholdBLL.CalculateSystemOperation(divisionalObjectivePerformance.Value, (int)w.Min, false, w.MinOperator) : true)
                                .Select(s => s.Code).FirstOrDefault();
                    }
                    else
                    {
                        objective.Status = "NA";
                    }
                    if (objective.Status == null)
                        objective.Status = "NA";
                }
                #endregion

                finalStrategicObjective = AutoMapper.Mapper.Map<StrategicObjectiveWithCardDTO>(strategicObjective);

                #region ChartData
                finalStrategicObjective.KPIChart = new ChartDTO
                {
                    Count = strategicObjective.KPIs.Count(),
                    ByStatus = Statuses.Select(f => new ByStatusDTO
                    {
                        Status = f.Code,
                        Count = strategicObjective.KPIs.Where(w => w.Status == f.Code).Count(),
                    }).ToList(),
                };

                finalStrategicObjective.DivisionalObjectiveChart = new ChartDTO
                {
                    Count = strategicObjective.DivisionalObjectives.Count(),
                    ByStatus = Statuses.Select(f => new ByStatusDTO
                    {
                        Status = f.Code,
                        Count = strategicObjective.DivisionalObjectives.Where(w => w.Status == f.Code).Count(),
                    }).ToList(),
                };
                #endregion

                return finalStrategicObjective;
            }
            return finalStrategicObjective;
        }

        public List<StrategicObjectiveChildDTO> ReadWithDivisional()
        {

            var ReadWithDivisionalObjectives = StrategicObjectiveBLL.ReadWithDivisionalObjectives();
            //var divisionals = DivisionalObjectiveBLL.ReadForWeightDepartmentalObjectie();
            List<StrategicObjectiveChildDTO> finalList = new List<StrategicObjectiveChildDTO>();
            StrategicObjectiveChildDTO final = null;



            foreach (var objective in ReadWithDivisionalObjectives)
            {
                final = new StrategicObjectiveChildDTO();
                final = Mapper.Map<StrategicObjectiveChildDTO>(objective);


                finalList.Add(final);
            }

            return finalList;
        }



        #endregion

        #region Update
        public CAStrategicObjectiveDTO Update(CAStrategicObjectiveDTO StrategicObjectiveDTO)
        {
            if (string.IsNullOrWhiteSpace(StrategicObjectiveDTO.ArabicName))
                StrategicObjectiveDTO.ArabicName = StrategicObjectiveDTO.EnglishName;
            var allData = StrategicObjectiveBLL.Read();
            if (allData.Any(a => a.Order == StrategicObjectiveDTO.Order && a.ID != StrategicObjectiveDTO.ID))
            {
                CAStrategicObjectiveDTO data = new CAStrategicObjectiveDTO();
                data.ID = -1;
                data.EnglishName = "Order Must be Unique";
                return data;
            }
            else
            {


                StrategicObjective strategicObjective = AutoMapper.Mapper.Map<StrategicObjective>(StrategicObjectiveDTO);
                var result = StrategicObjectiveBLL.Update(strategicObjective);
                return AutoMapper.Mapper.Map<CAStrategicObjectiveDTO>(result);
            }
        }
        #endregion

        #region Delete
        public bool Delete(int id)
        {
            return StrategicObjectiveBLL.Delete(id);
        }
        #endregion
    }
}

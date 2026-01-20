using AutoMapper;
using SPlus.BLL;
using SPlus.DTO;
using SPlus.Model.Domain;
using StructureMap;
using System.Collections.Generic;
using System.Linq;

namespace SPlus.UseCases
{
    public class StrategyUseCases : LoggingUseCases
    {


        Container _Container = IOC.InitializeContainer();
        private readonly StrategyBLL StrategyBLL;
        private readonly StrategicObjectiveBLL StrategicObjectiveBLL;
        private readonly KPIBLL KPIBLL;
        private readonly SystemPerformanceThresholdBLL SystemPerformanceThresholdBLL;
        private readonly KPITypeBLL KPITypeBLL;
        public StrategyUseCases()
        {
            StrategyBLL = _Container.GetInstance<StrategyBLL>();
            StrategicObjectiveBLL = _Container.GetInstance<StrategicObjectiveBLL>();
            KPIBLL = _Container.GetInstance<KPIBLL>();
            KPITypeBLL = _Container.GetInstance<KPITypeBLL>();
            SystemPerformanceThresholdBLL = _Container.GetInstance<SystemPerformanceThresholdBLL>();
        }


        #region Create
        public CAStrategyDTO Create(CAStrategyDTO strategy)
        {
            if (string.IsNullOrWhiteSpace(strategy.ArabicName))
                strategy.ArabicName = strategy.EnglishName;
            Strategy res = AutoMapper.Mapper.Map<Strategy>(strategy);
            return AutoMapper.Mapper.Map<CAStrategyDTO>(StrategyBLL.Create(res));
        }

        #endregion

        #region Read

        public List<CAStrategyDTO> Read(int? Year = null)
        {
            var res = StrategyBLL.Read(Year);

            if (Year.HasValue)
                res = res.Where(w => w.Years.Any(a => a == Year.ToString())).ToList();

            List<CAStrategyDTO> strategies = AutoMapper.Mapper.Map<List<CAStrategyDTO>>(res);
            return strategies;

        }


        public CAStrategyDTO ReadByID(int ID)
        {

            var res = StrategyBLL.ReadByID(ID);
            CAStrategyDTO strategy = AutoMapper.Mapper.Map<CAStrategyDTO>(res);
            return strategy;

        }
        public List<CAStrategyThemeDTO> ReadWithTheme(int? Year = null)
        {
            var res = StrategyBLL.ReadWithTheme(Year);

            if (Year.HasValue)
                res = res.Where(w => w.Years.Any(a => a == Year.ToString())).ToList();

            foreach (var strategy in res)
            {
                foreach (var theme in strategy.Themes)
                {
                    theme.IsDeletable = theme.StrategicObjectives.Count() == 0;
                }
            }

            List<CAStrategyThemeDTO> strategies = AutoMapper.Mapper.Map<List<CAStrategyThemeDTO>>(res);
            return strategies;

        }

        public List<CAStrategyPerspectiveDTO> ReadWithPerspective(int? Year = null)
        {
            var res = StrategyBLL.ReadWithPerspective(Year);

            if (Year.HasValue)
                res = res.Where(w => w.Years.Any(a => a == Year.ToString())).ToList();

            foreach (var strategy in res)
            {
                foreach (var perspective in strategy.Perspectives)
                {
                    perspective.IsDeletable = perspective.KPIs.Count() == 0;
                }
            }

            List<CAStrategyPerspectiveDTO> strategies = AutoMapper.Mapper.Map<List<CAStrategyPerspectiveDTO>>(res);
            return strategies;

        }
        public List<CAStrategyObjectiveDTO> ReadWithObjective(int? Year = null)
        {
            var res = StrategyBLL.ReadWithObjectives(Year);

            if (Year.HasValue)
                res = res.Where(w => w.Years.Any(a => a == Year.ToString())).ToList();

            foreach (var strategy in res)
            {
                foreach (var theme in strategy.Themes)
                {
                    foreach (var strategicObjetive in theme.StrategicObjectives)
                    {
                        if (strategicObjetive.KPIs.Count() == 0 && strategicObjetive.DivisionalObjectives.Count() == 0)
                            strategicObjetive.IsDeletable = true;
                        else
                            strategicObjetive.IsDeletable = false;
                    }
                }
            }
            List<CAStrategyObjectiveDTO> strategies = Mapper.Map<List<CAStrategyObjectiveDTO>>(res);
            return strategies;
        }

        public List<CAOrgStructureDivisionalObjectiveDTO> ReadWithDivisionalObjectives(int? Year = null)
        {
            var res = StrategyBLL.ReadWithDivisionalObjectives(Year);

            if (Year.HasValue)
                res = res.Where(w => w.Years.Any(a => a == Year.ToString())).ToList();

            foreach (var strategy in res)
            {
                foreach (var divisionalObjective in strategy.DivisionalObjective)
                {

                    if (divisionalObjective.KPIs.Count() == 0)
                        divisionalObjective.IsDeletable = true;
                    else
                        divisionalObjective.IsDeletable = false;

                }
            }
            List<CAOrgStructureDivisionalObjectiveDTO> strategies = Mapper.Map<List<CAOrgStructureDivisionalObjectiveDTO>>(res);
            return strategies;
        }

        public List<CAStrategyObjectiveListDTO> ReadWithObjectiveList(int? Year = null)
        {
            var strategies = StrategyBLL.ReadWithObjectives(Year);
            List<CAStrategyObjectiveListDTO> finalList = new List<CAStrategyObjectiveListDTO>();
            CAStrategyObjectiveListDTO final = null;

            if (Year.HasValue)
                strategies = strategies.Where(w => w.Years.Any(a => a == Year.ToString())).ToList();

            foreach (var strategy in strategies)
            {
                final = new CAStrategyObjectiveListDTO();
                final = Mapper.Map<CAStrategyObjectiveListDTO>(strategy);
                var strategyThemesObjectives = strategy.Themes.ToList();
                var StrategicObjectives = strategyThemesObjectives.SelectMany(s => s.StrategicObjectives).Distinct().ToList();
                final.StrategicObjectives = Mapper.Map<List<StrategicObjectiveListDTO>>(StrategicObjectives);
                finalList.Add(final);
            }

            return finalList;
        }
        public StrategyMapDTO StrategyMap(int StrategyID, string UserName, int? Year)
        {
            StrategyMapDTO strategyMap = null;
            Strategy strategy = StrategyBLL.ReadStrategyMap(StrategyID);
            if (strategy != null)
            {
                var systemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold();
                List<KPI> kpis = KPIBLL.Read(UserName,Year).Where(a => a.KPIType.IsDepartmental == false).ToList();
                if (Year.HasValue)
                    kpis = KPIBLL.FilterByYear(kpis, Year.Value);
                List<KPI> Calculationkpis = KPIBLL.ReadForCalculation(Year);
                List<Status> Statuses = KPITypeBLL.ReadStatuses();

                strategy.Themes = strategy.Themes.OrderBy(o => o.Order).ToList();
                strategyMap = Mapper.Map<StrategyMapDTO>(strategy);
                foreach (var theme in strategyMap.Themes)
                {
                    theme.StrategicObjectives = theme.StrategicObjectives.OrderBy(o => o.Order).ToList();

                    foreach (var strategicObjective in theme.StrategicObjectives)
                    {
                        var SOKPIs = kpis.Where(w => w.StrategicObjectiveID == strategicObjective.ID).ToList();

                        #region Map Cards

                        strategicObjective.KPIs = Mapper.Map<List<KPILEDDTO>>(SOKPIs);

                        #endregion

                        #region Performance
                        var SOKPIsForCalculation = Calculationkpis.Where(w => w.StrategicObjectiveID == strategicObjective.ID && w.DivisionalObjectiveID == null).ToList();
                        //decimal? SOPerformance = KPIBLL.CalculateKPIPerformace(SOKPIsForCalculation);
                        decimal? SOPerformance = KPIBLL.CalculateKPIPerformace(SOKPIsForCalculation);

                        strategicObjective.Performance = SOPerformance ?? 0;
                        if (SOPerformance.HasValue)
                        {
                            strategicObjective.Status = systemPerformances
                                    .Where(w => w.Max == null || SystemPerformanceThresholdBLL.CalculateSystemOperation(SOPerformance.Value, (int)w.Max, true, w.MaxOperator))
                                    .Where(w => w.Min == null || SystemPerformanceThresholdBLL.CalculateSystemOperation(SOPerformance.Value, (int)w.Min, false, w.MinOperator))
                                    .Select(s => s.Code).FirstOrDefault();
                        }
                        else
                        {
                            strategicObjective.Status = "NA";
                        }
                        #endregion

                    }
                }
            }
            return strategyMap;
        }

        #endregion

        #region Update
        public CAStrategyDTO Update(CAStrategyDTO strategy)
        {
            if (string.IsNullOrWhiteSpace(strategy.ArabicName))
                strategy.ArabicName = strategy.EnglishName;

            Strategy res = AutoMapper.Mapper.Map<Strategy>(strategy);
            return AutoMapper.Mapper.Map<CAStrategyDTO>(StrategyBLL.Update(res));
        }

        #endregion

        #region Delete
        public bool Delete(int id)
        {
            return StrategyBLL.Delete(id);
        }

        #endregion



    }
}

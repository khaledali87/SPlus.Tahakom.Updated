using SPlus.BLL;
using SPlus.Model.Domain;
using SPlus.DTO;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPlus.UseCases
{
    public class GeneralDashboardUseCases : LoggingUseCases
    {

        Container _Container = IOC.InitializeContainer();
        private readonly ThemeBLL ThemeBLL;
        private readonly StrategicObjectiveBLL StrategicObjectiveBLL;
        private readonly KPITypeBLL KPITypeBLL;
        private readonly KPIBLL KPIBLL;
        private readonly OrgStructureBLL OrgStructureBLL;
        private readonly SystemPerformanceThresholdBLL SystemPerformanceThresholdBLL;
        public GeneralDashboardUseCases()
        { 
            ThemeBLL = _Container.GetInstance<ThemeBLL>();
            StrategicObjectiveBLL = _Container.GetInstance<StrategicObjectiveBLL>();
            KPIBLL = _Container.GetInstance<KPIBLL>();
            KPITypeBLL = _Container.GetInstance<KPITypeBLL>();
            OrgStructureBLL = _Container.GetInstance<OrgStructureBLL>();
            SystemPerformanceThresholdBLL = _Container.GetInstance<SystemPerformanceThresholdBLL>();
        }
        
        public GeneralDashboardDTO GetGeneralDashboard(string UserName, int KPITypeID = 0)
        {
            GeneralDashboardDTO dashboard = new GeneralDashboardDTO();
            List<KPI> KPIsForCalculations = KPIBLL.ReadForCalculation();
            List<KPI> kpis = KPIBLL.Read(UserName);
            List<OrgStructure> orgStructures = OrgStructureBLL.Read();
            List<Status> statuses = KPITypeBLL.ReadStatuses();
            List<Theme> themes = ThemeBLL.Read();
            List<StrategicObjective> strategicObjectives = StrategicObjectiveBLL.Read();
            List<SystemPerformanceThreshold> systemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold();
            if (KPITypeID != 0)
            {
                kpis = kpis.Where(w => w.KPITypeID == KPITypeID).ToList();
            }

            dashboard.KPICount = kpis.Count();
            dashboard.KPIsByStatus = GetKPIsByStatus(kpis, statuses);
            dashboard.KPIsByUpdateStatus = GetKPIsByUpdateStatuse(kpis);
            dashboard.KPIsByDivision = GetKPIsByDivision(kpis, orgStructures, statuses);
            dashboard.KPIsByTheme = GetKPIsByTheme(kpis, themes,strategicObjectives, systemPerformances, KPIsForCalculations);
            dashboard.StrategicObjectivesByTheme = GetStrategicObjectiveByTheme(kpis, themes, strategicObjectives, systemPerformances, KPIsForCalculations);

            return dashboard;
        }
        public List<KPIByStatusDTO> GetKPIsByStatus(List<KPI> kpis, List<Status> statuses)
        {
            KPIByStatusDTO kPIByStatus = null;
            List<KPIByStatusDTO> kPIByStatuses = new List<KPIByStatusDTO>();
            foreach (var status in statuses)
            {
                List<KPI> kpisbystatus = KPIBLL.GetKPIsActiveMeasures(kpis.Where(w => w.KPIMeasures.FirstOrDefault().Status == status.Code)).ToList();
                kPIByStatus = new KPIByStatusDTO();
                kPIByStatus.KPICount = kpisbystatus.Count();
                kPIByStatus.Status = status.Code;
                kPIByStatus.KPIs = GetDashboardKPIs(kpisbystatus);
                kPIByStatuses.Add(kPIByStatus);
            }
           
            return kPIByStatuses;
        }
        public List<KPIByUpdateStatusDTO> GetKPIsByUpdateStatuse(List<KPI> kpis)
        {
            KPIByUpdateStatusDTO KPIByUpdateStatus = null;
            List<KPIByUpdateStatusDTO> KPIByUpdateStatuses = new List<KPIByUpdateStatusDTO>();
            Dictionary<bool, List<KPI>> dic = new Dictionary<bool, List<KPI>>();
            //kpis =kpis.Where(w => !w.RequireUpdate).ToList();

            dic.Add(true, kpis.Where(w => w.RequireUpdate).ToList());
            dic.Add(false, kpis.Where(w => !w.RequireUpdate).ToList());

            foreach (var gkpi in dic)
            {
                KPIByUpdateStatus = new KPIByUpdateStatusDTO();
                KPIByUpdateStatus.RequireUpdate = gkpi.Key;
                KPIByUpdateStatus.Percentage = kpis.Count() != 0 ? Math.Round((gkpi.Value.Count() / (decimal)kpis.Count() * 100), 2) : 0;
                KPIByUpdateStatus.KPICount = gkpi.Value.Count();
                var currentmeasuresKPI = KPIBLL.GetKPIsActiveMeasures(gkpi.Value.ToList());
                KPIByUpdateStatus.OldestDueDate = currentmeasuresKPI.Count() > 0 ? currentmeasuresKPI.Min(m => m.KPIMeasures.FirstOrDefault().DueDate) : default;
                KPIByUpdateStatus.KPIs = GetDashboardKPIs(gkpi.Value.ToList());
                KPIByUpdateStatuses.Add(KPIByUpdateStatus);
            }

            return KPIByUpdateStatuses;
        }
        public List<KPIByDivisionDTO> GetKPIsByDivision(List<KPI> kpis, List<OrgStructure> orgStructures, List<Status> statuses)
        {
            List<KPIByDivisionDTO> KPIByDivisions = new List<KPIByDivisionDTO>();
            KPIByDivisionDTO KPIByDivision = null;
            foreach (var org in orgStructures.Where(w => w.ParentID == null))
            {
                KPIByDivision = new KPIByDivisionDTO();
                List<KPI> relatedKPIs = kpis.Where(w => w.DivisionID == org.ID).ToList();
                KPIByDivision.KPICount = relatedKPIs.Count();
                KPIByDivision.RequireUpdate = relatedKPIs.Where(w => w.RequireUpdate).Count();
                KPIByDivision.Updated = relatedKPIs.Where(w => !w.RequireUpdate).Count();
                KPIByDivision.EnglishName = org.EnglishName;
                KPIByDivision.ArabicName = org.ArabicName;
                KPIByDivision.KPIsByStatus = GetKPIsByStatus(relatedKPIs, statuses);
                KPIByDivisions.Add(KPIByDivision);
            }
            return KPIByDivisions;
        }
        public List<KPIByThemeDTO> GetKPIsByTheme(List<KPI> kpis, List<Theme> themes, List<StrategicObjective> strategicObjectives, List<SystemPerformanceThreshold> systemPerformances, List<KPI> KPIsForCalculations)
        {
            List<KPIByThemeDTO> KPIByThemes = new List<KPIByThemeDTO>();
            KPIByThemeDTO KPIByTheme = null;
            foreach (var theme in themes)
            {
                List<KPI> relatedKPIs = kpis.Where(w => w.StrategicObjective?.ThemeID == theme.ID).ToList();
                KPIsForCalculations = KPIsForCalculations.Where(w => w.StrategicObjective?.ThemeID == theme.ID).ToList();
                List<StrategicObjective> relatedSO = strategicObjectives.Where(w => w.ThemeID == theme.ID).ToList();
                KPIByTheme = new KPIByThemeDTO();
                KPIByTheme.EnglishName = theme.EnglishName;
                KPIByTheme.ArabicName = theme.ArabicName;
                KPIByTheme.KPICount = relatedKPIs.Count();
                KPIByTheme.Performance = CalculateThemesPerformace(KPIsForCalculations, relatedSO);
                KPIByTheme.Status = systemPerformances
                        .Where(w => w.Max != null ? SystemPerformanceThresholdBLL.CalculateOperation(KPIByTheme.Performance, (int)w.Max, true, w.MaxOperator) : true)
                        .Where(w => w.Min != null ? SystemPerformanceThresholdBLL.CalculateOperation(KPIByTheme.Performance, (int)w.Min, false, w.MinOperator) : true)
                        .Select(s => s.Code).FirstOrDefault();
                KPIByTheme.KPIs = GetDashboardKPIs(relatedKPIs);
                KPIByThemes.Add(KPIByTheme);
            }
            return KPIByThemes;
        }
        public List<StrategicObjectiveByThemeDTO> GetStrategicObjectiveByTheme(List<KPI> kpis, List<Theme> themes, List<StrategicObjective> strategicObjectives, List<SystemPerformanceThreshold> systemPerformances, List<KPI> KPIsForCalculations)
        {
            List<StrategicObjectiveByThemeDTO> StrategicObjectiveByThemes = new List<StrategicObjectiveByThemeDTO>();
            StrategicObjectiveByThemeDTO StrategicObjectiveByTheme = null;
            foreach (var theme in themes)
            {
                var relatedKPIs = kpis.Where(w => w.StrategicObjective?.ThemeID == theme.ID).ToList();
                KPIsForCalculations = KPIsForCalculations.Where(w => w.StrategicObjective?.ThemeID == theme.ID).ToList();
                var relatedSO = strategicObjectives.Where(w => w.ThemeID == theme.ID).ToList();
                StrategicObjectiveByTheme = new StrategicObjectiveByThemeDTO();
                StrategicObjectiveByTheme.EnglishName = theme.EnglishName;
                StrategicObjectiveByTheme.ArabicName = theme.ArabicName;
                StrategicObjectiveByTheme.Attachment = AutoMapper.Mapper.Map<AttachmentDTO>(theme.Attachment);
                StrategicObjectiveByTheme.KPIsByStrategicObjective = GetKPIsByStrategicObjectives(relatedKPIs, relatedSO, systemPerformances, KPIsForCalculations);
                StrategicObjectiveByThemes.Add(StrategicObjectiveByTheme);
            }

            return StrategicObjectiveByThemes;
        }
        public List<KPIByStrategicObjectiveDTO> GetKPIsByStrategicObjectives(List<KPI> kpis, List<StrategicObjective> strategicObjectives, List<SystemPerformanceThreshold> systemPerformances, List<KPI> KPIsForCalculations)
        {
            List<KPIByStrategicObjectiveDTO> KPIByStrategicObjectives = new List<KPIByStrategicObjectiveDTO>();
            KPIByStrategicObjectiveDTO KPIByStrategicObjective = null;
            foreach (var strategic in strategicObjectives)
            {
                KPIByStrategicObjective = new KPIByStrategicObjectiveDTO();
                var relatedkpis = kpis.Where(w => w.StrategicObjectiveID == strategic.ID).ToList();
                KPIsForCalculations = KPIsForCalculations.Where(w => w.StrategicObjectiveID == strategic.ID).ToList();
                KPIByStrategicObjective.KPICount = relatedkpis.Count();
                KPIByStrategicObjective.EnglishName = strategic.EnglishName;
                KPIByStrategicObjective.ArabicName = strategic.ArabicName;
                KPIByStrategicObjective.Performance = CalculateStrategicObjectivesPerformace(KPIsForCalculations);
                KPIByStrategicObjective.Status = systemPerformances
                        .Where(w => w.Max != null ? SystemPerformanceThresholdBLL.CalculateOperation(KPIByStrategicObjective.Performance,  w.Max.Value, true, w.MaxOperator) : true)
                        .Where(w => w.Min != null ? SystemPerformanceThresholdBLL.CalculateOperation(KPIByStrategicObjective.Performance, (int)w.Min, false, w.MinOperator) : true)
                        .Select(s => s.Code).FirstOrDefault();
                KPIByStrategicObjective.KPIs = GetDashboardKPIs(relatedkpis);
                KPIByStrategicObjectives.Add(KPIByStrategicObjective);
            }
            return KPIByStrategicObjectives;
        }
        private List<DashboardKPIDetailsDTO> GetDashboardKPIs(List<KPI> kpis)
        {
            //var x = AutoMapper.Mapper.Map<OrgStructureListDTO>(kpis[0].OrgStructure);
            List<DashboardKPIDetailsDTO> KPIs = AutoMapper.Mapper.Map<List<DashboardKPIDetailsDTO>>(kpis);
            foreach (var kpi in KPIs)
            {
                var mappedKPI = kpis.Where(w => w.ID == kpi.ID).FirstOrDefault();
                kpi.Value = KPIBLL.GetKPIActiveMeasures(mappedKPI).KPIMeasures.FirstOrDefault().Value ?? default;
                kpi.Target = KPIBLL.GetKPIActiveMeasures(mappedKPI).KPIMeasures.FirstOrDefault().Target;
                kpi.OutOfTarget = KPIBLL.GetKPIActiveMeasures(mappedKPI).KPIMeasures.FirstOrDefault().OutOfTarget ?? default;
                kpi.LastUpdate = KPIBLL.GetKPIActiveMeasures(mappedKPI).KPIMeasures.FirstOrDefault().UpdateDate ?? default;
                kpi.Status = KPIBLL.GetKPIActiveMeasures(mappedKPI).KPIMeasures.FirstOrDefault().Status ?? default;
            }
            return KPIs;
        }
        private decimal CalculateStrategicObjectivesPerformace(List<KPI> kpis)
        {
            decimal performance = Math.Round(KPIBLL.GetKPIsActiveMeasures(kpis).Sum(s => (s.KPIMeasures.FirstOrDefault().OutOfTarget ?? default) * s.Weight) / 100, 2);
            return performance;
        }
        private decimal CalculateThemesPerformace(List<KPI> kpis, List<StrategicObjective> strategicObjectives)
        {
            decimal performance = 0;
            foreach (var strategic in strategicObjectives)
            {
                var relatedKPI = kpis.Where(w => w.StrategicObjectiveID == strategic.ID).ToList();
                performance += CalculateStrategicObjectivesPerformace(relatedKPI) * strategic.Weight;
            }
            performance = Math.Round(performance / 100,2);
            return performance;
        }
      
    }
}

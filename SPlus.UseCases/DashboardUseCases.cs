using AutoMapper;
using Newtonsoft.Json;
using SPlus.BLL;
using SPlus.DTO;
using SPlus.Helper;
using SPlus.Model;
using SPlus.Model.Domain;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace SPlus.UseCases
{
    public class DashboardUseCases : LoggingUseCases
    {
        Container _Container = IOC.InitializeContainer();
        private readonly ThemeBLL ThemeBLL;
        private readonly PerspectiveBLL PerspectiveBLL;
        private readonly DivisionalObjectiveBLL DivisionalObjectiveBLL;
        private readonly StrategicObjectiveBLL StrategicObjectiveBLL;
        private readonly KPIBLL KPIBLL;
        private readonly SystemPerformanceThresholdBLL SystemPerformanceThresholdBLL;
        private readonly KPITypeBLL KPITypeBLL;
        private readonly OrgStructureBLL OrgStructureBLL;
        private readonly HandshakeBLL HandshakeBLL;
        private readonly RequestBLL RequestBLL;

        public DashboardUseCases()
        {
            PerspectiveBLL = _Container.GetInstance<PerspectiveBLL>();
            KPIBLL = _Container.GetInstance<KPIBLL>();
            KPITypeBLL = _Container.GetInstance<KPITypeBLL>();
            StrategicObjectiveBLL = _Container.GetInstance<StrategicObjectiveBLL>();
            SystemPerformanceThresholdBLL = _Container.GetInstance<SystemPerformanceThresholdBLL>();
            ThemeBLL = _Container.GetInstance<ThemeBLL>();
            DivisionalObjectiveBLL = _Container.GetInstance<DivisionalObjectiveBLL>();
            OrgStructureBLL = _Container.GetInstance<OrgStructureBLL>();
            HandshakeBLL = _Container.GetInstance<HandshakeBLL>();
            RequestBLL = _Container.GetInstance<RequestBLL>();
        }

        #region Startegy Dashboard

        public StrategyPerformanceDashboardDTO GetStrategyPerformanceDashboardDTO(string UserName, int strategy, int? Year)
        {
            StrategyPerformanceDashboardDTO dashboard = new StrategyPerformanceDashboardDTO();

            List<KPI> allStrategyKPIs = new List<KPI>();
            List<KPI> allCalculationKPIs = new List<KPI>();
            List<KPI> StrategicCalculationKPIs = new List<KPI>();
            StrategicCalculationKPIs = new List<KPI>();
            List<Theme> allThemes = new List<Theme>();
            List<CADivisionalObjectiveDTO> DivisionalcObjectives = new List<CADivisionalObjectiveDTO>();
            List<StrategicObjective> allStrategicObjective = new List<StrategicObjective>();
            List<SystemPerformanceThreshold> systemPerformances = new List<SystemPerformanceThreshold>();

            List<Task> tasks = new List<Task>
            {
                Task.Run(() => allStrategyKPIs = KPIBLL.ReadDashboardKPIs(UserName,Year).Where(a => a.StrategicObjective?.Theme?.StrategyID == strategy && a.DivisionalObjectiveID == null).ToList()),
                Task.Run(() => allCalculationKPIs = KPIBLL.ReadForCalculation(Year).ToList()),
                Task.Run(() => StrategicCalculationKPIs = KPIBLL.ReadForCalculation(Year).Where(w => w.DivisionalObjectiveID == null).ToList()),
                Task.Run(() => allThemes = ThemeBLL.Read().Where(a => a.StrategyID == strategy).ToList()),
                Task.Run(() => allStrategicObjective = StrategicObjectiveBLL.ReadObjectivesForDashboard().Where(a => a.Theme?.StrategyID == strategy).ToList()),
                Task.Run(() => DivisionalcObjectives = AutoMapper.Mapper.Map<List<CADivisionalObjectiveDTO>>(DivisionalObjectiveBLL.Read())),
                Task.Run(() => systemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold())
            };

            Task.WaitAll(tasks.ToArray());

            if (Year.HasValue)
            {
                allStrategyKPIs = allStrategyKPIs.Where(a => a.StartDate.Year == Year).ToList();
                allCalculationKPIs = allCalculationKPIs.Where(a => a.StartDate.Year == Year).ToList();
                StrategicCalculationKPIs = StrategicCalculationKPIs.Where(a => a.StartDate.Year == Year).ToList();
            }

            dashboard.RequireUpdate = allStrategyKPIs.Where(w => w.RequireUpdate).Count();
            dashboard.UpdatedCount = allStrategyKPIs.Where(w => !w.RequireUpdate).Count();
            dashboard.KPIByStatuses = GetKPIsByStatus(allStrategyKPIs);
            dashboard.Themes = new List<StrategyPerformanceThemeDTO>();
            foreach (var item in allThemes)
            {
                StrategyPerformanceThemeDTO obj = new StrategyPerformanceThemeDTO();
                obj.ArabicName = item.ArabicName;
                obj.EnglishName = item.EnglishName;
                obj.ID = item.ID;
                obj.KPIs = Mapper.Map<List<KPIDetailsDTO>>(allStrategyKPIs.Where(a => a.StrategicObjective?.Theme.ID == item.ID && a.DivisionalObjectiveID == null).ToList());
                obj.RequireUpdate = allStrategyKPIs.Where(a => a.StrategicObjective?.Theme.ID == item.ID && a.RequireUpdate).Count();
                obj.UpdatedCount = allStrategyKPIs.Where(a => a.StrategicObjective?.Theme.ID == item.ID && !a.RequireUpdate).Count();
                dashboard.Themes.Add(obj);
            }
            dashboard.StrategicObjectives = new List<StrategyPerformanceSODTO>();
            dashboard.StrategicObjectives = Mapper.Map<List<StrategyPerformanceSODTO>>(allStrategicObjective).ToList();

            foreach (var item in dashboard.StrategicObjectives)
            {
                List<KPI> soKPIs = allStrategyKPIs.Where(a => a.StrategicObjectiveID == item.ID && a.DivisionalObjectiveID == null).ToList();
                item.KPIByStatuses = GetKPIsByStatus(soKPIs);
                decimal? SOPerformance = KPIBLL.CalculateKPIPerformace(soKPIs);

                item.Performance = SOPerformance ?? 0;
                if (SOPerformance.HasValue)
                {
                    item.Status = CalculateStatus(SOPerformance.Value, systemPerformances);

                }
                else
                {
                    item.Status = "NA";
                }

                item.DivisionalObjectives = new List<DistributionDataByStatus>();
                List<CADivisionalObjectiveDTO> cADivisionalObjectiveDTOs = ReadForDashboards(DivisionalcObjectives, allCalculationKPIs, systemPerformances)
                    .Where(a => a.StrategicObjective?.ID == item.ID).ToList();

                item.DivisionalObjectives = GetDivisionalObjectiveByStatus(cADivisionalObjectiveDTOs);
            }


            foreach (var item in dashboard.Themes.SelectMany(a => a.KPIs))
            {
                if (item.StrategicObjective != null)
                {
                    item.StrategicObjective.Performance = dashboard.StrategicObjectives.Where(a => a.ID == item.StrategicObjective.ID).FirstOrDefault().Performance;
                    item.StrategicObjective.Status = dashboard.StrategicObjectives.Where(a => a.ID == item.StrategicObjective.ID).FirstOrDefault().Status;
                }

            }

            return dashboard;
        }
        public StrategicObjectiveDashboardDTO GetStrategicObjectiveDetails(string userName, int ID, int? Year)
        {
            StrategicObjectiveDashboardDTO dashboard = new StrategicObjectiveDashboardDTO();

            StrategicObjective strategicObjective = StrategicObjectiveBLL.ReadByID(ID);
            List<Status> Statuses = KPITypeBLL.ReadStatuses();
            List<KPI> kpis = KPIBLL.Read(userName,Year);
            List<KPI> CalculationKPIs = KPIBLL.ReadForCalculation(Year).ToList();


            if (Year.HasValue)
            {
                kpis = kpis.Where(a => a.StartDate.Year == Year).ToList();
                CalculationKPIs = CalculationKPIs.Where(a => a.StartDate.Year == Year).ToList();
            }


            List<KPI> tmpKPIPreWithnoTrim = new List<KPI>();
            List<SystemPerformanceThreshold> SystemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold();
            strategicObjective.KPIs = kpis.Where(a => a.StrategicObjectiveID == ID && a.DivisionalObjectiveID.HasValue == false).ToList();


            if (strategicObjective != null)
            {
                foreach (var div in strategicObjective.DivisionalObjectives)
                {
                    div.KPIs = kpis.Where(a => a.DivisionalObjectiveID == div.ID).ToList();
                }

                #region Performance
                var strategicObjectiveKPIs = CalculationKPIs.Where(w => w.StrategicObjectiveID == strategicObjective.ID && w.DivisionalObjectiveID.HasValue == false).ToList();
                decimal? StrategicObjectivePerformance = KPIBLL.CalculateKPIPerformace(strategicObjectiveKPIs);
                strategicObjective.Performance = StrategicObjectivePerformance.HasValue ? StrategicObjectivePerformance.Value : 0;
                if (StrategicObjectivePerformance.HasValue)
                {
                    strategicObjective.Status = CalculateStatus(StrategicObjectivePerformance.Value, SystemPerformances);
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
                        objective.Status = CalculateStatus(divisionalObjectivePerformance.Value, SystemPerformances);
                    }
                    else
                    {
                        objective.Status = "NA";
                    }
                    if (objective.Status == null)
                        objective.Status = "NA";
                }
                #endregion


                dashboard = AutoMapper.Mapper.Map<StrategicObjectiveDashboardDTO>(strategicObjective);

                tmpKPIPreWithnoTrim = CalculationKPIs.Where(w => w.StrategicObjectiveID == strategicObjective.ID && w.DivisionalObjectiveID.HasValue == false).ToList();

                decimal? performance_Prev = KPIBLL.CalculateKPIPerformace_Prev(tmpKPIPreWithnoTrim);

                if (performance_Prev.HasValue && performance_Prev.HasValue)
                {
                    dashboard.VariancePerformance = dashboard.Performance - performance_Prev.Value;


                    if (dashboard.VariancePerformance < 0)
                        dashboard.VarianceStatus = "OVD";
                    else if (dashboard.VariancePerformance == 0)
                        dashboard.VarianceStatus = "DLY";
                    else

                        dashboard.VarianceStatus = "OTR";

                    //dashboard.VarianceStatus = CalculateStatus(dashboard.VariancePerformance, SystemPerformances);

                }
                else
                {
                    dashboard.VariancePerformance = 0;
                    dashboard.VarianceStatus = "NA";
                }

                if (StrategicObjectivePerformance.HasValue)
                {


                    dashboard.AvragePerformance = Math.Round(dashboard.KPIs.Any() ? dashboard.Performance / dashboard.KPIs.Where(w => w.KPIMeasures.Any(a => a.Status != "NA")).Count() : 0, 2);

                    if (dashboard.KPIs.Any())
                    {
                        dashboard.AvrageStatus = CalculateStatus(dashboard.AvragePerformance, SystemPerformances);
                    }
                    else
                        dashboard.AvrageStatus = "NA";
                }
                else
                {
                    dashboard.AvragePerformance = 0;
                    dashboard.AvrageStatus = "NA";
                }


            }


            return dashboard;
        }
        public ThemeDashboardDTO GetThemeDashboardDTO(string UserName, int themeId, int? Year)
        {
            ThemeDashboardDTO dashboard = new ThemeDashboardDTO();

            List<KPI> allStrategyKPIs = KPIBLL.ReadDashboardKPIs(UserName, Year).Where(a => a.StrategicObjective?.Theme?.ID == themeId && a.DivisionalObjectiveID == null).ToList();
            List<KPI> CalculationKPIs = KPIBLL.ReadForCalculation(Year).Where(a => a.StrategicObjective?.Theme?.ID == themeId && a.DivisionalObjectiveID == null).ToList();
            List<KPI> allCalculationKPIs = KPIBLL.ReadForCalculation(Year).ToList();
            Theme theme = ThemeBLL.ReadByID(themeId);
            List<StrategicObjective> allStrategicObjective = StrategicObjectiveBLL.ReadObjectivesForDashboard().Where(a => a.Theme?.ID == themeId).ToList();
            var SystemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold();

            if (Year.HasValue)
            {
                allStrategyKPIs = allStrategyKPIs.Where(a => a.StartDate.Year == Year).ToList();
                CalculationKPIs = CalculationKPIs.Where(a => a.StartDate.Year == Year).ToList();
                allCalculationKPIs = allCalculationKPIs.Where(a => a.StartDate.Year == Year).ToList();
            }

            List<CADivisionalObjectiveDTO> DivisionalcObjectives = AutoMapper.Mapper.Map<List<CADivisionalObjectiveDTO>>(DivisionalObjectiveBLL.Read());
            dashboard.ArabicName = theme.ArabicName;
            dashboard.EnglishName = theme.EnglishName;

            dashboard.StrategicObjectives = new List<StrategyPerformanceSODTO>();
            dashboard.StrategicObjectives = Mapper.Map<List<StrategyPerformanceSODTO>>(allStrategicObjective).ToList();

            foreach (var item in dashboard.StrategicObjectives)
            {
                List<KPI> soKPIs = allStrategyKPIs.Where(a => a.StrategicObjectiveID == item.ID).ToList();
                item.KPIByStatuses = GetKPIsByStatus(soKPIs);
                decimal? SOPerformance = KPIBLL.CalculateKPIPerformace(soKPIs);

                item.Performance = SOPerformance ?? 0;
                if (SOPerformance.HasValue)
                {
                    item.Status = CalculateStatus(SOPerformance.Value, SystemPerformances);
                }
                else
                {
                    item.Status = "NA";
                }

                item.DivisionalObjectives = new List<DistributionDataByStatus>();
                List<CADivisionalObjectiveDTO> cADivisionalObjectiveDTOs = ReadForDashboards(DivisionalcObjectives, allCalculationKPIs, SystemPerformances)
                    .Where(a => a.StrategicObjective?.ID == item.ID).ToList();

                item.DivisionalObjectives = GetDivisionalObjectiveByStatus(cADivisionalObjectiveDTOs);




            }





            #region Performance

            decimal? themePerformance = KPIBLL.CalculateKPIPerformace(CalculationKPIs);
            dashboard.Performance = themePerformance.HasValue ? themePerformance.Value : 0;
            if (themePerformance.HasValue)
            {
                dashboard.Status = CalculateStatus(themePerformance.Value, SystemPerformances);
            }
            else
            {
                dashboard.Status = "NA";
            }
            if (dashboard.Status == null)
                dashboard.Status = "NA";






            decimal? performance_Prev = KPIBLL.CalculateKPIPerformace_Prev(CalculationKPIs);

            if (performance_Prev.HasValue && performance_Prev.HasValue)
            {
                dashboard.VariancePerformance = dashboard.Performance - performance_Prev.Value;

                  if (dashboard.VariancePerformance < 0)
                        dashboard.VarianceStatus = "OVD";
                    else if (dashboard.VariancePerformance == 0)
                        dashboard.VarianceStatus = "DLY";
                    else

                        dashboard.VarianceStatus = "OTR";


              //  dashboard.VarianceStatus = CalculateStatus(dashboard.VariancePerformance, SystemPerformances);
            }
            else
            {
                dashboard.VariancePerformance = 0;
                dashboard.VarianceStatus = "NA";
            }

            if (themePerformance.HasValue)
            {


                dashboard.AvragePerformance = Math.Round(CalculationKPIs.Any() ? dashboard.Performance / CalculationKPIs.Where(w => w.KPIMeasures.Any(a => a.Status != "NA")).Count() : 0, 2);

                if (CalculationKPIs.Any())
                {
                    dashboard.AvrageStatus = CalculateStatus(dashboard.AvragePerformance, SystemPerformances);
                }
                else
                    dashboard.AvrageStatus = "NA";
            }
            else
            {
                dashboard.AvragePerformance = 0;
                dashboard.AvrageStatus = "NA";
            }



            #endregion
            return dashboard;
        }

        #endregion

        #region Operational Dashboard




        #region Commented 
        public OperationalDashboardDTO GetOperationalDashboardDTO(string userName, int? Year)
        {
            OperationalDashboardDTO Dashboard = new OperationalDashboardDTO();
            List<SystemPerformanceThreshold> SystemPerformances = new List<SystemPerformanceThreshold>();
            List<OrgStructure> OrgStructures = new List<OrgStructure>();
            List<KPI> CalculationKPIs = new List<KPI>();
            List<KPI> KPIs = new List<KPI>();
            List<KPI> AllSystemKPIs = new List<KPI>();
            List<StrategicObjective> StrategicObjective = new List<StrategicObjective>();
            List<DivisionalObjective> DivisionalObjective = new List<DivisionalObjective>();

            List<Task> tasks = new List<Task>
            {
                Task.Run(() => OrgStructures = OrgStructureBLL.ReadOrgWithDivisional_new(userName,Year)),
                Task.Run(() => SystemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold()),
                Task.Run(() => CalculationKPIs = KPIBLL.ReadOpertionalForCalculations(Year).Where(a => a.DivisionalObjectiveID.HasValue || a.OrgStructureID.HasValue).ToList()),
                Task.Run(() => KPIs = KPIBLL.ReadOpertionalForBalanceScoreCards(userName,Year).Where(a => a.DivisionalObjectiveID.HasValue || a.OrgStructureID.HasValue).ToList()),
                Task.Run(() => DivisionalObjective = DivisionalObjectiveBLL.Read()),
                Task.Run(() => StrategicObjective = StrategicObjectiveBLL.ReadObjectivesForDashboard().ToList()),
                Task.Run(() => AllSystemKPIs = KPIBLL.ReadOpertionalForBalanceScoreCards(userName,Year).ToList())
            };
            Task.WaitAll(tasks.ToArray());


            if (Year.HasValue)
            {
                KPIs = KPIs.Where(a => a.StartDate.Year == Year).ToList();
                CalculationKPIs = CalculationKPIs.Where(a => a.StartDate.Year == Year).ToList();
                AllSystemKPIs = AllSystemKPIs.Where(a => a.StartDate.Year == Year).ToList();
                OrgStructures = OrgStructures.Where(w => w.Years.Any(a => a == Year.ToString())).ToList();
            }
            OrgStructures = OrgStructures.Where(w => w.ParentID is null || OrgStructures.Any(a => a.ID == w.ParentID)).ToList();
            DivisionalObjective = DivisionalObjective.Where(w => OrgStructures.Any(a => a.ID == w.OrgStructureId)).ToList();

            Dashboard.DivisionalObjective = Mapper.Map<List<DivisionalObjectiveDTO>>(DivisionalObjective).ToList();
            foreach (var item in Dashboard.DivisionalObjective)
            {
                var divisionalObjectiveKPIs = CalculationKPIs.Where(a => a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == item.ID).ToList();
                decimal? divisionalObjectivePerformance = KPIBLL.CalculateKPIPerformace(divisionalObjectiveKPIs);
                item.Performance = divisionalObjectivePerformance.HasValue ? divisionalObjectivePerformance.Value : 0;
                if (divisionalObjectivePerformance.HasValue)
                {
                    item.Status = CalculateStatus(divisionalObjectivePerformance.Value, SystemPerformances);
                }
                else
                {
                    item.Status = "NA";
                }
                if (item.Status == null)
                    item.Status = "NA";


                item.KPIByStatuses = GetKPIsByStatus(KPIs.Where(a => a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == item.ID).ToList());

                item.RequireUpdate = KPIs.Where(a => a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == item.ID && a.RequireUpdate).Count();
                item.UpdatedCount = KPIs.Where(a => a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == item.ID && !a.RequireUpdate).Count();
            }



            Dashboard.OperationalOrgStructure = new List<OperationalOrgStructureDTO>();
            //for Departemnts

            #region Departemnts
            foreach (var item in OrgStructures.Where(a => a.ID != Constants.CorporateDepartmentID && a.ID != Constants.CorporateSectortID))
            {
                List<KPI> DepartmentCalculationKPI = new List<KPI>();
                List<KPI> DepartmentCalculationKPI_Previous = new List<KPI>();
                List<KPI> DepartmentKPIs = new List<KPI>();
                if (item.DivisionalObjective != null && item.DivisionalObjective.Any())
                {
                    foreach (var d in item.DivisionalObjective)
                    {
                        d.KPIs = KPIs.Where(a => (a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == d.ID)).ToList();

                        DepartmentKPIs.AddRange(KPIs.Where(a => (a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == d.ID)).ToList());

                        DepartmentCalculationKPI.AddRange(CalculationKPIs.Where(a => (a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == d.ID)).ToList());
                        DepartmentCalculationKPI_Previous.AddRange(CalculationKPIs.Where(a => (a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == d.ID)).ToList());
                    }
                }
                // add stratigic
                DepartmentKPIs.AddRange(KPIs.Where(a => (!a.DivisionalObjectiveID.HasValue && a.OrgStructureID.HasValue && a.OrgStructureID.Value == item.ID)).ToList());
                DepartmentCalculationKPI.AddRange(CalculationKPIs.Where(a => (!a.DivisionalObjectiveID.HasValue && a.OrgStructureID.HasValue && a.OrgStructureID.Value == item.ID)).ToList());
                DepartmentCalculationKPI_Previous.AddRange(CalculationKPIs.Where(a => (!a.DivisionalObjectiveID.HasValue && a.OrgStructureID.HasValue && a.OrgStructureID.Value == item.ID)).ToList());
                item.KPIList = DepartmentKPIs;
                item.KPIs = DepartmentKPIs;

                item.KPIsCount = DepartmentKPIs.Count();
                decimal? deprtmentPerformance = KPIBLL.CalculateDepartmentalKPIPerformace(DepartmentCalculationKPI);
                item.Performance = deprtmentPerformance.HasValue ? deprtmentPerformance.Value : 0;

                if (deprtmentPerformance.HasValue)
                {
                    item.Status = CalculateStatus(deprtmentPerformance.Value, SystemPerformances);
                }
                else
                {
                    item.Status = "NA";
                }


                decimal? DeprtmentPerformance_Prev = KPIBLL.CalculateDepartmentalKPIPerformace_Prev(DepartmentCalculationKPI_Previous);
                item.PreviousPerformance = DeprtmentPerformance_Prev.HasValue ? DeprtmentPerformance_Prev.Value : 0;

                if (DeprtmentPerformance_Prev.HasValue)
                {
                    item.PreviousStatus = CalculateStatus(DeprtmentPerformance_Prev.Value, SystemPerformances);
                }
                else
                {
                    item.PreviousStatus = "NA";
                }

                if (DeprtmentPerformance_Prev.HasValue && deprtmentPerformance.HasValue)
                {
                    item.Variance = item.Performance - item.PreviousPerformance;


                    if (item.Variance < 0)
                        item.VarianceStatus = "OVD";
                    else if (item.Variance == 0)
                        item.VarianceStatus = "DLY";
                    else

                        item.VarianceStatus = "OTR";


                    //item.VarianceStatus = CalculateStatus(item.Variance, SystemPerformances);

                }
                else
                {
                    item.Variance = 0;
                    item.VarianceStatus = "NA";
                }


                decimal? AveragePerformance = KPIBLL.CalculateDepartmentalKPIPeriodicPerformace(DepartmentCalculationKPI);
                item.AvragePerformance = AveragePerformance ?? 0;

                if (AveragePerformance.HasValue)
                {
                    item.AvrageStatus = CalculateStatus(item.AvragePerformance, SystemPerformances);
                }
                else
                    item.AvrageStatus = "NA";

                OperationalOrgStructureDTO Department = new OperationalOrgStructureDTO();

                Department.KPIs = Mapper.Map<List<KPIDetailsDTO>>(item.KPIs);
                Department.ID = item.ID;
                Department.ArabicName = item.ArabicName;
                Department.EnglishName = item.EnglishName;
                Department.AvrageStatus = item.AvrageStatus;
                Department.AvragePerformance = item.AvragePerformance;
                Department.Performance = item.Performance;
                Department.PreviousPerformance = item.PreviousPerformance;
                Department.Status = item.Status;
                Department.VariancePerformance = item.Variance;
                Department.VarianceStatus = item.VarianceStatus;
                Department.CanSeeOperationalPerformance = true;
                Department.OperationalPerformanceID = item.ID;
                Department.IsSector = item.ParentID is null;
                Dashboard.OperationalOrgStructure.Add(Department);

            }
            #endregion

            //for Sectors
            #region Sectors
            //foreach (var item in OrgStructures.Where(a => a.ID != Constants.CorporateSectortID && !a.ParentID.HasValue))
            //{
            //    List<OrgStructure> Departmenets = OrgStructures.Where(a => a.ParentID.HasValue && a.ParentID.Value == item.ID).OrderBy(a => a.ID).ToList();

            //    OperationalOrgStructureDTO department = new OperationalOrgStructureDTO();

            //    department.KPIs = Mapper.Map<List<KPIDetailsDTO>>(Departmenets.SelectMany(a => a.KPIs).ToList());
            //    department.ID = item.ID;
            //    department.IsSector = true;
            //    department.ArabicName = item.ArabicName;
            //    department.EnglishName = item.EnglishName;


            //    if (Departmenets.Any())
            //    {
            //        department.CanSeeOperationalPerformance = true;
            //        department.OperationalPerformanceID = Departmenets.FirstOrDefault().ID;
            //    }
            //    else
            //    {
            //        department.CanSeeOperationalPerformance = false;
            //        department.OperationalPerformanceID = 0;
            //    }
            //    if (Departmenets.Any() && Departmenets.SelectMany(a => a.KPIs).ToList().Any())
            //    {
            //        if (Departmenets.SelectMany(a => a.KPIs).ToList().Where(s => s.Status != "NA").Any())
            //        {
            //            if (Departmenets.Where(d => d.Status != "NA").Any())
            //            {
            //                department.Performance = Math.Round(Departmenets.Any() ? Departmenets.Sum(d => d.Performance) / Departmenets.Count() : 0, 2);
            //                department.Status = CalculateStatus(department.Performance, SystemPerformances);
            //            }
            //            else
            //            {
            //                department.Performance = 0;
            //                department.Status = "NA";
            //            }

            //            if (Departmenets.Where(d => d.AvrageStatus != "NA").Any())
            //            {
            //                department.AvragePerformance = Math.Round(Departmenets.Any() ? Departmenets.Sum(d => d.AvragePerformance) / Departmenets.Count() : 0, 2);
            //                 department.AvrageStatus = CalculateStatus(department.AvragePerformance, SystemPerformances);

            //            }
            //            else
            //            {
            //                department.AvragePerformance = 0;
            //                department.AvrageStatus = "NA";
            //            }
            //            if (Departmenets.Where(d => d.VarianceStatus != "NA").Any())
            //            {
            //                department.VariancePerformance = Math.Round(Departmenets.Any() ? Departmenets.Sum(d => d.Variance) / Departmenets.Count() : 0, 2);
            //                department.VarianceStatus = CalculateStatus(department.VariancePerformance, SystemPerformances);
            //            }
            //            else
            //            {
            //                department.VariancePerformance = 0;
            //                department.VarianceStatus = "NA";
            //            }
            //        }
            //        else
            //        {
            //            department.Performance = 0;
            //            department.Status = "NA";
            //            department.AvragePerformance = 0;
            //            department.AvrageStatus = "NA";
            //            department.VariancePerformance = 0;
            //            department.VarianceStatus = "NA";
            //        }
            //    }
            //    else
            //    {
            //        department.Performance = 0;
            //        department.Status = "NA";
            //        department.AvragePerformance = 0;
            //        department.AvrageStatus = "NA";
            //        department.VariancePerformance = 0;
            //        department.VarianceStatus = "NA";
            //    }



            //    Dashboard.OperationalOrgStructure.Add(department);
            //}
            #endregion

            //for Corporate
            #region Corporate
            {
                Func<KPI, bool> CorporateOrgStructureFilterPredicate = s =>
                   (!s.DivisionalObjectiveID.HasValue && s.OrgStructureID.HasValue &&
                       (s.OrgStructureID.Value == Constants.CorporateDepartmentID ||
                       s.OrgStructureID.Value == Constants.CorporateSectortID)) ||
                       (s.DivisionalObjectiveID.HasValue && s.DivisionalObjective != null &&
                       (s.DivisionalObjective.OrgStructureId == Constants.CorporateDepartmentID ||
                       s.DivisionalObjective.OrgStructureId == Constants.CorporateSectortID));

                OrgStructureCardDTO Corporate = new OrgStructureCardDTO();
                Corporate.DepartmentsCount = OrgStructures.Where(d => d.ParentID.HasValue && d.ParentID.Value == Constants.CorporateSectortID).Count();
                Corporate.ArabicName = OrgStructures.Where(d => d.ID == Constants.CorporateSectortID).FirstOrDefault()?.ArabicName;
                Corporate.EnglishName = OrgStructures.Where(d => d.ID == Constants.CorporateSectortID).FirstOrDefault()?.EnglishName;
                Corporate.ID = Constants.CorporateSectortID;
                var CorporateKPIs = KPIs.Where(CorporateOrgStructureFilterPredicate).ToList();

                List<KPI> tmpCoparteKPIsNoTrim = new List<KPI>();
                List<KPI> tmpCoparteKPIsNoTrim_prev = new List<KPI>();

                tmpCoparteKPIsNoTrim.AddRange(CalculationKPIs.Where(CorporateOrgStructureFilterPredicate).ToList());
                tmpCoparteKPIsNoTrim_prev.AddRange(CalculationKPIs.Where(CorporateOrgStructureFilterPredicate).ToList());
                Corporate.KPIsCount = CorporateKPIs.Count();

                Corporate.IsCorporate = true;

                decimal? coparatedeprtmentPerformance = KPIBLL.CalculateDepartmentalKPIPerformace(tmpCoparteKPIsNoTrim);
                Corporate.Performance = coparatedeprtmentPerformance.HasValue ? coparatedeprtmentPerformance.Value : 0;

                if (coparatedeprtmentPerformance.HasValue)
                {
                    Corporate.Status = CalculateStatus(coparatedeprtmentPerformance.Value, SystemPerformances);
                }
                else
                {
                    Corporate.Status = "NA";
                }


                decimal? copratedeprtmentPerformance_Prev = KPIBLL.CalculateDepartmentalKPIPerformace_Prev(tmpCoparteKPIsNoTrim_prev);
                var PreviousPerformance = copratedeprtmentPerformance_Prev.HasValue ? copratedeprtmentPerformance_Prev.Value : 0;


                if (copratedeprtmentPerformance_Prev.HasValue && coparatedeprtmentPerformance.HasValue)
                {
                    Corporate.VariancePerformance = Corporate.Performance - PreviousPerformance;

                    if (Corporate.VariancePerformance < 0)
                        Corporate.VarianceStatus = "OVD";
                    else if (Corporate.VariancePerformance == 0)
                        Corporate.VarianceStatus = "DLY";
                    else

                        Corporate.VarianceStatus = "OTR";
                    //Corporate.VarianceStatus = CalculateStatus(Corporate.VariancePerformance, SystemPerformances);
                }
                else
                {
                    Corporate.VariancePerformance = 0;
                    Corporate.VarianceStatus = "NA";
                }

                decimal? AveragePerformance = KPIBLL.CalculateDepartmentalKPIPeriodicPerformace(tmpCoparteKPIsNoTrim);
                Corporate.AvragePerformance = AveragePerformance ?? 0;

                if (AveragePerformance.HasValue)
                {
                    Corporate.AvrageStatus = CalculateStatus(Corporate.AvragePerformance, SystemPerformances);
                }
                else
                    Corporate.AvrageStatus = "NA";

                OperationalOrgStructureDTO coprateSector = new OperationalOrgStructureDTO();

                coprateSector.KPIs = Mapper.Map<List<KPIDetailsDTO>>(CorporateKPIs.ToList());
                coprateSector.ID = Corporate.ID;
                coprateSector.IsCorporate = true;
                coprateSector.IsSector = true;
                coprateSector.ArabicName = Corporate.ArabicName;
                coprateSector.EnglishName = Corporate.EnglishName;
                coprateSector.AvrageStatus = Corporate.AvrageStatus;
                coprateSector.AvragePerformance = Corporate.AvragePerformance;
                coprateSector.Performance = Corporate.Performance;
                coprateSector.PreviousPerformance = Corporate.PreviousPerformance;
                coprateSector.Status = Corporate.Status;
                coprateSector.VariancePerformance = Corporate.VariancePerformance;
                coprateSector.VarianceStatus = Corporate.VarianceStatus;
                Dashboard.OperationalOrgStructure.Add(coprateSector);
            }
            #endregion

            Dashboard.KPIByStatuses = GetKPIsByStatus(KPIs);

            if (Dashboard.OperationalOrgStructure.Where(a => a.Status != "NA").Any())
            {
                if (Dashboard.OperationalOrgStructure.Where(a => a.Status != "NA").Any())
                {
                    Dashboard.Performance = Math.Round(Dashboard.OperationalOrgStructure.Any() ? Dashboard.OperationalOrgStructure.Sum(d => d.Performance) / Dashboard.OperationalOrgStructure.Count() : 0, 2);
                    Dashboard.Status = CalculateStatus(Dashboard.Performance, SystemPerformances);
                }
                else
                {
                    Dashboard.Performance = 0;
                    Dashboard.Status = "NA";
                }

                if (Dashboard.OperationalOrgStructure.Where(a => a.AvrageStatus != "NA").Any())
                {
                    Dashboard.AvragePerformance = Math.Round(Dashboard.OperationalOrgStructure.Any() ? Dashboard.OperationalOrgStructure.Sum(d => d.AvragePerformance) / Dashboard.OperationalOrgStructure.Count() : 0, 2);
                    Dashboard.AvrageStatus = CalculateStatus(Dashboard.AvragePerformance, SystemPerformances);
                }
                else
                {
                    Dashboard.AvragePerformance = 0;
                    Dashboard.AvrageStatus = "NA";
                }

                if (Dashboard.OperationalOrgStructure.Where(a => a.VarianceStatus != "NA").Any())
                {
                    Dashboard.VariancePerformance = Math.Round(Dashboard.OperationalOrgStructure.Any() ? Dashboard.OperationalOrgStructure.Sum(d => d.VariancePerformance) / Dashboard.OperationalOrgStructure.Count() : 0, 2);


                    if (Dashboard.VariancePerformance < 0)
                        Dashboard.VarianceStatus = "OVD";
                    else if (Dashboard.VariancePerformance == 0)
                        Dashboard.VarianceStatus = "DLY";
                    else

                        Dashboard.VarianceStatus = "OTR";

                    //Dashboard.VarianceStatus = CalculateStatus(Dashboard.VariancePerformance, SystemPerformances);
                }
                else
                {
                    Dashboard.VariancePerformance = 0;
                    Dashboard.VarianceStatus = "NA";
                }

                if (Dashboard.OperationalOrgStructure.Any())
                {
                    Dashboard.PreviousPerformance = Math.Round(Dashboard.OperationalOrgStructure.Any() ? Dashboard.OperationalOrgStructure.Sum(d => d.PreviousPerformance) / Dashboard.OperationalOrgStructure.Count() : 0, 2);
                }
                else
                {
                    Dashboard.PreviousPerformance = 0;
                }
            }
            else
            {
                Dashboard.Performance = 0;
                Dashboard.Status = "NA";
                Dashboard.AvragePerformance = 0;
                Dashboard.AvrageStatus = "NA";
                Dashboard.VariancePerformance = 0;
                Dashboard.VarianceStatus = "NA";

            }
            Dashboard.OperationalOrgStructure = Dashboard.OperationalOrgStructure.OrderByDescending(a => a.IsSector).ThenByDescending(a => a.IsCorporate).ToList();


            foreach (var item in StrategicObjective)
            {
                List<KPI> StrategicObjectiveKPIs = AllSystemKPIs.Where(a => a.StrategicObjectiveID == item.ID && a.DivisionalObjectiveID == null).ToList();

                decimal? StrategicObjectivePerformance = KPIBLL.CalculateKPIPerformace(StrategicObjectiveKPIs);

                item.Performance = StrategicObjectivePerformance ?? 0;
                if (StrategicObjectivePerformance.HasValue)
                {
                    item.Status = CalculateStatus(StrategicObjectivePerformance.Value, SystemPerformances);
                }
                else
                {
                    item.Status = "NA";
                }
            }

            foreach (var item in Dashboard.OperationalOrgStructure.SelectMany(a => a.KPIs))
            {
                if (item.StrategicObjective != null)
                {
                    item.StrategicObjective.Performance = StrategicObjective.Where(a => a.ID == item.StrategicObjective.ID).Select(s => s.Performance).FirstOrDefault();
                    item.StrategicObjective.Status = StrategicObjective.Where(a => a.ID == item.StrategicObjective.ID).Select(s => s.Status).FirstOrDefault();
                }
            }

            return Dashboard;
        }
        #endregion
        #endregion

        #region CR Dashboard

        public CRDashboardDTO GetCRDashboardDTO(string UserName, int? Year)
        {
            CRDashboardDTO dashboard = new CRDashboardDTO();
            List<OrgStructure> orgStructures = new List<OrgStructure>();
            orgStructures = OrgStructureBLL.ReadOrgWithDivisional_new(UserName,Year).ToList();
            List<Request> allCRRequests = RequestBLL.ReadCRDashboardSubmitedChangeRequest().ToList();

            List<KPI> allKPIs = KPIBLL.ReadForCalculation(Year);
            var definition = new { ChangeType = new List<int>(), ID = 0, };

            if (Year.HasValue)
            {
                allKPIs = allKPIs.Where(a => a.StartDate.Year == Year).ToList();
                orgStructures = orgStructures.Where(a => a.Years.Any(year => year == Year.Value.ToString())).ToList();

            }

            allCRRequests = allCRRequests.Where(w => allKPIs.Any(k => k.ID == JsonConvert.DeserializeAnonymousType(w.Form, definition).ID)).ToList();

            dashboard.TotlalCount = allCRRequests.Count();
            dashboard.RejectedCount = allCRRequests.Where(a => a.Status == (int)EnumWFStatuses.Rejected).Count();

            List<LookupValue> CRType = HandshakeBLL.ReadByKey("kpichangerequestchangetype").LookupValues.ToList();
            List<DistributionDataByStatus> CRChangeType = new List<DistributionDataByStatus>();
            dashboard.CRByType = new List<DistributionDataByChangeType>();
            foreach (var item in CRType)
            {
                DistributionDataByChangeType obj = new DistributionDataByChangeType();
                obj.ChangeTypeId = Convert.ToInt32(item.Value);
                obj.ArabicName = item.Arabic;
                obj.EnglishName = item.English;
                obj.Count = allCRRequests.Where(a => JsonConvert.DeserializeAnonymousType(a.Form, definition).ChangeType.Contains(Convert.ToInt32(item.Value))).Count();
                dashboard.CRByType.Add(obj);
            }

            dashboard.CRByStatuses = GetRequestByStatus(allCRRequests);

            dashboard.CRByDepartment = new List<DistributionDataByDepartment>();
            foreach (var item in orgStructures)
            {
                DistributionDataByDepartment obj = new DistributionDataByDepartment();
                obj.ArabicName = item.ArabicName;
                obj.EnglishName = item.EnglishName;
                obj.DepartmentId = item.ID;

                var departemntKPIs = allKPIs.Where(a => a.DivisionalObjective?.OrgStructureId == item.ID || (a.OrgStructureID.HasValue && a.OrgStructureID.Value == item.ID)).ToList();
                var allCRKPIIDs = allCRRequests.Select(a => JsonConvert.DeserializeAnonymousType(a.Form, definition).ID).Where(i => departemntKPIs.Any(k => k.ID == i)).Count();

                obj.Count = allCRKPIIDs;

                dashboard.CRByDepartment.Add(obj);
            }



            return dashboard;

        }

        #endregion

        #region KPI Relation Dashboard

        public List<KPIRelationDTO> GetKPIRelationDashboard(string userName, int? Year)
        {
            List<KPIRelationDTO> dashboard = new List<KPIRelationDTO>();
            List<Theme> themes = new List<Theme>();
            List<Perspective> perspectivs = new List<Perspective>();
            List<KPI> allKPIs = new List<KPI>();
            List<KPI> CalculationKPIs = new List<KPI>();
            List<KPIAffect> KPIAffects = new List<KPIAffect>();
            List<StrategicObjective> strategicObjectives = new List<StrategicObjective>();
            List<SystemPerformanceThreshold> SystemPerformances = new List<SystemPerformanceThreshold>();

            List<Task> tasks = new List<Task>
            {
                Task.Run(() => themes = ThemeBLL.ReadWithAttachment()),
                Task.Run(() => perspectivs = PerspectiveBLL.ReadWithAttachment()),
                Task.Run(() => allKPIs = KPIBLL.ReadDashboardKPIs(userName,Year).Where(a => a.StrategicObjectiveID.HasValue && !a.DivisionalObjectiveID.HasValue).ToList()),
                Task.Run(() => CalculationKPIs = KPIBLL.ReadForCalculation(Year).Where(a => a.StrategicObjectiveID.HasValue && !a.DivisionalObjectiveID.HasValue).ToList()),
                Task.Run(() => KPIAffects = KPIBLL.ReadWithAffect().ToList()),
                Task.Run(() => strategicObjectives = StrategicObjectiveBLL.Read().ToList()),
                Task.Run(() => SystemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold())
            };

            Task.WaitAll(tasks.ToArray());

            if (Year.HasValue)
            {
                allKPIs = allKPIs.Where(a => a.StartDate.Year == Year).ToList();
                CalculationKPIs = CalculationKPIs.Where(a => a.StartDate.Year == Year).ToList();
            }

            foreach (var item in themes)
            {
                KPIRelationDTO objTheme = new KPIRelationDTO();
                objTheme.ArabicName = item.ArabicName;
                objTheme.Attachment = Mapper.Map<AttachmentDTO>(item.Attachment);
                objTheme.EnglishName = item.EnglishName;
                decimal? themePerformance = KPIBLL.CalculateKPIPerformaceForTheme(CalculationKPIs.Where(a => a.StrategicObjective?.ThemeID == item.ID).ToList(), strategicObjectives);
                objTheme.Performance = themePerformance.HasValue ? themePerformance.Value : 0;

                if (themePerformance.HasValue)
                {
                    objTheme.Status = CalculateStatus(themePerformance.Value, SystemPerformances);
                }
                else
                {
                    objTheme.Status = "NA";
                }

                //Add all perpective to facilate the FE team to draw the chart
                objTheme.Perspective = Mapper.Map<List<KPIRelationPerspectiveDTO>>(perspectivs).ToList();

                //Add new Other perspective for the KPI that does not has perspective
                objTheme.Perspective.Add(new KPIRelationPerspectiveDTO
                {
                    ArabicName = "أخرى",
                    EnglishName = "Other",
                    BackgroundColor = ConfigurationManager.AppSettings["OtherPerspectiveBackground"].ToString(),
                    ID = Convert.ToInt32(ConfigurationManager.AppSettings["OtherPerspectiveUniqueID"])
                });

                foreach (var pers in objTheme.Perspective)
                {
                    if (pers.ID != Convert.ToInt32(ConfigurationManager.AppSettings["OtherPerspectiveUniqueID"]))
                    {

                        pers.KPIs = Mapper.Map<List<KPIReleationKPIDTO>>(allKPIs.Where(a => a.StrategicObjective?.ThemeID == item.ID && a.PerspectiveID == pers.ID).ToList());
                    }
                    else
                    {
                        pers.KPIs = Mapper.Map<List<KPIReleationKPIDTO>>(allKPIs.Where(a => a.StrategicObjective?.ThemeID == item.ID && !a.PerspectiveID.HasValue).ToList());
                    }

                    foreach (var kpi in pers.KPIs)
                    {
                        kpi.KPIAffect = Mapper.Map<List<KPIAffectDTO>>(KPIAffects.Where(a => a.KPIID == kpi.ID).ToList());

                        if (kpi.Perspective == null)
                        {
                            kpi.Perspective = new CAPerspectiveDTO
                            {
                                ArabicName = "أخرى",
                                EnglishName = "Other",
                                BackgroundColor = ConfigurationManager.AppSettings["OtherPerspectiveBackground"].ToString(),
                                ID = Convert.ToInt32(ConfigurationManager.AppSettings["OtherPerspectiveUniqueID"])
                            };
                        }
                    }
                }
                dashboard.Add(objTheme);
            }
            return dashboard;
        }

        #endregion

        #region Privates
        private List<CADivisionalObjectiveDTO> ReadForDashboards(List<CADivisionalObjectiveDTO> DivisionalcObjectives, List<KPI> kpis, List<SystemPerformanceThreshold> SystemPerformances)
        {
            foreach (var item in DivisionalcObjectives)
            {

                var strategicObjectiveKPIs = kpis.Where(a => a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID == item.ID && a.KPIType.IsDepartmental == true).ToList();

                #region Performance

                decimal? StrategicObjectivePerformance = KPIBLL.CalculateKPIPerformace(strategicObjectiveKPIs);
                item.Performance = StrategicObjectivePerformance.HasValue ? StrategicObjectivePerformance.Value : 0;
                if (StrategicObjectivePerformance.HasValue)
                {
                    item.Status = CalculateStatus(StrategicObjectivePerformance.Value, SystemPerformances);
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
        private List<DistributionDataByStatus> GetKPIsByStatus(List<KPI> kpis)
        {
            List<DistributionDataByStatus> kPIByStatuses = kpis.GroupBy(g => new { g.Status })
            .Select(s => new DistributionDataByStatus
            {
                Status = s.Key.Status,
                Count = s.Count()
            }).ToList();
            return kPIByStatuses;
        }
        private List<DistributionDataByStatus> GetDivisionalObjectiveByStatus(List<CADivisionalObjectiveDTO> divisionalObjetive)
        {
            List<DistributionDataByStatus> itemsByStatuses = divisionalObjetive.GroupBy(g => new { g.Status })
            .Select(s => new DistributionDataByStatus
            {
                Status = s.Key.Status,
                Count = s.Count()
            }).ToList();
            return itemsByStatuses;
        }
        private List<DistributionDataByStatus> GetRequestByStatus(List<Request> requests)
        {
            List<DistributionDataByStatus> requestByStatuses = requests.GroupBy(g => new { g.Status })
            .Select(s => new DistributionDataByStatus
            {
                Status = s.Key.Status.ToString(),
                Count = s.Count()
            }).ToList();
            return requestByStatuses;
        }
        private string CalculateStatus(decimal Performance, List<SystemPerformanceThreshold> SystemPerformances)
        {
            string Status = string.Empty;
            Status = SystemPerformances.Where(w => w.Max == null || SystemPerformanceThresholdBLL.CalculateSystemOperation(Performance, (int)w.Max, true, w.MaxOperator))
                                    .Where(w => w.Min == null || SystemPerformanceThresholdBLL.CalculateSystemOperation(Performance, (int)w.Min, false, w.MinOperator))
                                    .Select(s => s.Code).FirstOrDefault();

            return Status;
        }
        #endregion
    }
}

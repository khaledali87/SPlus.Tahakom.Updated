using SPlus.BLL;
using SPlus.DTO;
using SPlus.Helper;
using SPlus.Model.Domain;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SPlus.UseCases
{
    public class BalanceScoreCardUseCases : LoggingUseCases
    {

        Container _Container = IOC.InitializeContainer();
        private readonly PerspectiveBLL PerspectiveBLL;
        private readonly KPIBLL KPIBLL;
        private readonly SystemPerformanceThresholdBLL SystemPerformanceThresholdBLL;
        private readonly OrgStructureBLL OrgStructureBLL;
        public BalanceScoreCardUseCases()
        {
            PerspectiveBLL = _Container.GetInstance<PerspectiveBLL>();
            KPIBLL = _Container.GetInstance<KPIBLL>();
            OrgStructureBLL = _Container.GetInstance<OrgStructureBLL>();
            SystemPerformanceThresholdBLL = _Container.GetInstance<SystemPerformanceThresholdBLL>();
        }

        #region Operational
        public OperationalBalanceScoreCardOrgStructureDTO GetOperationalBalanceScoreCard(string userName, int? Year)
        {
            OperationalBalanceScoreCardOrgStructureDTO OperationalBalanceScoreCard = new OperationalBalanceScoreCardOrgStructureDTO();

            #region Data Source

            List<SystemPerformanceThreshold> SystemPerformances = new List<SystemPerformanceThreshold>();
            List<OrgStructure> OrgStructures = new List<OrgStructure>();
            List<OrgStructure> OrgStructures_nopermission = new List<OrgStructure>();
            List<KPI> CalculationKPIs = new List<KPI>();
            List<KPI> KPIs = new List<KPI>();
            List<KPI> StrategicCalculationKPIs = new List<KPI>();
            List<KPI> AllStrategicKPIs = new List<KPI>();

            List<Task> Tasks = new List<Task>
            {
                Task.Run(() => OrgStructures = OrgStructureBLL.ReadOrgWithDivisional_new(userName,Year)),
                Task.Run(() => OrgStructures_nopermission = OrgStructureBLL.ReadOrgWithDivisional_noPermission(userName,Year)),
                
                Task.Run(() => SystemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold()),
                Task.Run(() => CalculationKPIs = KPIBLL.ReadOpertionalForCalculations(Year)),
                Task.Run(() => KPIs = KPIBLL.ReadOpertionalForBalanceScoreCards(userName,Year)),
               // Task.Run(() => StrategicCalculationKPIs = KPIBLL.ReadForCalculation(Year)),
                Task.Run(() => AllStrategicKPIs = KPIBLL.ReadForBalanceScoreCards(userName,Year))
            };
            Task.WaitAll(Tasks.ToArray());

            if (Year.HasValue)
            {
                OrgStructures = OrgStructures.Where(a => a.Years.Any(year => year == Year.Value.ToString())).ToList();
                KPIs = KPIs.Where(a => a.StartDate.Year == Year.Value).ToList();
                AllStrategicKPIs = AllStrategicKPIs.Where(a => a.StartDate.Year == Year.Value).ToList();
                CalculationKPIs = CalculationKPIs.Where(a => a.StartDate.Year == Year.Value).ToList();
                //StrategicCalculationKPIs = StrategicCalculationKPIs.Where(a => a.StartDate.Year == Year.Value).ToList();
            }
            #endregion

            #region Non Corporate OrgStructures

            foreach (var OrgStructure in OrgStructures.Where(a => a.ID != Constants.CorporateDepartmentID || a.ID != Constants.CorporateSectortID))
            {
                List<KPI> DepartmentCalculationKPI = new List<KPI>();
                List<KPI> DepartmentCalculationKPIPrevious = new List<KPI>();
                List<KPI> DepartmentKPIs = new List<KPI>();

                if (OrgStructure.DivisionalObjective != null && OrgStructure.DivisionalObjective.Any())
                {
                    foreach (var DivisionalObjective in OrgStructure.DivisionalObjective)
                    {
                        DivisionalObjective.KPIs = KPIs.Where(a => (a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == DivisionalObjective.ID)).ToList();
                        DepartmentKPIs.AddRange(KPIs.Where(a => (a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == DivisionalObjective.ID)).ToList());
                        DepartmentCalculationKPI.AddRange(CalculationKPIs.Where(a => (a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == DivisionalObjective.ID)).ToList());
                        DepartmentCalculationKPIPrevious.AddRange(CalculationKPIs.Where(a => (a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == DivisionalObjective.ID)).ToList());
                    }
                }

                DepartmentKPIs.AddRange(KPIs.Where(a => (!a.DivisionalObjectiveID.HasValue && a.OrgStructureID.HasValue && a.OrgStructureID.Value == OrgStructure.ID)).ToList());
                DepartmentCalculationKPI.AddRange(CalculationKPIs.Where(a => (!a.DivisionalObjectiveID.HasValue && a.OrgStructureID.HasValue && a.OrgStructureID.Value == OrgStructure.ID)).ToList());
                DepartmentCalculationKPIPrevious.AddRange(CalculationKPIs.Where(a => (!a.DivisionalObjectiveID.HasValue && a.OrgStructureID.HasValue && a.OrgStructureID.Value == OrgStructure.ID)).ToList());

                OrgStructure.KPIList = DepartmentKPIs;
                OrgStructure.KPIs = DepartmentKPIs;
                OrgStructure.KPIsCount = DepartmentKPIs.Count();
                decimal? DepartmentPerformance = KPIBLL.CalculateDepartmentalKPIPerformace(DepartmentCalculationKPI);
                OrgStructure.Performance = DepartmentPerformance.HasValue ? DepartmentPerformance.Value : 0;

                if (DepartmentPerformance.HasValue)
                {
                    OrgStructure.Status = CalculateStatus(DepartmentPerformance.Value, SystemPerformances);
                }
                else
                {
                    OrgStructure.Status = "NA";
                }

                decimal? DepartmentPerformance_Prev = KPIBLL.CalculateDepartmentalKPIPerformace_Prev(DepartmentCalculationKPIPrevious);
                OrgStructure.PreviousPerformance = DepartmentPerformance_Prev.HasValue ? DepartmentPerformance_Prev.Value : 0;

                if (DepartmentPerformance_Prev.HasValue)
                {
                    OrgStructure.PreviousStatus = CalculateStatus(DepartmentPerformance_Prev.Value, SystemPerformances);
                }
                else
                {
                    OrgStructure.PreviousStatus = "NA";
                }

                if (DepartmentPerformance_Prev.HasValue && DepartmentPerformance.HasValue)
                {
                    OrgStructure.Variance = OrgStructure.Performance - OrgStructure.PreviousPerformance;
                    OrgStructure.VariancePerformance = OrgStructure.Performance - OrgStructure.PreviousPerformance;


                    if (OrgStructure.VariancePerformance < 0)
                        OrgStructure.VarianceStatus = "OVD";
                    else if (OrgStructure.VariancePerformance == 0)
                        OrgStructure.VarianceStatus = "DLY";
                    else

                        OrgStructure.VarianceStatus = "OTR";

                    //OrgStructure.VarianceStatus = CalculateStatus(OrgStructure.Variance, SystemPerformances);
                }
                else
                {
                    OrgStructure.Variance = 0;
                    OrgStructure.VariancePerformance = 0;
                    OrgStructure.VarianceStatus = "NA";
                }

                decimal? AveragePerformance = KPIBLL.CalculateDepartmentalKPIPeriodicPerformace(DepartmentCalculationKPI);
                OrgStructure.AvragePerformance = AveragePerformance ?? 0;

                if (AveragePerformance.HasValue)
                {
                    OrgStructure.AvrageStatus = CalculateStatus(OrgStructure.AvragePerformance, SystemPerformances);
                }
                else
                    OrgStructure.AvrageStatus = "NA";


                OrgStructure.DepartmentsCount = OrgStructures.Where(d => d.ParentID.HasValue && d.ParentID.Value == OrgStructure.ID).Count();
            }
            List<OrgStructure> OrgStructuresTree = OrgStructureBLL.BuildTree(OrgStructures.Where(a => a.ID != Constants.CorporateDepartmentID && a.ID != Constants.CorporateSectortID).ToList());

            #region Map Sectors with DTO

            OperationalBalanceScoreCard.Sectors.AddRange(AutoMapper.Mapper.Map<List<OrgStructureCardDTO>>(OrgStructuresTree));

            #endregion

            #endregion



            //NO Permission

            #region Non Corporate OrgStructures

            foreach (var OrgStructure in OrgStructures_nopermission.Where(a => a.ID != Constants.CorporateDepartmentID || a.ID != Constants.CorporateSectortID))
            {
                List<KPI> DepartmentCalculationKPI = new List<KPI>();
                List<KPI> DepartmentCalculationKPIPrevious = new List<KPI>();
                List<KPI> DepartmentKPIs = new List<KPI>();

                if (OrgStructure.DivisionalObjective != null && OrgStructure.DivisionalObjective.Any())
                {
                    foreach (var DivisionalObjective in OrgStructure.DivisionalObjective)
                    {
                        DivisionalObjective.KPIs = KPIs.Where(a => (a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == DivisionalObjective.ID)).ToList();
                        DepartmentKPIs.AddRange(KPIs.Where(a => (a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == DivisionalObjective.ID)).ToList());
                        DepartmentCalculationKPI.AddRange(CalculationKPIs.Where(a => (a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == DivisionalObjective.ID)).ToList());
                        DepartmentCalculationKPIPrevious.AddRange(CalculationKPIs.Where(a => (a.DivisionalObjectiveID.HasValue && a.DivisionalObjectiveID.Value == DivisionalObjective.ID)).ToList());
                    }
                }

                DepartmentKPIs.AddRange(KPIs.Where(a => (!a.DivisionalObjectiveID.HasValue && a.OrgStructureID.HasValue && a.OrgStructureID.Value == OrgStructure.ID)).ToList());
                DepartmentCalculationKPI.AddRange(CalculationKPIs.Where(a => (!a.DivisionalObjectiveID.HasValue && a.OrgStructureID.HasValue && a.OrgStructureID.Value == OrgStructure.ID)).ToList());
                DepartmentCalculationKPIPrevious.AddRange(CalculationKPIs.Where(a => (!a.DivisionalObjectiveID.HasValue && a.OrgStructureID.HasValue && a.OrgStructureID.Value == OrgStructure.ID)).ToList());

                OrgStructure.KPIList = DepartmentKPIs;
                OrgStructure.KPIs = DepartmentKPIs;
                OrgStructure.KPIsCount = DepartmentKPIs.Count();
                decimal? DepartmentPerformance = KPIBLL.CalculateDepartmentalKPIPerformace(DepartmentCalculationKPI);
                OrgStructure.Performance = DepartmentPerformance.HasValue ? DepartmentPerformance.Value : 0;

                if (DepartmentPerformance.HasValue)
                {
                    OrgStructure.Status = CalculateStatus(DepartmentPerformance.Value, SystemPerformances);
                }
                else
                {
                    OrgStructure.Status = "NA";
                }

                decimal? DepartmentPerformance_Prev = KPIBLL.CalculateDepartmentalKPIPerformace_Prev(DepartmentCalculationKPIPrevious);
                OrgStructure.PreviousPerformance = DepartmentPerformance_Prev.HasValue ? DepartmentPerformance_Prev.Value : 0;

                if (DepartmentPerformance_Prev.HasValue)
                {
                    OrgStructure.PreviousStatus = CalculateStatus(DepartmentPerformance_Prev.Value, SystemPerformances);
                }
                else
                {
                    OrgStructure.PreviousStatus = "NA";
                }

                if (DepartmentPerformance_Prev.HasValue && DepartmentPerformance.HasValue)
                {
                    OrgStructure.Variance = OrgStructure.Performance - OrgStructure.PreviousPerformance;
                    OrgStructure.VariancePerformance = OrgStructure.Performance - OrgStructure.PreviousPerformance;

                    if (OrgStructure.VariancePerformance < 0)
                        OrgStructure.VarianceStatus = "OVD";
                    else if (OrgStructure.VariancePerformance == 0)
                        OrgStructure.VarianceStatus = "DLY";
                    else

                        OrgStructure.VarianceStatus = "OTR";
                    // OrgStructure.VarianceStatus = CalculateStatus(OrgStructure.Variance, SystemPerformances);
                }
                else
                {
                    OrgStructure.Variance = 0;
                    OrgStructure.VariancePerformance = 0;
                    OrgStructure.VarianceStatus = "NA";
                }

                decimal? AveragePerformance = KPIBLL.CalculateDepartmentalKPIPeriodicPerformace(DepartmentCalculationKPI);
                OrgStructure.AvragePerformance = AveragePerformance ?? 0;

                if (AveragePerformance.HasValue)
                {
                    OrgStructure.AvrageStatus = CalculateStatus(OrgStructure.AvragePerformance, SystemPerformances);
                }
                else
                    OrgStructure.AvrageStatus = "NA";


                OrgStructure.DepartmentsCount = OrgStructures_nopermission.Where(d => d.ParentID.HasValue && d.ParentID.Value == OrgStructure.ID).Count();
            }
            List<OrgStructure> OrgStructuresTree_noPermission = OrgStructureBLL.BuildTree(OrgStructures_nopermission.Where(a => a.ID != Constants.CorporateDepartmentID && a.ID != Constants.CorporateSectortID).ToList());

            #region Map Sectors with DTO

            OperationalBalanceScoreCard.Sectors_NoPermssion.AddRange(AutoMapper.Mapper.Map<List<OrgStructureCardDTO>>(OrgStructuresTree_noPermission));

            #endregion

            #endregion



            #region Corporate OrgStructure

            Func<KPI, bool> CorporateOrgStructureFilterPredicate = s =>
               (!s.DivisionalObjectiveID.HasValue && s.OrgStructureID.HasValue &&
                   (s.OrgStructureID.Value == Constants.CorporateDepartmentID ||
                   s.OrgStructureID.Value == Constants.CorporateSectortID)) ||
                   (s.DivisionalObjectiveID.HasValue && s.DivisionalObjective != null &&
                   (s.DivisionalObjective.OrgStructureId == Constants.CorporateDepartmentID ||
                   s.DivisionalObjective.OrgStructureId == Constants.CorporateSectortID));

            OrgStructureCardDTO Corporate = new OrgStructureCardDTO();
            OrgStructure CorporateOrgStructures = OrgStructures.Where(a => a.ID == Constants.CorporateDepartmentID || a.ID == Constants.CorporateSectortID).FirstOrDefault();
            Corporate.DepartmentsCount = OrgStructures.Where(d => d.ParentID.HasValue && d.ParentID.Value == Constants.CorporateSectortID).Count();
            Corporate.ArabicName = OrgStructures.Where(d => d.ParentID.HasValue && d.ParentID.Value == Constants.CorporateSectortID).FirstOrDefault()?.ArabicName;
            Corporate.EnglishName = OrgStructures.Where(d => d.ParentID.HasValue && d.ParentID.Value == Constants.CorporateSectortID).FirstOrDefault()?.EnglishName;
            var TmpCoparteKPIs = AllStrategicKPIs.Where(CorporateOrgStructureFilterPredicate).ToList();
            List<KPI> CorporateCalculationKPIs = new List<KPI>();
            List<KPI> CoparteCalculationKPIs_Previous = new List<KPI>();

            //AS 10-06-2025
            // CorporateCalculationKPIs.AddRange(StrategicCalculationKPIs.Where(CorporateOrgStructureFilterPredicate).ToList());
            //CoparteCalculationKPIs_Previous.AddRange(StrategicCalculationKPIs.Where(CorporateOrgStructureFilterPredicate).ToList());





             CorporateCalculationKPIs.AddRange(CalculationKPIs.Where(CorporateOrgStructureFilterPredicate).ToList());
            CoparteCalculationKPIs_Previous.AddRange(CalculationKPIs.Where(CorporateOrgStructureFilterPredicate).ToList());
            Corporate.KPIsCount = TmpCoparteKPIs.Count();
            Corporate.IsCorporate = true;

            decimal? CoporateDeprtmentPerformance = KPIBLL.CalculateDepartmentalKPIPerformace(CorporateCalculationKPIs);
            Corporate.Performance = CoporateDeprtmentPerformance.HasValue ? CoporateDeprtmentPerformance.Value : 0;

            if (CoporateDeprtmentPerformance.HasValue)
            {
                Corporate.Status = CalculateStatus(CoporateDeprtmentPerformance.Value, SystemPerformances);
            }
            else
            {
                Corporate.Status = "NA";
            }

            decimal? CoporateDeprtmentPerformance_Prev = KPIBLL.CalculateDepartmentalKPIPerformace_Prev(CoparteCalculationKPIs_Previous);
            var PreviousPerformance = CoporateDeprtmentPerformance_Prev.HasValue ? CoporateDeprtmentPerformance_Prev.Value : 0;

            if (CoporateDeprtmentPerformance_Prev.HasValue && CoporateDeprtmentPerformance.HasValue)
            {
                Corporate.VariancePerformance = Corporate.Performance - PreviousPerformance;

                if (Corporate.VariancePerformance < 0)
                    Corporate.VarianceStatus = "OVD";
                else if (Corporate.VariancePerformance == 0)
                    Corporate.VarianceStatus = "DLY";
                else

                    Corporate.VarianceStatus = "OTR";


               // Corporate.VarianceStatus = CalculateStatus(Corporate.VariancePerformance, SystemPerformances);
            }
            else
            {
                Corporate.VariancePerformance = 0;
                Corporate.VarianceStatus = "NA";
            }

            if (CoporateDeprtmentPerformance.HasValue)
            {
                KPIBLL.CalculateDepartmentalKPIPerformace(CorporateCalculationKPIs);
                decimal? Performance = KPIBLL.CalculateDepartmentalKPIPeriodicPerformace(CorporateCalculationKPIs);
                Corporate.AvragePerformance = Performance ?? 0;
                if (Performance.HasValue)
                {



                    Corporate.AvrageStatus = CalculateStatus(Corporate.AvragePerformance, SystemPerformances);
                }
                else
                    Corporate.AvrageStatus = "NA";
            }
            else
            {
                Corporate.AvragePerformance = 0;
                Corporate.AvrageStatus = "NA";
            }

            OperationalBalanceScoreCard.CorporateSectors = Corporate;

            #endregion

            #region Map Performances

            // Check if there are sectors with a status other than "NA" and if CorporateSectors is not null with a non-"NA" status
            if (OperationalBalanceScoreCard.Sectors_NoPermssion.SelectMany(s => s.Departments).Any(a => a.Status != "NA") || OperationalBalanceScoreCard.Sectors_NoPermssion.Any(a => a.Status != "NA") || (OperationalBalanceScoreCard.CorporateSectors != null && OperationalBalanceScoreCard.CorporateSectors.Status != "NA"))
            {
                // Calculate Performance
                decimal Performance = 0;
                if (OperationalBalanceScoreCard.Sectors_NoPermssion.SelectMany(s => s.Departments).Any(a => a.Status != "NA") || OperationalBalanceScoreCard.Sectors_NoPermssion.Any(a => a.Status != "NA") || (OperationalBalanceScoreCard.CorporateSectors != null && OperationalBalanceScoreCard.CorporateSectors.Status != "NA"))
                    Performance = (OperationalBalanceScoreCard.Sectors_NoPermssion.SelectMany(s => s.Departments).Sum(d => d.Performance) + OperationalBalanceScoreCard.Sectors_NoPermssion.Sum(d => d.Performance) + OperationalBalanceScoreCard.CorporateSectors.Performance) / (OperationalBalanceScoreCard.Sectors_NoPermssion.SelectMany(s => s.Departments).Count() + OperationalBalanceScoreCard.Sectors_NoPermssion.Count() + 1);

                OperationalBalanceScoreCard.Performance = Math.Round(Performance, 2);
                OperationalBalanceScoreCard.Status = CalculateStatus(Performance, SystemPerformances);

                // Calculate Average Performance
                decimal AveragePerformance = 0;
                if (OperationalBalanceScoreCard.Sectors_NoPermssion.SelectMany(s => s.Departments).Any(a => a.Status != "NA") || OperationalBalanceScoreCard.Sectors_NoPermssion.Any(a => a.Status != "NA") || (OperationalBalanceScoreCard.CorporateSectors != null && OperationalBalanceScoreCard.CorporateSectors.Status != "NA"))
                    AveragePerformance = (OperationalBalanceScoreCard.Sectors_NoPermssion.SelectMany(s => s.Departments).Sum(d => d.AvragePerformance) + OperationalBalanceScoreCard.Sectors_NoPermssion.Sum(d => d.AvragePerformance) + OperationalBalanceScoreCard.CorporateSectors.AvragePerformance) / (OperationalBalanceScoreCard.Sectors_NoPermssion.SelectMany(s => s.Departments).Count() + OperationalBalanceScoreCard.Sectors_NoPermssion.Count() + 1);

                OperationalBalanceScoreCard.AvragePerformance = Math.Round(AveragePerformance, 2);
                OperationalBalanceScoreCard.AvrageStatus = CalculateStatus(OperationalBalanceScoreCard.AvragePerformance, SystemPerformances);

                // Calculate Variance Performance
                decimal VariancePerformance = 0;
                if (OperationalBalanceScoreCard.Sectors_NoPermssion.Any(a => a.VarianceStatus != "NA"))
                    VariancePerformance = (OperationalBalanceScoreCard.Sectors_NoPermssion.SelectMany(s => s.Departments).Sum(d => d.Variance) + OperationalBalanceScoreCard.Sectors_NoPermssion.Sum(d => d.VariancePerformance) + OperationalBalanceScoreCard.CorporateSectors.VariancePerformance) / (OperationalBalanceScoreCard.Sectors_NoPermssion.SelectMany(s => s.Departments).Count() + OperationalBalanceScoreCard.Sectors_NoPermssion.Count() + 1);

                OperationalBalanceScoreCard.VariancePerformance = Math.Round(VariancePerformance, 2);

                if (OperationalBalanceScoreCard.VariancePerformance < 0)
                    OperationalBalanceScoreCard.VarianceStatus = "OVD";
                else if (OperationalBalanceScoreCard.VariancePerformance == 0)
                    OperationalBalanceScoreCard.VarianceStatus = "DLY";
                else

                    OperationalBalanceScoreCard.VarianceStatus = "OTR";

                //OperationalBalanceScoreCard.VarianceStatus = CalculateStatus(OperationalBalanceScoreCard.VariancePerformance, SystemPerformances);
            }
            //if (OperationalBalanceScoreCard.Sectors.SelectMany(s => s.Departments).Any(a => a.Status != "NA") || OperationalBalanceScoreCard.Sectors.Any(a => a.Status != "NA") || (OperationalBalanceScoreCard.CorporateSectors != null && OperationalBalanceScoreCard.CorporateSectors.Status != "NA"))
            //{
            //    // Calculate Performance
            //    decimal Performance = 0;
            //    if (OperationalBalanceScoreCard.Sectors.SelectMany(s => s.Departments).Any(a => a.Status != "NA") || OperationalBalanceScoreCard.Sectors.Any(a => a.Status != "NA") || (OperationalBalanceScoreCard.CorporateSectors != null && OperationalBalanceScoreCard.CorporateSectors.Status != "NA"))
            //        Performance = (OperationalBalanceScoreCard.Sectors.SelectMany(s => s.Departments).Sum(d => d.Performance) + OperationalBalanceScoreCard.Sectors.Sum(d => d.Performance) + OperationalBalanceScoreCard.CorporateSectors.Performance) / (OperationalBalanceScoreCard.Sectors.SelectMany(s => s.Departments).Count() + OperationalBalanceScoreCard.Sectors.Count() + 1);

            //    OperationalBalanceScoreCard.Performance = Math.Round(Performance, 2);
            //    OperationalBalanceScoreCard.Status = CalculateStatus(Performance, SystemPerformances);

            //    // Calculate Average Performance
            //    decimal AveragePerformance = 0;
            //    if (OperationalBalanceScoreCard.Sectors.SelectMany(s => s.Departments).Any(a => a.Status != "NA") || OperationalBalanceScoreCard.Sectors.Any(a => a.Status != "NA") || (OperationalBalanceScoreCard.CorporateSectors != null && OperationalBalanceScoreCard.CorporateSectors.Status != "NA"))
            //        AveragePerformance = (OperationalBalanceScoreCard.Sectors.SelectMany(s => s.Departments).Sum(d => d.AvragePerformance) + OperationalBalanceScoreCard.Sectors.Sum(d => d.AvragePerformance) + OperationalBalanceScoreCard.CorporateSectors.AvragePerformance) / (OperationalBalanceScoreCard.Sectors.SelectMany(s => s.Departments).Count() + OperationalBalanceScoreCard.Sectors.Count() + 1);

            //    OperationalBalanceScoreCard.AvragePerformance = Math.Round(AveragePerformance, 2);
            //    OperationalBalanceScoreCard.AvrageStatus = CalculateStatus(OperationalBalanceScoreCard.AvragePerformance, SystemPerformances);

            //    // Calculate Variance Performance
            //    decimal VariancePerformance = 0;
            //    if (OperationalBalanceScoreCard.Sectors.Any(a => a.VarianceStatus != "NA"))
            //        VariancePerformance = (OperationalBalanceScoreCard.Sectors.SelectMany(s => s.Departments).Sum(d => d.Variance) + OperationalBalanceScoreCard.Sectors.Sum(d => d.VariancePerformance) + OperationalBalanceScoreCard.CorporateSectors.VariancePerformance) / (OperationalBalanceScoreCard.Sectors.SelectMany(s => s.Departments).Count() + OperationalBalanceScoreCard.Sectors.Count() + 1);

            //    OperationalBalanceScoreCard.VariancePerformance = Math.Round(VariancePerformance, 2);
            //    OperationalBalanceScoreCard.VarianceStatus = CalculateStatus(OperationalBalanceScoreCard.VariancePerformance, SystemPerformances);
            //}
            else
            {
                // If conditions are not met, set everything to 0 and "NA"
                OperationalBalanceScoreCard.Performance = 0;
                OperationalBalanceScoreCard.Status = "NA";
                OperationalBalanceScoreCard.AvragePerformance = 0;
                OperationalBalanceScoreCard.AvrageStatus = "NA";
                OperationalBalanceScoreCard.VariancePerformance = 0;
                OperationalBalanceScoreCard.VarianceStatus = "NA";
            }


            #endregion

            return OperationalBalanceScoreCard;
        }


        #region Operational By ID

        public BalanceScoreDepratmentDetailsDTO ReadOperationalByDepartmentId(string userName, int id, List<int> months, int? Year)
        {
            BalanceScoreDepratmentDetailsDTO OperationalBalanceScoreCards = new BalanceScoreDepratmentDetailsDTO();
            OrgStructure orgStructure = new OrgStructure();
            List<SystemPerformanceThreshold> systemPerformances = new List<SystemPerformanceThreshold>();
            List<KPI> CalculationKPIs = new List<KPI>();
            List<KPI> AllKPIs = new List<KPI>();
            List<KPIHistory> AllKPIHistory = new List<KPIHistory>();
            List<Perspective> tmpKPIPerspective = new List<Perspective>();

            List<Task> tasks = new List<Task>
            {
                Task.Run(() => orgStructure = OrgStructureBLL.ReadByID_BlanceScoreDepartmentById(id,userName)),
                Task.Run(() => systemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold()),
                Task.Run(() => CalculationKPIs = KPIBLL.ReadOpertionalForCalculations(Year)),
                Task.Run(() => AllKPIs = KPIBLL.ReadOpertionalForBalanceScoreCards(userName,  Year,months,true)),
                Task.Run(() => AllKPIHistory = KPIBLL.ReadKPIHistory(Year))
            };
            Task.WaitAll(tasks.ToArray());

            if (Year.HasValue)
            {
                AllKPIs = AllKPIs.Where(a => a.StartDate.Year == Year.Value).ToList();
                CalculationKPIs = CalculationKPIs.Where(a => a.StartDate.Year == Year.Value).ToList();
                AllKPIHistory = AllKPIHistory.Where(a => a.HistoryDate.Year == Year.Value).ToList();
            }
            if (orgStructure != null)
            {
                var tmpDepartmentKPI = new List<KPI>();
                var tmpDepartmentKPIWithNotrim = new List<KPI>();

                if (orgStructure.DivisionalObjective != null && orgStructure.DivisionalObjective.Any())
                {
                    foreach (var d in orgStructure.DivisionalObjective)
                    {
                        d.KPIs = AllKPIs.Where(a => a.DivisionalObjectiveID == d.ID).ToList();
                        tmpDepartmentKPI.AddRange(d.KPIs);
                        tmpKPIPerspective.AddRange(d.KPIs.Where(a => a.Perspective != null).Select(a => a.Perspective));
                        tmpDepartmentKPIWithNotrim.AddRange(CalculationKPIs.Where(a => a.DivisionalObjectiveID == d.ID).ToList());
                    }
                }

                tmpDepartmentKPI.AddRange(AllKPIs.Where(a => !a.DivisionalObjectiveID.HasValue && a.OrgStructureID == orgStructure.ID));
                orgStructure.KPIList = orgStructure.KPIs = tmpDepartmentKPI;

                tmpKPIPerspective.AddRange(AllKPIs.Where(a => !a.DivisionalObjectiveID.HasValue && a.OrgStructureID == orgStructure.ID)
                                                  .Where(a => a.Perspective != null)
                                                  .Select(a => a.Perspective));

                tmpDepartmentKPIWithNotrim.AddRange(CalculationKPIs.Where(a => !a.DivisionalObjectiveID.HasValue && a.OrgStructureID == orgStructure.ID));
                tmpKPIPerspective = tmpKPIPerspective.Distinct().ToList();
                orgStructure.KPIsCount = tmpDepartmentKPI.Count();
                orgStructure.KPIs = tmpDepartmentKPI;

                decimal? departmentPerformance = KPIBLL.CalculateDepartmentalKPIPerformace(tmpDepartmentKPIWithNotrim);
                orgStructure.Performance = departmentPerformance ?? 0;

                orgStructure.Status = departmentPerformance.HasValue
                    ? CalculateStatus(departmentPerformance.Value, systemPerformances)
                    : "NA";
            }

            OperationalBalanceScoreCards = AutoMapper.Mapper.Map<BalanceScoreDepratmentDetailsDTO>(orgStructure);
            OperationalBalanceScoreCards.KPILastCumulative = new KPISpedoMeterChart();
            OperationalBalanceScoreCards.KPILastAvrage = new KPISpedoMeterChart();

            if (orgStructure.KPIs != null && orgStructure.KPIs.Any())
            {
                OperationalBalanceScoreCards.Perspectives = AutoMapper.Mapper.Map<List<OperationalDepartmentSectorDTO>>(tmpKPIPerspective);
                OperationalBalanceScoreCards.Perspectives.ForEach(a =>
                {
                    a.Weight = OperationalBalanceScoreCards.KPIs.Where(p => p.Perspective != null && p.Perspective.ID == a.ID).Sum(w => w.BusinessUnitWeight);
                    a.Performance = OperationalBalanceScoreCards.KPIs.Where(p => p.Perspective != null && p.Perspective.ID == a.ID).Any()
                        ? Math.Round(OperationalBalanceScoreCards.KPIs.Where(p => p.Perspective != null && p.Perspective.ID == a.ID)
                                                                  .Sum(w => w.AccumulutiveOutOfTarget) /
                                    OperationalBalanceScoreCards.KPIs.Where(p => p.Perspective != null && p.Perspective.ID == a.ID).Count(), 2)
                        : 0;
                });

                if (months != null && months.Any())
                    AllKPIHistory = AllKPIHistory.Where(a => orgStructure.KPIs.Any(k => k.ID == a.KPIID) && months.Contains(a.HistoryDate.Month)).ToList();
                else
                    AllKPIHistory = AllKPIHistory.Where(a => orgStructure.KPIs.Any(k => k.ID == a.KPIID)).ToList();

                var tmpKPIHistoryGroupBy = AllKPIHistory?.GroupBy(g => g.HistoryDate.Date).Select(grp => grp).ToList();

                OperationalBalanceScoreCards.AverageAchievement = CalculateAverageAchievement(tmpKPIHistoryGroupBy, systemPerformances);
                OperationalBalanceScoreCards.SumCumulativeAchievement = CalculateCumulativeAchievement(tmpKPIHistoryGroupBy, kpi => kpi.SumCumulativePerformance, systemPerformances);
                OperationalBalanceScoreCards.AverageCumulativeAchievement = CalculateCumulativeAchievement(tmpKPIHistoryGroupBy, kpi => kpi.AverageCumulativePerformance, systemPerformances);

                SetKPILastCumulative(OperationalBalanceScoreCards, systemPerformances);
                SetKPILastAvarage(OperationalBalanceScoreCards, systemPerformances);
            }
            else
            {
                SetDefaultValuesForEmptyKPIs(OperationalBalanceScoreCards);
            }

            return OperationalBalanceScoreCards;
        }

        #endregion

        #region Corporate

        public BalanceScoreDepratmentDetailsDTO ReadCorporate(string userName, CoparateSectorFilterDTO filter, int? Year)
        {
            BalanceScoreDepratmentDetailsDTO OperationalBalanceScoreCard = new BalanceScoreDepratmentDetailsDTO();

            List<SystemPerformanceThreshold> systemPerformances = new List<SystemPerformanceThreshold>();
            OrgStructure orgStructure = new OrgStructure();
            List<OrgStructure> OrgStructures = new List<OrgStructure>();
            List<KPI> CalculationKPIs = new List<KPI>();
            List<KPI> KPIs = new List<KPI>();
            List<KPI> FilteredKPIs = new List<KPI>();

            List<KPIHistory> AllKPIHistories = new List<KPIHistory>();

            List<Task> tasks = new List<Task>
            {
                Task.Run(() => orgStructure = OrgStructureBLL.ReadByID_BlanceScoreDepartmentById(Constants.CorporateDepartmentID, userName)),
                Task.Run(() => systemPerformances = SystemPerformanceThresholdBLL.SystemPerformanceThreshold()),
                Task.Run(() => CalculationKPIs = KPIBLL.ReadForCalculation(Year)),
                Task.Run(() => KPIs = KPIBLL.ReadForBalanceScoreCards(userName, Year,filter.Months)),
                Task.Run(() => AllKPIHistories = KPIBLL.ReadKPIHistory(Year))
            };
            Task.WaitAll(tasks.ToArray());

            if (Year.HasValue)
            {
                KPIs = KPIs.Where(a => a.StartDate.Year == Year.Value).ToList();
                CalculationKPIs = CalculationKPIs.Where(a => a.StartDate.Year == Year.Value).ToList();
            }
            List<Perspective> KPIPerspective = new List<Perspective>();

            Func<KPI, bool> CorporateOrgStructureFilterPredicate = a =>
                (!a.DivisionalObjectiveID.HasValue && a.OrgStructureID.HasValue &&
                    (a.OrgStructureID.Value == Constants.CorporateDepartmentID ||
                    a.OrgStructureID.Value == Constants.CorporateSectortID)) ||
                    (a.DivisionalObjectiveID.HasValue && a.DivisionalObjective != null &&
                    (a.DivisionalObjective.OrgStructureId == Constants.CorporateDepartmentID ||
                    a.DivisionalObjective.OrgStructureId == Constants.CorporateSectortID));

            List<KPI> NonFilteredKPIs = new List<KPI>();
            NonFilteredKPIs = KPIs.Where(CorporateOrgStructureFilterPredicate).ToList();

            if (filter.KPIs != null && filter.KPIs.Any())
                FilteredKPIs = KPIs.Where(a => filter.KPIs.Any(k => k == a.ID)).ToList();
            else
                FilteredKPIs = KPIs.ToList();

            if (orgStructure != null)
            {
                List<KPI> OrgStructureKPIs = new List<KPI>();
                List<KPI> OrgStructureCalculationKPIs = new List<KPI>();
                orgStructure.KPIList = FilteredKPIs.Where(CorporateOrgStructureFilterPredicate).ToList();

                OrgStructureKPIs.AddRange(orgStructure.KPIList);
                KPIPerspective.AddRange(orgStructure.KPIList.Where(a => a.Perspective != null).Select(a => a.Perspective));
                OrgStructureCalculationKPIs.AddRange(CalculationKPIs.Where(CorporateOrgStructureFilterPredicate).ToList());
                KPIPerspective = KPIPerspective.Distinct().ToList();
                orgStructure.KPIsCount = OrgStructureKPIs.Count();
                orgStructure.KPIList = OrgStructureKPIs;

                decimal? deprtmentPerformance = KPIBLL.CalculateDepartmentalKPIPerformace(OrgStructureCalculationKPIs);
                orgStructure.Performance = deprtmentPerformance.HasValue ? deprtmentPerformance.Value : 0;

                if (deprtmentPerformance.HasValue)
                {
                    orgStructure.Status = CalculateStatus(deprtmentPerformance.Value, systemPerformances);
                }
                else
                {
                    orgStructure.Status = "NA";
                }

                if (orgStructure.DivisionalObjective == null)
                    orgStructure.DivisionalObjective = new List<DivisionalObjective>();

                OperationalBalanceScoreCard = AutoMapper.Mapper.Map<BalanceScoreDepratmentDetailsDTO>(orgStructure);
                OperationalBalanceScoreCard.KPILastCumulative = new KPISpedoMeterChart();
                OperationalBalanceScoreCard.KPILastAvrage = new KPISpedoMeterChart();
                OperationalBalanceScoreCard.AllKPIs = AutoMapper.Mapper.Map<List<KPICardDepartmentListDTO>>(NonFilteredKPIs);
                OperationalBalanceScoreCard.KPIsCount = NonFilteredKPIs.Count();
                if (OperationalBalanceScoreCard.KPIs != null && OperationalBalanceScoreCard.KPIs.Any())
                {
                    OperationalBalanceScoreCard.Perspectives = AutoMapper.Mapper.Map<List<OperationalDepartmentSectorDTO>>(KPIPerspective);
                    OperationalBalanceScoreCard.Perspectives.ForEach(a =>
                    {
                        a.Weight = OperationalBalanceScoreCard.KPIs.Where(p => p.Perspective != null && p.Perspective.ID == a.ID).Sum(w => w.BusinessUnitWeight);
                        a.Performance = OperationalBalanceScoreCard.KPIs.Where(p => p.Perspective != null && p.Perspective.ID == a.ID).Any() ?
                        Math.Round(OperationalBalanceScoreCard.KPIs.Where(p => p.Perspective != null && p.Perspective.ID == a.ID).Sum(w => w.AccumulutiveOutOfTarget) /
                        OperationalBalanceScoreCard.KPIs.Where(p => p.Perspective != null && p.Perspective.ID == a.ID).Count(), 2) : 0;
                    });

                    if (filter.Months != null && filter.Months.Any())
                        AllKPIHistories = AllKPIHistories.Where(a => OperationalBalanceScoreCard.KPIs.Any(k => k.ID == a.KPIID) && filter.Months.Contains(a.HistoryDate.Month)).ToList();
                    else
                        AllKPIHistories = AllKPIHistories.Where(a => OperationalBalanceScoreCard.KPIs.Any(k => k.ID == a.KPIID)).ToList();

                    var GroupedKPIHistories = AllKPIHistories?.GroupBy(g => g.HistoryDate.Date).Select(group => group).ToList();
                    OperationalBalanceScoreCard.AverageAchievement = CalculateAverageAchievement(GroupedKPIHistories, systemPerformances);
                    OperationalBalanceScoreCard.SumCumulativeAchievement = CalculateCumulativeAchievement(GroupedKPIHistories, kpi => kpi.SumCumulativePerformance, systemPerformances);
                    OperationalBalanceScoreCard.AverageCumulativeAchievement = CalculateCumulativeAchievement(GroupedKPIHistories, kpi => kpi.AverageCumulativePerformance, systemPerformances);

                    SetKPILastCumulative(OperationalBalanceScoreCard, systemPerformances);
                    SetKPILastAvarage(OperationalBalanceScoreCard, systemPerformances);
                }
                else
                {
                    SetDefaultValuesForEmptyKPIs(OperationalBalanceScoreCard);
                }
            }

            return OperationalBalanceScoreCard;
        }

        #endregion

        #endregion

        #region Privates
        private string CalculateStatus(decimal Performance, List<SystemPerformanceThreshold> systemPerformances)
        {
            string Status = string.Empty;
            Status = systemPerformances
                                    .Where(w => w.Max == null || SystemPerformanceThresholdBLL.CalculateSystemOperation(Performance, (int)w.Max, true, w.MaxOperator))
                                    .Where(w => w.Min == null || SystemPerformanceThresholdBLL.CalculateSystemOperation(Performance, (int)w.Min, false, w.MinOperator))
                                    .Select(s => s.Code).FirstOrDefault();

            return Status;
        }


        private List<OperationalBalanceScoreCardKPIHistoryDTO> CalculateAverageAchievement(List<IGrouping<DateTime, KPIHistory>> GroupedKPIHistories, List<SystemPerformanceThreshold> systemPerformances)
        {
            return GroupedKPIHistories?.Select(item =>
            {
                var details = new OperationalBalanceScoreCardKPIHistoryDTO
                {
                    Date = item.Key.Date,
                    Performance = item.Sum(kpi => Math.Round(kpi.OutOfTarget * kpi.BusinessUnitWeight / 100, 2))
                };

                details.Status = CalculateStatus(details.Performance, systemPerformances);
                return details;
            }).ToList();
        }
        private List<OperationalBalanceScoreCardKPIHistoryDTO> CalculateCumulativeAchievement(List<IGrouping<DateTime, KPIHistory>> GroupedKPIHistories, Func<KPIHistory, decimal> selector, List<SystemPerformanceThreshold> systemPerformances)
        {
            return GroupedKPIHistories?.Select(item =>
            {
                var details = new OperationalBalanceScoreCardKPIHistoryDTO
                {
                    Date = item.Key.Date,
                    Performance = item.Sum(kpi => Math.Round(selector(kpi) * kpi.BusinessUnitWeight / 100, 2))
                };

                details.Status = CalculateStatus(details.Performance, systemPerformances);
                return details;
            }).ToList();
        }
        private void SetKPILastCumulative(BalanceScoreDepratmentDetailsDTO operationalBalanceScoreCard, List<SystemPerformanceThreshold> systemPerformances)
        {
            decimal OperationalKPIWeightSum = operationalBalanceScoreCard.KPIs.Where(a => a.Status != "NA").Select(a => a.BusinessUnitWeight).Sum();
            decimal CumulitaveOperationalKPIPerformance = operationalBalanceScoreCard.KPIs.Where(a => a.Status != "NA").Sum(s => s.AccumulutiveOutOfTarget * s.BusinessUnitWeight);

            if (operationalBalanceScoreCard.KPIs.Count(a => a.Status != "NA") > 0)
            {
                operationalBalanceScoreCard.KPILastCumulative.Perfromance = Math.Round(OperationalKPIWeightSum > 0 ? CumulitaveOperationalKPIPerformance / OperationalKPIWeightSum : 0, 2);
                operationalBalanceScoreCard.KPILastCumulative.Status = CalculateStatus(operationalBalanceScoreCard.KPILastCumulative.Perfromance, systemPerformances);
            }
            else
            {
                operationalBalanceScoreCard.KPILastCumulative.Perfromance = 0;
                operationalBalanceScoreCard.KPILastCumulative.Status = "NA";
            }
        }
        private void SetKPILastAvarage(BalanceScoreDepratmentDetailsDTO operationalBalanceScoreCard, List<SystemPerformanceThreshold> systemPerformances)
        {



            decimal OperationalKPIWeightSum = operationalBalanceScoreCard.KPIs.Where(a => a.Status != "NA").Select(a => a.BusinessUnitWeight).Sum();
            decimal CumulitaveOperationalKPIPerformance = operationalBalanceScoreCard.KPIs.Where(a => a.Status != "NA").Sum(s => s.OutOfTarget * s.BusinessUnitWeight);

            if (operationalBalanceScoreCard.KPIs.Count(a => a.Status != "NA") > 0)
            {
                operationalBalanceScoreCard.KPILastAvrage.Perfromance = Math.Round(OperationalKPIWeightSum > 0 ? CumulitaveOperationalKPIPerformance / OperationalKPIWeightSum : 0, 2);
                operationalBalanceScoreCard.KPILastAvrage.Status = CalculateStatus(operationalBalanceScoreCard.KPILastAvrage.Perfromance, systemPerformances);
            }
            else
            {
                operationalBalanceScoreCard.KPILastAvrage.Perfromance = 0;
                operationalBalanceScoreCard.KPILastAvrage.Status = "NA";
            }

            //decimal AverageOperationalKPIPerformance = operationalBalanceScoreCard.KPIs.Sum(s => s.OutOfTarget * s.BusinessUnitWeight);
            //decimal OperationalKPIWeightSum = operationalBalanceScoreCard.KPIs.Select(a => a.BusinessUnitWeight).Sum();

            //if (OperationalKPIWeightSum > 0)
            //{
            //    operationalBalanceScoreCard.KPILastAvrage.Perfromance = Math.Round(AverageOperationalKPIPerformance / OperationalKPIWeightSum, 2);
            //    operationalBalanceScoreCard.KPILastAvrage.Status = CalculateStatus(operationalBalanceScoreCard.KPILastAvrage.Perfromance, systemPerformances);
            //}
            //else
            //{
            //    operationalBalanceScoreCard.KPILastAvrage.Perfromance = 0;
            //    operationalBalanceScoreCard.KPILastAvrage.Status = "NA";
            //}
        }
        private void SetDefaultValuesForEmptyKPIs(BalanceScoreDepratmentDetailsDTO operationalBalanceScoreCard)
        {
            operationalBalanceScoreCard.KPILastCumulative.Perfromance = 0;
            operationalBalanceScoreCard.KPILastCumulative.Status = "NA";
            operationalBalanceScoreCard.KPILastAvrage.Perfromance = 0;
            operationalBalanceScoreCard.KPILastAvrage.Status = "NA";
        }
        #endregion

    }
}

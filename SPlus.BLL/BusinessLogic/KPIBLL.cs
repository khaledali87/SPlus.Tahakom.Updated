using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using SPlus.DataAccess;
using SPlus.Model.Domain;
using SPlus.Helper;
using System.Data.Entity;
using System.Reflection;
using model = SPlus.Model;
using SPlus.Model;
using Z.EntityFramework.Plus;
using SPlus.DTO;
using System.Configuration;
using Dangl.Calculator;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

namespace SPlus.BLL
{
    public class KPIBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();


        private readonly IUnitOfWorkFactory _factory;
        public KPIBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }

        #region Main




        #endregion

        #region Create
        public KPI Create(KPI kpi, string userName)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {

                kpi.Created = DateTime.Now;
                kpi.Modified = DateTime.Now;

                decimal? SummedTargets = 0;
                decimal? AverageTargets = 0;
                int PeriodCount = 0;
                foreach (var measure in kpi.KPIMeasures)
                {
                    if (measure.Status == null)
                        measure.Status = "NA";
                    if (measure.AccumulutiveStatus == null)
                        measure.AccumulutiveStatus = "NA";
                    measure.Created = DateTime.Now;
                    measure.Modified = DateTime.Now;
                    measure.CalculationMethod = kpi.CalculationMethod;
                    SummedTargets += measure.Target;
                    PeriodCount++;
                    AverageTargets = SummedTargets / PeriodCount;
                    //if (measure.Target == kpi.Baseline || SummedTargets == kpi.Baseline || AverageTargets == kpi.Baseline)
                    //{
                    //    throw new System.Exception("TargetEqualBaseline");
                    //}
                }
                if (kpi.Parameters != null && kpi.Parameters.Count > 0 && (kpi.DataSource == "semi-auto" || kpi.DataSource == "auto"))
                {
                    foreach (var param in kpi.Parameters)
                    {
                        param.IsConstant = param.IsConstant;
                        param.ConstantValue = param.ConstantValue;
                        param.ParamId = param.ParameterName;
                        param.Created = DateTime.Now;
                        param.Modified = DateTime.Now;
                    }
                }

                dataAccess.KPI.Save(kpi);
                result = dataAccess.Complete();

                ParameterValue parameterValue = null;
                foreach (KPIMeasure _KPIMeasure in kpi.KPIMeasures)
                {
                    foreach (Parameter Parameter in kpi.Parameters)
                    {
                        parameterValue = new ParameterValue();
                        parameterValue.ParameterID = Parameter.ID;
                        parameterValue.Title = Parameter.ParameterName;
                        parameterValue.Value = Parameter.Value;
                        parameterValue.MeasureID = _KPIMeasure.ID;
                        parameterValue.Created = DateTime.Now;
                        parameterValue.Modified = DateTime.Now;
                        dataAccess.ParameterValue.Save(parameterValue);
                    }
                }
                result = dataAccess.Complete();

                if (kpi.Attachments != null && kpi.Attachments.Count() > 0)
                {
                    IEnumerable<Attachment> SourceAttachments = dataAccess.Attachment.Query().ToList();
                    List<Attachment> Attachments = new List<Attachment>();
                    foreach (var Attachment in kpi.Attachments)
                    {
                        Attachment attach = new Attachment();
                        attach = SourceAttachments.Where(w => w.ID == Attachment.ID).FirstOrDefault();
                        attach.RelatedItemID = kpi.ID;
                        attach.Type = typeof(KPI).Name;
                        if (attach != null)
                            dataAccess.Attachment.Save(attach, true);
                    }
                }
                result = dataAccess.Complete();

                dataAccess.Dispose();
                return ReadByID(kpi.ID, userName);
            }
        }
        public KPIComment CreateComment(KPIComment kpiComment, string userName)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                kpiComment.Created = DateTime.Now;
                kpiComment.Modified = DateTime.Now;
                kpiComment.CreatedBy = userName;
                dataAccess.KPIComment.Save(kpiComment);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return kpiComment;
            }
        }



        public List<KPIAffect> ConnectWithAffect(List<KPIAffect> _KPIAffect, bool Delete)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {

                if (Delete == true)
                {
                    foreach (var item in _KPIAffect)
                    {
                        //KPIAffect KPIAffect = dataAccess.KPIAffect.Get(item.kpi);
                        //if (KPIAffect != null)
                        //{
                        //    dataAccess.KPIAffect.Delete(KPIAffect);
                        //    result = dataAccess.Complete();
                        //    dataAccess.Dispose();
                        //}
                        DeleteAffectByKPIID(item.KPIID);
                    }
                }


                foreach (var item in _KPIAffect)
                {
                    if (item.Affected == 0 && item.Effecting == 0)
                    {
                        return _KPIAffect;
                    }
                    else
                        dataAccess.KPIAffect.Save(item);
                }

                result = dataAccess.Complete();
                dataAccess.Dispose();
                return _KPIAffect;
            }

        }



        public void DeleteAffectByKPIID(int kpiID)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                List<KPIAffect> KPIAffect = dataAccess.KPIAffect.Query().Where(a => a.KPIID == kpiID).ToList();



                foreach (var item in KPIAffect)
                {

                    dataAccess.KPIAffect.Delete(item);
                    result = dataAccess.Complete();

                }

                dataAccess.Dispose();



            }

        }
        public List<KPIAffect> ReadWithAffect()
        {

            using (var dataAccess = _factory.Create())
            {

                IEnumerable<KPIAffect> affects = dataAccess.KPIAffect.Query();


                return affects.ToList();

            }

        }

        #endregion

        #region Read


        public List<KPIHistory> ReadKPIHistory(int? year)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPIHistory> kpiHisories = dataAccess.KPIHistory.Query(a => !year.HasValue || a.HistoryDate.Year == year.Value);
                return kpiHisories.ToList();
            }
        }

        public List<KPI> TaskCenterKPIs(string username)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPI> kpis = dataAccess.KPI.Query()
                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.ChampionModel)
                    .IncludeOptimized(a => a.OwnerModel)
                    .IncludeOptimized(a => a.Parameters)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimizedByPath("KPIType.Workflows")
                    .IncludeOptimizedByPath("KPIType.Workflows.WorkflowSteps")
                    .IncludeOptimizedByPath("KPIType.Workflows.BaseWorkflow")
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimizedByPath("DivisionalObjective.OrgStructure")
                    .SecureListObj(dataAccess, username).Cast<KPI>().ToList();

                //  kpis.MapAttachment(dataAccess);
                foreach (var kpi in kpis)
                {
                    kpi.KPIMeasures = SetActiveMeasure(kpi.KPIMeasures.ToList());
                }
                kpis = SetCanUpdate(kpis, username);
                return kpis.ToList();
            }
        }

        public List<KPI> Read(string username, int? Year)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPI> kpis = dataAccess.KPI.Query(a => !Year.HasValue || a.StartDate.Year == Year.Value)

                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimized(a => a.StrategicObjective)
                    .IncludeOptimized(a => a.ChampionModel)
                    .IncludeOptimized(a => a.OwnerModel)

                    .SecureListObj(dataAccess, username).Cast<KPI>().ToList();

                foreach (var item in kpis)
                {
                    item.KPIMeasures = item.KPIMeasures.Where(x => x.HasNoTarget != true).ToList();
                }

                MapKPIProperties(kpis.ToList(), false);

                
                return kpis.ToList();
            }
        }
        public List<KPI> Read_LessData(string username, int? Year)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPI> kpis = dataAccess.KPI.Query(a => !Year.HasValue || a.StartDate.Year == Year.Value)

                    .SecureListObj(dataAccess, username).Cast<KPI>().ToList();

                return kpis.ToList();
            }
        }
        public List<KPI> Read(int? Year)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPI> kpis = dataAccess.KPI.Query(a => !Year.HasValue || a.StartDate.Year == Year.Value)
                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimized(a => a.StrategicObjective)
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimized(a => a.ChampionModel)
                    .IncludeOptimized(a => a.OwnerModel)
                  .ToList();


                MapKPIProperties(kpis.ToList(), false);

                return kpis.ToList();
            }
        }
        public List<KPI> ReadMitigationActionKpis(int? year)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPI> kpis = dataAccess.KPI.Query(a => !year.HasValue || a.StartDate.Year == year.Value)
                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimized(a => a.StrategicObjective)
                    .IncludeOptimized(a => a.ChampionModel)
                    .IncludeOptimized(a => a.OwnerModel)
                     .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimizedByPath("DivisionalObjective.OrgStructure")
               .ToList();
                //.SecureListObj(dataAccess, username).Cast<KPI>().ToList();

                MapKPIProperties(kpis.ToList(), false);

                foreach (KPI kpi in kpis)
                {
                    if (kpi.DivisionalObjective != null)
                        kpi.OrgStructureID = kpi.DivisionalObjective.OrgStructureId;
                    else if (kpi.OrgStructure != null)
                        kpi.OrgStructureID = kpi.OrgStructure.ID;

                }
                return kpis.ToList();
            }
        }

        public List<KPI> ReadReportKPIs(string username, int? year)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPI> kpis = dataAccess.KPI.Query(a => !year.HasValue || a.StartDate.Year == year.Value)
                    .IncludeOptimized(a => a.KPIComments)
                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimizedByPath("KPIType.KPIThresholds")
                    .IncludeOptimizedByPath("KPIType.KPIThresholds.Status")
                    .IncludeOptimizedByPath("KPIType.Workflows")
                    .IncludeOptimizedByPath("KPIType.Workflows.BaseWorkflow")
                    .IncludeOptimized(a => a.StrategicObjective)
                    .IncludeOptimizedByPath("StrategicObjective.Theme")
                    .IncludeOptimizedByPath("StrategicObjective.Theme.Strategy")
                    .IncludeOptimized(a => a.Perspective)
                    .IncludeOptimizedByPath("Perspective.Strategy")
                    .IncludeOptimized(a => a.Parameters)
                    .IncludeOptimized(a => a.ChampionModel)
                    .IncludeOptimizedByPath("ChampionModel.UsersGroups")
                    .IncludeOptimizedByPath("ChampionModel.UsersGroups.Group")
                    .IncludeOptimized(a => a.OwnerModel)
                    .IncludeOptimizedByPath("OwnerModel.UsersGroups")
                    .IncludeOptimizedByPath("OwnerModel.UsersGroups.Group")
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimizedByPath("DivisionalObjective.OrgStructure")
                    .IncludeOptimized(a => a.OrgStructure)
                    .ToList();



                if (kpis != null)
                    kpis = kpis.SecureListObj(dataAccess, username).Cast<KPI>().ToList();

                
                    foreach (var item in kpis)
                    {
                        item.KPIMeasures = item.KPIMeasures.Where(x => x.HasNoTarget != true).ToList();
                    }

                MapKPIProperties(kpis.ToList());

                return kpis.ToList();
            }
        }

        public List<KPI> ReadReportKPIsAllMeasures(string username, int? year)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPI> kpis = dataAccess.KPI.Query(a => !year.HasValue || a.StartDate.Year == year.Value)
                    .IncludeOptimized(a => a.KPIComments)
                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimizedByPath("KPIType.KPIThresholds")
                    .IncludeOptimizedByPath("KPIType.KPIThresholds.Status")
                    .IncludeOptimizedByPath("KPIType.Workflows")
                    .IncludeOptimizedByPath("KPIType.Workflows.BaseWorkflow")
                    .IncludeOptimized(a => a.StrategicObjective)
                    .IncludeOptimizedByPath("StrategicObjective.Theme")
                    .IncludeOptimizedByPath("StrategicObjective.Theme.Strategy")
                    .IncludeOptimized(a => a.Perspective)
                    .IncludeOptimizedByPath("Perspective.Strategy")
                    .IncludeOptimized(a => a.Parameters)
                    .IncludeOptimized(a => a.ChampionModel)
                    .IncludeOptimizedByPath("ChampionModel.UsersGroups")
                    .IncludeOptimizedByPath("ChampionModel.UsersGroups.Group")
                    .IncludeOptimized(a => a.OwnerModel)
                    .IncludeOptimizedByPath("OwnerModel.UsersGroups")
                    .IncludeOptimizedByPath("OwnerModel.UsersGroups.Group")
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimizedByPath("DivisionalObjective.OrgStructure")
                    .IncludeOptimized(a => a.OrgStructure)
                    .ToList();



                if (kpis != null)
                    kpis = kpis.SecureListObj(dataAccess, username).Cast<KPI>().ToList();



                MapKPIPropertiesAdmin(kpis.ToList());

                return kpis.ToList();
            }
        }


        public List<KPI> ReadDashboardKPIs(string username, int? year)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPI> kpis = dataAccess.KPI.Query(a => !year.HasValue || a.StartDate.Year == year.Value)
                    .IncludeOptimized(a => a.KPIComments)
                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimizedByPath("KPIType.KPIThresholds")
                    .IncludeOptimizedByPath("KPIType.KPIThresholds.Status")
                    .IncludeOptimizedByPath("KPIType.Workflows")
                    .IncludeOptimizedByPath("KPIType.Workflows.BaseWorkflow")
                    .IncludeOptimized(a => a.StrategicObjective)
                    .IncludeOptimizedByPath("StrategicObjective.Theme")
                    .IncludeOptimizedByPath("StrategicObjective.Theme.Strategy")
                    .IncludeOptimized(a => a.Perspective)
                    .IncludeOptimizedByPath("Perspective.Strategy")
                    .IncludeOptimized(a => a.Parameters)
                    .IncludeOptimized(a => a.ChampionModel)
                    .IncludeOptimizedByPath("ChampionModel.UsersGroups")
                    .IncludeOptimizedByPath("ChampionModel.UsersGroups.Group")
                    .IncludeOptimized(a => a.OwnerModel)
                    .IncludeOptimizedByPath("OwnerModel.UsersGroups")
                    .IncludeOptimizedByPath("OwnerModel.UsersGroups.Group")
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimizedByPath("DivisionalObjective.OrgStructure")
                    .IncludeOptimized(a => a.OrgStructure)
                    .ToList();

                if (kpis != null)
                    kpis = kpis.SecureListObj(dataAccess, username).Cast<KPI>().ToList();

                foreach (var item in kpis)
                {
                    item.KPIMeasures = item.KPIMeasures.Where(x => x.HasNoTarget != true).ToList();
                }

                MapKPIProperties(kpis.ToList(), false);
                return kpis.ToList();
            }
        }

        public List<KPI> ReadForOperationalStrategy(string username)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPI> kpis = dataAccess.KPI.Query()
                    .IncludeOptimized(a => a.KPIMeasures)
                    .SecureListObj(dataAccess, username).Cast<KPI>().ToList();

                foreach (var item in kpis)
                {
                    item.KPIMeasures = item.KPIMeasures.Where(x => x.HasNoTarget != true).ToList();
                }

                MapKPIProperties(kpis.ToList(), false);
                return kpis.ToList();
            }
        }

        public List<KPI> ReadForBalanceScoreCards(string username, int? year, List<int> months = null)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPI> kpis = dataAccess.KPI.Query(a => !year.HasValue || a.StartDate.Year == year.Value)
                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.ChampionModel)
                    .IncludeOptimized(a => a.OwnerModel)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimized(a => a.Perspective)
                    .IncludeOptimizedByPath("KPIType.KPIThresholds")
                    .IncludeOptimizedByPath("KPIType.KPIThresholds.Status")
                     .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimizedByPath("DivisionalObjective.OrgStructure");
                List<KPI> finalKPIs = new List<KPI>();

                foreach (var item in kpis)
                {
                    item.KPIMeasures = item.KPIMeasures.Where(x => x.HasNoTarget != true).ToList();
                }

                MapKPIProperties(kpis.ToList(), true);

                foreach (KPI kpi in kpis)
                {
                    if (months != null && months.Count > 0)
                    {
                        var kpiMeasures = kpi.KPIMeasures.Where(w => w.DueDate.Year == DateTime.Now.Year && months.Contains(w.DueDate.Month));
                        //kpi.KPIMeasures.Where(w => w.DueDate.Year == DateTime.Now.Year).Where(w => months.Contains(w.DueDate.Month));

                        kpi.KPIMeasures = kpiMeasures.ToList();

                    }
                    if (kpi.KPIMeasures.Count() > 0)
                        finalKPIs.Add(kpi);
                }
                return finalKPIs.SecureListObj(dataAccess, username).Cast<KPI>().ToList();
                //kpis.SecureListObj(dataAccess, username).Cast<KPI>().ToList();
            }
        }

        public List<KPI> ReadForCalculation(int? Year)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPI> kpis = dataAccess.KPI.Query(a => !Year.HasValue || a.StartDate.Year == Year.Value)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.StrategicObjective)
                    .IncludeOptimizedByPath("StrategicObjective.Theme")
                       .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimizedByPath("DivisionalObjective.OrgStructure");
                foreach (var item in kpis)
                {
                    item.KPIMeasures = item.KPIMeasures.Where(x => x.HasNoTarget != true).ToList();
                }
                MapKPIProperties(kpis.ToList(), false);
                return kpis.ToList();
            }
        }

        public List<KPI> ReadOpertionalForCalculations(int? Year)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPI> kpi = dataAccess.KPI.Query(a => !Year.HasValue || a.StartDate.Year == Year.Value)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.StrategicObjective)
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimizedByPath("DivisionalObjective.StrategicObjective").ToList();
                // .Where(a => a.DivisionalObjectiveID != null);

                foreach (var item in kpi)
                {
                    item.KPIMeasures = item.KPIMeasures.Where(x => x.HasNoTarget != true).ToList();
                }

                return kpi.ToList();
            }
        }

        public List<KPI> ReadOpertionalForBalanceScoreCards(string username, int? Year, List<int> months = null, bool readCode = false)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPI> kpis = dataAccess.KPI.Query(a => !Year.HasValue || a.StartDate.Year == Year.Value)
                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.ChampionModel)
                    .IncludeOptimized(a => a.OwnerModel)
                    .IncludeOptimized(a => a.StrategicObjective)
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimizedByPath("DivisionalObjective.StrategicObjective")
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimizedByPath("KPIType.KPIThresholds")
                    .IncludeOptimizedByPath("KPIType.KPIThresholds.Status")
                    .IncludeOptimized(a => a.Perspective)
                    .IncludeOptimizedByPath("Perspective.Strategy")
                    // .Where(a => a.DivisionalObjectiveID != null)
                    .SecureListObj(dataAccess, username).Cast<KPI>()
                    .ToList();

                List<KPI> finalKPIs = new List<KPI>();

                foreach (KPI kpi in kpis)
                {
                    kpi.KPIMeasures = kpi.KPIMeasures.Where(x => x.HasNoTarget != true).ToList();


                    if (months != null && months.Count > 0)
                    {
                        var kpiMeasures = kpi.KPIMeasures.Where(w => Year.HasValue ? w.DueDate.Year == Year.Value : w.DueDate.Year == DateTime.Now.Year && months.Contains(w.DueDate.Month));
                        //kpi.KPIMeasures.Where(w => w.DueDate.Year == DateTime.Now.Year).Where(w => months.Contains(w.DueDate.Month));

                        kpi.KPIMeasures = kpiMeasures.ToList();

                    }
                    if (kpi.Perspective != null)
                        kpi.Perspective = kpi.Perspective.MapAttachment(dataAccess);

                    if (kpi.KPIMeasures.Count() > 0)
                        finalKPIs.Add(kpi);
                }
                MapKPIProperties(finalKPIs.ToList(), readCode);

                //  MapKPIProperties(finalKPIs.ToList());
                return finalKPIs;//.SecureListObj(dataAccess, username).Cast<KPI>().ToList();

            }
        }
        //public List<KPI> ReadOpertionalForBalanceScoreCardsByMonths(string username, List<int> months)
        //{
        //    using (var dataAccess = _factory.Create())
        //    {
        //        IEnumerable<KPI> kpis = dataAccess.KPI.Query()
        //            .IncludeOptimized(a => a.KPIMeasures)
        //            .IncludeOptimized(a => a.ChampionModel)
        //            .IncludeOptimized(a => a.OwnerModel)
        //            .IncludeOptimized(a => a.DivisionalObjective)
        //            .IncludeOptimized(a => a.KPIType)
        //            .IncludeOptimizedByPath("KPIType.KPIThresholds")
        //            .IncludeOptimizedByPath("KPIType.KPIThresholds.Status")
        //            .IncludeOptimized(a => a.Perspective)
        //            .IncludeOptimizedByPath("Perspective.Strategy")
        //            .Where(a => a.DivisionalObjectiveID != null)
        //            .ToList();


        //        foreach (KPI kpi in kpis)
        //        {
        //            if (months != null && months.Count > 0)
        //            {
        //                var kpiMeasures = kpi.KPIMeasures.Where(w => months.Contains(w.DueDate.Month));
        //                if (kpiMeasures != null && kpiMeasures.Count() > 0)
        //                {
        //                    kpi.KPIMeasures = kpiMeasures.ToList();
        //                }
        //            }

        //            MapKPIProperties(kpi);
        //            if (kpi.Perspective != null)
        //                kpi.Perspective = kpi.Perspective.MapAttachment(dataAccess);

        //        }

        //        return kpis.SecureListObj(dataAccess, username).Cast<KPI>().ToList();

        //    }
        //}

        public KPI ReadByID(int id, string username)
        {
            using (var dataAccess = _factory.Create())
            {
                KPI kpi = dataAccess.KPI.Query()
                    .IncludeOptimized(a => a.KPIComments)
                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimizedByPath("KPIType.KPIThresholds")
                    .IncludeOptimizedByPath("KPIType.KPIThresholds.Status")
                    .IncludeOptimizedByPath("KPIType.Workflows")
                    .IncludeOptimizedByPath("KPIType.Workflows.BaseWorkflow")
                    .IncludeOptimized(a => a.StrategicObjective)
                    .IncludeOptimizedByPath("StrategicObjective.Theme")
                    .IncludeOptimizedByPath("StrategicObjective.Theme.Strategy")
                    .IncludeOptimized(a => a.Perspective)
                    .IncludeOptimizedByPath("Perspective.Strategy")
                    .IncludeOptimized(a => a.Parameters)
                    .IncludeOptimized(a => a.ChampionModel)
                    .IncludeOptimizedByPath("ChampionModel.UsersGroups")
                    .IncludeOptimizedByPath("ChampionModel.UsersGroups.Group")
                    .IncludeOptimized(a => a.OwnerModel)
                    .IncludeOptimizedByPath("OwnerModel.UsersGroups")
                    .IncludeOptimizedByPath("OwnerModel.UsersGroups.Group")
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimizedByPath("DivisionalObjective.OrgStructure")
                    .IncludeOptimized(a => a.OrgStructure)
                    .Where(s => s.ID == id).FirstOrDefault();

                kpi.KPIMeasures = kpi.KPIMeasures.Where(x => x.HasNoTarget != true).ToList();


                if (kpi != null)
                    kpi = (KPI)kpi.SecureObj(dataAccess, username);

                if (kpi != null)
                {
                    if (kpi.DivisionalObjective != null)
                        kpi.OrgStructureID = kpi.DivisionalObjective.OrgStructureId;
                    else if (kpi.OrgStructure != null)
                        kpi.OrgStructureID = kpi.OrgStructure.ID;


                    kpi.IsDeletable = true;
                    kpi = SetCanUpdate(kpi, username);

                    List<KPI> KPIs = ReadAll();
                    List<OrgStructure> OrgStructures = GetOrgStructures();
                    List<DivisionalObjective> DivisionalObjective = GetDivisionalObjectives();

                    MapKPIProperties(kpi, KPIs, OrgStructures, DivisionalObjective);
                    kpi.KPIType.MapAttachment(dataAccess);
                    if (kpi.Perspective != null)
                        kpi.Perspective.MapAttachment(dataAccess);
                    return kpi.MapAttachment(dataAccess);
                }
                return null;
            }
        }

        public KPI ReadByIDAdmin(int id, string username)
        {
            using (var dataAccess = _factory.Create())
            {
                KPI kpi = dataAccess.KPI.Query()
                    .IncludeOptimized(a => a.KPIComments)
                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimizedByPath("KPIType.KPIThresholds")
                    .IncludeOptimizedByPath("KPIType.KPIThresholds.Status")
                    .IncludeOptimizedByPath("KPIType.Workflows")
                    .IncludeOptimizedByPath("KPIType.Workflows.BaseWorkflow")
                    .IncludeOptimized(a => a.StrategicObjective)
                    .IncludeOptimizedByPath("StrategicObjective.Theme")
                    .IncludeOptimizedByPath("StrategicObjective.Theme.Strategy")
                    .IncludeOptimized(a => a.Perspective)
                    .IncludeOptimizedByPath("Perspective.Strategy")
                    .IncludeOptimized(a => a.Parameters)
                    .IncludeOptimized(a => a.ChampionModel)
                    .IncludeOptimizedByPath("ChampionModel.UsersGroups")
                    .IncludeOptimizedByPath("ChampionModel.UsersGroups.Group")
                    .IncludeOptimized(a => a.OwnerModel)
                    .IncludeOptimizedByPath("OwnerModel.UsersGroups")
                    .IncludeOptimizedByPath("OwnerModel.UsersGroups.Group")
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimizedByPath("DivisionalObjective.OrgStructure")
                    .IncludeOptimized(a => a.OrgStructure)
                    .Where(s => s.ID == id).FirstOrDefault();



                if (kpi != null)
                    kpi = (KPI)kpi.SecureObj(dataAccess, username);

                if (kpi != null)
                {
                    if (kpi.DivisionalObjective != null)
                        kpi.OrgStructureID = kpi.DivisionalObjective.OrgStructureId;
                    else if (kpi.OrgStructure != null)
                        kpi.OrgStructureID = kpi.OrgStructure.ID;


                    kpi.IsDeletable = true;
                    kpi = SetCanUpdate(kpi, username);

                    List<KPI> KPIs = ReadAll();
                    List<OrgStructure> OrgStructures = GetOrgStructures();
                    List<DivisionalObjective> DivisionalObjective = GetDivisionalObjectives();

                    MapKPIPropertiesAdmin(kpi, KPIs, OrgStructures, DivisionalObjective);
                    kpi.KPIType.MapAttachment(dataAccess);
                    if (kpi.Perspective != null)
                        kpi.Perspective.MapAttachment(dataAccess);
                    return kpi.MapAttachment(dataAccess);
                }
                return null;
            }
        }
        public KPI ReadByID_LessData(int id, string username)
        {
            using (var dataAccess = _factory.Create())
            {
                KPI kpi = dataAccess.KPI.Query()
                    .IncludeOptimized(a => a.KPIComments)
                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimizedByPath("KPIType.KPIThresholds")
                    .IncludeOptimizedByPath("KPIType.KPIThresholds.Status")
                    .IncludeOptimizedByPath("KPIType.Workflows")
                    .IncludeOptimizedByPath("KPIType.Workflows.BaseWorkflow")
                    .IncludeOptimized(a => a.StrategicObjective)
                    .IncludeOptimizedByPath("StrategicObjective.Theme")
                    .IncludeOptimizedByPath("StrategicObjective.Theme.Strategy")
                    .IncludeOptimized(a => a.Perspective)
                    .IncludeOptimizedByPath("Perspective.Strategy")
                    .IncludeOptimized(a => a.Parameters)
                    .IncludeOptimized(a => a.ChampionModel)
                    .IncludeOptimizedByPath("ChampionModel.UsersGroups")
                    .IncludeOptimizedByPath("ChampionModel.UsersGroups.Group")
                    .IncludeOptimized(a => a.OwnerModel)
                    .IncludeOptimizedByPath("OwnerModel.UsersGroups")
                    .IncludeOptimizedByPath("OwnerModel.UsersGroups.Group")
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimizedByPath("DivisionalObjective.OrgStructure")
                    .IncludeOptimized(a => a.OrgStructure)
                    .Where(s => s.ID == id).FirstOrDefault();

                kpi.KPIMeasures = kpi.KPIMeasures.Where(x => x.HasNoTarget != true).ToList();


                if (kpi != null)
                    kpi = (KPI)kpi.SecureObj(dataAccess, username);

                if (kpi != null)
                {
                    if (kpi.DivisionalObjective != null)
                        kpi.OrgStructureID = kpi.DivisionalObjective.OrgStructureId;
                    else if (kpi.OrgStructure != null)
                        kpi.OrgStructureID = kpi.OrgStructure.ID;


                    kpi.IsDeletable = true;
                    kpi = SetCanUpdate(kpi, username);


                    MapKPIProperties(kpi, null, null, null, false);
                    kpi.KPIType.MapAttachment(dataAccess);
                    if (kpi.Perspective != null)
                        kpi.Perspective.MapAttachment(dataAccess);
                    return kpi.MapAttachment(dataAccess);
                }
                return null;
            }
        }

        public KPI SingleKPI(int id)
        {
            using (var dataAccess = _factory.Create())
            {
                KPI kpi = dataAccess.KPI.Query()
                    .Include(a => a.KPIMeasures)
                    .Include(a => a.Parameters)
                    .Include(a => a.ChampionModel.UsersGroups.Select(s => s.Group))
                    .Include(a => a.OwnerModel.UsersGroups.Select(s => s.Group))
                    .Where(s => s.ID == id).SingleOrDefault();


                List<KPI> KPIs = ReadAll();
                List<OrgStructure> OrgStructures = GetOrgStructures();
                List<DivisionalObjective> DivisionalObjective = GetDivisionalObjectives();

                MapKPIProperties(kpi, KPIs, OrgStructures, DivisionalObjective);
                return kpi.MapAttachment(dataAccess);
            }
        }
        public List<KPIComment> ReadComments(int KPIID, string userName)
        {

            using (var dataAccess = _factory.Create())
            {
                KPI kpi = (KPI)dataAccess.KPI.Query().IncludeOptimized(a => a.KPIComments)
                    .IncludeOptimized(a => a.ChampionModel)
                    .IncludeOptimized(a => a.OwnerModel).Where(a => a.ID == KPIID).FirstOrDefault().SecureObj(dataAccess, userName);
                if (kpi != null)
                {
                    return kpi.KPIComments.ToList();
                }
                else
                    return new List<KPIComment>();

            }
        }


        public KPI GetKPIByMeasureID(int id, string userName)
        {
            using (var dataAccess = _factory.Create())
            {
                KPI kpi = (KPI)dataAccess.KPI.Query()
                    .Include(a => a.ChampionModel)
                    .Include(a => a.OwnerModel)
                    .Include(a => a.KPIMeasures)
                    .Include(a => a.KPIType.Workflows.Select(s => s.WorkflowSteps))
                    .Include(a => a.Parameters)
                    .Where(a => a.KPIMeasures.Where(m => m.ID == id).Count() > 0)
                    .FirstOrDefault().SecureObj(dataAccess, userName);

                List<KPI> KPIs = ReadAll();
                List<OrgStructure> OrgStructures = GetOrgStructures();
                List<DivisionalObjective> DivisionalObjective = GetDivisionalObjectives();

                if (kpi != null)
                    MapKPIProperties(kpi, KPIs, OrgStructures, DivisionalObjective);
                return kpi;
            }
        }


        public KPI GetKPIByMeasureID_All(int id)
        {
            using (var dataAccess = _factory.Create())
            {
                KPI kpi = (KPI)dataAccess.KPI.Query()
                    .Include(a => a.ChampionModel)
                    .Include(a => a.OwnerModel)
                    .Include(a => a.KPIMeasures)
                    .Include(a => a.KPIType.Workflows.Select(s => s.WorkflowSteps))
                    .Include(a => a.Parameters)
                    .Where(a => a.KPIMeasures.Where(m => m.ID == id).Count() > 0)
                    .FirstOrDefault();

                List<KPI> KPIs = ReadAll();
                List<OrgStructure> OrgStructures = GetOrgStructures();
                List<DivisionalObjective> DivisionalObjective = GetDivisionalObjectives();

                if (kpi != null)
                    MapKPIProperties(kpi, KPIs, OrgStructures, DivisionalObjective);
                return kpi;
            }
        }
        #endregion

        #region Update

        public KPI KPIChangeRequest(KPI kpi, string username, List<int> ChangeType)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                KPI current = dataAccess.KPI.Query().IncludeOptimized(a => a.KPIMeasures).Where(w => w.ID == kpi.ID).FirstOrDefault();

                if (current != null)
                {
                    if (ChangeType.Contains((int)KPIChangeRequestType.ChangeKPICalculationMethod))
                    {

                        current.CalculationMethod = kpi.CalculationMethod;
                        dataAccess.KPI.Save(current);



                        foreach (var measure in current.KPIMeasures)
                        {
                            if (measure.ID != 0)
                            {
                                KPIMeasure currentMeasure = current.KPIMeasures.Where(w => w.ID == measure.ID && w.Status == "NA").FirstOrDefault();
                                if (currentMeasure != null)
                                {

                                    measure.CalculationMethod = kpi.CalculationMethod;

                                    dataAccess.KPIMeasure.Save(currentMeasure);
                                }
                            }

                        }

                        result = dataAccess.Complete();

                    }
                    if (ChangeType.Contains((int)KPIChangeRequestType.ChangeKPIWeight))
                    {
                        List<KPI> currentKpis = dataAccess.KPI.Query().ToList();
                        foreach (var item in currentKpis)
                        {
                            if (kpi.KPIsWeight.Select(a => a.ID).Contains(item.ID))
                            {

                                item.Weight = kpi.KPIsWeight.Where(a => a.ID == item.ID).FirstOrDefault().Weight;
                                dataAccess.KPI.Save(item);
                                result = dataAccess.Complete();

                            }
                        }


                    }

                    if (ChangeType.Contains((int)KPIChangeRequestType.ChangeKPIInformation))
                    {

                        current.ArabicDescription = kpi.ArabicDescription;
                        current.EnglishDescription = kpi.EnglishDescription;
                        current.ArabicEquation = kpi.ArabicEquation;
                        current.EnglishEquation = kpi.EnglishEquation;
                        current.ArabicName = kpi.ArabicName;
                        current.EnglishName = kpi.EnglishName;
                        current.Champion = kpi.Champion;
                        current.Owner = kpi.Owner;
                        dataAccess.KPI.Save(current);
                        result = dataAccess.Complete();
                    }
                    if (ChangeType.Contains((int)KPIChangeRequestType.ChangeKPITarget))
                    {
                        current.Years = kpi.Years;

                        dataAccess.KPI.Save(current);
                        result = dataAccess.Complete();

                        bool changeCalculation = ChangeType.Contains((int)KPIChangeRequestType.ChangeKPICalculationMethod);
                        foreach (var measure in kpi.KPIMeasures)
                        {
                            if (measure.ID == 0 && ChangeType.Contains((int)KPIChangeRequestType.ChangeKPITarget))
                            {
                                measure.Status = "NA";
                                measure.Created = DateTime.Now;
                                measure.Modified = DateTime.Now;
                                measure.KPIID = kpi.ID;
                                if (changeCalculation)
                                    measure.CalculationMethod = kpi.CalculationMethod;
                                else
                                    measure.CalculationMethod = current.CalculationMethod;

                                dataAccess.KPIMeasure.Save(measure);
                            }
                            else if (measure.ID != 0 && ChangeType.Contains((int)KPIChangeRequestType.ChangeKPITarget))
                            {
                                KPIMeasure currentMeasure = current.KPIMeasures.Where(w => w.ID == measure.ID && DateTime.Now.Date <= w.DueDate.Date).FirstOrDefault();
                                if (currentMeasure != null && (measure.Target != currentMeasure.Target || 
                                                               measure.HasNoTarget != currentMeasure.HasNoTarget))
                                {
                                    currentMeasure.Target = measure.HasNoTarget ? 0 : measure.Target;
                                    currentMeasure.HasNoTarget = measure.HasNoTarget;
                                    currentMeasure.Modified = DateTime.Now;
                                    if (changeCalculation)
                                        measure.CalculationMethod = kpi.CalculationMethod;
                                    else
                                           if (!changeCalculation)
                                        measure.CalculationMethod = current.CalculationMethod;
                                    dataAccess.KPIMeasure.Save(currentMeasure);
                                }
                            }
                        }
                        result = dataAccess.Complete();
                    }
                }
                return ReadByID(kpi.ID, username);
            }
        }

        public KPI Update(KPI kpi, string username)
        {
            int result;


            using (var dataAccess = _factory.Create())
            {
                KPI current = dataAccess.KPI.Query().Where(w => w.ID == kpi.ID).FirstOrDefault();

                kpi.Modified = DateTime.Now;
                kpi.Created = current.Created;
                kpi.KPITypeID = current.KPITypeID;
                kpi.DataSource = current.DataSource;
                //kpi.CalculationMethod = current.CalculationMethod;
                kpi.Polarity = current.Polarity;
                kpi.Frequency = current.Frequency;
                kpi.StartDate = current.StartDate;
                kpi.UnitOfMeasure = current.UnitOfMeasure;
                kpi.RequireUpdate = current.RequireUpdate;
                kpi.Weight = current.Weight;
                kpi.DepartmentalWeight = current.DepartmentalWeight;
                kpi.BusinessUnitWeight = current.BusinessUnitWeight;


                //if (kpi.StrategicObjectiveID.HasValue && kpi.StrategicObjectiveID != 0)
                //    kpi.DivisionalObjectiveID = null;


                //if (kpi.DivisionalObjectiveID.HasValue && kpi.DivisionalObjectiveID != 0)
                //    kpi.StrategicObjectiveID = null;
                decimal? SummedTargets = 0;
                decimal? AverageTargets = 0;
                int PeriodCount = 0;
                foreach (var measure in kpi.KPIMeasures)
                {
                    if (measure.ID == 0)
                    {
                        measure.Status = "NA";
                        measure.AccumulutiveStatus = "NA";
                        measure.Created = DateTime.Now;
                        measure.Modified = DateTime.Now;
                        measure.KPIID = kpi.ID;
                        measure.CalculationMethod = kpi.CalculationMethod;
                        SummedTargets += measure.Target;
                        PeriodCount++;
                        AverageTargets = SummedTargets / PeriodCount;
                        //if (measure.Target == kpi.Baseline || SummedTargets == kpi.Baseline || AverageTargets == kpi.Baseline)
                        //{
                        //    throw new System.Exception("TargetEqualBaseline");
                        //}

                        dataAccess.KPIMeasure.Save(measure);
                    }
                    SummedTargets += measure.Target;
                    PeriodCount++;
                    AverageTargets = SummedTargets / PeriodCount;
                    //if (measure.Target == kpi.Baseline || SummedTargets == kpi.Baseline || AverageTargets == kpi.Baseline)
                    //{
                    //    throw new System.Exception("TargetEqualBaseline");
                    //}
                }


                #region Parameter
                List<ParameterValue> parameterValues = dataAccess.ParameterValue.Query().ToList();
                List<Parameter> parameters = kpi.Parameters.ToList();
                List<Parameter> currentparameter = dataAccess.Parameter.Query().Where(w => w.KPIID == kpi.ID).ToList();
                List<Parameter> parameterstoDelete = new List<Parameter>();
                parameterstoDelete = currentparameter.Where(w => !parameters.Select(s => s.ID).Contains(w.ID)).ToList();
                if (parameterstoDelete != null && parameterstoDelete.Count > 0)
                {
                    foreach (var param in parameterstoDelete)
                    {
                        var paramvalues = parameterValues.Where(w => w.ParameterID == param.ID).ToList();
                        foreach (var paramvalue in paramvalues)
                        {
                            dataAccess.ParameterValue.Delete(paramvalue);
                        }
                        dataAccess.Parameter.Delete(param);
                    }
                }
                // result = dataAccess.Complete();
                if (kpi.Parameters != null && kpi.Parameters.Count > 0 && (kpi.DataSource == "semi-auto" || kpi.DataSource == "auto"))
                {
                    foreach (var param in kpi.Parameters)
                    {
                        if (param.ID == 0)
                        {
                            param.IsConstant = param.IsConstant;
                            param.ConstantValue = param.ConstantValue;
                            param.ParamId = param.ParameterName;
                            param.Created = DateTime.Now;
                            param.Modified = DateTime.Now;
                            param.KPIID = kpi.ID;
                            dataAccess.Parameter.Save(param);
                        }
                        else
                        {
                            var paramtoupdate = currentparameter.Where(w => w.ID == param.ID).FirstOrDefault();
                            if (paramtoupdate != null)
                            {
                                paramtoupdate.ParamId = param.ParameterName;
                                paramtoupdate.ParameterName = param.ParameterName;
                                paramtoupdate.Modified = DateTime.Now;
                                param.IsConstant = param.IsConstant;
                                param.ConstantValue = param.ConstantValue;
                                dataAccess.Parameter.Save(paramtoupdate);
                            }
                        }
                    }
                }
                #endregion

                #region Attachment
                var SourceAttachments = dataAccess.Attachment.Query();


                List<Attachment> Attachments = new List<Attachment>();
                foreach (var Attachment in kpi.Attachments)
                {
                    Attachment attach = new Attachment();
                    attach = SourceAttachments.ToList().Where(w => w.ID == Attachment.ID).FirstOrDefault();
                    if (attach != null)
                    {
                        attach.RelatedItemID = kpi.ID;
                        attach.Type = typeof(KPI).Name;
                        Attachments.Add(attach);
                        dataAccess.Attachment.Save(attach);
                    }
                }
                #endregion

                dataAccess.KPI.Save(kpi);
                result = dataAccess.Complete();

                #region Parameter Value
                ParameterValue parameterValue = null;
                foreach (KPIMeasure _KPIMeasure in kpi.KPIMeasures)
                {
                    foreach (Parameter Parameter in kpi.Parameters)
                    {
                        var paramValue = parameterValues.Where(w => w.MeasureID == _KPIMeasure.ID && w.ParameterID == Parameter.ID).FirstOrDefault();
                        if (paramValue == null)
                        {
                            parameterValue = new ParameterValue();
                            parameterValue.ParameterID = Parameter.ID;
                            parameterValue.Title = Parameter.ParameterName;
                            parameterValue.Value = Parameter.Value;
                            parameterValue.MeasureID = _KPIMeasure.ID;
                            parameterValue.Created = DateTime.Now;
                            parameterValue.Modified = DateTime.Now;
                            dataAccess.ParameterValue.Save(parameterValue);
                        }
                        else
                        {
                            paramValue.Title = Parameter.ParameterName;
                            paramValue.Modified = DateTime.Now;
                            dataAccess.ParameterValue.Save(paramValue);
                        }
                    }
                }
                result = dataAccess.Complete();
                #endregion

                var attachmentstodelete = kpi.MapAttachment(dataAccess).Attachments.ToList();

                if (attachmentstodelete != null && attachmentstodelete.Count() > 0)
                {
                    attachmentstodelete = attachmentstodelete.Where(w => !Attachments.Select(s => s.ID).Contains(w.ID)).ToList();
                    foreach (var attach in attachmentstodelete)
                    {
                        if (attach != null)
                        {
                            Task.Run(() => Task.Run(async () => { await DeleteAttachment(attachmentstodelete); }));
                        }
                    }
                }

                return ReadByID(kpi.ID, username);
            }
        }

        public void Update_ManualLock(KPI kpi)
        {
            int result;


            using (var dataAccess = _factory.Create())
            {
                KPI current = dataAccess.KPI.Query().Where(w => w.ID == kpi.ID).FirstOrDefault();

                kpi.Modified = DateTime.Now;
                kpi.ManualUnLock = false;

                dataAccess.KPI.Save(kpi);
                result = dataAccess.Complete();





            }
        }
        public List<KPI> Update(List<KPI> kpis)
        {
            int result;

            using (var dataAccess = _factory.Create())
            {
                List<KPI> contexts = dataAccess.KPI.Query().ToList();
                foreach (KPI kpi in kpis)
                {
                    KPI context = contexts.Where(w => w.ID == kpi.ID).FirstOrDefault();
                    context.Weight = kpi.Weight;
                    context.DepartmentalWeight = kpi.DepartmentalWeight;
                    context.Modified = DateTime.Now;
                    dataAccess.KPI.Save(context);
                }
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return kpis;
            }
        }
        public List<KPI> UpdateBusinessWeight(List<KPI> kpis)
        {
            int result;

            using (var dataAccess = _factory.Create())
            {
                List<KPI> contexts = dataAccess.KPI.Query().ToList();
                foreach (KPI kpi in kpis)
                {
                    KPI context = contexts.Where(w => w.ID == kpi.ID).FirstOrDefault();
                    context.BusinessUnitWeight = kpi.BusinessUnitWeight;
                    context.Modified = DateTime.Now;
                    dataAccess.KPI.Save(context);
                }
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return kpis;
            }
        }
        public KPIComment UpdateComment(KPIComment kpiComment)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                var oldkpiComment = dataAccess.KPIComment.Get(kpiComment.ID);

                oldkpiComment.Modified = DateTime.Now;
                oldkpiComment.Comment = kpiComment.Comment;
                dataAccess.KPIComment.Save(oldkpiComment);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return oldkpiComment;
            }
        }
        //public void UpdateKPIPeriod(UpdateKPIForm form)
        //{
        //    using (var dataAccess = _factory.Create())
        //    {
        //        KPI kpi = dataAccess.KPI.Query().Include(a => a.KPIMeasures).Include(a => a.KPIType.KPIThresholds.Select(s => s.Status)).Where(a => a.KPIMeasures.Where(m => m.ID == form.RelatedID).Count() > 0).FirstOrDefault();
        //        KPIMeasure measure = kpi.KPIMeasures.FirstOrDefault(a => a.ID == form.RelatedID);
        //        kpi.KPIMeasures = SetActiveMeasure(kpi.KPIMeasures.ToList());
        //        // Update Measure Details
        //        measure.AllowUpdate = false;

        //        measure.Value = form.Value;
        //        decimal baseline = kpi.Baseline;
        //        decimal OutOfTarget;

        //        if (measure.Target - kpi.Baseline != 0)
        //            OutOfTarget = (form.Value - kpi.Baseline) / (measure.Target - kpi.Baseline) * 100;
        //        else
        //            OutOfTarget = 0;

        //        measure.OutOfTarget = (decimal)Math.Round((double)OutOfTarget, 2);
        //        measure.Status = kpi.KPIType.KPIThresholds
        //            .Where(w => w.Max != null ? CalculateKPIOperation(measure.OutOfTarget.Value, w.Max.Value, true, w.Operator) : true)
        //            .Where(w => w.Min != null ? CalculateKPIOperation(measure.OutOfTarget.Value, w.Min.Value, false, w.MinOperator) : true)
        //            .Select(s => s.Code).FirstOrDefault();

        //        if (measure.Status == null)
        //            measure.Status = "NA";


        //        decimal comparer = 0;
        //        if (kpi.KPIMeasures.Min(x => x.ID) == measure.ID)
        //            comparer = kpi.Baseline;
        //        else
        //            comparer = kpi.KPIMeasures.Where(w => w.IsActive).FirstOrDefault().Value.Value;

        //        //Update KPI Details
        //        if (kpi?.Polarity?.ToLower() == "positive")
        //        {
        //            if (form.Value > comparer)
        //                kpi.Direction = "Up";
        //            else if (form.Value < comparer)
        //                kpi.Direction = "Down";
        //            else
        //                kpi.Direction = "Same";
        //        }
        //        else if (kpi?.Polarity?.ToLower() == "negative")
        //        {
        //            if (form.Value < comparer)
        //                kpi.Direction = "Up";
        //            else if (form.Value > comparer)
        //                kpi.Direction = "Down";
        //            else
        //                kpi.Direction = "Same";
        //        }
        //        else
        //            kpi.Direction = null;

        //        if (kpi.KPIMeasures.Where(a => a.AllowUpdate).Count() == 0 && !kpi.KPIMeasures.Any(a => a.Status == "NA"))
        //            kpi.RequireUpdate = false;

        //        dataAccess.KPI.Save(kpi);
        //        dataAccess.Complete();


        //    }
        //}

        public void UpdateKPIPeriod(UpdateKPIForm form)
        {
            using (var dataAccess = _factory.Create())
            {
                KPI kpi = dataAccess.KPI.Query().Include(a => a.KPIMeasures).Include(a => a.Parameters).Include(a => a.KPIType.KPIThresholds.Select(s => s.Status)).Where(a => a.KPIMeasures.Where(m => m.ID == form.RelatedID).Count() > 0).FirstOrDefault();
                KPIMeasure measure = kpi.KPIMeasures.FirstOrDefault(a => a.ID == form.RelatedID);
                kpi.KPIMeasures = SetActiveMeasure(kpi.KPIMeasures.ToList());
                // Update Measure Details
                measure.AllowUpdate = false;
                decimal baseline = kpi.Baseline;
                decimal OutOfTarget;
                //measure.Value = form.Value;

                //Accumulative values
                decimal AccumulutiveOutOfTarget = 0;
                List<KPIMeasure> oldFrequenciesMeasures = kpi.KPIMeasures.Where(w => w.ID <= measure.ID).ToList();
                measure.Value = form.Value;
                decimal ParameterizedValue = CalculatePeriodActual(measure, kpi);
                decimal Target = CalculateTarget(measure, kpi, oldFrequenciesMeasures);

                OutOfTarget = CalculateOutOfTarget(measure, kpi, form.Value);
                AccumulutiveOutOfTarget = CalculateAccumulutiveOutOfTarget(measure, kpi, Target, ParameterizedValue);

                measure.AccumulutiveValue = ParameterizedValue;
                measure.AccumulutiveTarget = Target;
                measure.Value = form.Value;
                measure.OutOfTarget = OutOfTarget;
                measure.AccumulutiveOutOfTarget = AccumulutiveOutOfTarget;

                #region Commented
                //switch (kpi.CalculationMethod)
                //{
                //    case (int)CalculationMethodsEnum.LastValue:
                //        if (kpi?.Polarity?.ToLower() == "positive")
                //        {
                //            //( Current Actual - Baseline  / Current Target- Baseline ) * 100 
                //            if (measure.Target - baseline != 0)
                //                OutOfTarget = ((form.Value - baseline) / (measure.Target - baseline)) * 100;
                //            else
                //                OutOfTarget = 0;
                //        }
                //        else if (kpi?.Polarity?.ToLower() == "negative")
                //        {
                //            //Based on Last CR Equation is "1-((Actual-Target)/(Baseline-Target)) * 100"

                //            //based on client commnet 2024074
                //            //if value=0, baseline=0 , and taregt =0 > 100
                //            //baseline=0, target=0 >0

                //            if (baseline == 0 && measure.Target == 0 && measure.Value == 0)
                //            {
                //                OutOfTarget = 100;
                //            }

                //            else if (baseline == 0 && measure.Target == 0)
                //            {
                //                OutOfTarget = 0;
                //            }
                //            else
                //            {

                //                if (measure.Target - baseline != 0)
                //                    OutOfTarget = (1 - (form.Value - measure.Target) / (measure.Target - baseline)) * 100;
                //                else
                //                    OutOfTarget = 0;
                //            }
                //        }
                //        else
                //        {
                //            OutOfTarget = 0;
                //        }
                //        //Accumulative values
                //        if (kpi?.Polarity?.ToLower() == "positive")
                //        {
                //            //((Sum of the Actuals Values - Baseline) / (Sum of the Target Values -Baseline)) *100 %
                //            if (targetSum - baseline != 0)
                //                AccumulutiveOutOfTarget = ((valuesSum - baseline) / (targetSum - baseline)) * 100;
                //            else
                //                AccumulutiveOutOfTarget = 0;

                //        }
                //        else if (kpi?.Polarity?.ToLower() == "negative")
                //        {

                //            //based on client commnet 2024074
                //            //if value=0, baseline=0 , and taregt =0 > 100
                //            //baseline=0, target=0 >0

                //            if (baseline == 0 && targetSum == 0 && valuesSum == 0)
                //            {
                //                AccumulutiveOutOfTarget = 100;
                //            }

                //            else if (baseline == 0 && targetSum == 0)
                //            {
                //                AccumulutiveOutOfTarget = 0;
                //            }
                //            else

                //            {
                //                //1-(( Sum of Actuals - Sum of Targets)/ (Baseline - Sum of Targets)) *100 %
                //                if (targetSum - baseline != 0)
                //                    AccumulutiveOutOfTarget = (1 - (valuesSum - targetSum) / (targetSum - baseline)) * 100;
                //                else
                //                    AccumulutiveOutOfTarget = 0;
                //            }
                //        }
                //        else
                //            AccumulutiveOutOfTarget = 0;

                //        measure.AccumulutiveOutOfTarget = AccumulutiveOutOfTarget;
                //        measure.AccumulutiveValue = valuesSum;
                //        measure.AccumulutiveTarget = targetSum;
                //        break;
                //    case (int)CalculationMethodsEnum.Sum:

                //        #region Commented
                //        //if (kpi?.Polarity?.ToLower() == "positive")
                //        //{
                //        //    //((Sum of the Actuals Values - Baseline) / (Sum of the Target Values -Baseline)) *100 %
                //        //    if (targetSum - baseline != 0)
                //        //        OutOfTarget = ((valuesSum - baseline) / (targetSum - baseline)) * 100;
                //        //    else
                //        //        OutOfTarget = 0;

                //        //}
                //        //else if (kpi?.Polarity?.ToLower() == "negative")
                //        //{

                //        //    //based on client commnet 2024074
                //        //    //if value=0, baseline=0 , and taregt =0 > 100
                //        //    //baseline=0, target=0 >0

                //        //    if (baseline == 0 && targetSum == 0 && valuesSum == 0)
                //        //    {
                //        //        OutOfTarget = 100;
                //        //    }
                //        //    else if (baseline == 0 && targetSum == 0)
                //        //    {
                //        //        OutOfTarget = 0;
                //        //    }
                //        //    else

                //        //    {
                //        //        //1-(( Sum of Actuals - Sum of Targets)/ (Baseline - Sum of Targets)) *100 %
                //        //        if (targetSum - baseline != 0)
                //        //            OutOfTarget = (1 - (valuesSum - targetSum) / (targetSum - baseline)) * 100;
                //        //        else
                //        //            OutOfTarget = 0;
                //        //    }
                //        //}
                //        //else
                //        //{
                //        //    OutOfTarget = 0;
                //        //}
                //        #endregion

                //        //Accumulative values
                //        if (kpi?.Polarity?.ToLower() == "positive")
                //        {
                //            //((Sum of the Actuals Values - Baseline) / (Sum of the Target Values -Baseline)) *100 %
                //            if (targetSum - baseline != 0)
                //                AccumulutiveOutOfTarget = ((valuesSum - baseline) / (targetSum - baseline)) * 100;
                //            else
                //                AccumulutiveOutOfTarget = 0;

                //        }
                //        else if (kpi?.Polarity?.ToLower() == "negative")
                //        {
                //            //based on client commnet 2024074
                //            //if value=0, baseline=0 , and taregt =0 > 100
                //            //baseline=0, target=0 >0
                //            if (baseline == 0 && targetSum == 0 && valuesSum == 0)
                //            {
                //                AccumulutiveOutOfTarget = 100;
                //            }
                //            else if (baseline == 0 && targetSum == 0)
                //            {
                //                AccumulutiveOutOfTarget = 0;
                //            }
                //            else
                //            {
                //                //1-(( Sum of Actuals - Sum of Targets)/ (Baseline - Sum of Targets)) *100 %
                //                if (targetSum - baseline != 0)
                //                    AccumulutiveOutOfTarget = (1 - (valuesSum - targetSum) / (targetSum - baseline)) * 100;
                //                else
                //                    AccumulutiveOutOfTarget = 0;
                //            }
                //        }
                //        else
                //            AccumulutiveOutOfTarget = 0;

                //        measure.AccumulutiveOutOfTarget = AccumulutiveOutOfTarget;
                //        measure.AccumulutiveValue = valuesSum;
                //        measure.AccumulutiveTarget = targetSum;
                //        break;
                //    case (int)CalculationMethodsEnum.Average:
                //        List<KPIMeasure> oldFrequenciesMeasures_avg = kpi.KPIMeasures.Where(w => w.ID <= measure.ID).ToList();
                //        decimal valuesSum_avg = (oldFrequenciesMeasures_avg.Sum(s => s.Value)) ?? default;
                //        decimal targetSum_avg = oldFrequenciesMeasures_avg.Sum(s => s.Target);

                //        decimal ValuesAverage = oldFrequenciesMeasures_avg.Any() ? valuesSum_avg / oldFrequenciesMeasures_avg.Count() : 0;
                //        decimal TargetAverage = oldFrequenciesMeasures_avg.Any() ? targetSum_avg / oldFrequenciesMeasures_avg.Count() : 0;
                //        #region Commented
                //        //if (kpi?.Polarity?.ToLower() == "positive")
                //        //{
                //        //    //((Average of the Actuals Values - Baseline) / (Average of the Target Values -Baseline)) *100 %
                //        //    if (TargetAverage - baseline != 0)
                //        //        OutOfTarget = ((ValuesAverage - baseline) / (TargetAverage - baseline)) * 100;
                //        //    else
                //        //        OutOfTarget = 0;

                //        //}
                //        //else if (kpi?.Polarity?.ToLower() == "negative")
                //        //{

                //        //    //based on client commnet 2024074
                //        //    //if value=0, baseline=0 , and taregt =0 > 100
                //        //    //baseline=0, target=0 >0
                //        //    if (baseline == 0 && TargetAverage == 0 && ValuesAverage == 0)
                //        //    {
                //        //        OutOfTarget = 100;
                //        //    }
                //        //    else if (baseline == 0 && TargetAverage == 0)
                //        //    {
                //        //        OutOfTarget = 0;
                //        //    }
                //        //    else
                //        //    {
                //        //        //= 1-(( Average of Actuals- Average of Targets)/ (Baseline - Average of Targets)) *100 %
                //        //        if (TargetAverage - baseline != 0)
                //        //            OutOfTarget = (1 - (ValuesAverage - TargetAverage) / (TargetAverage - baseline)) * 100;
                //        //        else
                //        //            OutOfTarget = 0;
                //        //    }
                //        //}
                //        //else
                //        //{
                //        //    OutOfTarget = 0;
                //        //}
                //        #endregion

                //        //Accumulative values
                //        if (kpi?.Polarity?.ToLower() == "positive")
                //        {
                //            //((Average of the Actuals Values - Baseline) / (Average of the Target Values -Baseline)) *100 %
                //            if (TargetAverage - baseline != 0)
                //                AccumulutiveOutOfTarget = ((ValuesAverage - baseline) / (TargetAverage - baseline)) * 100;
                //            else
                //                AccumulutiveOutOfTarget = 0;

                //        }
                //        else if (kpi?.Polarity?.ToLower() == "negative")
                //        {
                //            //based on client commnet 2024074
                //            //if value=0, baseline=0 , and taregt =0 > 100
                //            //baseline=0, target=0 >0
                //            if (baseline == 0 && TargetAverage == 0 && ValuesAverage == 0)
                //            {
                //                AccumulutiveOutOfTarget = 100;
                //            }
                //            else if (baseline == 0 && TargetAverage == 0)
                //            {
                //                AccumulutiveOutOfTarget = 0;
                //            }
                //            else
                //            {
                //                //= 1-(( Average of Actuals- Average of Targets)/ (Baseline - Average of Targets)) *100 %
                //                if (TargetAverage - baseline != 0)
                //                    AccumulutiveOutOfTarget = (1 - (ValuesAverage - TargetAverage) / (TargetAverage - baseline)) * 100;
                //                else
                //                    AccumulutiveOutOfTarget = 0;
                //            }
                //        }
                //        else
                //            AccumulutiveOutOfTarget = 0;

                //        measure.AccumulutiveOutOfTarget = AccumulutiveOutOfTarget;
                //        measure.AccumulutiveValue = (decimal)Math.Round((double)(ValuesAverage), 2);
                //        measure.AccumulutiveTarget = (decimal)Math.Round((double)(TargetAverage), 2);
                //        break;
                //    default:
                //        OutOfTarget = 0;
                //        //
                //        measure.AccumulutiveOutOfTarget = 0;
                //        measure.AccumulutiveValue = measure.Value;
                //        measure.AccumulutiveTarget = measure.Target;
                //        //
                //        break;
                //}
                #endregion

                //end Accu

                measure.Status = kpi.KPIType.KPIThresholds
                    .Where(w => w.Max != null ? CalculateKPIOperation(measure.OutOfTarget.Value, w.Max.Value, true, w.Operator) : true)
                    .Where(w => w.Min != null ? CalculateKPIOperation(measure.OutOfTarget.Value, w.Min.Value, false, w.MinOperator) : true)
                    .Select(s => s.Code).FirstOrDefault();

                if (measure.Status == null)
                    measure.Status = "NA";


                //start Accu
                measure.AccumulutiveStatus = kpi.KPIType.KPIThresholds
                  .Where(w => w.Max != null ? CalculateKPIOperation(measure.AccumulutiveOutOfTarget.Value, w.Max.Value, true, w.Operator) : true)
                  .Where(w => w.Min != null ? CalculateKPIOperation(measure.AccumulutiveOutOfTarget.Value, w.Min.Value, false, w.MinOperator) : true)
                  .Select(s => s.Code).FirstOrDefault();

                if (measure.AccumulutiveStatus == null)
                    measure.AccumulutiveStatus = "NA";
                //end Accu


                decimal comparer = 0;
                if (kpi.KPIMeasures.Min(x => x.ID) == measure.ID)
                    comparer = kpi.Baseline;
                else
                    comparer = kpi.KPIMeasures.Where(w => w.IsActive).FirstOrDefault().Value.Value;

                //Update KPI Details
                if (kpi?.Polarity?.ToLower() == "positive")
                {
                    if (form.Value > comparer)
                        kpi.Direction = "Up";
                    else if (form.Value < comparer)
                        kpi.Direction = "Down";
                    else
                        kpi.Direction = "Same";
                }
                else if (kpi?.Polarity?.ToLower() == "negative")
                {
                    if (form.Value < comparer)
                        kpi.Direction = "Up";
                    else if (form.Value > comparer)
                        kpi.Direction = "Down";
                    else
                        kpi.Direction = "Same";
                }
                else
                    kpi.Direction = null;

                if (kpi.KPIMeasures.Where(a => a.AllowUpdate).Count() == 0)
                    kpi.RequireUpdate = false;

                kpi.Modified = DateTime.Now;
                dataAccess.KPI.Save(kpi);
                dataAccess.Complete();


            }
        }


        public void UpdateKPIPeriod_Script(UpdateKPIForm form)
        {
            using (var dataAccess = _factory.Create())
            {
                KPI kpi = dataAccess.KPI.Query().Include(a => a.KPIMeasures).Include(a => a.Parameters).Include(a => a.KPIType.KPIThresholds.Select(s => s.Status)).Where(a => a.KPIMeasures.Where(m => m.ID == form.RelatedID).Count() > 0).FirstOrDefault();
                if (kpi != null)
                {
                    KPIMeasure measure = kpi.KPIMeasures.FirstOrDefault(a => a.ID == form.RelatedID);
                    if (measure != null && measure.Status == "NA" && measure.DueDate.Date < DateTime.Now.Date)
                    {
                        kpi.KPIMeasures = SetActiveMeasure(kpi.KPIMeasures.ToList());
                        // Update Measure Details
                        measure.AllowUpdate = false;
                        decimal baseline = kpi.Baseline;
                        decimal OutOfTarget;
                        //measure.Value = form.Value;

                        //Accumulative values
                        decimal AccumulutiveOutOfTarget = 0;
                        List<KPIMeasure> oldFrequenciesMeasures = kpi.KPIMeasures.Where(w => w.ID <= measure.ID).ToList();
                        measure.Value = form.Value;
                        decimal ParameterizedValue = CalculatePeriodActual(measure, kpi);
                        decimal Target = CalculateTarget(measure, kpi, oldFrequenciesMeasures);

                        OutOfTarget = CalculateOutOfTarget(measure, kpi, form.Value);
                        AccumulutiveOutOfTarget = CalculateAccumulutiveOutOfTarget(measure, kpi, Target, ParameterizedValue);

                        measure.AccumulutiveValue = ParameterizedValue;
                        measure.AccumulutiveTarget = Target;
                        measure.Value = form.Value;
                        measure.OutOfTarget = OutOfTarget;
                        measure.AccumulutiveOutOfTarget = AccumulutiveOutOfTarget;



                        //end Accu

                        measure.Status = kpi.KPIType.KPIThresholds
                            .Where(w => w.Max != null ? CalculateKPIOperation(measure.OutOfTarget.Value, w.Max.Value, true, w.Operator) : true)
                            .Where(w => w.Min != null ? CalculateKPIOperation(measure.OutOfTarget.Value, w.Min.Value, false, w.MinOperator) : true)
                            .Select(s => s.Code).FirstOrDefault();

                        if (measure.Status == null)
                            measure.Status = "NA";


                        //start Accu
                        measure.AccumulutiveStatus = kpi.KPIType.KPIThresholds
                          .Where(w => w.Max != null ? CalculateKPIOperation(measure.AccumulutiveOutOfTarget.Value, w.Max.Value, true, w.Operator) : true)
                          .Where(w => w.Min != null ? CalculateKPIOperation(measure.AccumulutiveOutOfTarget.Value, w.Min.Value, false, w.MinOperator) : true)
                          .Select(s => s.Code).FirstOrDefault();

                        if (measure.AccumulutiveStatus == null)
                            measure.AccumulutiveStatus = "NA";
                        //end Accu


                        decimal comparer = 0;
                        if (kpi.KPIMeasures.Min(x => x.ID) == measure.ID)
                            comparer = kpi.Baseline;
                        else
                            comparer = kpi.KPIMeasures.Where(w => w.IsActive).FirstOrDefault().Value.Value;

                        //Update KPI Details
                        if (kpi?.Polarity?.ToLower() == "positive")
                        {
                            if (form.Value > comparer)
                                kpi.Direction = "Up";
                            else if (form.Value < comparer)
                                kpi.Direction = "Down";
                            else
                                kpi.Direction = "Same";
                        }
                        else if (kpi?.Polarity?.ToLower() == "negative")
                        {
                            if (form.Value < comparer)
                                kpi.Direction = "Up";
                            else if (form.Value > comparer)
                                kpi.Direction = "Down";
                            else
                                kpi.Direction = "Same";
                        }
                        else
                            kpi.Direction = null;

                        if (kpi.KPIMeasures.Where(a => a.AllowUpdate).Count() == 0)
                            kpi.RequireUpdate = false;

                        kpi.Modified = DateTime.Now;
                        dataAccess.KPI.Save(kpi);
                        dataAccess.Complete();

                    }
                }
            }
        }
        public decimal CalculatePeriodActual(KPIMeasure CurrentMeasure, KPI kpi, bool OnlyLastValue = false)
        {
            decimal result = 0;
            string formula = kpi.Formula;
            bool ManualDispose = true;
            var dataAccess = _factory.Create();
            try
            {
                List<KPIMeasure> oldFrequenciesMeasures = kpi.KPIMeasures.Where(w => w.ID <= CurrentMeasure.ID).ToList();
                if (kpi.DataSource.ToLower() == "semi-auto" && !OnlyLastValue)
                {
                    List<Parameter> Parameters = dataAccess.Parameter.Query(a => a.KPIID == CurrentMeasure.KPIID).ToList();
                    List<ParameterValue> ParameterValues = dataAccess.ParameterValue.Query().ToList().Where(w => w.MeasureID == CurrentMeasure.ID || oldFrequenciesMeasures.Any(a => a.ID == w.MeasureID)).ToList();
                    Parameters.Sort((a, b) => b.ParameterName.Length.CompareTo(a.ParameterName.Length));
                    foreach (var parameter in Parameters)
                    {
                        var CurrentParameterValues = ParameterValues.Where(w => w.ParameterID == parameter.ID).ToList();
                        if (parameter.IsConstant)
                        {
                            formula = formula.Replace(parameter.ParameterName, kpi.Parameters.Where(a => a.ParameterName == parameter.ParameterName).FirstOrDefault()?.ConstantValue.ToString());
                        }
                        else
                        {
                            decimal ParameterValue = 0;
                            switch (kpi.CalculationMethod)
                            {
                                case (int)CalculationMethodsEnum.Sum:
                                    ParameterValue = CurrentParameterValues.Sum(s => s.Value);
                                    break;
                                case (int)CalculationMethodsEnum.Average:
                                    if (CurrentParameterValues.Any())
                                        ParameterValue = CurrentParameterValues.Sum(s => s.Value) / CurrentParameterValues.Count();
                                    break;
                                default:
                                    ParameterValue = 0;
                                    break;
                            }
                            formula = formula.Replace(parameter.ParameterName, ParameterValue.ToString());
                        }
                    }
                    result = CalculateFormula(formula);
                }
                else
                {
                    result = CalculateActual(CurrentMeasure, kpi, oldFrequenciesMeasures);
                }

            }
            catch (System.Exception ex)
            {
                CreateException("KPI", "CalculatePeriodActual", formula, ex.Message);
            }
            if (ManualDispose)
            {
                dataAccess.Dispose();
            }
            return result;
        }
        public decimal CalculateActual(KPIMeasure measure, KPI kpi, List<KPIMeasure> KPIMeasures)
        {
            decimal Actual;
            switch (kpi.CalculationMethod)
            {
                case (int)CalculationMethodsEnum.LastValue:
                    Actual = measure.Value ?? 0;
                    break;
                case (int)CalculationMethodsEnum.Sum:
                    Actual = KPIMeasures.Sum(s => s.Value ?? 0);
                    break;
                case (int)CalculationMethodsEnum.Average:
                    List<KPIMeasure> oldFrequenciesMeasures_avg = kpi.KPIMeasures.Where(w => w.ID <= measure.ID).ToList();
                    decimal valueSum_avg = oldFrequenciesMeasures_avg.Sum(s => s.Value ?? 0);
                    Actual = oldFrequenciesMeasures_avg.Any() ? valueSum_avg / oldFrequenciesMeasures_avg.Count() : 0;
                    break;
                default:
                    Actual = 0;
                    break;
            }
            return Actual;
        }
        public decimal CalculateOutOfTarget(KPIMeasure measure, KPI kpi, decimal Value)
        {
            decimal OutOfTarget = 0;
            if (kpi?.Polarity?.ToLower() == "positive")
            {
                //( Current Actual - Baseline  / Current Target- Baseline ) * 100 
                if (measure.Target - kpi.Baseline != 0)
                    OutOfTarget = ((Value - kpi.Baseline) / (measure.Target - kpi.Baseline)) * 100;
                else
                    OutOfTarget = 0;
            }
            else if (kpi?.Polarity?.ToLower() == "negative")
            {
                //Based on Last CR Equation is "1-((Actual-Target)/(Baseline-Target)) * 100"
                //based on client commnet 2024074
                //if value=0, baseline=0 , and taregt =0 > 100
                //baseline=0, target=0 >0

                if (kpi.Baseline == 0 && measure.Target == 0 && measure.Value == 0)
                {
                    OutOfTarget = 100;
                }

                else if (kpi.Baseline == 0 && measure.Target == 0)
                {
                    OutOfTarget = 0;
                }
                else
                {

                    if (measure.Target - kpi.Baseline != 0)
                        OutOfTarget = (1 - (Value - measure.Target) / (measure.Target - kpi.Baseline)) * 100;
                    else
                        OutOfTarget = 0;
                }
            }
            else
            {
                OutOfTarget = 0;
            }
            OutOfTarget = (decimal)Math.Round((double)OutOfTarget, 2);

            if (OutOfTarget > 100)
            {
                OutOfTarget = 100;
            }

            if (OutOfTarget < 0)
            {
                OutOfTarget = 0;
            }

            return OutOfTarget;

        }
        public decimal CalculateTarget(KPIMeasure measure, KPI kpi, List<KPIMeasure> KPIMeasures)
        {
            decimal Target;

            //AS chnages IsConstantTaregt
            if (kpi.IsConstantTargets)
            {
                Target = measure.Target;
            }
            else
            {
                switch (kpi.CalculationMethod)
                {
                    case (int)CalculationMethodsEnum.LastValue:
                        Target = measure.Target;
                        break;
                    case (int)CalculationMethodsEnum.Sum:

                        Target = KPIMeasures.Sum(s => s.Target);
                        break;
                    case (int)CalculationMethodsEnum.Average:
                        List<KPIMeasure> oldFrequenciesMeasures_avg = kpi.KPIMeasures.Where(w => w.ID <= measure.ID).ToList();
                        decimal targetSum_avg = oldFrequenciesMeasures_avg.Sum(s => s.Target);
                        Target = oldFrequenciesMeasures_avg.Any() ? targetSum_avg / oldFrequenciesMeasures_avg.Count() : 0;
                        break;
                    default:
                        Target = 0;
                        break;
                }
            }

            return Target;
        }
        public decimal CalculateAccumulutiveOutOfTarget(KPIMeasure measure, KPI kpi, decimal Target, decimal Value)
        {
            decimal AccumulutiveOutOfTarget;
            //Accumulative values
            if (kpi?.Polarity?.ToLower() == "positive")
            {
                //((Sum of the Actuals Values - Baseline) / (Sum of the Target Values -Baseline)) *100 %
                if (Target - kpi.Baseline != 0)
                    AccumulutiveOutOfTarget = ((Value - kpi.Baseline) / (Target - kpi.Baseline)) * 100;
                else
                    AccumulutiveOutOfTarget = 0;
            }
            else if (kpi?.Polarity?.ToLower() == "negative")
            {
                //based on client commnet 2024074
                //if value=0, baseline=0 , and taregt =0 > 100
                //baseline=0, target=0 >0

                if (kpi.Baseline == 0 && Target == 0 && Value == 0)
                {
                    AccumulutiveOutOfTarget = 100;
                }

                else if (kpi.Baseline == 0 && Target == 0)
                {
                    AccumulutiveOutOfTarget = 0;
                }
                else

                {
                    //1-(( Sum of Actuals - Sum of Targets)/ (Baseline - Sum of Targets)) *100 %
                    if (Target - kpi.Baseline != 0)
                        AccumulutiveOutOfTarget = (1 - (Value - Target) / (Target - kpi.Baseline)) * 100;
                    else
                        AccumulutiveOutOfTarget = 0;
                }
            }
            else
                AccumulutiveOutOfTarget = 0;

            if (AccumulutiveOutOfTarget > 100)
            {
                AccumulutiveOutOfTarget = 100;
            }

            if (AccumulutiveOutOfTarget < 0)
            {
                AccumulutiveOutOfTarget = 0;
            }
            //start Accu
            AccumulutiveOutOfTarget = (decimal)Math.Round(AccumulutiveOutOfTarget, 2);

            return AccumulutiveOutOfTarget;
        }
        private decimal CalculateFormula(string formula, int recursionCounter = 0)
        {

            CalculationResult calculationResult = new CalculationResult();
            if (recursionCounter <= 10)
            {
                calculationResult = Calculator.Calculate(formula);
                if (!calculationResult.IsValid)
                {
                    formula = FixFormula(formula);
                    recursionCounter++;
                    return CalculateFormula(formula, recursionCounter);
                }
                else
                    return Convert.ToDecimal(calculationResult.Result);
            }
            else
            {
                throw new System.Exception("KPI Formula is Invalid");
            }
            if (recursionCounter <= 10)
            {
                recursionCounter++;
                CalculateFormula(formula, recursionCounter);
            }
            else
            {
                throw new System.Exception("KPI Formula is Invalid");
            }
        }
        private string FixFormula(string formula)
        {
            if (!string.IsNullOrWhiteSpace(formula))
            {
                formula = formula.Replace("%", "");
                return formula;
            }
            throw new System.Exception("KPI Formula is Invalid");
        }
        public bool UpdateKPIPeriod_FromMigration()
        {
            bool result = false;
            using (var dataAccess = _factory.Create())
            {

                List<KPI> kpis = dataAccess.KPI.Query().Include(a => a.KPIMeasures).ToList();

                foreach (var item in kpis)
                {
                    foreach (var updatedmeasure in item.KPIMeasures.Where(s => s.Status == "NA"))
                    {
                        if (updatedmeasure.Value.HasValue)
                        {
                            KPI kpi = dataAccess.KPI.Query().Include(a => a.KPIMeasures).Include(a => a.KPIType.KPIThresholds.Select(s => s.Status)).Where(a => a.ID == item.ID && a.KPIMeasures.Where(m => m.ID == updatedmeasure.ID).Count() > 0).FirstOrDefault();
                            KPIMeasure measure = kpi.KPIMeasures.FirstOrDefault(a => a.ID == updatedmeasure.ID);
                            kpi.KPIMeasures = SetActiveMeasure(kpi.KPIMeasures.ToList());
                            // Update Measure Details
                            measure.AllowUpdate = false;
                            measure.Value = updatedmeasure.Value;
                            decimal baseline = kpi.Baseline;
                            //Accumulative values

                            #region Commented
                            //decimal AccumulutiveOutOfTarget = 0;
                            //decimal valuesSum = (oldFrequenciesMeasures.Sum(s => s.Value)) ?? default;
                            //decimal targetSum = oldFrequenciesMeasures.Sum(s => s.Target);

                            //switch (kpi.CalculationMethod)
                            //{
                            //    case (int)CalculationMethodsEnum.LastValue:
                            //        if (kpi?.Polarity?.ToLower() == "positive")
                            //        {
                            //            //( Current Actual - Baseline  / Current Target- Baseline ) * 100 
                            //            if (measure.Target - baseline != 0)
                            //                OutOfTarget = ((updatedmeasure.Value.Value - baseline) / (measure.Target - baseline)) * 100;
                            //            else
                            //                OutOfTarget = 0;
                            //        }
                            //        else if (kpi?.Polarity?.ToLower() == "negative")
                            //        {
                            //            ////Based on Last CR Equation is "1-((Actual-Target)/(Baseline-Target)) * 100"
                            //            //if (baseline - measure.Target != 0)
                            //            //    OutOfTarget = (1 - (updatedmeasure.Value.Value - measure.Target) / (baseline - measure.Target)) * 100;
                            //            //else
                            //            //    OutOfTarget = 0;

                            //            if (baseline == 0 && targetSum == 0 && valuesSum == 0)
                            //            {
                            //                OutOfTarget = 100;
                            //            }
                            //            else if (baseline == 0 && targetSum == 0)
                            //            {
                            //                OutOfTarget = 0;
                            //            }
                            //            else
                            //            {
                            //                //1-(( Sum of Actuals - Sum of Targets)/ (Baseline - Sum of Targets)) *100 %
                            //                if (targetSum - baseline != 0)
                            //                    OutOfTarget = (1 - (valuesSum - targetSum) / (targetSum - baseline)) * 100;
                            //                else
                            //                    OutOfTarget = 0;
                            //            }
                            //        }
                            //        else
                            //        {
                            //            OutOfTarget = 0;
                            //        }
                            //        //Accumulative values
                            //        if (kpi?.Polarity?.ToLower() == "positive")
                            //        {
                            //            //((Sum of the Actuals Values - Baseline) / (Sum of the Target Values -Baseline)) *100 %
                            //            if (targetSum - baseline != 0)
                            //                AccumulutiveOutOfTarget = ((valuesSum - baseline) / (targetSum - baseline)) * 100;
                            //            else
                            //                AccumulutiveOutOfTarget = 0;

                            //        }
                            //        else if (kpi?.Polarity?.ToLower() == "negative")
                            //        {
                            //            //1-(( Sum of Actuals - Sum of Targets)/ (Baseline - Sum of Targets)) *100 %
                            //            //if (baseline - targetSum != 0)
                            //            //    AccumulutiveOutOfTarget = (1 - (valuesSum - targetSum) / (baseline - targetSum)) * 100;
                            //            //else
                            //            //    AccumulutiveOutOfTarget = 0;

                            //            if (baseline == 0 && targetSum == 0 && valuesSum == 0)
                            //            {
                            //                AccumulutiveOutOfTarget = 100;
                            //            }
                            //            else if (baseline == 0 && targetSum == 0)
                            //            {
                            //                AccumulutiveOutOfTarget = 0;
                            //            }
                            //            else
                            //            {
                            //                //1-(( Sum of Actuals - Sum of Targets)/ (Baseline - Sum of Targets)) *100 %
                            //                if (targetSum - baseline != 0)
                            //                    AccumulutiveOutOfTarget = (1 - (valuesSum - targetSum) / (targetSum - baseline)) * 100;
                            //                else
                            //                    AccumulutiveOutOfTarget = 0;
                            //            }
                            //        }
                            //        else
                            //            AccumulutiveOutOfTarget = 0;

                            //        measure.AccumulutiveOutOfTarget = AccumulutiveOutOfTarget;
                            //        measure.AccumulutiveValue = valuesSum;
                            //        measure.AccumulutiveTarget = targetSum;
                            //        break;
                            //    case (int)CalculationMethodsEnum.Sum:

                            //        if (kpi?.Polarity?.ToLower() == "positive")
                            //        {
                            //            //((Sum of the Actuals Values - Baseline) / (Sum of the Target Values -Baseline)) *100 %
                            //            if (targetSum - baseline != 0)
                            //                OutOfTarget = ((valuesSum - baseline) / (targetSum - baseline)) * 100;
                            //            else
                            //                OutOfTarget = 0;

                            //        }
                            //        else if (kpi?.Polarity?.ToLower() == "negative")
                            //        {
                            //            //1-(( Sum of Actuals - Sum of Targets)/ (Baseline - Sum of Targets)) *100 %
                            //            if (baseline == 0 && targetSum == 0 && valuesSum == 0)
                            //            {
                            //                OutOfTarget = 100;
                            //            }
                            //            else if (baseline == 0 && targetSum == 0)
                            //            {
                            //                OutOfTarget = 0;
                            //            }
                            //            else

                            //            {
                            //                //1-(( Sum of Actuals - Sum of Targets)/ (Baseline - Sum of Targets)) *100 %
                            //                if (targetSum - baseline != 0)
                            //                    OutOfTarget = (1 - (valuesSum - targetSum) / (targetSum - baseline)) * 100;
                            //                else
                            //                    OutOfTarget = 0;
                            //            }
                            //        }
                            //        else
                            //        {
                            //            OutOfTarget = 0;
                            //        }

                            //        //Accumulative values
                            //        if (kpi?.Polarity?.ToLower() == "positive")
                            //        {
                            //            //((Sum of the Actuals Values - Baseline) / (Sum of the Target Values -Baseline)) *100 %
                            //            if (targetSum - baseline != 0)
                            //                AccumulutiveOutOfTarget = ((valuesSum - baseline) / (targetSum - baseline)) * 100;
                            //            else
                            //                AccumulutiveOutOfTarget = 0;

                            //        }
                            //        else if (kpi?.Polarity?.ToLower() == "negative")
                            //        {
                            //            //1-(( Sum of Actuals - Sum of Targets)/ (Baseline - Sum of Targets)) *100 %
                            //            if (baseline == 0 && targetSum == 0 && valuesSum == 0)
                            //            {
                            //                AccumulutiveOutOfTarget = 100;
                            //            }
                            //            else if (baseline == 0 && targetSum == 0)
                            //            {
                            //                AccumulutiveOutOfTarget = 0;
                            //            }
                            //            else
                            //            {
                            //                //1-(( Sum of Actuals - Sum of Targets)/ (Baseline - Sum of Targets)) *100 %
                            //                if (targetSum - baseline != 0)
                            //                    AccumulutiveOutOfTarget = (1 - (valuesSum - targetSum) / (targetSum - baseline)) * 100;
                            //                else
                            //                    AccumulutiveOutOfTarget = 0;
                            //            }
                            //        }
                            //        else
                            //            AccumulutiveOutOfTarget = 0;

                            //        measure.AccumulutiveOutOfTarget = AccumulutiveOutOfTarget;
                            //        measure.AccumulutiveValue = valuesSum;
                            //        measure.AccumulutiveTarget = targetSum;
                            //        break;
                            //    case (int)CalculationMethodsEnum.Average:
                            //        List<KPIMeasure> oldFrequenciesMeasures_avg = kpi.KPIMeasures.Where(w => w.ID <= measure.ID).ToList();
                            //        decimal valuesSum_avg = (oldFrequenciesMeasures_avg.Sum(s => s.Value)) ?? default;
                            //        decimal targetSum_avg = oldFrequenciesMeasures_avg.Sum(s => s.Target);

                            //        decimal ValuesAverage = oldFrequenciesMeasures_avg.Any() ? valuesSum_avg / oldFrequenciesMeasures_avg.Count() : 0;
                            //        decimal TargetAverage = oldFrequenciesMeasures_avg.Any() ? targetSum_avg / oldFrequenciesMeasures_avg.Count() : 0;
                            //        if (kpi?.Polarity?.ToLower() == "positive")
                            //        {
                            //            //((Average of the Actuals Values - Baseline) / (Average of the Target Values -Baseline)) *100 %
                            //            if (TargetAverage - baseline != 0)
                            //                OutOfTarget = ((ValuesAverage - baseline) / (TargetAverage - baseline)) * 100;
                            //            else
                            //                OutOfTarget = 0;

                            //        }
                            //        else if (kpi?.Polarity?.ToLower() == "negative")
                            //        {
                            //            //= 1-(( Average of Actuals- Average of Targets)/ (Baseline - Average of Targets)) *100 %
                            //            //if (baseline - TargetAverage != 0)
                            //            //    OutOfTarget = (1 - (ValuesAverage - TargetAverage) / (baseline - TargetAverage)) * 100;
                            //            //else
                            //            //    OutOfTarget = 0;

                            //            if (baseline == 0 && TargetAverage == 0 && ValuesAverage == 0)
                            //            {
                            //                OutOfTarget = 100;
                            //            }
                            //            else if (baseline == 0 && TargetAverage == 0)
                            //            {
                            //                OutOfTarget = 0;
                            //            }
                            //            else
                            //            {
                            //                //= 1-(( Average of Actuals- Average of Targets)/ (Baseline - Average of Targets)) *100 %
                            //                if (TargetAverage - baseline != 0)
                            //                    OutOfTarget = (1 - (ValuesAverage - TargetAverage) / (TargetAverage - baseline)) * 100;
                            //                else
                            //                    OutOfTarget = 0;
                            //            }
                            //        }
                            //        else
                            //        {
                            //            OutOfTarget = 0;
                            //        }

                            //        //Accumulative values
                            //        if (kpi?.Polarity?.ToLower() == "positive")
                            //        {
                            //            //((Average of the Actuals Values - Baseline) / (Average of the Target Values -Baseline)) *100 %
                            //            if (TargetAverage - baseline != 0)
                            //                AccumulutiveOutOfTarget = ((ValuesAverage - baseline) / (TargetAverage - baseline)) * 100;
                            //            else
                            //                AccumulutiveOutOfTarget = 0;

                            //        }
                            //        else if (kpi?.Polarity?.ToLower() == "negative")
                            //        {
                            //            //= 1-(( Average of Actuals- Average of Targets)/ (Baseline - Average of Targets)) *100 %
                            //            //if (baseline - TargetAverage != 0)
                            //            //    AccumulutiveOutOfTarget = (1 - (ValuesAverage - TargetAverage) / (baseline - TargetAverage)) * 100;
                            //            //else
                            //            //    AccumulutiveOutOfTarget = 0;

                            //            if (baseline == 0 && TargetAverage == 0 && ValuesAverage == 0)
                            //            {
                            //                AccumulutiveOutOfTarget = 100;
                            //            }
                            //            else if (baseline == 0 && TargetAverage == 0)
                            //            {
                            //                AccumulutiveOutOfTarget = 0;
                            //            }
                            //            else
                            //            {
                            //                //= 1-(( Average of Actuals- Average of Targets)/ (Baseline - Average of Targets)) *100 %
                            //                if (TargetAverage - baseline != 0)
                            //                    AccumulutiveOutOfTarget = (1 - (ValuesAverage - TargetAverage) / (TargetAverage - baseline)) * 100;
                            //                else
                            //                    AccumulutiveOutOfTarget = 0;
                            //            }
                            //        }
                            //        else
                            //            AccumulutiveOutOfTarget = 0;

                            //        measure.AccumulutiveOutOfTarget = AccumulutiveOutOfTarget;
                            //        measure.AccumulutiveValue = (decimal)Math.Round((double)(ValuesAverage), 2);
                            //        measure.AccumulutiveTarget = (decimal)Math.Round((double)(TargetAverage), 2);
                            //        break;
                            //    default:
                            //        OutOfTarget = 0;
                            //        //
                            //        measure.AccumulutiveOutOfTarget = 0;
                            //        measure.AccumulutiveValue = measure.Value;
                            //        measure.AccumulutiveTarget = measure.Target;
                            //        //
                            //        break;
                            //}
                            #endregion

                            List<KPIMeasure> oldFrequenciesMeasures = kpi.KPIMeasures.Where(w => w.ID <= measure.ID).ToList();

                            decimal Value = CalculatePeriodActual(updatedmeasure, kpi);
                            decimal Target = CalculateTarget(updatedmeasure, kpi, oldFrequenciesMeasures);

                            measure.OutOfTarget = CalculateOutOfTarget(updatedmeasure, kpi, Value);
                            //start Accu
                            measure.AccumulutiveOutOfTarget = CalculateAccumulutiveOutOfTarget(measure, kpi, Value, Target);
                            //end Accu
                            measure.Status = kpi.KPIType.KPIThresholds
                                .Where(w => w.Max != null ? CalculateKPIOperation(measure.OutOfTarget.Value, w.Max.Value, true, w.Operator) : true)
                                .Where(w => w.Min != null ? CalculateKPIOperation(measure.OutOfTarget.Value, w.Min.Value, false, w.MinOperator) : true)
                                .Select(s => s.Code).FirstOrDefault();

                            if (measure.Status == null)
                                measure.Status = "NA";

                            //start Accu
                            measure.AccumulutiveStatus = kpi.KPIType.KPIThresholds
                              .Where(w => w.Max != null ? CalculateKPIOperation(measure.AccumulutiveOutOfTarget.Value, w.Max.Value, true, w.Operator) : true)
                              .Where(w => w.Min != null ? CalculateKPIOperation(measure.AccumulutiveOutOfTarget.Value, w.Min.Value, false, w.MinOperator) : true)
                              .Select(s => s.Code).FirstOrDefault();

                            if (measure.AccumulutiveStatus == null)
                                measure.AccumulutiveStatus = "NA";
                            //end Accu


                            decimal comparer = 0;
                            if (kpi.KPIMeasures.Min(x => x.ID) == measure.ID)
                                comparer = kpi.Baseline;
                            else
                                comparer = kpi.KPIMeasures.Where(w => w.IsActive).FirstOrDefault().Value.Value;

                            //Update KPI Details
                            if (kpi?.Polarity?.ToLower() == "positive")
                            {
                                if (updatedmeasure.Value.Value > comparer)
                                    kpi.Direction = "Up";
                                else if (updatedmeasure.Value.Value < comparer)
                                    kpi.Direction = "Down";
                                else
                                    kpi.Direction = "Same";
                            }
                            else if (kpi?.Polarity?.ToLower() == "negative")
                            {
                                if (updatedmeasure.Value.Value < comparer)
                                    kpi.Direction = "Up";
                                else if (updatedmeasure.Value.Value > comparer)
                                    kpi.Direction = "Down";
                                else
                                    kpi.Direction = "Same";
                            }
                            else
                                kpi.Direction = null;

                            if (kpi.KPIMeasures.Where(a => a.AllowUpdate).Count() == 0)
                                kpi.RequireUpdate = false;
                            dataAccess.KPI.Save(kpi);
                            dataAccess.Complete();

                        }

                    }
                }

                result = true;
            }
            return result;

        }

        public void UpdateMeasureAllowUpdate(int measureID, bool AllowUpdate)
        {
            using (var dataAccess = _factory.Create())
            {
                KPI kpi = dataAccess.KPI.Query().Include(a => a.KPIMeasures).Where(a => a.KPIMeasures.Any(m => m.ID == measureID)).FirstOrDefault();
                KPIMeasure measure = kpi.KPIMeasures.Where(a => a.ID == measureID).FirstOrDefault();
                measure.AllowUpdate = AllowUpdate;
                dataAccess.KPI.Save(kpi);
                dataAccess.Complete();
            }
        }

        public void SetMeasureIsSkipped(int measureID, bool IsSkipped)
        {
            using (var dataAccess = _factory.Create())
            {
                KPI kpi = dataAccess.KPI.Query().Include(a => a.KPIMeasures).Where(a => a.KPIMeasures.Any(m => m.ID == measureID)).FirstOrDefault();
                var measure = kpi.KPIMeasures.Where(a => a.ID == measureID).FirstOrDefault();
               
                var previous = kpi.KPIMeasures
                    .Where(x => x.HasNoTarget != true && x.DueDate < measure.DueDate)
                    .OrderByDescending(x => x.DueDate)
                    .FirstOrDefault(); 
                
                measure.IsSkipped = IsSkipped;
                measure.Status = "NAU";
                measure.OutOfTarget = 0;
                measure.Value = 0;


                measure.AccumulutiveValue = previous?.AccumulutiveValue ?? 0;
                measure.AccumulutiveStatus = "NAU"; 
                measure.AccumulutiveTarget = previous?.AccumulutiveTarget ?? 0; 
                measure.AccumulutiveOutOfTarget = previous?.AccumulutiveOutOfTarget ?? 0; 
                dataAccess.KPI.Save(kpi);
                dataAccess.Complete();
            }
        }

        //public void SetKPIMeasuresNoAchievementSubmitted(int kpiId)
        //{
        //    using (var dataAccess = _factory.Create())
        //    {
        //        KPI kpi = dataAccess.KPI.Query().Include(a => a.KPIMeasures).Where(a => a.ID == kpiId).FirstOrDefault();

        //        var current = kpi.KPIMeasures.FirstOrDefault(s => s.AllowUpdate == true && s.Status == "NA");
                
        //        var previous = kpi.KPIMeasures
        //            .Where(x => x.DueDate < current?.DueDate)
        //            .OrderByDescending(x => x.DueDate)
        //            .FirstOrDefault();

        //        var next = kpi.KPIMeasures
        //            .Where(x => x.DueDate > current?.DueDate)
        //            .OrderBy(x => x.DueDate)
        //            .FirstOrDefault();


        //        foreach (var measure in kpi.KPIMeasures)
        //        {
                   
        //            if(measure.ID == current?.ID)
        //            {
        //                measure.Status = "NAS";
        //                measure.Value = 0;
        //                measure.OutOfTarget = CalculateTarget(measure, kpi, kpi.KPIMeasures.ToList());
        //                measure.AllowUpdate = false;    
        //                measure.AccumulutiveStatus = "NAS";
        //                measure.AccumulutiveValue = previous?.AccumulutiveValue ?? 0;
        //                measure.AccumulutiveOutOfTarget = previous?.AccumulutiveOutOfTarget ?? 0;
        //                measure.AccumulutiveTarget = previous?.AccumulutiveTarget ?? 0;
        //            }

        //            if (measure.ID == next?.ID)
        //            {
        //                measure.AllowUpdate = true;
        //            }
        //        }
              
        //        dataAccess.KPI.Save(kpi);
        //        dataAccess.Complete();
        //    }
        //}

        public KPIMeasure SetKPIMeasuresNoAchievementSubmitted(KPI kpi)
        {
            if(kpi.ID == 4355)
            {

            }
                var autoApproved = false;
                var current = kpi.KPIMeasures.FirstOrDefault(s => s.AllowUpdate == true && s.Status == "NA");

                var previous = kpi.KPIMeasures
                    .Where(x => x.DueDate < current?.DueDate)
                    .OrderByDescending(x => x.DueDate)
                    .FirstOrDefault();

                var next = kpi.KPIMeasures
                    .Where(x => x.DueDate > current?.DueDate)
                    .OrderBy(x => x.DueDate)
                    .FirstOrDefault();


                foreach (var measure in kpi.KPIMeasures)
                {

                    if (measure.ID == current?.ID)
                    {
                        measure.Status = "NAS";
                        measure.Value = 0;
                        measure.OutOfTarget = 0;//CalculateTarget(measure, kpi, kpi.KPIMeasures.ToList());
                        measure.AllowUpdate = false;
                        measure.AccumulutiveStatus = "NAS";
                        measure.AccumulutiveValue = previous?.AccumulutiveValue ?? 0;
                        measure.AccumulutiveOutOfTarget = previous?.AccumulutiveOutOfTarget ?? 0;
                        measure.AccumulutiveTarget = previous?.AccumulutiveTarget ?? 0;
                        measure.NeedRequest = true;

                       
                }

                    if (measure.ID == next?.ID && measure.DueDate.Date <= DateTime.Now.Date && measure.Status == "NA")
                    {
                        measure.AllowUpdate = true;
                    }
                }




            if (!kpi.KPIMeasures.Any(m => m.AllowUpdate && m.Status == "NA"))
            {
                kpi.RequireUpdate = false;
            }
                
            return autoApproved ? current : null;   
        }
        

        private bool CalculateKPIOperation(decimal OutOfTarget, decimal Comparer, bool isMax, string Operator)
        {

            if (isMax)
            {
                switch (Operator)
                {
                    case "<": return OutOfTarget < Comparer;
                    case "<=": return OutOfTarget <= Comparer;
                    case "=": return OutOfTarget == Comparer;
                    default: throw new System.Exception("invalid logic");
                }
            }
            else
            {
                switch (Operator)
                {
                    case "<": return Comparer < OutOfTarget;
                    case "<=": return Comparer <= OutOfTarget;
                    case "=": return Comparer == OutOfTarget;
                    default: throw new System.Exception("invalid logic");
                }
            }


        }


        #endregion

        #region Delete

        private async Task<bool> DeleteAttachment(List<Attachment> Attachment)
        {
            int result = 0;

            using (var dataAccess = _factory.Create())
            {
                List<Attachment> Attachments = await dataAccess.Attachment.Query().Include(a => a.Content).ToListAsync();
                Attachments = Attachments.Where(s => Attachment.Select(x => x.ID).Contains(s.ID)).ToList();
                foreach (var attach in Attachments)
                {
                    dataAccess.Attachment.Delete(attach);
                    result = dataAccess.Complete();
                }
                if (result >= 1)
                    return true;
                else
                    return false;
            }
        }

        public bool Delete(int id)
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                KPI kpi = dataAccess.KPI.Get(id);
                if (kpi != null)
                {
                    kpi = kpi.MapAttachment(dataAccess);
                    if (kpi.Attachments != null && kpi.Attachments.Count() > 0)
                        Task.Run(() => DeleteAttachment(kpi.Attachments));
                    dataAccess.KPI.Delete(kpi);
                    result = dataAccess.Complete();
                    dataAccess.Dispose();
                }
                if (result >= 1)
                    return true;
                else
                    return false;
            }
        }
        public bool DeleteComment(int id)
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                KPIComment kpiComment = dataAccess.KPIComment.Get(id);
                if (kpiComment != null)
                {
                    dataAccess.KPIComment.Delete(kpiComment);
                    result = dataAccess.Complete();
                    dataAccess.Dispose();
                }
                if (result >= 1)
                    return true;
                else
                    return false;
            }
        }

        #endregion


        #region Processes

        public bool UnlockKPI(int id)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                KPI kpi = dataAccess.KPI.Get(id);
                kpi.IsLocked = false;
                kpi.UnlockDate = DateTime.Now.AddMonths(5);
                kpi.ManualUnLock = true;
                dataAccess.KPI.Save(kpi);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                if (result >= 1)
                    return true;
                return false;
            }
        }
        public bool LockKPI(int id)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                KPI kpi = dataAccess.KPI.Get(id);
                kpi.IsLocked = true;
                dataAccess.KPI.Save(kpi);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                if (result >= 1)
                    return true;
                return false;
            }
        }

        #endregion


        public IEnumerable<KPI> GetKPIsActiveMeasures(IEnumerable<KPI> kpis)
        {
            List<KPIMeasure> measures = new List<KPIMeasure>();
            KPIMeasure measure = null;
            foreach (var kpi in kpis)
            {
                kpi.KPIMeasures = SetActiveMeasure(kpi.KPIMeasures.ToList());
                kpi.Status = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.Status;
                //kpi.KPIMeasures = kpi.KPIMeasures.Where(w => w.IsActive).ToList();
            }
            return kpis;

        }

        public KPI GetKPIActiveMeasures(KPI kpi)
        {


            kpi.KPIMeasures = SetActiveMeasure(kpi.KPIMeasures.ToList());
            //kpi.KPIMeasures = kpi.KPIMeasures.Where(w => w.IsActive).ToList();
            return kpi;

        }

        private List<KPIMeasure> SetActiveMeasure(List<KPIMeasure> KPIMeasures)
        {
            if (KPIMeasures != null && KPIMeasures.Count() > 0)
            {
                if (KPIMeasures.OrderBy(o => o.ID).Where(w => w.Status != "NA").Count() > 0)
                {
                    KPIMeasures.OrderBy(o => o.ID).Where(w => w.Status != "NA").LastOrDefault().IsActive = true;
                }
                else
                {
                    KPIMeasures.OrderBy(o => o.ID).FirstOrDefault().IsActive = true;
                }
            }

            KPIMeasures.ForEach(a => a.IsEditable = a.DueDate.Date > DateTime.Now.Date);
            return KPIMeasures;

        }


        #region Hangfire
        public List<KPI> ReadForHangfire()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPI> kpi = dataAccess.KPI.Query()
                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimized(a => a.StrategicObjective)
                    .ToList();

                foreach (var item in kpi)
                {
                    item.KPIMeasures = item.KPIMeasures.Where(x => x.HasNoTarget != true).ToList() ?? new List<KPIMeasure>();
                }

                return kpi.ToList();
            }
        }
        public KPI ReadForHangfireByID(int id)
        {
            using (var dataAccess = _factory.Create())
            {
                KPI kpi = dataAccess.KPI.Query()
                    .IncludeOptimized(a => a.KPIMeasures)
                    .IncludeOptimized(a => a.KPIType)
                    .IncludeOptimized(a => a.StrategicObjective)
                    .Where(s => s.ID == id).FirstOrDefault();

                kpi.KPIMeasures = kpi.KPIMeasures.Where(x => x.HasNoTarget != true).ToList() ?? new List<KPIMeasure>();



                return kpi;
            }
        }
        public void OpenKPIForUpdate(List<KPI> kpis)
        {
            using (var dataAccess = _factory.Create())
            {
                foreach (KPI kpi in kpis)
                {
                    var CurrentKPI = dataAccess.KPI.Query().Where(w => w.ID == kpi.ID).FirstOrDefault();
                    if (CurrentKPI.Modified > kpi.Modified)
                    {
                        continue;
                    }
                    dataAccess.KPI.Save(kpi);
                    
                    foreach (KPIMeasure item in kpi.KPIMeasures)
                    {
                        dataAccess.KPIMeasure.Save(item);
                    }
                }

                var result = dataAccess.Complete();
            }
        }

        public void LockUnlockKPIs(List<KPI> kpis)
        {
            using (var dataAccess = _factory.Create())
            {
                //List<KPI> kpis = dataAccess.KPI.Query().IncludeOptimized(a => a.KPIType).IncludeOptimized(a => a.KPIMeasures).ToList();
                foreach (KPI kpi in kpis)
                {
                    //if (IsInGracePeriod(kpi))
                    //    kpi.IsLocked = false;
                    //else if (kpi.ManualUnLock)
                    //    kpi.IsLocked = false;
                    //else
                    //    kpi.IsLocked = true;
                    dataAccess.KPI.Save(kpi);
                }
                var result = dataAccess.Complete();
            }

        }

        public void LockUnlockKPI(KPI kpi)
        {
            using (var dataAccess = _factory.Create())
            {
                dataAccess.KPI.Save(kpi);
                var result = dataAccess.Complete();
            }

        }
        public List<KPI> GetKPIsForReminder()
        {
            using (var dataAccess = _factory.Create())
            {
                List<KPI> kpis = dataAccess.KPI.Query().Include(a => a.KPIType).Include(a => a.KPIType.ReminderConfiguration).Include(a => a.KPIMeasures).ToList();
                return kpis;
            }
        }
        #endregion

        private KPI SetCanUpdate(KPI kpi, string UserName)
        {
            if (kpi.KPIMeasures.Any(a => a.AllowUpdate))
            {
                if (kpi.Champion.ToLower() == UserName.ToLower() && kpi.RequireUpdate)
                {
                    kpi.CanUpdate = true;
                }
            }
            return kpi;
        }
        private IEnumerable<KPI> SetCanUpdate(IEnumerable<KPI> kpis, string UserName)
        {
            foreach (var kpi in kpis)
            {
                if (kpi.KPIMeasures.Any(a => a.AllowUpdate))
                {
                    if (kpi.Champion.ToLower() == UserName.ToLower() && kpi.RequireUpdate)
                    {
                        kpi.CanUpdate = true;
                    }
                }
            }

            return kpis;
        }

        public bool IsInGracePeriod(KPI kpi, List<DateTime> holidays = default)
        {
            holidays = holidays ?? new List<DateTime>();
            if (kpi.KPIType.GracePeriod == 0)
                return true;
            else
            {
                KPIMeasure currentMeasure = kpi.KPIMeasures.Where(m=> m.HasNoTarget != true).OrderBy(a => a.ID).Where(a => a.DueDate.Date <= DateTime.Now.Date && a.Status == "NA").LastOrDefault();
                if (currentMeasure != null)
                {
                    var GetEndDateWorkingDays = DateHelper.GetEndDateWorkingDays(currentMeasure.DueDate.Date, kpi.KPIType.GracePeriod, holidays);
                    if (DateTime.Now.Date >= currentMeasure.DueDate.Date && DateTime.Now.Date <= GetEndDateWorkingDays.Date)
                        return true;
                    else
                        return false;
                }
                return false;
            }
        }

        public bool IsInGracePeriodUpdated(KPI kpi, List<DateTime> holidays = default)
        {
            holidays = holidays ?? new List<DateTime>();
            if (kpi.KPIType.GracePeriod == 0)
                return true;
            else
            {
                KPIMeasure currentMeasure = kpi.KPIMeasures.OrderBy(a => a.ID)
                    .Where(a => a.HasNoTarget != true && a.AllowUpdate == true && a.DueDate.Date <= DateTime.Now.Date && a.Status == "NA").FirstOrDefault();
                if (currentMeasure != null)
                {
                    var GetEndDateWorkingDays = DateHelper.GetEndDateWorkingDays(currentMeasure.DueDate.Date, kpi.KPIType.GracePeriod, holidays);
                    if (DateTime.Now.Date >= currentMeasure.DueDate.Date && DateTime.Now.Date <= GetEndDateWorkingDays.Date)
                        return true;
                    else
                        return false;
                }

                return false;
            }
        }

        public bool IsInGracePeriod_Rejected(DateTime rejectedDate, KPI kpi , List<DateTime> holidays)
        {
            
            if (kpi.KPIType.GracePeriod == 0)
                return true;
            else
            {
                // KPIMeasure currentMeasure = kpi.KPIMeasures.OrderBy(a => a.ID).Where(a => a.DueDate.Date <= DateTime.Now.Date).LastOrDefault();
                // if (currentMeasure != null)
                {
                    var GetEndDateWorkingDays = DateHelper.GetEndDateWorkingDays(rejectedDate.Date, 2 , holidays);

                    DateTime endOfDay = GetEndDateWorkingDays.Date.AddDays(1).AddTicks(-1);

                    if (DateTime.Now.Date >= rejectedDate.Date && DateTime.Now.Date <= endOfDay )
                        return true;
                    else
                        return false;

                    //if (DateTime.Now >= rejectedDate.Date && DateTime.Now<= GetEndDateWorkingDays.Date)
                    //    return true;
                    //else
                    //    return false;
                }
                return false;
            }
        }

        private void MapKPIProperties(KPI kpi, List<KPI> kpis, List<OrgStructure> orgStructures, List<DivisionalObjective> divisionalObjectives, bool calculateCode = true)
        {
            kpi.KPIMeasures = SetActiveMeasure(kpi.KPIMeasures.Where(m=> m.HasNoTarget != true).ToList());
            if (kpi.Direction == null)
                kpi.Direction = "Same";
            kpi.Target = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.Target ?? 0;

            //Based on the meeting with Samman Hamzah if calculation method is Last value the accumulative will be same as periodic
            //This change is made based on a github item opened by the client after a year of applying the "Last Change on formula"
            // 3/DEC/2024
            if (kpi.CalculationMethod == (int)CalculationMethodsEnum.LastValue)
            {
                kpi.KPIMeasures.ToList().ForEach(f =>
                {
                    if(f.Status != "NAS" && f.Status != "NAU")
                    {
                        f.AccumulutiveOutOfTarget = f.OutOfTarget;
                        f.AccumulutiveValue = f.Value;
                        f.AccumulutiveTarget = f.Target;
                        f.AccumulutiveStatus = f.Status;
                    }
                });
            }

            //Changed Status to always take AccumulutiveStatus to effect all system without changing FE on all system, if you want the Periodic status you need to access it from the KPI Measures 
            kpi.Status = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.AccumulutiveStatus ?? "NA";
            kpi.AccumulutiveStatus = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.AccumulutiveStatus ?? "NA";
            kpi.OutOfTarget = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.OutOfTarget ?? 0;

            if (kpi.OutOfTarget > 100)
            {
                kpi.OutOfTarget = 100;
            }
            else if (kpi.OutOfTarget < 0)
            {
                kpi.OutOfTarget = 0;
            }
            kpi.AccumulutiveOutOfTarget = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.AccumulutiveOutOfTarget ?? 0;

            if (kpi.AccumulutiveOutOfTarget > 100)
            {
                kpi.AccumulutiveOutOfTarget = 100;
            }
            else if (kpi.AccumulutiveOutOfTarget < 0)
            {
                kpi.AccumulutiveOutOfTarget = 0;
            }
            kpi.Value = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.Value ?? 0;

            kpi.AccumulutiveTarget = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.AccumulutiveTarget ?? 0;
            kpi.AccumulutiveValue = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.AccumulutiveValue ?? 0;

            if (calculateCode)
                CreateKPICode(kpi, kpis, orgStructures, divisionalObjectives);
        }

        private void MapKPIPropertiesAdmin(KPI kpi, List<KPI> kpis, List<OrgStructure> orgStructures, List<DivisionalObjective> divisionalObjectives, bool calculateCode = true)
        {
            kpi.KPIMeasures = SetActiveMeasure(kpi.KPIMeasures.ToList());
            if (kpi.Direction == null)
                kpi.Direction = "Same";
            kpi.Target = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.Target ?? 0;

            //Based on the meeting with Samman Hamzah if calculation method is Last value the accumulative will be same as periodic
            //This change is made based on a github item opened by the client after a year of applying the "Last Change on formula"
            // 3/DEC/2024
            if (kpi.CalculationMethod == (int)CalculationMethodsEnum.LastValue)
            {
                kpi.KPIMeasures.ToList().ForEach(f =>
                {
                    if (f.Status != "NAS" && f.Status != "NAU")
                    {
                        f.AccumulutiveOutOfTarget = f.OutOfTarget;
                        f.AccumulutiveValue = f.Value;
                        f.AccumulutiveTarget = f.Target;
                        f.AccumulutiveStatus = f.Status;
                    }
                });
            }

            //Changed Status to always take AccumulutiveStatus to effect all system without changing FE on all system, if you want the Periodic status you need to access it from the KPI Measures 
            kpi.Status = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.AccumulutiveStatus ?? "NA";
            kpi.AccumulutiveStatus = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.AccumulutiveStatus ?? "NA";
            kpi.OutOfTarget = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.OutOfTarget ?? 0;

            if (kpi.OutOfTarget > 100)
            {
                kpi.OutOfTarget = 100;
            }
            else if (kpi.OutOfTarget < 0)
            {
                kpi.OutOfTarget = 0;
            }
            kpi.AccumulutiveOutOfTarget = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.AccumulutiveOutOfTarget ?? 0;

            if (kpi.AccumulutiveOutOfTarget > 100)
            {
                kpi.AccumulutiveOutOfTarget = 100;
            }
            else if (kpi.AccumulutiveOutOfTarget < 0)
            {
                kpi.AccumulutiveOutOfTarget = 0;
            }
            kpi.Value = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.Value ?? 0;

            kpi.AccumulutiveTarget = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.AccumulutiveTarget ?? 0;
            kpi.AccumulutiveValue = kpi.KPIMeasures.Where(a => a.IsActive).LastOrDefault()?.AccumulutiveValue ?? 0;

            if (calculateCode)
                CreateKPICode(kpi, kpis, orgStructures, divisionalObjectives);
        }


        public void CreateKPICode(KPI kpi, List<KPI> kpis, List<OrgStructure> orgStructures, List<DivisionalObjective> divisionalObjectives)
        {
            if (!kpi.IsCoded)
            {
                // using (var dataAccess = _factory.Create())
                {
                    int? orgStructureId = kpi.OrgStructureID;

                    //if (!orgStructures.Any())
                    //    orgStructures = dataAccess.OrgStructure.Query().AsNoTracking().ToList();

                    //if (!kpis.Any())
                    //    kpis = dataAccess.KPI.Query().IncludeOptimized(a => a.OrgStructure).OrderBy(o => o.ID).AsNoTracking().ToList();


                    if (orgStructureId.HasValue && kpis != null)
                    {
                        //if (!divisionalObjectives.Any())
                        //    divisionalObjectives = dataAccess.DivisionalObjective
                        //          .Query()
                        //          .IncludeOptimized(a => a.OrgStructure).AsNoTracking()
                        //          .ToList();

                        List<KPI> filteredKPIs = kpis
                            .Where(w => w.OrgStructureID == orgStructureId.Value ||
                                        divisionalObjectives.Any(a => w.DivisionalObjectiveID == a.ID && a.OrgStructureId == orgStructureId.Value))
                            .ToList();

                        OrgStructure orgStructure = orgStructures.FirstOrDefault(w => w.ID == orgStructureId.Value);

                        if (orgStructure != null)
                        {
                            orgStructure.Parent = orgStructures.FirstOrDefault(w => w.ID == orgStructure.ParentID);
                            int level = (orgStructure.ID == Constants.CorporateDepartmentID ||
                                         orgStructure.ID == Constants.CorporateSectortID) ? 1 :
                                        (orgStructure.ParentID.HasValue && orgStructure.Parent != null) ? 3 : 2;

                            int kpiMaxNumber = filteredKPIs.FindIndex(k => k.ID == kpi.ID);
                            string CodeIndex = (kpiMaxNumber + 1).ToString("00");
                            switch (level)
                            {
                                case 1:
                                    if (!orgStructure.ParentID.HasValue)
                                        kpi.Code = $"L{level}.{orgStructure.Abbreviation}.{CodeIndex}";
                                    else
                                        kpi.Code = $"L{level}.{orgStructure.Parent.Abbreviation}.{CodeIndex}";
                                    break;
                                case 2:
                                    kpi.Code = $"L{level}.{orgStructure.Abbreviation}.{CodeIndex}";
                                    break;

                                case 3:
                                    kpi.Code = $"L{level}.{orgStructure.Parent.Abbreviation}.{orgStructure.Abbreviation}.{CodeIndex}";
                                    break;

                                default:
                                    break;
                            }
                        }
                        
                        kpi.IsCoded = true;
                    }
                    else if (kpi.DivisionalObjectiveID.HasValue && kpis != null)
                    {
                        //if (!divisionalObjectives.Any())
                        //    divisionalObjectives = dataAccess.DivisionalObjective
                        //        .Query()
                        //        .IncludeOptimized(a => a.OrgStructure).AsNoTracking()
                        //        .ToList();

                        DivisionalObjective divisionalObjective = divisionalObjectives.FirstOrDefault(w => w.ID == kpi.DivisionalObjectiveID);

                        if (divisionalObjective != null)
                        {
                            OrgStructure orgStructure = orgStructures.FirstOrDefault(w => w.ID == divisionalObjective.OrgStructureId);

                            if (orgStructure != null)
                            {
                                orgStructure.Parent = orgStructures.FirstOrDefault(w => w.ID == orgStructure.ParentID);

                                int level = (orgStructure.ID == Constants.CorporateDepartmentID ||
                                             orgStructure.ID == Constants.CorporateSectortID) ? 1 :
                                            (orgStructure.ParentID.HasValue && orgStructure.Parent != null) ? 3 : 2;

                                List<KPI> filteredKPIs = kpis
                                    .Where(w => w.OrgStructureID == orgStructure.ID ||
                                                divisionalObjectives.Any(a => w.DivisionalObjectiveID == a.ID && a.OrgStructureId == orgStructure.ID))
                                    .ToList();

                                int kpiMaxNumber = filteredKPIs.FindIndex(k => k.ID == kpi.ID);
                                string CodeIndex = (kpiMaxNumber + 1).ToString("00");
                                switch (level)
                                {
                                    case 1:
                                        if (!orgStructure.ParentID.HasValue)
                                            kpi.Code = $"L{level}.{orgStructure.Abbreviation}.{CodeIndex}";
                                        else
                                            kpi.Code = $"L{level}.{orgStructure.Parent.Abbreviation}.{CodeIndex}";
                                        break;
                                    case 2:
                                        kpi.Code = $"L{level}.{orgStructure.Abbreviation}.{CodeIndex}";
                                        break;

                                    case 3:
                                        kpi.Code = $"L{level}.{orgStructure.Parent.Abbreviation}.{orgStructure.Abbreviation}.{CodeIndex}";
                                        break;

                                    default:
                                        break;
                                }
                            }
                            kpi.IsCoded = true;
                        }
                    }
                }
            }
        }

        //public void CreateKPICode(List<KPI> kpis)
        //{
        //    foreach (var kpi in kpis)
        //    {
        //        CreateKPICode(kpi);
        //    }
        //}

        public void MapKPIProperties(List<KPI> kpis, bool CalculateCode = true)
        {
            kpis = GetKPIsActiveMeasures(kpis).ToList();


            List<KPI> AllKPIs = new List<KPI>();
            List<OrgStructure> AllOrgStructures = new List<OrgStructure>();
            List<DivisionalObjective> DivisionalObjectives = new List<DivisionalObjective>();

            if (CalculateCode)
            {
                AllKPIs = ReadAll();
                AllOrgStructures = GetOrgStructures();
                DivisionalObjectives = GetDivisionalObjectives();
            }

            foreach (var kpi in kpis)
            {
                kpi.KPIMeasures= kpi.KPIMeasures.Where(x=> x.HasNoTarget != true).ToList(); 
                MapKPIProperties(kpi, AllKPIs, AllOrgStructures, DivisionalObjectives);
            }


        }


        public void MapKPIPropertiesAdmin(List<KPI> kpis, bool CalculateCode = true)
        {
            kpis = GetKPIsActiveMeasures(kpis).ToList();


            List<KPI> AllKPIs = new List<KPI>();
            List<OrgStructure> AllOrgStructures = new List<OrgStructure>();
            List<DivisionalObjective> DivisionalObjectives = new List<DivisionalObjective>();

            if (CalculateCode)
            {
                AllKPIs = ReadAll();
                AllOrgStructures = GetOrgStructures();
                DivisionalObjectives = GetDivisionalObjectives();
            }

            foreach (var kpi in kpis)
            {
                MapKPIPropertiesAdmin(kpi, AllKPIs, AllOrgStructures, DivisionalObjectives);
            }


        }

        //private void MapKPIProperties_Prev(List<KPI> kpis)
        //{
        //    kpis = GetKPIsActiveMeasures(kpis).ToList();
        //    foreach (var kpi in kpis)
        //    {
        //        if (kpi.Direction == null)
        //            kpi.Direction = "Same";

        //    //    kpi.KPIMeasures.Where(w => w.Status != "NA" && w.DueDate.Date<DateTime.Now.Date).OrderByDescending(a => a.DueDate).FirstOrDefault()

        //        kpi.Target = kpi.KPIMeasures.Where(w => w.Status != "NA" && !w.IsActive).OrderByDescending(a => a.DueDate).FirstOrDefault()?.Target ?? 0;
        //        //Changed Status to always take AccumulutiveStatus to effect all system without changing FE on all system, if you want the Periodic status you need to access it from the KPI Measures 
        //        kpi.Status = kpi.KPIMeasures.Where(w => w.Status != "NA" && !w.IsActive).OrderByDescending(a => a.DueDate).FirstOrDefault()?.AccumulutiveStatus ?? "NA";
        //        kpi.AccumulutiveStatus = kpi.KPIMeasures.Where(w => w.Status != "NA" && !w.IsActive).OrderByDescending(a => a.DueDate).FirstOrDefault()?.AccumulutiveStatus ?? "NA";
        //        kpi.OutOfTarget = kpi.KPIMeasures.Where(w => w.Status != "NA" && !w.IsActive).OrderByDescending(a => a.DueDate).FirstOrDefault()?.OutOfTarget ?? 0;
        //        if (kpi.OutOfTarget > 100)
        //        {
        //            kpi.OutOfTarget = 100;
        //        }
        //        else if (kpi.OutOfTarget < 0)
        //        {
        //            kpi.OutOfTarget = 0;
        //        }
        //        kpi.AccumulutiveOutOfTarget = kpi.KPIMeasures.Where(w => w.Status != "NA" && !w.IsActive).OrderByDescending(a => a.DueDate).FirstOrDefault()?.AccumulutiveOutOfTarget ?? 0;
        //        if (kpi.AccumulutiveOutOfTarget > 100)
        //        {
        //            kpi.AccumulutiveOutOfTarget = 100;
        //        }
        //        else if (kpi.AccumulutiveOutOfTarget < 0)
        //        {
        //            kpi.AccumulutiveOutOfTarget = 0;
        //        }
        //        kpi.Value = kpi.KPIMeasures.Where(w => w.Status != "NA" && !w.IsActive).OrderByDescending(a => a.DueDate).FirstOrDefault()?.Value ?? 0;

        //    }
        //}

        private void MapKPIProperties_Prev(List<KPI> kpis)
        {
            kpis = GetKPIsActiveMeasures(kpis).ToList();
            foreach (var kpi in kpis)
            {
                if (kpi.Direction == null)
                    kpi.Direction = "Same";

                //    kpi.KPIMeasures.Where(w => w.Status != "NA" && w.DueDate.Date<DateTime.Now.Date).OrderByDescending(a => a.DueDate).FirstOrDefault()

                int kpiYear = kpi.StartDate.Year;
                kpi.Target = kpi.KPIMeasures.Where(w => w.Status != "NA" && w.DueDate.Month < (kpiYear < DateTime.Now.Year ? new DateTime(kpiYear, 12, 31).Month : DateTime.Now.Month)).OrderByDescending(a => a.DueDate).FirstOrDefault()?.Target ?? 0;
                //Changed Status to always take AccumulutiveStatus to effect all system without changing FE on all system, if you want the Periodic status you need to access it from the KPI Measures 
                kpi.Status = kpi.KPIMeasures.Where(w => w.Status != "NA" && w.DueDate.Month < (kpiYear < DateTime.Now.Year ? new DateTime(kpiYear, 12, 31).Month : DateTime.Now.Month)).OrderByDescending(a => a.DueDate).FirstOrDefault()?.AccumulutiveStatus ?? "NA";
                kpi.AccumulutiveStatus = kpi.KPIMeasures.Where(w => w.Status != "NA" && w.DueDate.Month < (kpiYear < DateTime.Now.Year ? new DateTime(kpiYear, 12, 31).Month : DateTime.Now.Month)).OrderByDescending(a => a.DueDate).FirstOrDefault()?.AccumulutiveStatus ?? "NA";
                kpi.OutOfTarget = kpi.KPIMeasures.Where(w => w.Status != "NA" && w.DueDate.Month < (kpiYear < DateTime.Now.Year ? new DateTime(kpiYear, 12, 31).Month : DateTime.Now.Month)).OrderByDescending(a => a.DueDate).FirstOrDefault()?.OutOfTarget ?? 0;
                if (kpi.OutOfTarget > 100)
                {
                    kpi.OutOfTarget = 100;
                }
                else if (kpi.OutOfTarget < 0)
                {
                    kpi.OutOfTarget = 0;
                }
                kpi.AccumulutiveOutOfTarget = kpi.KPIMeasures.Where(w => w.Status != "NA" && w.DueDate.Month < (kpiYear < DateTime.Now.Year ? new DateTime(kpiYear, 12, 31).Month : DateTime.Now.Month)).OrderByDescending(a => a.DueDate).FirstOrDefault()?.AccumulutiveOutOfTarget ?? 0;
                if (kpi.AccumulutiveOutOfTarget > 100)
                {
                    kpi.AccumulutiveOutOfTarget = 100;
                }
                else if (kpi.AccumulutiveOutOfTarget < 0)
                {
                    kpi.AccumulutiveOutOfTarget = 0;
                }
                kpi.Value = kpi.KPIMeasures.Where(w => w.Status != "NA" && w.DueDate.Month < (kpiYear < DateTime.Now.Year ? new DateTime(kpiYear, 12, 31).Month : DateTime.Now.Month)).OrderByDescending(a => a.DueDate).FirstOrDefault()?.Value ?? 0;

            }
        }

        //private void MapKPIProperties_Prev(List<KPI> kpis)
        //{
        //    kpis = GetKPIsActiveMeasures(kpis).ToList();
        //    foreach (var kpi in kpis)
        //    {
        //        if (kpi.Direction == null)
        //            kpi.Direction = "Same";

        //        //    kpi.KPIMeasures.Where(w => w.Status != "NA" && w.DueDate.Date<DateTime.Now.Date).OrderByDescending(a => a.DueDate).FirstOrDefault()

        //        kpi.Target = kpi.KPIMeasures.Where(w => w.Status != "NA" && w.DueDate.Month < DateTime.Now.Month).OrderByDescending(a => a.DueDate).FirstOrDefault()?.Target ?? 0;
        //        //Changed Status to always take AccumulutiveStatus to effect all system without changing FE on all system, if you want the Periodic status you need to access it from the KPI Measures 
        //        kpi.Status = kpi.KPIMeasures.Where(w => w.Status != "NA" && w.DueDate.Month < DateTime.Now.Month).OrderByDescending(a => a.DueDate).FirstOrDefault()?.AccumulutiveStatus ?? "NA";
        //        kpi.AccumulutiveStatus = kpi.KPIMeasures.Where(w => w.Status != "NA" && w.DueDate.Month < DateTime.Now.Month).OrderByDescending(a => a.DueDate).FirstOrDefault()?.AccumulutiveStatus ?? "NA";
        //        kpi.OutOfTarget = kpi.KPIMeasures.Where(w => w.Status != "NA" && w.DueDate.Month < DateTime.Now.Month).OrderByDescending(a => a.DueDate).FirstOrDefault()?.OutOfTarget ?? 0;
        //        if (kpi.OutOfTarget > 100)
        //        {
        //            kpi.OutOfTarget = 100;
        //        }
        //        else if (kpi.OutOfTarget < 0)
        //        {
        //            kpi.OutOfTarget = 0;
        //        }
        //        kpi.AccumulutiveOutOfTarget = kpi.KPIMeasures.Where(w => w.Status != "NA" && w.DueDate.Month < DateTime.Now.Month).OrderByDescending(a => a.DueDate).FirstOrDefault()?.AccumulutiveOutOfTarget ?? 0;
        //        if (kpi.AccumulutiveOutOfTarget > 100)
        //        {
        //            kpi.AccumulutiveOutOfTarget = 100;
        //        }
        //        else if (kpi.AccumulutiveOutOfTarget < 0)
        //        {
        //            kpi.AccumulutiveOutOfTarget = 0;
        //        }
        //        kpi.Value = kpi.KPIMeasures.Where(w => w.Status != "NA" && w.DueDate.Month < DateTime.Now.Month).OrderByDescending(a => a.DueDate).FirstOrDefault()?.Value ?? 0;

        //    }
        //}
        public decimal? CalculateKPIPerformace(List<KPI> kpis)
        {
            decimal? performance;
            MapKPIProperties(kpis, false);
            if (kpis.Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA")).Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA")).Sum(s => s.Weight) > 0)
                performance = Math.Round(kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA")).Sum(s => (s.AccumulutiveOutOfTarget) * s.Weight) / kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA")).Sum(s => s.Weight), 2);
            else
                performance = null;
            return performance;
        }
        public decimal? CalculateDepartmentalKPIPerformace(List<KPI> kpis)
        {
            decimal? performance;
            MapKPIProperties(kpis, false);
            if (kpis.Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA")).Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA")).Sum(s => s.BusinessUnitWeight) > 0)
                performance = Math.Round(kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA")).Sum(s => (s.AccumulutiveOutOfTarget) * s.BusinessUnitWeight) / kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA")).Sum(s => s.BusinessUnitWeight), 2);
            else
                performance = null;
            return performance;
        }
        public decimal? CalculateDepartmentalKPIPeriodicPerformace(List<KPI> kpis)
        {
            decimal? performance;
            MapKPIProperties(kpis, false);
            if (kpis.Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA")).Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA")).Sum(s => s.BusinessUnitWeight) > 0)
                performance = Math.Round(kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA")).Sum(s => (s.OutOfTarget) * s.BusinessUnitWeight) / kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA")).Sum(s => s.BusinessUnitWeight), 2);
            else
                performance = null;
            return performance;
        }

        public decimal? CalculateKPIPerformaceForTheme(List<KPI> kpis, List<StrategicObjective> strategicObjectives)
        {
            decimal? ThemePerformance;

            decimal StrategicObjectiveSummedPerformance = 0;
            foreach (var strategicObjective in strategicObjectives)
            {
                List<KPI> StrategicObjectiveKPIs = kpis.Where(w => w.StrategicObjectiveID == strategicObjective.ID).ToList();
                decimal? StrategicObjectivePerformance = CalculateKPIPerformace(StrategicObjectiveKPIs);

                strategicObjective.Performance = StrategicObjectivePerformance ?? 0;
                if (StrategicObjectivePerformance.HasValue)
                {
                    StrategicObjectiveSummedPerformance += strategicObjective.Performance * strategicObjective.Weight;
                }
            }
            ThemePerformance = StrategicObjectiveSummedPerformance / 100;

            return ThemePerformance;
        }

        //public decimal? CalculateKPIPerformace_Prev(List<KPI> kpis)
        //{
        //    decimal? performance;
        //    MapKPIProperties_Prev(kpis);
        //    if (kpis.Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && !a.IsActive)).Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && !a.IsActive)).Sum(s => s.Weight) > 0)
        //        performance = Math.Round(kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && !a.IsActive)).Sum(s => (s.AccumulutiveOutOfTarget) * s.Weight) / kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && !a.IsActive)).Sum(s => s.Weight), 2);

        //    else
        //        performance = null;
        //    return performance;
        //}
        //public decimal? CalculateDepartmentalKPIPerformace_Prev(List<KPI> kpis)
        //{
        //    decimal? performance;
        //    MapKPIProperties_Prev(kpis);
        //    if (kpis.Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && !a.IsActive)).Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && !a.IsActive)).Sum(s => s.BusinessUnitWeight) > 0)
        //        performance = Math.Round(kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && !a.IsActive)).Sum(s => (s.AccumulutiveOutOfTarget) * s.BusinessUnitWeight) / kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && !a.IsActive)).Sum(s => s.BusinessUnitWeight), 2);

        //    else
        //        performance = null;
        //    return performance;
        //}
        //public decimal? CalculateKPIPerformace_Prev(List<KPI> kpis)
        //{
        //    decimal? performance;
        //    MapKPIProperties_Prev(kpis);
        //    if (kpis.Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" &&a.DueDate.Month < DateTime.Now.Month)).Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && a.DueDate.Month < DateTime.Now.Month)).Sum(s => s.Weight) > 0)
        //        performance = Math.Round(kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && a.DueDate.Month < DateTime.Now.Month)).Sum(s => (s.AccumulutiveOutOfTarget) * s.Weight) / kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && a.DueDate.Month < DateTime.Now.Month)).Sum(s => s.Weight), 2);

        //    else
        //        performance = null;
        //    return performance;
        //}


        public decimal? CalculateKPIPerformace_Prev(List<KPI> kpis)
        {
            decimal? performance;
            MapKPIProperties_Prev(kpis);
            if (kpis.Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && a.DueDate.Month < (w.StartDate.Year < DateTime.Now.Year ? new DateTime(w.StartDate.Year, 12, 31).Month : DateTime.Now.Month))).Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && a.DueDate.Month < (w.StartDate.Year < DateTime.Now.Year ? new DateTime(w.StartDate.Year, 12, 31).Month : DateTime.Now.Month))).Sum(s => s.Weight) > 0)
                performance = Math.Round(kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && a.DueDate.Month < (w.StartDate.Year < DateTime.Now.Year ? new DateTime(w.StartDate.Year, 12, 31).Month : DateTime.Now.Month))).Sum(s => (s.AccumulutiveOutOfTarget) * s.Weight)
  / kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && a.DueDate.Month < (w.StartDate.Year < DateTime.Now.Year ? new DateTime(w.StartDate.Year, 12, 31).Month : DateTime.Now.Month))).Sum(s => s.Weight), 2);
            // kpis.Sum(s => s.Weight), 2);
            else
                performance = null;
            return performance;
        }
        public decimal? CalculateDepartmentalKPIPerformace_Prev(List<KPI> kpis)
        {
            decimal? performance;
            MapKPIProperties_Prev(kpis);

            if (kpis.Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && a.DueDate.Month < (w.StartDate.Year < DateTime.Now.Year ? new DateTime(w.StartDate.Year, 12, 31).Month : DateTime.Now.Month))).Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && a.DueDate.Month < (w.StartDate.Year < DateTime.Now.Year ? new DateTime(w.StartDate.Year, 12, 31).Month : DateTime.Now.Month))).Sum(s => s.BusinessUnitWeight) > 0)
                performance = Math.Round(kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && a.DueDate.Month < (w.StartDate.Year < DateTime.Now.Year ? new DateTime(w.StartDate.Year, 12, 31).Month : DateTime.Now.Month))).Sum(s => (s.AccumulutiveOutOfTarget) * s.BusinessUnitWeight)
                / kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && a.DueDate.Month < (w.StartDate.Year < DateTime.Now.Year ? new DateTime(w.StartDate.Year, 12, 31).Month : DateTime.Now.Month))).Sum(s => s.BusinessUnitWeight), 2);

            //kpis.Sum(s => s.BusinessUnitWeight), 2);

            else
                performance = null;
            return performance;
        }
        //public decimal? CalculateDepartmentalKPIPerformace_Prev(List<KPI> kpis)
        //{
        //    decimal? performance;
        //    MapKPIProperties_Prev(kpis);
        //    if (kpis.Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && a.DueDate.Month < DateTime.Now.Month)).Count() > 0 && kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && a.DueDate.Month < DateTime.Now.Month)).Sum(s => s.BusinessUnitWeight) > 0)
        //        performance = Math.Round(kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && a.DueDate.Month < DateTime.Now.Month)).Sum(s => (s.AccumulutiveOutOfTarget) * s.BusinessUnitWeight) / kpis.Where(w => w.KPIMeasures.Any(a => a.Status != "NA" && a.DueDate.Month < DateTime.Now.Month)).Sum(s => s.BusinessUnitWeight), 2);

        //    else
        //        performance = null;
        //    return performance;
        //}

        public List<KPI> KPIUpdateReminder()
        {
            using (var dataAccess = _factory.Create())
            {
                List<KPI> kpis = dataAccess.KPI.Query().IncludeOptimized(a => a.KPIType).IncludeOptimizedByPath("KPIType.ReminderConfiguration").IncludeOptimized(a => a.KPIMeasures).ToList();
                return kpis;
            }
        }

        public List<int> GetValidYears(string UserName)
        {
            using (var dataAccess = _factory.Create())
            {
                List<KPI> kpis = dataAccess.KPI.Query().IncludeOptimized(a => a.KPIMeasures).ToList();
                List<Strategy> Strategies = dataAccess.Strategy.Query().ToList();

                var Years = kpis.Select(s => s.StartDate.Year)
       .Concat(Strategies.Where(a => a.Years != null && a.Years.Count > 0).SelectMany(y => y.Years).Select(y => Convert.ToInt32(y)))
       .Distinct()
       .ToList();
                Years.Sort();
                return Years;
            }
        }
        public List<KPI> FilterByYear(List<KPI> kpis, int Year)
        {
            return kpis.Where(w => w.StartDate.Year == Year).ToList();
        }
        public bool SyncKPIHistory()
        {
            int result = 0;
            List<KPI> allKPIs = Read(null);
            List<KPIHistory> tmpKPIHistories = new List<KPIHistory>();
            DateTime Today = DateTime.Now;

            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPIHistory> kpihistory = dataAccess.KPIHistory.Query().ToList();

                if (kpihistory.Count() == 0)
                {
                    foreach (var kpi in allKPIs)
                    {
                        List<KPIMeasure> KPIMeasures = new List<KPIMeasure>();
                        KPIMeasures = kpi.KPIMeasures.Where(a => a.Status != "NA").ToList();
                        decimal SummedValues = KPIMeasures.Sum(a => a.Value).Value;
                        decimal SummedTargets = KPIMeasures.Sum(a => a.Target);
                        decimal ValuesAverage = KPIMeasures.Count > 0 ? KPIMeasures.Sum(a => a.Value).Value / KPIMeasures.Count : 0;
                        decimal TargetsAverage = KPIMeasures.Count > 0 ? KPIMeasures.Sum(a => a.Target) / KPIMeasures.Count : 0;
                        tmpKPIHistories.Add(new KPIHistory
                        {
                            Baseline = kpi.Baseline,
                            HistoryDate = Today,
                            KPIID = kpi.ID,
                            OutOfTarget = kpi.OutOfTarget,
                            AccumulutiveOutOfTarget = kpi.AccumulutiveOutOfTarget,
                            Status = kpi.Status,
                            Target = kpi.Target,
                            Value = kpi.Value,
                            Weight = kpi.Weight,
                            BusinessUnitWeight = kpi.BusinessUnitWeight,

                            SumCumulativePerformance = KPIMeasures.Any() ? GetCumulitavePerformance(kpi, SummedValues, SummedTargets) : 0,
                            AverageCumulativePerformance = KPIMeasures.Any() ? GetCumulitaveAveragePerformance(kpi, ValuesAverage, TargetsAverage) : 0,

                        });

                    }
                }
                else
                {

                    foreach (var kpi in allKPIs)
                    {
                        if (kpihistory.Where(a => a.KPIID == kpi.ID && a.HistoryDate.Date.Month == Today.Month && a.HistoryDate.Date.Year == Today.Year).Any())
                        {
                            continue;
                        }
                        else
                        {
                            List<KPIMeasure> KPIMeasures = new List<KPIMeasure>();
                            KPIMeasures = kpi.KPIMeasures.Where(a => a.Status != "NA").ToList();
                            decimal SummedValues = KPIMeasures.Sum(a => a.Value).Value;
                            decimal SummedTargets = KPIMeasures.Sum(a => a.Target);
                            decimal ValuesAverage = KPIMeasures.Count > 0 ? KPIMeasures.Sum(a => a.Value).Value / KPIMeasures.Count : 0;
                            decimal TargetsAverage = KPIMeasures.Count > 0 ? KPIMeasures.Sum(a => a.Target) / KPIMeasures.Count : 0;
                            tmpKPIHistories.Add(new KPIHistory
                            {
                                Baseline = kpi.Baseline,
                                HistoryDate = Today,
                                KPIID = kpi.ID,
                                OutOfTarget = kpi.OutOfTarget,
                                AccumulutiveOutOfTarget = kpi.AccumulutiveOutOfTarget,
                                Status = kpi.Status,
                                Target = kpi.Target,
                                Value = kpi.Value,
                                Weight = kpi.Weight,
                                BusinessUnitWeight = kpi.BusinessUnitWeight,
                                SumCumulativePerformance = KPIMeasures.Any() ? GetCumulitavePerformance(kpi, SummedValues, SummedTargets) : 0,
                                AverageCumulativePerformance = KPIMeasures.Any() ? GetCumulitaveAveragePerformance(kpi, ValuesAverage, TargetsAverage) : 0,
                            });
                        }
                    }
                }

                dataAccess.KPIHistory.SaveRange(tmpKPIHistories);
                result = dataAccess.Complete();
                return true;
            }
        }
        public bool TSyncKPIHistory()
        {
            DateTime MinDate = Read(null).SelectMany(s => s.KPIMeasures.Select(x => x.DueDate)).Min();
            DateTime Today = DateTime.Now;
            Today = new DateTime(MinDate.Year, MinDate.Month, MinDate.Day);
            for (int Year = MinDate.Year; Year <= DateTime.Now.Year; Year++)
            {
                if (Today > DateTime.Now.Date)
                    break;
                if (Today.Year <= 2024)
                    Today = new DateTime(Year, 1, 1);
                for (int Month = 1; Month <= 12; Month++)
                {
                    if (Today > DateTime.Now.Date)
                        break;
                    if (Today.Month <= 12)
                        Today = new DateTime(Year, Month, 1);
                    #region Historical Code
                    int result = 0;
                    List<KPI> allKPIs = Read(null);
                    List<KPIHistory> tmpKPIHistories = new List<KPIHistory>();

                    using (var dataAccess = _factory.Create())
                    {
                        IEnumerable<KPIHistory> kpihistory = dataAccess.KPIHistory.Query().ToList();

                        if (kpihistory.Count() == 0)
                        {
                            foreach (var kpi in allKPIs)
                            {

                                List<KPIMeasure> KPIMeasures = new List<KPIMeasure>();
                                kpi.KPIMeasures = kpi.KPIMeasures.Where(w => (w.DueDate.Year < Today.Year) || (w.DueDate.Year == Today.Year && w.DueDate.Month <= Today.Month)).ToList();
                                MapKPIProperties(kpi, new List<KPI>(), new List<OrgStructure>(), new List<DivisionalObjective>());

                                KPIMeasures = kpi.KPIMeasures.Where(a => a.Status != "NA").ToList();
                                decimal SummedValues = KPIMeasures.Sum(a => a.Value).Value;
                                decimal SummedTargets = KPIMeasures.Sum(a => a.Target);
                                decimal ValuesAverage = KPIMeasures.Count > 0 ? KPIMeasures.Sum(a => a.Value).Value / KPIMeasures.Count : 0;
                                decimal TargetsAverage = KPIMeasures.Count > 0 ? KPIMeasures.Sum(a => a.Target) / KPIMeasures.Count : 0;
                                tmpKPIHistories.Add(new KPIHistory
                                {
                                    Baseline = kpi.Baseline,
                                    HistoryDate = Today,
                                    KPIID = kpi.ID,
                                    OutOfTarget = kpi.OutOfTarget,
                                    AccumulutiveOutOfTarget = kpi.AccumulutiveOutOfTarget,
                                    Status = kpi.Status,
                                    Target = kpi.Target,
                                    Value = kpi.Value,
                                    Weight = kpi.Weight,
                                    BusinessUnitWeight = kpi.BusinessUnitWeight,
                                    SumCumulativePerformance = KPIMeasures.Any() ? GetCumulitavePerformance(kpi, SummedValues, SummedTargets) : 0,
                                    AverageCumulativePerformance = KPIMeasures.Any() ? GetCumulitaveAveragePerformance(kpi, ValuesAverage, TargetsAverage) : 0,
                                });

                            }
                        }
                        else
                        {

                            foreach (var kpi in allKPIs)
                            {
                                kpi.KPIMeasures = kpi.KPIMeasures.Where(w => (w.DueDate.Year < Today.Year) || (w.DueDate.Year == Today.Year && w.DueDate.Month <= Today.Month)).ToList();
                                MapKPIProperties(kpi, new List<KPI>(), new List<OrgStructure>(), new List<DivisionalObjective>());

                                if (kpihistory.Where(a => a.KPIID == kpi.ID && a.HistoryDate.Date.Month == Today.Month && a.HistoryDate.Date.Year == Today.Year).Any())
                                {
                                    continue;
                                }
                                else
                                {
                                    List<KPIMeasure> KPIMeasures = new List<KPIMeasure>();
                                    KPIMeasures = kpi.KPIMeasures.Where(a => a.Status != "NA").ToList();
                                    decimal SummedValues = KPIMeasures.Sum(a => a.Value ?? 0);
                                    decimal SummedTargets = KPIMeasures.Sum(a => a.Target);
                                    decimal ValuesAverage = KPIMeasures.Count > 0 ? KPIMeasures.Sum(a => a.Value ?? 0) / KPIMeasures.Count : 0;
                                    decimal TargetsAverage = KPIMeasures.Count > 0 ? KPIMeasures.Sum(a => a.Target) / KPIMeasures.Count : 0;
                                    tmpKPIHistories.Add(new KPIHistory
                                    {
                                        Baseline = kpi.Baseline,
                                        HistoryDate = Today,
                                        KPIID = kpi.ID,
                                        OutOfTarget = kpi.OutOfTarget,
                                        AccumulutiveOutOfTarget = kpi.AccumulutiveOutOfTarget,
                                        Status = kpi.Status,
                                        Target = kpi.Target,
                                        Value = kpi.Value,
                                        Weight = kpi.Weight,
                                        BusinessUnitWeight = kpi.BusinessUnitWeight,
                                        SumCumulativePerformance = KPIMeasures.Any() ? GetCumulitavePerformance(kpi, SummedValues, SummedTargets) : 0,
                                        AverageCumulativePerformance = KPIMeasures.Any() ? GetCumulitaveAveragePerformance(kpi, ValuesAverage, TargetsAverage) : 0,
                                    });
                                }
                            }
                        }
                        tmpKPIHistories = tmpKPIHistories.Where(w => w.HistoryDate.Date <= DateTime.Now && w.Status != "NA").ToList();
                        dataAccess.KPIHistory.SaveRange(tmpKPIHistories);
                        result = dataAccess.Complete();
                    }
                    #endregion
                }
            }
            return true;
        }
        private decimal GetCumulitavePerformance(KPI kpi, decimal sumMeasureValues, decimal sumMeasureTarget)
        {
            decimal SumCumulativePerformance;

            //tmpKPIMeasure_approved != null && tmpKPIMeasure_approved.Count() > 0 ? ((sumMeasureTarget - kpi.Baseline) != 0 ? Math.Round(((sumMeasureValues - kpi.Baseline) / (sumMeasureTarget - kpi.Baseline)) * 100, 2) : 0) : 0,

            if (kpi?.Polarity?.ToLower() == "positive")
            {
                //((Sum of the Actuals Values - Baseline) / (Sum of the Target Values -Baseline)) *100 %
                if (sumMeasureTarget - kpi.Baseline != 0)
                    SumCumulativePerformance = ((sumMeasureValues - kpi.Baseline) / (sumMeasureTarget - kpi.Baseline)) * 100;
                else
                    SumCumulativePerformance = 0;

            }
            else if (kpi?.Polarity?.ToLower() == "negative")
            {
                //1-(( Sum of Actuals - Sum of Targets)/ (Baseline - Sum of Targets)) *100 %
                if (sumMeasureTarget - kpi.Baseline != 0)
                    SumCumulativePerformance = (1 - (sumMeasureValues - sumMeasureTarget) / (sumMeasureTarget - kpi.Baseline)) * 100;
                else
                    SumCumulativePerformance = 0;
            }
            else
            {
                SumCumulativePerformance = 0;
            }
            return SumCumulativePerformance > 100 ? 100 : Math.Round(SumCumulativePerformance, 2);
        }
        private decimal GetCumulitaveAveragePerformance(KPI kpi, decimal ValuesAverage, decimal TargetsAverage)
        {
            decimal AverageCumulativePerformance;


            if (kpi?.Polarity?.ToLower() == "positive")
            {
                //((Average of the Actuals Values - Baseline) / (Average of the Target Values -Baseline)) *100 %
                if (TargetsAverage - kpi.Baseline != 0)
                    AverageCumulativePerformance = ((ValuesAverage - kpi.Baseline) / (TargetsAverage - kpi.Baseline)) * 100;
                else
                    AverageCumulativePerformance = 0;

            }
            else if (kpi?.Polarity?.ToLower() == "negative")
            {
                //= 1-(( Average of Actuals- Average of Targets)/ (Baseline - Average of Targets)) *100 %
                if (TargetsAverage - kpi.Baseline != 0)
                    AverageCumulativePerformance = (1 - (ValuesAverage - TargetsAverage) / (TargetsAverage - kpi.Baseline)) * 100;
                else
                    AverageCumulativePerformance = 0;
            }
            else
            {
                AverageCumulativePerformance = 0;
            }
            return AverageCumulativePerformance > 100 ? 100 : Math.Round(AverageCumulativePerformance, 2);
        }

        #region KPI Code Creation

        private List<OrgStructure> GetOrgStructures()
        {
            using (var dataAccess = _factory.Create())
            {
                List<OrgStructure> OrgStructures = dataAccess.OrgStructure.Query().AsNoTracking().ToList();
                return OrgStructures;
            }

        }
        private List<DivisionalObjective> GetDivisionalObjectives()
        {
            using (var dataAccess = _factory.Create())
            {
                List<DivisionalObjective> DivisionalObjectives = dataAccess.DivisionalObjective.Query().AsNoTracking().ToList();
                return DivisionalObjectives;
            }
        }

        public List<KPI> ReadAll()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<KPI> kpis = dataAccess.KPI.Query().AsNoTracking()
                  .ToList();

                return kpis.ToList();
            }
        }
        #endregion
    }
}

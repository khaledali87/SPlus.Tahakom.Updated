using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using SPlus.DataAccess;
using SPlus.Model.Domain;
using System.Data.Entity;
using System.Reflection;
using Z.EntityFramework.Plus;


namespace SPlus.BLL
{
    public class StrategicObjectiveBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();

        private readonly IUnitOfWorkFactory _factory;
        public StrategicObjectiveBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }


        #region Create

        public StrategicObjective Create(StrategicObjective strategicObjective)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                strategicObjective.Created = DateTime.Now;
                strategicObjective.Modified = DateTime.Now;
                dataAccess.StrategicObjective.Save(strategicObjective);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return ReadByID(strategicObjective.ID);
            }
        }

        #endregion

        #region Read
        public List<StrategicObjective> Read()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<StrategicObjective> strategicObjectives = dataAccess.StrategicObjective.Query().OrderBy(a => a.Order).ToList();
                return strategicObjectives.ToList();
            }
        }

        public List<StrategicObjective> ReadObjectivesForDashboard()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<StrategicObjective> strategicObjectives = dataAccess.StrategicObjective.Query()
                        .IncludeOptimized(a => a.KPIs)
                        .IncludeOptimized(a => a.Theme)
                        .IncludeOptimizedByPath("Theme.Strategy")
                        .IncludeOptimized(a => a.DivisionalObjectives)
                        .IncludeOptimizedByPath("DivisionalObjectives.KPIs")
                        .IncludeOptimizedByPath("DivisionalObjectives.OrgStructure")

                    .OrderBy(a => a.Order).ToList();
                return strategicObjectives.ToList();
            }
        }

        public List<StrategicObjective> ReadWithDivisionalObjectives()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<StrategicObjective> strategicObjectives = dataAccess.StrategicObjective.Query()
                        .IncludeOptimized(a => a.DivisionalObjectives)
                        .IncludeOptimizedByPath("DivisionalObjectives.KPIs")
                                                 .IncludeOptimizedByPath("DivisionalObjectives.OrgStructure")
                        .IncludeOptimizedByPath("DivisionalObjectives.KPIs.KPIType")
                    .OrderBy(a => a.Order).ToList();
                return strategicObjectives.ToList();
            }
        }

        public StrategicObjective ReadByID(int ID)
        {
            using (var dataAccess = _factory.Create())
            {
                StrategicObjective strategicObjective = dataAccess.StrategicObjective.Query()
                    .IncludeOptimized(a => a.DivisionalObjectives.OrderBy(d => d.Order))
                    .IncludeOptimized(a => a.KPIs)
                    .IncludeOptimized(a => a.Theme)
                    .IncludeOptimizedByPath("Theme.Strategy")
                    .Where(s => s.ID == ID).OrderBy(s => s.Order)
                    .FirstOrDefault();

                if (strategicObjective != null)
                {
                    strategicObjective.IsDeletable = IsDeletable(strategicObjective);
                }
                return strategicObjective;
            }
        }

        public List<StrategicObjective> ReadByThemeID(int ID)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<StrategicObjective> strategicObjectives = dataAccess.StrategicObjective.Query()
                    .IncludeOptimized(a => a.Theme).Where(a => a.ThemeID == ID)
                    .ToList();
                return strategicObjectives.ToList();
            }
        }

        public List<StrategicObjective> ReadChild(string UserName)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<StrategicObjective> objectives = dataAccess.StrategicObjective.Query()
                    .IncludeOptimized(a => a.DivisionalObjectives)
                    .IncludeOptimized(a => a.KPIs)
                    .IncludeOptimizedByPath("KPIs.KPIMeasures")
                    .IncludeOptimizedByPath("KPIs.KPIType").ToList();

                foreach (var objective in objectives)
                {
                    objective.IsDeletable = IsDeletable(objective);
                    objective.KPIs = objective.KPIs.SecureListObj(dataAccess, UserName).Cast<KPI>().ToList();
                }
                return objectives.ToList();
            }
        }

        #endregion

        #region Update

        public StrategicObjective Update(StrategicObjective strategicObjective)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                var current = dataAccess.StrategicObjective.Query().IncludeOptimized(a => a.Theme).Where(w => w.ID == strategicObjective.ID).FirstOrDefault();

                strategicObjective.Created = current.Created;
                strategicObjective.Modified = DateTime.Now;
                strategicObjective.Weight = current.Weight;
                dataAccess.StrategicObjective.Save(strategicObjective);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return ReadByID(strategicObjective.ID);
            }
        }

        public List<StrategicObjective> Update(List<StrategicObjective> strategicObjectives)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                var contexts = dataAccess.StrategicObjective.GetAll().ToList();
                foreach (var strategicObjective in strategicObjectives)
                {
                    var context = contexts.Where(w => w.ID == strategicObjective.ID).FirstOrDefault();
                    if (context != null)
                    {
                        context.Weight = strategicObjective.Weight;
                        context.Modified = DateTime.Now;
                        dataAccess.StrategicObjective.Save(context);
                    }
                }
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return contexts;
            }
        }
        #endregion

        #region Delete
        public bool Delete(int id)
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                StrategicObjective strategicObjective = dataAccess.StrategicObjective.Query()
                    .IncludeOptimized(a => a.DivisionalObjectives).IncludeOptimized(a => a.KPIs).Where(a => a.ID == id).FirstOrDefault();
                if (strategicObjective != null)
                {
                    if (IsDeletable(strategicObjective))
                    {
                        dataAccess.StrategicObjective.Delete(strategicObjective);
                        result = dataAccess.Complete();
                        dataAccess.Dispose();
                    }
                    else
                        throw new System.Exception("Objective has related items and cannot be deleted");
                }
                if (result >= 1)
                    return true;
                else
                    return false;
            }
        }

        private bool IsDeletable(StrategicObjective strategicObjective)
        {
            bool IsDeletable = false;
            if (strategicObjective.KPIs.Count() == 0 && strategicObjective.DivisionalObjectives.Count() == 0)
            {
                IsDeletable = true;
            }
            else
            {
                IsDeletable = false;
            }
            return IsDeletable;
        }
        #endregion



    }
}

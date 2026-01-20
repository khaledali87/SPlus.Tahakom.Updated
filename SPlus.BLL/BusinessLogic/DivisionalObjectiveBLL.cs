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
    public class DivisionalObjectiveBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();

        private readonly IUnitOfWorkFactory _factory;
        public DivisionalObjectiveBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }


        #region Create

        public DivisionalObjective Create(DivisionalObjective divisionalObjective)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                divisionalObjective.Created = DateTime.Now;
                divisionalObjective.Modified = DateTime.Now;
                dataAccess.DivisionalObjective.Save(divisionalObjective);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return ReadByID(divisionalObjective.ID);
            }
        }

        #endregion

        #region Read
        public List<DivisionalObjective> Read()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<DivisionalObjective> divisionalObjectives = dataAccess.DivisionalObjective.Query()
                     .IncludeOptimized(a => a.StrategicObjective)
                     .IncludeOptimized(a => a.OrgStructure)
                    .OrderBy(a => a.Order).ToList();
                return divisionalObjectives.ToList();
            }
        }

        public DivisionalObjective ReadByID(int ID)
        {
            using (var dataAccess = _factory.Create())
            {
                DivisionalObjective divisionalObjective = dataAccess.DivisionalObjective.Query()
                    .IncludeOptimized(a=>a.StrategicObjective)
                    .IncludeOptimizedByPath("StrategicObjective.Theme")
                    .IncludeOptimized(a => a.OrgStructure)
                    .IncludeOptimized(a => a.KPIs)                    
                    
                    .Where(s => s.ID == ID).FirstOrDefault();

                if (divisionalObjective != null)
                {
                    divisionalObjective.IsDeletable = IsDeletable(divisionalObjective);
                  
                }
                return divisionalObjective;
            }
        }

        public List<DivisionalObjective> ReadByStrategicObjectivesIDWithKPIs(int id)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<DivisionalObjective> divisionalObjectives = dataAccess.DivisionalObjective.Query()
                    .IncludeOptimized(a=>a.KPIs).Where(a=>a.StrategicObjectiveID == id).OrderBy(a => a.Order).ToList();

                foreach(var divisionalObjective in divisionalObjectives)
                {
                    divisionalObjective.IsDeletable = IsDeletable(divisionalObjective);
                }
                return divisionalObjectives.ToList();
            }
        }


        public List<DivisionalObjective> ReadForWeightDepartmentalObjectie()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<DivisionalObjective> divisionalObjectives = dataAccess.DivisionalObjective.Query()
                    .IncludeOptimized(a => a.KPIs)                    
                    .IncludeOptimizedByPath("KPIs.KPIType")
                    .ToList();
                return divisionalObjectives.ToList();
            }
        }


        #endregion

        #region Update

        public DivisionalObjective Update(DivisionalObjective divisionalObjective)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                var current = dataAccess.DivisionalObjective.Query().Where(w => w.ID == divisionalObjective.ID).FirstOrDefault();

                divisionalObjective.Created = current.Created;
                divisionalObjective.Modified = DateTime.Now;
                divisionalObjective.Weight = current.Weight;
                dataAccess.DivisionalObjective.Save(divisionalObjective);

                            
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return ReadByID(divisionalObjective.ID);
            }
        }

        public List<DivisionalObjective> Update(List<DivisionalObjective> divisionalObjectives)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                var contexts = dataAccess.DivisionalObjective.GetAll().ToList();
                foreach (var divisionalObjective in divisionalObjectives)
                {
                    var context = contexts.Where(w => w.ID == divisionalObjective.ID).FirstOrDefault();
                    if (context != null)
                    {
                        context.Weight = divisionalObjective.Weight;
                        context.Modified = DateTime.Now;
                        dataAccess.DivisionalObjective.Save(context);
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
                DivisionalObjective divisionalObjective = dataAccess.DivisionalObjective.Query()                          
                .IncludeOptimized(a => a.KPIs)
                .Where(a => a.ID == id).FirstOrDefault();
                if (divisionalObjective != null)
                {
                    if (IsDeletable(divisionalObjective))
                    {
                        dataAccess.DivisionalObjective.Delete(divisionalObjective);
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

        private bool IsDeletable(DivisionalObjective divisionalObjective)
        {
            bool IsDeletable = false;
           if (divisionalObjective.KPIs.Count() > 0)
            {
                IsDeletable = false;
           }
           else
            {
                IsDeletable = true;
            }
            return IsDeletable;
        }
        #endregion

    }
}

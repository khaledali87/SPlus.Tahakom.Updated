using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.DTO;
using SPlus.DataAccess;
using SPlus.Model.Domain;
using SPlus.Model;
using System.Data.Entity;
using Z.EntityFramework.Plus;
using System.Reflection;
using SPlus.Helper;
using StructureMap;

namespace SPlus.BLL
{
    public class OrgStructureBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();


        private readonly IUnitOfWorkFactory _factory;
        public OrgStructureBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();
        }

        #region Create
        public OrgStructure Create(OrgStructure orgStructure, string UserName)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                dataAccess.OrgStructure.Save(orgStructure);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return ReadByID(orgStructure.ID, UserName);
            }
        }

        #endregion

        #region Read
        public List<OrgStructure> Read(string UserName,int ?year)
        {
            using (var dataAccess = _factory.Create())
            {
                List<OrgStructure> orgStructures = dataAccess.OrgStructure.Query()
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimized(a => a.KPIList)
                    .IncludeOptimized(a => a.ManagerModel)
                    .IncludeOptimized(a => a.Group)
                    .SecureListObj(dataAccess, UserName).Cast<OrgStructure>().ToList()
                    .Where(a => !year.HasValue || a.Years.Contains(year.Value.ToString())).ToList();
                orgStructures = SetIsDeletable(orgStructures);
                return orgStructures.ToList();
            }
        }
        public List<OrgStructure> ReadAll()
        {
            using (var dataAccess = _factory.Create())
            {
                List<OrgStructure> orgStructures = dataAccess.OrgStructure.Query()
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimized(a => a.KPIList)
                    .IncludeOptimized(a => a.ManagerModel)
                    .IncludeOptimized(a => a.Group)
             .Cast<OrgStructure>().ToList();
                orgStructures = SetIsDeletable(orgStructures);
                return orgStructures.ToList();
            }
        }
        public OrgStructure ReadByID(int ID, string UserName)
        {

            using (var dataAccess = _factory.Create())
            {
                OrgStructure orgStructure = dataAccess.OrgStructure.Query()
                    .Where(w => w.ID == ID)
                    .IncludeOptimized(a => a.ManagerModel)
                    .IncludeOptimized(a => a.Group)
                    .FirstOrDefault();
                return orgStructure;
            }
        }
        public OrgStructure ReadByID_BlanceScoreDepartmentById(int ID, string UserName)
        {

            using (var dataAccess = _factory.Create())
            {
                OrgStructure orgStructure = dataAccess.OrgStructure.Query()
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimized(a => a.ManagerModel)
                    .IncludeOptimized(a => a.Group)
                    .Where(w => w.ID == ID)

                    .FirstOrDefault();
                return orgStructure;
            }
        }


        public List<DivisionalObjective> ReadDivisionalObjectiveByOrgId(int id, string UserName)
        {
            using (var dataAccess = _factory.Create())
            {
                List<DivisionalObjective> DivisionalObjective = dataAccess.DivisionalObjective.Query()
                  
                    .Where(w => w.OrgStructureId == id).ToList();

                return DivisionalObjective.ToList();
            }
        }


        public List<OrgStructure> ReadForWeightDivisional(string UserName)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<OrgStructure> orgStructures = dataAccess.OrgStructure.Query()
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimizedByPath("DivisionalObjective.KPIs")
                    .IncludeOptimized(a => a.Group)
                    .IncludeOptimized(a => a.ManagerModel)
                    .SecureListObj(dataAccess, UserName).Cast<OrgStructure>()
                    .ToList();

                foreach (var org in orgStructures)
                {
                    foreach (var divisional in org.DivisionalObjective)
                    {
                        divisional.KPIs = divisional.KPIs.Where(a => !a.StrategicObjectiveID.HasValue && a.DivisionalObjectiveID.HasValue).ToList();
                        divisional.KPIs.SecureListObj(dataAccess, UserName);
                    }

                }
                return orgStructures.ToList();
            }
        }


        public List<OrgStructure> ReadOrgWithDivisional(string UserName, int? Year)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<OrgStructure> orgStructures = dataAccess.OrgStructure.Query()
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimized(a => a.ManagerModel)
                    .IncludeOptimized(a => a.Group)
                    .SecureListObj(dataAccess, UserName).Cast<OrgStructure>()
                    .ToList()
                    .Where(a => !Year.HasValue || a.Years.Contains(Year.Value.ToString())).ToList();


                return orgStructures.ToList();
            }
        }
        public List<OrgStructure> ReadOrgWithDivisional_new(string UserName, int? Year)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<OrgStructure> orgStructures = dataAccess.OrgStructure.Query()
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimized(a => a.KPIList)
                    .IncludeOptimized(a => a.ManagerModel)
                    .IncludeOptimized(a => a.Group)
                    .SecureListObj(dataAccess, UserName).Cast<OrgStructure>()
                    .ToList().Where(a => !Year.HasValue || a.Years.Contains(Year.Value.ToString())).ToList();


                return orgStructures.ToList();
            }
        }

        public List<OrgStructure> ReadOrgWithDivisional_noPermission(string UserName, int? Year)
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<OrgStructure> orgStructures = dataAccess.OrgStructure.Query()
                    .IncludeOptimized(a => a.DivisionalObjective)
                    .IncludeOptimized(a => a.KPIList)
                    .IncludeOptimized(a => a.ManagerModel)
                    .IncludeOptimized(a => a.Group)
                   
                    .ToList().Where(a => !Year.HasValue || a.Years.Contains(Year.Value.ToString())).ToList();


                return orgStructures.ToList();
            }
        }

        #endregion

        #region Update
        public OrgStructure Update(OrgStructure orgStructure, string UserName)
        {
            using (var dataAccess = _factory.Create())
            {
                var current = dataAccess.OrgStructure.Get(orgStructure.ID);
                orgStructure.Weight = current.Weight;
                dataAccess.OrgStructure.Save(orgStructure);
                dataAccess.Complete();
                dataAccess.Dispose();
                return ReadByID(orgStructure.ID, UserName);
            }

        }

        public List<OrgStructure> Update(List<OrgStructure> orgStructures)
        {
            int result;

            using (var dataAccess = _factory.Create())
            {
                List<OrgStructure> contexts = dataAccess.OrgStructure.Query().ToList();
                foreach (OrgStructure orgStructure in orgStructures)
                {
                    OrgStructure context = contexts.Where(w => w.ID == orgStructure.ID).FirstOrDefault();
                    context.Weight = orgStructure.Weight;
                    dataAccess.OrgStructure.Save(context);
                }
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return orgStructures;
            }
        }
        #endregion

        #region Delete
        public bool Delete(int id)
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                List<OrgStructure> orgStructures = dataAccess.OrgStructure.Query()
                      .IncludeOptimized(a => a.DivisionalObjective).ToList();
                bool isDeletable = SetIsDeletable(orgStructures, id);
                if (isDeletable)
                {
                    dataAccess.OrgStructure.Delete(orgStructures.Where(a => a.ID == id).FirstOrDefault());
                    result = dataAccess.Complete();
                    dataAccess.Dispose();
                    if (result >= 1)
                        return true;
                    else
                        return false;
                }
                else
                    return false;

            }
        }
        #endregion

        #region Privates

        private bool SetIsDeletable(List<OrgStructure> orgStructures, int id)
        {
            bool IsDeletable = false;


            foreach (var orgStructure in orgStructures)
            {
                if (orgStructure.ID == id)
                {
                    if (orgStructure.ParentID == null)
                    {
                        if (orgStructures.Where(a => a.ParentID == id).Count() == 0)
                        {
                            IsDeletable = true;
                            break;
                        }
                    }
                    else
                    {
                        IsDeletable = true;
                    }

                    //check if Divisional objective is connected
                    if (orgStructure.DivisionalObjective != null && orgStructure.DivisionalObjective.Count > 0)
                        IsDeletable = false;
                }
            }


            return IsDeletable;
        }

        private List<OrgStructure> SetIsDeletable(List<OrgStructure> orgStructures)
        {
            foreach (var orgStructure in orgStructures)
            {

                if (orgStructure.ParentID == null)
                {
                    if (orgStructures.Where(a => a.ParentID == orgStructure.ID).Count() == 0)
                    {
                        orgStructure.IsDeletable = true;
                    }
                }
                else
                {
                    orgStructure.IsDeletable = true;
                }

                //check if Divisional objective is connected
                if (orgStructure.DivisionalObjective != null && orgStructure.DivisionalObjective.Count > 0)
                    orgStructure.IsDeletable = false;
                if (orgStructure.KPIList != null && orgStructure.KPIList.Any(a => a.OrgStructureID.HasValue && a.OrgStructureID.Value == orgStructure.ID))
                    orgStructure.IsDeletable = false;

            }
            return orgStructures;
        }

        #endregion

        #region OrgStructure Tree

        public List<OrgStructure> BuildTreeReturnAll(List<OrgStructure> source)
        {
            var groups = source.GroupBy(i => i.ParentID);

            List<OrgStructure> roots = new List<OrgStructure>();
            if (groups.Where(g => !g.Key.HasValue).FirstOrDefault() != null)
                roots = groups.Where(g => !g.Key.HasValue).FirstOrDefault().ToList();
            else
                roots = groups.Where(g => groups.Any(w => w.Any(o => o.ParentID == g.Key))).FirstOrDefault().ToList();

            if (roots.Count > 0)
            {
                var dict = groups.Where(g => g.Key.HasValue).ToDictionary(g => g.Key.Value, g => g.ToList());
                for (int i = 0; i < roots.Count; i++)
                    AddChildren(roots[i], dict);
            }

            return source;
        }
        public List<OrgStructure> BuildTree(List<OrgStructure> source)
        {
            var groups = source.GroupBy(i => i.ParentID);

            List<OrgStructure> roots = new List<OrgStructure>();
            if (source.Any())
            {
                if (groups.Where(g => !g.Key.HasValue).FirstOrDefault() != null)
                    roots = groups.Where(g => !g.Key.HasValue).FirstOrDefault().ToList();
                else
                    roots = groups.Where(g => groups.Any(w => w.Any(o => o.ParentID == g.Key))).FirstOrDefault().ToList();

                if (roots.Count > 0)
                {
                    var dict = groups.Where(g => g.Key.HasValue).ToDictionary(g => g.Key.Value, g => g.ToList());
                    for (int i = 0; i < roots.Count; i++)
                        AddChildren(roots[i], dict);
                }

            }

            return roots;
        }
        private void AddChildren(OrgStructure node, IDictionary<int, List<OrgStructure>> source)
        {
            if (source.ContainsKey(node.ID))
            {
                node.Childrens = source[node.ID];
                for (int i = 0; i < node.Childrens.Count; i++)
                    AddChildren(node.Childrens[i], source);
            }
            else
            {
                node.Childrens = new List<OrgStructure>();
            }
        }

        public OrgStructure BuildUpwardTree(OrgStructure item, List<OrgStructure> orgStructures)
        {
            IDictionary<int, OrgStructure> source = orgStructures.ToDictionary(org => org.ID);
            if (item.ParentID.HasValue && source.ContainsKey(item.ParentID.Value))
            {
                item.Parent = source[item.ParentID.Value];

                if (item.Parent != null && item.Parent.ParentID.HasValue && source.ContainsKey(item.Parent.ParentID.Value))
                {
                    item.Parent.Parent = source[item.Parent.ParentID.Value];
                }
            }

            return item;
        }
        public List<OrgStructure> BuildTreeWithParents(List<OrgStructure> source)
        {
            var groups = source.GroupBy(i => i.ParentID);

            List<OrgStructure> roots = groups.FirstOrDefault(g => g.Key == null)?.ToList() ?? new List<OrgStructure>();

            if (roots.Count > 0)
            {
                var dict = groups.Where(g => g.Key.HasValue).ToDictionary(g => g.Key.Value, g => g.ToList());
                for (int i = 0; i < roots.Count; i++)
                    AddParents(roots[i], null, dict);
            }

            return roots;
        }
        private void AddParents(OrgStructure node, OrgStructure parent, IDictionary<int, List<OrgStructure>> source)
        {
            node.Parent = parent; // Set the parent reference

            if (source.ContainsKey(node.ID))
            {
                for (int i = 0; i < source[node.ID].Count; i++)
                    AddParents(source[node.ID][i], node, source);
            }
        }
        #endregion

        public decimal? CalculateDepartmentsPerformace(List<OrgStructure> departments)
        {
            decimal? performance;

            if (departments.Count() > 0 && departments.Any(a => a.Status != "NA"))
                performance = Math.Round(departments.Sum(s => s.Performance * s.Weight) / 100, 2);
            else
                performance = null;
            return performance;
        }
    }
}

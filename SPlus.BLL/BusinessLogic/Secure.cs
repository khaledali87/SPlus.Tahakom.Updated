using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.Model;
using SPlus.BLL;
using SPlus.Model.Domain;
using SPlus.DataAccess;
using StructureMap;
using System.Data.Entity;
using Z.EntityFramework.Plus;
using System.Configuration;
using SPlus.Helper;
using System.Runtime.InteropServices;

namespace SPlus.BLL
{
    public static class Secure
    {
        // For one object
        public static object SecureObj(this object obj, IUnitOfWork dataAccess, string UserName)
        {

            //Get LevelID 
            Type type = obj.GetType();
            int LevelID = (int)Enum.Parse(typeof(LevelTypeEnum), type.Name);

            //Get Resources By LevelID
            List<Resource> resources = dataAccess.Resource.Query().Where(w => w.LevelID == LevelID).ToList();

            //Get PropertyName From Resources 
            string PropertyName = resources.Select(s => s.PropertyName).FirstOrDefault();

            // Get Value From KPI Based on Propery Name

            //Filter Resources Based on Property Name And value
            // Get Allowed Groups 
            List<Group> group = dataAccess.Group.Query()
                .Include(a => a.UsersGroups)
                .Include(a => a.Matrices.Select(s => s.Resource))
                .Include(a => a.Matrices.Select(s => s.Role))
                .ToList();

            List<OrgStructure> OrgStructures = dataAccess.OrgStructure.Query()
                .Include(a => a.Group)
                .ToList();
            List<DivisionalObjective> divisionalObjectives = dataAccess.DivisionalObjective.Query()
               .Include(a => a.OrgStructure)
               .ToList();
            List<Matrix> matrices = group.SelectMany(s => s.Matrices).ToList();

            return obj.secure(resources, matrices, group, OrgStructures, divisionalObjectives, UserName, PropertyName);
        }

        // For List of objects
        public static IEnumerable<object> SecureListObj(this IEnumerable<object> objs, IUnitOfWork dataAccess, string UserName)
        {
            if (objs != null && objs.Count() > 0)
            {
                //Get LevelID 
                Type type = objs.FirstOrDefault().GetType();
                int LevelID = (int)Enum.Parse(typeof(LevelTypeEnum), type.Name);
                //Get Resources By LevelID
                List<Resource> resources = dataAccess.Resource.Query().Where(w => w.LevelID == LevelID).ToList();
                //Get PropertyName From Resources 
                string PropertyName = resources.Select(s => s.PropertyName).FirstOrDefault();
                // Get Value From KPI Based on Propery Name
                //Filter Resources Based on Property Name And value
                // Get Allowed Groups 
                List<Group> group = dataAccess.Group.Query()
                    .Include(a => a.UsersGroups)
                    .Include(a => a.Matrices.Select(s => s.Resource))
                    .Include(a => a.Matrices.Select(s => s.Role))
                    .ToList();
                List<Matrix> matrices = group.SelectMany(s => s.Matrices).ToList();
                List<OrgStructure> OrgStructures = dataAccess.OrgStructure.Query()
                  .Include(a => a.Group)
                  .ToList();
                List<DivisionalObjective> divisionalObjectives = dataAccess.DivisionalObjective.Query()
              .Include(a => a.OrgStructure)
              .ToList();

                List<object> tempobjects = new List<object>();
                foreach (var obj in objs)
                {
                    var tempobject = obj.secure(resources, matrices, group, OrgStructures, divisionalObjectives, UserName, PropertyName);
                    if (tempobject != null)
                    {
                        tempobjects.Add(tempobject);
                    }
                }
                return tempobjects;
            }
            else
                return new List<object>();
        }

        #region Commented
        //private static object secure(this object obj, List<Resource> resources, List<Matrix> matrices, List<Group> group, List<OrgStructure> OrgStructures, string UserName, string PropertyName)
        //{

        //    int ObjectPropertyValue = (int)obj.GetType().GetProperty(PropertyName).GetValue(obj, default);
        //    string AdminGroup = Constants._AdminGroup;
        //    List<Group> AdminGroups = group.Where(w => w.Title.ToLower() == AdminGroup.ToLower()).ToList();
        //    AdminGroups = AdminGroups.Where(w => w.UsersGroups.Select(s => s.UserName?.ToLower()).Contains(UserName?.ToLower())).ToList();
        //    Resource resource = resources.Where(w => w.PropertyName == PropertyName && w.PropertyValue == ObjectPropertyValue).FirstOrDefault();
        //    if (resource != null)
        //    {
        //        matrices = matrices.Where(w => w.ResourceID == resource.ID).ToList();
        //        group = group.Where(w => matrices.Select(s => s.GroupID).Contains(w.ID) && w.UsersGroups.Select(s => s.UserName?.ToLower()).Contains(UserName?.ToLower())).ToList();
        //        matrices = matrices.Where(w => group.Select(s => s.ID).Contains(w.GroupID)).ToList();

        //        // Check if User is in any of the allowed Groups
        //        if (AdminGroups.Count() > 0)
        //        {
        //            obj.GetType().GetProperty("CanAccess").SetValue(obj, true);
        //            return obj;
        //        }
        //        else if (group.Count() > 0)
        //        {
        //            //Check Access By Role ID
        //            if (matrices.Where(w => w.RoleID == 2).Count() > 0)
        //            {
        //                //Set Can Access True
        //                obj.GetType().GetProperty("CanAccess").SetValue(obj, true);
        //            }
        //            else
        //            {
        //                //else Set Can Access False
        //                obj.GetType().GetProperty("CanAccess").SetValue(obj, false);
        //            }
        //            return obj;
        //        }
        //        else
        //        {
        //            //else return null
        //            return null;
        //        }
        //    }

        //    List<OrgStructure> SecuredOrgStructure = OrgStructures.Where(w => group.Any(a => a.ID == w.GroupID)).ToList();

        //    //Get LevelID 
        //    if (obj.GetType() == typeof(KPI))
        //    {
        //        foreach (var prop in obj.GetType().GetProperties())
        //        {
        //            if (prop.PropertyType == typeof(User))
        //            {
        //                User user = (User)obj.GetType().GetProperty(prop.Name).GetValue(obj);
        //                if (user != null && user.UserName.ToLower() == UserName.ToLower())
        //                {
        //                    obj.GetType().GetProperty("CanAccess").SetValue(obj, true);
        //                    return obj;
        //                }
        //            }
        //            if (prop.PropertyType == typeof(OrgStructure))
        //            {
        //                OrgStructure orgStructure = (OrgStructure)obj.GetType().GetProperty(prop.Name).GetValue(obj);
        //                if (SecuredOrgStructure.Any(a => a.ID == orgStructure.ID))
        //                    return obj;

        //                foreach (var orgprop in orgStructure.GetType().GetProperties().Where(w => w.PropertyType == typeof(User)))
        //                {
        //                    User user = (User)orgStructure.GetType().GetProperty(orgprop.Name).GetValue(orgStructure);
        //                    if (user != null && user.UserName.ToLower() == UserName.ToLower())
        //                    {
        //                        obj.GetType().GetProperty("CanAccess").SetValue(obj, true);
        //                        return obj;
        //                    }
        //                }
        //            }
        //            if (prop.PropertyType == typeof(DivisionalObjective))
        //            {
        //                DivisionalObjective divisionalObjective = (DivisionalObjective)obj.GetType().GetProperty(prop.Name).GetValue(obj);
        //                if (SecuredOrgStructure.Any(a => a.ID == divisionalObjective.OrgStructureId))
        //                    return obj;

        //                foreach (var item in divisionalObjective.GetType().GetProperties().Where(w => w.PropertyType == typeof(OrgStructure)))
        //                {
        //                    OrgStructure orgStructure = (OrgStructure)divisionalObjective.GetType().GetProperty(item.Name).GetValue(divisionalObjective);
        //                    if (SecuredOrgStructure.Any(a => a.ID == orgStructure.ID))
        //                        return obj;
        //                }
        //            }
        //        }
        //    }
        //    if (obj.GetType() == typeof(OrgStructure))
        //    {
        //        OrgStructure orgStructure = (OrgStructure)obj;
        //        if (SecuredOrgStructure.Any(a => a.ID == orgStructure.ID))
        //            return obj;

        //        foreach (var prop in obj.GetType().GetProperties().Where(w => w.PropertyType == typeof(User)))
        //        {
        //            User user = (User)obj.GetType().GetProperty(prop.Name).GetValue(obj);
        //            if (user != null && user.UserName.ToLower() == UserName.ToLower())
        //            {
        //                obj.GetType().GetProperty("CanAccess").SetValue(obj, true);
        //                return obj;
        //            }
        //        }
        //    }

        //    return null;
        //}

        #endregion

        public static object secure(
        this object obj,
        List<Resource> resources,
        List<Matrix> matrices,
        List<Group> groups,
        List<OrgStructure> orgStructures,
          List<DivisionalObjective> divisionalObjectives,
        string userName,
        string propertyName)
        {
            if (obj == null) return null;
            object result = null;
            groups = FilterUserGroups(groups, userName);
            var securedOrgStructures = FilterSecuredOrgStructures(orgStructures, groups);
            var securedDivisionalObj = FilterSecuredDivisionalObjective(securedOrgStructures, divisionalObjectives);
            if (IsUserInAdminGroup(groups, userName))
            {
                SetCanAccess(obj, true);
                return obj;
            }
            if (!string.IsNullOrEmpty(propertyName))
            {
                int? objectPropertyValue = GetObjectPropertyValue(obj, propertyName);
                var resource = resources.FirstOrDefault(r => r.PropertyName == propertyName && r.PropertyValue == objectPropertyValue);
                if (resource != null)
                {
                    result = ProcessResourceAccess(obj, matrices, groups, userName, resource);
                }
            }
            if (result is null)
                result = CheckObjectHierarchyAccess(obj, securedOrgStructures, securedDivisionalObj, userName);

            return result;
        }

        private static int? GetObjectPropertyValue(object obj, string propertyName)
        {
            return (int?)obj.GetType().GetProperty(propertyName)?.GetValue(obj);
        }

        private static List<Group> FilterUserGroups(List<Group> groups, string userName)
        {
            return groups.Where(g => g.UsersGroups.Any(ug => ug.UserName?.ToLower() == userName?.ToLower())).ToList();
        }

        private static List<OrgStructure> FilterSecuredOrgStructures(List<OrgStructure> orgStructures, List<Group> groups)
        {
            return orgStructures.Where(os => groups.Any(g => g.ID == os.GroupID)).ToList();
        }
        private static List<DivisionalObjective> FilterSecuredDivisionalObjective(List<OrgStructure> orgStructures, List<DivisionalObjective> divisionalObjectives)
        {
            return divisionalObjectives.Where(os => orgStructures.Any(g => g.ID == os.OrgStructureId)).ToList();
        }


        private static bool IsUserInAdminGroup(List<Group> groups, string userName)
        {
            string adminGroup = Constants._AdminGroup.ToLower();
            return groups.Any(g => g.Title.ToLower() == adminGroup);
        }

        private static void SetCanAccess(object obj, bool value)
        {
            obj.GetType().GetProperty("CanAccess")?.SetValue(obj, value);
        }

        private static object ProcessResourceAccess(object obj, List<Matrix> matrices, List<Group> groups, string userName, Resource resource)
        {
            var resourceMatrices = matrices.Where(m => m.ResourceID == resource.ID).ToList();
            var userGroups = groups.Where(g => resourceMatrices.Select(m => m.GroupID).Contains(g.ID)).ToList();

            if (userGroups.Any())
            {
                bool hasAccess = resourceMatrices.Any(m => m.RoleID == 2);
                SetCanAccess(obj, hasAccess);
                return obj;
            }

            return null;
        }

        private static object CheckObjectHierarchyAccess(object obj, List<OrgStructure> securedOrgStructures, List<DivisionalObjective> divisionalObjectives, string userName)
        {
            if (obj is KPI kpi)
            {
                return CheckKPIAccess(kpi, securedOrgStructures, divisionalObjectives, userName);
            }
            if (obj is OrgStructure orgStructure)
            {
                return securedOrgStructures.Any(os => os.ID == orgStructure.ID) ? obj : CheckUserAccess(obj, userName);
            }
            return null;
        }

        private static object CheckKPIAccess(KPI kpi, List<OrgStructure> securedOrgStructures, List<DivisionalObjective> divisionalObjectives, string userName)
        {
            foreach (var prop in kpi.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(User))
                {
                    
                    if (CheckUserAccess(kpi, userName) != null)
                    {
                        return kpi;
                    }

                    if (CheckUserAccess_user(kpi, userName) != null)
                    {
                        return kpi;
                    }

                }
                if (prop.PropertyType == typeof(OrgStructure) && CheckOrgStructureAccess(kpi, securedOrgStructures))
                {
                    bool hasAccess = CheckOrgStructureAccess(kpi, securedOrgStructures);
                    SetCanAccess(kpi, hasAccess);
                    return kpi;
                }
                if (prop.PropertyType == typeof(DivisionalObjective) && CheckDivisionalObjAccess(kpi, divisionalObjectives))
                {
                    bool hasAccess = CheckDivisionalObjAccess(kpi, divisionalObjectives);
                    SetCanAccess(kpi, hasAccess);
                    return kpi;
                }


          
            }
            return null;
        }

        private static bool CheckOrgStructureAccess(object obj, List<OrgStructure> securedOrgStructures)
        {
            var orgStructure = obj.GetType().GetProperties()
                                  .Where(p => p.PropertyType == typeof(OrgStructure))
                                  .Select(p => (OrgStructure)p.GetValue(obj))
                                  .FirstOrDefault();
            return orgStructure != null && securedOrgStructures.Any(os => os.ID == orgStructure.ID);

        }
        private static bool CheckDivisionalObjAccess(object obj, List<DivisionalObjective> securedDivisionalObj)
        {
            var divisionalObjective = obj.GetType().GetProperties()
                                  .Where(p => p.PropertyType == typeof(DivisionalObjective))
                                  .Select(p => (DivisionalObjective)p.GetValue(obj))
                                  .FirstOrDefault();
            return divisionalObjective != null && securedDivisionalObj.Any(os => os.ID == divisionalObjective.ID);

        }

        private static object CheckUserAccess(object obj, string userName)
        {
            var user = obj.GetType().GetProperties()
                           .Where(p => p.PropertyType == typeof(User))
                           .Select(p => (User)p.GetValue(obj))
                           .FirstOrDefault();

            if (user != null && user.UserName.ToLower() == userName.ToLower())
            {
                SetCanAccess(obj, true);
                return obj;
            }
            return null;
        }
        private static object CheckUserAccess_user(object obj,string userName)
        {
            

            var users = obj.GetType().GetProperties()
                         .Where(p => p.PropertyType == typeof(User))
                         .Select(p => (User)p.GetValue(obj))
                         .ToList();
            foreach (var user in users)
            {


                if (user != null && user.UserName.ToLower() == userName.ToLower())
                {
                    SetCanAccess(obj, true);
                    return obj;
                }
            }
            return null;
        }


        public static RequestStep isAllowedToDoAction(this RequestStep step, User user)
        {
            step.CanApprove = false;

            if (step.IsCancelled)
            {
                step.CanApprove = false;
            }
            else
            {
                if (step.Status == (int)EnumWFStatuses.Pending)
                {

                    if (step.IsGroup)
                    {
                        // if user part of group
                        if (user.UsersGroups.Where(w => w.Group.Title.ToLower() == step.Approver.ToLower()).Any(w => w.UserName.ToLower() == user.UserName.ToLower()))
                        {
                            step.CanApprove = true;
                        }
                    }
                    else
                    {
                        //check if same user
                        if (user.UserName.ToLower() == step.Approver.ToLower())
                        {
                            step.CanApprove = true;
                        }
                    }
                }
            }
            return step;
        }
        public static IEnumerable<RequestStep> isAllowedToDoActionList(this IEnumerable<RequestStep> steps, User user)
        {
            foreach (RequestStep step in steps)
            {
                step.CanApprove = false;
                if (step.IsCancelled)
                {
                    step.CanApprove = false;
                }
                else
                {
                    if (step.Status == (int)EnumWFStatuses.Pending)
                    {
                        if (step.IsGroup)
                        {
                            // if user part of group
                            if (user.UsersGroups.Where(w => w.Group.Title.ToLower() == step.Approver.ToLower()).Any(w => w.UserName.ToLower() == user.UserName.ToLower()))
                            {
                                step.CanApprove = true;
                            }
                        }
                        else
                        {
                            //check if same user
                            if (user.UserName.ToLower() == step.Approver.ToLower())
                            {
                                step.CanApprove = true;
                            }
                        }
                    }
                }
            }
            return steps;
        }
    }
}

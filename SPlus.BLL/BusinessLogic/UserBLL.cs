using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using SPlus.DataAccess;
using SPlus.Model.Domain;
using System.Data.Entity;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using model = SPlus.Model;
using System.Drawing;
using SPlus.Helper;
using System.Configuration;
using Z.EntityFramework.Plus;
using SPlus.Model;

namespace SPlus.BLL
{
    public class UserBLL : LoggingBLL
    {
        Container _Container = IOC.InitializeContainer();
        private readonly IUnitOfWorkFactory _factory;
        public UserBLL()
        {
            _factory = _Container.GetInstance<IUnitOfWorkFactory>();

        }


        #region Create 
        public User Create(User user)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                user.UserName = user.UserName.ToLower();
                User User = dataAccess.User.GetAll()
                    .Where(s => s.UserName.ToLower() == user.UserName.ToLower()).SingleOrDefault();
                if (User != null)
                {
                    User.Modified = DateTime.Now;
                    User.Deleted = false;
                    User.UserName = User.UserName.ToLower();
                    dataAccess.User.Save(User);
                    List<UsersGroup> dUserGroups = new List<UsersGroup>();
                    dUserGroups = User.UsersGroups.ToList();
                    foreach (var item in dUserGroups)
                    {
                        dataAccess.UsersGroup.Delete(item);
                    }

                    foreach (var usersGroup in user.UsersGroups)
                    {
                        UsersGroup usergroup = new UsersGroup();
                        usergroup.User = null;
                        usergroup.GroupID = usersGroup.GroupID;
                        usergroup.UserName = user.UserName;
                        dataAccess.UsersGroup.Save(usergroup);
                    }

                }
                else
                {
                    user.Created = DateTime.Now;
                    user.Modified = DateTime.Now;
                    dataAccess.User.Save(user);
                }
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return user;
            }
        }

        public Group CreateGroup(Group group)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                dataAccess.Group.Save(group);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return ReadGroupByID(group.ID);
            }
        }
        #endregion

        #region Read 

        public List<Group> ReadGroup()
        {
            using (var dataAccess = _factory.Create())
            {
                IEnumerable<Group> groups = dataAccess.Group.Query().Include(a => a.UsersGroups.Select(s => s.User)).Include(a => a.Matrices.Select(s => s.Resource)).Include(a => a.Matrices.Select(s => s.Role));
                return groups.ToList();
            }
        }
        public List<User> Read()
        {
            using (var dataAccess = _factory.Create())
            {
                
                IEnumerable<User> user = dataAccess.User.Query()
                    .Include(a => a.ChampionKPIs)
                    .Include(a => a.OwnerKPIs)
                    .Include(a => a.UsersGroups.Select(s => s.Group))
                    .Where(w => !w.Deleted);
                return user.ToList();
            }
        }
        public User ReadByUserName(string user)
        {
            using (var dataAccess = _factory.Create())
            {
                User User = dataAccess.User.Query()
                    .IncludeOptimized(a => a.ChampionKPIs)
                    .IncludeOptimized(a => a.OwnerKPIs)
                    .IncludeOptimized(a => a.Managers)
                    .IncludeOptimized(a => a.UsersGroups)
                    .IncludeOptimizedByPath("UsersGroups.Group")
                    .IncludeOptimizedByPath("UsersGroups.Group.Matrices")
                    .IncludeOptimizedByPath("UsersGroups.Group.Matrices.Resource")
                    .IncludeOptimizedByPath("UsersGroups.Group.Matrices.Role")
                    .Where(s => !s.Deleted && s.UserName.ToLower() == user.ToLower()).SingleOrDefault();
                return User;
            }
        }
        public Group ReadGroupForDelete(int id)
        {
            using (var dataAccess = _factory.Create())
            {
                Group Group = dataAccess.Group.Query()
                    .IncludeOptimized(a => a.UsersGroups.Select(s => s.User))
                    .Where(s => s.ID == id).SingleOrDefault();

                return Group;
            }
        }
        public List<Group> ReadGroupByUserName(string user)
        {
            using (var dataAccess = _factory.Create())
            {
                List<Group> group = dataAccess.Group.Query().Include(a => a.UsersGroups.Select(s => s.User)).Where(s => s.UsersGroups.Where(w => w.UserName == user).Count() > 0).ToList();
                return group;
            }
        }
        public Group ReadGroupByID(int id)
        {
            using (var dataAccess = _factory.Create())
            {
                Group group = dataAccess.Group.Query().Include(a => a.UsersGroups.Select(s => s.User))
                    .Include(a => a.Matrices)
                    .Include(a => a.Matrices.Select(s => s.Resource))
                    .Include(a => a.Matrices.Select(s => s.Role)).Where(s => s.ID == id).FirstOrDefault();
                return group;
            }
        }
        public List<Resource> ReadResources()
        {
            using (var dataAccess = _factory.Create())
            {
                List<Resource> resources = dataAccess.Resource.GetAll().ToList();

                return resources;
            }
        }
        public List<User> Validate(string user)
        {
            List<User> users = null;

            users = GetActiveDirectoryUsers(user);

            return users;
        }
        #endregion

        #region Update 
        public User Update(User user)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                var context = dataAccess.User.Query().Include(a => a.UsersGroups).SingleOrDefault(s => s.UserName.ToLower() == user.UserName.ToLower());

                context.Modified = DateTime.Now;
                context.Department = user.Department;
                List<UsersGroup> usersGroups = new List<UsersGroup>();
                UsersGroup usergroup;
                List<UsersGroup> dUserGroups = new List<UsersGroup>();
                dUserGroups = context.UsersGroups.ToList();
                foreach (var item in dUserGroups)
                {
                    dataAccess.UsersGroup.Delete(item);
                }
                foreach (var usersGroup in user.UsersGroups)
                {
                    usergroup = new UsersGroup();
                    usergroup.User = null;
                    usergroup.GroupID = usersGroup.GroupID;
                    usergroup.UserName = user.UserName;
                    usersGroups.Add(usergroup);
                }
                context.UsersGroups = usersGroups;
                dataAccess.User.Save(context, true);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return context;
            }
        }

        public Group Update(Group group)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                var context = dataAccess.Group.Query().Include(a => a.UsersGroups).Include(a => a.Matrices).SingleOrDefault(s => s.ID == group.ID);


                context.Title = group.Title;
                context.Description = group.Description;
                List<UsersGroup> usersGroups = new List<UsersGroup>();
                UsersGroup usergroup;
                List<UsersGroup> dUserGroups = new List<UsersGroup>();
                Matrix matrix;
                List<Matrix> dMatrices = new List<Matrix>();
                List<Matrix> matrices = new List<Matrix>();
                dUserGroups = context.UsersGroups.ToList();
                dMatrices = context.Matrices.ToList();

                foreach (var item in dUserGroups)
                {
                    dataAccess.UsersGroup.Delete(item);
                }
                foreach (var usersGroup in group.UsersGroups)
                {
                    usergroup = new UsersGroup();
                    usergroup.GroupID = group.ID;
                    usergroup.UserName = usersGroup.UserName;
                    usersGroups.Add(usergroup);
                }
                foreach (var item in dMatrices)
                {
                    dataAccess.Matrix.Delete(item);
                }
                foreach (var mat in group.Matrices)
                {
                    matrix = new Matrix();
                    matrix.GroupID = group.ID;
                    matrix.ResourceID = mat.ResourceID;
                    matrix.RoleID = mat.RoleID;
                    matrices.Add(matrix);
                }

                context.UsersGroups = usersGroups;
                context.Matrices = matrices;
                dataAccess.Group.Save(context);
                result = dataAccess.Complete();
                dataAccess.Dispose();
                return ReadGroupByID(context.ID);
            }
        }

        public bool ReplaceUser(string oldUser, string newUser)
        {
            int result = 0;
            oldUser = oldUser.ToLower();
            newUser = newUser.ToLower();
            using (var dataAccess = _factory.Create())
            {
                // KPIs
                List<KPI> kpis = dataAccess.KPI.Query().Where(a => a.Champion.ToLower() == oldUser.ToLower()
                    || a.Owner.ToLower() == oldUser.ToLower()
                    ).ToList();
                foreach (var item in kpis)
                {
                    if (item.Champion.ToLower() == oldUser.ToLower())
                        item.Champion = newUser;

                    if (item.Owner.ToLower() == oldUser.ToLower())
                        item.Owner = newUser;

                    dataAccess.KPI.Save(item);
                }

                // Request Steps
                List<RequestStep> steps = dataAccess.RequestStep.Query()
                   .Where(a => a.Status == (int)model.EnumWFStatuses.Pending &&
                   !a.IsGroup &&
                   a.Approver.ToLower() == oldUser.ToLower()).ToList();
                foreach (var item in steps)
                {
                    item.Approver = newUser;
                    dataAccess.RequestStep.Save(item);
                }

                //Groups
                List<UsersGroup> usersGroups = dataAccess.UsersGroup.Query().Where(a => a.UserName.ToLower() == oldUser.ToLower()).ToList();
                List<UsersGroup> newUsersGroups = dataAccess.UsersGroup.Query().Where(a => a.UserName.ToLower() == newUser.ToLower()).ToList();
                foreach (var item in usersGroups)
                {
                    if (newUsersGroups.Select(s => s.GroupID).Contains(item.GroupID))
                        continue;
                    item.UserName = newUser;
                    dataAccess.UsersGroup.Save(item);
                }
                foreach (var item in usersGroups)
                {
                    if (newUsersGroups.Select(s => s.GroupID).Contains(item.GroupID))
                        dataAccess.UsersGroup.Delete(item);
                }

                //Delegations
                List<Delegation> delegations = dataAccess.Delegation.Query().Where(a => a.FromUser.ToLower() == oldUser.ToLower() ||
                a.ToUser.ToLower() == oldUser.ToLower()).ToList();
                foreach (var item in delegations)
                {
                    dataAccess.Delegation.Delete(item);
                }

                result = dataAccess.Complete();
                if (result > 1)
                    return true;
                else
                    return false;

            }
        }
        #endregion

        #region Delete 

        public bool Delete(string user)
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                User User = ReadByUserName(user);
                List<UsersGroup> usersGroups = dataAccess.UsersGroup.GetAll().ToList();
                usersGroups = usersGroups.Where(w => w.UserName == user).ToList();
                if (User != null)
                {
                    User.Deleted = true;
                    foreach (var usersGroup in usersGroups)
                    {
                        dataAccess.UsersGroup.Delete(usersGroup);
                    }
                    dataAccess.User.Save(User);
                    result = dataAccess.Complete();
                    dataAccess.Dispose();
                }
                if (result >= 1)
                    return true;
                else
                    return false;
            }
        }
        public bool DeleteForGroup(int id)
        {
            int result = 0;
            using (var dataAccess = _factory.Create())
            {
                Group Group = dataAccess.Group.Query()
                    .IncludeOptimized(a => a.UsersGroups.Select(s => s.User))
                    .Where(s => s.ID == id).SingleOrDefault();
                List<UsersGroup> usersGroups = dataAccess.UsersGroup.Query().ToList();
                usersGroups = usersGroups.Where(w => w.GroupID == id).ToList();
                if (Group != null)
                {
                    foreach (var usersGroup in usersGroups)
                    {
                        dataAccess.UsersGroup.Delete(usersGroup);
                    }
                    dataAccess.Group.Delete(Group);
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

        #region User Photo

        public HttpResponseMessage GetUserPhoto(string userName)
        {

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            PrincipalContext domainContext;
            PrincipalContext domainContext1 = new PrincipalContext(ContextType.Domain);
            if (string.IsNullOrWhiteSpace(Constants._DomainName))
                domainContext = new PrincipalContext(ContextType.Domain);
            else
                domainContext = new PrincipalContext(ContextType.Domain, Constants._DomainName);

            try
            {
                using (domainContext)
                {
                    UserPrincipal user = UserPrincipal.FindByIdentity(domainContext, userName);
                    using (var foundUser = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, user.SamAccountName))
                    {
                        if (foundUser != null)
                        {
                            DirectoryEntry directoryEntry = foundUser.GetUnderlyingObject() as DirectoryEntry;
                            if (directoryEntry.Properties["thumbnailPhoto"].Value != null)
                            {
                                Image image;
                                byte[] bytes = directoryEntry.Properties["thumbnailPhoto"].Value as byte[];
                                using (MemoryStream ms = new MemoryStream(bytes))
                                {
                                    if (Constants._UserImageExtenstion.ToLower() == "jpg")
                                    {
                                        image = Image.FromStream(ms);
                                        //image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                                        result.Content = new ByteArrayContent(ms.ToArray());
                                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");

                                    }
                                    else if (Constants._UserImageExtenstion.ToLower() == "png")
                                    {
                                        image = Image.FromStream(ms);
                                        //image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                        result.Content = new ByteArrayContent(ms.ToArray());
                                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("png");
                                    }




                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                using (domainContext1)
                {
                    UserPrincipal user = UserPrincipal.FindByIdentity(domainContext1, userName);
                    using (var foundUser = UserPrincipal.FindByIdentity(domainContext1, IdentityType.SamAccountName, user.SamAccountName))
                    {
                        if (foundUser != null)
                        {
                            DirectoryEntry directoryEntry = foundUser.GetUnderlyingObject() as DirectoryEntry;
                            if (directoryEntry.Properties["thumbnailPhoto"].Value != null)
                            {
                                Image image;
                                byte[] bytes = directoryEntry.Properties["thumbnailPhoto"].Value as byte[];
                                using (MemoryStream ms = new MemoryStream(bytes))
                                {
                                    if (Constants._UserImageExtenstion.ToLower() == "jpg")
                                    {
                                        image = Image.FromStream(ms);
                                        image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                        result.Content = new ByteArrayContent(ms.ToArray());
                                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                                    }
                                    else if (Constants._UserImageExtenstion.ToLower() == "png")
                                    {
                                        image = Image.FromStream(ms);
                                        image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                        result.Content = new ByteArrayContent(ms.ToArray());
                                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                                    }

                                }
                            }

                        }
                    }
                }
            }
            return result;
        }

        #endregion

        #region Hangfire
        public List<User> GetActiveDirectoryUsers(string search)
        {

            List<User> users = new List<User>();

            using (var domainContext = new PrincipalContext(ContextType.Domain, Environment.UserDomainName))
            {
                search = $"{search}*";
                var searchPrinciples = new List<UserPrincipal>
                {
                    new UserPrincipal(domainContext) {DisplayName = search},
                    new UserPrincipal(domainContext) {Name = search},
                    new UserPrincipal(domainContext) {Surname = search},
                    new UserPrincipal(domainContext) {GivenName = search},
                    new UserPrincipal(domainContext) {MiddleName = search},
                    new UserPrincipal(domainContext) {SamAccountName = search}
                };

                foreach (var item in searchPrinciples)
                {
                    var searcher = new PrincipalSearcher(item);
                    foreach (var result in searcher.FindAll())
                    {
                        User _User = new User();
                        DirectoryEntry directoryEntry = result.GetUnderlyingObject() as DirectoryEntry;
                        if (directoryEntry.Properties["displayName"].Value != null)
                        {
                            if (directoryEntry.Properties["department"].Value != null)
                            {
                                _User.Department = directoryEntry.Properties["department"].Value.ToString();
                            }
                            else
                            {
                                _User.Department = "";
                            }
                            if (directoryEntry.Properties["mobile"].Value != null)
                            {
                                _User.PhoneNumber = directoryEntry.Properties["mobile"].Value.ToString();
                            }
                            else
                            {
                                _User.PhoneNumber = "";
                            }
                            if (directoryEntry.Properties["mail"].Value != null)
                            {
                                _User.Email = directoryEntry.Properties["mail"].Value.ToString() != null ? directoryEntry.Properties["mail"].Value.ToString() : string.Empty;
                            }
                            else
                            {
                                _User.Email = "";
                            }
                            if (directoryEntry.Properties["displayName"].Value != null)
                            {
                                _User.DisplayName = directoryEntry.Properties["displayName"].Value.ToString() != null ? directoryEntry.Properties["displayName"].Value.ToString() : string.Empty;
                            }
                            else
                            {
                                _User.DisplayName = "";
                            }
                            if (directoryEntry.Properties["sAMAccountName"].Value != null)
                            {
                                _User.UserName = directoryEntry.Properties["sAMAccountName"].Value.ToString();
                            }
                            if (!users.Select(s => s.UserName).Contains(_User.UserName))
                                users.Add(_User);
                        }
                    }
                }
            }
            return users;
        }
        public List<User> GetActiveDirectoryUsers()
        {

            List<User> users = new List<User>();

            using (var domainContext = new PrincipalContext(ContextType.Domain, Environment.UserDomainName))
            {
                using (var searcher = new PrincipalSearcher(new UserPrincipal(domainContext)))
                {
                    foreach (var result in searcher.FindAll())
                    {
                        User _User = new User();
                        DirectoryEntry directoryEntry = result.GetUnderlyingObject() as DirectoryEntry;
                        if (directoryEntry.Properties["displayName"].Value != null)
                        {
                            if (directoryEntry.Properties["department"].Value != null)
                            {
                                _User.Department = directoryEntry.Properties["department"].Value.ToString();
                            }
                            else
                            {
                                _User.Department = "";
                            }
                            if (directoryEntry.Properties["mobile"].Value != null)
                            {
                                _User.PhoneNumber = directoryEntry.Properties["mobile"].Value.ToString();
                            }
                            else
                            {
                                _User.PhoneNumber = "";
                            }
                            if (directoryEntry.Properties["mail"].Value != null)
                            {
                                _User.Email = directoryEntry.Properties["mail"].Value.ToString() != null ? directoryEntry.Properties["mail"].Value.ToString() : string.Empty;
                            }
                            else
                            {
                                _User.Email = "";
                            }
                            if (directoryEntry.Properties["displayName"].Value != null)
                            {
                                _User.DisplayName = directoryEntry.Properties["displayName"].Value.ToString() != null ? directoryEntry.Properties["displayName"].Value.ToString() : string.Empty;
                            }
                            else
                            {
                                _User.DisplayName = "";
                            }
                            if (directoryEntry.Properties["sAMAccountName"].Value != null)
                            {
                                _User.UserName = directoryEntry.Properties["sAMAccountName"].Value.ToString();
                            }
                            users.Add(_User);
                        }


                    }
                }
            }
            return users;
        }
        public void SetUsersDeleted(List<User> UsersToDelete)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                foreach (var user in UsersToDelete)
                {
                    user.Deleted = true;
                    dataAccess.User.Save(user);
                }
                dataAccess.Complete();
            }
        }
        public void UpdateDatabaseUsers(List<User> UpdatedUsers)
        {
            int result;
            using (var dataAccess = _factory.Create())
            {
                foreach (var user in UpdatedUsers)
                {
                    dataAccess.User.Save(user);
                }
                dataAccess.Complete();
            }
        }
        public void CleanResources()
        {
            using (var dataAccess = _factory.Create())
            {
                List<Resource> resources = dataAccess.Resource.Query().ToList();
                List<KPIType> KPITypes = dataAccess.KPIType.Query().ToList();

                List<Resource> resourcesToDelete = new List<Resource>();

                //    resources.Where(w =>
                //!w.IsScreen &&
                //((!KPITypes.Any(a => a.KPITypeID == w.PropertyValue && w.LevelID == (int)LevelTypeEnum.KPI)) &&
                //!OutcomeTypes.Any(a => a.OutcomeTypeID == w.PropertyValue && w.LevelID == (int)LevelTypeEnum.Outcome))
                //).ToList();

                foreach (var resource in resources)
                {
                    if (!resource.IsScreen)
                    {
                        if (!KPITypes.Any(a => a.KPITypeID == resource.PropertyValue && resource.LevelID == (int)LevelTypeEnum.KPI))
                        {
                            resourcesToDelete.Add(resource);
                        }

                        var duplicates = resources.Where(w => w.PropertyName == resource.PropertyName && w.PropertyValue == resource.PropertyValue && resource.LevelID == w.LevelID).ToList();
                        if (duplicates.Any() && duplicates.Count() > 1)
                        {
                            int DuplicateCount = duplicates.Count();
                            duplicates = duplicates.Take(DuplicateCount - 1).ToList();
                            resourcesToDelete.AddRange(duplicates);
                        }
                    }
                }

                foreach (var resource in resourcesToDelete)
                {
                    dataAccess.Resource.Delete(resource);
                }
                dataAccess.Complete();
            }
        }
        #endregion

    }
}

using SPlus.BLL;
using SPlus.DTO;
using SPlus.Helper;
using SPlus.Model.Domain;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;

namespace SPlus.UseCases
{
    public class UserUseCases : LoggingUseCases
    {
        static readonly string AdminGroup = Constants._AdminGroup;
        static readonly string ChampionGroup = Constants._ChampionGroup;
        static readonly bool isLocal = Convert.ToBoolean(ConfigurationManager.AppSettings["isLocal"]);
        Container _Container = IOC.InitializeContainer();
        private readonly UserBLL UserBLL;
        private readonly RequestBLL RequestBLL;
        private readonly OrgStructureBLL OrgStructureBLL;
        public UserUseCases()
        {
            UserBLL = _Container.GetInstance<UserBLL>();
            RequestBLL = _Container.GetInstance<RequestBLL>();
            OrgStructureBLL = _Container.GetInstance<OrgStructureBLL>();
        }


        #region Create 
        public UserDTO Create(UserDTO user)
        {
            User User = UserBLL.ReadByUserName(user.UserName);
            if (User == null)
            {
                var res = AutoMapper.Mapper.Map<User>(user);
                return AutoMapper.Mapper.Map<UserDTO>(UserBLL.Create(res));
            }
            else
            {
                throw new System.Exception("User Already Exist.");
            }
        }

        public GroupListDTO CreateGroup(SingularGroupDTO group)
        {
            List<Group> groups = UserBLL.ReadGroup();
            var currentGroup = groups.Where(w => w.Title == group.Title).FirstOrDefault();
            if (currentGroup == null)
            {
                var res = AutoMapper.Mapper.Map<Group>(group);
                //List<WorkflowStep> workflowSteps = RequestBLL.GetWorklowSteps().ToList();
                //  List<RequestStep> pendingsteps = RequestBLL.GetAllPendingAndNewTasks().ToList();

                var result = UserBLL.CreateGroup(res);
                result = SetGroupIsDeletable(result, new List<WorkflowStep>(), new List<RequestStep>(), new List<OrgStructure>());

                var Group = AutoMapper.Mapper.Map<GroupListDTO>(result);
                var UsersGroups = result.UsersGroups;
                Group.UsersCount = UsersGroups.Count();
                if (group.Title.ToLower() == AdminGroup.ToLower())
                {
                    Group.IsDeletable = false;
                    Group.IsReadOnly = true;
                    Group.IsEditable = false;
                }
                else
                {
                    Group.IsReadOnly = false;
                    Group.IsEditable = true;
                }
                return Group;
            }
            else
            {
                throw new System.Exception("Group Already Exist");
            }

        }
        #endregion

        #region Read 

        public List<GroupListDTO> ReadGroup()
        {
            var res = UserBLL.ReadGroup();

            List<WorkflowStep> workflowSteps = RequestBLL.GetWorklowSteps().ToList();
            List<RequestStep> pendingsteps = RequestBLL.GetAllPendingAndNewTasks().ToList();
            List<OrgStructure> alOrg = OrgStructureBLL.ReadAll();
            res = SetGroupIsDeletable(res, workflowSteps, pendingsteps, alOrg);

            var groupedUser = AutoMapper.Mapper.Map<List<SingularGroupDTO>>(res);
            var groups = AutoMapper.Mapper.Map<List<GroupListDTO>>(res);

            foreach (var group in groups)
            {

                var UsersGroups = res.Where(w => w.ID == group.ID).SelectMany(s => s.UsersGroups).ToList();
                group.UsersCount = UsersGroups.Count();
                if (group.Title.ToLower() == AdminGroup.ToLower())
                {
                    group.IsDeletable = false;
                    group.IsReadOnly = true;
                    group.IsEditable = false;
                }
                else
                {
                    group.IsReadOnly = false;
                    group.IsEditable = true;
                }
            }
            return groups;

        }
        public List<UserListDTO> Read()
        {
            var res = UserBLL.Read();
            List<RequestStep> pendingsteps = RequestBLL.GetAllPendingAndNewTasks().ToList();
            res = SetUserIsDeletable(res, pendingsteps);
            List<UserListDTO> Users = AutoMapper.Mapper.Map<List<UserListDTO>>(res);
            return Users;

        }
        public List<UserListDTO> GetChampionUsers()
        {
            var res = UserBLL.Read().Where(a => a.UsersGroups.Any(g => g.Group?.Title.ToLower() == ChampionGroup.ToLower())).ToList();
            List<RequestStep> pendingsteps = RequestBLL.GetAllPendingAndNewTasks().ToList();
            res = SetUserIsDeletable(res, pendingsteps);
            List<UserListDTO> Users = AutoMapper.Mapper.Map<List<UserListDTO>>(res);
            return Users;

        }



        public UserDTO ReadByUserName(string user)
        {
            var res = UserBLL.ReadByUserName(user);
            List<RequestStep> pendingsteps = RequestBLL.GetAllPendingAndNewTasks().Where(w => w.IsGroup && w.Approver.ToLower() == res.UserName.ToLower()).ToList();
            res = SetUserIsDeletable(res, pendingsteps);

            var User = AutoMapper.Mapper.Map<UserDTO>(res);
            return User;
        }
        public List<SingularGroupDTO> ReadGroupByUserName(string user)
        {
            var res = UserBLL.ReadGroupByUserName(user);

            List<WorkflowStep> workflowSteps = RequestBLL.GetWorklowSteps().ToList();
            List<RequestStep> pendingsteps = RequestBLL.GetAllPendingAndNewTasks().ToList();
            List<OrgStructure> allOrg = OrgStructureBLL.ReadAll();
            res = SetGroupIsDeletable(res, workflowSteps, pendingsteps, allOrg);
            var groups = AutoMapper.Mapper.Map<List<SingularGroupDTO>>(res);
            foreach (var group in groups)
            {
                if (group.Title.ToLower() == AdminGroup.ToLower())
                {
                    group.IsReadOnly = true;
                    group.IsEditable = false;
                }
                else
                {
                    group.IsReadOnly = false;
                    group.IsEditable = true;
                }
            }
            return groups;
        }
        public SingularGroupDTO ReadGroupByID(int id)
        {
            var res = UserBLL.ReadGroupByID(id);
            List<WorkflowStep> workflowSteps = RequestBLL.GetWorklowSteps().ToList();
            List<RequestStep> pendingsteps = RequestBLL.GetAllPendingAndNewTasks().ToList();
            List<OrgStructure> allOrg = OrgStructureBLL.ReadAll();
            res = SetGroupIsDeletable(res, workflowSteps, pendingsteps, allOrg);
            var group = AutoMapper.Mapper.Map<SingularGroupDTO>(res);

            if (group.Title.ToLower() == AdminGroup.ToLower())
            {
                group.IsReadOnly = true;
                group.IsEditable = false;
            }
            else
            {
                group.IsReadOnly = false;
                group.IsEditable = true;
            }
            return group;


        }
        public List<ResourceDTO> ReadResources()
        {
            return AutoMapper.Mapper.Map<List<ResourceDTO>>(UserBLL.ReadResources());
        }
        public List<UserDTO> Validate(string user)
        {
            var res = UserBLL.Validate(user);
            return AutoMapper.Mapper.Map<List<UserDTO>>(res);

        }
        #endregion

        #region Update 
        public UserDTO Update(UserDTO user)
        {
            var res = AutoMapper.Mapper.Map<User>(user);


            return AutoMapper.Mapper.Map<UserDTO>(UserBLL.Update(res));
        }

        public GroupListDTO Update(SingularGroupDTO group)
        {
            var res = AutoMapper.Mapper.Map<Group>(group);

            List<WorkflowStep> workflowSteps = RequestBLL.GetWorklowSteps().ToList();
            List<RequestStep> pendingsteps = RequestBLL.GetAllPendingAndNewTasks().ToList();
            List<OrgStructure> allOrg = OrgStructureBLL.ReadAll();
            var result = UserBLL.Update(res);

            result = SetGroupIsDeletable(result, workflowSteps, pendingsteps, allOrg);

            var Group = AutoMapper.Mapper.Map<GroupListDTO>(result);
            var UsersGroups = result.UsersGroups;
            Group.UsersCount = UsersGroups.Count();
            if (group.Title.ToLower() == AdminGroup.ToLower())
            {
                Group.IsDeletable = false;
                Group.IsReadOnly = true;
                Group.IsEditable = false;
            }
            else
            {
                Group.IsReadOnly = false;
                Group.IsEditable = true;
            }
            return Group;
        }

        public bool ReplaceUser(ReplaceUserDTO user)
        {
            return UserBLL.ReplaceUser(user.User, user.NewUser);
        }
        #endregion

        #region Delete 
        #region Set IsDeletable
        private User SetUserIsDeletable(User user, List<RequestStep> pendingsteps)
        {
            bool CanDelete = false;

            pendingsteps = pendingsteps.Where(w => !w.IsGroup && w.Approver.ToLower() == user.UserName.ToLower()).ToList();
            Group group = UserBLL.ReadGroup().Where(a => a.Title.ToLower() == AdminGroup.ToLower()).FirstOrDefault();
            if (group.UsersGroups.Where(a => a.UserName.ToLower() == user.UserName.ToLower()).Count() > 0 && group.UsersGroups.Count() > 1)
            {
                CanDelete = true;
            }
            else if (group.UsersGroups.Where(a => a.UserName.ToLower() == user.UserName.ToLower()).Count() == 0)
                CanDelete = true;
            else
                CanDelete = false;

            if (user.ChampionKPIs.Count() > 0 || user.OwnerKPIs.Count() > 0 || pendingsteps.Count() > 0 || !CanDelete)
            {
                user.IsDeletable = false;
                return user;
            }
            else
            {
                user.IsDeletable = true;
                return user;
            }
        }
        private List<User> SetUserIsDeletable(List<User> users, List<RequestStep> pendingsteps)
        {
            List<RequestStep> tempstep = new List<RequestStep>();
            Group group = UserBLL.ReadGroup().Where(a => a.Title.ToLower() == AdminGroup.ToLower()).FirstOrDefault();
            foreach (var user in users)
            {
                tempstep = pendingsteps.Where(w => !w.IsGroup && w.Approver.ToLower() == user.UserName.ToLower()).ToList();
                bool CanDelete = false;

                if (group.UsersGroups.Where(a => a.UserName.ToLower() == user.UserName.ToLower()).Count() > 0 && group.UsersGroups.Count() > 1)
                {
                    CanDelete = true;
                }
                else if (group.UsersGroups.Where(a => a.UserName.ToLower() == user.UserName.ToLower()).Count() == 0)
                    CanDelete = true;
                else
                    CanDelete = false;
                if (user.ChampionKPIs.Count() > 0 || user.OwnerKPIs.Count() > 0 || tempstep.Count() > 0 || !CanDelete)
                {
                    user.IsDeletable = false;
                }
                else
                {
                    user.IsDeletable = true;
                }

            }
            return users;

        }

        private Group SetGroupIsDeletable(Group Group, List<WorkflowStep> workflowSteps, List<RequestStep> pendingsteps, List<OrgStructure> allOrg)
        {
            workflowSteps = workflowSteps.Where(w => w.IsGroup && w.Approver == Group.ID).ToList();
            pendingsteps = pendingsteps.Where(w => w.IsGroup && w.Approver.ToLower() == Group.Title.ToLower()).ToList();
           
            bool CanDelete = false;

            if (Group.Title.ToLower() != AdminGroup.ToLower())
            {
                CanDelete = true;
            }
            if (workflowSteps.Count() > 0 || pendingsteps.Count() > 0 || allOrg.Count>0|| !CanDelete)
            {
                Group.IsDeletable = false;
                return Group;
            }
            else
            {
                Group.IsDeletable = true;
                return Group;
            }
        }
        private List<Group> SetGroupIsDeletable(List<Group> Groups, List<WorkflowStep> workflowSteps, List<RequestStep> pendingsteps, List<OrgStructure> allorgstructures)
        {
            List<RequestStep> tempstep = new List<RequestStep>();
            List<WorkflowStep> tempwfstep = new List<WorkflowStep>();
            List<OrgStructure> orgStructures = new List<OrgStructure>();

            foreach (var Group in Groups)
            {
                tempstep = new List<RequestStep>();
                tempwfstep = new List<WorkflowStep>();
                tempwfstep = workflowSteps.Where(w => w.IsGroup && w.Approver == Group.ID).ToList();
                tempstep = pendingsteps.Where(w => w.IsGroup && w.Approver.ToLower() == Group.Title.ToLower()).ToList();
                orgStructures= allorgstructures.Where(w => w.GroupID.HasValue&& w.GroupID.Value==Group.ID).ToList();
                bool CanDelete = false;
                if (Group.Title.ToLower() != AdminGroup.ToLower() && Group.Title.ToLower() != ChampionGroup.ToLower())
                {
                    CanDelete = true;
                }
                if (tempwfstep.Count() > 0 || tempstep.Count() > 0 || orgStructures.Count>0 || !CanDelete)
                {
                    Group.IsDeletable = false;
                }
                else
                {
                    Group.IsDeletable = true;
                }
            }
            return Groups;

        }
        #endregion

        public bool Delete(string userName)
        {
            var pendingsteps = RequestBLL.GetAllPendingAndNewTasks();
            var User = UserBLL.ReadByUserName(userName);
            if (User != null)
            {
                var IsDeletable = SetUserIsDeletable(User, pendingsteps).IsDeletable;
                if (!IsDeletable)
                {
                    throw new System.Exception("The User is a Champion/Owner/Sponsor in a KPI or Has a pending Task(s) Or the only User in Admin Group");
                }
                else
                {
                    return UserBLL.Delete(userName);
                }
            }
            else
            {
                throw new System.Exception("The User is Deleted or Does not exist.");
            }
        }
        public bool DeleteGroup(int id)
        {
            List<WorkflowStep> workflowSteps = RequestBLL.GetWorklowSteps();
            List<RequestStep> pendingSteps = RequestBLL.GetAllPendingAndNewTasks();
            List<OrgStructure> allOrg = OrgStructureBLL.ReadAll();
            allOrg = allOrg.Where(w => w.GroupID.HasValue && w.GroupID.Value == id).ToList();
            Group group = SetGroupIsDeletable(UserBLL.ReadGroupForDelete(id), workflowSteps, pendingSteps, allOrg);
            bool IsDeletable = group.IsDeletable;
            if (!IsDeletable)
            {
                throw new System.Exception("The group is currently engaged in a workflow step, either with a pending task or assigned to an organizational structure");
            }
            else
            {
                return UserBLL.DeleteForGroup(id);
            }
        }
        #endregion


        #region Hangfire

        public void UpdateUsersFromActiveDirectory()
        {
            try
            {
                if (!isLocal)
                {
                    //get database users
                    List<User> ActiveDirectoryUsers = UserBLL.GetActiveDirectoryUsers();
                    List<User> DatabaseUsers = UserBLL.Read();

                    //get active directory users
                    List<User> UsersToDelete = new List<User>();
                    List<User> UsersToUpdate = new List<User>();
                    //match active directory users with the data base users
                    foreach (var ActiveDirectoryUser in ActiveDirectoryUsers)
                    {
                        //if the user exist in both then map the user from the active directory and update in the database
                        User UserToUpdate = new User();
                        UserToUpdate = DatabaseUsers.Where(w => w.UserName.ToLower() == ActiveDirectoryUser.UserName.ToLower()).FirstOrDefault();
                        if (UserToUpdate != null)
                        {
                            UserToUpdate.DisplayName = ActiveDirectoryUser.DisplayName;
                            UserToUpdate.Email = ActiveDirectoryUser.Email;
                            UserToUpdate.PhoneNumber = ActiveDirectoryUser.PhoneNumber;
                            UserToUpdate.UserProfilePicture = ActiveDirectoryUser.UserProfilePicture;
                            UserToUpdate.Modified = DateTime.Now;
                            UserToUpdate.UserName = UserToUpdate.UserName.ToLower();
                            UsersToUpdate.Add(UserToUpdate);
                        }
                    }
                    //if user exist in data base and not in the active directory then set Deleted to true
                    UsersToDelete = DatabaseUsers.Where(w => !UsersToUpdate.Any(s => s.UserName.ToLower() == w.UserName.ToLower())).ToList();
                    foreach (var UsertoDelete in UsersToDelete)
                    {
                        //SetDeleted True
                        if (UsertoDelete != null)
                        {
                            UsertoDelete.Modified = DateTime.Now;
                            UsertoDelete.Deleted = true;
                        }
                    }
                    UserBLL.UpdateDatabaseUsers(UsersToUpdate);
                    UserBLL.SetUsersDeleted(UsersToDelete);
                }
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("An Error has occured while Updating Users From Active Directory");
            }
        }

        public void CleanResources()
        {
            UserBLL.CleanResources();
        }
        #endregion

        #region User Photo

        public HttpResponseMessage GetUserPhoto(string userName)
        {
            return UserBLL.GetUserPhoto(userName);
        }

        #endregion

    }
}
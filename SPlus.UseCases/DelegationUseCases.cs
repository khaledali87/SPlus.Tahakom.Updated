using AutoMapper;
using SPlus.BLL;
using SPlus.DTO;
using SPlus.DTO.Delegation;
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
    public class DelegationUseCases : LoggingUseCases
    {
        Container _Container = IOC.InitializeContainer();
        private readonly DelegationBLL DelegationBLL;
        private readonly NotificationConfigurationBLL NotificationConfigurationBLL;
        private readonly ReminderRegistryBLL ReminderRegistryBLL;
        private readonly UserBLL UserBLL;
        public DelegationUseCases()
        {
            DelegationBLL = _Container.GetInstance<DelegationBLL>();
            NotificationConfigurationBLL = _Container.GetInstance<NotificationConfigurationBLL>();
            ReminderRegistryBLL = _Container.GetInstance<ReminderRegistryBLL>();
            UserBLL = _Container.GetInstance<UserBLL>();
        }

        #region Create

        public DelegationDTO Create(string userName, DelegationDTO delegation)
        {
            var res = AutoMapper.Mapper.Map<Delegation>(delegation);


            if (ValidateDelegationPeriod(res))
            {
                var result = DelegationBLL.Create(res);
                Task.Run(() => NotificationConfigurationBLL.SendNotificationForDelegation(result.ID, enumNotificationEventType.AddDelegation));
                return AutoMapper.Mapper.Map<DelegationDTO>(DelegationBLL.ReadByID(result.ID));
            }
            else
            {
                throw new System.Exception("Delegation Period is not valid.");
            }

        }

        #endregion

        #region Read
        public SystemDelegationDTO Read(string userName)
        {
            string AdminGroup = Constants._AdminGroup;
            bool IsAdmin = false;
            SystemDelegationDTO delegationPage = new SystemDelegationDTO();
            UserDTO CurrentUser = Mapper.Map<UserDTO>(UserBLL.ReadByUserName(userName));
            if (CurrentUser.Groups.Select(s => s.Title.ToLower()).Contains(AdminGroup.ToLower()))
            {
                IsAdmin = true;
            }
            if (IsAdmin)
                delegationPage.AdminActiveDelegations = Mapper.Map<List<DelegationDTO>>(DelegationBLL.ReadForAdmin());
            else
                delegationPage.AdminActiveDelegations = new List<DelegationDTO>();
            delegationPage.Delegations = AutoMapper.Mapper.Map<List<DelegationDTO>>(DelegationBLL.Read(userName));
            return delegationPage;

        }

        public DelegationDTO ReadByID(int ID)
        {
            return AutoMapper.Mapper.Map<DelegationDTO>(DelegationBLL.ReadByID(ID));
        }

        public List<ActiveDelegationDTO> GetActiveDelegations(string userName)
        {
            List<ActiveDelegationDTO> activeDelegations = new List<ActiveDelegationDTO>();
            var res = DelegationBLL.GetActiveDelegations(userName);
            var delegations = Mapper.Map<List<ActiveDelegationDTO>>(res);
            return delegations;
        }

        #endregion

        #region Update
        public DelegationDTO Update(DelegationDTO delegation)
        {
            var res = AutoMapper.Mapper.Map<Delegation>(delegation);

            if (ValidateDelegationPeriod(res))
            {
                var result = DelegationBLL.Update(res);
                Task.Run(() => NotificationConfigurationBLL.SendNotificationForDelegation(result.ID, enumNotificationEventType.UpdateDelegation));
                return AutoMapper.Mapper.Map<DelegationDTO>(DelegationBLL.ReadByID(result.ID));
            }
            else
            {
                throw new System.Exception("Delegation Period is not valid.");
            }
        }
        public List<DelegationDTO> Update(List<DelegationDTO> delegations)
        {
            var res = AutoMapper.Mapper.Map<List<Delegation>>(delegations);

            return AutoMapper.Mapper.Map<List<DelegationDTO>>(DelegationBLL.Update(res));
        }

        public ActivatedDelegationDTO ActivateDelegation(int delegationID, string token, string SSOtoken)
        {
            UserBLL _UserBLL = new UserBLL();
            string AdminGroup = Constants._AdminGroup;
            List<ResourceDTO> resources = AutoMapper.Mapper.Map<List<ResourceDTO>>(_UserBLL.ReadResources());
            ActivatedDelegationDTO User = new ActivatedDelegationDTO();

            bool ssoEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["ssoEnabled"]);

            string[] credentials = Encryption.GetCredentialsFromSecurityToken(token);
            Guid sessionid = Guid.Parse(credentials[3]);
            if (!ssoEnabled)
            {
                if (credentials.Length > 0 && !string.IsNullOrWhiteSpace(credentials[0]) && !string.IsNullOrWhiteSpace(credentials[1]))
                {
                    Delegation delegation = DelegationBLL.ReadByID(delegationID);
                    if (delegation != null)
                    {
                        string updatedToken = Encryption.GenerateSecurityToken(credentials[0], credentials[1], sessionid, delegation.DelegatorUser.UserName);
                        User.Token = updatedToken;


                        UserDTO CurrentUser = AutoMapper.Mapper.Map<UserDTO>(_UserBLL.ReadByUserName(delegation.FromUser));
                        List<MatrixDTO> matrices = CurrentUser.Groups.SelectMany(s => s.Matrices).ToList();
                        User.UserData = CurrentUser;
                        List<string> Screens = new List<string>();
                        if (CurrentUser.Groups.Select(s => s.Title.ToLower()).Any(a => a == AdminGroup.ToLower()))
                        {
                            Screens = resources.Where(w => w.IsScreen).Select(s => s.Code).ToList();
                            User.UserData.isAdmin = true;

                        }


                        foreach (var matrix in matrices)
                        {
                            if (matrix.Resource.IsScreen)
                            {
                                if (!Screens.Contains(matrix.Resource.Code))
                                {
                                    Screens.Add(matrix.Resource.Code);
                                }
                            }
                        }

                        User.Screens = Screens;
                        //string USERNAME = Encryption.GetCurrentUser(updatedToken); 
                        //UserDTO CurrentUser = AutoMapper.Mapper.Map<UserDTO>(_UserBLL.ReadByUserName(USERNAME));
                        //List<MatrixDTO> matrices = CurrentUser.Groups.SelectMany(s => s.Matrices).ToList();
                        //User.UserData = CurrentUser;
                        //List<string> Screens = new List<string>();
                        //foreach (var matrix in matrices)
                        //{
                        //    if (matrix.Resource.IsScreen)
                        //    {
                        //        if (!Screens.Contains(matrix.Resource.Code))
                        //        {
                        //            Screens.Add(matrix.Resource.Code);
                        //        }
                        //    }
                        //User.Screens = Screens;
                        //}

                        return User;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            else
            {
                if (credentials.Length > 0 && !string.IsNullOrWhiteSpace(credentials[0]))
                {
                    Delegation delegation = DelegationBLL.ReadByID(delegationID);
                    if (delegation != null)
                    {
                        string updatedToken = Encryption.GenerateSecurityTokenSSO(credentials[0], sessionid, delegation.DelegatorUser.UserName);
                        //Encryption.GenerateSecurityToken(credentials[0],"", sessionid, delegation.DelegatorUser.UserName);

                        //  User.Token = Encryption.GenerateSecurityTokenSSO(username, UserSessionID);
                        User.Token = updatedToken;
                        User.SSOToken = SSOtoken;

                        UserDTO CurrentUser = AutoMapper.Mapper.Map<UserDTO>(_UserBLL.ReadByUserName(delegation.FromUser));
                        List<MatrixDTO> matrices = CurrentUser.Groups.SelectMany(s => s.Matrices).ToList();
                        User.UserData = CurrentUser;
                        List<string> Screens = new List<string>();
                        if (CurrentUser.Groups.Select(s => s.Title.ToLower()).Any(a => a == AdminGroup.ToLower()))
                        {
                            Screens = resources.Where(w => w.IsScreen).Select(s => s.Code).ToList();
                            User.UserData.isAdmin = true;

                        }


                        foreach (var matrix in matrices)
                        {
                            if (matrix.Resource.IsScreen)
                            {
                                if (!Screens.Contains(matrix.Resource.Code))
                                {
                                    Screens.Add(matrix.Resource.Code);
                                }
                            }
                        }

                        User.Screens = Screens;
                     

                        return User;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
        }
        #endregion

        #region Delete

        public bool Delete(int id)
        {
            NotificationConfigurationBLL.SendNotificationForDelegation(id, enumNotificationEventType.DeleteDelegation);
            return DelegationBLL.Delete(id);
        }

        #endregion

        #region Hangfire
        public void ExpiredDelegation()
        {
            List<Delegation> delegations = DelegationBLL.GetAllDelegations();
            List<Delegation> expiredDelegations = delegations.Where(w => w.ToDate.HasValue && w.ToDate.Value.Date < DateTime.Now.Date).ToList();
            List<WFReminderRegistry> reminderRegistries = ReminderRegistryBLL.Read();
            foreach (var delegation in expiredDelegations)
            {
                WFReminderRegistry reminderRegistry = reminderRegistries.Where(w => w.RelatedItemID == delegation.ID && w.Type == typeof(Delegation).Name).FirstOrDefault();
                if (reminderRegistry == null)
                {
                    Task.Run(() => NotificationConfigurationBLL.SendNotificationForDelegation(delegation.ID, enumNotificationEventType.ExpiredDelegation));
                    reminderRegistry = new WFReminderRegistry()
                    {
                        RelatedItemID = delegation.ID,
                        Type = typeof(Delegation).Name,
                        ReminderDate = DateTime.Now
                    };
                    ReminderRegistryBLL.Create(reminderRegistry);
                }
            }
        }
        #endregion


        #region Private

        public bool ValidateDelegationPeriod(Delegation delegation)
        {
            List<Delegation> UserDelegations = DelegationBLL.Read(delegation.FromUser);
            //UserActiveDelegations = UserActiveDelegations.Where(d => DateTime.Now.Date <= d.ToDate).ToList();

            List<Delegation> ActiveDelegations = UserDelegations;
            bool IsValid = false;

            Delegation currentDelegation;

            if (delegation.ID < 1)
            {
                currentDelegation = ActiveDelegations.Where(w => w.ToUser.ToLower() == delegation.ToUser.ToLower())
               .Where(d =>
                    (delegation.FromDate >= d.FromDate && delegation.FromDate <= d.ToDate) ||
                    (delegation.ToDate >= d.FromDate && delegation.ToDate <= d.ToDate)).FirstOrDefault();
            }
            else
            {
                currentDelegation = ActiveDelegations.Where(w => w.ID != delegation.ID).Where(w => w.ToUser.ToLower() == delegation.ToUser.ToLower())
               .Where(d =>
                   (delegation.FromDate >= d.FromDate && delegation.FromDate <= d.ToDate) ||
                   (delegation.ToDate >= d.FromDate && delegation.ToDate <= d.ToDate)).FirstOrDefault();
            }

            if (delegation.ToUser.ToLower() == delegation.FromUser.ToLower())
                IsValid = false;
            else if (currentDelegation != null)
                IsValid = false;
            else
            {
                IsValid = true;
            }

            return IsValid;
        }

        #endregion
    }
}

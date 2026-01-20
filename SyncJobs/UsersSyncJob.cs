using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using SyncJobs.Models;
using System.Data;
using System.Data.SqlClient;

namespace SyncJobs
{
    class UsersSyncJob : SPJobDefinition
    {
        string SiteURL = null;
        string Connection = null;
        public UsersSyncJob() : base() { }

        public UsersSyncJob(string jobName, SPService service) : base(jobName, service, null, SPJobLockType.None)
        {
            this.Title = "UsersSyncTimerJob";
        }

        public UsersSyncJob(string jobName, SPWebApplication webapp) : base(jobName, webapp, null, SPJobLockType.ContentDatabase)
        {
            this.Title = "UsersSyncTimerJob";
        }
        public override void Execute(Guid targetInstanceId)
        {
            try
            {
                SiteURL = this.Properties["SiteUrl"].ToString();

                Connection = GetConnectionStringFromSP(SiteURL);
                if (!string.IsNullOrEmpty(Connection))
                {
                    string DomainName = GetConfigurations(Connection, "DomainName");
                    List<UserInfo> _Users = GetUsers();
                    if (_Users != null && _Users.Count > 0)
                    {
                        List<UserInfo> _UserInfos = ReadUsersInfoActiveDirectory(_Users, DomainName);
                        if (_UserInfos != null && _UserInfos.Count > 0)
                        {
                            UpdateUsersInfo(_UserInfos);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddException("Execute", ex.Message);
            }
        }
        public void UpdateUsersInfo(List<UserInfo> _UserInfo)
        {
            try
            {
                List<UserInfo> _CurrentUserInfos = new List<UserInfo>();
                UserInfo _CurrentUserInfo = new UserInfo();
                _CurrentUserInfos = _UserInfo;
                using (SqlConnection con = new SqlConnection(Connection))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Sync_Users_Add_and_Update", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Title", SqlDbType.NVarChar);
                        cmd.Parameters.Add("@DisplayName", SqlDbType.NVarChar);
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar);
                        cmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar);
                        cmd.Parameters.Add("@UserProfilePicture", SqlDbType.NVarChar);

                        foreach (var adUsers in _CurrentUserInfos)
                        {
                            cmd.Parameters["@Title"].Value = adUsers.UserName;
                            if (adUsers.DisplayName != null)
                            {
                                cmd.Parameters["@DisplayName"].Value = adUsers.DisplayName;
                            }
                            else
                            {
                                cmd.Parameters["@DisplayName"].Value = string.Empty;
                            }
                            if (adUsers.Email != null)
                                cmd.Parameters["@Email"].Value = adUsers.Email;
                            else
                                cmd.Parameters["@Email"].Value = string.Empty;

                            if (adUsers.PhoneNumber != null)
                                cmd.Parameters["@PhoneNumber"].Value = adUsers.PhoneNumber;
                            else
                                cmd.Parameters["@PhoneNumber"].Value = string.Empty;

                            if (adUsers.UserProfilePicture != null)
                                cmd.Parameters["@UserProfilePicture"].Value = adUsers.UserProfilePicture;
                            else
                                cmd.Parameters["@UserProfilePicture"].Value = string.Empty;

                            cmd.ExecuteNonQuery();
                        }
                    }
                    con.Close();
                }
            }
            catch (System.Exception ex)
            {
                AddException("AddUpdateToSQL", ex.Message);
            }
        }
        public void AddException(string FunctionName, string Message)
        {
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("Exceptions_Create", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = "Exception";
                    cmd.Parameters.Add("@Message", SqlDbType.NVarChar).Value = Message;
                    cmd.Parameters.Add("@Controller", SqlDbType.NVarChar).Value = "Users Sync Job";
                    cmd.Parameters.Add("@FunctionName", SqlDbType.NVarChar).Value = FunctionName;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        public List<UserInfo> GetUsers()
        {
            try
            {
                List<UserInfo> _UserInfos = new List<UserInfo>();
                UserInfo _UserInfo;
                using (SqlConnection conn = new SqlConnection(Connection))
                {
                    using (SqlCommand cmd = new SqlCommand("Users_Read", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                _UserInfo = new UserInfo();
                                _UserInfo.UserID = Convert.ToInt32(dr["ID"]);

                                if (dr["Title"] != DBNull.Value)
                                {
                                    _UserInfo.UserName = dr["Title"].ToString();
                                    _UserInfo.SamAccountName = dr["Title"].ToString();
                                }
                                _UserInfos.Add(_UserInfo);
                            }
                        }
                        conn.Close();
                    }
                }
                return _UserInfos;
            }
            catch (Exception ex)
            {
                AddException("ReadSharePoint", ex.Message);
                return null;
            }
        }
        public List<UserInfo> ReadUsersInfoActiveDirectory(List<UserInfo> _UserInfos, string DomainName)
        {
            try
            {

                List<UserInfo> _FinalUserInfos = new List<UserInfo>();
                PrincipalContext domainContext;
                //PrincipalContext domainContext1 = new PrincipalContext(ContextType.Domain);
                if (string.IsNullOrEmpty(DomainName))
                    domainContext = new PrincipalContext(ContextType.Domain);
                else
                    domainContext = new PrincipalContext(ContextType.Domain, DomainName);
                using (domainContext)
                {
                    foreach (UserInfo _UserInfo in _UserInfos)
                    {
                        UserPrincipal user = UserPrincipal.FindByIdentity(domainContext, _UserInfo.UserName);
                        if (user != null)
                        {
                            using (var foundUser = UserPrincipal.FindByIdentity(domainContext, System.DirectoryServices.AccountManagement.IdentityType.SamAccountName, user.SamAccountName))
                            {
                                if (foundUser != null)
                                {
                                    DirectoryEntry directoryEntry = foundUser.GetUnderlyingObject() as DirectoryEntry;
                                    if (directoryEntry.Properties["mobile"].Value != null)
                                    {
                                        _UserInfo.PhoneNumber = directoryEntry.Properties["mobile"].Value.ToString();
                                    }
                                    else
                                        _UserInfo.PhoneNumber = "";
                                    if (directoryEntry.Properties["mail"].Value != null)
                                    {
                                        _UserInfo.Email = directoryEntry.Properties["mail"].Value.ToString() != null ? directoryEntry.Properties["mail"].Value.ToString() : string.Empty;
                                    }
                                    else
                                    {
                                        _UserInfo.Email = "";
                                    }
                                    if (directoryEntry.Properties["displayName"].Value != null)
                                    {
                                        _UserInfo.DisplayName = directoryEntry.Properties["displayName"].Value.ToString() != null ? directoryEntry.Properties["displayName"].Value.ToString() : string.Empty;
                                    }
                                    else
                                    {
                                        _UserInfo.DisplayName = "";
                                    }
                                    if (directoryEntry.Properties["thumbnailPhoto"].Value != null)
                                    {
                                        _UserInfo.UserProfilePicture = "UserPhoto/" + user.SamAccountName;
                                    }

                                    else
                                    {
                                        _UserInfo.UserProfilePicture = "UserPhoto/" + user.SamAccountName;
                                    }

                                    _FinalUserInfos.Add(_UserInfo);
                                }
                            }
                        }
                    }
                }

                return _FinalUserInfos;
            }
            catch (System.Exception ex)
            {
                AddException("ReadActiveDirectory", ex.Message);
                return null;
            }
        }
        public string GetConfigurations(string Connection, string Key)
        {
            string Value = null;
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("Sync_Configurations_ByType_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = Key;
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            if (dr["Value"] != DBNull.Value)
                            {
                                Value = dr["Value"].ToString();
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return Value;
        }
        public string GetConnectionStringFromSP(string SiteURL)
        {
            string connectionString = string.Empty;
            try
            {

                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite sps = new SPSite(SiteURL))
                    {
                        using (SPWeb spw = sps.OpenWeb())
                        {
                            spw.AllowUnsafeUpdates = true;
                            SPList spl = spw.Lists["DBConfiguration"];
                            SPListItemCollection splic = spl.GetItems();
                            if (splic.Count > 0)
                            {
                                SPListItem spli = splic[0];

                                if (spli["Title"] != null)
                                {
                                    connectionString = spli["Title"].ToString();
                                }


                            }
                            spw.AllowUnsafeUpdates = false;
                        }
                    }
                });
                return connectionString;
            }
            catch (Exception ex)
            {
                AddException("GetConnectionStringFromSP", ex.Message);
                return string.Empty;
            }
        }
    }
}

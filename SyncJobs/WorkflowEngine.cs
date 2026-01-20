using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using SyncJobs.Models;
using System.Security.Cryptography;
using Microsoft.SharePoint;

namespace SyncJobs
{
    class WorkflowEngine : SPJobDefinition
    {
        public static string Connection = null;

        string SiteURL = null;

        public static string key = "secret";
        public WorkflowEngine() : base() { }
        public WorkflowEngine(string jobName, SPService service) : base(jobName, service, null, SPJobLockType.None)
        {
            this.Title = "WorkflowEngineTimerJob";
        }
        public WorkflowEngine(string jobName, SPWebApplication webapp) : base(jobName, webapp, null, SPJobLockType.ContentDatabase)
        {
            this.Title = "WorkflowEngineTimerJob";
        }
        public override void Execute(Guid targetInstanceId)
        {
            try
            {
                SiteURL = this.Properties["SiteUrl"].ToString();
                Connection = GetConnectionStringFromSP(SiteURL);
                List<KPI> _KPIs = GetKPIs();
                List<ReminderConfiguration> _reminders = GetReminderConfigration();
                List<ReminderRegistry> reminderRegistries = GetReminderRegistry();
                List<KPIType> _Types = GetKPITypes();
                List<WFRequest> Requests = GetRequests(_Types);
                List<Delegation> _Delegations = GetDelegations();
                List<WFRequestStep> RequestSteps = GetRequestSteps();
                OpenKPIForUpdate(_KPIs, _reminders, _Types);
                List<KPIMeasure> kPIMeasures = GetKPIMeasures();
                LockKPI(_KPIs, _Types);
                KPIUpdateReminder(_KPIs, _reminders, reminderRegistries, _Types, kPIMeasures);
                KPIApprovalReminder(_KPIs, kPIMeasures, _Types, _reminders, reminderRegistries, Requests, RequestSteps);
                ExpiredDelegations(_Delegations, reminderRegistries);
            }
            catch (Exception ex)
            {
                AddException("Execute", ex.Message);
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
                    cmd.Parameters.Add("@Controller", SqlDbType.NVarChar).Value = "Workflow Sync Job";
                    cmd.Parameters.Add("@FunctionName", SqlDbType.NVarChar).Value = FunctionName;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        #region KPI
        public List<KPI> GetKPIs()
        {
            List<KPI> _KPIs = new List<KPI>();
            List<KPIMeasure> _Measures = GetKPIMeasures();
            KPI _KPI = null;
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("KPI_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            _KPI = new KPI();
                            _KPI.GeneralData.ID = Convert.ToInt32(dr["Id"]);
                            if (dr["Title"] != DBNull.Value && !string.IsNullOrEmpty(dr["Title"].ToString()))
                            {
                                _KPI.GeneralData.EnglishName = dr["Title"].ToString();
                            }
                            if (dr["ArabicName"] != DBNull.Value && !string.IsNullOrEmpty(dr["ArabicName"].ToString()))
                            {
                                _KPI.GeneralData.ArabicName = dr["ArabicName"].ToString();
                            }
                            if (dr["RequireUpdate"] != DBNull.Value && !string.IsNullOrEmpty(dr["RequireUpdate"].ToString()))
                            {
                                _KPI.RequireUpdate = dr["RequireUpdate"].ToString();
                            }
                            if (dr["KPIType"] != DBNull.Value && !string.IsNullOrEmpty(dr["KPIType"].ToString()))
                            {
                                _KPI.KPIType = dr["KPIType"].ToString();
                            }
                            if (dr["KPIOwner1"] != DBNull.Value && !string.IsNullOrEmpty(dr["KPIOwner1"].ToString()))
                            {
                                _KPI.Champion = GetUserByUserName(dr["KPIOwner1"].ToString());
                            }
                            if (dr["KPIOwner2"] != DBNull.Value && !string.IsNullOrEmpty(dr["KPIOwner2"].ToString()))
                            {
                                _KPI.Owner = GetUserByUserName(dr["KPIOwner2"].ToString());
                            }
                            if (dr["Sponsor"] != DBNull.Value && !string.IsNullOrEmpty(dr["Sponsor"].ToString()))
                            {
                                _KPI.Sponser = GetUserByUserName(dr["Sponsor"].ToString());
                            }
                            if (dr["IsLocked"] != DBNull.Value && !string.IsNullOrEmpty(dr["IsLocked"].ToString()))
                            {
                                _KPI.IsLocked = bool.Parse(dr["IsLocked"].ToString());
                            }
                            if (dr["UnlockDate"] != DBNull.Value && !string.IsNullOrEmpty(dr["UnlockDate"].ToString()))
                            {
                                _KPI.UnlockDate = DateTime.Parse(dr["UnlockDate"].ToString());
                            }
                            _KPIs.Add(_KPI);
                        }
                    }
                    conn.Close();
                }
            }

            foreach (KPI KPI in _KPIs)
            {
                KPI.Measures = _Measures.Where(a => a.KPIID == KPI.GeneralData.ID).ToList();
            }
            return _KPIs;
        }
        public List<KPIMeasure> GetKPIMeasures()
        {
            KPIMeasure _KPIMeasure;
            List<KPIMeasure> _KPIMeasures = new List<KPIMeasure>();

            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("KPIMeasures_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            _KPIMeasure = new KPIMeasure();
                            _KPIMeasure.ID = Convert.ToInt32(dr["Id"].ToString());
                            if (dr["KPI"] != DBNull.Value && !string.IsNullOrEmpty(dr["KPI"].ToString()))
                            {
                                _KPIMeasure.KPIID = Convert.ToInt32(dr["KPI"]);
                            }
                            if (dr["DueDate"] != DBNull.Value && !string.IsNullOrEmpty(dr["DueDate"].ToString()))
                            {
                                _KPIMeasure.DueDate = dr["DueDate"].ToString();
                            }
                            if (dr["TempValue"] != DBNull.Value && !string.IsNullOrEmpty(dr["TempValue"].ToString()))
                            {
                                _KPIMeasure.TempValue = decimal.Parse(dr["TempValue"].ToString());
                            }
                            if (dr["Status"] != DBNull.Value)
                            {
                                _KPIMeasure.Status = dr["Status"].ToString();
                            }

                            _KPIMeasures.Add(_KPIMeasure);
                        }
                    }
                    conn.Close();
                }
            }
            return _KPIMeasures.OrderBy(a => a.ID).ToList();
        }
        public List<KPIType> GetKPITypes()
        {
            KPIType _KPIType;
            List<KPIType> _KPITypes = new List<KPIType>();
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("KPIType_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            _KPIType = new KPIType();
                            if (dr["ID"] != DBNull.Value && !string.IsNullOrEmpty(dr["ID"].ToString()))
                            {
                                _KPIType.ID = Convert.ToInt32(dr["ID"]);
                            }
                            if (dr["Name"] != DBNull.Value && !string.IsNullOrEmpty(dr["Name"].ToString()))
                            {
                                _KPIType.Name = dr["Name"].ToString();
                            }
                            if (dr["GracePeriod"] != DBNull.Value && !string.IsNullOrEmpty(dr["GracePeriod"].ToString()))
                            {
                                _KPIType.GracePeriod = int.Parse(dr["GracePeriod"].ToString());
                            }
                            if (_KPIType != null)
                                _KPITypes.Add(_KPIType);
                        }
                    }
                    conn.Close();
                }
            }
            return _KPITypes;
        }


        #endregion

        #region Request
        public List<ReminderConfiguration> GetReminderConfigration()
        {
            ReminderConfiguration _ReminderConfiguration;
            List<ReminderConfiguration> _ReminderConfigurations = new List<ReminderConfiguration>();
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("Reminder_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            _ReminderConfiguration = new ReminderConfiguration();
                            if (dr["ID"] != DBNull.Value && !string.IsNullOrEmpty(dr["ID"].ToString()))
                            {
                                _ReminderConfiguration.ID = Convert.ToInt32(dr["ID"]);
                            }
                            if (dr["TypeID"] != DBNull.Value && !string.IsNullOrEmpty(dr["TypeID"].ToString()))
                            {
                                _ReminderConfiguration.TypeID = int.Parse(dr["TypeID"].ToString());
                            }
                            if (dr["BeforeReminder"] != DBNull.Value && !string.IsNullOrEmpty(dr["BeforeReminder"].ToString()))
                            {
                                _ReminderConfiguration.BeforeReminder = int.Parse(dr["BeforeReminder"].ToString());
                            }
                            if (dr["FirstReminder"] != DBNull.Value && !string.IsNullOrEmpty(dr["FirstReminder"].ToString()))
                            {
                                _ReminderConfiguration.FirstReminder = int.Parse(dr["FirstReminder"].ToString());
                            }
                            if (dr["SecondReminder"] != DBNull.Value && !string.IsNullOrEmpty(dr["SecondReminder"].ToString()))
                            {
                                _ReminderConfiguration.SecondReminder = int.Parse(dr["SecondReminder"].ToString());
                            }
                            if (_ReminderConfiguration != null)
                                _ReminderConfigurations.Add(_ReminderConfiguration);
                        }
                    }
                    conn.Close();
                }
            }
            return _ReminderConfigurations;
        }
        public List<ReminderRegistry> GetReminderRegistry()
        {
            ReminderRegistry _ReminderRegistry;
            List<ReminderRegistry> _ReminderRegistries = new List<ReminderRegistry>();
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("WFReminderRegistry_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            _ReminderRegistry = new ReminderRegistry();
                            if (dr["ID"] != DBNull.Value && !string.IsNullOrEmpty(dr["ID"].ToString()))
                            {
                                _ReminderRegistry.ID = Convert.ToInt32(dr["ID"]);
                            }
                            if (dr["RelatedItemID"] != DBNull.Value && !string.IsNullOrEmpty(dr["RelatedItemID"].ToString()))
                            {
                                _ReminderRegistry.RelatedItemID = int.Parse(dr["RelatedItemID"].ToString());
                            }
                            if (dr["Type"] != DBNull.Value && !string.IsNullOrEmpty(dr["Type"].ToString()))
                            {
                                _ReminderRegistry.Type = dr["Type"].ToString();
                            }
                            if (dr["ReminderDate"] != DBNull.Value && !string.IsNullOrEmpty(dr["ReminderDate"].ToString()))
                            {
                                _ReminderRegistry.ReminderDate = DateTime.Parse(dr["ReminderDate"].ToString()).Date;
                            }
                            if (_ReminderRegistry != null)
                                _ReminderRegistries.Add(_ReminderRegistry);
                        }
                    }
                    conn.Close();
                }
            }
            return _ReminderRegistries;
        }
        public List<WFRequest> GetRequests(List<KPIType> _Types)
        {
            List<WFRequest> Requests = new List<WFRequest>();
            WFRequest Request = null;
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("WFRequests_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Request = new WFRequest();
                            if (dr["WorkflowID"] != DBNull.Value && !string.IsNullOrEmpty(dr["WorkflowID"].ToString()) && _Types.Where(a => a.ID == int.Parse(dr["WorkflowID"].ToString())).Count() > 0)
                            {
                                if (dr["ID"] != DBNull.Value && !string.IsNullOrEmpty(dr["ID"].ToString()))
                                {
                                    Request.ID = Convert.ToInt32(dr["ID"]);
                                }
                                if (dr["RelatedItemID"] != DBNull.Value && !string.IsNullOrEmpty(dr["RelatedItemID"].ToString()))
                                {
                                    Request.RelatedItemID = Convert.ToInt32(dr["RelatedItemID"]);
                                }
                                if (dr["Status"] != DBNull.Value && !string.IsNullOrEmpty(dr["Status"].ToString()))
                                {
                                    Request.Status = dr["Status"].ToString();
                                }
                                Requests.Add(Request);
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return Requests;
        }
        public List<WFRequestStep> GetRequestSteps()
        {
            List<WFRequestStep> requestSteps = new List<WFRequestStep>();
            WFRequestStep requestStep;
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("WFRequestSteps_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            requestStep = new WFRequestStep();
                            if (dr["ID"] != DBNull.Value && !string.IsNullOrEmpty(dr["ID"].ToString()))
                            {
                                requestStep.ID = Convert.ToInt32(dr["ID"]);
                            }
                            if (dr["RequestID"] != DBNull.Value && !string.IsNullOrEmpty(dr["RequestID"].ToString()))
                            {
                                requestStep.RequestID = Convert.ToInt32(dr["RequestID"]);
                            }
                            if (dr["WorkflowStepID"] != DBNull.Value && !string.IsNullOrEmpty(dr["WorkflowStepID"].ToString()))
                            {
                                requestStep.WorkflowStepID = Convert.ToInt32(dr["WorkflowStepID"]);
                            }
                            if (dr["ActionBy"] != DBNull.Value && !string.IsNullOrEmpty(dr["ActionBy"].ToString()))
                            {
                                requestStep.ActionBy = dr["ActionBy"].ToString();
                            }
                            if (dr["Approver"] != DBNull.Value && !string.IsNullOrEmpty(dr["Approver"].ToString()))
                            {
                                requestStep.Approver = dr["Approver"].ToString();
                            }
                            if (dr["IsGroup"] != DBNull.Value && !string.IsNullOrEmpty(dr["IsGroup"].ToString()))
                            {
                                requestStep.IsGroup = bool.Parse(dr["IsGroup"].ToString());
                            }
                            if (dr["Status"] != DBNull.Value && !string.IsNullOrEmpty(dr["Status"].ToString()))
                            {
                                requestStep.Status = dr["Status"].ToString();
                            }
                            if (requestStep.Status != "Not Started")
                            {
                                if (dr["Modified"] != DBNull.Value && !string.IsNullOrEmpty(dr["Modified"].ToString()))
                                {
                                    requestStep.Modified = DateTime.Parse(dr["Modified"].ToString());
                                }
                            }
                            requestSteps.Add(requestStep);
                        }
                    }
                    conn.Close();
                }
            }
            return requestSteps;
        }
        public void CreateWFReminderRegistry(int RelatedItemID, string Type)
        {
            using (SqlConnection con = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("WFReminderRegistry_Create", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    var returnParameter = cmd.Parameters.Add("@ID", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("@RelatedItemID", SqlDbType.Int).Value = RelatedItemID;
                    cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Type;
                    cmd.Parameters.Add("@ReminderDate", SqlDbType.Date).Value = DateTime.Now;

                    cmd.ExecuteNonQuery();
                    if (returnParameter.Value != null)
                    {
                        int ID = Convert.ToInt32(returnParameter.Value);
                    }
                    con.Close();
                }
            }
        }
        #endregion

        #region Users
        public UserInfo GetUserByUserName(string userName)
        {
            List<Group> Groups = GetGroups();
            List<UserRelation> _Relations = GetUserRelations();
            UserInfo _UserInfo = null;
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("Users_ByUserName_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = userName;
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            _UserInfo = new UserInfo();

                            if (dr["Title"] != DBNull.Value)
                            {
                                _UserInfo.UserName = dr["Title"].ToString();
                            }
                            if (dr["Email"] != DBNull.Value)
                            {
                                _UserInfo.Email = dr["Email"].ToString();
                            }
                            if (dr["DisplayName"] != DBNull.Value)
                            {
                                _UserInfo.DisplayName = dr["DisplayName"].ToString();
                            }
                            _UserInfo.UserID = Convert.ToInt32(dr["ID"]);
                        }
                    }
                    conn.Close();
                }
            }

            if (_UserInfo != null)
            {
                List<UserRelation> _userGroups = _Relations.Where(a => a.UserID == _UserInfo.UserID).ToList();
                foreach (UserRelation group in _userGroups)
                {
                    _UserInfo.Groups = Groups.Where(a => a.ID == group.ID).ToList();
                }
            }

            return _UserInfo;
        }
        public List<Group> GetGroups()
        {
            Group _Group;
            List<Group> _Groups = new List<Group>();
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("Groups_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            _Group = new Group();
                            if (dr["ID"] != DBNull.Value && !string.IsNullOrEmpty(dr["ID"].ToString()))
                            {
                                _Group.ID = Convert.ToInt32(dr["ID"]);
                            }
                            if (dr["Title"] != DBNull.Value && !string.IsNullOrEmpty(dr["Title"].ToString()))
                            {
                                _Group.Title = dr["Title"].ToString();
                            }
                            _Groups.Add(_Group);
                        }
                    }
                    conn.Close();
                }
            }
            return _Groups;
        }
        public List<UserRelation> GetUserRelations()
        {
            List<UserRelation> _Relations = new List<UserRelation>();
            UserRelation _Relation = null;
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("UsersRelation_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            _Relation = new UserRelation();
                            _Relation.UserID = Convert.ToInt32(dr["ID"]);
                            if (dr["ID"] != DBNull.Value && !string.IsNullOrEmpty(dr["ID"].ToString()))
                            {
                                _Relation.ID = Convert.ToInt32(dr["ID"]);
                            }
                            if (dr["UserID"] != DBNull.Value && !string.IsNullOrEmpty(dr["UserID"].ToString()))
                            {
                                _Relation.UserID = Convert.ToInt32(dr["UserID"].ToString());
                            }
                            if (dr["GroupID"] != DBNull.Value && !string.IsNullOrEmpty(dr["GroupID"].ToString()))
                            {
                                _Relation.GroupID = Convert.ToInt32(dr["GroupID"].ToString());
                            }
                            _Relations.Add(_Relation);
                        }
                    }
                    conn.Close();
                }
            }
            return _Relations;
        }
        public List<UserInfo> GetUsersByGroupName(string GroupName)
        {
            UserInfo _UserInfo = null;
            List<UserInfo> _UserInfolst = new List<UserInfo>();

            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("Users_ByGroupName_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    cmd.Parameters.Add("@GroupName", SqlDbType.NVarChar).Value = GroupName;
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
                            }
                            if (dr["Email"] != DBNull.Value)
                            {
                                _UserInfo.Email = dr["Email"].ToString();
                            }
                            if (dr["DisplayName"] != DBNull.Value)
                            {
                                _UserInfo.DisplayName = dr["DisplayName"].ToString();
                            }
                            _UserInfolst.Add(_UserInfo);
                        }
                    }
                    conn.Close();
                }
            }
            return _UserInfolst;
        }
        public List<UserInfo> GetAllUsers()
        {
            UserInfo _UserInfo = null;
            List<UserInfo> _UserInfolst = new List<UserInfo>();

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
                            }
                            if (dr["Email"] != DBNull.Value)
                            {
                                _UserInfo.Email = dr["Email"].ToString();
                            }
                            if (dr["DisplayName"] != DBNull.Value)
                            {
                                _UserInfo.DisplayName = dr["DisplayName"].ToString();
                            }
                            _UserInfolst.Add(_UserInfo);
                        }
                    }
                    conn.Close();
                }
            }
            return _UserInfolst;
        }
        #endregion

        #region Process
        public void OpenKPIForUpdate(List<KPI> _KPIs, List<ReminderConfiguration> _reminders, List<KPIType> _Types)
        {
            foreach (KPI _KPI in _KPIs)
            {
                if (_KPI.GeneralData.ID == 1097)
                {

                }
                string Type = _KPI.KPIType;
                int TypeID = _Types.Where(a => a.Name == Type).Select(s => s.ID).FirstOrDefault();
                int Before = _reminders.Where(a => a.TypeID == TypeID).Select(s => s.BeforeReminder).FirstOrDefault();
                foreach (KPIMeasure _Measure in _KPI.Measures)
                {
                    if (DateTime.Today.Date >= DateTime.Parse(_Measure.DueDate).Date && _Measure.Status == "Not Started")
                    {
                        //Set KPI to Yes "KPI_Status_Update" @Status @ID
                        using (SqlConnection conn = new SqlConnection(Connection))
                        {
                            using (SqlCommand cmd = new SqlCommand("Sync_OpenKPIForUpdate", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                conn.Open();
                                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = _KPI.GeneralData.ID;
                                cmd.Parameters.Add("@MeasureID", SqlDbType.Int).Value = _Measure.ID;
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                        }
                    }
                }
            }
        }

        public void LockKPI(List<KPI> _KPIs, List<KPIType> _Types)
        {
            foreach (KPI _KPI in _KPIs)
            {
                string Type = _KPI.KPIType;
                int TypeID = _Types.Where(a => a.Name == Type).Select(s => s.ID).FirstOrDefault();
                int GracePeriod = _Types.Where(a => a.ID == TypeID).Select(s => s.GracePeriod).FirstOrDefault();
                foreach (KPIMeasure _Measure in _KPI.Measures.Where(a => a.Status == "New" || a.Status == "Rejected"))
                {
                    if (DateTime.Today.Date == GetEndDateWorkingDays(DateTime.Parse(_Measure.DueDate), GracePeriod + 1) && (!_KPI.UnlockDate.HasValue || _KPI.UnlockDate.Value.Date != DateTime.Now.Date))
                    {
                        using (SqlConnection conn = new SqlConnection(Connection))
                        {
                            using (SqlCommand cmd = new SqlCommand("KPI_Lock", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                conn.Open();
                                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = _KPI.GeneralData.ID;
                                cmd.ExecuteNonQuery();
                                conn.Close();
                                break;
                            }
                        }
                    }
                }
            }
        }
        public void KPIUpdateReminder(List<KPI> _KPIs, List<ReminderConfiguration> _reminders, List<ReminderRegistry> reminderRegistries, List<KPIType> _Types, List<KPIMeasure> kPIMeasures)
        {
            List<KPIMeasure> newMeasures = kPIMeasures.Where(a => a.Status == "Not Started" || a.Status == "New" || a.Status == "Rejected").ToList();
            foreach (KPIMeasure newMeasure in newMeasures)
            {

                KPI KPI = _KPIs.Where(a => a.GeneralData.ID == newMeasure.KPIID).FirstOrDefault();
                if (KPI.GeneralData.ID == 1097)
                {

                }
                string Type = _KPIs.Where(a => a.GeneralData.ID == newMeasure.KPIID).Select(s => s.KPIType).FirstOrDefault();
                int TypeID = _Types.Where(a => a.Name == Type).Select(s => s.ID).FirstOrDefault();
                int GracePeriod = _Types.Where(a => a.Name == Type).Select(s => s.GracePeriod).FirstOrDefault();
                int Before = _reminders.Where(a => a.TypeID == TypeID).Select(s => s.BeforeReminder).FirstOrDefault();
                int First = _reminders.Where(a => a.TypeID == TypeID).Select(s => s.FirstReminder).FirstOrDefault();
                int Second = _reminders.Where(a => a.TypeID == TypeID).Select(s => s.SecondReminder).FirstOrDefault();

                if (GetEndDateWorkingDays(DateTime.Today.Date, Before) == DateTime.Parse(newMeasure.DueDate).Date)
                {
                    ReminderRegistry reminderRegistry = reminderRegistries.Where(a => a.RelatedItemID == newMeasure.KPIID && a.Type == "KPI").LastOrDefault();
                    if ((reminderRegistry == null && reminderRegistry?.ReminderDate == null) || reminderRegistry?.ReminderDate.Date != DateTime.Today.Date)
                    {
                        //Send Before Reminder
                        SendNotificationWorkflow(KPI.GeneralData.ID, 0, enumNotificationEventType.BeforeUpdateReminder, _Types, _reminders, KPI, newMeasure);
                        //CreateWFReminderRegistry
                        CreateWFReminderRegistry(KPI.GeneralData.ID, "KPI");
                        //Type is KPI
                        //KPI.ID

                    }
                }
                else if (DateTime.Parse(newMeasure.DueDate).Date == DateTime.Today.Date)
                {
                    ReminderRegistry reminderRegistry = reminderRegistries.Where(a => a.RelatedItemID == newMeasure.KPIID && a.Type == "KPI").LastOrDefault();
                    if ((reminderRegistry == null && reminderRegistry?.ReminderDate == null) || reminderRegistry.ReminderDate.Date != DateTime.Today.Date)
                    {
                        //Send Same Day Reminder
                        SendNotificationWorkflow(KPI.GeneralData.ID, 0, enumNotificationEventType.UpdateSameDayReminder, _Types, _reminders, KPI, newMeasure);
                        //CreateWFReminderRegistry
                        CreateWFReminderRegistry(KPI.GeneralData.ID, "KPI");
                        //Type is KPI
                        //KPI.ID
                    }
                }
                else if (GetEndDateWorkingDays(DateTime.Parse(newMeasure.DueDate).Date, First) == DateTime.Today.Date)
                {
                    ReminderRegistry reminderRegistry = reminderRegistries.Where(a => a.RelatedItemID == newMeasure.KPIID && a.Type == "KPI").LastOrDefault();
                    if ((reminderRegistry == null && reminderRegistry?.ReminderDate == null) || reminderRegistry.ReminderDate.Date != DateTime.Today.Date)
                    {
                        //Send First Reminder
                        SendNotificationWorkflow(KPI.GeneralData.ID, 0, enumNotificationEventType.UpdateFirstReminder, _Types, _reminders, KPI, newMeasure);
                        //CreateWFReminderRegistry
                        CreateWFReminderRegistry(KPI.GeneralData.ID, "KPI");
                        //Type is KPI
                        //KPI.ID
                    }
                }
                else if (GetEndDateWorkingDays(DateTime.Parse(newMeasure.DueDate).Date, Second) == DateTime.Today.Date)
                {
                    ReminderRegistry reminderRegistry = reminderRegistries.Where(a => a.RelatedItemID == newMeasure.KPIID && a.Type == "KPI").LastOrDefault();
                    if ((reminderRegistry == null && reminderRegistry?.ReminderDate == null) || reminderRegistry.ReminderDate.Date != DateTime.Today.Date)
                    {
                        //Send Second Reminder
                        SendNotificationWorkflow(KPI.GeneralData.ID, 0, enumNotificationEventType.UpdateSecondReminder, _Types, _reminders, KPI, newMeasure);
                        //CreateWFReminderRegistry
                        CreateWFReminderRegistry(KPI.GeneralData.ID, "KPI");
                        //Type is KPI
                        //KPI.ID
                    }
                }
                else if (GetEndDateWorkingDays(DateTime.Parse(newMeasure.DueDate).Date, GracePeriod) == DateTime.Today.Date)
                {
                    ReminderRegistry reminderRegistry = reminderRegistries.Where(a => a.RelatedItemID == newMeasure.KPIID && a.Type == "KPI").LastOrDefault();
                    if ((reminderRegistry == null && reminderRegistry?.ReminderDate == null) || reminderRegistry.ReminderDate.Date != DateTime.Today.Date)
                    {
                        //Send Second Reminder
                        SendNotificationWorkflow(KPI.GeneralData.ID, 0, enumNotificationEventType.UpdateLastReminder, _Types, _reminders, KPI, newMeasure);
                        //CreateWFReminderRegistry
                        CreateWFReminderRegistry(KPI.GeneralData.ID, "KPI");
                        //Type is KPI
                        //KPI.ID
                    }
                }
                //End of Grace Period 
            }
        }

        public void KPIApprovalReminder(List<KPI> _KPIs, List<KPIMeasure> KPIMeasures, List<KPIType> _Types, List<ReminderConfiguration> _reminders, List<ReminderRegistry> reminderRegistries, List<WFRequest> Requests, List<WFRequestStep> RequestSteps)
        {
            List<KPIMeasure> newMeasures = KPIMeasures.Where(a => a.Status == "Pending").ToList();
            foreach (KPIMeasure newMeasure in newMeasures)
            {
                if (newMeasure.ID == 5730)
                {

                }
                KPI KPI = _KPIs.Where(a => a.GeneralData.ID == newMeasure.KPIID).FirstOrDefault();
                int GracePeriod = _Types.Where(a => a.Name.ToLower() == KPI.KPIType.ToLower()).FirstOrDefault().GracePeriod;
                WFRequest measureRequest = Requests.Where(a => a.RelatedItemID == newMeasure.ID && a.Status == "Pending").LastOrDefault();
                if (measureRequest != null)
                {
                    WFRequestStep measureRequestStep = RequestSteps.Where(a => a.RequestID == measureRequest.ID && a.Status == "Pending").FirstOrDefault();
                    ReminderRegistry reminderRegistry = reminderRegistries.Where(a => a.RelatedItemID == measureRequestStep.ID && a.Type == "RequestStep").LastOrDefault();
                    if ((reminderRegistry == null && reminderRegistry?.ReminderDate == null) || reminderRegistry?.ReminderDate.Date != DateTime.Today.Date)
                    {
                        //Date before grace period in one day
                        if (GetEndDateWorkingDays(DateTime.Today.Date, 1) == GetEndDateWorkingDays(DateTime.Parse(newMeasure.DueDate).Date, GracePeriod))
                        {

                            //Send Last Approval reminder
                            SendNotificationWorkflow(KPI.GeneralData.ID, measureRequest.ID, enumNotificationEventType.LastApprovalReminder, _Types, _reminders, KPI, newMeasure);
                            //CreateWFReminderRegistry
                            CreateWFReminderRegistry(measureRequestStep.ID, "RequestStep");
                            //Type is RequestStep
                            //measureRequestStep.ID

                        }
                        else if (measureRequestStep.Modified.HasValue && measureRequestStep.Modified.Value.Date != DateTime.Now.Date)
                        {
                            //Send Normal Update Reminder
                            SendNotificationWorkflow(KPI.GeneralData.ID, measureRequest.ID, enumNotificationEventType.ApprovalDailyReminder, _Types, _reminders, KPI, newMeasure);
                            //CreateWFReminderRegistry
                            CreateWFReminderRegistry(measureRequestStep.ID, "RequestStep");
                            //Type is RequestStep
                            //measureRequestStep.ID
                        }

                    }
                }
            }
        }

        public void ExpiredDelegations(List<Delegation> _Delegations, List<ReminderRegistry> reminderRegistries)
        {
            foreach (Delegation _Delegation in _Delegations)
            {
                ReminderRegistry reminderRegistry = reminderRegistries.Where(a => a.RelatedItemID == _Delegation.ID && a.Type == "Delegation").LastOrDefault();
                if (_Delegation.ToDate.Date < DateTime.Now.Date)
                {
                    if ((reminderRegistry == null && reminderRegistry?.ReminderDate == null) || reminderRegistry?.ReminderDate.Date != DateTime.Today.Date)
                    {
                        SendNotificationForDelegation(_Delegation, enumNotificationEventType.ExpiredDelegation);
                        CreateWFReminderRegistry(_Delegation.ID, "Delegation");
                    }
                }
            }
        }

        #endregion

        #region Notification
        public List<NotificationConfiguration> GetNotificationConfigurations()
        {
            List<NotificationConfiguration> _NotificationConfigurations = new List<NotificationConfiguration>();
            NotificationConfiguration _NotificationConfiguration;
            List<NotificationConfigurationReciever> _NotificationConfigurationReciever = ReadReceiver();
            List<NotificationConfigurationParameter> _NotificationConfigurationParameter = ReadParameter();

            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("NotificationConfiguration_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            _NotificationConfiguration = new NotificationConfiguration();
                            if (dr["ID"] != DBNull.Value && !string.IsNullOrEmpty(dr["ID"].ToString()))
                                _NotificationConfiguration.ID = Convert.ToInt32(dr["ID"].ToString());

                            if (dr["Title"] != DBNull.Value && !string.IsNullOrEmpty(dr["Title"].ToString()))
                                _NotificationConfiguration.Title = dr["Title"].ToString();

                            if (dr["NotificationEnglish"] != DBNull.Value && !string.IsNullOrEmpty(dr["NotificationEnglish"].ToString()))
                                _NotificationConfiguration.NotificationEnglish = dr["NotificationEnglish"].ToString();

                            if (dr["NotificationArabic"] != DBNull.Value && !string.IsNullOrEmpty(dr["NotificationArabic"].ToString()))
                                _NotificationConfiguration.NotificationArabic = dr["NotificationArabic"].ToString();

                            if (dr["EmailSubject"] != DBNull.Value && !string.IsNullOrEmpty(dr["EmailSubject"].ToString()))
                                _NotificationConfiguration.EmailSubject = dr["EmailSubject"].ToString();

                            if (dr["EmailBody"] != DBNull.Value && !string.IsNullOrEmpty(dr["EmailBody"].ToString()))
                                _NotificationConfiguration.EmailBody = dr["EmailBody"].ToString();

                            if (dr["ActionType"] != DBNull.Value && !string.IsNullOrEmpty(dr["ActionType"].ToString()))
                                _NotificationConfiguration.ActionType = Convert.ToInt32(dr["ActionType"].ToString());

                            _NotificationConfigurations.Add(_NotificationConfiguration);
                        }
                    }
                    conn.Close();
                }
            }
            foreach (NotificationConfiguration NotificationConfiguration in _NotificationConfigurations)
            {
                NotificationConfiguration.Receivers = _NotificationConfigurationReciever.Where(a => a.TemplateID == NotificationConfiguration.ID).ToList();
            }
            foreach (NotificationConfiguration NotificationConfiguration in _NotificationConfigurations)
            {
                NotificationConfiguration.Parameters = _NotificationConfigurationParameter.Where(a => a.TemplateID == NotificationConfiguration.ID).ToList();
            }
            return _NotificationConfigurations;
        }
        public List<NotificationConfigurationReciever> ReadReceiver()
        {
            List<NotificationConfigurationReciever> _Receivers = new List<NotificationConfigurationReciever>();
            NotificationConfigurationReciever _Receiver;
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("Receiver_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            _Receiver = new NotificationConfigurationReciever();
                            if (dr["ID"] != DBNull.Value && !string.IsNullOrEmpty(dr["ID"].ToString()))
                                _Receiver.ID = Convert.ToInt32(dr["ID"].ToString());

                            if (dr["TemplateID"] != DBNull.Value && !string.IsNullOrEmpty(dr["TemplateID"].ToString()))
                                _Receiver.TemplateID = Convert.ToInt32(dr["TemplateID"].ToString());

                            if (dr["Receiver"] != DBNull.Value && !string.IsNullOrEmpty(dr["Receiver"].ToString()))
                                _Receiver.ReceiverName = dr["Receiver"].ToString();

                            if (dr["IsCC"] != DBNull.Value && !string.IsNullOrEmpty(dr["IsCC"].ToString()))
                                _Receiver.IsCC = bool.Parse(dr["IsCC"].ToString());

                            _Receivers.Add(_Receiver);
                        }
                    }
                    conn.Close();
                }
            }
            return _Receivers;
        }
        public List<NotificationConfigurationParameter> ReadParameter()
        {
            List<NotificationConfigurationParameter> _NotificationConfigurationParameters = new List<NotificationConfigurationParameter>();
            NotificationConfigurationParameter _NotificationConfigurationParameter;
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("NotificationParameters_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            _NotificationConfigurationParameter = new NotificationConfigurationParameter();
                            if (dr["ID"] != DBNull.Value && !string.IsNullOrEmpty(dr["ID"].ToString()))
                                _NotificationConfigurationParameter.ID = Convert.ToInt32(dr["ID"].ToString());

                            if (dr["TemplateID"] != DBNull.Value && !string.IsNullOrEmpty(dr["TemplateID"].ToString()))
                                _NotificationConfigurationParameter.TemplateID = Convert.ToInt32(dr["TemplateID"].ToString());

                            if (dr["Value"] != DBNull.Value && !string.IsNullOrEmpty(dr["Value"].ToString()))
                                _NotificationConfigurationParameter.Value = dr["Value"].ToString();

                            if (dr["Title"] != DBNull.Value && !string.IsNullOrEmpty(dr["Title"].ToString()))
                                _NotificationConfigurationParameter.Title = dr["Title"].ToString();

                            if (dr["IsUser"] != DBNull.Value && !string.IsNullOrEmpty(dr["IsUser"].ToString()))
                                _NotificationConfigurationParameter.IsUser = Convert.ToBoolean(dr["IsUser"].ToString());

                            _NotificationConfigurationParameters.Add(_NotificationConfigurationParameter);
                        }
                    }
                    conn.Close();
                }
            }
            return _NotificationConfigurationParameters;
        }
        public void SendNotificationWorkflow(int itemID, int requestID, enumNotificationEventType eventType, List<KPIType> _Types, List<ReminderConfiguration> _Reminders, KPI _KPI, KPIMeasure Measure)
        {
            if (_KPI.GeneralData.ID == 1097)
            {

            }
            string NotificationType = string.Empty;
            NotificationType = "Reminder";
            List<WFRequestStep> _RequestSteps = GetRequestSteps();
            List<NotificationConfiguration> _Templates = GetNotificationConfigurations();
            NotificationConfiguration _template = null;
            Notification _Notification = new Notification();
            List<Notification> ReturnNotifications = new List<Notification>();
            Notification ReturnNotification;
            EmailTemplate _EmailTemplate = new EmailTemplate();
            if (_Templates.Count > 0)
            {
                _template = _Templates.Where(a => a.ActionType == (int)eventType).FirstOrDefault();
            }
            if (_template != null)
            {
                _template = ResolveNotifications(itemID, requestID, _template, _Types, _Reminders, _KPI, Measure, _RequestSteps);

                List<string> CC = new List<string>();
                foreach (NotificationConfigurationReciever reciever in _template.Receivers.Where(a => a.IsCC))
                {
                    foreach (UserInfo user in reciever.Users)
                    {
                        CC.Add(user.Email);
                    }
                }

                foreach (NotificationConfigurationReciever reciever in _template.Receivers)
                {
                    foreach (UserInfo user in reciever.Users)
                    {
                        try
                        {
                            //Handle User Parameter
                            if (_template.NotificationEnglish.Contains("[ReceiverDisplayName]"))
                                _template.NotificationEnglish = _template.NotificationEnglish.Replace("[ReceiverDisplayName]", user.DisplayName);

                            if (_template.NotificationArabic.Contains("[ReceiverDisplayName]"))
                                _template.NotificationArabic = _template.NotificationArabic.Replace("[ReceiverDisplayName]", user.DisplayName);

                            if (_template.EmailSubject.Contains("[ReceiverDisplayName]"))
                                _template.EmailSubject = _template.EmailSubject.Replace("[ReceiverDisplayName]", user.DisplayName);

                            if (_template.EmailBody.Contains("[ReceiverDisplayName]"))
                                _template.EmailBody = _template.EmailBody.Replace("[ReceiverDisplayName]", user.DisplayName);



                            // Send Push Notification
                            ReturnNotification = new Notification();
                            _Notification = new Notification();
                            _Notification.GeneralData.EnglishName = _template.NotificationEnglish;
                            _Notification.GeneralData.ArabicName = _template.NotificationArabic;
                            _Notification.NotificationType = NotificationType;
                            _Notification.KPIID = itemID;
                            _Notification.AssignedTo.UserName = user.UserName;
                            ReturnNotification = CreateNotification(_Notification);
                            ReturnNotifications.Add(ReturnNotification);

                            // Send Email
                            if (!reciever.IsCC)
                            {
                                _EmailTemplate = new EmailTemplate();
                                _EmailTemplate.To = user.Email;
                                _EmailTemplate.CC = CC;
                                _EmailTemplate.Body = _template.EmailBody;
                                _EmailTemplate.Subject = _template.EmailSubject;
                                SendEmail(_EmailTemplate.To, _EmailTemplate.CC, _EmailTemplate.Subject, _EmailTemplate.Body);
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                    }
                }
            }
        }
        public NotificationConfiguration ResolveNotifications(int ItemID, int RequestID, NotificationConfiguration _template, List<KPIType> _Types, List<ReminderConfiguration> _reminders, KPI _KPI, KPIMeasure Measure, List<WFRequestStep> requestSteps)
        {
            #region Resolve Recievers
            List<Delegation> _Delegations = ValidDelegation();
            List<UserInfo> _TempReceivers = new List<UserInfo>();
            foreach (NotificationConfigurationReciever _Reciever in _template.Receivers)
            {
                if (!_Reciever.IsGroup)
                {
                    if (_Reciever.ReceiverName == "Champion")
                        _Reciever.Users.Add(_KPI.Champion);
                    else if (_Reciever.ReceiverName == "Owner")
                        _Reciever.Users.Add(_KPI.Owner);
                    else if (_Reciever.ReceiverName == "Sponsor")
                        _Reciever.Users.Add(_KPI.Sponser);
                    else if (_Reciever.ReceiverName == "AboveApprovers")
                    {
                        List<WFRequestStep> steps = requestSteps.Where(a => a.RequestID == RequestID && a.Status == "Not Started").ToList();
                        foreach (var step in steps)
                        {
                            if (step != null)
                            {
                                if (step.IsGroup)
                                {
                                    List<UserInfo> users = GetUsersByGroupName(step.Approver);
                                    foreach (UserInfo user in users)
                                    {
                                        _Reciever.Users.Add(user);
                                    }
                                }
                                else
                                {
                                    UserInfo user = GetUserByUserName(step.Approver);
                                    _Reciever.Users.Add(user);
                                }
                            }
                        }

                    }
                    else if (_Reciever.ReceiverName == "AllApprovers")
                    {
                        List<WFRequestStep> steps = requestSteps.Where(a => a.RequestID == RequestID).ToList();
                        foreach (var step in steps)
                        {
                            if (step != null)
                            {
                                if (step.IsGroup)
                                {
                                    List<UserInfo> users = GetUsersByGroupName(step.Approver);
                                    foreach (UserInfo user in users)
                                    {
                                        _Reciever.Users.Add(user);
                                    }
                                }
                                else
                                {
                                    UserInfo user = GetUserByUserName(step.Approver);
                                    _Reciever.Users.Add(user);
                                }
                            }
                        }

                    }
                    else if (_Reciever.ReceiverName == "CurrentApprover")
                    {
                        WFRequestStep step = requestSteps.Where(a => a.RequestID == RequestID && a.Status == "Pending").FirstOrDefault();
                        if (step != null)
                        {
                            if (step.IsGroup)
                            {
                                List<UserInfo> users = GetUsersByGroupName(step.Approver);
                                foreach (UserInfo user in users)
                                {
                                    _Reciever.Users.Add(user);
                                }
                            }
                            else
                            {
                                UserInfo user = GetUserByUserName(step.Approver);
                                _Reciever.Users.Add(user);
                            }
                        }
                    }
                    else
                    {
                        List<Group> _Groups = GetGroups();
                        int count = _Groups.Where(a => a.Title.ToLower() == _Reciever.ReceiverName.ToLower()).Count();
                        if (count > 0)
                        {
                            List<UserInfo> _groupUsers = GetUsersByGroupName(_Reciever.ReceiverName);
                            _Reciever.Users.AddRange(_groupUsers);
                        }
                    }                   
                }
                else
                {
                    List<UserInfo> users = GetUsersByGroupName(_Reciever.ReceiverName);
                    foreach (UserInfo user in users)
                    {
                        _Reciever.Users.Add(user);
                    }
                }
                foreach (UserInfo user in _Reciever.Users)
                {
                    List<Delegation> _CurrentDelegations = _Delegations.Where(a => a.FromUser.UserName.ToLower() == user.UserName.ToLower()).ToList();
                    if (_CurrentDelegations.Count() > 0)
                    {
                        //Add Users
                        foreach (Delegation DelegatedUser in _CurrentDelegations)
                        {
                            _TempReceivers.Add(DelegatedUser.ToUser);
                        }
                    }
                }
                _Reciever.Users.AddRange(_TempReceivers);

            }
            #endregion
            #region Resolve Parametrs
            foreach (NotificationConfigurationParameter _Parameter in _template.Parameters)
            {
                if (_Parameter.Title.ToLower() == "[kpiname]")
                    _Parameter.Value = _KPI.GeneralData.EnglishName;

                if (_Parameter.Title.ToLower() == "[kpinamear]")
                    _Parameter.Value = _KPI.GeneralData.ArabicName;

                if (_Parameter.Title.ToLower() == "[measuredate]")
                    _Parameter.Value = DateTime.Parse(Measure.DueDate).Date.ToString("dd-MMM-yyyy");

                if (_Parameter.Title.ToLower() == "[championdisplayname]")
                    _Parameter.Value = _KPI.Champion.DisplayName;

                if (_Parameter.Title.ToLower() == "[value]")
                    _Parameter.Value = Measure.TempValue.ToString();

                if (_Parameter.Title.ToLower() == "[beforereminderdays]")
                {
                    int KPITypeID = _Types.Where(a => a.Name.ToLower() == _KPI.KPIType.ToLower()).FirstOrDefault().ID;
                    _Parameter.Value = _reminders.Where(a => a.TypeID == KPITypeID).FirstOrDefault().BeforeReminder.ToString();
                }


                if (_Parameter.Title.ToLower() == "[kpiurl]")
                {
                    string URL = GetConfigurations("SPlusURL");
                    if (URL.Contains("{ItemID}"))
                    {
                        URL = URL.Replace("{ItemID}", _KPI.GeneralData.ID.ToString());
                    }
                    _Parameter.Value = URL;
                }

            }
            _template = ReplaceText(_template);
            #endregion

            return _template;

        }
        public NotificationConfiguration ReplaceText(NotificationConfiguration _template)
        {
            foreach (NotificationConfigurationParameter _Parameter in _template.Parameters)
            {
                _template.NotificationEnglish = _template.NotificationEnglish.Replace(_Parameter.Title, _Parameter.Value);
                _template.NotificationArabic = _template.NotificationArabic.Replace(_Parameter.Title, _Parameter.Value);
                _template.EmailSubject = _template.EmailSubject.Replace(_Parameter.Title, _Parameter.Value);
                _template.EmailBody = _template.EmailBody.Replace(_Parameter.Title, _Parameter.Value);
            }
            return _template;
        }
        public Notification CreateNotification(Notification _Notification)
        {
            using (SqlConnection con = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("Notifications_Create", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    var returnParameter = cmd.Parameters.Add("@ID", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("@AssignedTo", SqlDbType.NVarChar).Value = _Notification.AssignedTo.UserName;
                    cmd.Parameters.Add("@KPIID", SqlDbType.Decimal).Value = _Notification.KPIID;
                    cmd.Parameters.Add("@NotificationType", SqlDbType.NVarChar).Value = _Notification.NotificationType;
                    cmd.Parameters.Add("@Status", SqlDbType.NVarChar).Value = "Pending";
                    cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = _Notification.GeneralData.EnglishName;
                    cmd.Parameters.Add("@TitleArabic", SqlDbType.NVarChar).Value = _Notification.GeneralData.ArabicName;
                    cmd.ExecuteNonQuery();
                    if (returnParameter.Value != null)
                    {
                        _Notification.ID = Convert.ToInt32(returnParameter.Value);
                    }
                    con.Close();
                }
            }
            return _Notification;
        }
        public void SendNotificationForDelegation(Delegation _Delegation, enumNotificationEventType eventType)
        {
            string NotificationType = string.Empty;
            NotificationType = "Delegation";
            List<NotificationConfiguration> _Templates = GetNotificationConfigurations();
            NotificationConfiguration _template = null;
            Notification _Notification = new Notification();
            List<Notification> ReturnNotifications = new List<Notification>();
            Notification ReturnNotification;
            EmailTemplate _EmailTemplate = new EmailTemplate();
            if (_Templates.Count > 0)
            {
                _template = _Templates.Where(a => a.ActionType == (int)eventType).FirstOrDefault();
            }
            if (_template != null)
            {
                _template = ResolveNotificationForDelegation(_Delegation, _template);

                List<string> CC = new List<string>();
                foreach (NotificationConfigurationReciever reciever in _template.Receivers.Where(a => a.IsCC))
                {
                    foreach (UserInfo user in reciever.Users)
                    {
                        CC.Add(user.Email);
                    }
                }

                foreach (NotificationConfigurationReciever reciever in _template.Receivers)
                {
                    foreach (UserInfo user in reciever.Users)
                    {
                        try
                        {
                            //Handle User Parameter
                            if (_template.NotificationEnglish.Contains("[ReceiverDisplayName]"))
                                _template.NotificationEnglish = _template.NotificationEnglish.Replace("[ReceiverDisplayName]", user.DisplayName);

                            if (_template.NotificationArabic.Contains("[ReceiverDisplayName]"))
                                _template.NotificationArabic = _template.NotificationArabic.Replace("[ReceiverDisplayName]", user.DisplayName);

                            if (_template.EmailSubject.Contains("[ReceiverDisplayName]"))
                                _template.EmailSubject = _template.EmailSubject.Replace("[ReceiverDisplayName]", user.DisplayName);

                            if (_template.EmailBody.Contains("[ReceiverDisplayName]"))
                                _template.EmailBody = _template.EmailBody.Replace("[ReceiverDisplayName]", user.DisplayName);



                            // Send Push Notification
                            ReturnNotification = new Notification();
                            _Notification = new Notification();
                            _Notification.GeneralData.EnglishName = _template.NotificationEnglish;
                            _Notification.GeneralData.ArabicName = _template.NotificationArabic;
                            _Notification.NotificationType = NotificationType;
                            _Notification.DelegationID = _Delegation.ID;
                            _Notification.AssignedTo.UserName = user.UserName;
                            ReturnNotification = CreateDelegationNotification(_Notification);
                            ReturnNotifications.Add(ReturnNotification);

                            // Send Email
                            if (!reciever.IsCC)
                            {
                                _EmailTemplate = new EmailTemplate();
                                _EmailTemplate.To = user.Email;
                                _EmailTemplate.CC = CC;
                                _EmailTemplate.Body = _template.EmailBody;
                                _EmailTemplate.Subject = _template.EmailSubject;
                                SendEmail(_EmailTemplate.To, _EmailTemplate.CC, _EmailTemplate.Subject, _EmailTemplate.Body);
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                    }
                }
            }
        }
        public NotificationConfiguration ResolveNotificationForDelegation(Delegation _Delegation, NotificationConfiguration _template)
        {
            List<ReminderConfiguration> _Reminders = GetReminderConfigration();

            #region Resolve Recievers
            foreach (NotificationConfigurationReciever _Reciever in _template.Receivers)
            {
                if (_Reciever.ReceiverName == "ToDelegationUser")
                    _Reciever.Users.Add(_Delegation.ToUser);
                else if (_Reciever.ReceiverName == "FromDelegationUser")
                    _Reciever.Users.Add(_Delegation.FromUser);
                else
                {
                    List<Group> _Groups = GetGroups();
                    int count = _Groups.Where(a => a.Title.ToLower() == _Reciever.ReceiverName.ToLower()).Count();
                    if (count > 0)
                    {
                        List<UserInfo> _groupUsers = GetUsersByGroupName(_Reciever.ReceiverName);
                        _Reciever.Users.AddRange(_groupUsers);
                    }
                }
            }
            #endregion
            #region Resolve Parametrs
            foreach (NotificationConfigurationParameter _Parameter in _template.Parameters)
            {
                if (_Parameter.Title.ToLower() == "[delegationdatefrom]")
                    _Parameter.Value = _Delegation.FromDate.Date.ToString("dd-MMM-yyyy");

                if (_Parameter.Title.ToLower() == "[delegationdateto]")
                    _Parameter.Value = _Delegation.ToDate.Date.ToString("dd-MMM-yyyy");

                if (_Parameter.Title.ToLower() == "[delegationfromdisplayname]")
                    _Parameter.Value = _Delegation.FromUser.DisplayName;

                if (_Parameter.Title.ToLower() == "[delegationurl]")
                {
                    string URL = GetConfigurations("SPlusDelegationURL");

                    _Parameter.Value = URL;
                }

            }
            _template = ReplaceText(_template);
            #endregion

            return _template;
        }
        public Notification CreateDelegationNotification(Notification _Notification)
        {
            using (SqlConnection con = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("Notifications_Create_Delegation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    var returnParameter = cmd.Parameters.Add("@ID", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("@AssignedTo", SqlDbType.NVarChar).Value = _Notification.AssignedTo.UserName;
                    cmd.Parameters.Add("@DelegationID", SqlDbType.Decimal).Value = _Notification.DelegationID;
                    cmd.Parameters.Add("@NotificationType", SqlDbType.NVarChar).Value = _Notification.NotificationType;
                    cmd.Parameters.Add("@Status", SqlDbType.NVarChar).Value = "Pending";
                    cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = _Notification.GeneralData.EnglishName;
                    cmd.Parameters.Add("@TitleArabic", SqlDbType.NVarChar).Value = _Notification.GeneralData.ArabicName;
                    cmd.ExecuteNonQuery();
                    if (returnParameter.Value != null)
                    {
                        _Notification.ID = Convert.ToInt32(returnParameter.Value);
                    }
                    con.Close();
                }
            }
            return _Notification;
        }
        #endregion

        #region Delegation
        public Delegation GetDelegationByID(int ID)
        {
            Delegation _Delegation = null;

            List<UserInfo> _UserInfos = new List<UserInfo>();
            _UserInfos = GetAllUsers();
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("Delegation_ByID_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.Add("@ID", SqlDbType.NVarChar).Value = ID;
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            _Delegation = new Delegation();
                            if (dr["ID"] != DBNull.Value && !string.IsNullOrEmpty(dr["ID"].ToString()))
                            {
                                _Delegation.ID = Convert.ToInt32(dr["ID"]);
                            }
                            if (dr["FromUser"] != DBNull.Value && !string.IsNullOrEmpty(dr["FromUser"].ToString()))
                            {
                                var value = dr["FromUser"].ToString();
                                if (value.Contains("#"))
                                {
                                    string UserName = value.Remove(0, value.IndexOf('#') + 1).ToLower();
                                    _Delegation.FromUser = _UserInfos.Where(a => a.UserName.ToLower() == UserName.ToLower() || a.DisplayName.ToLower() == UserName.ToLower()).ToList().FirstOrDefault();
                                }
                                else
                                {
                                    _Delegation.FromUser = _UserInfos.Where(a => a.UserName.ToLower() == value.ToLower() || a.DisplayName.ToLower() == value.ToLower()).ToList().FirstOrDefault();
                                }
                            }
                            if (dr["ToUser"] != DBNull.Value && !string.IsNullOrEmpty(dr["ToUser"].ToString()))
                            {
                                var value = dr["ToUser"].ToString();
                                if (value.Contains("#"))
                                {
                                    string UserName = value.Remove(0, value.IndexOf('#') + 1).ToLower();
                                    _Delegation.ToUser = _UserInfos.Where(a => a.UserName.ToLower() == UserName.ToLower() || a.DisplayName.ToLower() == UserName.ToLower()).ToList().FirstOrDefault();
                                }
                                else
                                {
                                    _Delegation.ToUser = _UserInfos.Where(a => a.UserName.ToLower() == value.ToLower() || a.DisplayName.ToLower() == value.ToLower()).ToList().FirstOrDefault();
                                }
                            }
                            if (dr["FromDate"] != DBNull.Value && !string.IsNullOrEmpty(dr["FromDate"].ToString()))
                            {
                                _Delegation.FromDate = Convert.ToDateTime(dr["FromDate"].ToString());
                            }
                            if (dr["ToDate"] != DBNull.Value && !string.IsNullOrEmpty(dr["ToDate"].ToString()))
                            {
                                _Delegation.ToDate = Convert.ToDateTime(dr["ToDate"].ToString());
                            }
                            if (dr["CreatedBy"] != DBNull.Value && !string.IsNullOrEmpty(dr["CreatedBy"].ToString()))
                            {
                                var value = dr["CreatedBy"].ToString();
                                if (value.Contains("#"))
                                {
                                    string UserName = value.Remove(0, value.IndexOf('#') + 1).ToLower();
                                    _Delegation.CreatedBy = _UserInfos.Where(a => a.UserName.ToLower() == UserName.ToLower() || a.DisplayName.ToLower() == UserName.ToLower()).ToList().FirstOrDefault();
                                }
                                else
                                {
                                    _Delegation.CreatedBy = _UserInfos.Where(a => a.UserName.ToLower() == value.ToLower() || a.DisplayName.ToLower() == value.ToLower()).ToList().FirstOrDefault();
                                }
                            }
                            if (dr["ModifiedBy"] != DBNull.Value && !string.IsNullOrEmpty(dr["ModifiedBy"].ToString()))
                            {
                                var value = dr["ModifiedBy"].ToString();
                                if (value.Contains("#"))
                                {
                                    string UserName = value.Remove(0, value.IndexOf('#') + 1).ToLower();
                                    _Delegation.ModifiedBy = _UserInfos.Where(a => a.UserName.ToLower() == UserName.ToLower() || a.DisplayName.ToLower() == UserName.ToLower()).ToList().FirstOrDefault();
                                }
                                else
                                {
                                    _Delegation.ModifiedBy = _UserInfos.Where(a => a.UserName.ToLower() == value.ToLower() || a.DisplayName.ToLower() == value.ToLower()).ToList().FirstOrDefault();
                                }
                            }
                            if (dr["Created"] != DBNull.Value && !string.IsNullOrEmpty(dr["Created"].ToString()))
                            {
                                _Delegation.Created = Convert.ToDateTime(dr["Created"].ToString());
                            }
                            if (dr["Modified"] != DBNull.Value && !string.IsNullOrEmpty(dr["Modified"].ToString()))
                            {
                                _Delegation.Modified = Convert.ToDateTime(dr["Modified"].ToString());
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return _Delegation;
        }

        public List<Delegation> GetDelegations()
        {
            List<Delegation> _Delegations = new List<Delegation>();
            Delegation _Delegation = null;

            List<UserInfo> _UserInfos = new List<UserInfo>();
            _UserInfos = GetAllUsers();
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("Delegation_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            _Delegation = new Delegation();
                            if (dr["ID"] != DBNull.Value && !string.IsNullOrEmpty(dr["ID"].ToString()))
                            {
                                _Delegation.ID = Convert.ToInt32(dr["ID"]);
                            }
                            if (dr["FromUser"] != DBNull.Value && !string.IsNullOrEmpty(dr["FromUser"].ToString()))
                            {
                                var value = dr["FromUser"].ToString();
                                if (value.Contains("#"))
                                {
                                    string UserName = value.Remove(0, value.IndexOf('#') + 1).ToLower();
                                    _Delegation.FromUser = _UserInfos.Where(a => a.UserName.ToLower() == UserName.ToLower() || a.DisplayName.ToLower() == UserName.ToLower()).ToList().FirstOrDefault();
                                }
                                else
                                {
                                    _Delegation.FromUser = _UserInfos.Where(a => a.UserName.ToLower() == value.ToLower() || a.DisplayName.ToLower() == value.ToLower()).ToList().FirstOrDefault();
                                }
                            }
                            if (dr["ToUser"] != DBNull.Value && !string.IsNullOrEmpty(dr["ToUser"].ToString()))
                            {
                                var value = dr["ToUser"].ToString();
                                if (value.Contains("#"))
                                {
                                    string UserName = value.Remove(0, value.IndexOf('#') + 1).ToLower();
                                    _Delegation.ToUser = _UserInfos.Where(a => a.UserName.ToLower() == UserName.ToLower() || a.DisplayName.ToLower() == UserName.ToLower()).ToList().FirstOrDefault();
                                }
                                else
                                {
                                    _Delegation.ToUser = _UserInfos.Where(a => a.UserName.ToLower() == value.ToLower() || a.DisplayName.ToLower() == value.ToLower()).ToList().FirstOrDefault();
                                }
                            }
                            if (dr["FromDate"] != DBNull.Value && !string.IsNullOrEmpty(dr["FromDate"].ToString()))
                            {
                                _Delegation.FromDate = Convert.ToDateTime(dr["FromDate"].ToString());
                            }
                            if (dr["ToDate"] != DBNull.Value && !string.IsNullOrEmpty(dr["ToDate"].ToString()))
                            {
                                _Delegation.ToDate = Convert.ToDateTime(dr["ToDate"].ToString());
                            }
                            if (dr["CreatedBy"] != DBNull.Value && !string.IsNullOrEmpty(dr["CreatedBy"].ToString()))
                            {
                                var value = dr["CreatedBy"].ToString();
                                if (value.Contains("#"))
                                {
                                    string UserName = value.Remove(0, value.IndexOf('#') + 1).ToLower();
                                    _Delegation.CreatedBy = _UserInfos.Where(a => a.UserName.ToLower() == UserName.ToLower() || a.DisplayName.ToLower() == UserName.ToLower()).ToList().FirstOrDefault();
                                }
                                else
                                {
                                    _Delegation.CreatedBy = _UserInfos.Where(a => a.UserName.ToLower() == value.ToLower() || a.DisplayName.ToLower() == value.ToLower()).ToList().FirstOrDefault();
                                }
                            }
                            if (dr["ModifiedBy"] != DBNull.Value && !string.IsNullOrEmpty(dr["ModifiedBy"].ToString()))
                            {
                                var value = dr["ModifiedBy"].ToString();
                                if (value.Contains("#"))
                                {
                                    string UserName = value.Remove(0, value.IndexOf('#') + 1).ToLower();
                                    _Delegation.ModifiedBy = _UserInfos.Where(a => a.UserName.ToLower() == UserName.ToLower() || a.DisplayName.ToLower() == UserName.ToLower()).ToList().FirstOrDefault();
                                }
                                else
                                {
                                    _Delegation.ModifiedBy = _UserInfos.Where(a => a.UserName.ToLower() == value.ToLower() || a.DisplayName.ToLower() == value.ToLower()).ToList().FirstOrDefault();
                                }
                            }
                            if (dr["Created"] != DBNull.Value && !string.IsNullOrEmpty(dr["Created"].ToString()))
                            {
                                _Delegation.Created = Convert.ToDateTime(dr["Created"].ToString());
                            }
                            if (dr["Modified"] != DBNull.Value && !string.IsNullOrEmpty(dr["Modified"].ToString()))
                            {
                                _Delegation.Modified = Convert.ToDateTime(dr["Modified"].ToString());
                            }
                            _Delegations.Add(_Delegation);
                        }
                    }
                    conn.Close();
                }
            }
            return _Delegations;
        }

        public List<Delegation> ValidDelegation()
        {
            List<Delegation> _Delegations = GetDelegations();
            List<Delegation> _ValidDelegations = new List<Delegation>();

            _ValidDelegations = _Delegations.Where(w => w.FromDate.Date <= DateTime.Today.Date).Where(w => w.ToDate.Date >= DateTime.Today.Date).ToList();

            return _ValidDelegations;
        }
        #endregion

        #region Email
        public DataTable EmailConfigurationList()
        {
            DataTable dtConfig = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection))
                {
                    using (SqlCommand cmd = new SqlCommand("EmailConfiguration_Read", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            dtConfig.Load(dr);
                        }
                        conn.Close();
                    }
                }
                return dtConfig;
            }
            catch (Exception)
            {

                dtConfig = null;
            }

            return dtConfig;
        }
        public void SendEmail(string To, List<string> CC, string Subject, string Body)
        {
            var dtConfig = EmailConfigurationList();
            try
            {
                if (dtConfig != null)
                {
                    MailMessage message = new MailMessage();
                    //message.Body += eNemailBody;
                    //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
                    //  message.Body,
                    //  null, "text/html");
                    var From = dtConfig.Rows[0]["SenderEmail"].ToString();
                    var SMTP = dtConfig.Rows[0]["SmtpServer"].ToString();
                    var Port = int.Parse(dtConfig.Rows[0]["SmtpPort"].ToString());
                    var credentialsEmail = dtConfig.Rows[0]["SenderEmail"].ToString();
                    var credentialsPassword = dtConfig.Rows[0]["SenderPassword"].ToString();
                    var enableSsl = !string.IsNullOrEmpty(Convert.ToString(dtConfig.Rows[0]["EnableSsl"])) ? Convert.ToBoolean(dtConfig.Rows[0]["EnableSsl"].ToString()) : false;
                    if (!string.IsNullOrEmpty(To) &&!string.IsNullOrEmpty(From) && !string.IsNullOrEmpty(SMTP) && !string.IsNullOrEmpty(Port.ToString()) && !string.IsNullOrEmpty(credentialsEmail) && !string.IsNullOrEmpty(credentialsPassword))
                    {
                        using (MailMessage mm = new MailMessage(From, To))
                        {
                            mm.Subject = Subject;

                            mm.Body = Body;
                            mm.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = SMTP;
                            smtp.EnableSsl = enableSsl;//true;
                            NetworkCredential NetworkCred = new NetworkCredential(credentialsEmail, Decrypt(credentialsPassword));
                            smtp.UseDefaultCredentials = false;//true;
                            smtp.Credentials = NetworkCred;
                            smtp.Port = Port;
                            foreach (string ccmail in CC)
                            {
                                if (!string.IsNullOrEmpty(ccmail))
                                    mm.CC.Add(ccmail);
                            }

                            smtp.Send(mm);

                        }
                    }

                }
            }
            catch
            {
            }

        }
        public string Decrypt(string cipherText)
        {

            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        #endregion

        #region Helper
        public DateTime GetEndDateWorkingDays(DateTime startDate, int Duration)
        {
            DateTime finishDate = startDate;
            for (int i = 1; i <= Duration; i++)
            {
                finishDate = finishDate.AddDays(1);
                if (finishDate.DayOfWeek == DayOfWeek.Friday || finishDate.DayOfWeek == DayOfWeek.Saturday)
                    Duration = Duration + 1;
            }
            return finishDate;
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
        public string GetConfigurations(string Key)
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

        #endregion
    }
}
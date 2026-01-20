using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SyncJobs.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SyncJobs
{
    class PortfolioSyncJob : SPJobDefinition
    {
        string SiteURL = null;
        List<Portfolio> _SPlusPortfolios = new List<Portfolio>();
        List<Portfolio> _PPlusPortfolios = new List<Portfolio>();
        DataTable dtPPlusCredential = null;
        string PPlusUserName = string.Empty;
        string PPlusPassword = string.Empty;
        string PPlusSiteURL = string.Empty;
        string Token = string.Empty;
        string Authorization = string.Empty;
        string Connection = "";
        public PortfolioSyncJob() : base() { }

        public PortfolioSyncJob(string jobName, SPService service) : base(jobName, service, null, SPJobLockType.None)
        {
            this.Title = "PortfolioSyncTimerJob";
        }
        public PortfolioSyncJob(string jobName, SPWebApplication webapp) : base(jobName, webapp, null, SPJobLockType.ContentDatabase)
        {
            this.Title = "PortfolioSyncTimerJob";
        }
        public override void Execute(Guid targetInstanceId)
        {
            try
            {
                SiteURL = this.Properties["SiteUrl"].ToString();
                Connection = GetConnectionStringFromSP(SiteURL);
                if (!string.IsNullOrEmpty(Connection))
                {
                    dtPPlusCredential = GetPPlusCredential();
                    PPlusUserName = dtPPlusCredential.Rows[0]["Title"].ToString();
                    PPlusPassword = dtPPlusCredential.Rows[0]["Password"].ToString();
                    PPlusSiteURL = dtPPlusCredential.Rows[0]["SiteURL"].ToString();
                    Token = Encryption.GenerateSecurityToken(PPlusUserName, Encryption.Decrypt(PPlusPassword));
                    Authorization = dtPPlusCredential.Rows[0]["Authorization"].ToString();

                    _SPlusPortfolios = GetSPlusPortfolio();
                    _PPlusPortfolios = GetPPlusPortfolio();
                    if (_PPlusPortfolios != null)
                        CheckSPlusPortfolio(_SPlusPortfolios, _PPlusPortfolios);
                }
            }
            catch (Exception ex)
            {
                AddException("Execute", ex.Message);
            }
        }
        #region Log Area
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
                    cmd.Parameters.Add("@Controller", SqlDbType.NVarChar).Value = "Portfolio Sync Job";
                    cmd.Parameters.Add("@FunctionName", SqlDbType.NVarChar).Value = FunctionName;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        #endregion
        #region SPlus

        private List<Portfolio> GetSPlusPortfolio()
        {
            try
            {
                List<Portfolio> _Portfolios = new List<Portfolio>();
                Portfolio _Portfolio;
                using (SqlConnection conn = new SqlConnection(Connection))
                {
                    using (SqlCommand cmd = new SqlCommand("Portfolio_Read", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                _Portfolio = new Portfolio();
                                _Portfolio.Id = Convert.ToInt32(dr["ID"]);
                                if (dr["Title"] != DBNull.Value && !string.IsNullOrEmpty(dr["Title"].ToString()))
                                {
                                    _Portfolio.NameEnglish = Convert.ToString(dr["Title"]);
                                }
                                if (dr["ArabicName"] != DBNull.Value && !string.IsNullOrEmpty(dr["ArabicName"].ToString()))
                                {
                                    _Portfolio.NameArabic = Convert.ToString(dr["ArabicName"]);
                                }
                                if (dr["Progress"] != DBNull.Value && !string.IsNullOrEmpty(dr["Progress"].ToString()))
                                {
                                    _Portfolio.Progress = Convert.ToInt32(dr["Progress"].ToString());
                                }
                                if (dr["Status"] != DBNull.Value && !string.IsNullOrEmpty(dr["Status"].ToString()))
                                {
                                    _Portfolio.Status = Convert.ToString(dr["Status"]);
                                }
                                if (dr["PPlusPortfolioId"] != DBNull.Value && !string.IsNullOrEmpty(dr["PPlusPortfolioId"].ToString()))
                                {
                                    _Portfolio.ProjectUID = Convert.ToString(dr["PPlusPortfolioId"]);
                                }


                                _Portfolios.Add(_Portfolio);
                            }
                        }
                        conn.Close();
                    }
                }
                return _Portfolios;
            }
            catch (Exception ex)
            {
                AddException("GetSPlusPortfolio", ex.Message);
                return null;
            }
        }

        private DataTable GetPPlusCredential()
        {

            DataTable dtPPlusCredential = new DataTable();
            using (SqlConnection conn = new SqlConnection(Connection))
            {
                using (SqlCommand cmd = new SqlCommand("PPlusCredential_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dtPPlusCredential.Load(dr);
                    }
                    conn.Close();
                }
            }
            return dtPPlusCredential;
        }
        private void CheckSPlusPortfolio(List<Portfolio> splusPortfolios, List<Portfolio> pplusPortfolios)
        {
            try
            {
                List<Portfolio> portfoliosNeedToUpdate = new List<Portfolio>();
                List<Portfolio> portfoliosNeedToBeAdded = new List<Portfolio>();
                foreach (Portfolio pplusPItem in pplusPortfolios)
                {
                    if (splusPortfolios != null)
                    {
                        bool isExist = splusPortfolios.Where(i => i.ProjectUID == pplusPItem.ProjectUID.ToString()).Count() > 0;
                        if (isExist)
                        {
                            portfoliosNeedToUpdate.Add(pplusPItem);

                        }
                        else
                        {
                            portfoliosNeedToBeAdded.Add(pplusPItem);
                        }
                    }
                    else
                        portfoliosNeedToBeAdded.Add(pplusPItem);


                }
                UpdateAddPlusPortfolio(portfoliosNeedToUpdate, portfoliosNeedToBeAdded);


            }
            catch (Exception ex)
            {
                AddException("CheckSPlusPortfolio", ex.Message);

            }
        }
        private void UpdateAddPlusPortfolio(List<Portfolio> portfoliosNeedToUpdate, List<Portfolio> portfoliosNeedToBeAdded)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Connection))
                {
                    using (SqlCommand cmd = new SqlCommand("Portfolio_Create_Update", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@IsAdd", SqlDbType.Bit);
                        cmd.Parameters.Add("@Title", SqlDbType.NVarChar);
                        cmd.Parameters.Add("@ArabicName", SqlDbType.NVarChar);
                        cmd.Parameters.Add("@Progress", SqlDbType.NVarChar);
                        cmd.Parameters.Add("@Status", SqlDbType.NVarChar);
                        cmd.Parameters.Add("@PPlusPortfolioId", SqlDbType.NVarChar);
                        conn.Open();
                        foreach (var updateItem in portfoliosNeedToUpdate)
                        {
                            cmd.Parameters["@IsAdd"].Value = 0;
                            cmd.Parameters["@Progress"].Value = updateItem.Progress;
                            cmd.Parameters["@Status"].Value = updateItem.Status;
                            cmd.Parameters["@Title"].Value = updateItem.NameEnglish;
                            cmd.Parameters["@ArabicName"].Value = updateItem.NameArabic;
                            cmd.Parameters["@PPlusPortfolioId"].Value = updateItem.ProjectUID;
                            cmd.ExecuteNonQuery();
                        }
                        foreach (var addItem in portfoliosNeedToBeAdded)
                        {
                            cmd.Parameters["@IsAdd"].Value = 1;
                            cmd.Parameters["@Progress"].Value = addItem.Progress;
                            cmd.Parameters["@Status"].Value = addItem.Status;
                            cmd.Parameters["@Title"].Value = addItem.NameEnglish;
                            cmd.Parameters["@ArabicName"].Value = addItem.NameArabic;
                            cmd.Parameters["@PPlusPortfolioId"].Value = addItem.ProjectUID;
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                AddException("UpdateAddPlusPortfolio", ex.Message);

            }
        }

        #endregion
        #region PPlus
        private List<Portfolio> GetPPlusPortfolio()
        {
            try
            {
                List<Portfolio> _PPlusPortfolios = new List<Portfolio>();
                Portfolio _objPPlusPortfolio = null; ;
                HttpClient client = new HttpClient();
                var request = (HttpWebRequest)WebRequest.Create(PPlusSiteURL);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Token", Token);
                request.Headers.Add("Authorization", Authorization);

                var response = (HttpWebResponse)request.GetResponse();
                var FinalResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();

                JObject JsonObject = JsonConvert.DeserializeObject<JObject>(FinalResponse);
                JArray JsonData = (JArray)JsonObject["Data"];

                foreach (var result in JsonData)
                {
                    _objPPlusPortfolio = new Portfolio();
                    _objPPlusPortfolio.NameArabic = Convert.ToString(result["NameArabic"]);
                    _objPPlusPortfolio.NameEnglish = Convert.ToString(result["NameEnglish"]);
                    _objPPlusPortfolio.Status = Convert.ToString(result["Status"]);
                    _objPPlusPortfolio.Progress = Convert.ToInt32(result["Progress"]);
                    _objPPlusPortfolio.ProjectUID = Convert.ToString(result["ProjectUID"]);
                    _PPlusPortfolios.Add(_objPPlusPortfolio);
                }



                return _PPlusPortfolios;

            }
            catch (Exception ex)
            {
                AddException("GetPPlusPortfolio", ex.Message);
                return null;
            }
        }
        #endregion
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

/*
 * 
 *   #region Log Area
        public void AddException(string FunctionName, string Message)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite sps = new SPSite(SiteURL))
                {
                    using (SPWeb spw = sps.OpenWeb())
                    {
                        spw.AllowUnsafeUpdates = true;
                        SPList spl = spw.Lists.TryGetList("Exceptions");
                        SPListItem item = spl.Items.Add();
                        item["Title"] = "Exception";
                        item["Message"] = Message;
                        item["Controller"] = "Portfolio Sync Job";
                        item["Function"] = FunctionName;
                        item.Update();
                        spw.AllowUnsafeUpdates = false;
                    }
                }
            });
        }

        #endregion
        #region SPlus

        private List<Portfolio> GetSPlusPortfolio()
        {
            try
            {
                List<Portfolio> _Portfolios = new List<Portfolio>();
                Portfolio _Portfolio = null;
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite sps = new SPSite(SiteURL))
                    {
                        using (SPWeb spw = sps.OpenWeb())
                        {
                            spw.AllowUnsafeUpdates = true;
                            SPList spl = spw.Lists["Portfolio"];
                            SPListItemCollection splic = spl.GetItems();
                            foreach (SPListItem spli in splic)
                            {
                                _Portfolio = new Portfolio();
                                _Portfolio.Id = spli.ID;
                                if (spli["Title"] != null)
                                {
                                    _Portfolio.NameEnglish = Convert.ToString(spli["Title"]);
                                }
                                if (spli["ArabicName"] != null)
                                {
                                    _Portfolio.NameArabic = Convert.ToString(spli["ArabicName"]);
                                }
                                if (spli["Progress"] != null)
                                {
                                    _Portfolio.Progress = Convert.ToInt32(spli["Progress"].ToString());
                                }
                                if (spli["Status"] != null)
                                {
                                    _Portfolio.Status = Convert.ToString(spli["Status"]);
                                }
                                if (spli["PPlusPortfolioId"] != null)
                                {
                                    _Portfolio.ProjectUID = Convert.ToString(spli["PPlusPortfolioId"]);
                                }


                                _Portfolios.Add(_Portfolio);
                            }
                            spw.AllowUnsafeUpdates = false;
                        }
                    }
                });
                return _Portfolios;
            }
            catch (Exception ex)
            {
                AddException("GetSPlusPortfolio", ex.Message);
                return null;
            }
        }

        private DataTable GetPPlusCredential()
        {

            DataTable dtPPlusCredential = null;
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite sps = new SPSite(SiteURL))
                {
                    using (SPWeb spw = sps.OpenWeb())
                    {

                        SPList _list = spw.Lists["PPlusCredential"];
                        if (_list != null)
                        {

                            dtPPlusCredential = _list.GetItems().GetDataTable();
                        }



                    }
                }
            });
            return dtPPlusCredential;
        }
        private void CheckSPlusPortfolio(List<Portfolio> splusPortfolios, List<Portfolio> pplusPortfolios)
        {
            try
            {
                List<Portfolio> portfoliosNeedToUpdate = new List<Portfolio>();
                List<Portfolio> portfoliosNeedToBeAdded = new List<Portfolio>();
                foreach (Portfolio pplusPItem in pplusPortfolios)
                {
                    if (splusPortfolios != null)
                    {
                        bool isExist = splusPortfolios.Where(i => i.ProjectUID == pplusPItem.ProjectUID.ToString()).Count() > 0;
                        if (isExist)
                        {
                            portfoliosNeedToUpdate.Add(pplusPItem);

                        }
                        else
                        {
                            portfoliosNeedToBeAdded.Add(pplusPItem);
                        }
                    }
                    else
                        portfoliosNeedToBeAdded.Add(pplusPItem);


                }
                UpdateAddPlusPortfolio(portfoliosNeedToUpdate, portfoliosNeedToBeAdded);


            }
            catch (Exception ex)
            {
                AddException("CheckSPlusPortfolio", ex.Message);

            }
        }
        private void UpdateAddPlusPortfolio(List<Portfolio> portfoliosNeedToUpdate, List<Portfolio> portfoliosNeedToBeAdded)
        {
            try
            {

                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite sps = new SPSite(SiteURL))
                    {
                        using (SPWeb spw = sps.OpenWeb())
                        {
                            spw.AllowUnsafeUpdates = true;
                            SPList spl = spw.Lists["Portfolio"];
                            SPListItemCollection splic = null;

                            foreach (var updateItem in portfoliosNeedToUpdate)
                            {
                                SPQuery qry = new SPQuery();
                                qry.Query = @"<Where><Eq><FieldRef Name='PPlusPortfolioId' />
                                               <Value Type='Text'>" + updateItem.ProjectUID + "</Value></Eq></Where>";

                                splic = spl.GetItems(qry);
                                foreach (SPListItem spli in splic)
                                {
                                    spli["Progress"] = updateItem.Progress;
                                    spli["Status"] = updateItem.Status;
                                    spli.Update();

                                }
                            }
                            foreach (var addItem in portfoliosNeedToBeAdded)
                            {
                                SPListItem spli = spl.Items.Add();

                                spli["Title"] = addItem.NameEnglish;
                                spli["ArabicName"] = addItem.NameArabic;
                                spli["Progress"] = addItem.Progress;
                                spli["Status"] = addItem.Status;
                                spli["PPlusPortfolioId"] = addItem.ProjectUID;
                                spli.Update();
                            }

                            spw.AllowUnsafeUpdates = false;
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                AddException("UpdateAddPlusPortfolio", ex.Message);

            }
        }

        #endregion
        #region PPlus
        private List<Portfolio> GetPPlusPortfolio()
        {
            try
            {
                List<Portfolio> _PPlusPortfolios = new List<Portfolio>();
                Portfolio _objPPlusPortfolio = null; ;
                HttpClient client = new HttpClient();
                var request = (HttpWebRequest)WebRequest.Create(PPlusSiteURL);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Token", Token);
                request.Headers.Add("Authorization", Authorization);

                var response = (HttpWebResponse)request.GetResponse();
                var FinalResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();

                JObject JsonObject = JsonConvert.DeserializeObject<JObject>(FinalResponse);
                JArray JsonData = (JArray)JsonObject["Data"];

                foreach (var result in JsonData)
                {
                    _objPPlusPortfolio = new Portfolio();
                    _objPPlusPortfolio.NameArabic = Convert.ToString(result["NameArabic"]);
                    _objPPlusPortfolio.NameEnglish = Convert.ToString(result["NameEnglish"]);
                    _objPPlusPortfolio.Status = Convert.ToString(result["Status"]);
                    _objPPlusPortfolio.Progress = Convert.ToInt32(result["Progress"]);
                    _objPPlusPortfolio.ProjectUID = Convert.ToString(result["ProjectUID"]);
                    _PPlusPortfolios.Add(_objPPlusPortfolio);
                }



                return _PPlusPortfolios;

            }
            catch (Exception ex)
            {
                AddException("GetSPlusPortfolio", ex.Message);
                return null;
            }
        }
        #endregion
  */

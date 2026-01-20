using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace SyncJobs
{
    internal static class Helper
    {
        public static string GetConfigurations(string Connection, string Key)
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
    }
}

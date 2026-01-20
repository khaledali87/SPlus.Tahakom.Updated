using SPlus.Integration.IntegrationKitchen.AccessInterfaces;
using SPlus.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static SPlus.Integration.Enums;

namespace SPlus.Integration.IntegrationKitchen.AccessImplementations.DatabaseLevel
{
    public class SQLAccess : IAccess
    {
        string strConnect = "";
        //private readonly SQLAccess instance = new SQLAccess(); 
        private string whereCommand = null;
        private SqlDataAdapter adapter;
        private DataSet ds;
        private List<Database> databases;
        private Schema schema;
        private Table table;
        private Column column;

        private string orderByColumn;

        private List<string> databsesNotToAccess = new List<string>();
        private List<string> databsesToAccess = new List<string>();

        private List<string> schemasNotToAccess = new List<string>();
        private List<string> schemasToAccess = new List<string>();

        private List<string> tablesNotToAccess = new List<string>();
        private List<string> tablesToAccess = new List<string>();

        private List<string> columnsNotToAccess = new List<string>();
        private List<string> columnsToAccess = new List<string>();

        public SQLAccess()
        {
            PrepareConfigs();
            adapter = new SqlDataAdapter();
            databases = new List<Database>();
            ds = new DataSet();
            schema = new Schema();
            table = new Table();
            column = new Column();
        }

        private void PrepareConfigs()
        {
            strConnect = ConfigurationManager.AppSettings["IntegrationSQLConnectionString"];
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DataBasesNotToAccess"].ToString()))
            {
                databsesNotToAccess.AddRange(ConfigurationManager.AppSettings["DataBasesNotToAccess"].ToString().Split(',').ToList());
            }
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DataBasesToAccess"].ToString()))
            {
                databsesToAccess.AddRange(ConfigurationManager.AppSettings["DataBasesToAccess"].ToString().Split(',').ToList());
            }
        }

        private void PrepareDataBaseCommand()
        {
            List<string> temp = new List<string>();
            if (databsesToAccess?.Count > 0)
            {
                whereCommand += "( ";
                temp = databsesToAccess.ToList();
            }
            else if (databsesNotToAccess?.Count > 0)
            {
                whereCommand += "not ( ";
                temp = databsesNotToAccess.ToList();
            }

            for (int i = 0; i < temp.Count; i++)
            {
                if (i != 0)
                {
                    whereCommand += " or ";
                }
                whereCommand += "(name like '%" + temp[i] + "%')";
            }
            whereCommand += " )";
        }

        private void PrepareSchemaCondition(string dbName)
        {
            List<string> temp = new List<string>();
            if (schemasToAccess?.Count > 0)
            {
                whereCommand += "( ";
                temp = schemasToAccess.Where(s => s.Split('.')[0] == dbName).ToList();
            }
            else if (schemasNotToAccess?.Count > 0)
            {
                whereCommand += "not ( ";
                temp = schemasNotToAccess.Where(s => s.Split('.')[0] == dbName).ToList();
            }

            for (int i = 0; i < temp.Count; i++)
            {
                if (i != 0)
                {
                    whereCommand += " or ";
                }
                whereCommand += "(TABLE_SCHEMA like '%" + temp[i] + "%')";
            }
            whereCommand += " )";
        }

        private void PrepareTableCondition(string dbName)
        {
            List<string> temp = new List<string>();
            bool isFirst = true;

            if (!string.IsNullOrEmpty(whereCommand))
            {
                whereCommand += " and ";
            }

            if (tablesToAccess?.Count > 0)
            {
                whereCommand += "( ";
                temp = tablesToAccess.Where(s => s.Split('.')[0] == dbName).ToList();
            }
            else if (tablesNotToAccess?.Count > 0)
            {
                whereCommand += "not ( ";
                temp = tablesNotToAccess.Where(s => s.Split('.')[0] == dbName).ToList();

            }

            for (int i = 0; i < temp.Count; i++)
            {
                if (!isFirst)
                {
                    whereCommand += " or ";
                }
                whereCommand += "(TABLE_SCHEMA like '%" + temp[i].Split('.')[1] + "%' && TABLE_NAME like '%" + temp[i].Split('.')[2] + "%')";
            }
            whereCommand += " )";
        }

        private void PrepareColumnsCondition(string dbName)
        {
            List<string> temp = new List<string>();

            if (!string.IsNullOrEmpty(whereCommand))
            {
                whereCommand += " and ";
            }

            if (columnsToAccess?.Count > 0)
            {
                whereCommand += "( ";
                temp = columnsToAccess.Where(s => s.Split('.')[0] == dbName).ToList();
            }
            else if (columnsNotToAccess?.Count > 0)
            {
                whereCommand += "not ( ";
                temp = columnsNotToAccess.Where(s => s.Split('.')[0] == dbName).ToList();
            }

            for (int i = 0; i < temp.Count; i++)
            {
                if (i != 0)
                {
                    whereCommand += " or ";
                }
                whereCommand += "(TABLE_SCHEMA like '%" + temp[i].Split('.')[1] + "%' && TABLE_NAME like '%" + temp[i].Split('.')[2] + "%' && COLUMN_NAME like '%" + temp[i].Split('.')[3] + "%')";
            }
            whereCommand += " )";
        }

        private void PrepareCommand(string dbName)
        {
            whereCommand = null;
            PrepareSchemaCondition(dbName);
            PrepareTableCondition(dbName);
            PrepareColumnsCondition(dbName);
        }

        public async Task<string> GetDatabaseDefintions()
        {
            using (SqlConnection con = new SqlConnection(strConnect))
            {
                con.Open();

                //TODO: Enahnce perfomrance : try to avoid getting databases information one by one 

                GetAllDataBases(con);

                for (int i = 0; i < databases.Count(); i++)
                {
                    GetAllDatabaseInfomration(i, con);
                }

                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(databases);
                return jsonString.ToString();
            }
        }

        //public async Task<Dictionary<string, double>> GetSelectedData(List<Field> Fields)
        public async Task<List<Parameters>> GetSelectedData(List<Field> Fields)
        {
            //List<string> keys = new List<string>(); IntegrationDataFilter integrationDataFilter = IntegrationDataFilter.First;
            using (SqlConnection con = new SqlConnection(strConnect))
            {
                //Dictionary<string, double> result = new Dictionary<string, double>();
                List < Parameters> result = new List<Parameters>();
                string[] accessInfo;
                con.Open();
                string query;
                double value;
                bool converstionResult;

                for (int i = 0; i < Fields.Count(); i++)
                {
                    accessInfo = Fields[i].FielId.Split('.');


                    if (Fields[i].FildAggregationType.ToLower() == IntegrationDataFilter.Count.ToString().ToLower())
                    {
                        query = @"select Count(" + accessInfo[3] + ") FROM " + accessInfo[0] + "." + accessInfo[1] + "." + accessInfo[2];
                    }
                    else if (Fields[i].FildAggregationType.ToLower() == IntegrationDataFilter.Min.ToString().ToLower())

                    {
                        query = @"select Min(" + accessInfo[3] + ") FROM " + accessInfo[0] + "." + accessInfo[1] + "." + accessInfo[2];
                    }
                    else if (Fields[i].FildAggregationType.ToLower() == IntegrationDataFilter.Max.ToString().ToLower())
                    {
                        query = @"select Max(" + accessInfo[3] + ") FROM " + accessInfo[0] + "." + accessInfo[1] + "." + accessInfo[2];
                    }
                    else if (Fields[i].FildAggregationType.ToLower() == IntegrationDataFilter.Sum.ToString().ToLower())
                    {
                        query = @"select Sum(" + accessInfo[3] + ") FROM " + accessInfo[0] + "." + accessInfo[1] + "." + accessInfo[2];
                    }
                    else if (Fields[i].FildAggregationType.ToLower() == IntegrationDataFilter.Average.ToString().ToLower())
                    {
                        query = @"select avg(" + accessInfo[3] + ") FROM " + accessInfo[0] + "." + accessInfo[1] + "." + accessInfo[2];
                    }
                    else
                    {
                        throw new Exception("UnValid Operation!");
                    }

                    using (SqlCommand com = new SqlCommand(query, con))
                    {
                        var returnValue = com.ExecuteScalar().ToString();
                        converstionResult = double.TryParse(returnValue, out value);
                        if (!converstionResult)
                        {
                            throw new Exception("Error in data");
                        }
                        // result.Add(Fields[i].FielId, Math.Round(value, 2));
                        result.Add(new Parameters
                        {

                            FieldId = Fields[i].FielId,
                            Value = Convert.ToString(Math.Round(value, 2))
                        });
                    }
                }
                return result;
            }
        }

        private void GetAllDataBases(SqlConnection con)
        {
            string query = "select * from sys.databases where database_id > 4";

            whereCommand = null;
            PrepareDataBaseCommand();
            if (!string.IsNullOrEmpty(whereCommand) && (databsesNotToAccess.Count > 0 || databsesToAccess.Count > 0))
            {
                query += " and " + whereCommand;
            }
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                using (IDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        //TODO: do preformance test on MultipleActiveResultSets=True; to allow a new sql command inside another this command
                        databases.Add(new Database
                        {
                            DatabaseName = dr[0].ToString()
                        });
                    }
                }
            }
        }

        private void GetAllDatabaseInfomration(int databaseIndex, SqlConnection con)
        {
            ds = new DataSet();
            string query = @"select TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, DATA_TYPE FROM [" + databases[databaseIndex].DatabaseName + "].Information_schema.columns";

            PrepareCommand(databases[databaseIndex].DatabaseName);
            if (!string.IsNullOrEmpty(whereCommand) &&
                (tablesNotToAccess.Count > 0 ||
                tablesToAccess.Count > 0 ||
                columnsNotToAccess.Count > 0 ||
                columnsToAccess.Count > 0 ||
                schemasNotToAccess.Count > 0 ||
                schemasToAccess.Count > 0))
            {
                query += " where " + whereCommand;
            }

            using (SqlCommand com = new SqlCommand(query, con))
            {
                adapter.SelectCommand = com;
                adapter.Fill(ds);
                PrepareSchemas(databaseIndex);
                adapter.Dispose();
            }
        }

        private void PrepareSchemas(int databaseIndex)
        {
            if (ds?.Tables?.Count <= 0)
            {
                throw new Exception("No Data Available");
            }
            var allSchemas = ds.Tables[0].AsEnumerable();
            var schemaNames = allSchemas.Select(c => c.Field<string>("TABLE_SCHEMA")).Distinct().ToList();

            databases[databaseIndex].Schemas = new List<Schema>();
            for (int i = 0; i < schemaNames.Count(); i++)
            {
                schema = new Schema();
                schema.SchemaName = schemaNames[i];
                schema.SchemaId = databases[databaseIndex].DatabaseName + "." + schema.SchemaName;
                schema.Tables = PrepareTables(databases[databaseIndex].DatabaseName, schema.SchemaName);
                databases[databaseIndex].Schemas.Add(schema);
            }
        }

        private List<Table> PrepareTables(string databaseName, string schemaName)
        {
            var schemaData = ds.Tables[0].AsEnumerable().Where(c => c.Field<string>("TABLE_SCHEMA") == schemaName)
                .Select(c => c.Field<string>("TABLE_NAME")).Distinct().ToList();
            var tables = new List<Table>();
            for (int i = 0; i < schemaData.Count(); i++)
            {
                table = new Table();
                table.TableName = schemaData[i];
                table.TableId = databaseName + "." + schemaName + "." + table.TableName;
                table.Columns = PrepareColumns(databaseName, schemaName, table.TableName);
                tables.Add(table);
            }
            return tables;
        }

        private List<Column> PrepareColumns(string databaseName, string schemaName, string tableName)
        {
            var tableData = ds.Tables[0].AsEnumerable().Where(c => c.Field<string>("TABLE_SCHEMA") == schemaName &&
            c.Field<string>("TABLE_NAME") == tableName).ToList();

            var columns = new List<Column>();
            for (int j = 0; j < tableData.Count(); j++)
            {
                column = new Column();
                column.ColumnName = tableData[j].Field<string>("COLUMN_NAME");
                column.ColumnId = databaseName + "." + schemaName + "." + tableName + "." + column.ColumnName;
                column.ColumnType = tableData[j].Field<string>("DATA_TYPE").ToLower();
                columns.Add(column);
            }
            return columns;
        }
    }
}

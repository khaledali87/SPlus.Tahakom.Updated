using SPlus.Integration.IntegrationKitchen.AccessInterfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SPlus.Integration.Enums;
using SPlus.Model;

namespace SPlus.Integration.IntegrationKitchen.AccessImplementations.DatabaseLevel
{
    public class MySqlAccess : IAccess
    {
        private MySqlConnection connection;
        private string whereCommand = null;
        private string server;
        private string port;
        private string database;
        private string uid;
        private string password;
        private List<Database> databases;
        private Schema schema;
        private Table table;
        private Column column;

        private string orderByColumn;

        private List<string> databsesNotToAccess;
        private List<string> databsesToAccess;

        private List<string> schemasNotToAccess;
        private List<string> schemasToAccess;

        private List<string> tablesNotToAccess;
        private List<string> tablesToAccess;

        private List<string> columnsNotToAccess;
        private List<string> columnsToAccess;

        //Constructor
        public MySqlAccess()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server = "localhost";
            port = "3306";
            database = "testintegration";
            uid = "root";
            password = "Mutasem!1991";
            string connectionString;

            connectionString = "SERVER=" + server + ";" + "Port=" + port + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        public async Task<string> GetDatabaseDefintions()
        {
            GetAllDataBases();
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(databases);
            return jsonString.ToString();
        }

        //public async Task<Dictionary<string, double>> GetSelectedData(List<Field> Fields)
        public async Task<List<Parameters>> GetSelectedData(List<Field> Fields)
        {

            //Dictionary<string, double> result = new Dictionary<string, double>();
            List<Parameters> result = new List<Parameters>();

            if (this.OpenConnection() == true)
            {
                string query;
                MySqlCommand cmd;
                MySqlDataReader dataReader;
                string[] accessInfo;
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
                        query = @"select Min(" + accessInfo[3] + ") FROM " + accessInfo[0] + "." + accessInfo[1] + "." + accessInfo[2];
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

                    cmd = new MySqlCommand(query, connection);
                    var returnValue = cmd.ExecuteScalar().ToString();
                    converstionResult = double.TryParse(returnValue, out value);
                    if (!converstionResult)
                    {
                        throw new Exception("Error in data");
                    }
                    //result.Add(Fields[i].FielId, Math.Round(value, 2));
                    result.Add(new Parameters
                    {

                        FieldId = Fields[i].FielId,
                        Value = Convert.ToString(Math.Round(value, 2))
                    });
                }

                //close Connection
                this.CloseConnection();
            }
            return result;
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        //("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        //("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }

        private void PrepareDataBaseCommand()
        {
            List<string> temp = new List<string>();
            if (databsesToAccess?.Count > 0)
            {
                whereCommand = "( ";
                temp = schemasToAccess.ToList();
            }
            else if (schemasNotToAccess?.Count > 0)
            {
                whereCommand = "not ( ";
                temp = schemasNotToAccess.ToList();
            }

            for (int i = 0; i < databsesNotToAccess.Count; i++)
            {
                if (i != 0)
                {
                    whereCommand += " || ";
                }
                whereCommand += "(schema_name not like '%" + databsesNotToAccess[i] + "%')";
            }
            whereCommand = " )";
        }

        private void PrepareTableCondition(string dbName)
        {
            List<string> temp = new List<string>();
            bool isFirst = true;

            if (tablesToAccess?.Count > 0)
            {
                whereCommand = "( ";
                temp = tablesToAccess.Where(s => s.Split('.')[0] == dbName).ToList();
            }
            else if (tablesNotToAccess?.Count > 0)
            {
                whereCommand = "not ( ";
                temp = tablesNotToAccess.Where(s => s.Split('.')[0] == dbName).ToList();

            }

            for (int i = 0; i < temp.Count; i++)
            {
                if (!isFirst)
                {
                    whereCommand += " || ";
                }
                whereCommand += "(tables_in_" + dbName + " like '%" + temp[i].Split('.')[2] + "%')";
            }
            whereCommand = " )";
        }

        private void PrepareColumnsCondition(string dbName)
        {
            List<string> temp = new List<string>();

            if (!string.IsNullOrEmpty(whereCommand))
            {
                whereCommand += " && ";
            }

            if (columnsToAccess?.Count > 0)
            {
                whereCommand = "( ";
                temp = columnsToAccess.Where(s => s.Split('.')[0] == dbName).ToList();
            }
            else if (columnsNotToAccess?.Count > 0)
            {
                whereCommand = "not ( ";
                temp = columnsNotToAccess.Where(s => s.Split('.')[0] == dbName).ToList();
            }

            for (int i = 0; i < temp.Count; i++)
            {
                if (i != 0)
                {
                    whereCommand += " || ";
                }
                whereCommand += "(Field like '%" + temp[i].Split('.')[3] + "%')";
            }
            whereCommand = " )";
        }

        private void GetAllDataBases()
        {
            string query = "select schema_name from information_schema.schemata where schema_name <> 'information_schema' and schema_name <> 'mysql' and  schema_name <> 'performance_schema' and  schema_name <> 'sakila' and  schema_name <> 'sys' and  schema_name <> 'world';";

            whereCommand = null;
            PrepareDataBaseCommand();
            if (!string.IsNullOrEmpty(whereCommand))
            {
                query += " && " + whereCommand;
            }

            //Open connection
            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                databases = new List<Database>();
                while (dataReader.Read())
                {
                    databases.Add(new Database
                    {
                        DatabaseName = dataReader["schema_name"].ToString()
                    });
                }
                dataReader.Close();
                GetAllTablesInfomration(cmd, dataReader);

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();
            }
        }

        private void GetAllTablesInfomration(MySqlCommand cmd, MySqlDataReader dataReader)
        {
            string query = "";
            for (int i = 0; i < databases.Count(); i++)
            {
                query = "SHOW tables FROM " + databases[i].DatabaseName;

                whereCommand = null;
                PrepareTableCondition(databases[i].DatabaseName);
                if (!string.IsNullOrEmpty(whereCommand))
                {
                    query += " where " + whereCommand;
                }
                query += ";";

                databases[i].Schemas = new List<Schema>();
                databases[i].Schemas.Add(new Schema
                {
                    SchemaId = "Default",
                    SchemaName = "Default",
                    Tables = new List<Table>()
                });

                cmd = new MySqlCommand(query, connection);
                dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    databases[i].Schemas.FirstOrDefault().Tables.Add(new Table
                    {
                        TableId = databases[i].DatabaseName + "." + dataReader[0].ToString(),
                        TableName = dataReader[0].ToString(),
                        Columns = new List<Column>()
                    });
                }

                dataReader.Close();
                GetAllColumns(databases[i].Schemas.FirstOrDefault().Tables, cmd, dataReader);
            }
        }

        private void GetAllColumns(List<Table> tables, MySqlCommand cmd, MySqlDataReader dataReader)
        {
            string query = "";
            for (int i = 0; i < tables.Count(); i++)
            {
                tables[i].Columns = new List<Column>();

                query = "SHOW Columns FROM " + tables[i].TableId;

                whereCommand = null;
                PrepareColumnsCondition(databases[i].DatabaseName);
                if (!string.IsNullOrEmpty(whereCommand))
                {
                    query += " where " + whereCommand;
                }
                query += ";";

                cmd = new MySqlCommand(query, connection);
                dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    tables[i].Columns.Add(new Column
                    {
                        ColumnName = dataReader[0].ToString(),
                        ColumnId = tables[i].TableId + "." + dataReader[0].ToString(),
                        ColumnType = dataReader[1].ToString(),
                    });
                }
                dataReader.Close();
            }
        }
    }
}

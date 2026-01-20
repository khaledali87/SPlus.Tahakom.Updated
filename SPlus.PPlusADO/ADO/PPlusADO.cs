using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.Model.Domain;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using SPlus.Helper;

namespace SPlus.PPlusADO
{
    public class PPlusADO
    {
        #region Strategic Initiative
        public List<StrategicInitiative> GetStrategicInitiatives()
        {
            try
            {
                List<User> users = GetUsers();
                List<StrategicInitiative> strategicInitiatives = new List<StrategicInitiative>();
                StrategicInitiative strategicInitiative;
                using (SqlConnection conn = new SqlConnection(Constants.PPlusDBConnectionString))
                {
                    string selectQuery = "SP_GetPrograms";

                    SqlCommand command = new SqlCommand(selectQuery, conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    conn.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        strategicInitiative = new StrategicInitiative();
                        if (reader["ProjectUID"] != DBNull.Value)
                            strategicInitiative.ProjectUID = Guid.Parse(reader["ProjectUID"].ToString());
                        if (reader["ProjectName"] != DBNull.Value)
                            strategicInitiative.EnglishName = reader["ProjectName"].ToString();
                        if (reader["Name Arabic"] != DBNull.Value)
                            strategicInitiative.ArabicName = reader["Name Arabic"].ToString();
                        if (reader["Description"] != DBNull.Value)
                            strategicInitiative.ArabicDescription = reader["Description"].ToString();
                        if (reader["EnglishDescription"] != DBNull.Value)
                            strategicInitiative.EnglishDescription = reader["EnglishDescription"].ToString();
                        if (reader["StartDate"] != DBNull.Value)
                            strategicInitiative.StartDate = DateTime.Parse(reader["StartDate"].ToString()).Date;
                        if (reader["FinishDate"] != DBNull.Value)
                            strategicInitiative.EndDate = DateTime.Parse(reader["FinishDate"].ToString()).Date;
                        if (reader["Status"] != DBNull.Value)
                            strategicInitiative.Status = reader["Status"].ToString();
                        if (reader["CaluclatedActualProgress"] != DBNull.Value)
                            strategicInitiative.Progress = decimal.Parse(reader["CaluclatedActualProgress"].ToString());
                        if (reader["ProjectOwnerName"] != DBNull.Value)
                            strategicInitiative.ManagerModel = users.Where(a => a.UserName.ToLower() == reader["ProjectOwnerName"].ToString().ToLower()).FirstOrDefault();

                        strategicInitiatives.Add(strategicInitiative);

                    }
                    conn.Close();

                    return strategicInitiatives;
                }
            }
            catch (System.Exception ex)
            {

                throw;
            }
        }

        #endregion



        public List<Project> GetProjects()
        {
            List<User> users = GetUsers();
            List<Project> projects = new List<Project>();
            Project project;
            using (SqlConnection conn = new SqlConnection(Constants.PPlusDBConnectionString))
            {
                string selectQuery = "SP_GetProjectsBasic";

                SqlCommand command = new SqlCommand(selectQuery, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    project = new Project();
                    if (reader["ProjectUID"] != DBNull.Value)
                        project.ProjectUID = Guid.Parse(reader["ProjectUID"].ToString());
                    if (reader["ProjectName"] != DBNull.Value)
                        project.EnglishName = reader["ProjectName"].ToString();
                    if (reader["Name Arabic"] != DBNull.Value)
                        project.ArabicName = reader["Name Arabic"].ToString();
                    if (reader["Status"] != DBNull.Value)
                        project.Status = reader["Status"].ToString();
                    if (reader["StartDate"] != DBNull.Value)
                        project.StartDate = DateTime.Parse(reader["StartDate"].ToString()).Date;
                    if (reader["FinishDate"] != DBNull.Value)
                        project.EndDate = DateTime.Parse(reader["FinishDate"].ToString()).Date;
                    if (reader["CaluclatedActualProgress"] != DBNull.Value)
                        project.Progress = decimal.Parse(reader["CaluclatedActualProgress"].ToString());
                    if (reader["ProjectOwnerName"] != DBNull.Value)
                        project.Manager = users.Where(a => a.UserName.ToLower() == reader["ProjectOwnerName"].ToString()).FirstOrDefault();
                    if (reader["DepartmentID"] != DBNull.Value && !string.IsNullOrEmpty(reader["DepartmentID"].ToString()))
                        project.DepartmentID =int.Parse(reader["DepartmentID"].ToString());
                    if (reader["Is Operational"] != DBNull.Value)
                        project.IsOperational = bool.Parse(reader["Is Operational"].ToString());

                    projects.Add(project);
                }
                conn.Close();


                return projects;
            }
        }

        public List<User> GetUsers()
        {
            List<User> users = new List<User>();
            User user;
            using (SqlConnection conn = new SqlConnection(Constants.PPlusDBConnectionString))
            {
                string selectQuery = "SP_GetAllResources";

                SqlCommand command = new SqlCommand(selectQuery, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    user = new User();
                    user.UserName = reader["Account"].ToString();
                    user.DisplayName = reader["Name"].ToString();
                    user.Email = reader["Email"].ToString();
                    user.PhoneNumber = reader["Phone"].ToString();
                    //user.UserProfilePicture = "";   
                    users.Add(user);
                }
                conn.Close();
                return users;
            }
        }

        public List<User> GetProjectManagers()
        {
            List<User> users = new List<User>();
            User user;
            using (SqlConnection conn = new SqlConnection(Constants.PPlusDBConnectionString))
            {
                string selectQuery = "SP_GetGroupUsers";

                SqlCommand command = new SqlCommand(selectQuery, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add("@groupName", SqlDbType.NVarChar).Value = "ProjectManagers";

                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    user = new User();
                    user.UserName = reader["Account"].ToString();
                    user.DisplayName = reader["Name"].ToString();
                    user.Email = reader["Email"].ToString();
                    user.PhoneNumber = reader["Phone"].ToString();
                    //user.UserProfilePicture = "";   
                    users.Add(user);
                }
                conn.Close();
                return users;
            }
        }
    }
}

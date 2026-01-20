using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.Model;
using SPlus.BLL;
using SPlus.Model.Domain;
using SPlus.DataAccess;
using StructureMap;
using System.Data.Entity;
using static System.Net.Mime.MediaTypeNames;


namespace SPlus.BLL
{
    public static class UserMapping
    {
        static Container _Container = IOC.InitializeContainer();
        static private readonly IUnitOfWorkFactory _factory = _Container.GetInstance<IUnitOfWorkFactory>();
      
        // For one object
        public static User MapUserWithGroups(this string UserName)
        {
            using (var dataAccess = _factory.Create())
            {
                User User = dataAccess.User.Query()
                    .Include(a => a.UsersGroups.Select(s => s.Group))
                    .Where(s => s.UserName.ToLower() == UserName.ToLower()).SingleOrDefault();
                return User;
            }            
        }

        public static User MapUser(this string UserName)
        {
            using (var dataAccess = _factory.Create())
            {
                User User = dataAccess.User.Query()
                    .Where(s => s.UserName.ToLower() == UserName.ToLower()).SingleOrDefault();
                return User;
            }
        }


        public static IEnumerable<object> MapUsers(this IEnumerable<object> objs)
        {
            using (var dataAccess = _factory.Create())
            {
                List<User> Users = dataAccess.User.Query().ToList();


                foreach (object obj in objs)
                {
                    foreach (var prop in obj.GetType().GetProperties().Where(a=>a.PropertyType == typeof(User)))
                    {
                        User x = (User)obj.GetType().GetProperty(prop.Name).GetValue(obj);
                            if (x != null && ((User)(x)).UserName != null)
                                obj.GetType().GetProperty(prop.Name).SetValue(obj, Users.Where(a => a.UserName.ToLower() == x.UserName.ToLower()).FirstOrDefault());
                        
                    }
                }
                
            }
            return objs;           
        }

        public static object MapUser(this object obj)
        {
            using (var dataAccess = _factory.Create())
            {
                List<User> Users = dataAccess.User.Query().ToList();
                foreach (var prop in obj.GetType().GetProperties().Where(a => a.PropertyType == typeof(User)))
                {
                    User x = (User)obj.GetType().GetProperty(prop.Name).GetValue(obj);
                    if (x != null && ((User)(x)).UserName != null)
                        obj.GetType().GetProperty(prop.Name).SetValue(obj, Users.Where(a => a.UserName.ToLower() == x.UserName.ToLower()).FirstOrDefault());
                }               
            }
            return obj;
        }
    }
}

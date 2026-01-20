using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using SPlus.Model.Domain;
using System.Data.Entity;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace SPlus.DataAccess
{
    public class UserDataAccess : Repository<User>, IUserRepository
    {

        public UserDataAccess(EntityModel context)
            : base(context)
        {
        }
       
    }
}

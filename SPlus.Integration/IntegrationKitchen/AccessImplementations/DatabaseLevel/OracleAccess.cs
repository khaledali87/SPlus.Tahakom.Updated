using SPlus.Integration.IntegrationKitchen.AccessInterfaces;
using SPlus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SPlus.Integration.Enums;

namespace SPlus.Integration.IntegrationKitchen.AccessImplementations.DatabaseLevel
{
    public class OracleAccess : IAccess
    {
        //waiting access to database 

        public async Task<string> GetDatabaseDefintions()
        {
            throw new NotImplementedException();
        }

        //public async Task<Dictionary<string, double>> GetSelectedData(List<Field> Fields)
        public async Task<List<Parameters>> GetSelectedData(List<Field> Fields)
        { 
            throw new Exception();
        }
    }
}

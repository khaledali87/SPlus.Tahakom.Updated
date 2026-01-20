using SPlus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SPlus.Integration.Enums;

namespace SPlus.Integration.IntegrationKitchen.AccessInterfaces
{
    public interface IAccess
    { 
        Task<string> GetDatabaseDefintions();

        //Task<Dictionary<string, double>> GetSelectedData(List<Field> Fields); 
        Task<List<Parameters>> GetSelectedData(List<Field> Fields);
    }
}

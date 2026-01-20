using SPlus.Integration.IntegrationKitchen.AccessInterfaces;
using SPlus.Integration.IntegrationKitchen.Factories;
using SPlus.Integration.IntegrationKitchen.Factories.DatabaseLevel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPlus.Integration
{
    public class IntegrationAccess
    {
        private readonly Dictionary<Enums.IntegrationAccessType, IntegrationAccessFactory> _factories;
        public IAccess ExecuteCreation(Enums.IntegrationAccessType access) => _factories[access].Create();
        public static IntegrationAccess InitializeFactories() => new IntegrationAccess();

        private IntegrationAccess()
        {
            _factories = new Dictionary<Enums.IntegrationAccessType, IntegrationAccessFactory>()
            {
                { Enums.IntegrationAccessType.SQL, new SQLAccessFactory() },
                { Enums.IntegrationAccessType.Oracle, new OracleAccessFactory() },
                { Enums.IntegrationAccessType.MySql, new MySqlAccessFactory() },
                //{ Enums.DbAccessTypes.Oracle, new WarmingFactory() } //TODO: implement oracle and mysql
            };


            //TODO: move more dynamic
            //foreach (Enums.DbAccessTypes access in Enum.GetValues(typeof(Enums.DbAccessTypes)))
            //    {
            //        var factory = (DbAccessFactory)Activator.
            //            CreateInstance(Type.GetType("FactoryMethod." + Enum.GetName(typeof(Enums.DbAccessTypes), access) + "Factory"));
            //        _factories.Add(access, factory);
            //    }
        }
    }
}

using SPlus.Integration.IntegrationKitchen.AccessImplementations;
using SPlus.Integration.IntegrationKitchen.AccessImplementations.DatabaseLevel;
using SPlus.Integration.IntegrationKitchen.AccessInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPlus.Integration.IntegrationKitchen.Factories.DatabaseLevel
{
    public class OracleAccessFactory : IntegrationAccessFactory
    {
        public override IAccess Create() => new OracleAccess();
    }
}

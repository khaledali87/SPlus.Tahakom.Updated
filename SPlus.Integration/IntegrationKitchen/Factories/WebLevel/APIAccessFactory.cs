using SPlus.Integration.IntegrationKitchen.AccessImplementations;
using SPlus.Integration.IntegrationKitchen.AccessImplementations.DatabaseLevel;
using SPlus.Integration.IntegrationKitchen.AccessImplementations.WebLevel;
using SPlus.Integration.IntegrationKitchen.AccessInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPlus.Integration.IntegrationKitchen.Factories
{
    public class APIAccessFactory : IntegrationAccessFactory
    {
        private string baseUrl = "";
        public override IAccess Create() => new APIAccess(baseUrl);

        public APIAccessFactory()
        {
            baseUrl = "read from config";
        }
    }
}

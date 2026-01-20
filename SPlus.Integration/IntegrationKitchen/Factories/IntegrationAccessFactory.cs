using SPlus.Integration.IntegrationKitchen.AccessInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPlus.Integration.IntegrationKitchen.Factories
{
    public abstract class IntegrationAccessFactory
    {
        public abstract IAccess Create();
    }
}

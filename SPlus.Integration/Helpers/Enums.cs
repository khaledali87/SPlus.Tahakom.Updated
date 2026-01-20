using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPlus.Integration
{
    public class Enums
    {
        public enum IntegrationAccessType
        {
            SQL = 1,
            Oracle = 2,
            MySql = 3,
            WCF = 4,
            REST = 5
        }

        public enum IntegrationDataFilter
        {

            Sum = 1,
            Average = 2,
            Max = 3,
            Min = 4,
            Count = 5,
            Oldest = 6,
            Newest = 7
        }
    }
}

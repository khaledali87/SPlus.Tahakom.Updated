using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
   public class StrategyPerformanceThemeDTO:BaseDTO
    {
        public List<KPIDetailsDTO> KPIs{ get; set; }
        public int UpdatedCount { get; set; }
        public int RequireUpdate { get; set; }
    }
}

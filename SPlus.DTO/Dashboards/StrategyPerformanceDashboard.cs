using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
   public class StrategyPerformanceDashboardDTO
    {
        public List<DistributionDataByStatus> KPIByStatuses { get; set; }
        public int UpdatedCount { get; set; }
        public int RequireUpdate { get; set; }

        public List<StrategyPerformanceThemeDTO> Themes { get; set; }
        public List<StrategyPerformanceSODTO> StrategicObjectives { get; set; }
    }
}

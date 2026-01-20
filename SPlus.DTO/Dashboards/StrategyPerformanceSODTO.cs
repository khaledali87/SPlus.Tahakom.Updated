using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class StrategyPerformanceSODTO:BaseDTO
    {
        public decimal Performance { get; set; }
        public string Status { get; set; }
        public decimal Weight { get; set; }
        public List<DistributionDataByStatus> KPIByStatuses { get; set; }
        public List<DistributionDataByStatus> DivisionalObjectives { get; set; }
    }
}

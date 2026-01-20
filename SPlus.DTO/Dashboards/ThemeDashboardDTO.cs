using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SPlus.DTO
{
    public class ThemeDashboardDTO : BaseDTO
    {
        public decimal AvragePerformance { get; set; }
        public decimal VariancePerformance { get; set; }
        public string AvrageStatus { get; set; }
        public string VarianceStatus { get; set; }
        public decimal Performance { get; set; }
        public string Status { get; set; }
        public decimal  Weight { get; set; }
        public List<StrategyPerformanceSODTO> StrategicObjectives { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class OperationalOrgStructureDTO:BaseDTO
    {
        public List<KPIDetailsDTO> KPIs { get; set; }
        public bool IsCorporate { get; set; }
        public bool IsSector { get; set; }

        public bool CanSeeOperationalPerformance { get; set; }
        public int OperationalPerformanceID { get; set; } 
        public decimal Performance { get; set; }
        public decimal PreviousPerformance { get; set; }
        public decimal AvragePerformance { get; set; }
        public decimal VariancePerformance { get; set; }

        public string Status { get; set; }
        public string AvrageStatus { get; set; }
        public string VarianceStatus { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
   public class OperationalDashboardDTO
    {
        public decimal Performance { get; set; }
        public decimal PreviousPerformance { get; set; }
        public decimal AvragePerformance { get; set; }
        public decimal VariancePerformance { get; set; }

        public string Status { get; set; }
        public string AvrageStatus { get; set; }
        public string VarianceStatus { get; set; }
        public List<DistributionDataByStatus> KPIByStatuses { get; set; }

        public List<OperationalOrgStructureDTO> OperationalOrgStructure { get; set; }

        public List<DivisionalObjectiveDTO> DivisionalObjective { get; set; }

        

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class DivisionalObjectiveDTO : BaseDTO
    {
        public decimal Performance { get; set; }
        public string Status { get; set; }
        public List<DistributionDataByStatus> KPIByStatuses { get; set; }
        public OrgStructureDTO OrgStructure { get; set; }
        public int UpdatedCount { get; set; }
        public int RequireUpdate { get; set; }
    }
}

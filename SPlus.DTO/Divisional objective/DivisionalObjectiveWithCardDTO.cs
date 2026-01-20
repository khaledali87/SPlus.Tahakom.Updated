using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
   public class DivisionalObjectiveWithCardDTO : BaseDTO
    {
        public StrategicObjectiveDTO StrategicObjective { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public decimal Weight { get; set; }
        public decimal Performance { get; set; }
        public List<KPICardDTO> KPIs { get; set; }
        public ChartDTO ChartData { get; set; }
        public OrgStructureDTO OrgStructure { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class OperationalDetailsDTO : BaseDTO
    {
        public OrgStructureDTO Parent { get; set; }
        public string Status { get; set; }
        public ChartDTO KPIChart { get; set; }
        public ChartDTO InititiativeChart { get; set; }
        public ChartDTO ProjectChart { get; set; }
        public List<KPICardDTO> KPIs { get; set; }
        public List<OperationalLevelDTO> Initiatives { get; set; }
        public List<OperationalLevelDTO> Projects { get; set; }
    }
}

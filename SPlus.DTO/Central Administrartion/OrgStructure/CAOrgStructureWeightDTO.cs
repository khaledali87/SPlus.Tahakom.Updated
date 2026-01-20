using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class CAOrgStructureWeightDTO : BaseDTO
    {
        public decimal Weight { get; set; }
        public int? ParentID { get; set; }
        public List<CAOrgStructureWeightDTO> Childrens { get; set; }

        public List<CAKPIWeightDTO> KPIs { get; set; }
    }
}

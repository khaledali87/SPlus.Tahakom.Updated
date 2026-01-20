using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class OrgStructureWeightDTO : OrgStructureDTO
    {
        public int LevelID { get; set; }
        public List<CAKPIWeightDTO> KPIs { get; set; }
    }
}

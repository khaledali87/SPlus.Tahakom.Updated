using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class KPIByDivisionDTO : BaseDTO
    {
        public int KPICount { get; set; }
        public List<KPIByStatusDTO> KPIsByStatus { get; set; }
        public int RequireUpdate { get; set; }
        public int Updated { get; set; }

    }
}

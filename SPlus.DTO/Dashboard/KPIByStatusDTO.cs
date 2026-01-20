using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class KPIByStatusDTO
    {
        public string Status { get; set; }
        public int KPICount { get; set; }
        public List<DashboardKPIDetailsDTO> KPIs { get; set; }

    } 
}

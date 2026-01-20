using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class KPIByUpdateStatusDTO
    {
        public bool RequireUpdate { get; set; }
        public int KPICount { get; set; }
        public decimal Percentage { get; set; }
        public DateTime OldestDueDate { get; set; }
        public List<DashboardKPIDetailsDTO> KPIs { get; set; }
    }
}

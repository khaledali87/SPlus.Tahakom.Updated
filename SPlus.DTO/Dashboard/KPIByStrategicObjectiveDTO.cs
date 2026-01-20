using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class KPIByStrategicObjectiveDTO : BaseDTO
    {
        public List<DashboardKPIDetailsDTO> KPIs { get; set; }
        public decimal Performance { get; set; }
        public int KPICount { get; set; }
        public string Status { get; set; }

    }
}

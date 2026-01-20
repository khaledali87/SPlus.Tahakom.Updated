using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class GeneralDashboardDTO
    {
        public int KPICount { get; set; }
        public List<KPIByStatusDTO> KPIsByStatus { get; set; }
        public List<KPIByUpdateStatusDTO> KPIsByUpdateStatus { get; set; }
        public List<KPIByDivisionDTO> KPIsByDivision { get; set; }
        public List<KPIByPerspectiveDTO> KPIsByPerspective { get; set; }
        public List<StrategicObjectiveByPerspectiveDTO> StrategicObjectivesByPerspective { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class StrategicObjectiveWithCardDTO : BaseDTO
    {
        public ThemeStrategicObjectiveDetailsDTO Theme { get; set; }

        public string Code { get; set; }
        public string Status { get; set; }
        public decimal Weight { get; set; }
        public List<KPICardDTO> KPIs { get; set; }
        public List<DivisionalObjectiveWithLEDDTO> DivisionalObjectives { get; set; }

        public ChartDTO KPIChart { get; set; }
        public ChartDTO DivisionalObjectiveChart { get; set; }
    }
}
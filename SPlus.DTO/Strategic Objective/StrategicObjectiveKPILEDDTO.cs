using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class StrategicObjectiveWithLEDDTO : BaseDTO
    {
        public string Status { get; set; }
        public decimal Performance { get; set; }
        public decimal Weight { get; set; }
        public int Order { get; set; }

        public string Code { get; set; }
        public List<KPILEDDTO> KPIs { get; set; }        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class KPIAffectDTO
    {
        public int ID { get; set; }
        public int KPIID { get; set; }
        public int Effecting { get; set; }
        public int Affected { get; set; }
    }
}

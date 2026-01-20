using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class SubStrategicObjectiveAffect
    {
        public int ID { get; set; }
        public int SSOID { get; set; }
        public int Effecting { get; set; }
        public int Affected { get; set; }
    }
}

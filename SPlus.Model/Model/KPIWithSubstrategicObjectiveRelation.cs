using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class KPIWithSubstrategicObjectiveRelation
    {
        public int ID { get; set; }
        public int KPIID { get; set; }
        public int SubStrategicObjectiveID { get; set; }
    }
}

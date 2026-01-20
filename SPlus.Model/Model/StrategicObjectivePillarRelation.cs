using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
   public class StrategicObjectivePillarRelation
    {
        public int ID { get; set; }
        public int StrategicObjectiveID { get; set; }
        public int PillarID { get; set; }
    }
}

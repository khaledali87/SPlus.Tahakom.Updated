using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Benefit
    {
        private Lookup _StrategicObjective = new Lookup();

        private General _GeneralData = new General();

        public General GeneralData
        {
            get
            {
                return _GeneralData;
            }
            set
            {
                _GeneralData = value;
            }
        }

      
        public int Progress
        {
            set;
            get;
        }

        public decimal Weight
        {
            set;
            get;
        }

        public string StrategicObjectiveId
        {
            set;
            get;
        }

        public List<Lookup> StrategicObjective { get; set; }
       

        //public string StrategicObjectiveId { get; set; }
    }
}

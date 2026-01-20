using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class StrategicObjectivePerformance
    {
        private StrategicObjective _StrategicObjective = new StrategicObjective();
        private List<KPI> _KPIs = new List<KPI>();
        public StrategicObjective StrategicObjective
        {
            get
            {
                return _StrategicObjective;
            }
            set
            {
                _StrategicObjective = value;
            }
        }

        public decimal AT
        {
            set;
            get;
        }
        public decimal ATP
        {
            set;
            get;
        }
        public decimal NAT
        {
            set;
            get;
        }
        public decimal NON
        {
            set;
            get;
        }
        public int TotalKPIs
        {
            set;
            get;
        }

        public List<KPI> KPIs
        {
            get
            {
                return _KPIs;
            }
            set
            {
                _KPIs = value;
            }
        }
    }
}

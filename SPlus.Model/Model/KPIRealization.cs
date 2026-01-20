using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class KPIRealization
    {
        private General _GeneralData = new General();
        private StrategicObjective _StrategicObjective = new StrategicObjective();
        private KPI _KPI = new KPI();

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

        public KPI KPI
        {
            get
            {
                return _KPI;
            }
            set
            {
                _KPI = value;
            }
        }
    }
}

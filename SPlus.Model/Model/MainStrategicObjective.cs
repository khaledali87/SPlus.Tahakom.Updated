using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class MainStrategicObjective
    {
        private List<Lookup> _Theme = new List<Lookup>();       
        private General _GeneralData = new General();
        private List<StrategicObjective> _StrategicObjectives = new List<StrategicObjective>();

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

        public List<Lookup> Theme
        {
            get
            {
                return _Theme;
            }
            set
            {
                _Theme = value;
            }
        }

        public List<StrategicObjective> StrategicObjectives
        {
            get
            {
                return _StrategicObjectives;
            }
            set
            {
                _StrategicObjectives = value;
            }
        }
    }
}

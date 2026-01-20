
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class StrategicObjectiveViewModel
    {
        private List<Lookup> _Theme = new List<Lookup>();
        private List<Lookup> _Pillar = new List<Lookup>();
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

        public int BenefitProgress
        {
            set;
            get;
        }
        public int KPIProgress
        {
            set;
            get;
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
        public List<Lookup> Pillar
        {
            get
            {
                return _Pillar;
            }
            set
            {
                _Pillar = value;
            }
        }
    }
}

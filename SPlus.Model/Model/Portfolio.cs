using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Portfolio
    {
        private Lookup _StrategicObjective = new Lookup();
        private List<Lookup> _StrategicObjectives = new List<Lookup>();

        public int ID
        {
            set;
            get;
        }
        public string EnglishName
        {
            set;
            get;
        }

        public string ArabicName
        {
            set;
            get;
        }

        public int Progress
        {
            set;
            get;
        }

        public string Status
        {
            set;
            get;
        }
        public List<Lookup> StrategicObjectives
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


        public string ProjectUID
        {
            set;
            get;
        }
    }
}

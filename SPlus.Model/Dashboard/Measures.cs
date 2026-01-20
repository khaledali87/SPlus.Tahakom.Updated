using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class AnalyticMeasures
    {
        private List<Measures> _Measures = new List<Measures>();
        public List<Measures> Measures
        {
            get
            {
                return _Measures;
            }
            set
            {
                _Measures = value;
            }
        }

        public string DueDate
        {
            set;
            get;
        }
        public string Frequency
        {
            set;
            get;
        }
    }
}

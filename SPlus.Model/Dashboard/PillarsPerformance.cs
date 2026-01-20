using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class PillarsPerformance
    {
        private Theme _Pillar = new Theme();

        public Theme Pillar
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
    }
}

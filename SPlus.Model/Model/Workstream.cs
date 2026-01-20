using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Workstream
    {
        private List<Pillar> _Pillar = new List<Pillar>();
        public int ID 
        { 
            get; 
            set; 
        }
        public string NameEnglish
        { 
            get;
            set;
        }
        public string NameArabic
        {
            get;
            set; 
        }
        public List<Pillar> Pillars
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

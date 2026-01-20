using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Initiative
    {
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
        public string Code
        {
            get;
            set;
        }
        public string DescriptionEnglish
        {
            get;
            set;
        }
        public string DescriptionArabic
        {
            get;
            set;
        }
        public string Progress
        {
            get;
            set;
        }
        public string Budget
        {
            get;
            set;
        }
        public string Expedenture
        {
            get;
            set;
        }
        public string ExpedenturePercentage
        {
            get;
            set;
        }
    }
}

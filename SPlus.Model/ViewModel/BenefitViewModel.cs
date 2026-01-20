
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class BenefitViewModel
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
    }
}

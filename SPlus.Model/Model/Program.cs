using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Program
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
    }
}

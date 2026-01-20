using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class DoAction
    {
        public int RequestID { get; set; }
        public int KPIID { get; set; }
        public int MeasureID { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
    }

}

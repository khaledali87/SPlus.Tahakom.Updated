using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class WFChangeRequest
    {
        public int ID { get; set; }
        public int RequestID { get; set; }
        public int MeasureID { get; set; }
        public int KPIID { get; set; }
        public decimal NewTarget { get; set; }
        public decimal OldTarget { get; set; }
    }
}

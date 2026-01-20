using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class WFFormUpdateKPIDTO
    {
        public int ID { get; set; }
        public int MeasureID { get; set; }
        public decimal Value { get; set; }
        public decimal OldValue { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Target { get; set; }
    }
   
}

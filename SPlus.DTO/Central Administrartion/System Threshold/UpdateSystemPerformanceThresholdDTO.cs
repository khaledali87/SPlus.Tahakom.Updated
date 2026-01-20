using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class UpdateSystemPerformanceThresholdDTO
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public string MaxOperator { get; set; }
        public string MinOperator { get; set; }
    }
}

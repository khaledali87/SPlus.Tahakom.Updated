using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class KPIPerformanceThreshold
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string NameEnglish { get; set; }
        public string NameArabic { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public string MinOperator { get; set; }
        public string MaxOperator { get; set; }
        public string Color { get; set; }
    }
}

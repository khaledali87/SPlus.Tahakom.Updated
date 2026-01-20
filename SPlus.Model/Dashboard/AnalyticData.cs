using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    
    public class Datum
    {
        public string date { get; set; }
        public double lowerValue { get; set; }
        public double upperValue { get; set; }
        public double value { get; set; }
    }

    public class AnalyticData
    {
        public List<Datum> data { get; set; }
        public string error { get; set; }
    }
}

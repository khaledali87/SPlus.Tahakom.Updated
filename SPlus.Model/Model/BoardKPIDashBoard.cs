using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class HistoricalPerformanceTimeline
    {       
        // Current Year Months (the 12 Months always)
        public int Month { get; set; }
        // Current Year (Can change into filter Year)
        public int Year { get; set; }
        // KPI Goal
        public string Type { get; set; }
        // To be Clarified
        public decimal Performance { get; set; }
        // Calculated Using  Configuration (The Configuration is not implemented yet)
        public string Status { get; set; }
        // Calculated Using  Configuration (The Configuration is not implemented yet)
        public string Color { get; set; }

        public decimal OutOfTarget { get; set; }
    }
  
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{

    public class StrategicObjective
    {
        public int ID
        {
            get;
            set;
        }
        public decimal Performance
        {
            get;
            set;
        }
        public int PillarID
        {
            get;
            set;
        }
        public decimal BenefitsPerformance
        {
            get;
            set;
        }
    }
}

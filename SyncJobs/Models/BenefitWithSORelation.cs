using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    public class BenefitWithSORelation
    {
        public string StrategicObjectiveId { get; set; }
        public string BenefitId { get; set; }
        public decimal Weight { get; set; }
    }
}

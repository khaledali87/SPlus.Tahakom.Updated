using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
   public class KPIReleationKPIDTO:BaseDTO
    {
        public string Status { get; set; }
        public int KPITypeID { get; set; }

        public decimal OutOfTarget { get; set; }
        public decimal AccumulutiveOutOfTarget { get; set; }
        public decimal Value { get; set; }
        public decimal Target { get; set; }
        public decimal? MaxTarget { get; set; }
        public string Direction { get; set; }
        public bool RequireUpdate { get; set; }
        public string Frequency { get; set; }
        public decimal Baseline { get; set; }
        public string EnglishUnitDetails { get; set; }
        public string UnitOfMeasure { get; set; }
        public string ArabicUnitDetails { get; set; }
        public CAPerspectiveDTO Perspective { get; set; }
        public CAStrategicObjectiveDTO StrategicObjective { get; set; }
        public CADivisionalObjectiveDTO DivisionalObjective { get; set; }
        public OrgStructureDTO OrgStructure { get; set; }
        public List<KPIAffectDTO> KPIAffect { get; set; }
    }
}

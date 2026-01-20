using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
   public class CADivisionalObjectiveDTO : BaseDTO
    {
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
        public int Order { get; set; }
        public string Code { get; set; }
        public decimal Weight { get; set; }


        public decimal Performance { get; set; }

        public string Status { get; set; }
        public CAStrategicObjectiveDTO StrategicObjective { get; set; }
        public OrgStructureDTO OrgStructure { get; set; }
    }
}

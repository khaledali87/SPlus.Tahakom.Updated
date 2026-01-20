using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class CAStrategicObjectiveDivisionalWeightDTO : BaseDTO 
    {
        public List<CADivisionalObjectiveWeightDTO> DivisionalObjectives { get; set; }
    }
}

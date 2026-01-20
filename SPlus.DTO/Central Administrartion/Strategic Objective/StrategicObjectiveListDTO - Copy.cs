using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class StrategicObjectiveChildDTO : BaseDTO 
    {
        public List<CADivisionalObjectiveDTO> DivisionalObjectives { get; set; }
    }
}

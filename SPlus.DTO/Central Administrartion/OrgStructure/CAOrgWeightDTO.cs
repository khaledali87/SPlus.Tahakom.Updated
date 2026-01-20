using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class CAOrgWeightDTO : BaseDTO
    {
        public List<CADivisionalObjectiveWeightDTO> DivisionalObjective { get; set; }
    }
}

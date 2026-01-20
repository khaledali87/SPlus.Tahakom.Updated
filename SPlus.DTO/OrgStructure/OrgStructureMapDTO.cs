using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
   public class OrgStructureMapDTO : BaseDTO
    {
        public List<DivisionalObjectiveWithLEDDTO> DivisionalObjectives { get; set; }
        public string Abbreviation { get; set; }

    }
}

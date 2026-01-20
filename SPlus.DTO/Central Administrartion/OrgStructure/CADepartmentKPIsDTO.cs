using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class CADepartmentKPIsDTO : BaseDTO
    {
        public List<CAKPIListingDTO> KPIs { get; set; }
        public string Abbreviation { get; set; }

    }
}

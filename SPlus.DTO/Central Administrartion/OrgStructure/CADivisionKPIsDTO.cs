using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class CADivisionKPIsDTO : BaseDTO
    {
        public List<CADepartmentKPIsDTO> Departments { get; set; }
    }
}

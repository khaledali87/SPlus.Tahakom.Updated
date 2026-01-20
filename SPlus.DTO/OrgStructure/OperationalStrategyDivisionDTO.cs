using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class OperationalStrategyDivisionDTO : BaseDTO
    {
        public List<OperationalStrategyDepartmentDTO> Departments { get; set; }
        public decimal Performance { get; set; }
        public string Status { get; set; }
    }
}

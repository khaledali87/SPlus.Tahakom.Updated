using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class DepartmentOrgStructureCardDTO : BaseDTO
    {
        public decimal Performance { get; set; }
        public decimal AvragePerformance { get; set; }
        public decimal Variance { get; set; }

        public string Status { get; set; }
        public string AvrageStatus { get; set; }
        public string VarianceStatus { get; set; }
        public List<KPILEDDTO> KPIs { get; set; } = new List<KPILEDDTO>();
    }
}

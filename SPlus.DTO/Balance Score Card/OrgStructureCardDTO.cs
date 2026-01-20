using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class OrgStructureCardDTO:BaseDTO
    {
        public decimal Performance { get; set; }
        public decimal PreviousPerformance { get; set; }
        public decimal AvragePerformance { get; set; }
        public decimal VariancePerformance { get; set; }

        public string Status { get; set; }
        public string AvrageStatus { get; set; }
        public string VarianceStatus { get; set; }

        public int KPIsCount { get; set; }
        public int DepartmentsCount { get; set; }
        public bool IsCorporate { get; set; }

        public List<KPILEDDTO> KPIs { get; set; } = new List<KPILEDDTO>();

        public List<DepartmentOrgStructureCardDTO> Departments { get; set; } = new List<DepartmentOrgStructureCardDTO>();
    }
}

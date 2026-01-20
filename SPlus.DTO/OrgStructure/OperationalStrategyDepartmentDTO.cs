using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class OperationalStrategyDepartmentDTO : BaseDTO
    {
        public int InitiativesCount { get; set; }
        public int ProjectsCount { get; set; }
        public int FilesCount { get; set; }
        public List<KPILEDDTO> KPIs { get; set; }
        public string Status { get; set; }
    }
}

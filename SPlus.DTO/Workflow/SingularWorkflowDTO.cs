using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class SingularWorkflowDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public SingularKPITypeDTO KPIType { get; set; }
    }
}

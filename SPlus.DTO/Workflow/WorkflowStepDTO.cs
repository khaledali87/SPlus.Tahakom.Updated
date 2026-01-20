using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class WorkflowStepDTO : BaseDTO
    {
        public int Approver { get; set; }
        public int WorkflowID { get; set; }
        public bool isGroup { get; set; }
        public int Order { get; set; }

    }
}

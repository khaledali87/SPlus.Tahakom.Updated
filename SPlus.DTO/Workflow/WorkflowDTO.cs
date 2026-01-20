using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class WorkflowDTO
    {
        public int ID { get; set; }
        public int KPITypeID { get; set; }
        public int BaseWorkflowID { get; set; }
        public int WorkflowID { get; set; }
        public string Name { get; set; }
        public List<WorkflowStepDTO> WorkflowSteps { get; set; }
    }
}
 
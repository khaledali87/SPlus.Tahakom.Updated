using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class RequestStepDTO : BaseDTO
    {
        public int RequestID { get; set; }
        public int Order { get; set; }
        public string Approver { get; set; }
        public object ActionBy { get; set; }
        public int WorkflowStepID { get; set; }
        public UserDTO ActionByModel { get; set; }
        public string Comments { get; set; }
        public bool IsGroup { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool CanApprove { get; set; }
        public string QualityType { get; set; }
    }
}

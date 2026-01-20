using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class RequestDetailsDTO 
    {
        public int ID { get; set; }
        public int WorkflowID { get; set; }
        public string RequestedBy { get; set; }
        public string CreatedBy { get; set; }
        public int QualityType { get; set; }
        public int Status { get; set; }
        public bool CanApprove { get; set; }
        public virtual object Form { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public List<AttachmentDTO> Attachments { get; set; }
        public virtual List<RequestStepDTO> Steps { get; set; }

        public virtual List<WFHistoryDTO> WFHistory { get; set; }
    }
}

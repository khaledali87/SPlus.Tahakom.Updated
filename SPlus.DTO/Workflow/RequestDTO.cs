using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class RequestDTO
    {
        public int ID { get; set; }
        public int WorkflowID { get; set; }
        public string RequestedBy { get; set; }
        public int Status { get; set; }
        public int RequestType { get; set; }
        public bool CanApprove { get; set; }
        public virtual object Form { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        //public virtual List<WFRequestStepDTO> Steps { get; set; }
        //public AttachmentDTO Attachment { get; set; }

    }
}

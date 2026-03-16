using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{

    public class SaveWFFormUpdateKPIDTO
    {
        public int Type { get; set; }
        public int BaseWorkflowID { get; set; }
        public int ID { get; set; }
        public int RelatedID { get; set; }
        public decimal Value { get; set; }
        public decimal OldValue { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Target { get; set; }

        public bool IsReUpdate { get; set; } = false;   
        public bool IsSkipped { get; set; } = false;   

        public List<AttachmentDTO> Attachments { get; set; }
        //public AttachmentDTO Attachment { get; set; }
        public WFRequestDTO WFRequest { get; set; }
        public List<ParameterDTO> Parameters { get; set; }
    }
}

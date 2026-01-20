using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class ActionDTO
    {
        public int RequestID { get; set; }        
        public int Action { get; set; }
        public int QualityType { get; set; }
        public string Comments { get; set; }
        public string Reasons { get; set; }
        public int Level { get; set; }
        public List<AttachmentDTO> Attachments { get; set; }
    }
}

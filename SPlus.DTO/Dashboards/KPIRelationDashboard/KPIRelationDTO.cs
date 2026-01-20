using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class KPIRelationDTO:BaseDTO
    {
        public decimal Performance { get; set; }
        public string Status { get; set; }
        public AttachmentDTO Attachment { get; set; } = null;

        public List<KPIRelationPerspectiveDTO> Perspective { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class KPIRelationPerspectiveDTO:BaseDTO
    {
        public AttachmentDTO Attachment { get; set; } = null;
        public string BackgroundColor { get; set; }
        public List<KPIReleationKPIDTO> KPIs { get; set; }
    }
}

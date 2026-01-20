using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class SingularThemeDTO : BaseDTO
    {
        public decimal Weight { get; set; }
        public AttachmentDTO Attachment { get; set; }
        public string Color { get; set; }

    }
}
 
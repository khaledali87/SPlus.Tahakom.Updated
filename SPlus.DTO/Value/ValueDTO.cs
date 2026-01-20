using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class ValueDTO : BaseDTO
    {
        public int Order { get; set; }
        public AttachmentDTO Attachment { get; set; } = null;
    }
}

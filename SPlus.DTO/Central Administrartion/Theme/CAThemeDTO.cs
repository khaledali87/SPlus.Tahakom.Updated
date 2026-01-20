using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class CAThemeDTO : BaseDTO
    {        
        public CAStrategyDTO Strategy { get; set; }
        public int Order { get; set; }
        public AttachmentDTO Attachment { get; set; } = null;
        public string BackgroundColor { get; set; }
        public string FrameColor { get; set; }
    }
}

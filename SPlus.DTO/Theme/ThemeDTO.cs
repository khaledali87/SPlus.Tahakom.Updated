using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class ThemeDTO : BaseDTO
    {
        public int Order { get; set; }
        public string BackgroundColor { get; set; }
        public string FrameColor { get; set; }
        public AttachmentDTO Attachment { get; set; } = null;
        public List<StrategicObjectiveWithLEDDTO> StrategicObjectives { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class ThemeStrategicObjectiveDetailsDTO : BaseDTO
    {
        public CAStrategyDTO Strategy { get; set; }
        public AttachmentDTO Attachment { get; set; }
    }
}

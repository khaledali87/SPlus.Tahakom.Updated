using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class StrategicObjectiveByPerspectiveDTO : BaseDTO
    {
        public List<KPIByStrategicObjectiveDTO> KPIsByStrategicObjective { get; set; }
        public AttachmentDTO Attachment { get; set; }

    }
}

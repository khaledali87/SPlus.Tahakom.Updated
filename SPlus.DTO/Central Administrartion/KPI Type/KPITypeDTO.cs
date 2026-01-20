using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class KPITypeDTO : BaseDTO
    {
       
        public int KPITypeID { get; set; }


        public int GracePeriod { get; set; }

        public bool IsDepartmental { get; set; }

        public List<StatusDTO> Status { get; set; }

        public List<WorkflowDTO> Workflows { get; set; }
        public AttachmentDTO Attachment { get; set; }
        public ReminderConfigurationDTO ReminderConfiguration { get; set; }

    }
}

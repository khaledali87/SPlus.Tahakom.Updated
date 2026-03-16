using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class MyRequestDTO : BaseDTO
    {
        public int Type { get; set; }
        public int BaseWorkflowID { get; set; }
        public int RequestID { get; set; }
        public int Status { get; set; }
        public string Justification { get; set; }
        public UserListDTO CreatedBy { get; set; }
        public DateTime Created { get; set; }

        public decimal? AccumulutiveTarget { get; set; } = 0;
        public decimal? AccumulutiveValue { get; set; } = 0;

        public decimal? OldActualValue { get; set; } = 0;
        public decimal? ActualValue { get; set; } = 0;


        public UserListDTO OwnerModel { get; set; }
        public AttachmentDTO Attachment { get; set; }
        public OrgStructureDTO OrgStructure { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class MitigationActionRequestFormDTO : BaseLevelDTO
    {
        public bool IsDraft { get; set; }
        public int Type { get; set; }
        public int BaseWorkflow { get; set; }
        public int RequestID { get; set; }
        public string CreatedBy { get; set; }
        public UserListDTO CreatedByModel { get; set; }
        public DateTime Created { get; set; }

        public OrgStructureDTO Sector { get; set; }

        public OrgStructureDTO OrgStructure { get; set; }

        public CAKPIListingDTO KPI { get; set; }

        public string Status { get; set; }
        public string OtherStatus { get; set; }
        public string Findings { get; set; }

        public string Reasons { get; set; }
        public List<AttachmentDTO> Attachments { get; set; }
    }
}

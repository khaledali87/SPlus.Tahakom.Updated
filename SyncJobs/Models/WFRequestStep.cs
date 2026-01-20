using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    public class WFRequestStep
    {
        public int ID { get; set; }
        public int RequestID { get; set; }
        public string Approver { get; set; }
        public string ActionBy { get; set; }
        public UserInfo ActionByInfo { get; set; }
        public int WorkflowStepID { get; set; }
        public bool IsGroup { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public DateTime? Modified { get; set; }
        public string ActionTypeEnglishDetails { get; set; }
        public string ActionTypeArabicDetails { get; set; }
        public string Old { get; set; }
        public string New { get; set; }

    }
}

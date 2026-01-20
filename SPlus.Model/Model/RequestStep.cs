using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class RequestStep
    {
        private List<MeasureHistory> _MeasureHistory = new List<MeasureHistory>();
        public int ID { get; set; }
        public int RequestID { get; set; }
        public string RequestStepName { get; set; }
        public string RequestStepNameAr { get; set; }
        public string Role { get; set; }
        public string Approver { get; set; }
        public string ActionBy { get; set; }
        public UserInfo ActionByInfo { get; set; }
        public int WorkflowStepID { get; set; }
        public int WorkflowSnapshotID { get; set; }
        public bool IsGroup { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public DateTime? Modified { get; set; }
        public string ActionTypeEnglishDetails { get; set; }
        public string ActionTypeArabicDetails { get; set; }
        public string Old { get; set; }
        public string New { get; set; }
        public List<MeasureHistory> MeasureHistory
        {
            get
            {
                return _MeasureHistory;
            }
            set
            {
                _MeasureHistory = value;
            }
        }
    }
    public class MeasureHistory
    {
        public int MeasureID { get; set; } 
        public string DueDate { get; set; } 
        public decimal Old { get; set; }
        public decimal New { get; set; }
    }
}

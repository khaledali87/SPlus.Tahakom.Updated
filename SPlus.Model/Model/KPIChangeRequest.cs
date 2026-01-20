using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class KPIChangeRequest
    {
        private List<MeasureHistory> _MeasureHistory = new List<MeasureHistory>();
        private UserInfo _RequestedBy = new UserInfo();
        private UserInfo _UserName = new UserInfo(); 
        public int RequestID { get; set; }
        public string SubmitDate { get; set; }
        public UserInfo UserName
        {
            get
            {
                return _RequestedBy;
            }
            set
            {
                _RequestedBy = value;
            }
        }
        public string RequestedBy { get; set; }  
        public string Status { get; set; }
        public bool IsPermitiveToApproveReject { get; set; } = false;
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
}

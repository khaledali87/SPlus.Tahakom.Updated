using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class UpdateAction
    {
        private List<ChangeRequest> _ChangeRequest = new List<ChangeRequest>();
        public int KPIID { get; set; }
        public int MeasureID { get; set; }
        public int? RequestID { get; set; }
        public decimal Value { get; set; }
        public List<ChangeRequest> ChangeRequest
        {
            get
            {
                return _ChangeRequest;
            }
            set
            {
                _ChangeRequest = value;
            }
        }
        public int WorkflowType { get; set; } = 2;
    }
    public class ChangeRequest 
    {
        public decimal Target { get; set; }
        public int MeasureID { get; set; }
    }
}

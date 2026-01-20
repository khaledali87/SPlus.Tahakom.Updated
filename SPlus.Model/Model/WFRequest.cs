using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class WFRequest
    {
        private UserInfo _RequestedBy = new UserInfo();
        public int ID { get; set; }
        public int RelatedItemID { get; set; }
        public List<int> RelatedItemIDList { get; set; } = new List<int>();
        public UserInfo RequestedBy
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
        public string Status { get; set; } 
        public int WorkflowID { get; set; }
        public string Created { get; set; }
        public string CurrentStep { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class MyTask
    {
        private List<Attachement> _Attachements = new List<Attachement>();
        public int ID
        {
            get;
            set;
        }

        public int RelatedItemID
        {
            get;
            set;
        }
        public int? KPIID
        {
            get;
            set;
        }
        public int? MeasureID
        {
            get;
            set;
        }
        public int? RequestID
        {
            get;
            set;
        }
        public int RequestType { get; set; } 
        public bool IsGroup { get; set; }
        public string Status { get; set; }
        public string Group { get; set; }
        public bool isPermittiveToUpdate { get; set; }
        public KPI KPI
        {
            get;
            set;
        }
        public KPIMeasure KPIMeasure
        {
            get;
            set;
        }
        public UserInfo AssignedTo
        {
            get;
            set;
        }
        public List<Attachement> Attachements
        {
            get
            {
                return _Attachements;
            }
            set
            {
                _Attachements = value;
            }
        }
        public List<UserInfo> ActionBy { get; set; }

    }
}

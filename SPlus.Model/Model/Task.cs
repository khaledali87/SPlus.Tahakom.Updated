using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    
    public class GeneralTask
    {
        private KPI _KPI = new KPI();
        private KPIMeasure _KPIMeasure = new KPIMeasure();
        //private UserInfo _AssignedTo = new UserInfo();
        private List<Attachement> _Attachements = new List<Attachement>();
        public int ID
        {
            get; 
            set;
        }
        public int InstanceID
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
        public bool IsGroup { get; set; }
        public string Status { get; set; }
        public string Group { get; set; }
        public KPI KPI
        {
            get
            {
                return _KPI;
            }
            set
            {
                _KPI = value;
            }
        }
        public KPIMeasure KPIMeasure
        {
            get
            {
                return _KPIMeasure;
            }
            set
            {
                _KPIMeasure = value;
            }
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

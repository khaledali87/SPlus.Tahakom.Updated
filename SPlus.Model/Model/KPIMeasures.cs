using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class KPIMeasure
    {
        private General _GeneralData = new General();

        public General GeneralData
        {
            get
            {
                return _GeneralData;
            }
            set
            {
                _GeneralData = value;
            }
        }
        public int ID
        {
            set;
            get;
        }
        public decimal Target
        {
            set;
            get;
        }
        public decimal Value
        {
            set;
            get;
        }
        public decimal TempValue
        {
            set;
            get;
        }
        public decimal NewTarget 
        {
            set;
            get;
        }
        public decimal OldValue
        {
            set;
            get;
        }
        public int KPIID
        {
            set;
            get;
        }
        public int? RequestID
        {
            set;
            get;
        }
        public string DueDate
        {
            set;
            get;
        }
        public bool IsPermittiveToApproveReject
        {
            set;
            get;
        }
        public string KPIStatus
        {
            set;
            get;
        }
        public string LastUpdate
        {
            set;
            get;
        }
        public string TempLastUpdate
        {
            set;
            get;
        }
        public string Status
        {
            set;
            get;
        }
        public decimal OutOfTarget
        {
            set;
            get;
        }

        public int GracePeriod
        {
            set;
            get;
        }

        public DateTime? RequestCreatedDate
        {
            set;
            get;
        }
        public bool? ChampionPerformance
        {
            set;
            get;
        }
        public bool? OwnerPerformance
        {
            set;
            get;
        }
        public bool? SponserPerformance
        {
            set;
            get;
        }
    }
    public class MeasureYear
    {
        private List<string> _DueDate = new List<string>();
        public List<string> Years
        {
            get 
            {
                return _DueDate;
            }
            set
            {
                _DueDate = value;
            }
        } 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    public class KPI
    {
        private General _GeneralData = new General();
        private UserInfo champion = new UserInfo();
        private UserInfo owner = new UserInfo();
        private UserInfo sponser = new UserInfo();
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
        public string RequireUpdate
        {
            get;
            set;
        }
        public string KPIType
        {
            get;
            set;
        }
        public UserInfo Champion
        {
            get
            {
                return champion;
            }
            set
            {
                champion = value;
            }
        }
        public UserInfo Owner
        {
            get
            {
                return owner;
            }
            set
            {
                owner = value;
            }
        }
        public UserInfo Sponser
        {
            get
            {
                return sponser;
            }
            set
            {
                sponser = value;
            }
        }
        public List<KPIMeasure> Measures
        {
            set;
            get;
        }
        public bool? IsLocked
        {
            set;
            get;
        }
        public DateTime? UnlockDate
        {
            set;
            get;
        }
    }
}

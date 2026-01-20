using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    public class Notification
    {
        private General _GeneralData = new General();
        private UserInfo _UserInfo = new UserInfo();
        public int ID
        {
            set;
            get;
        }
        public string Title
        {
            set;
            get;
        }
        public int MeausreID
        {
            set;
            get;
        }
        public int DelegationID
        {
            set;
            get;
        }
        public int KPIID
        {
            set;
            get;
        }
        public string Created
        {
            set;
            get;
        }
        public UserInfo AssignedTo
        {
            get
            {
                return _UserInfo;
            }
            set
            {
                _UserInfo = value;
            }
        }
        public string Status
        {
            set;
            get;
        }
        public string NotificationType
        {
            set;
            get;
        }
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

    }
}

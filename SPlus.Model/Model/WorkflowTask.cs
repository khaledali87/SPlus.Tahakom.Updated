using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    
    public class WorkflowTask
    {
        //private UserInfo _UserInfo1 = new UserInfo();
        private List<UserInfo> _UserInfo1 = new List<UserInfo>();
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

        public int StepID
        {
            get;
            set;
        }
        public string EscalateLevel

        {
            get;
            set;
        }
        public string Status
        {
            get;
            set;
        }
        public string Comment
        {
            get;
            set;
        }
        public string ActionDate
        {
            get;
            set;
        }
        //public UserInfo ActionBy
        //{
        //    get
        //    {
        //        return _UserInfo1;
        //    }
        //    set
        //    {
        //        _UserInfo1 = value;
        //    }
        //}

        public List<UserInfo> ActionBy
        {
            get
            {
                return _UserInfo1;
            }
            set
            {
                _UserInfo1 = value;
            }
        }
        public string GroupName
        {
            get;
            set;
        }
    }
}

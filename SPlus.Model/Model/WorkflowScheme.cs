using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class WorkflowScheme
    {
        UserInfo _UserInfo1 = new UserInfo();
        public int ID
        {
            set;
            get;
        }
        public string Step
        {
            set;
            get;
        }

        public UserInfo Approver
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
        public bool IsGroup
        {
            set;
            get;
        }
        public int Order
        {
            set;
            get;
        }
        public int SchemeID
        {
            set;
            get;
        }
        public string StepAr { get; set; }
    }
}

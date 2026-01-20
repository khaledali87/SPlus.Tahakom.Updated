using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class WorkflowActionHistory
    { 
        UserInfo _UserInfo = new UserInfo(); 
        public int ID 
        {
            set;
            get;
        }

        public string Action
        {
            set;
            get;
        }
        public string Comment
        {
            set;
            get;
        }
        public int SchemeStepID
        {
            set;
            get;
        }
        public UserInfo UserInfo
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

    }
}

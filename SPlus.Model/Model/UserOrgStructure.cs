using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPlus.Model.Common;

namespace SPlus.Model
{
    public class UserOrgStructure
    {

        public string UserName
        {
            set;
            get;
        }
        public string DisplayName
        {
            set;
            get;
        }

        public string Role
        {
            set;
            get;
        }

        public int FunctionL1
        {
            set;
            get;
        }

        public int FunctionL2
        {
            set;
            get;
        }
        public int FunctionL3
        {
            set;
            get;
        }        
    }
}

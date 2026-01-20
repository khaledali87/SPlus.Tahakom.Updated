using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class UpdatedStatusChampion
    {
        private General _FunctionL2 = new General();
        
        public UserInfo UserInfo
        {
            set;
            get;
        }
        public decimal RequireUpdate
        {
            set;
            get;
        }
        public decimal Updated
        {
            set;
            get;
        }
    }
}

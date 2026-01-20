using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class UpdatedStatusDepartment
    {
        private General _FunctionL2 = new General();
        public General FunctionL2
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

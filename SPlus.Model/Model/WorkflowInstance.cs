using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class WorkflowInstance
    {
        public int ID
        {
            get;
            set;
        }
        public int MeasureID
        {
            get;
            set;
        }
      
        public string StepID
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
    }
}

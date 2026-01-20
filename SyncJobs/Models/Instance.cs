using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    internal class Instance
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
        public int SchemeID
        {
            get;
            set;
        }
        public string StepID
        {
            get;
            set;
        }

        public string WorkflowStatus
        {
            get;
            set;
        }
    }
}

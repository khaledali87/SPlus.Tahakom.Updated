using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    public class KPIMeasure
    {
        public int ID
        {
            set;
            get;
        }
        public int KPIID
        {
            set;
            get;
        }
        public string DueDate
        {
            set;
            get;
        }
        public decimal TempValue
        {
            set;
            get;
        }
        public string Status
        {
            set;
            get;
        }

    }
}

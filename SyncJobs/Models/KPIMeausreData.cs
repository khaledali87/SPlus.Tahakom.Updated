using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    public class KPIMeausreData
    {
        public int ID
        {
            get;
            set;
        }
        public int KPIID
        {
            get;
            set;
        }
        public decimal TempTarget
        {
            get;
            set;
        }
        public decimal Target
        {
            get;
            set;
        }
        public decimal Value
        {
            get;
            set;
        }
        public decimal TempValue
        {
            get;
            set;
        }
    }
}

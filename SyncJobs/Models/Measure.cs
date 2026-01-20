using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    internal class Measure
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
        public string Target
        {
            get;
            set;
        }
        public string Status
        {
            get;
            set;
        }
        public string Value
        {
            get;
            set;
        }
        public string TempValue
        {
            get;
            set;
        }
        public DateTime DueDate { get; set; }
    }
}

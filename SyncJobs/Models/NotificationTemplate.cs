using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    internal class NotificationTemplate
    {
        public int ID
        {
            get;
            set;
        }
        public string Type
        {
            get;
            set;
        }
        public string English
        {
            get;
            set;
        }
        public string Arabic
        {
            get;
            set;
        }
    }
}

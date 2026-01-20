using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    public class ReminderRegistry
    {
        public int ID
        {
            set;
            get;
        }
        public int RelatedItemID
        {
            set;
            get;
        }
        public string Type
        {
            set;
            get;
        }
        public DateTime ReminderDate
        {
            set;
            get;
        }

    }
}

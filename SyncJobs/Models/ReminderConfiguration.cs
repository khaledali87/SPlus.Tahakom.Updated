using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    public class ReminderConfiguration
    {
        public int ID
        {
            set;
            get;
        }
        public int TypeID
        {
            set;
            get;
        }
        public int BeforeReminder
        {
            set;
            get;
        }
        public int FirstReminder
        {
            set;
            get;
        }
        public int SecondReminder
        {
            set;
            get;
        }

    }
}

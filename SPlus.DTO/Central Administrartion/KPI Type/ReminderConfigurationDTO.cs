using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class ReminderConfigurationDTO
    {
        public int ID { get; set; }

        public int KPITypeID { get; set; }

        public int BeforeReminder { get; set; }

        public int FirstReminder { get; set; }

        public int SecondReminder { get; set; }
    }
}

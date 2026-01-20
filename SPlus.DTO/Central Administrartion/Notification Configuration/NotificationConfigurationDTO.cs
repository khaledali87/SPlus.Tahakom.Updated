using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class NotificationConfigurationDTO
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string NotificationEnglish { get; set; }

        public string NotificationArabic { get; set; }

        public string EmailSubject { get; set; }

        public string EmailBody { get; set; }

        public int? ActionType { get; set; }
        public List<NotificationParameterDTO> NotificationParameters { get; set; }

        //public List<NotificationReceiverDTO> NotificationReceivers { get; set; }
    }
}

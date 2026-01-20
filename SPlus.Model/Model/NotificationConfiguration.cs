using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class NotificationConfiguration
    {
        private List<NotificationConfigurationReciever> _receivers = new List<NotificationConfigurationReciever>();
        private List<NotificationConfigurationParameter> _parameters = new List<NotificationConfigurationParameter>();

        public int ID { get; set; }
        public string Title { get; set; }
        public string NotificationEnglish { get; set; }
        public string NotificationArabic { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }

        public int ActionType { get; set; }
        public List<NotificationConfigurationReciever> Receivers
        {
            get
            {
                return _receivers;
            }
            set
            {
                _receivers = value;
            }
        }
        public List<NotificationConfigurationParameter> Parameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                _parameters = value;
            }
        }
    }
    public class NotificationConfigurationReciever
    {
        List<UserInfo> _users = new List<UserInfo>();
        public int ID { get; set; }
        public int TemplateID { get; set; }
        public bool IsGroup { get; set; }
        public string ReceiverName { get; set; }

        public bool IsCC { get; set; }

        public List<UserInfo> Users
        {
            get { return _users; }
            set { _users = value; }
        }
    }
    public class NotificationConfigurationParameter
    {
        public int ID { get; set; }
        public int TemplateID { get; set; }
        public string Title { get; set; }
        public bool IsUser { get; set; }
        public string Value { get; set; }
    }
}

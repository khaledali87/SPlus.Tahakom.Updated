using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    internal class Escalation
    {
        public int KPIID
        {
            get;
            set;
        }
        public string EnglishName
        {
            get;
            set;
        }
        public string ArabicName
        {
            get;
            set;
        }
        public string AssignedTo
        {
            get;
            set;
        }
        public string _NotificationType
        {
            get;
            set;
        }
        public string _CCGroups
        {
            get;
            set;
        }
        public int _TaskID
        {
            get;
            set;
        }

    }
}

using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    internal class KPIDetails
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
        public bool IsBoard
        {
            get;
            set;
        }
        public string Owner
        {
            get;
            set;
        }
        public string KPIOwner
        {
            get;
            set;
        }
        public string KPIEnglishName { get; set; }
        public string KPIArabicName { get; set; }

        public string Champion
        {
            get;
            set;
        }

        public bool EnableEscalation { get; set; }
    }
}

using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    internal class SchemeStep
    {
        public int ID
        {
            get;
            set;
        }
        public int SchemeID
        {
            get;
            set;
        }
        public string Step
        {
            get;
            set;
        }
        public string Group
        {
            get;
            set;
        }
        public string Type
        {
            get;
            set;
        }
        public int Order
        {
            get;
            set;
        }
    }
}

using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    internal class Task
    {
        public int ID
        {
            get;
            set;
        }
        public int InstanceID
        {
            get;
            set;
        }
        public string TaskName
        {
            get;
            set;
        }
        public string AssignedTo
        {
            get;
            set;
        }
        public string Status
        {
            get;
            set;
        }
        public DateTime CreatedDate
        {
            get;
            set;
        }
        public DateTime ModifiedDate
        {
            get;
            set;
        }
        public string EscalatedLevel
        {
            get;
            set;
        }
        public int StepID
        {
            get;
            set;
        }
        public DateTime TaskCreatedDate { get; set; }
    }
}

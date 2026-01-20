using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class CAP
    {
        public int ID
        {
            get;
            set;
        }
        public string CAPID
        {
            get;
            set;
        }
        public DateTime StartDate
        {
            get;
            set;
        }
        public DateTime DueDate
        {
            get;
            set;
        }
        public string Item
        {
            get;
            set;
        }
        public string Type
        {
            get;
            set;
        }
        public int KPIID
        {
            get;
            set;
        }
        public DateTime Created
        {
            get;
            set;
        }
        public DateTime Modified
        {
            get;
            set;
        }
        public Attachement Attachements
        {
            get;
            set;
        }
    }
}

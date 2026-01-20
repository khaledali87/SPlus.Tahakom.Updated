using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    public class EmailTemplate
    {
        List<string> _CC = new List<string>();
        public int ID
        {
            get;
            set;
        }
        public string Subject
        {
            get;
            set;
        }
        public string Type
        {
            get;
            set;
        }
        public string Body
        {
            get;
            set;
        }

        public string To
        {
            get;
            set;
        }
        public List<string> CC
        {
            get
            {
                return _CC;
            }
            set
            {
                _CC = value;
            }
        }

    }
}

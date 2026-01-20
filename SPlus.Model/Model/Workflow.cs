using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Workflow
    {
        public int ID
        {
            set;
            get;
        }
        public int TaskID
        {
            set;
            get;
        }

        public string Status
        {
            set;
            get;
        }
        public string ActionBy
        {
            set;
            get;
        }
        public string Comment
        {
            set;
            get;
        }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}

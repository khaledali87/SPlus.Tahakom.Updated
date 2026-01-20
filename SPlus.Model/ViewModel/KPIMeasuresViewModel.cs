using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class KPIMeasuresViewModel
    {
        public int ID
        {
            set;
            get;
        }
        public int Target
        {
            set;
            get;
        }
        public int Value
        {
            set;
            get;
        }
        public int KPIID
        {
            set;
            get;
        }
        public string DueDate
        {
            set;
            get;
        }
        public string LastUpdate
        {
            set;
            get;
        }
    }
}

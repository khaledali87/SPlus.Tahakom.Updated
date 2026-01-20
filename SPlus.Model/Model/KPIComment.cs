using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class KPIComment
    {
        public int ID { get; set; }
        public int KPIID { get; set; }
        public string Comment { get; set; }
        public UserInfo Createdby { get; set; }
        public DateTime Created { get; set; } 
        public DateTime Modified { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Resource
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public bool IsKPI { get; set; }
        public int TypeID { get; set; }  
        //public string Code { get; set; }
    }
}

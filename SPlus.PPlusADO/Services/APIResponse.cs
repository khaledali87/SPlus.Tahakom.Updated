using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.PPlusADO
{
    public class APIResponse<T> where T : class
    {
        public T Data { get; set; }
        public object ErrorCode { get; set; }
        public string StatusCode { get; set; }
        public object StatusMessage { get; set; }
    }
  
}

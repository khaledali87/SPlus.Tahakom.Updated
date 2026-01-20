using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO 
{ 
    public class SimpleKPIDTO : BaseDTO
    {
        public bool IsLocked { get; set; }
        public bool IsEditable { get; set; }
        public bool AllowLock { get; set; }
    }
}

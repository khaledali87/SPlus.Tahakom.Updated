using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class KPILEDDTO : BaseLevelDTO
    {
        public string Status { get; set; }
        public decimal OutOfTarget { get; set; }
        public decimal AccumulutiveOutOfTarget { get; set; }
        public int KPITypeID { get; set; }
        public bool RequireUpdate { get; set; }
    }
}

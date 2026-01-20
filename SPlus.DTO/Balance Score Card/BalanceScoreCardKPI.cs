using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class BalanceScoreCardKPIListDTO : BaseLevelDTO
    {
        public bool RequireUpdate { get; set; }
        public string Status { get; set; }
        public decimal OutOfTarget { get; set; }
        public int KPITypeID { get; set; }


    }
}
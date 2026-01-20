using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class BalanceScoreCardKPIWithDetailDTO : BaseLevelDTO
    {
        public string Frequency { get; set; }
        public decimal Baseline { get; set; }
        public string Direction { get; set; }
        public bool RequireUpdate { get; set; }
        public string Status { get; set; }
        public decimal OutOfTarget { get; set; }
        public List<KPIMeasureDTO> KPIMeasures { get; set; }
        public int KPITypeID { get; set; }

        public decimal Target { get; set; }
    }
}

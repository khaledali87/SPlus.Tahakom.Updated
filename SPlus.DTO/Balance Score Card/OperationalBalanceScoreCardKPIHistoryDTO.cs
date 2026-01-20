using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class OperationalBalanceScoreCardKPIHistoryDTO
    {
        public DateTime Date { get; set; }
        public decimal Performance { get; set; }
        public string Status { get; set; }
    }
}

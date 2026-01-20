using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class ChartDTO
    {
        public int Count { get; set; }
        public List<ByStatusDTO> ByStatus { get; set; }
    }
    public class ByStatusDTO
    {
        public int Count { get; set; }
        public string Status { get; set; }
    }
}

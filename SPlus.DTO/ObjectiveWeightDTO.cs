using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class ObjectiveWeightDTO
    {
        public decimal Weight { get; set; }
        public List<CAKPIWeightDTO> KPIs { get; set; }
    }
}

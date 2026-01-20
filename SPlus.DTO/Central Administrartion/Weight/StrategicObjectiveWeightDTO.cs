using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class StrategicObjectiveWeightDTO
    {
        public int ID { get; set; }
        public string EnglishName { get; set; }
        public decimal Weight { get; set; }
        public List<KPIWeightDTO> KPIs { get; set; }
    }
}

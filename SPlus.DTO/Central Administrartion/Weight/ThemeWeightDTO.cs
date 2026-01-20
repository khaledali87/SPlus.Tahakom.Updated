using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class ThemeWeightDTO
    {

        public int ID { get; set; }
        public string EnglishName { get; set; }
        public decimal Weight { get; set; }
        public List<StrategicObjectiveWeightDTO> StrategicObjectives { get; set; }
    }
    
}

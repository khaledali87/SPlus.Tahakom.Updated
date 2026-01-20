using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class CAThemeWeightDTO : BaseDTO
    {
        public decimal Weight { get; set; }
        public List<CAStrategicObjectiveWeightDTO> StrategicObjectives { get; set; }
    }
}

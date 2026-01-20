using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class CAStrategyObjectiveDTO : CAStrategyDTO
    {
        public List<CAThemeStrategicObjectiveDTO> Themes { get; set; }
    }
}
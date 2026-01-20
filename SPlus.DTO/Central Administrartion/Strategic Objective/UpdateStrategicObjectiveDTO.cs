using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class UpdateStrategicObjectiveDTO : BaseDTO
    {
        public ThemeListDTO Perspective { get; set; }
        public ThemeListDTO Theme { get; set; }
    }
}

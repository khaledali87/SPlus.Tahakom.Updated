using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class SingularStrategicObjectiveDTO : BaseDTO
    {
        public decimal Weight { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public SingularThemeDTO Perspective { get; set; }
        public SingularThemeDTO Theme { get; set; }
    }
}

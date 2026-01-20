using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class CAStrategyWeightDTO : BaseDTO
    {
        public List<CAThemeWeightDTO> Themes { get; set; }
    }
}

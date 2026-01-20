using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class StrategyObjectiveDTO : BaseDTO
    {
        public List<ThemeObjectiveDTO> Themes { get; set; }
    }
}
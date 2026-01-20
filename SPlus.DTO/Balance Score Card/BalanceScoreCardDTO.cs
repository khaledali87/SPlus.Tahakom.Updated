using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class BalanceScoreCardDTO
    {
        public List<BalanceScoreCardTheme> Themes { get; set; }
        public List<BalanceScoreCardPerspective> Perspectives { get; set; }
        public List<BalanceScoreCardStrategicObjective> StrategicObjectives { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class BalanceScoreCardStrategicObjectiveDetails : BaseDTO
    {
        public BalanceScoreCardPerspective Perspective { get; set; }
        public BalanceScoreCardTheme Theme { get; set; }
        public List<BalanceScoreCardKPIWithDetailDTO> KPIs { get; set; }

    }
}

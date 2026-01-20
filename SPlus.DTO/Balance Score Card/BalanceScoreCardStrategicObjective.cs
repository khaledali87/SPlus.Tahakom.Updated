using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class BalanceScoreCardStrategicObjective : BaseDTO
    {
        public BalanceScoreCardTheme Theme { get; set; }
        public BalanceScoreCardPerspective Perspective { get; set; }
        public List<BalanceScoreCardKPIListDTO> KPIs { get; set; }

    }
} 

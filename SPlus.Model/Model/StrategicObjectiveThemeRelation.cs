using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class StrategicObjectiveThemeRelation
    {
        public int Id { get; set; }
        public int StrategicObjectiveId { get; set; }
        public int ThemeId { get; set; }
    }
}

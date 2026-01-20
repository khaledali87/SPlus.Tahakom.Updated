using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Helper
{
    public class DateHelper
    {
        static public DateTime GetEndDateWorkingDays(DateTime startDate, int Duration)
        {
            DateTime finishDate = startDate;
            for (int i = 1; i <= Duration; i++)
            {
                finishDate = finishDate.AddDays(1);
                if (finishDate.DayOfWeek == DayOfWeek.Friday || finishDate.DayOfWeek == DayOfWeek.Saturday)
                    Duration = Duration + 1;
            }
            return finishDate;
        }
    }
}

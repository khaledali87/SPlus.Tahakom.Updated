using Org.BouncyCastle.Asn1.X509;
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

        static public DateTime GetEndDateWorkingDays(DateTime startDate, int Duration, List<DateTime> holidays = default)
        {
            DateTime finishDate = startDate;
            for (int i = 1; i <= Duration; i++)
            {
                finishDate = finishDate.AddDays(1);
                if (finishDate.DayOfWeek == DayOfWeek.Friday || finishDate.DayOfWeek == DayOfWeek.Saturday || holidays.Any(h => h.Date == finishDate.Date))
                    Duration = Duration + 1;
            }

            return finishDate;
        }

        static public bool IsTodayWorkingDay(List<DateTime> holidays = default)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || holidays.Any(h => h.Date == DateTime.Today.Date))
                   return false;

            return true;
        }
    }
}

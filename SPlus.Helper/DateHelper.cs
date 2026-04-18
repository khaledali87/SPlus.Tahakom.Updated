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
            for (int i = 0; i < Duration; i++)
            {
                finishDate = i > 0 ? finishDate.AddDays(1) : finishDate;
                if (finishDate.DayOfWeek == DayOfWeek.Friday || finishDate.DayOfWeek == DayOfWeek.Saturday)
                    Duration = Duration + 1;
            }

            return finishDate;
        }

        //static public DateTime GetEndDateWorkingDays(DateTime startDate, int Duration, List<DateTime> holidays = default)
        //{
        //    DateTime finishDate = startDate;

        //    for (int i = 1; i <= Duration; i++)
        //    {
        //        finishDate = i > 1 ? finishDate.AddDays(1) : finishDate;
        //        if (!IsWorkingDay(finishDate, holidays))
        //            Duration = Duration + 1;
        //    }

        //    return finishDate;
        //}

        public static DateTime GetEndDateWorkingDays( DateTime startDate,int duration,List<DateTime> holidays = null)
        {
            if (duration <= 0)
                return startDate;

            DateTime currentDate = startDate;
            int workingDaysCount = 0;

            while (workingDaysCount < duration)
            {
                if (IsWorkingDay(currentDate, holidays))
                {
                    workingDaysCount++;
                }

                if (workingDaysCount < duration)
                {
                    currentDate = currentDate.AddDays(1);
                }
            }

            return currentDate;
        }


        static public bool IsTodayWorkingDay(List<DateTime> holidays = default)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || holidays.Any(h => h.Date == DateTime.Today.Date))
                   return false;

            return true;
        }

        static public bool IsWorkingDay(DateTime date, List<DateTime> holidays = default)
        {
            if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday || holidays.Any(h => h.Date == date.Date))
                return false;

            return true;
        }
    }
}

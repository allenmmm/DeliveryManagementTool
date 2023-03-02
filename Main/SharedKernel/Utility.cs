using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.SharedKernel
{
    public static class Utility
    {
        public static int CalculateOrderbookWeek(DateTime dateTime)
        {
            //returns in the format YYYYWW
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            int weekOfYear = cal.GetWeekOfYear(dateTime, CalendarWeekRule.FirstDay, DayOfWeek.Thursday);
            int year = cal.GetYear(dateTime);
            if (weekOfYear == 53)
            {
                year++;
                weekOfYear = 1;
            }
            else if ((cal.GetDayOfWeek(dateTime) == DayOfWeek.Thursday &&
                    cal.GetHour(dateTime) < 12))
            {
                weekOfYear--;
            }
            return int.Parse(year.ToString() +
                 weekOfYear.ToString("D2"));
        }
    }
}

using System;
using System.Globalization;

namespace DMT.SharedKernel.ValueObjects
{
    public class OrderbookWeek_VO : ValueObject<OrderbookWeek_VO>
    {
        private const DayOfWeek _FirstDayOfOrderbookWeek = DayOfWeek.Thursday;
        private const int _FirstHourOfOrderbookWeek = 12;

        public int Year { get; private set; }
        public int Week { get; private set; }
        public DateTime StartOfOrderBookWeek { get; private set; }

        private OrderbookWeek_VO() { }

        private OrderbookWeek_VO(int year, int week, DateTime startOfOrderBookWeek)
        {
            Year = year;
            Week = week;
            StartOfOrderBookWeek = startOfOrderBookWeek;
        }

        public  String FormatString()
        { //Week 2019:43
            return (Year.ToString() + ":" + Week.ToString("D2"));
        }

        public override string ToString()
        {
            return Year.ToString() + Week.ToString("D2");
        }

        public static OrderbookWeek_VO Create(DateTime dateTime)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;
            
            int weekOfYear = cal.GetWeekOfYear(dateTime, 
                                               CalendarWeekRule.FirstDay, 
                                               _FirstDayOfOrderbookWeek);
            int year = cal.GetYear(dateTime);

            if (weekOfYear == 53)
            {
                year++;
                weekOfYear = 1;
            }
            else if ((cal.GetDayOfWeek(dateTime) == _FirstDayOfOrderbookWeek &&
                    cal.GetHour(dateTime) < _FirstHourOfOrderbookWeek))
            {
                weekOfYear--;
            }
            return new OrderbookWeek_VO(year, 
                                        weekOfYear,
                                        FindStartOfOrderbookWeek(dateTime));
        }

        static DateTime FindStartOfOrderbookWeek(DateTime currentDateTime)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;
            DateTime dateSubtractor = currentDateTime;

            if (cal.GetDayOfWeek(currentDateTime) != _FirstDayOfOrderbookWeek ||
                (cal.GetDayOfWeek(currentDateTime) == _FirstDayOfOrderbookWeek &&
                     cal.GetHour(currentDateTime) < _FirstHourOfOrderbookWeek))
            {
                dateSubtractor = dateSubtractor.AddDays(-1);
                while (dateSubtractor.DayOfWeek != _FirstDayOfOrderbookWeek)
                    dateSubtractor = dateSubtractor.AddDays(-1);
            }

            return (new DateTime(dateSubtractor.Year,
                          dateSubtractor.Month,
                          dateSubtractor.Day,
                          _FirstHourOfOrderbookWeek,
                          0,
                          0));
        }
    }
}

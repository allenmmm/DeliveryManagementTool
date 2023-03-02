using DMT.SharedKernel;
using DMT.SharedKernel.ValueObjects;
using System;

namespace DMT.GeneratingOrderBooks.Domain.ValueObjects
{
    public class DateStamp_VO : ValueObject<DateStamp_VO>
    {
        public DateTime DatePulled { get; private set; }
        public DateTime DateCreated { get; private set; }
        public String OrderbookWeek { get; private set; }

        private DateStamp_VO(DateTime datePulled,
                             DateTime dateCreated)
        {
            DatePulled = datePulled;
            DateCreated = dateCreated;
            OrderbookWeek = OrderbookWeek_VO.Create(dateCreated).FormatString();
        }

        public static DateStamp_VO Create(DateTime datePulled,
                                          DateTime dateCreated)
        {
            Gaurd.AgainstNotEqual(OrderbookWeek_VO.Create(datePulled),
                OrderbookWeek_VO.Create(dateCreated),
                "Calculated orderbook week from date pulled (in planned and overdues) is not equal to current order book week");
            return new DateStamp_VO(datePulled, dateCreated);
        }
    }
}

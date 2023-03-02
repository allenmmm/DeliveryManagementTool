namespace DMT.DTO
{
    public class OrderbookWeekDTO
    {
        public string OrderbookWeek { get; private set; }

        public OrderbookWeekDTO(string orderbookWeek)
        {
            OrderbookWeek = orderbookWeek;
        }

        public OrderbookWeekDTO(OrderbookWeekDTO orderbookWeekDTO)
        {
            OrderbookWeek = orderbookWeekDTO.OrderbookWeek;
        }
    }
}

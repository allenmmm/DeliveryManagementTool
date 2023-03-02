using System;
using System.Collections.Generic;
using System.Linq;

namespace DMT.DTO
{
    public class OrderbookPreviewsDTO
    {
        public DateTime OrderbookWeekStart { get; private set; }
        public IReadOnlyList<OrderbookPreviewDTO> OrderbookPreviewDTOs { get; private set; }

        public OrderbookPreviewsDTO()
        {
            OrderbookPreviewDTOs = new List<OrderbookPreviewDTO>();
        }

        public OrderbookPreviewsDTO( DateTime orderbookWeekStart, 
                                     IEnumerable<OrderbookPreviewDTO> orderbookPreviewDTOs )
        {
            OrderbookWeekStart = orderbookWeekStart;
            OrderbookPreviewDTOs = orderbookPreviewDTOs
                                    .Select(obp => new OrderbookPreviewDTO(obp)).ToList();
        }
    }
}

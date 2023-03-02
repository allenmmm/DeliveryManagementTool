using DMT.DTO;
using System.Collections.Generic;

namespace DMT.Web.ViewModels
{
    public class GenerateOrderbooksIndexVM
    {
        public IEnumerable<OrderbookWeekDTO> OrderbookWeeks { get; private set; }
        public OrderbookPreviewsDTO OrderbookPreviews { get; private set; }

        public GenerateOrderbooksIndexVM(IEnumerable<OrderbookWeekDTO> orderbookWeeks,
                                         OrderbookPreviewsDTO orderbookPreviews)
        {
            OrderbookWeeks = orderbookWeeks;
            OrderbookPreviews = orderbookPreviews;
        }

        public GenerateOrderbooksIndexVM(GenerateOrderbooksIndexVM goiVM)
        {
            OrderbookWeeks = goiVM.OrderbookWeeks;
            OrderbookPreviews = goiVM.OrderbookPreviews;
        }
    }
}
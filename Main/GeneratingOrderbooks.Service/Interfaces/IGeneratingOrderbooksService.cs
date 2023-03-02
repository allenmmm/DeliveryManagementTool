using DMT.DTO;
using DMT.SharedKernel.Interface;
using System.Collections.Generic;

namespace DMT.GeneratingOrderbooks.Service.Interfaces
{
    public interface IGeneratingOrderbooksService
    {
        IReadOnlyList<OrderbookWeekDTO> GetOrderbookWeeks();
        OrderbookDTO GetOrderbook(ulong id); 
        OrderbookPreviewsDTO GetOrderbookPreviews(string orderbookWeek = "");
        void GenerateOrderbooks(IDateTime dateTime);
    }
}

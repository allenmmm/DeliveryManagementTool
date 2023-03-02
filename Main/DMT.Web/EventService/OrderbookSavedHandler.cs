using DMT.GeneratingOrderbooks.EventService;
using DMT.SharedKernel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMT.Web.EventService
{
    public class OrderBookSavedHandler : IHandle<OrderbookSavedEvent>
    {
        public void Handle(OrderbookSavedEvent orderbookSavedEvent)
        {
            

        }
    }
}
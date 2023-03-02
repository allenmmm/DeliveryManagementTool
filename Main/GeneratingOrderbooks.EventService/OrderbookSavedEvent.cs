using DMT.SharedKernel;
using DMT.SharedKernel.Interface;
using DMT.SharedKernel.ValueObjects;
using System;

namespace DMT.GeneratingOrderbooks.EventService
{
    public class OrderbookSavedEvent : IDomainEvent
    {
        public ulong OrderbookId { get; private set; }
        public DateTime DateTimeEventFired { get; private set; }
        public int VendorCode { get; private set; }
        public string VendorName { get; private set; }

        public OrderbookSavedEvent( ulong  orderBookId,
                                    DateTime dateGenerated,
                                    int vendorCode,
                                    string vendorName)
        {
            OrderbookId = orderBookId;
            DateTimeEventFired = dateGenerated;
            VendorCode = vendorCode;
            VendorName = vendorName;
        }
    }
}
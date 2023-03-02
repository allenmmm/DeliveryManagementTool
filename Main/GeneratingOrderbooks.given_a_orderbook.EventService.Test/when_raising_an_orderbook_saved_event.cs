
using DMT.GeneratingOrderbooks.EventService;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace GeneratingOrderbooks.given_an_orderbook.EventService.Test
{
    [TestFixture]
    public class when_raising_orderbook_saved_event
    {
        [Test]
        public void then_create_orderbook_saved_event()
        {
            //ARRANGE
            var dateTime = DateTime.Now; 

            //ACT
            OrderbookSavedEvent orderbookSavedEvent_ACT =
                new OrderbookSavedEvent(808080808,
                                        dateTime,
                                        123456,
                                        "Raytheon");

            //ASSERT
            orderbookSavedEvent_ACT.OrderbookId.Should().Be(808080808);
            orderbookSavedEvent_ACT.DateTimeEventFired.Should().Be(dateTime);
            orderbookSavedEvent_ACT.VendorCode.Should().Be(123456);
            orderbookSavedEvent_ACT.VendorName.Should().Be("Raytheon");
        }
    }
}

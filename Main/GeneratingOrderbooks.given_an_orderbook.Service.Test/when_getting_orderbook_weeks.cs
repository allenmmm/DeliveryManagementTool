using DMT.DTO;
using DMT.GeneratingOrderbooks.Service;
using DMT.ManagingNotifications.EventService;
using DMT.SharedKernel;
using DMT.SharedKernel.ValueObjects;
using FluentAssertions;
using FluentAssertions.Extensions;
using Moq;
using NUnit.Framework;
using SharedKernel.Test;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratingOrderbooks.given_an_orderbook.Service.Test
{
    [TestFixture]
    class when_getting_orderbook_weeks : TestBase
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void then_retrieve_orderbook_weeks()
        {
            //ARRANGE
            DateTime dateTime = new DateTime(2019, 10, 30);
            List<String> orderbookWeeks_EXP = new List<String>()
            {
                 OrderbookWeek_VO.Create(dateTime).FormatString()
            };
            List<OrderbookWeekDTO> orderbookWeekDTOs_EXP = new List<OrderbookWeekDTO>()
            {
                new  OrderbookWeekDTO("2019:44")             
            };
            MockGeneratingOrderBooksRepo.Setup(fn => fn.GetOrderbookWeeks())
                .Returns(orderbookWeeks_EXP);

            //ACT
            var orderbookWeeks_ACT = GeneratingOrderbooksService_SUT.GetOrderbookWeeks();

            //ASSERT
            MockGeneratingOrderBooksRepo.Verify(fn => fn.GetOrderbookWeeks(), Times.Once);
            orderbookWeeks_ACT.Count().Should().Be(1);
            orderbookWeeks_ACT.First().OrderbookWeek.Should()
                .Be(orderbookWeekDTOs_EXP.First().OrderbookWeek);
        }

        [Test]
        public void then_catch_exceptions()
        {
            MockGeneratingOrderBooksRepo.Setup(fn => fn.GetOrderbookWeeks()).
                Throws(new Exception("Unable to generate add notification table, database error"));
            //ACT
            Action sut = () => GeneratingOrderbooksService_SUT
                                    .GenerateOrderbooks(MockDateTime.Object);
            //ASSERT
            sut.Should().NotThrow<Exception>();
        }

        [Test]
        public void then_convert_orderbook_week_to_orderbook_week_dto()
        {
            //ARRANGE
            String orderbookWeek_EXP = "2019:41";            
            List<OrderbookWeekDTO> orderbookWeeksDTO_EXP = new List<OrderbookWeekDTO>()
            {
                new OrderbookWeekDTO(orderbookWeek_EXP)
            };

            List<String> orderbookWeeks_EXP = new List<String>()
            {
                orderbookWeek_EXP
            };
            //ACT
            var notificationsDTO_ACT = DTOConversion.
                ConvertOrderbookWeeksToOrderbookWeekDTOs(orderbookWeeks_EXP);
            //ASSERT
            Compare.Collection(notificationsDTO_ACT, orderbookWeeksDTO_EXP);
        }

        [Test]
        public void then_raise_notification_when_orderbook_weeks_invalid()
        {
            //ARRANGE
            DateTime dateTime = DateTime.Now;
            NotificationRaisedEvent notificationEvent_EXP =
                new NotificationRaisedEvent(NotificationCodes.UnableToRetrieveOrderbookWeeks,
                                            dateTime,
                                            "Exception of type 'System.Exception' was thrown.");
            NotificationRaisedEvent notificationEvent_ACT = null;
            DomainEvents.Register<NotificationRaisedEvent>(ab => notificationEvent_ACT = ab);
            MockGeneratingOrderBooksRepo.Setup(fn => fn.GetOrderbookWeeks())
                 .Throws(new Exception());
            //ACT
            Action act = () => GeneratingOrderbooksService_SUT.GetOrderbookWeeks();
            //ASSERT
            act.Should().NotThrow<Exception>();
            notificationEvent_ACT.DateTimeEventFired.Should().BeWithin(20.Seconds())
                .After(notificationEvent_EXP.DateTimeEventFired);
            notificationEvent_ACT.CustomMessage.Should().Be(notificationEvent_EXP.CustomMessage);
            notificationEvent_ACT.StatusId.Should().Be(notificationEvent_EXP.StatusId);
        }
    }
}

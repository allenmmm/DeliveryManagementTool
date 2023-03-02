using DMT.DTO;
using DMT.GeneratingOrderbooks.Service;
using DMT.GeneratingOrderBooks.Data;
using DMT.ManagingNotifications.Data;
using DMT.ManagingNotifications.Service;
using DMT.SharedKernel;
using DMT.SharedKernel.ValueObjects;
using DMT.Web.Controllers;
using DMT.Web.ViewModels;
using FluentAssertions;
using FluentAssertions.Extensions;
using MvcContrib.TestHelper;
using NUnit.Framework;
using SharedKernel.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GeneratingOrderbooks.given_an_orderbook.Integration.Test
{
    [TestFixture]
    public class when_getting_orderbook_weeks : TestBase
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
            DateTime dateTime = DateTime.Now;
            Seed(dateTime);
            IReadOnlyList<OrderbookWeekDTO> orderbookWeeks_EXP = new List<OrderbookWeekDTO>()
            {
                    new OrderbookWeekDTO(OrderbookWeek_VO.Create(dateTime).FormatString())
            };
            using (var repo_SUT = new GeneratingOrderbooksRepository(GeneratingOrderbooksContext))
            {
                GeneratingOrderbooksService GeneratingOrderbooksService =
                    new GeneratingOrderbooksService(repo_SUT);
                TestControllerBuilder testControllerBuilder = new TestControllerBuilder();
                GenerateOrderbooksController controller_SUT =
                    testControllerBuilder.CreateController<GenerateOrderbooksController>
                    (GeneratingOrderbooksService);
                Action act = () => controller_SUT.GenerateOrderbooks();
                act.Should().NotThrow<Exception>();
            }
            using (var repo_SUT = new GeneratingOrderbooksRepository(GeneratingOrderbooksContext))
            {
                GeneratingOrderbooksService GeneratingOrderbooksService =
                    new GeneratingOrderbooksService(repo_SUT);
                TestControllerBuilder testControllerBuilder = new TestControllerBuilder();
                GenerateOrderbooksController controller_SUT =
                    testControllerBuilder.CreateController<GenerateOrderbooksController>
                    (GeneratingOrderbooksService);
                //ACT
                var orderbookWeeksActionResult = controller_SUT.Index();
                var orderbookWeeksView = (ViewResult)orderbookWeeksActionResult;
                var generateOrderbooksIndexVM = orderbookWeeksView.Model as GenerateOrderbooksIndexVM;
                var orderbookWeeks_ACT = generateOrderbooksIndexVM.OrderbookWeeks;
                //ASSERT
                Compare.Collection(orderbookWeeks_ACT, orderbookWeeks_EXP);
            }
        }

        [Test]
        public void then_ensure_orderbook_weeks_is_not_null_on_exception_and_notification_raised()
        {
            //ARRANGE
            GeneratingOrderbooksService generatingOrderbooksService =
                new GeneratingOrderbooksService(null);
            TestControllerBuilder testControllerBuilder = new TestControllerBuilder();
            GenerateOrderbooksController controller_SUT =
                testControllerBuilder.CreateController<GenerateOrderbooksController>
                (generatingOrderbooksService);

            NotificationDTO notificationDTO_EXP = 
                new NotificationDTO(DateTime.Now,
                                    "Orderbook weeks can not be retrieved, unable to query for " +
                                            "generated orderbooks" +
                                    " (Object reference not set to an instance of an object.)",
                                     NotificationState.Error);

            //ACT
            var orderbookWeeksActionResult = controller_SUT.Index();
            var orderbookWeeksView = (ViewResult)orderbookWeeksActionResult;
            var generateOrderbooksIndexVM = orderbookWeeksView.Model as GenerateOrderbooksIndexVM;
            var orderbookWeeks_ACT = generateOrderbooksIndexVM.OrderbookWeeks;
            //ASSERT
            orderbookWeeks_ACT.Should().NotBeNull();
            using (var repo_SUT = new ManagingNotificationsRepository(ManagingNotificationsContext))
            {
                ManagingNotificationsService managingNotificationsService = 
                    new ManagingNotificationsService(repo_SUT);
                var notifications_ACT = managingNotificationsService.GetNotifications()
                    .Where(nId => nId.Description == notificationDTO_EXP.Description).First();
                notifications_ACT.DateRaised.Should().BeWithin(30.Seconds()).
                    After(notificationDTO_EXP.DateRaised);
                notifications_ACT.NotificationState.Should().Be(notificationDTO_EXP.NotificationState);
            }
        }
    }
}

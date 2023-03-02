using DMT.DTO;
using DMT.GeneratingOrderbooks.Service;
using DMT.GeneratingOrderBooks.Data;
using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.ManagingNotifications.Data;
using DMT.ManagingNotifications.Service;
using DMT.SharedKernel;
using DMT.SharedKernel.ValueObjects;
using DMT.Web.Controllers;
using DMT.Web.ViewModels;
using FluentAssertions;
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
    public class when_getting_orderbook_previews : TestBase
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void then_retrieve_orderbook_previews()
        {
            //ARRANGE
            DateTime dateTime = DateTime.Now;
            Seed(dateTime);

            PlannedAndOverdueFileParser fileParser = 
                new PlannedAndOverdueFileParser(dateTime);
            FileReadUtility.FileRead(fileParser);

            var suppliers_EXP = fileParser.PlannedAndOverdueOrders
                                .Select(s => new
                                        {
                                            SupplierId = s.SupplierId,
                                            SupplierName = s.SupplierName
                                        }).Distinct().ToList();

            List<OrderbookPreviewDTO> orderbookPreviewDTOs = new List<OrderbookPreviewDTO>();

            foreach(var supplier in suppliers_EXP)
            {
                orderbookPreviewDTOs.Add(
                    new OrderbookPreviewDTO(Orderbook.CalculateOrderbookId(dateTime, 
                                                                            supplier.SupplierId), 
                                            dateTime, 
                                            supplier.SupplierId, 
                                            supplier.SupplierName));
            };

            OrderbookPreviewsDTO orderbookPreviews_EXP = 
                new OrderbookPreviewsDTO(OrderbookWeek_VO.Create(dateTime).StartOfOrderBookWeek, 
                                        orderbookPreviewDTOs);

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
                var orderbookPreviewsActionResult = controller_SUT.Index();
                var orderbookPreviewsView = (ViewResult)orderbookPreviewsActionResult;
                var generateOrderbooksIndexVM = orderbookPreviewsView.Model as GenerateOrderbooksIndexVM;
                var orderbookPreviews_ACT = generateOrderbooksIndexVM.OrderbookPreviews;

                //ASSERT
                orderbookPreviews_ACT.OrderbookWeekStart.Should()
                    .Be(orderbookPreviews_EXP.OrderbookWeekStart);
   
                foreach(var orderbookPreviewDTO_ACT in orderbookPreviews_ACT.OrderbookPreviewDTOs)
                {
                    var orderPrev_EXP = orderbookPreviews_EXP.OrderbookPreviewDTOs
                        .Where(o => o.Id == orderbookPreviewDTO_ACT.Id).First();
                    orderbookPreviewDTO_ACT.DateGenerated.Should()
                        .BeOnOrAfter(orderPrev_EXP.DateGenerated);
                    orderbookPreviewDTO_ACT.Description.Should()
                        .Be(orderPrev_EXP.Description);
                    orderbookPreviewDTO_ACT.VendorCode.Should()
                        .Be(orderPrev_EXP.VendorCode);
                }
            }
        }

        [Test]
        public void then_ensure_orderbook_previews_is_not_null_on_exception_and_notification_raised()
        {
            //ARRANGE
            NotificationDTO notificationDTO_EXP =
                new NotificationDTO(DateTime.Now,
                                    "Orderbook previews can not be retrieved, none exist or database error" +
                                    " (Sequence contains no elements)",
                                     NotificationState.Error);
            using (var repo_SUT = new GeneratingOrderbooksRepository(GeneratingOrderbooksContext))
            {
                GeneratingOrderbooksService generatingOrderbooksService =
                new GeneratingOrderbooksService(repo_SUT);
                TestControllerBuilder testControllerBuilder = new TestControllerBuilder();
                GenerateOrderbooksController controller_SUT =
                    testControllerBuilder.CreateController<GenerateOrderbooksController>
                    (generatingOrderbooksService);
                //ACT
                var orderbookPreviewsActionResult = controller_SUT.Index();
                var orderbookPreviewsView = (ViewResult)orderbookPreviewsActionResult;
                var generateOrderbooksIndexVM = orderbookPreviewsView.Model as GenerateOrderbooksIndexVM;
                var orderbookWeeks_ACT = generateOrderbooksIndexVM.OrderbookPreviews;
                //ASSERT
                generateOrderbooksIndexVM.OrderbookPreviews.Should().NotBeNull();
            }

            using (var repo_SUT = new ManagingNotificationsRepository(ManagingNotificationsContext))
            {
                ManagingNotificationsService managingNotificationsService =
                    new ManagingNotificationsService(repo_SUT);
                var notifications_ACT = managingNotificationsService.GetNotifications();
                notifications_ACT.Count().Should().Be(1);
                notifications_ACT.First().DateRaised.Should().BeOnOrAfter(notificationDTO_EXP.DateRaised);
                notifications_ACT.First().Description.Should().Be(notificationDTO_EXP.Description);
                notifications_ACT.First().NotificationState.Should().Be(notificationDTO_EXP.NotificationState);
            }
        }
    }
}

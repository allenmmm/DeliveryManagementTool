using DMT.DTO;
using DMT.GeneratingOrderbooks.Service;
using DMT.GeneratingOrderBooks.Data;
using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.ManagingNotifications.Data;
using DMT.ManagingNotifications.Service;
using DMT.SharedKernel;
using DMT.SharedKernel.ValueObjects;
using DMT.Web.Controllers;
using FluentAssertions;
using Moq;
using MvcContrib.TestHelper;
using NUnit.Framework;
using SharedKernel.Test;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratingOrderbooks.given_an_orderbook.Integration.Test
{
    public class when_getting_orderbook : TestBase
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void then_retrieve_orderbook()
        {
            //ARRANGE  
            DateTime dateTime = new DateTime(2019, 8, 3);
            PlannedAndOverdueFileParser fileParser =
                new PlannedAndOverdueFileParser(DateTime.Now);
            FileReadUtility.FileRead(fileParser);

            var firstPandO = fileParser.PlannedAndOverdueOrders.First();
            var pandos_EXP = fileParser.PlannedAndOverdueOrders
                                       .Where(pando => pando.SupplierId == firstPandO.SupplierId).ToList();

            List<OrderDTO> orderDTOs_EXP = new List<OrderDTO>();
            foreach (var pando in pandos_EXP)
            {
                orderDTOs_EXP.Add(
                    new OrderDTO(pando.PurchaseOrder,
                                pando.POLineItem,
                                pando.POSchedLine,
                                pando.OpenPOQty,
                                pando.ItemDeliveryDate,
                                pando.StatDeliverySchedule,
                                pando.PartNumber,
                                pando.PartDescription));
            };

            OrderbookDTO orderbookDTO_EXP =
                new OrderbookDTO(firstPandO.SupplierName,
                                 firstPandO.SupplierId,
                                 OrderbookWeek_VO.Create(dateTime).FormatString(),
                                 dateTime,
                                 orderDTOs_EXP);

            PopulateTable("PlannedAndOverdueOrders", 
                           SqlSeeder.SetPandOSQLValues(pandos_EXP.GetRange(0, 1)));

            PopulateTable("Supplier",
                          SqlSeeder.SetSupplierSQLValues(pandos_EXP.GetRange(0, 1)));

            PopulateTable("Orderbooks",
                          SqlSeeder.SetOrderbookSQLValues(pandos_EXP.GetRange(0, 1), dateTime));

            PopulateTable("Orders",
              SqlSeeder.SetOrderSQLValues(pandos_EXP, dateTime));

            using (var repo_SUT = new GeneratingOrderbooksRepository(GeneratingOrderbooksContext))
            {
                GeneratingOrderbooksService GeneratingOrderbooksService =
                         new GeneratingOrderbooksService(repo_SUT);
                TestControllerBuilder testControllerBuilder = new TestControllerBuilder();
                GenerateOrderbooksController controller_SUT =
                    testControllerBuilder.CreateController<GenerateOrderbooksController>
                    (GeneratingOrderbooksService);

                var obId = Orderbook.CalculateOrderbookId(dateTime, firstPandO.SupplierId);

                //ACT
                var orderbook_ACT = controller_SUT.GetOrderbook(obId);

                //ASSERT
                orderbook_ACT.SupplierName.Should().Be(firstPandO.SupplierName);
                orderbook_ACT.SupplierCode.Should().Be(firstPandO.SupplierId);
                orderbook_ACT.OrderbookWeek.Should()
                    .Be(OrderbookWeek_VO.Create(dateTime).FormatString());
                orderbook_ACT.DateGenerated.Should().Be(dateTime);
                orderbook_ACT.OrderDTOs.Count().Should().Be(orderDTOs_EXP.Count());
                Compare.Collection(orderbook_ACT.OrderDTOs, orderbookDTO_EXP.OrderDTOs);
            }
        }

        [Test]
        public void then_ensure_orderbook_is_empty_on_exception_and_notification_raised_when_getting_orderbook()
        {
            //ARRANGE
            NotificationDTO notificationDTO_EXP =
                new NotificationDTO(DateTime.Now,
                                    "Orderbook can not be retrieved, none exist or database error " +
                                    "(Object reference not set to an instance of an object.)",
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
                var orderbookWeeks_ACT = controller_SUT.GetOrderbook(It.IsAny<ulong>());

                //ASSERT
                orderbookWeeks_ACT.Should().Should().NotBeNull();
                orderbookWeeks_ACT.OrderDTOs.Should().NotBeNull();
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

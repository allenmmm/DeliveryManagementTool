using DMT.DTO;
using DMT.GeneratingOrderbooks.Service;
using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.ManagingNotifications.EventService;
using DMT.SharedKernel;
using DMT.SharedKernel.Interface;
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
    public class when_getting_orderbook : TestBase
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void then_retrieve_orderbook_for_given_orderbook_week()
        {
            //ARRANGE
            DateTime dateTime = DateTime.Now;
            Mock<IDateTime> MockDateTime = new Mock<IDateTime>();
            MockDateTime.Setup(fn => fn.GetTime()).Returns(dateTime);

            var firstPandO = FileParser.PlannedAndOverdueOrders.First();
            var pandos_EXP = FileParser.PlannedAndOverdueOrders
                                .Where(pando => pando.SupplierId == firstPandO.SupplierId);

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

            Supplier supplier = new Supplier(firstPandO.SupplierId,firstPandO.SupplierName);
            supplier.AddOrderBook(new Orderbook(pandos_EXP, MockDateTime.Object));

            var orderbookWeekId_EXP = Orderbook.CalculateOrderbookId(dateTime, firstPandO.SupplierId);

            ulong orderbookWeekId_ACT = 0;
            MockGeneratingOrderBooksRepo.Setup(fn =>
                fn.GetSupplierWithOrderbookAndOrders(It.IsAny<ulong>()))
                .Callback<ulong>((id) => { orderbookWeekId_ACT = id; })
                .Returns(supplier);

            //ACT
            var orderbook_ACT = GeneratingOrderbooksService_SUT.GetOrderbook(orderbookWeekId_EXP);

            //ASSERT
            MockGeneratingOrderBooksRepo.Verify(fn => 
                fn.GetSupplierWithOrderbookAndOrders(It.IsAny<ulong>()), Times.Once);
            orderbookWeekId_ACT.Should().Be(orderbookWeekId_EXP);
            orderbook_ACT.SupplierName.Should().Be(firstPandO.SupplierName);
            orderbook_ACT.SupplierCode.Should().Be(firstPandO.SupplierId);
            orderbook_ACT.OrderbookWeek.Should()
                .Be(OrderbookWeek_VO.Create(dateTime).FormatString());
            orderbook_ACT.DateGenerated.Should().BeOnOrAfter(dateTime);
            orderbook_ACT.OrderDTOs.Count().Should().Be(orderDTOs_EXP.Count());
            Compare.Collection(orderbook_ACT.OrderDTOs, orderbookDTO_EXP.OrderDTOs);
        }

        [Test]
        public void then_convert_orderbook_to_orderbook_dto()
        {
            //ARRANGE           
            Mock<IDateTime> MockDateTime = new Mock<IDateTime>();
            MockDateTime.Setup(fn => fn.GetTime()).Returns(new DateTime(2019, 11, 6));
            Supplier supplier_EXP = new Supplier(123456, "flashy company");
            PlannedAndOverdueFileParser fileParser =
                new PlannedAndOverdueFileParser(MockDateTime.Object.GetTime());
            FileReadUtility.FileRead(fileParser);
            var pandos_EXP = fileParser.PlannedAndOverdueOrders
                                        .Where(o => o.SupplierId ==
                                                FileParser.PlannedAndOverdueOrders
                                                  .First().SupplierId);
            supplier_EXP.AddOrderBook(new Orderbook(pandos_EXP, MockDateTime.Object));  
            
            //ACT
            var orderbookDTO_ACT =
                DTOConversion.ConvertOrderbookToOrderbookDTO(supplier_EXP);

            //ASSERT
            orderbookDTO_ACT.SupplierCode.Should().Be(supplier_EXP.Id);
            orderbookDTO_ACT.SupplierName.Should().Be(supplier_EXP.Details.Name);
            orderbookDTO_ACT.OrderbookWeek.Should()
                .Be(supplier_EXP.Orderbooks.First().DateStamp.OrderbookWeek);
            orderbookDTO_ACT.DateGenerated.Should()
                .Be(supplier_EXP.Orderbooks.First().DateStamp.DateCreated);
            orderbookDTO_ACT.OrderDTOs.Count().Should()
                .Be(supplier_EXP.Orderbooks.First().Orders.Count());

            int i = 0;
            foreach (var orderDTO_ACT in orderbookDTO_ACT.OrderDTOs)
            {
                var firstOrder_EXP = supplier_EXP.Orderbooks.First().Orders.ElementAt(i++);
                orderDTO_ACT.Description.Should().Be(firstOrder_EXP.Part.Description);
                orderDTO_ACT.ItemDeliveryDate.Should().Be(firstOrder_EXP.Details.ItemDeliveryDate);
                orderDTO_ACT.Number.Should().Be(firstOrder_EXP.Part.Number);
                orderDTO_ACT.OpenPOQty.Should().Be(firstOrder_EXP.Details.OpenPOQty);
                orderDTO_ACT.POLineItem.Should().Be(firstOrder_EXP.Details.POLineItem);
                orderDTO_ACT.POSchedLine.Should().Be(firstOrder_EXP.Details.POSchedLine);
                orderDTO_ACT.PurchaseOrder.Should().Be(firstOrder_EXP.Details.PurchaseOrder);
                orderDTO_ACT.StatDeliverySchedule.Should().Be(firstOrder_EXP.Details.StatDeliverySchedule);
            }           
        }

        [Test]
        public void then_catch_exceptions()
        {
            //ARRANGE
            MockGeneratingOrderBooksRepo.Setup(fn => 
                fn.GetSupplierWithOrderbookAndOrders(It.IsAny<ulong>())).
                Throws(new Exception("Unable to generate add notification table, database error"));

            //ACT
            Action sut = () => GeneratingOrderbooksService_SUT.GetOrderbook(It.IsAny<ulong>());

            //ASSERT
            sut.Should().NotThrow<Exception>();
        }

        [Test]
        public void then_raise_notification_when_retrieval_of_orderbook_invalid()
        {
            //ARRANGE
            DateTime dateTime = DateTime.Now;
            NotificationRaisedEvent notificationEvent_EXP =
                new NotificationRaisedEvent(NotificationCodes.UnableToRetrieveOrderbook,
                                            dateTime,
                                            "Object reference not set to an instance of an object.");
            NotificationRaisedEvent notificationEvent_ACT = null;
            DomainEvents.Register<NotificationRaisedEvent>(ab => notificationEvent_ACT = ab);

            //ACT
            var orderbook_ACT = GeneratingOrderbooksService_SUT.GetOrderbook(It.IsAny<ulong>());

            //ASSERT
            orderbook_ACT.Should().NotBeNull();
            orderbook_ACT.OrderDTOs.Should().NotBeNull();
            notificationEvent_ACT.DateTimeEventFired.Should().BeWithin(20.Seconds())
                .After(notificationEvent_EXP.DateTimeEventFired);
            notificationEvent_ACT.CustomMessage.Should().Be(notificationEvent_EXP.CustomMessage);
            notificationEvent_ACT.StatusId.Should().Be(notificationEvent_EXP.StatusId);
        }

        [Test]
        //IN THIS TEST
        public void then_raise_orderbook_saved_event()
        {
            //ARRANGE
            DateTime dateTime = DateTime.Now;
            NotificationRaisedEvent notificationEvent_EXP =
                new NotificationRaisedEvent(NotificationCodes.UnableToRetrieveOrderbook,
                                            dateTime,
                                            "Object reference not set to an instance of an object.");
            NotificationRaisedEvent notificationEvent_ACT = null;
            DomainEvents.Register<NotificationRaisedEvent>(ab => notificationEvent_ACT = ab);

            //ACT
            var orderbook_ACT = GeneratingOrderbooksService_SUT.GetOrderbook(It.IsAny<ulong>());

            //ASSERT
            orderbook_ACT.Should().NotBeNull();
            orderbook_ACT.OrderDTOs.Should().NotBeNull();
            notificationEvent_ACT.DateTimeEventFired.Should().BeWithin(20.Seconds())
                .After(notificationEvent_EXP.DateTimeEventFired);
            notificationEvent_ACT.CustomMessage.Should().Be(notificationEvent_EXP.CustomMessage);
            notificationEvent_ACT.StatusId.Should().Be(notificationEvent_EXP.StatusId);        
        }
    }
}

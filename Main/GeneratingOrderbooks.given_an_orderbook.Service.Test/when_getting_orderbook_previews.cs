using DMT.DTO;
using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.SharedKernel.Interface;
using DMT.SharedKernel.ValueObjects;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SharedKernel.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using DMT.GeneratingOrderbooks.Service;
using DMT.SharedKernel;
using DMT.ManagingNotifications.EventService;
using FluentAssertions.Extensions;

namespace GeneratingOrderbooks.given_an_orderbook.Service.Test
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
        public void then_retrieve_orderbook_previews_for_given_orderbook_week()
        {
            //ARRANGE
            DateTime dateTime = DateTime.Now;
            Mock<IDateTime> MockDateTime = new Mock<IDateTime>();
            MockDateTime.Setup(fn => fn.GetTime()).Returns(dateTime);

            var firstPandO = FileParser.PlannedAndOverdueOrders.First();

            var pandos_EXP = FileParser.PlannedAndOverdueOrders
                .Where(po => po.SupplierId == firstPandO.SupplierId);

            List<OrderbookPreviewDTO> orderbookPreviewDTOs_EXP = new List<OrderbookPreviewDTO>();

            var obID = Orderbook.CalculateOrderbookId(dateTime, pandos_EXP.First().SupplierId);
            orderbookPreviewDTOs_EXP.Add(new OrderbookPreviewDTO(obID, 
                                                                dateTime, 
                                                                pandos_EXP.First().SupplierId,
                                                                pandos_EXP.First().SupplierName));
        
            var orderbookWeek = OrderbookWeek_VO.Create(dateTime);

            OrderbookPreviewsDTO orderbookPreviewsDTO_EXP =
                    new OrderbookPreviewsDTO(dateTime, orderbookPreviewDTOs_EXP);

            List<Supplier> suppliers = new List<Supplier>();
 
            var supplier = new Supplier(firstPandO.SupplierId, firstPandO.SupplierName);
            supplier.AddOrderBook(new Orderbook(pandos_EXP, MockDateTime.Object));
            suppliers.Add(supplier);

            string orderbookWeek_ACT ="";
            MockGeneratingOrderBooksRepo.Setup(fn => 
                fn.GetSuppliersWithOrderbookForWeek(It.IsAny<string>()))
                .Callback<string>((obw) => { orderbookWeek_ACT = obw; })
                .Returns(suppliers);

            //ACT
            var orderbookPreviewsDTO_ACT = GeneratingOrderbooksService_SUT.GetOrderbookPreviews();

            //ASSERT
            orderbookWeek_ACT.Should().Be(orderbookWeek.FormatString());
            MockGeneratingOrderBooksRepo.Verify
                (fn => fn.GetSuppliersWithOrderbookForWeek(It.IsAny<String>()), Times.Once);
            orderbookPreviewsDTO_ACT.OrderbookWeekStart.Should()
                .Be(orderbookWeek.StartOfOrderBookWeek);
            Compare.Collection(orderbookPreviewsDTO_ACT.OrderbookPreviewDTOs, 
                                orderbookPreviewsDTO_EXP.OrderbookPreviewDTOs);
        }

        [Test]
        public void then_catch_exceptions()
        {
            MockGeneratingOrderBooksRepo.Setup(fn => fn.GetSuppliersWithOrderbookForWeek(It.IsAny<string>())).
                Throws(new Exception("Unable to generate add notification table, database error"));
            //ACT
            Action sut = () => GeneratingOrderbooksService_SUT
                                   .GenerateOrderbooks(MockDateTime.Object);
            //ASSERT
            sut.Should().NotThrow<Exception>();
        }

        [Test]
        public void then_convert_supplier_orderbook_to_orderbook_previews_dto()
        {
            //ARRANGE           
            Mock<IDateTime> MockDateTime = new Mock<IDateTime>();
            MockDateTime.Setup(fn => fn.GetTime()).Returns(new DateTime(2019,11,6));
            Supplier supplier_EXP = new Supplier(123456, "flashy company");
            PlannedAndOverdueFileParser fileParser = 
                new PlannedAndOverdueFileParser(MockDateTime.Object.GetTime());
            FileReadUtility.FileRead(fileParser);
            var pandos_EXP = fileParser.PlannedAndOverdueOrders
                                        .Where(o => o.SupplierId == 
                                                FileParser.PlannedAndOverdueOrders
                                                  .First().SupplierId);
            supplier_EXP.AddOrderBook(new Orderbook(pandos_EXP, MockDateTime.Object));
            List<Supplier> suppliers_EXP = new List<Supplier>();

            suppliers_EXP.Add(supplier_EXP);
            //ACT
            var orderbookPreviewsDTO_ACT = 
                DTOConversion.ConvertSuppliersToOrderbookPreviewsDTO(suppliers_EXP);
            //ASSERT
            orderbookPreviewsDTO_ACT.OrderbookWeekStart.Should()
                .Be(new DateTime(2019,10,31,12,0,0));
            foreach(var orderbookPreviewDTO_ACT in orderbookPreviewsDTO_ACT.OrderbookPreviewDTOs)
            {
                orderbookPreviewDTO_ACT.Id.Should().Be(supplier_EXP.Orderbooks.First().Id);
                orderbookPreviewDTO_ACT.VendorCode.Should().Be(supplier_EXP.Id);
                orderbookPreviewDTO_ACT.Description.Should().Be(supplier_EXP.Details.Name);
            }        
        }

        [Test]
        public void then_raise_notification_when_retrieval_of_orderbook_previews_invalid()
        {
            //ARRANGE
            DateTime dateTime = DateTime.Now;
            NotificationRaisedEvent notificationEvent_EXP =
                new NotificationRaisedEvent(NotificationCodes.UnableToRetrieveOrderbookPreviews,
                                            dateTime,
                                            "Exception of type 'System.Exception' was thrown.");
            NotificationRaisedEvent notificationEvent_ACT = null;
            DomainEvents.Register<NotificationRaisedEvent>(ab => notificationEvent_ACT = ab);
            MockGeneratingOrderBooksRepo.Setup(fn => fn.GetSuppliersWithOrderbookForWeek(It.IsAny<string>()))
                 .Throws(new Exception());
            //ACT
            Action act = () => GeneratingOrderbooksService_SUT.GetOrderbookPreviews();
            //ASSERT
            act.Should().NotThrow<Exception>();
            notificationEvent_ACT.DateTimeEventFired.Should().BeWithin(20.Seconds())
                .After(notificationEvent_EXP.DateTimeEventFired);
            notificationEvent_ACT.CustomMessage.Should().Be(notificationEvent_EXP.CustomMessage);
            notificationEvent_ACT.StatusId.Should().Be(notificationEvent_EXP.StatusId);
        }
    }
}

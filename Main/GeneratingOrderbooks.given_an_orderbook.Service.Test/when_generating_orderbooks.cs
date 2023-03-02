using DMT.GeneratingOrderbooks.Service;
using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.ManagingNotifications.EventService;
using DMT.SharedKernel;
using DMT.SharedKernel.Interface;
using FluentAssertions;
using FluentAssertions.Extensions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratingOrderbooks.given_an_orderbook.Service.Test
{
    [TestFixture]
    public class when_generating_orderbooks : TestBase
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
        }
        
        [Test]
        public void then_generate_orderbooks_when_supplier_registered()
        {
            //ARRANGE
            List<NotificationRaisedEvent> notificationEvents_EXP = new List<NotificationRaisedEvent>()
            {
                new NotificationRaisedEvent(NotificationCodes.OrderbookGenerationStarted,
                                      DateTime.Now),
                new NotificationRaisedEvent(NotificationCodes.OrderbookGeneratedOK,
                                      DateTime.Now,
                                      "Supplier (149024), "),
                new NotificationRaisedEvent(NotificationCodes.OrderbooksGeneratedOK,
                                     DateTime.Now,
                                     "1/1 succeeded for ")
            };

            int supplierCode_ACT = 0;
            int supplierCodeToPANDO_ACT = 0;
            int firstSupplierId_EXP = FileParser.PlannedAndOverdueOrders.First().SupplierId;
            var firstOrder = FileParser.PlannedAndOverdueOrders.First();

            MockGeneratingOrderBooksRepo.Setup(fn => fn.GetDistinctSupplierCodesFromPANDO())
                .Returns(new List<int>() { firstSupplierId_EXP } );
            MockGeneratingOrderBooksRepo.Setup(fn => 
            fn.GetSupplierWithOrderbookAndOrders(It.IsAny<int>(), It.IsAny<IDateTime>()))
                .Callback<int,IDateTime>((supplierCode,dateTime) => { supplierCode_ACT = supplierCode; })
                .Returns(new Supplier(firstSupplierId_EXP,
                                      FileParser.PlannedAndOverdueOrders.First().SupplierName));

            IEnumerable<PlannedAndOverdueOrder> newOrder_EXP = new List<PlannedAndOverdueOrder>()
            { 
                new PlannedAndOverdueOrder( "uniquenumber", 
                                            firstOrder.SupplierId, 
                                            "complete test", 
                                            firstOrder.PartNumber, 
                                            firstOrder.PartDescription,
                                            firstOrder.PurchaseOrder,
                                            firstOrder.POLineItem, 
                                            firstOrder.POSchedLine, 
                                            firstOrder.OpenPOQty, 
                                            firstOrder.ItemDeliveryDate,
                                            firstOrder.StatDeliverySchedule,
                                            DateTime.Now )
             };

            MockGeneratingOrderBooksRepo.Setup(fn => fn.GetOrdersPerSupplierFromPANDO(It.IsAny<int>())).
                Callback<int>(supplierCode => { supplierCodeToPANDO_ACT = supplierCode; })
                .Returns(newOrder_EXP);

            MockGeneratingOrderBooksRepo.Setup(fn => fn.SaveSupplier(It.IsAny<Supplier>()));

            NotificationRaisedEvent notificationEvent_EXP = new NotificationRaisedEvent(Guid.NewGuid(), DateTime.Now, "Event Happened" );
            List<NotificationRaisedEvent> notificationEvents_ACT = new List<NotificationRaisedEvent>();
            DomainEvents.Register<NotificationRaisedEvent>(ab => notificationEvents_ACT.Add(ab));
            //ACT
            GeneratingOrderbooksService_SUT.GenerateOrderbooks(MockDateTime.Object);
            //ASSERT
            MockGeneratingOrderBooksRepo.Verify(fn => fn.ValidatePlannedAndOverDues(), Times.Once);
            MockGeneratingOrderBooksRepo.Verify(fn => fn.GetDistinctSupplierCodesFromPANDO(), Times.Once);
            MockGeneratingOrderBooksRepo.Verify(fn => 
                fn.GetSupplierWithOrderbookAndOrders(It.IsAny<int>(), It.IsAny<IDateTime>()), Times.Once);
            MockGeneratingOrderBooksRepo.Verify(fn => fn.GetOrdersPerSupplierFromPANDO(It.IsAny<int>()), Times.Once);
            MockGeneratingOrderBooksRepo.Verify(fn => fn.SaveSupplier(It.IsAny<Supplier>()),Times.Once);
            MockGeneratingOrderBooksRepo.Verify(fn => fn.DeletePlannedAndOverdues(), Times.Once);
            supplierCode_ACT.Should().Be(firstSupplierId_EXP);
            supplierCodeToPANDO_ACT.Should().Be(firstSupplierId_EXP);
            int i = 0;
            foreach (var notification_ACT in notificationEvents_ACT)
            {
                var notification_EXP = notificationEvents_EXP.ElementAt(i++);
                notification_ACT.DateTimeEventFired.Should().BeWithin(20.Seconds()).After(notification_EXP.DateTimeEventFired);
                notification_ACT.CustomMessage.Should().Be(notification_EXP.CustomMessage);
                notification_ACT.StatusId.Should().Be(notification_EXP.StatusId);
            }
        }

        [Test]
        public void then_generate_orderbooks_when_supplier_NOT_registered()
        {
            //ARRANGE
            int supplierCode_ACT = 0;
            int supplierCodeToPANDO_ACT = 0;
            int firstSupplierId_EXP = FileParser.PlannedAndOverdueOrders.First().SupplierId;
            var firstSupplierName_EXP = FileParser.PlannedAndOverdueOrders.First().SupplierName;
            var firstOrder = FileParser.PlannedAndOverdueOrders.First();
            MockGeneratingOrderBooksRepo.Setup(fn => fn.GetDistinctSupplierCodesFromPANDO())
                .Returns(new List<int>() { firstSupplierId_EXP });

            MockGeneratingOrderBooksRepo.Setup(fn => fn.GetSupplierWithOrderbookAndOrders(It.IsAny<int>(), It.IsAny<IDateTime>()))
               .Callback<int, IDateTime>((supplierCode, dateTime) => { supplierCode_ACT = supplierCode; });

            MockGeneratingOrderBooksRepo.Setup(fn => fn.GetSupplierNameFromPANDO(It.IsAny<int>()))
                    .Callback<int>(supplierCode => { supplierCode_ACT = supplierCode; })
                    .Returns(firstSupplierName_EXP);

            IEnumerable<PlannedAndOverdueOrder> newOrder_EXP = new List<PlannedAndOverdueOrder>()
            {
                new PlannedAndOverdueOrder( "uniquenumber", 
                                            firstOrder.SupplierId,
                                            "complete test", 
                                            firstOrder.PartNumber, 
                                            firstOrder.PartDescription,
                                            firstOrder.PurchaseOrder, 
                                            firstOrder.POLineItem, 
                                            firstOrder.POSchedLine, 
                                            firstOrder.OpenPOQty, 
                                            firstOrder.ItemDeliveryDate,
                                            firstOrder.StatDeliverySchedule,
                                            DateTime.Now )
            };

            MockGeneratingOrderBooksRepo.Setup(fn => fn.GetOrdersPerSupplierFromPANDO(It.IsAny<int>())).
                Callback<int>(supplierCode => { supplierCodeToPANDO_ACT = supplierCode; })
                .Returns(newOrder_EXP);

            MockGeneratingOrderBooksRepo.Setup(fn => fn.SaveSupplier(It.IsAny<Supplier>()));

            //ACT
             GeneratingOrderbooksService_SUT.GenerateOrderbooks(MockDateTime.Object);
            //ASSERT
            MockGeneratingOrderBooksRepo.Verify(fn => fn.ValidatePlannedAndOverDues(), Times.Once);
            MockGeneratingOrderBooksRepo.Verify(fn => fn.ValidatePlannedAndOverDues(), Times.Once);
            MockGeneratingOrderBooksRepo.Verify(fn => fn.ValidatePlannedAndOverDues(), Times.Once);
            MockGeneratingOrderBooksRepo.Verify(fn => fn.GetDistinctSupplierCodesFromPANDO(), Times.Once);
            MockGeneratingOrderBooksRepo.Verify(fn => 
                fn.GetSupplierWithOrderbookAndOrders(It.IsAny<int>(), It.IsAny<IDateTime>()), Times.Once);
            MockGeneratingOrderBooksRepo.Verify(fn => fn.GetSupplierNameFromPANDO(It.IsAny<int>()), Times.Once);
            MockGeneratingOrderBooksRepo.Verify(fn => fn.GetOrdersPerSupplierFromPANDO(It.IsAny<int>()), Times.Once);
            MockGeneratingOrderBooksRepo.Verify(fn => fn.SaveSupplier(It.IsAny<Supplier>()), Times.Once);
            MockGeneratingOrderBooksRepo.Verify(fn => fn.DeletePlannedAndOverdues(), Times.Once);
            supplierCode_ACT.Should().Be(firstSupplierId_EXP);
            supplierCodeToPANDO_ACT.Should().Be(firstSupplierId_EXP);
        }

        [Test]
        public void then_raise_notification_when_pando_table_invalid()
        {
            //ARRANGE
            DateTime dateTime = DateTime.Now;
            List<NotificationRaisedEvent> notificationEvents_EXP = new List<NotificationRaisedEvent>()
            {
                new NotificationRaisedEvent(NotificationCodes.OrderbookGenerationStarted,
                                      dateTime),
                new NotificationRaisedEvent(NotificationCodes.PandOInvalidTableAccess,
                                     dateTime,
                                     "Exception of type 'System.Exception' was thrown."),
                new NotificationRaisedEvent(NotificationCodes.OrderbooksGeneratedError,
                                     dateTime)
            };

            List<NotificationRaisedEvent> notificationEvents_ACT = new List<NotificationRaisedEvent>();
            DomainEvents.Register<NotificationRaisedEvent>(ab => notificationEvents_ACT.Add(ab));
            MockGeneratingOrderBooksRepo.Setup(fn => fn.GetDistinctSupplierCodesFromPANDO())
                 .Throws(new Exception());
            //ACT
            Action act = ()=> GeneratingOrderbooksService_SUT.GenerateOrderbooks(MockDateTime.Object);
            //ASSERT
            act.Should().NotThrow<Exception>();
            notificationEvents_EXP.Count().Should().Be(notificationEvents_ACT.Count);
            int i = 0;
            foreach (var notification_ACT in notificationEvents_ACT)
            {
                var notification_EXP = notificationEvents_EXP.ElementAt(i++);
                notification_ACT.DateTimeEventFired.Should().BeWithin(20.Seconds()).After(notification_EXP.DateTimeEventFired);
                notification_ACT.CustomMessage.Should().Be(notification_EXP.CustomMessage);
                notification_ACT.StatusId.Should().Be(notification_EXP.StatusId);
            }
        }

        [Test]
        public void then_raise_notification_when_cant_access_suppliers_table()
        {
            DateTime dateTime = DateTime.Now;

            List<NotificationRaisedEvent> notificationEvents_EXP = new List<NotificationRaisedEvent>()
            {
                new NotificationRaisedEvent(NotificationCodes.OrderbookGenerationStarted,
                                    dateTime),
                new NotificationRaisedEvent(NotificationCodes.OrderbookGeneratedError,
                                     dateTime,
                                      "Supplier (123456), Exception of type 'System.Exception' was thrown.  "),
                new NotificationRaisedEvent(NotificationCodes.OrderbooksGeneratedError,
                                   dateTime,
                                   "0/1 succeeded for ")
            };

            List<NotificationRaisedEvent> notificationEvents_ACT = new List<NotificationRaisedEvent>();
            DomainEvents.Register<NotificationRaisedEvent>(ab => notificationEvents_ACT.Add(ab));
            MockGeneratingOrderBooksRepo.Setup(fn => fn.GetDistinctSupplierCodesFromPANDO())
                 .Returns(new List<int>() { 123456 });
            MockGeneratingOrderBooksRepo.Setup(fn => 
               fn.GetSupplierWithOrderbookAndOrders(It.IsAny<int>(), It.IsAny<IDateTime>()))
                .Throws(new Exception());                                                                                   
                
            //ACT
            Action act = () => GeneratingOrderbooksService_SUT.GenerateOrderbooks(MockDateTime.Object);
            //ASSERT
            act.Should().NotThrow<Exception>();
            notificationEvents_EXP.Count().Should().Be(notificationEvents_ACT.Count);
            int i = 0;
            foreach (var notification_ACT in notificationEvents_ACT)
            {
                var notification_EXP = notificationEvents_EXP.ElementAt(i++);
                notification_ACT.DateTimeEventFired.Should().BeWithin(20.Seconds()).After(notification_EXP.DateTimeEventFired);
                notification_ACT.CustomMessage.Should().Be(notification_EXP.CustomMessage);
                notification_ACT.StatusId.Should().Be(notification_EXP.StatusId);
            }
        }

        [Test]
        public void then_catch_exceptions()
        {
            MockGeneratingOrderBooksRepo.Setup(fn => fn.GetDistinctSupplierCodesFromPANDO()).
                Throws( new DomainException("Unable to generate add notification table, database error"));
            //ACT
            Action sut = () => GeneratingOrderbooksService_SUT
            .GenerateOrderbooks(MockDateTime.Object);
            //ASSERT
            sut.Should().NotThrow<Exception>();
        }

        [Test]
        public void then_create_orderbooks_service()
        {
            //ARRANGE
            GeneratingOrderbooksService generatingOrderbooksService_SUT = null;
            //ACT
            generatingOrderbooksService_SUT = new GeneratingOrderbooksService(MockGeneratingOrderBooksRepo.Object);
            //ASSERT         
            generatingOrderbooksService_SUT.Should().NotBeNull();
        }
    }
}

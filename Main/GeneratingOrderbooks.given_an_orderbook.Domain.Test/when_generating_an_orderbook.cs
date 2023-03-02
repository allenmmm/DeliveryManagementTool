using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.SharedKernel;
using DMT.SharedKernel.Interface;
using DMT.SharedKernel.ValueObjects;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SharedKernel.Test;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratingOrderbooks.given_an_orderbook.Domain.Test
{
    [TestFixture]
    class when_generating_an_orderbook : TestBase
    {
        [Test]
        public void then_add_a_new_orderbook_for_a_supplier()
        {
            //ARRANGE     
            //get a bunch of orders for the first supplier
            var suplierId_EXP = FileParser.PlannedAndOverdueOrders.First().SupplierId;
            var suppliers_pando_EXP = FileParser.PlannedAndOverdueOrders.Where(
                                po => po.SupplierId == suplierId_EXP).ToList();

            Supplier supplier = new Supplier(suplierId_EXP,
                                             suppliers_pando_EXP.First().SupplierName);
            var orderbookCount_ACT = supplier.Orderbooks.Count();

            //ACT
            supplier.UpdateOrderbook(suppliers_pando_EXP, MockDateTime.Object);
            //ASSERT
            orderbookCount_ACT.Should().Be(0);
            supplier.Orderbooks.Count().Should().Be(1);
            Compare.OrderbookWithPandO(supplier.Orderbooks.First(), 
                                       suppliers_pando_EXP,
                                       MockDateTime.Object);
        }

        [Test]
        public void then_throw_exception_when_pull_for_new_orderbook_is_older_than_existing_orderbook_for_given_week()
        {
            //ARRANGE     
            DateTime dateTime = DateTime.MinValue;
            var laterFileParser = new PlannedAndOverdueFileParser(dateTime);
            FileReadUtility.FileRead(laterFileParser, "MIS PLY Purchasing Data-201941_091019_115959.csv");
            var laterMockDateTime = new Mock<IDateTime>();
            laterMockDateTime.Setup(fn => fn.GetTime())
                .Returns(laterFileParser.PlannedAndOverdueOrders.First().DatePulled);


            var suplierId_EXP = laterFileParser.PlannedAndOverdueOrders.First().SupplierId;
            var suppliers_pando_EXP = laterFileParser.PlannedAndOverdueOrders.Where(
                                po => po.SupplierId == suplierId_EXP).ToList();

            Supplier supplier_SUT = new Supplier(suplierId_EXP,
                                             suppliers_pando_EXP.First().SupplierName);

            supplier_SUT.UpdateOrderbook(suppliers_pando_EXP, laterMockDateTime.Object);

            //ACT
            Mock<IDateTime> mockCurrentDateTime = new Mock<IDateTime>();
            mockCurrentDateTime.Setup(fn => fn.GetTime()).Returns(DateTime.Now);

            Action act = () => supplier_SUT.UpdateOrderbook(suppliers_pando_EXP, mockCurrentDateTime.Object);
            //ASSERT
            act.Should().Throw<DomainException>();
            supplier_SUT.Orderbooks.Count().Should().Be(1);
            Compare.OrderbookWithPandO(supplier_SUT.Orderbooks.First(),
                                       suppliers_pando_EXP,
                                       laterMockDateTime.Object);
        }

        [Test]
        public void then_add_orderbooks_for_different_weeks_for_a_supplier()
        {
            //ARRANGE
            var firstOrder = FileParser.PlannedAndOverdueOrders.First();
            var firstOrderBookPando_EXP = FileParser.PlannedAndOverdueOrders.Where(
                                po => po.SupplierId == firstOrder.SupplierId).ToList();
            Supplier supplier_SUT = new Supplier(firstOrder.SupplierId,
                                              firstOrder.SupplierName);

            supplier_SUT.UpdateOrderbook(firstOrderBookPando_EXP, MockDateTime.Object);

            //create new order
            var EarlyFileParser = new PlannedAndOverdueFileParser(DateTime.Now);
            FileReadUtility.FileRead(EarlyFileParser, "MIS PLY Purchasing Data-201940_031019_115959.csv");
            var secondOrderBookPando_EXP = EarlyFileParser.PlannedAndOverdueOrders.Where(
                                  po => po.SupplierId == firstOrder.SupplierId).ToList();

            var earlyMockDateTime = new Mock<IDateTime>();
            earlyMockDateTime.Setup(fn => fn.GetTime())
                .Returns(EarlyFileParser.PlannedAndOverdueOrders.First().DatePulled);

            //ACT
            supplier_SUT.UpdateOrderbook(secondOrderBookPando_EXP, earlyMockDateTime.Object);

            //ASSERT
            supplier_SUT.Orderbooks.Count().Should().Be(2);
            Compare.OrderbookWithPandO(supplier_SUT.Orderbooks.ElementAt(0), firstOrderBookPando_EXP, MockDateTime.Object);
            Compare.OrderbookWithPandO(supplier_SUT.Orderbooks.ElementAt(1), secondOrderBookPando_EXP, earlyMockDateTime.Object);
        }

        [Test]
        public void then_update_an_existing_orderbook_for_the_same_orderbook_week_for_a_supplier()
        {
            //ARRANGE
            var firstOrder = FileParser.PlannedAndOverdueOrders.First();
            var firstOrderBookPando_EXP = FileParser.PlannedAndOverdueOrders.Where(
                                po => po.SupplierId == firstOrder.SupplierId).ToList();
            Supplier supplier_SUT = new Supplier(firstOrder.SupplierId,
                                              firstOrder.SupplierName);

            supplier_SUT.UpdateOrderbook(firstOrderBookPando_EXP, MockDateTime.Object);

            //create new order
            DateTime dateTime = DateTime.MinValue;
            var laterFileParser = new PlannedAndOverdueFileParser(dateTime);
            FileReadUtility.FileRead(laterFileParser, "MIS PLY Purchasing Data-201941_091019_115959.csv");

            firstOrder = laterFileParser.PlannedAndOverdueOrders.First();

            var laterMockDateTime = new Mock<IDateTime>();
            laterMockDateTime.Setup(fn => fn.GetTime())
                .Returns(laterFileParser.PlannedAndOverdueOrders.First().DatePulled);

            IEnumerable<PlannedAndOverdueOrder> secondOrderBookPando_EXP = new List<PlannedAndOverdueOrder>()
                 {
                      new PlannedAndOverdueOrder(
                     "uniquenumber", firstOrder.SupplierId, "complete test", firstOrder.PartNumber, firstOrder.PartDescription,
                     firstOrder.PurchaseOrder, firstOrder.POLineItem, firstOrder.POSchedLine, firstOrder.OpenPOQty, firstOrder.ItemDeliveryDate,
                     firstOrder.StatDeliverySchedule, firstOrder.DatePulled)
                };

            //ACT
            supplier_SUT.UpdateOrderbook(secondOrderBookPando_EXP, laterMockDateTime.Object);

            //ASSERT
            supplier_SUT.Orderbooks.Count().Should().Be(1);
            Compare.OrderbookWithPandO(supplier_SUT.Orderbooks.First(), secondOrderBookPando_EXP, laterMockDateTime.Object);
        }

        [Test]
        public void then_throw_exception_when_current_orderbook_week_is_greater_than_pulled_orderbook_week()
        {
            //ARRANGE
            var firstOrder = FileParser.PlannedAndOverdueOrders.First();
            var firstOrderBookPando_EXP = FileParser.PlannedAndOverdueOrders.Where(
                                po => po.SupplierId == firstOrder.SupplierId).ToList();
            Supplier supplier_SUT = new Supplier(firstOrder.SupplierId,
                                              firstOrder.SupplierName);

            var laterMockDateTime = new Mock<IDateTime>();
            laterMockDateTime.Setup(fn => fn.GetTime())
                .Returns(DateTime.Now);
            //ACT
            Action act = () => supplier_SUT.UpdateOrderbook(firstOrderBookPando_EXP, laterMockDateTime.Object);
            //ASSERT
            act.Should().Throw<DomainException>();
            supplier_SUT.Orderbooks.Count().Should().Be(0);

        }

        [Test]
        public void then_create_new_orderbook()
        {
            var suplierId_EXP = FileParser.PlannedAndOverdueOrders.First().SupplierId;
            var suppliers_pando_EXP = FileParser.PlannedAndOverdueOrders.Where(
                    po => po.SupplierId == suplierId_EXP).ToList();
            //ACT
            var orderbook_ACT = new Orderbook(suppliers_pando_EXP,MockDateTime.Object);
            //ASSERT
            Compare.OrderbookWithPandO(orderbook_ACT, suppliers_pando_EXP, MockDateTime.Object);
        }

        private static object[] DateTimes =
        {
            new object[] {new DateTime(2018,12, 31, 11, 59, 59),
                         "2019:01",
                          new DateTime(2018,12,27,12,0,0)},//ACT - Week 1, Monday 11:59:59
            new object[] {new DateTime(2019, 1, 3, 11, 59, 59),
                          "2019:01",
                          new DateTime(2018,12,27,12,0,0)}, //ACT - Week 1, Thursday 11:59:59
            new object[] {new DateTime(2019, 1, 3, 12, 0, 0),
                          "2019:02",
                          new DateTime(2019, 1, 3, 12, 0, 0)}, //ACT - Week 2 Thursday 12:00:00
            new object[] {new DateTime(2019, 12, 19, 12, 0, 0),
                          "2019:52",
                          new DateTime(2019, 12, 19, 12, 0, 0)}, //ACT - Week 52 Thursday 12:00:00
            new object[] { new DateTime(2019, 12, 26, 12, 0, 1),
                           "2020:01",
                           new DateTime(2019, 12, 26, 12, 0, 0)
                         } //ACT - Week 1 2020
        };

        [Test]
        [TestCaseSource("DateTimes")]
        public void then_calculate_orderbook_week(DateTime dateTime, 
                                                  string orderbookIndex_EXP,
                                                  DateTime startOfOrderbookWeek)
        {
            var orderbookIndex_ACT = OrderbookWeek_VO.Create(dateTime);
            //ASSERT
            orderbookIndex_ACT.FormatString().Should().Be(orderbookIndex_EXP);
            orderbookIndex_ACT.StartOfOrderBookWeek.Should().Be(startOfOrderbookWeek);
        }

        [Test]
        public void then_calculate_orderbook_index()
        {
            //ACT
            ulong orderbookIndex_ACT = Orderbook.CalculateOrderbookId(new DateTime(2018, 12, 31, 11, 59, 59), 123456);
            //ASSERT
            orderbookIndex_ACT.Should().Be(123456201901);
        }
    }
}

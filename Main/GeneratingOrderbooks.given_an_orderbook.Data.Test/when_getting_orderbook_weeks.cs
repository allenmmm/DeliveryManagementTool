using DMT.GeneratingOrderBooks.Data;
using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.SharedKernel.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratingOrderbooks.given_an_orderbook.Data.Test
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
            var pando_EXP = fileParser.PlannedAndOverdueOrders.First();
            var ordersPerSupplier_EXP = fileParser.PlannedAndOverdueOrders
                                        .Where(si => si.SupplierId == pando_EXP.SupplierId).ToList();

            using (var repo_SUT = 
                    new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                         (new DbContextOptionsBuilder<DbContext>()
                                                             .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                Supplier supplier = new Supplier(pando_EXP.SupplierId, pando_EXP.SupplierName);
                supplier.UpdateOrderbook(ordersPerSupplier_EXP, MockDateTime.Object);
                repo_SUT.SaveSupplier(supplier);
            }

            using (var repo_SUT = 
                new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                     (new DbContextOptionsBuilder<DbContext>()
                                                        .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                //ACT
                var orderbookWeeks_ACT = repo_SUT.GetOrderbookWeeks();
                //ASSERT
                orderbookWeeks_ACT.Count().Should().Be(1);
                orderbookWeeks_ACT.First().Should()
                    .Be(OrderbookWeek_VO.Create(MockDateTime.Object.GetTime()).FormatString());
            }
        }

        [Test]
        public void then_retrieve_empty_orderbook_weeks_if_no_orderbooks()
        {   
            //ARRANGE
            using (var repo_SUT = 
                new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                     (new DbContextOptionsBuilder<DbContext>()
                                                        .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                //ACT
                var orderbookWeeks_ACT = repo_SUT.GetOrderbookWeeks();
                //ASSERT
                orderbookWeeks_ACT.Should().BeEmpty();
            }
        }

        [Test]
        public void then_retrieve_orderbook_weeks_confirm_dates_descending()
        {
            //ARRANGE
            List<DateTime> dateTimes_EXP = new List<DateTime>()
            {
               new DateTime(2019,03,08),
               new DateTime(2019,05,23),
               new DateTime(2019,09,04)
            };

            var pando_EXP = fileParser.PlannedAndOverdueOrders.First();
            var ordersPerSupplier_EXP = fileParser.PlannedAndOverdueOrders
                                        .Where(si => si.SupplierId == pando_EXP.SupplierId).ToList();

            foreach (var date in dateTimes_EXP)
            {
                MockDateTime.Setup(fn => fn.GetTime()).Returns(date);
                var context = new GeneratingOrderbooksContext(new DbContextOptionsBuilder<DbContext>()
                                                                .UseInMemoryDatabase(databaseName: conn).Options);
           
                using (var repo_SUT = new GeneratingOrderbooksRepository(context))
                {
                    repo_SUT.DeletePlannedAndOverdues();
                    Seed(context, date);
                    var Orders = repo_SUT.GetOrdersPerSupplierFromPANDO(pando_EXP.SupplierId);

                    Supplier supplier = new Supplier(pando_EXP.SupplierId, pando_EXP.SupplierName);
                    supplier.UpdateOrderbook(Orders, MockDateTime.Object);
                    repo_SUT.SaveSupplier(supplier);
                }

                using (var repo_SUT = new GeneratingOrderbooksRepository(context))
                {
                    repo_SUT.DeletePlannedAndOverdues();
                }
            }

            using (var repo_SUT = 
                new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                     (new DbContextOptionsBuilder<DbContext>()
                                                         .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                //ACT
                var orderbookWeeks_ACT = repo_SUT.GetOrderbookWeeks();
                //ASSERT
                orderbookWeeks_ACT.Count().Should().Be(dateTimes_EXP.Count());
                int i = dateTimes_EXP.Count()-1;
                foreach(var dateTime_EXP in dateTimes_EXP)
                {
                    orderbookWeeks_ACT.ElementAt(i--).Should().
                        Be(OrderbookWeek_VO.Create(dateTime_EXP).FormatString());
                }
            }
        }
    }
}

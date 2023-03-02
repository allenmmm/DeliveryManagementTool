using DMT.GeneratingOrderBooks.Data;
using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.SharedKernel.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SharedKernel.Test;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace GeneratingOrderbooks.given_an_orderbook.Data.Test
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
            var distinctVendors = fileParser.PlannedAndOverdueOrders
                .Select(si => si.SupplierId).Distinct();
            var orderbookWeek_EXP = OrderbookWeek_VO.Create(MockDateTime.Object.GetTime()).FormatString();
            List<Supplier> suppliersWithOrderbooks_EXP = new List<Supplier>();

            foreach (var vendor in distinctVendors)
            {
                var ordersPerSupplier_EXP = fileParser.PlannedAndOverdueOrders
                    .Where(si => si.SupplierId == vendor).ToList();

                using (var repo_SUT = 
                        new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                             (new DbContextOptionsBuilder<DbContext>()
                                                                .UseInMemoryDatabase(databaseName: conn).Options)))
                {
                    Supplier supplier = new Supplier(vendor, ordersPerSupplier_EXP.First().SupplierName);
                    supplier.UpdateOrderbook(ordersPerSupplier_EXP, MockDateTime.Object);
                    suppliersWithOrderbooks_EXP.Add(supplier);
                    repo_SUT.SaveSupplier(supplier);
                }
            }
            //ACT
            using (var repo_SUT = 
                new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                     (new DbContextOptionsBuilder<DbContext>()
                                                         .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                var suppliersWithOrderbooks_ACT = repo_SUT.GetSuppliersWithOrderbookForWeek(orderbookWeek_EXP);
                //ASSERT
                //cant gaurantee the order so will have to find
                suppliersWithOrderbooks_ACT.Count().Should().Be(suppliersWithOrderbooks_EXP.Count);

                foreach(var supplier_ACT in suppliersWithOrderbooks_ACT)
                {
                    var Supplier_EXP = suppliersWithOrderbooks_EXP
                        .Where(sid => sid.Id == supplier_ACT.Id).First();
                    supplier_ACT.Id.Should().Be(Supplier_EXP.Id);
                    supplier_ACT.Details.Should().Be(Supplier_EXP.Details);
                    supplier_ACT.Orderbooks.Count().Should().Be(1);
                    supplier_ACT.Orderbooks.First().Id.Should().Be(Supplier_EXP.Orderbooks.First().Id);
                    supplier_ACT.Orderbooks.First().DateStamp.Should()
                        .Be(Supplier_EXP.Orderbooks.First().DateStamp);
                    supplier_ACT.Orderbooks.First().SupplierId.Should()
                        .Be(Supplier_EXP.Orderbooks.First().SupplierId);
                    supplier_ACT.Orderbooks.First().Orders.Should().BeEmpty();    
                }
            }
        }
    }
}

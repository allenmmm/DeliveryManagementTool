using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.GeneratingOrderBooks.Domain.ValueObjects;
using DMT.SharedKernel;
using FluentAssertions;
using NUnit.Framework;
using SharedKernel.Test;
using System;
using System.Linq;


namespace GeneratingOrderbooks.given_an_orderbook.Domain.Test
{
    [TestFixture]
    public class when_registering_a_supplier : TestBase
    {
        [Test]
        public void then_populate_supplier()
        {
            //ARRANGE
            var supplierId_EXP = FileParser.PlannedAndOverdueOrders.First();
            //ACT
            var supplier_SUT = new Supplier(FileParser.PlannedAndOverdueOrders.First().SupplierId,
                                            FileParser.PlannedAndOverdueOrders.First().SupplierName);
            //ASSERT
            Compare.SupplierWithPandO(supplier_SUT, FileParser.PlannedAndOverdueOrders.First());
            supplier_SUT.Orderbooks.Count().Should().Be(0);
        }
    }
}

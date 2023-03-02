using DMT.GeneratingOrderBooks.Data;
using DMT.GeneratingOrderBooks.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SharedKernel.Test;
using System.Linq;

namespace GeneratingOrderbooks.given_an_orderbook.Data.Test
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
            var firstPandO = fileParser.PlannedAndOverdueOrders.First();
            var pandos_EXP = fileParser.PlannedAndOverdueOrders
                                .Where(pando => pando.SupplierId == firstPandO.SupplierId);
            var orderbookID = Orderbook.CalculateOrderbookId(MockDateTime.Object.GetTime(),
                                                             firstPandO.SupplierId);
            Supplier supplier_EXP;
            using (var repo_SUT =
                    new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                            (new DbContextOptionsBuilder<DbContext>()
                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                supplier_EXP = new Supplier(firstPandO.SupplierId, firstPandO.SupplierName);
                supplier_EXP.AddOrderBook(new Orderbook(pandos_EXP, MockDateTime.Object));
                repo_SUT.SaveSupplier(supplier_EXP);
            }

            //ACT
            using (var repo_SUT =
                new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                     (new DbContextOptionsBuilder<DbContext>()
                                                         .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                var supplierWithOrderbook_ACT = repo_SUT.GetSupplierWithOrderbookAndOrders(orderbookID);

                //ASSERT
                supplierWithOrderbook_ACT.Id.Should().Be(supplier_EXP.Id);
                supplierWithOrderbook_ACT.Details.Should().Be(supplier_EXP.Details);
                supplierWithOrderbook_ACT.Orderbooks.Count().Should().Be(1);
                Compare.OrderbookWithPandO(supplierWithOrderbook_ACT.Orderbooks.First(), pandos_EXP, MockDateTime.Object);
            }
        }
    }
}

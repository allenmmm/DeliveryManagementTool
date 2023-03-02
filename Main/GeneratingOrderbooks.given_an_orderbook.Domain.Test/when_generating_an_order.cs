using DMT.GeneratingOrderBooks.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using SharedKernel.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratingOrderbooks.given_an_orderbook.Domain.Test
{
    [TestFixture]
    class when_generating_an_order : TestBase
    {
        [Test]
        public void then_create_order()
        {
            //ARRANGE
            var FirstOrder = FileParser.PlannedAndOverdueOrders.First();
            //ACT
            var Order_ACT = new Order(FirstOrder);
            //ASSERT
            Compare.OrderDetailsWithPandO(Order_ACT.Details, FirstOrder);
         }
    }
}

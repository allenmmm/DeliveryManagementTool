using DMT.GeneratingOrderBooks.Domain.ValueObjects;
using DMT.SharedKernel;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;

namespace GeneratingOrderbooks.given_an_orderbook.Domain.Test
{
    [TestFixture]
    class when_populating_an_order : TestBase
    {
        [Test]
        public void then_create_order_when_valid()
        {
            var order_EXP = FileParser.PlannedAndOverdueOrders.First();
            //ACT
            var order_ACT = Order_VO.Create(order_EXP);
            //ASSERT
            order_ACT.ItemDeliveryDate.Should().Be(order_EXP.ItemDeliveryDate);
            order_ACT.POLineItem.Should().Be(order_EXP.POLineItem);
            order_ACT.POSchedLine.Should().Be(order_EXP.POSchedLine);
            order_ACT.PurchaseOrder.Should().Be(order_EXP.PurchaseOrder);
            order_ACT.StatDeliverySchedule.Should().Be(order_EXP.StatDeliverySchedule);
        }

        [Test]
        public void then_throw_exception_when_order_vo_is_null()
        {
            //ACT         
            Action actionSut = () => Supplier_VO.Create(null);
            //ASSERT
            actionSut.Should().Throw<DomainException>();
        }

    }
}

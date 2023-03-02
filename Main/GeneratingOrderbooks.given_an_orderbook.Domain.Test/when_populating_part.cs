using DMT.GeneratingOrderBooks.Domain.ValueObjects;
using DMT.SharedKernel;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;

namespace GeneratingOrderbooks.given_an_orderbook.Domain.Test
{
    [TestFixture]
    class when_populating_part : TestBase
    {
        [Test]
        public void then_create_part_when_valid()
        {
            var order_EXP = FileParser.PlannedAndOverdueOrders.First();
            //ACT
            var part = Part_VO.Create(order_EXP);
            //ASSERT
            part.Description.Should().Be(order_EXP.PartDescription);
            part.Number.Should().Be(order_EXP.PartNumber);
        }

        [Test]
        public void then_throw_exception_when_part_empty()
        {
            //ACT         
            Action actionSut = () => Part_VO.Create(null);
            //ASSERT
            actionSut.Should().Throw<DomainException>();
        }
    }
}

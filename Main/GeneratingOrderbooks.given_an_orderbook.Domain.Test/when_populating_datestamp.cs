using DMT.GeneratingOrderBooks.Domain.ValueObjects;
using DMT.SharedKernel;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace GeneratingOrderbooks.given_an_orderbook.Domain.Test
{
    [TestFixture]
    public class when_populating_datestamp
    {
        [Test]
        public void then_create_datestamp_when_valid()
        {
            //ARRANGE
            DateTime dateTime_EXP = new DateTime(2019, 9, 5, 11, 59, 59);
            //ACT
            DateStamp_VO dateStamp_SUT = DateStamp_VO.Create(dateTime_EXP, dateTime_EXP);
            //ASSERT
            dateStamp_SUT.DateCreated.Should().Be(dateTime_EXP);
            dateStamp_SUT.DatePulled.Should().Be(dateTime_EXP);
            dateStamp_SUT.OrderbookWeek.Should().Be("2019:36");
        }

        [Test]
        public void then_throw_exception_when_date_created_and_pulled_not_equal_to_same_orderbook_week()
        {
            DateTime dateTimePulledEarlyOrderbook = new DateTime(2019,9,5,11,59,59); //Thursday pre
            DateTime dateTimeCreatedLateOrderbook = new DateTime(2019, 9, 5, 12, 00, 00); //Thursday pulled
            //ACT
            Action act = () => DateStamp_VO.Create(dateTimePulledEarlyOrderbook, dateTimeCreatedLateOrderbook);
            //ASSERT
            act.Should().Throw<DomainException>();
        }
    }
}

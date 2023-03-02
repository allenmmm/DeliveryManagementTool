using DMT.ManagingNotifications.Domain.ValueObjects;
using DMT.SharedKernel;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace ManagingNotifications.given_a_notification.Domain.Test
{
    [TestFixture]
    class when_populating_notification
    {
        [Test]
        public void then_create_notification()
        {
            //ARRANGE
            Guid LookUpID_EXP = Guid.NewGuid();
            String customMessage_EXP = "Orderbook for week 41 created";
            DateTime notificationRaised_EXP = DateTime.Now;
            //ACT
            var notification_ACT = Notification_VO.Create(LookUpID_EXP, notificationRaised_EXP, customMessage_EXP );
            //ASSERT 
            notification_ACT.CustomMessage.Should().Be(customMessage_EXP);
            notification_ACT.DateRaised.Should().Be(notificationRaised_EXP);
            notification_ACT.StatusId.Should().Be(LookUpID_EXP); 
        }

        [Test]
        public void then_create_notification_from_existing()
        {
            //ARRANGE
            Guid LookUpID_EXP = Guid.NewGuid();
            String customMessage_EXP = "Orderbook for week 41 created";
            DateTime notificationRaised_EXP = DateTime.Now;
            var notification_EXP = Notification_VO.Create(LookUpID_EXP, notificationRaised_EXP, customMessage_EXP);
            //ACT
            var notification_ACT = new Notification_VO(notification_EXP);
            //ASSERT 
            notification_ACT.CustomMessage.Should().Be(customMessage_EXP);
            notification_ACT.DateRaised.Should().Be(notificationRaised_EXP);
            notification_ACT.StatusId.Should().Be(LookUpID_EXP);
        }

        [Test]
        public void then_thrown_exception_when_notifcation_invalid()
        {
            //ARRANGE
            Guid LookUpID_EXP = Guid.NewGuid();
  
            DateTime notificationRaised_EXP = DateTime.Now;
            //ACT
            Action action_SUT = () => Notification_VO.Create(LookUpID_EXP, notificationRaised_EXP, null);
            //ASSERT 
            action_SUT.Should().Throw<DomainException>();
        }
    }
}

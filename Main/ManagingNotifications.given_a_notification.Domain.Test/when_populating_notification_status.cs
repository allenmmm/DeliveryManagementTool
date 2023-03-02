using DMT.ManagingNotifications.Domain.ValueObjects;
using DMT.SharedKernel;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace ManagingNotifications.given_a_notification.Domain.Test
{
    [TestFixture]
    public class when_populating_notification_status
    {
        [Test]
        public void then_create_notification_summary()
        {
            //ARRANGE
            String description = "Scheduled Orderbooks Generated Successfully";
            NotificationState notificationState = NotificationState.Success;
            //ACT
            var status_ACT = Status_VO.Create(description, notificationState);
            //ASSERT
            status_ACT.Description.Should().Be(description);
            status_ACT.NotificationState.Should().Be(notificationState);
            status_ACT.Should().BeOfType<Status_VO>();
        }

        [Test]
        public void then_throw_exception_when_notification_detail_invalid()
        {
            //ARRANGE
            String description = null;
            NotificationState notificationState = NotificationState.Success;
            //ACT
            Action actionSut = () => Status_VO.Create(description, notificationState);
            //ASSERT
            actionSut.Should().Throw<DomainException>();
        }
    }
}

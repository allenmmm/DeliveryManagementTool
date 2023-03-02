using DMT.ManagingNotifications.Domain.Entities;
using DMT.ManagingNotifications.Domain.ValueObjects;
using DMT.SharedKernel;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingNotifications.given_a_notification.Domain.Test
{
    [TestFixture]
    class when_generating_notification_status
    {
        [Test]
        public void then_create_notification_summary()
        {
            //ARRANGE
            Guid id_EXP = Guid.NewGuid();
            String description_EXP = "Scheduled Orderbooks Generated Successfully";
            NotificationState notificationState_EXP = NotificationState.Success;
            //ACT
            var status_ACT = new Status(id_EXP, Status_VO.Create( description_EXP, notificationState_EXP));
            //ASSERT
            status_ACT.Id.Should().Be(id_EXP);
            status_ACT.Description.Should().Be(description_EXP);
            status_ACT.NotificationState.Should().Be(notificationState_EXP);
        }
    }
}

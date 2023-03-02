using DMT.ManagingNotifications.Domain.ValueObjects;
using DMT.SharedKernel;
using FluentAssertions;
using ManagingNotifications.Domain.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingNotifications.given_a_notification.Domain.Test
{
    [TestFixture]
    class when_generating_notification
    {
        [Test]
        public void then_create_notification_summary()
        {
            //ARRANGE
            Guid id_EXP = Guid.NewGuid();
            DateTime dateRaised_EXP = DateTime.Now;
            String description_EXP = "Scheduled Orderbooks Generated Successfully";
            //ACT
            var notification_ACT = new Notification(id_EXP, dateRaised_EXP, description_EXP);
            //ASSERT
            notification_ACT.Detail.StatusId.Should().Be(id_EXP);
            notification_ACT.Detail.CustomMessage.Should().Be(description_EXP);
            notification_ACT.Detail.DateRaised.Should().Be(dateRaised_EXP);
        }
    }
}

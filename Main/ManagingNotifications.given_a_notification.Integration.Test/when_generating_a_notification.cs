using DMT.ManagingNotifications.Data;
using DMT.ManagingNotifications.Service;
using DMT.SharedKernel;
using FluentAssertions;
using ManagingNotifications.Domain.Entities;
using NUnit.Framework;
using System;
using System.Linq;
namespace ManagingNotifications.given_a_notification.Integration.Test
{
    [TestFixture]
    public class when_generating_a_notification : TestBase
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void then_add_notification()
        {
            //ARRANGE
            using (var repo = new ManagingNotificationsRepository(ManagingNotificationsContext))
            {

                Notification notification = new Notification(NotificationCodes.OrderbookGeneratedOK,
                                                             DateTime.Now,
                                                             "Ok");
                repo.AddNotification(notification);
                //ACT
                ManagingNotificationsService mns_SUT = new ManagingNotificationsService(repo);
                //ASSERT
                var notifications_ACT = mns_SUT.GetNotifications();
                notifications_ACT.Count.Should().Be(1);
                notifications_ACT.First().Description.Should().Be("Orderbook generated (" + notification.Detail.CustomMessage + ")");
                notifications_ACT.First().NotificationState.Should().Be(NotificationState.Success);
                notifications_ACT.First().DateRaised.Should().BeOnOrAfter(notification.Detail.DateRaised);
            }
        }

        [Test]
        public void then_catch_exception_when_status_invalid()
        {
            //ARRANGE
            using (var repo = new ManagingNotificationsRepository(null))
            {
                DateTime dateTime = DateTime.Now;
                //ACT
                ManagingNotificationsService mns_SUT = new ManagingNotificationsService(repo);
                //ASSERT
                var notifications_ACT = mns_SUT.GetNotifications();
                notifications_ACT.Count.Should().Be(1);
                notifications_ACT.First().Description.Should().Be("Unexpected error whilst retrieving notifications (Object reference not set to an instance of an object.)");
                notifications_ACT.First().NotificationState.Should().Be(NotificationState.Error);
                notifications_ACT.First().DateRaised.Should().BeOnOrAfter(dateTime);
            }
        }
    }
}

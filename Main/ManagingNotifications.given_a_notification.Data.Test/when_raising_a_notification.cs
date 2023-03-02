using DMT.ManagingNotifications.Data;
using DMT.SharedKernel;
using FluentAssertions;
using ManagingNotifications.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SharedKernel.Test;
using System;
using System.Linq;

namespace ManagingNotifications.given_a_notification.Data.Test
{
    [TestFixture]
    public class when_raising_a_notification : TestBase
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void then_store_and_retrieve_notification()
        {
            //ARRANGE
            Guid LookUpID_EXP = NotificationCodes.OrderbooksGeneratedOK;
            String customMessage_EXP = "Orderbook for week 41 created";
            DateTime notificationRaised_EXP = DateTime.Now;
            var notification_EXP = new Notification(LookUpID_EXP, notificationRaised_EXP, customMessage_EXP);
            int currentNotifications = 1;

            using (var repo_SUT = new ManagingNotificationsRepository(new ManagingNotificationsContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                currentNotifications = repo_SUT.GetNotifications().Count();
                //ACT
                repo_SUT.AddNotification(notification_EXP);

            }
            //ASSERT
            using (var repo_SUT = new ManagingNotificationsRepository(new ManagingNotificationsContext
                                                            (new DbContextOptionsBuilder<DbContext>()
                                                                .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                var notifications_ACT = repo_SUT.GetNotifications();
                currentNotifications.Should().Be(0);
                notifications_ACT.Count().Should().Be(1);
                Compare.CompareItem(notifications_ACT.First(), notification_EXP);
            }
        }


        [Test]
        public void then_throw_exception_when_status_code_invalid_when_adding_notification()
        {
            Guid LookUpID_EXP = default(Guid);
            String customMessage_EXP = "Orderbook for week 41 created";
            DateTime notificationRaised_EXP = DateTime.Now;
            var notification_EXP = new Notification(LookUpID_EXP, notificationRaised_EXP, customMessage_EXP);
            using (var repo_SUT = new ManagingNotificationsRepository(new ManagingNotificationsContext
                                                            (new DbContextOptionsBuilder<DbContext>()
                                                                .UseInMemoryDatabase(databaseName: conn).Options)))
            { 
                //ACT
                Action action_SUT = () => repo_SUT.AddNotification(notification_EXP);
                //ASSERT
                action_SUT.Should().Throw<DomainException>();
            }
        }
    }    
}

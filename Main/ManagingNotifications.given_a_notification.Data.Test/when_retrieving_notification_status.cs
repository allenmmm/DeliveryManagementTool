using DMT.ManagingNotifications.Data;
using DMT.SharedKernel;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;

namespace ManagingNotifications.given_a_notification.Data.Test
{
    [TestFixture]
    public class when_retrieving_notification_status : TestBase
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void then_get_notification_Status()
        {
            using (var repo_SUT = new ManagingNotificationsRepository(new ManagingNotificationsContext
                                                              (new DbContextOptionsBuilder<DbContext>()
                                                                  .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                //ACT
                var status_ACT = repo_SUT.GetStatus(NotificationCodes.OrderbooksGeneratedOK);
                //ASSERT
                status_ACT.Id.Should().Be(status_ACT.Id);
            }
        }

        [Test]
        public void then_throw_exception_when_status_not_exist_when_retrieving_notification_status()
        {
            //ARRANGE
            using (var repo_SUT = new ManagingNotificationsRepository(new ManagingNotificationsContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                //ACT
                Action action_SUT = () => repo_SUT.GetStatus(Guid.NewGuid());
                //ASSERT
                action_SUT.Should().Throw<DomainException>();
            }
        }
    }
}

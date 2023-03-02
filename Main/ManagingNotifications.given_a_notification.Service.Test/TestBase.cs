using DMT.ManagingNotifications.Domain.Interfaces;
using DMT.ManagingNotifications.Service;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingNotifications.given_a_notification.Service.Test
{
    public abstract class TestBase
    {
        protected Mock<IManagingNotificationsRepo> MockManagingNotificationsRepo;
        protected ManagingNotificationsService ManagingNotificationsService_SUT;
        public TestBase()
        {

        }
        protected virtual void SetUp()
        {
            MockManagingNotificationsRepo = new Mock<IManagingNotificationsRepo>();
            ManagingNotificationsService_SUT = new ManagingNotificationsService(MockManagingNotificationsRepo.Object);
        }
    }
}

using DMT.ManagingNotifications.Service.Interfaces;
using DMT.Web.Controllers;
using Moq;
using MvcContrib.TestHelper;

namespace ManagingNotifications.given_a_notification.Presentation.Test
{
    public abstract class TestBase
    {
        protected Mock<IManagingNotificationsService> MockManagingNotificationsService;
        protected ManagingNotificationsController ManagingNotificationsController_SUT;

        protected virtual void SetUp()
        {
            MockManagingNotificationsService = new Mock<IManagingNotificationsService>();
            TestControllerBuilder testControllerBuilder = new TestControllerBuilder();
            ManagingNotificationsController_SUT = testControllerBuilder.CreateController<ManagingNotificationsController>(MockManagingNotificationsService.Object);
        }
    }
}
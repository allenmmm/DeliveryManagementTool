using DMT.DTO;
using DMT.ManagingNotifications.EventService;
using DMT.SharedKernel;
using DMT.Web.EventService;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ManagingNotifications.given_a_notification.Presentation.Test
{
    [TestFixture]
    public class when_managing_notifications : TestBase
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void then_view_all_notifications()
        {
            //ARRANGE 
            //TODO: Create list of expected notifications
            //      Mock service layer to return list of expected notifications
            IReadOnlyList<NotificationDTO> expectedNotifications = new List<NotificationDTO>
            {
                new NotificationDTO(DateTime.Now,
                                    "success description",
                                    NotificationState.Success),
                new NotificationDTO(DateTime.Now,
                                    "warning description",
                                    NotificationState.Warning),
                new NotificationDTO(DateTime.Now,
                                    "error description",
                                    NotificationState.Error)
            };
            MockManagingNotificationsService.Setup(fn => fn.GetNotifications())
                                            .Returns(expectedNotifications);

            //ACT 
            //TODO: Call the view in the ManageNotificationsController
            var actualActionResult = ManagingNotificationsController_SUT.GetNotifications();

            //ASSERT: 
            //TODO: Verify correct method is called
            //      Verify correct types are returned
            actualActionResult.Should().BeOfType<PartialViewResult>();

            var actualView = (PartialViewResult)actualActionResult;
            actualView.Model.Should().BeOfType<List<NotificationDTO>>();

            var model = actualView.Model as IReadOnlyList<NotificationDTO>;
            model.Count.Should().Be(expectedNotifications.Count);
            MockManagingNotificationsService.Verify(fn => fn.GetNotifications(), Times.Once());
        }

        [Test]
        public void then_handle_notifications_updated_event()
        {
            //ARRANGE
            NotificationDTO notificationDTO_EXP = new NotificationDTO(DateTime.Now, "desc", NotificationState.Information);
            NotificationsUpdatedEvent notificationEvent_EXP = new NotificationsUpdatedEvent(notificationDTO_EXP);
            NotificationsUpdatedHandler notificationsUpdatedHandler_SUT = new NotificationsUpdatedHandler();
            //ACT
            Action act = () => notificationsUpdatedHandler_SUT.Handle(notificationEvent_EXP);
            //ASSERT
            act.Should().NotThrow<Exception>();
        }
    }
}
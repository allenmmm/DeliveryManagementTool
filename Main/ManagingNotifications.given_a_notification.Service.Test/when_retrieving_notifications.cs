using DMT.DTO;
using DMT.ManagingNotifications.Domain.Entities;
using DMT.ManagingNotifications.Domain.Interfaces;
using DMT.ManagingNotifications.Domain.ValueObjects;
using DMT.ManagingNotifications.Service;
using DMT.SharedKernel;
using FluentAssertions;
using ManagingNotifications.Domain.Entities;
using Moq;
using NUnit.Framework;
using SharedKernel.Test;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ManagingNotifications.given_a_notification.Service.Test
{
    [TestFixture]
    public class when_retrieving_notifications : TestBase
    {
        [Test]
        public void then_get_all_notifications()
        {
            //ARRANGE
            NotificationDTO successNotificationDTO =
                new NotificationDTO(new DateTime(2019, 4, 24),
                                     "Scheduled order books successfully (Orderbooks generated for week 41)",
                                     NotificationState.Success);

            List<NotificationDTO> notificationDTOs_EXP = new List<NotificationDTO>()
            {
                new NotificationDTO(successNotificationDTO ),
            };


            List<Notification> notifications_EXP = new List<Notification>()
            {
                new Notification(NotificationCodes.OrderbooksGeneratedOK,
                                                        new DateTime(2019, 4, 24),
                                                        "Orderbooks generated for week 41"),
            };

            Mock<IManagingNotificationsRepo> MockManagingNotificationsRepo =
                new Mock<IManagingNotificationsRepo>();

            ManagingNotificationsService ManagingNotificationsService_SUT = new
                ManagingNotificationsService(MockManagingNotificationsRepo.Object);

            MockManagingNotificationsRepo.Setup(fn => fn.GetNotifications())
               .Returns(notifications_EXP);

            MockManagingNotificationsRepo.Setup(fn => fn.GetStatus(It.IsAny<Guid>()))
                .Returns(new Status(NotificationCodes.OrderbooksGeneratedOK,
                                    Status_VO.Create("Scheduled order books successfully",
                                                         NotificationState.Success)));

            //ACT
            var notificationsDTO_ACT = ManagingNotificationsService_SUT.GetNotifications();

            //ASSERT
            MockManagingNotificationsRepo.Verify(fn => fn.GetNotifications(), Times.Once);
            MockManagingNotificationsRepo.Verify(fn => fn.GetStatus(It.IsAny<Guid>()),Times.Once);
            Compare.CompareItem(notificationsDTO_ACT.First(), notificationDTOs_EXP.First());
        }

        [Test]
        public void then_convert_notification_and_status_to_notification_dto()
        {
            //ARRANGE
            //set up the notification input
            Guid ScheduledOrderBooksGeneratedOK = Guid.NewGuid();


            Notification notification_EXP = new Notification(ScheduledOrderBooksGeneratedOK,
                                                     new DateTime(2019, 4, 24),
                                                     "Orderbooks generated for week 41");

            Status Status_EXP = new Status(ScheduledOrderBooksGeneratedOK,
                                           Status_VO.Create("Scheduled order books successfully",
                                           NotificationState.Success));

            NotificationDTO notificationDTO_EXP = new NotificationDTO(notification_EXP.Detail.DateRaised,
                                                                 Status_EXP.Description + " (" + notification_EXP.Detail.CustomMessage + ")",
                                                                 NotificationState.Success);

            //ACT
            var notificationsDTO_ACT = DTOConversion.ConvertNotificationAndStatusToNotificationDTO(notification_EXP, Status_EXP);
            //ASSERT
            Compare.CompareItem(notificationsDTO_ACT, notificationDTO_EXP);
        }

        [Test]
        public void then_raise_notification_when_exception_during_notification_retrieval()
        {
            //ARRANGE
            NotificationDTO notificationDTO_EXP = new NotificationDTO(DateTime.Now,
                    "Unexpected error whilst retrieving notifications (Domain Exception - Unable to read table, database error)", 
                    NotificationState.Error);
            Mock<IManagingNotificationsRepo> MockManagingNotificationsRepo =
                    new Mock<IManagingNotificationsRepo>();

            ManagingNotificationsService ManagingNotificationsService_SUT = new
                ManagingNotificationsService(MockManagingNotificationsRepo.Object);

            MockManagingNotificationsRepo.Setup(fn => fn.GetNotifications()).
                Throws(new DomainException("Unable to read table, database error"));
            //ACT
            var notificationsDTO_ACT = ManagingNotificationsService_SUT.GetNotifications();
            //ASSERT
            notificationsDTO_ACT.Count().Should().Be(1);
            notificationsDTO_ACT.First().NotificationState.Should().Be(notificationDTO_EXP.NotificationState);
            notificationsDTO_ACT.First().Description.Should().Be(notificationDTO_EXP.Description);
            notificationsDTO_ACT.First().DateRaised.Should().BeOnOrAfter(notificationDTO_EXP.DateRaised);
        }

        [Test]
        public void then_catch_exceptions()
        {
            Mock<IManagingNotificationsRepo> MockManagingNotificationsRepo =
                new Mock<IManagingNotificationsRepo>();
            MockManagingNotificationsRepo.Setup(fn => fn.GetNotifications()).
                Throws( new DomainException("Unable to read table, database error"));
            ManagingNotificationsService ManagingNotificationsService_SUT = new
                 ManagingNotificationsService(MockManagingNotificationsRepo.Object);

            //ACT
            Action action_SUT = () => ManagingNotificationsService_SUT.GetNotifications();
            //ASSERT
            action_SUT.Should().NotThrow<Exception>();
        }
    }
}

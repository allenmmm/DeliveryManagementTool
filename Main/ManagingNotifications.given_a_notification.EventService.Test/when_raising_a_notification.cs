using DMT.ManagingNotifications.Data;
using DMT.ManagingNotifications.Domain.Entities;
using DMT.ManagingNotifications.EventService;
using DMT.SharedKernel;
using DMT.SharedKernel.ValueObjects;
using FluentAssertions;
using ManagingNotifications.Domain.Entities;
using NUnit.Framework;
using System;
using System.Linq;

namespace ManagingNotifications.given_a_notification.EventService.Test
{
    [TestFixture]
    public class when_raising_a_notification
    {
        [Test]
        public void then_create_notification_event()
        {
            //ARRANGE
            Notification notification_EXP = 
                new Notification(NotificationCodes.PandOInvalidTableAccess,
                                    DateTime.Now, 
                                    "Event Happened");
            //ACT
            NotificationRaisedEvent notificationEvent_ACT =
                new NotificationRaisedEvent(notification_EXP.Detail.StatusId,
                                            notification_EXP.Detail.DateRaised,
                                            notification_EXP.Detail.CustomMessage);
            //ASSERT
            notificationEvent_ACT.DateTimeEventFired.Should().Be(notification_EXP.Detail.DateRaised);
            notificationEvent_ACT.CustomMessage.Should().Be(notification_EXP.Detail.CustomMessage);
            notificationEvent_ACT.StatusId.Should().Be(notification_EXP.Detail.StatusId);
        }

        [Test]
        public void then_append_orderbook_week_to_custom_message_if_required()
        {
            //ARRANGE
            Notification notification_EXP =
                new Notification(NotificationCodes.OrderbooksGeneratedOK, 
                                    DateTime.Now, 
                                    "Event Happened");
            //ACT
            NotificationRaisedEvent notificationEvent_ACT = 
                new NotificationRaisedEvent(notification_EXP.Detail.StatusId,
                                            notification_EXP.Detail.DateRaised);
            //ASSERT
            notificationEvent_ACT.DateTimeEventFired.Should().Be(notification_EXP.Detail.DateRaised);
            notificationEvent_ACT.StatusId.Should().Be(notification_EXP.Detail.StatusId);
            notificationEvent_ACT.CustomMessage.Should()
                .Be("Week " + OrderbookWeek_VO.Create(DateTime.Now).FormatString());
        }

        [Test]
        public void then_handle_notification_raised_event()
        {
            /*  Note I would usually inject the context into the handle so I can test independently, however that would mean 
             *  the bounded context raising the event would have to know about the context in which the event will be caught.
             *  That defeats the object and that is my justification.
             *  
             *  Therefore this test we are using a real database purely for this test
             */
            //ARRANGE
            Notification notification_EXP = 
                new Notification(NotificationCodes.PandOInvalidTableAccess, 
                                    DateTime.Now, 
                                    "Event Happened");
            NotificationRaisedEvent notificationEvent_EXP = 
                new NotificationRaisedEvent(notification_EXP.Detail.StatusId,
                                            notification_EXP.Detail.DateRaised, 
                                            notification_EXP.Detail.CustomMessage);
            NotificationRaisedHandler notificationHandler = new NotificationRaisedHandler();
            NotificationsUpdatedEvent notificationUpdatedEvent_ACT = null;
            Status status_EXP = null;
            using (var context = new ManagingNotificationsContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                using (var repo = new ManagingNotificationsRepository(context))
                {
                    DomainEvents.Register<NotificationsUpdatedEvent>(ev => notificationUpdatedEvent_ACT = ev);
                    status_EXP = repo.GetStatus(NotificationCodes.PandOInvalidTableAccess);
                    //ACT
                    notificationHandler.Handle(notificationEvent_EXP);
                }
            }

            //ASSERT
            notificationUpdatedEvent_ACT.DateTimeEventFired.Should().BeOnOrBefore(DateTime.Now);
            notificationUpdatedEvent_ACT.NotificationDTO.Description.Should().Be(
                status_EXP.Description + " (" + notification_EXP.Detail.CustomMessage + ")");
            notificationUpdatedEvent_ACT.NotificationDTO.NotificationState.Should()
                .Be(status_EXP.NotificationState);
            notificationUpdatedEvent_ACT.NotificationDTO.DateRaised.Should()
                .Be(notification_EXP.Detail.DateRaised);

            using (var repo = 
                new ManagingNotificationsRepository(new ManagingNotificationsContext()))
            {
                var notificationACT = repo.GetNotifications();

                notificationACT.First().Id.Should().Be(1);
                notificationACT.First().Detail.CustomMessage.Should()
                    .Be(notification_EXP.Detail.CustomMessage);
                notificationACT.First().Detail.DateRaised.Should()
                    .Be(notification_EXP.Detail.DateRaised);
                notificationACT.First().Detail.StatusId.Should()
                    .Be(notification_EXP.Detail.StatusId);
                notificationACT.Count().Should().Be(1);
            }
        }
    }
}

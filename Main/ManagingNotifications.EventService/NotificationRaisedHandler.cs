using DMT.SharedKernel.Interface;
using DMT.ManagingNotifications.Data;
using ManagingNotifications.Domain.Entities;
using DMT.SharedKernel;
using DMT.ManagingNotifications.Service;

namespace DMT.ManagingNotifications.EventService
{
    public class NotificationRaisedHandler : IHandle<NotificationRaisedEvent>
    {
        public void Handle(NotificationRaisedEvent notificationEvent)
        {
            using (var repod = new ManagingNotificationsRepository(new ManagingNotificationsContext()))
            {
                var notification = new Notification(notificationEvent.StatusId,
                                                         notificationEvent.DateTimeEventFired,
                                                         notificationEvent.CustomMessage);

                repod.AddNotification(notification);

                var status = repod.GetStatus(notificationEvent.StatusId);
                var notificationDTO =  DTOConversion.ConvertNotificationAndStatusToNotificationDTO(notification, status);
                DomainEvents.Raise(new NotificationsUpdatedEvent(notificationDTO));
            }
        }
    }
}

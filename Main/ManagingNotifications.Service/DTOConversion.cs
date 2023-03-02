using DMT.DTO;
using DMT.ManagingNotifications.Domain.Entities;
using DMT.SharedKernel;
using ManagingNotifications.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.ManagingNotifications.Service
{
    public static class DTOConversion
    {
        public static NotificationDTO ConvertNotificationAndStatusToNotificationDTO(Notification notification,
                                                                                    Status status)
        {
            var notificationDTO = new NotificationDTO(notification.Detail.DateRaised,
                                                    status.Description + " (" + notification.Detail.CustomMessage + ")",
                                                    status.NotificationState);
            return (notificationDTO);
        }

    }
}

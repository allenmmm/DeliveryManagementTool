using DMT.DTO;
using DMT.SharedKernel;
using DMT.SharedKernel.Interface;
using DMT.SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.ManagingNotifications.EventService
{
    public class NotificationsUpdatedEvent : IDomainEvent
    {
        public DateTime DateTimeEventFired { get; private set; }
        
        public NotificationDTO NotificationDTO { get; private set; }

        public NotificationsUpdatedEvent(NotificationDTO notificationDTO)
        {
            DateTimeEventFired = DateTime.Now;
            NotificationDTO = new NotificationDTO(notificationDTO);
        }
    }
}
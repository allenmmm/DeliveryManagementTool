using DMT.DTO;
using DMT.ManagingNotifications.Domain.Entities;
using ManagingNotifications.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.ManagingNotifications.Domain.Interfaces
{
    public interface IManagingNotificationsRepo
    {
        void AddNotification(Notification notification);

        IEnumerable<Notification> GetNotifications();
        Status GetStatus(Guid id);
    }
}

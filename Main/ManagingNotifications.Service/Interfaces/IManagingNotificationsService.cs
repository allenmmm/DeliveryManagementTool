using DMT.DTO;
using ManagingNotifications.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.ManagingNotifications.Service.Interfaces
{
    public interface IManagingNotificationsService
    {
        IReadOnlyList<NotificationDTO> GetNotifications();
    }
}

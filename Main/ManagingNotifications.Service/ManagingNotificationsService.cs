using DMT.DTO;
using DMT.ManagingNotifications.Domain.Interfaces;
using DMT.ManagingNotifications.Service.Interfaces;
using DMT.SharedKernel;
using DMT.SharedKernel.Interface;
using ManagingNotifications.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.ManagingNotifications.Service
{
    public class ManagingNotificationsService : IManagingNotificationsService
    {
        private readonly IManagingNotificationsRepo _ManagingNotificationsRepo;
        public ManagingNotificationsService(IManagingNotificationsRepo managingNotificationsRepo)
        {
            _ManagingNotificationsRepo = managingNotificationsRepo;
        }

        public IReadOnlyList<NotificationDTO> GetNotifications()
        {
            List<NotificationDTO> notificationDTOs = new List<NotificationDTO>();
            try
            {
                var notifications = _ManagingNotificationsRepo.GetNotifications();
                foreach(var notification in notifications)
                {
                    var status = _ManagingNotificationsRepo.GetStatus(notification.Detail.StatusId);
                    notificationDTOs.Add(DTOConversion.ConvertNotificationAndStatusToNotificationDTO(notification, status));
                }
            }
            catch(Exception ex)
            {
                notificationDTOs.Add(new NotificationDTO(DateTime.Now,
                    "Unexpected error whilst retrieving notifications (" + ex.Message + ")", NotificationState.Error));
            }
            return (notificationDTOs);
        }
    }
}

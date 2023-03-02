using DMT.SharedKernel.Interface;
using DMT.SharedKernel;
using DMT.ManagingNotifications.EventService;
using Microsoft.AspNet.SignalR;

namespace DMT.Web.EventService
{
    public class NotificationsUpdatedHandler : IHandle<NotificationsUpdatedEvent>
    {
        public void Handle(NotificationsUpdatedEvent notificationEvent)
        {
            var nDTO = notificationEvent.NotificationDTO;
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationsUpdatedHub>();
            
            context.Clients.All.appendToNotificationsTable(nDTO.DateRaised.ToString(), 
                                                           nDTO.Description, 
                                                           ConvertStateToIcon(nDTO.NotificationState));
        }

        private string ConvertStateToIcon(NotificationState notificationState)
        {
            switch (notificationState)
            {
                case NotificationState.Success:
                    return "<i class='fas fa-check-circle'></i>";
                case NotificationState.Warning:
                    return "<i class='fas fa-exclamation-triangle'></i>";
                case NotificationState.Error:
                    return "<i class='fas fa-exclamation-circle'></i>";
                case NotificationState.Information:
                    return "<i class='fas fa-info-circle'></i>";
                default:
                    return "<i class='fas fa-exclamation-triangle'></i>";
            }
        }
    }
}
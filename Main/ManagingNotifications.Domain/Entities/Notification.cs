using DMT.DTO;
using DMT.ManagingNotifications.Domain.Entities;
using DMT.ManagingNotifications.Domain.ValueObjects;
using DMT.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingNotifications.Domain.Entities
{
    public class Notification : Entity<int>
    {
        public Notification_VO Detail { get;  private set; }

        private Notification() { }
    
        public Notification(Guid statusId, DateTime dateRaised, string customMessage)
        {
            Detail = Notification_VO.Create(statusId, dateRaised, customMessage);
        }
    }
}

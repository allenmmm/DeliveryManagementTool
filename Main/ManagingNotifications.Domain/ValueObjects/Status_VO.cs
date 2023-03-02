using DMT.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.ManagingNotifications.Domain.ValueObjects
{
    public class Status_VO : ValueObject<Status_VO>
    {
        public string Description { get; private set; }
        public NotificationState NotificationState { get;  private set; }
        private Status_VO(){}
        private Status_VO(string description, NotificationState notificationState)
        {
            Description = description;
            NotificationState = notificationState;
        }

        public static Status_VO Create(string description, NotificationState notificationState)
        {
            Gaurd.AgainstNull(description, "code summary is null");
            Gaurd.AgainstEmpty(description, "code object is null");
            return new Status_VO(description, notificationState);
        }
    }
}

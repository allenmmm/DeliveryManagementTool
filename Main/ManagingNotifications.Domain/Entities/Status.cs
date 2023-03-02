using DMT.ManagingNotifications.Domain.ValueObjects;
using DMT.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.ManagingNotifications.Domain.Entities
{
    public class Status : Entity<Guid>
    {
        public string Description { get; private set; }
        public NotificationState NotificationState { get; private set; }
        public Status(Guid id, Status_VO status_VO  )
            : base(id)
        {
            Description = status_VO.Description;
            NotificationState = status_VO.NotificationState;
        }
        private Status() { }
    }
}

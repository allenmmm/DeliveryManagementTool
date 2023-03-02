using DMT.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.ManagingNotifications.Domain.ValueObjects
{
    public class Notification_VO : ValueObject<Notification_VO>
    {
        public Guid StatusId { get;  private set; }

        public String CustomMessage { get; private set; }

        public DateTime DateRaised { get; private set; }

        private Notification_VO() { }

        private Notification_VO(Guid summaryId, DateTime dateRaised, string customMessage )
        {
            StatusId = summaryId;
            CustomMessage = customMessage;
            DateRaised = dateRaised;
        }

        public Notification_VO(Notification_VO notification_VO)
        {
            StatusId = notification_VO.StatusId;
            CustomMessage = notification_VO.CustomMessage;
            DateRaised = notification_VO.DateRaised;
        }

        public static Notification_VO Create(Guid statusId, DateTime dateRaised, string customMessage)
        {
            Gaurd.AgainstNull(customMessage, "code summary is null");
            return new Notification_VO(statusId, dateRaised, customMessage);
        }
    }
}

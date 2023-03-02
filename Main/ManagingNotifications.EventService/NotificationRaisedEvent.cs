using DMT.SharedKernel;
using DMT.SharedKernel.Interface;
using DMT.SharedKernel.ValueObjects;
using System;

namespace DMT.ManagingNotifications.EventService
{
    public class NotificationRaisedEvent : IDomainEvent
    {
        public DateTime DateTimeEventFired { get; private set; }
        public string CustomMessage { get; private set; }
        public Guid StatusId { get; private set; }

        public NotificationRaisedEvent(Guid statusId,
                                DateTime dateTime,
                                 String customMessage = "")
        {
            DateTimeEventFired = dateTime;
            StatusId = statusId;
            CustomMessage = FormatCustomMessage(statusId, dateTime, customMessage);
        }

        private string FormatCustomMessage(Guid statusId, DateTime dateTime, string custom)
        {
            string formattedMessage = custom;
            //some event messages requires orderbook week appending 
            if (statusId == NotificationCodes.OrderbooksGeneratedOK ||
                statusId == NotificationCodes.OrderbookGeneratedOK ||
                statusId == NotificationCodes.OrderbooksGeneratedError ||
                statusId == NotificationCodes.OrderbookGeneratedError ||
                statusId == NotificationCodes.OrderbookGenerationStarted)
            {
                formattedMessage += "Week " + OrderbookWeek_VO.Create(dateTime)
                    .FormatString();
            }
            return formattedMessage;
        }
    }
}
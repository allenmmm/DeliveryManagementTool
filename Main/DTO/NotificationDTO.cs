using DMT.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace DMT.DTO
{
    public class NotificationDTO
    {
        [Display(Name = "Date Raised")]
        public  DateTime DateRaised { get; private set; }
        public String Description { get; private set; }
        [Display(Name = "State")]
        public NotificationState NotificationState { get; private set; }

        public NotificationDTO(DateTime dateRaised,
                               String description,
                               NotificationState notificationState)
        {
            DateRaised = dateRaised;
            Description = description;
            NotificationState = notificationState;
        }

        public NotificationDTO(NotificationDTO notificationDTO )
        {
            DateRaised = notificationDTO.DateRaised;
            Description = notificationDTO.Description;
            NotificationState = notificationDTO.NotificationState;
        }
    }
}
using DMT.DTO;
using DMT.ManagingNotifications.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DMT.Web.Controllers
{
    public class ManagingNotificationsController : Controller
    {
        private readonly IManagingNotificationsService _managingNotificationsService;
        
        public ManagingNotificationsController(IManagingNotificationsService managingNotificationsService)
        {
            _managingNotificationsService = managingNotificationsService;
        }
        
        public ActionResult GetNotifications()
        {
            IReadOnlyList<NotificationDTO> notifications =
                _managingNotificationsService.GetNotifications();
        
            return PartialView("_Notifications", notifications);
        }
    }
}
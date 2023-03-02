using DMT.ManagingNotifications.Domain.Entities;
using DMT.ManagingNotifications.Domain.Interfaces;
using DMT.SharedKernel;
using ManagingNotifications.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMT.ManagingNotifications.Data
{
    public class ManagingNotificationsRepository : IManagingNotificationsRepo, IDisposable
    {
        private readonly ManagingNotificationsContext _Context;

        public ManagingNotificationsRepository(ManagingNotificationsContext _ManagingNotificationsContext)
        {
            _Context = _ManagingNotificationsContext;
        }
        protected virtual void Dispose(bool disposing)
        {
            // then free native/unmanaged resources here
            if (_Context != null)
                _Context.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void AddNotification(Notification notification)
        {
            if( _Context.Statuses.Where(s => s.Id == notification.Detail.StatusId).Count() == 0 )
            {
                throw new DomainException("Unable to find notification status in lookup table " 
                                            + notification.Detail.StatusId.ToString());
            }
            _Context.Notifications.Add(notification);
            _Context.SaveChanges();
        }

        public IEnumerable<Notification> GetNotifications()
        {
             return(_Context.Notifications.ToList());
        }

        public Status GetStatus(Guid id)
        {
            if(_Context.Statuses.Where(s => s.Id == id).Count() == 0)
            {
                throw new DomainException("Notification status code " + id.ToString() + " does not exist");
            }
            return (_Context.Statuses.First(s => s.Id == id));
        }
    }
}

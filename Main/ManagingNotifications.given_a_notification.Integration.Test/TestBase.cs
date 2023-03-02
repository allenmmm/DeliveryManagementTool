using DMT.ManagingNotifications.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingNotifications.given_a_notification.Integration.Test
{
    public abstract class TestBase : IDisposable
    {
        protected readonly ManagingNotificationsContext ManagingNotificationsContext;

        public TestBase()
        {
            ManagingNotificationsContext = new ManagingNotificationsContext();
        }

        protected virtual void SetUp()
        {
            ManagingNotificationsContext.Database.EnsureDeleted();
            ManagingNotificationsContext.Database.EnsureCreated();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (ManagingNotificationsContext != null)
                ManagingNotificationsContext.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

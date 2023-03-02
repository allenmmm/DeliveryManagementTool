using DMT.ManagingNotifications.Data;
using Microsoft.EntityFrameworkCore;
using System;


namespace ManagingNotifications.given_a_notification.Data.Test
{
    public abstract class TestBase
    {
        protected static string conn = Guid.NewGuid().ToString();

        public TestBase()
        {
        }

        protected virtual void SetUp()
        {
            ManagingNotificationsContext context = new ManagingNotificationsContext(new DbContextOptionsBuilder<DbContext>()
                    .UseInMemoryDatabase(databaseName: conn).Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}


using DMT.GeneratingOrderBooks.Data;
using DMT.ManagingNotifications.Data;
using Microsoft.EntityFrameworkCore;

namespace DMT.Web.App_Start
{
    public class DatabaseConfig
    {
        public static void ExecuteMigrations()
        {
            GeneratingOrderbooksContext generatingOrderbooksContext = new GeneratingOrderbooksContext();
            ManagingNotificationsContext managingNotificationsContext = new ManagingNotificationsContext();

            generatingOrderbooksContext.Database.Migrate();
            managingNotificationsContext.Database.Migrate();
        }
    }
}
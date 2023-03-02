using DMT.ManagingNotifications.Domain.Entities;
using DMT.ManagingNotifications.Domain.ValueObjects;
using DMT.SharedKernel;
using ManagingNotifications.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DMT.ManagingNotifications.Data
{
    public class ManagingNotificationsContext : DbContext
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ManagingNotificationsContext(DbContextOptions options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        internal DbSet<Notification> Notifications { get; set; }
        internal DbSet<Status> Statuses { get; set; }

        public ManagingNotificationsContext(){
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var settings = ConfigurationManager.ConnectionStrings;
                var connectionString = settings["DMTDatabase"].ConnectionString;
                optionsBuilder.UseSqlServer(connectionString);

            }
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("ManagingNotifications");
            modelBuilder.Entity<Notification>()
              .ToTable("Notifications", schema: "ManagingNotifications");
            modelBuilder.Entity<Status>()
              .ToTable("Statuses", schema: "ManagingNotifications");

            modelBuilder.Entity<Status>().Property(p => p.NotificationState).HasConversion<int>();
            modelBuilder.Entity<Status>().Property(p => p.Id).ValueGeneratedNever();
            modelBuilder.Entity<Notification>().OwnsOne(ep => ep.Detail);

            modelBuilder.Entity<Status>().HasData(
                new Status(NotificationCodes.OrderbooksGeneratedOK,
                           Status_VO.Create("All expected orderbook(s) generated",
                           NotificationState.Success)),
                new Status(NotificationCodes.PandOTableDeletionError,
                           Status_VO.Create("The planned and overdues table could not be deleted," +
                           "this will potentially corrupt future orderbook generations",
                           NotificationState.Warning)),
                new Status(NotificationCodes.OrderbooksGeneratedError,
                           Status_VO.Create("Errors occurred during generating of orderbook(s)",
                           NotificationState.Error)),
                new Status(NotificationCodes.OrderbookGeneratedError,
                           Status_VO.Create("Orderbook not generated",
                           NotificationState.Error)),
                new Status(NotificationCodes.PandOInvalidTableAccess,
                           Status_VO.Create("Orderbook(s) generation terminated.  " +
                           "Planned and overdues table empty or could not be validated/accessed",
                           NotificationState.Error)),
                new Status(NotificationCodes.OrderbookGenerationStarted,
                           Status_VO.Create("Orderbook(s) generation started",
                           NotificationState.Information)),
                new Status(NotificationCodes.OrderbookGeneratedOK,
                           Status_VO.Create("Orderbook generated",
                           NotificationState.Success)),
                new Status(NotificationCodes.UnableToRetrieveOrderbookWeeks,
                           Status_VO.Create("Orderbook weeks can not be retrieved, unable to query for " +
                                            "generated orderbooks",
                           NotificationState.Error)),
                new Status(NotificationCodes.UnableToRetrieveOrderbookPreviews,
                           Status_VO.Create("Orderbook previews can not be retrieved, none exist or database error",
                           NotificationState.Error)),
                new Status(NotificationCodes.UnableToRetrieveOrderbook,
                           Status_VO.Create("Orderbook can not be retrieved, none exist or database error",
                           NotificationState.Error))          
            );
            base.OnModelCreating(modelBuilder);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Then free managed resources here:
                ChangeTracker.Entries()
                     .Where(e => e.Entity != null)
                     .ToList().ForEach(p => p.State = EntityState.Detached);
            }
        }

        public sealed override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

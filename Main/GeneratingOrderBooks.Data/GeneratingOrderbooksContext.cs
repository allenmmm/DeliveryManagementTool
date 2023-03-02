using DMT.GeneratingOrderbooks.Data;
using DMT.GeneratingOrderBooks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DMT.GeneratingOrderBooks.Data
{
    public class GeneratingOrderbooksContext : DbContext
    {
        internal DbSet<PlannedAndOverdueOrder> PlannedAndOverdueOrders { get; set; }
        internal DbSet<Supplier> Supplier { get; set; }
        internal DbSet<Orderbook> Orderbooks { get; set; }
        internal DbSet<Order> Orders { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GeneratingOrderbooksContext(DbContextOptions options) : base(options) {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public GeneratingOrderbooksContext() {
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
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new PlannedAndOverdueOrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderbookConfiguration());
            modelBuilder.ApplyConfiguration(new SupplierConfiguration());
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

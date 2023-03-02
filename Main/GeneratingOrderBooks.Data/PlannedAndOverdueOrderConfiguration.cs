using DMT.GeneratingOrderBooks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMT.GeneratingOrderbooks.Data
{
    public class PlannedAndOverdueOrderConfiguration : IEntityTypeConfiguration<PlannedAndOverdueOrder>
    {
        public void Configure(EntityTypeBuilder<PlannedAndOverdueOrder> modelBuilder)
        {
            modelBuilder.HasKey(p => p.Id);

            modelBuilder
              .Property(p => p.Id)
              .HasMaxLength(23);

            modelBuilder
                  .Property(p => p.Id)
                  .ValueGeneratedNever();

            modelBuilder
                .Property(p => p.PartDescription)
                .HasMaxLength(100);

            modelBuilder
                .Property(p => p.PartNumber)
                .HasMaxLength(80);

            modelBuilder
                .Property(p => p.PurchaseOrder)
                .HasMaxLength(8);

            modelBuilder
                .Property(p => p.SupplierName)
                .HasMaxLength(80);

            modelBuilder
            .Property(sample => sample.ItemDeliveryDate)
            .HasColumnType("date");

            modelBuilder
            .Property(sample => sample.StatDeliverySchedule)
            .HasColumnType("date");

            modelBuilder
            .Property(sample => sample.DatePulled)
            .HasColumnType("smalldatetime");
        }
    }
}

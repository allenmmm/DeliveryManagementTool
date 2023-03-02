using DMT.GeneratingOrderBooks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMT.GeneratingOrderbooks.Data
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> modelBuilder)
        {
            modelBuilder
               .Property(b => b.PandOId)
               .HasMaxLength(23);

            modelBuilder.OwnsOne(o => o.Details, a =>
            {
                a.Property(sample => sample.StatDeliverySchedule)
                 .HasColumnType("date");
                a.Property(sample => sample.ItemDeliveryDate)
                 .HasColumnType("date");
                a.Property(sample => sample.PurchaseOrder)
                .HasMaxLength(8);
            });

            modelBuilder.OwnsOne(o => o.Part, a =>
            {
                a.Property(sample => sample.Description)
                   .HasMaxLength(100);
                a.Property(sample => sample.Number)
                    .HasMaxLength(80);
            });

            modelBuilder.HasKey(o => new { o.PandOId, o.OrderbookId });
        }
    }
}

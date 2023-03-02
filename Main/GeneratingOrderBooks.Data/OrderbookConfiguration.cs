using DMT.GeneratingOrderBooks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMT.GeneratingOrderbooks.Data
{
    public class OrderbookConfiguration : IEntityTypeConfiguration<Orderbook>
    {
        public void Configure(EntityTypeBuilder<Orderbook> modelBuilder)
        {
            modelBuilder.Property(p => p.Id).ValueGeneratedNever();

            var navigation = modelBuilder
               .Metadata.FindNavigation(nameof(Orderbook.Orders));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.OwnsOne(o => o.DateStamp, a =>
            {
                a.Property(sample => sample.DatePulled)
                    .HasColumnType("smalldatetime");
                a.Property(sample => sample.OrderbookWeek)
                    .HasMaxLength(7);
            });
        }
    }
}

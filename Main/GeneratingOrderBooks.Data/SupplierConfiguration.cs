using DMT.GeneratingOrderBooks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMT.GeneratingOrderbooks.Data
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> modelBuilder)
        {
            modelBuilder.Property(s => s.Id)
            .HasMaxLength(12).IsFixedLength();
            modelBuilder.Property(p => p.Id).ValueGeneratedNever();

            var navigation = modelBuilder
             .Metadata.FindNavigation(nameof(Supplier.Orderbooks));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.OwnsOne(s => s.Details, a =>
            {
                a.Property(p => p.Name)
                .HasMaxLength(80);
            });
        }
    }
}

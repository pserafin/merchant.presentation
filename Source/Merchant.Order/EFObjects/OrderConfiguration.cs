using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Merchant.Order.EFObjects
{
    public class OrderConfiguration : IEntityTypeConfiguration<CORE.EFModels.Order>
    {
        public void Configure(EntityTypeBuilder<CORE.EFModels.Order> builder)
        {
            builder.ToTable("Order", "order");
            builder.HasKey(x => x.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Date).IsRequired();
            builder.Property(p => p.UserId).IsRequired();
            builder.HasMany(p => p.Items).WithOne(p => p.Order).HasForeignKey(p => p.OrderId);
        }
    }
}

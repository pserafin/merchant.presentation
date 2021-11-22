using Merchant.CORE.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Merchant.Order.EFObjects
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product", "product");
            builder.HasKey(x => x.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Name).HasMaxLength(512).IsRequired();
            builder.Property(p => p.Quantity).IsRequired();
            builder.Property(p => p.Price).IsRequired();
            builder.HasMany(p => p.OrderItems).WithOne(p => p.Product).HasForeignKey(p => p.ProductId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}


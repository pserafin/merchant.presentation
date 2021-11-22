using Merchant.CORE.EFModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Merchant.Order.EFObjects
{
    public class OrderUserConfiguration : IEntityTypeConfiguration<OrderUser>
    {
        public void Configure(EntityTypeBuilder<OrderUser> builder)
        {
            builder.ToTable("AspNetUsers", "dbo");
            builder.HasKey(x => x.Id);
            builder.Property(p => p.Name).HasColumnName("NormalizedUserName");
            builder.Property(p => p.Email).HasColumnName("NormalizedEmail");
            builder.HasMany(p => p.Orders).WithOne(p => p.User).HasForeignKey(p => p.UserId);
        }
    }
}

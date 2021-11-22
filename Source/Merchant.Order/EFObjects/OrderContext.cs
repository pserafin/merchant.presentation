using Merchant.CORE.EFModels;
using Microsoft.EntityFrameworkCore;

namespace Merchant.Order.EFObjects
{
    public class OrderContext: DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options) { }

        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public virtual DbSet<CORE.EFModels.Order> Orders { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<OrderUser> OrdersUsers { get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new OrderUserConfiguration());
        }
        
    }
}

using Merchant.CORE.EFModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Merchant.CORE.EFObjects
{
    public class IdentityDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

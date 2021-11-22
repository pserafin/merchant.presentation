using System;
using Merchant.CORE.EFObjects;
using Merchant.Order.EFObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Merchant.API.Setup
{
    public static class ContextAndMappingConfig
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(connectionString, o => o.MigrationsAssembly("Merchant.API")));
            services.AddDbContext<OrderContext>(options => options.UseSqlServer(connectionString, o => o.MigrationsAssembly("Merchant.API")));
        }
    }
}

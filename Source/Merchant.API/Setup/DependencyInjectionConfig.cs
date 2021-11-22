using Merchant.CORE.IHelpers;
using Merchant.CORE.IRepositories;
using Merchant.CORE.IServices;
using Merchant.CORE.Services;
using Merchant.Order.Helpers;
using Merchant.Order.Repositories;
using Merchant.Order.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Merchant.API.Setup
{
    public static class DependencyInjectionConfig
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ICartValidator, CartValidator>();
        }
    }
}

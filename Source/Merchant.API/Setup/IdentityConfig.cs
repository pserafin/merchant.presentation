using System.Net;
using Merchant.CORE.EFModels;
using Merchant.CORE.EFObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Merchant.API.Setup
{
    public static class IdentityConfig
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole<int>>().AddEntityFrameworkStores<IdentityDbContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(opts => {
                opts.Password.RequiredLength = 4;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = true;
            });

            services.ConfigureApplicationCookie(opts =>
            {
                opts.LoginPath = "/login";
                opts.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        statusCode = HttpStatusCode.Unauthorized,
                        message = "You are not logged in"
                    }));
                    return Task.CompletedTask;
                };
                opts.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        statusCode = HttpStatusCode.Forbidden,
                        message = "You are not allowed to perform this operation"
                    }));
                    return Task.CompletedTask;
                };
            });
        }
    }
}

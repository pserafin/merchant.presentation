using Merchant.API.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Merchant.API
{
    public class Startup
    {
        private const string AllowedOrginConfigKey = "AllowedOrigins";
        private const string ConnectionStringConfigKey = "ConnectionStrings:DefaultConnection";
        private const string CorsPolicyName = "DefaultPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = true);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Merchant", Version = "v1" });
            });
            services.AddCors(options => options.AddPolicy(CorsPolicyName, SetCorsPolicy()));

            ContextAndMappingConfig.Configure(services, Configuration[ConnectionStringConfigKey]);
            IdentityConfig.Configure(services);
            DependencyInjectionConfig.Configure(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(CorsPolicyName);
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private CorsPolicy SetCorsPolicy()
        {
            var policy = new CorsPolicy();
            policy.Headers.Add("*");
            policy.Methods.Add("*");
            foreach (var origin in Configuration[AllowedOrginConfigKey].Split(";"))
            {
                policy.Origins.Add(origin);
            }
            policy.SupportsCredentials = true;

            return policy;
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;
using ServiceDiscovery;

namespace FinancePlanner.ApiGateway.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddGatewayServices(this IServiceCollection services, IConfiguration config)
        {
            services.ConfigureOcelot();
            services.ConfigureAuthentication(config);
            services.AddServiceDiscovery(config);
            return services;
        }

        // Ocelot Configuration
        private static void ConfigureOcelot(this IServiceCollection services)
        {
            services.AddOcelot()
                .AddCacheManager(builder =>
                {
                    builder.WithDictionaryHandle();
                })
                .AddConsul();
        }

        // Service Discovery
        private static void AddServiceDiscovery(this IServiceCollection services, IConfiguration config)
        {
            ServiceConfig serviceConfig = config.GetServiceConfig();
            services.RegisterConsulServices(serviceConfig);
        }

        // Authentication
        private static void ConfigureAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication()
                .AddJwtBearer(authenticationScheme: config.GetSection("IdentityServer:IdentityApiKey").Value, options =>
                {
                    options.Authority = config.GetSection("IdentityServer:Url").Value; // IDENTITY SERVER URL
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });
        }
    }
}

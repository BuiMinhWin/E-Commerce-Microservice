using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services,
            IConfiguration config, string fileName) where TContext : DbContext
        {
            //Add generic database context 
            services.AddDbContext<TContext>(option => option.UseSqlServer(
                config
                    .GetConnectionString("ECommerceConnection"), sqlserverOption => sqlserverOption.EnableRetryOnFailure())); 
            //configure serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd:mm:ss.fff zzz} [{Level:u3}] {message: lj}{NewLine} {Exception}",
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            //Add JWT authentication Scheme
            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, config);
            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            //use global exception
            app.UseMiddleware<GlobalException>();
            //refister middleware to block all outsiders API call
            app.UseMiddleware<ListenToOnlyAPIGateway>();

            return app;
        }
    }
}

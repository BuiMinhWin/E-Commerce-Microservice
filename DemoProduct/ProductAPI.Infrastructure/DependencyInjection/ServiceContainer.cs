using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductAPI.Application.Interfaces;
using ProductAPI.Domain.Entities;
using ProductAPI.Infrastructure.Data;
using ProductAPI.Infrastructure.Repositories;

namespace ProductAPI.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
        
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services,
            IConfiguration config)
        {
            //add db connect
            //add authentication scheme
            SharedServiceContainer.AddSharedServices<ProductDbContext>(services,config, config["MySerilog:FineName"]!);
            //Create DI
            services.AddScoped<IProduct, ProductRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            //register middleware such as:
            //global exception: handles external error 
            //Listen to only api gateway: block all outsider call
            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}

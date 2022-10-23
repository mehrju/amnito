using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using System;

namespace Nop.plugin.Orders.ExtendedShipment.Infrastructure
{
    public class StartUp : INopStartup
    {
        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => Int32.MaxValue;

        public void Configure(IApplicationBuilder application)
        {
            
        }

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<MvcOptions>(config =>
            {
                config.Filters.Add<ExtendedShipmentActionFilter>();
            });
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ViewLocationExpander());
            });
        }
    }
}

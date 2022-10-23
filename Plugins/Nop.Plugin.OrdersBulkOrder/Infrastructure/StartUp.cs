using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using System;

namespace Nop.Plugin.Orders.BulkOrder.Infrastructure
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
            
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using System;
using Microsoft.AspNetCore.Http.Extensions;
using Nop.Plugin.Orders.MultiShipping.Services;

namespace Nop.Plugin.Orders.MultiShipping.Infrastructure
{
    public class StartUp : INopStartup
    {
        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => Int32.MaxValue;

        public void Configure(IApplicationBuilder application)
        {
            //application.Use(async (context, next) =>
            //{
            //    string url = context.Request.GetDisplayUrl().ToLower();
            //    var _newCheckout = context.RequestServices.GetRequiredService<INewCheckout>();
            //    _newCheckout.ProccessIncomeUrl(url);
            //    await next();
            //});
        }

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<MvcOptions>(config =>
            {
                config.Filters.Add<MultiShipingActionFilter>();
            });
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ViewLocationExpander());
            });
        }
    }
}

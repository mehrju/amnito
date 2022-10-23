using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Linq;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.WebApi.Logger;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using Nop.Web.Framework.FluentValidation;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.Infrastructure.Extensions;


namespace BS.Plugin.NopStation.MobileWebApi.Infrastructure.WebApi
{
    public static class CustomServiceCollectionExtensions
    {
        /// <summary>
        /// Add and configure MVC for the application
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <returns>A builder for configuring MVC services</returns>
        public static IMvcBuilder AddNopMvc(this IServiceCollection services)
        {
            //add basic MVC feature
            var mvcBuilder = services.AddMvc();

           

            //add global exception filter
            mvcBuilder.AddMvcOptions(options => options.Filters.Add(new SimpleExceptionFilter()));

           

            return mvcBuilder;
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Infrastructure;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;

namespace Nop.plugin.Orders.ExtendedShipment
{
    public class ExtendedShipmentStartup : INopStartup
    {


        public int Order => 499;

        public void Configure(IApplicationBuilder application)
        {
           // application.UseAntiXssMiddleware();
            var _settingService = EngineContext.Current.Resolve<ISettingService>();
            var setting = _settingService.LoadSetting<TozicoSetting>();
            if (setting == null || string.IsNullOrEmpty(setting.AccessToken) || string.IsNullOrEmpty(setting.AccessToken))
            {
                _settingService.SaveSetting(new TozicoSetting()
                {
                    AccessToken = "tz.jr3JR2sqe9htCeAt3dwgZEVl1jNJPW",
                    BaseAddress = "http://api.tozico.com/api"
                });
            }
			var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".apk"] = MimeTypes.ApplicationOctetStream;
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
               // RequestPath = new PathString("/"),
                ContentTypeProvider = provider
            });
			//application.UseRewriteMiddleware();	
        }

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            //throw new NotImplementedException();
        }
    }
}

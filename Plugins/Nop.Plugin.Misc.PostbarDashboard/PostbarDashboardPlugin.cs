using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.PostbarDashboard
{
    public class PostbarDashboardPlugin : BasePlugin, IMiscPlugin
    {
        private readonly PostbarDashboardSettings _dashboardSettings;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly CustomerSettings _customerSettings;
        private readonly IWebHelper _webHelper;
        private readonly PostbarDashboardObjectContext _context;

        public PostbarDashboardPlugin(
            PostbarDashboardSettings dashboardSettings,
            ILogger logger,
            ISettingService settingService,
            IStoreContext storeContext,
            CustomerSettings customerSettings,
            IWebHelper webHelper,
            PostbarDashboardObjectContext context
            )
        {
            this._dashboardSettings = dashboardSettings;
            this._logger = logger;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._customerSettings = customerSettings;
            this._webHelper = webHelper;
            this._context = context;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "Dashboard";
            routeValues = new RouteValueDictionary { { "Namespaces", ".Plugin.Misc.Dashboard.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            var settings = new PostbarDashboardSettings
            {
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PostbarDashboard.TitleServiceProvider", "عنوان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PostbarDashboard.UrlPageDiscreption", "لینک توضیحات");

            #region slidshow
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PostbarDashboard.Title_Carousel_slideshow", "عنوان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PostbarDashboard.Discrition_Carousel_slideshow", "توضیحات");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PostbarDashboard.UrlPageDiscreption", "لینک");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PostbarDashboard.TimeInterval", "زمان پخش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PostbarDashboard.DateInsert", "تاریخ شروع");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PostbarDashboard.DateExpire", "تایخ پایان");




            #endregion



            base.Install();
            _context.Install();
            PluginManager.MarkPluginAsInstalled(this.PluginDescriptor.SystemName);
        }
        public void ManageSiteMap(SiteMapNode rootNode)
        {
            SiteMapNode siteMapNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Configuration");
          

        }
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<PostbarDashboardSettings>();
            PluginManager.MarkPluginAsUninstalled(this.PluginDescriptor.SystemName);
            _context.Uninstall();
            base.Uninstall();
        }

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/Dashboard/Configure";
        }
    }
}

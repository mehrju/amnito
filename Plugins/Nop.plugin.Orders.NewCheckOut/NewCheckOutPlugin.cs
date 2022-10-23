using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Plugins;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;

namespace Nop.plugin.Orders.NewCheckOut
{
    /// <summary>
    /// Manual payment processor
    /// </summary>
    public class NewCheckOutPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IDbContext _dbContext;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly DataSettings _dataSettings;

        public NewCheckOutPlugin(IWebHelper webHelper,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IDbContext dbContext,
            IHostingEnvironment hostingEnvironment,
            DataSettings dataSettings
            )
        {
            this._webHelper = webHelper;
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._hostingEnvironment = hostingEnvironment;
            this._dbContext = dbContext;
            this._dataSettings = dataSettings;

        }

        public override string GetConfigurationPageUrl()
        {
            return  $"{_webHelper.GetStoreLocation()}Admin/OrderDivisionConfig/Configure";
        }

        public override void Install()
        {
            base.Install();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                SystemName= "Configuration",
                Title = "تنظیم قیمت دلار و طلا",
                ControllerName = "OrderDivisionConfig",
                ActionName = "Configure",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
      
    }
   
}

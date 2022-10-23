using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Plugins;
using Nop.Plugin.Orders.BulkOrder.Models;
using Nop.Services.Common;
using Nop.Web.Framework.Menu;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Nop.Services.Localization;

namespace Nop.plugin.Product.ExtendedShipment
{
    /// <summary>
    /// 
    /// </summary>
    public class BulkOrderPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly BulkOrderObjectContext _context;
        private readonly ILocalizationService _localizationService;
        public BulkOrderPlugin(IWebHelper webHelper,
            BulkOrderObjectContext context,
            ILocalizationService localizationService)
        {
            this._webHelper = webHelper;
            this._context = context;
            this._localizationService = localizationService;
        }
        public override void Install()
        {
            base.Install();
            _context.Install();
            PluginManager.MarkPluginAsInstalled(this.PluginDescriptor.SystemName);
        }
        public override void Uninstall()
        {
            PluginManager.MarkPluginAsUninstalled(this.PluginDescriptor.SystemName);
            _context.Uninstall();
            base.Uninstall();
        }
        public override string GetConfigurationPageUrl()
        {
            return "";// $"{_webHelper.GetStoreLocation()}Admin/ExtendedOrderSetting/Configure";
        }
        public void ManageSiteMap(SiteMapNode rootNode)
        {
            SiteMapNode siteMapNode1 = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Sales");

            SiteMapNode siteMapNode2 = new SiteMapNode()
            {
                SystemName = "Sales.BulkOrder",
                Title = "سفارشات انبوه",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                ActionName = "BulkOrderAdminIndex",
                ControllerName = "BulkOrderAdmin",
                RouteValues = new RouteValueDictionary()
                {
                    {
                        "area",
                        "admin"
                    }
                }
            };
            siteMapNode1.ChildNodes.Add(siteMapNode2);
            
        }
    }

}


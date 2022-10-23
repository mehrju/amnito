using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Plugins;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Services.Common;
using Nop.Web.Framework.Menu;
using System.Linq;

namespace Nop.plugin.Product.ExtendedShipment
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiShippingPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ExnShippmentObjectContext _context;
        public MultiShippingPlugin(IWebHelper webHelper, ExnShippmentObjectContext context)
        {
            this._context = context;
            this._webHelper = webHelper;
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
            //SiteMapNode item = new SiteMapNode
            //{
            //    SystemName = "Product.ExtendedShipment",
            //    Title = "تنظیم ارسال محموله",
            //    ControllerName = "ExtendedOrderSetting",
            //    ActionName = "Configure",
            //    Visible = true,
            //    IconClass = "fa fa-dot-circle-o",
            //    RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary
            //    {
            //        {
            //            "area",
            //            "admin"
            //        }
            //    }
            //};

            //SiteMapNode siteMapNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Configuration");
            //if (siteMapNode != null)
            //{
            //    siteMapNode.ChildNodes.Add(item);
            //    return;
            //}
            //rootNode.ChildNodes.Add(item);
        }
    }

}


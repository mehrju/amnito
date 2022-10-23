using FoxNetSoft.Plugin.Payments.PaymentRules.Data;
using FoxNetSoft.Plugin.Payments.PaymentRules.Logger;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FoxNetSoft.Plugin.Payments.PaymentRules
{
    public class PaymentRulesPlugin : BasePlugin, IMiscPlugin, IPlugin, IAdminMenuPlugin
    {
        private readonly IWebHelper iwebHelper_0;
        private readonly PaymentRulesObjectContext _context;
        private readonly ISettingService _settingService;
        public PaymentRulesPlugin(IWebHelper webHelper,
            PaymentRulesObjectContext context,
            ISettingService settingService)
        {
            _context = context;
            _settingService = settingService;
            this.iwebHelper_0 = webHelper;
        }

        public override string GetConfigurationPageUrl()
        {
            bool? nullable = null;
            return string.Concat(this.iwebHelper_0.GetStoreLocation(nullable), "Admin/PaymentRulesSettings/Configure");
        }

        public override void Install()
        {
            PaymentRulesSettings paymentRulesSetting = new PaymentRulesSettings()
            {
                Enabled = false,
                SerialNumber = "",
                showDebugInfo = false,
                Version = 111
            };
            _settingService.SaveSetting<PaymentRulesSettings>(paymentRulesSetting, 0);
            _context.Install();
            (new InstallLocaleResources(string.Concat(PluginLog.Folder, "Resources/"), false)).Install();
            base.Install();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            if (!EngineContext.Current.Resolve<IPermissionService>().Authorize(StandardPermissionProvider.ManagePaymentMethods))
            {
                return;
            }
            ILocalizationService localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            SiteMapNode siteMapNode = rootNode.ChildNodes.FirstOrDefault<SiteMapNode>((SiteMapNode x) => x.SystemName == "FoxNetSoft");
            if (siteMapNode == null)
            {
                SiteMapNode siteMapNode1 = new SiteMapNode();
                siteMapNode1.Title = "FoxNetSoft";
                siteMapNode1.ControllerName = "";
                siteMapNode1.ActionName = "";
                siteMapNode1.Visible = true;
                RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
                routeValueDictionary.Add("area", null);
                siteMapNode1.RouteValues = routeValueDictionary;
                siteMapNode1.SystemName = "FoxNetSoft";
                siteMapNode1.IconClass = "fa-bars";
                siteMapNode = siteMapNode1;
                rootNode.ChildNodes.Add(siteMapNode);
            }
            SiteMapNode siteMapNode2 = new SiteMapNode();
            siteMapNode2.Title = localizationService.GetResource("Admin.FoxNetSoft.PaymentRules.Menu.Name");
            siteMapNode2.ControllerName = "";
            siteMapNode2.ActionName = "";
            siteMapNode2.Visible = true;
            RouteValueDictionary routeValueDictionary1 = new RouteValueDictionary();
            routeValueDictionary1.Add("area", null);
            siteMapNode2.RouteValues = routeValueDictionary1;
            siteMapNode2.SystemName = "FoxNetSoft.PaymentRules";
            siteMapNode2.IconClass = "fa-dot-circle-o";
            SiteMapNode siteMapNode3 = siteMapNode2;
            siteMapNode.ChildNodes.Add(siteMapNode3);

            siteMapNode.ChildNodes=(
                from p in siteMapNode.ChildNodes
                orderby p.Title
                select p).ToList();

            SiteMapNode siteMapNode4 = new SiteMapNode();
            siteMapNode4.Title = localizationService.GetResource("Admin.FoxNetSoft.PaymentRules.Menu.SubMenu1");
            siteMapNode4.ControllerName = "";
            siteMapNode4.ActionName = "";
            siteMapNode4.Url = "~/Admin/PaymentRulesAdmin/List";
            siteMapNode4.Visible = true;
            RouteValueDictionary routeValueDictionary2 = new RouteValueDictionary();
            routeValueDictionary2.Add("area", null);
            siteMapNode4.RouteValues = routeValueDictionary2;
            siteMapNode4.SystemName = "FoxNetSoft.PaymentRules.List";
            siteMapNode4.IconClass = "fa-dot-circle-o";
            siteMapNode3.ChildNodes.Add(siteMapNode4);
            SiteMapNode siteMapNode5 = new SiteMapNode();
            siteMapNode5.Title = localizationService.GetResource("Admin.FoxNetSoft.PaymentRules.Menu.Configure");
            siteMapNode5.ControllerName = "";
            siteMapNode5.ActionName = "";
            siteMapNode5.Url = "~/Admin/PaymentRulesSettings/Configure";
            siteMapNode5.Visible = true;
            RouteValueDictionary routeValueDictionary3 = new RouteValueDictionary();
            routeValueDictionary3.Add("area", null);
            siteMapNode5.RouteValues = routeValueDictionary3;
            siteMapNode5.SystemName = "FoxNetSoft.PaymentRules.Configure";
            siteMapNode5.IconClass = "fa-dot-circle-o";
            siteMapNode3.ChildNodes.Add(siteMapNode5);
        }

        public override void Uninstall()
        {
            EngineContext.Current.Resolve<ISettingService>().DeleteSetting<PaymentRulesSettings>();
            EngineContext.Current.Resolve<PaymentRulesObjectContext>().Uninstall();
            base.Uninstall();
        }
    }
}
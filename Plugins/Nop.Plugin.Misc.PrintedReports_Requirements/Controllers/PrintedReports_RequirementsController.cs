using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Misc.PrintedReports_Requirements.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;


namespace Nop.Plugin.Misc.PrintedReports_Requirements.Controllers
{
    [Area(AreaNames.Admin)]
    public class PrintedReports_RequirementsController : BasePluginController
    {

        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public PrintedReports_RequirementsController
            (
            IWorkContext workContext,
            IStoreService storeService,
            IPermissionService permissionService,
            ISettingService settingService,
            ICacheManager cacheManager,
            ILocalizationService localizationService
            )
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var Settings = _settingService.LoadSetting<PrintedReports_RequirementsSettings>(storeScope);

            var model = (dynamic)null;

            return View("~/Plugins/Misc.PrintedReports_RequirementsController/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var Settings = _settingService.LoadSetting<PrintedReports_RequirementsSettings>(storeScope);

            //now clear settings cache
            _settingService.ClearCache();
            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.plugin.Orders.NewCheckOut.Models;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;

namespace Nop.plugin.Orders.NewCheckOut.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class OrderDivisionConfigController : BasePluginController
    {
        #region Fields
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ICustomerService _customerService;
        #endregion

        #region Ctor
        public OrderDivisionConfigController(ILocalizationService localizationService,
          IPermissionService permissionService,
          ISettingService settingService,
          IStoreService storeService,
          IWorkContext workContext,
          IHostingEnvironment hostingEnvironment,
          ICustomerService customerService
            )
        {
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._hostingEnvironment = hostingEnvironment;
            this._customerService = customerService;
        }
        #endregion

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var newCheckOutSettings = _settingService.LoadSetting<NewCheckOutSettings>(storeScope);
            var model = new OrderDivisionModel
            {
                BaseDallerPrice = newCheckOutSettings.DollerPrice,
                BaseGoldPriceInGram = newCheckOutSettings.GlodPriceInGram,
                RoleId = newCheckOutSettings.RoleId,
                RoleList = GetListOFRole(newCheckOutSettings.RoleId)
            };
            return View("/Plugins/Orders.NewCheckOut/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        public IActionResult Configure(OrderDivisionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var newCheckOutSettings = _settingService.LoadSetting<NewCheckOutSettings>(storeScope);
            newCheckOutSettings.DollerPrice = model.BaseDallerPrice;
            newCheckOutSettings.GlodPriceInGram = model.BaseGoldPriceInGram;
            newCheckOutSettings.RoleId = model.RoleId;
            _settingService.SaveSetting(newCheckOutSettings);
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            //ManageBackupJob(true, BackupTo.CurrentServer, "0 0/3 * 1/1 * ? *"); //test
            return Configure();

        }
        private List<SelectListItem> GetListOFRole(int RoleId)
        {
            return _customerService.GetAllCustomerRoles(true).Where(p => p.Active == true).Select(p =>
             new SelectListItem()
             {
                 Text = p.Name,
                 Value = p.Id.ToString(),
                 Selected = RoleId != 0 ? (p.Id == RoleId ? true : false) : false
             }).ToList();
        }
    }
}

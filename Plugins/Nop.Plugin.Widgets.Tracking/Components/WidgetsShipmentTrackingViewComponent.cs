using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.ShipmentTracking.Components
{
    [ViewComponent(Name = "WidgetsShipmentTracking")]
    public class WidgetsShipmentTrackingViewComponent : NopViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WidgetsShipmentTrackingViewComponent(
            IWorkContext workContext,
            IStoreContext storeContext,
            ISettingService settingService,
            IOrderService orderService,
            IHttpContextAccessor httpContextAccessor,
            ILogger logger)
        {
            _httpContextAccessor = httpContextAccessor;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._settingService = settingService;
            this._orderService = orderService;
            this._logger = logger;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            string host = _httpContextAccessor.HttpContext.Request.Host.Host.ToLower();
            string path = _httpContextAccessor.HttpContext.Request.Path.HasValue ? _httpContextAccessor.HttpContext.Request.Path.Value : "";
            if (host.Contains("postbar") || host.Contains("postbaar"))
                return View("~/Plugins/Widgets.ShipmentTracking/Views/TrackingInfo.cshtml", widgetZone);
            else if (path.ToLower().Contains("/ap/"))
                return View("~/Plugins/Widgets.ShipmentTracking/Views/Ap_TrackingInfo.cshtml", widgetZone);
            else if (path.ToLower().Contains("/sep/"))
                return View("~/Plugins/Widgets.ShipmentTracking/Views/Sep_TrackingInfo.cshtml", widgetZone);
            else if (_httpContextAccessor.HttpContext.Request.Host.Host.ToLower().Contains("postex"))
                return View("~/Plugins/Widgets.ShipmentTracking/Views/ptx_TrackingInfo.cshtml", widgetZone);
            return View("~/Plugins/Widgets.ShipmentTracking/Views/ptx_TrackingInfo.cshtml", widgetZone);
        }
    }
}
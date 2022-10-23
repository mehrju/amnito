using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Directory;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Kendoui;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    public class QualityControlController : BaseAdminController
    {
        private readonly IShipmentTrackingService _shipmentTrackingService;
        private readonly IPermissionService _permissionService;
        private readonly ICountryService _countryService;
        public QualityControlController(
            IShipmentTrackingService shipmentTrackingService
            , IPermissionService permissionService
            , ICountryService countryService
            )
        {
            _shipmentTrackingService = shipmentTrackingService;
            _permissionService = permissionService;
            _countryService = countryService;
        }
        public IActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var model = new QualityControlInputModel();
            //countries
            model.AvailableCountries.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            //states
            model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });

            return View("/Plugins/Orders.ExtendedShipment/Views/QualityControl.cshtml",model);
        }
        public IActionResult getQualityControlList(DataSourceRequest command, QualityControlInputModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();
            int count = 0;
            var QualityControlList = _shipmentTrackingService.getQualityControl(model.orderId
                , model.trackingNumber
                , model.countryId
                , model.stateId, model.orderState
                , model.dateFrom
                , model.dateTo,
                command.PageSize,
                command.Page - 1,
                out count);
            var gridModel = new DataSourceResult
            {
                Data = QualityControlList,
                Total = count
            };
            return Json(gridModel);
        }
    }
}

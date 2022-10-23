using Microsoft.AspNetCore.Mvc;
using Nop.plugin.Orders.ExtendedShipment.Services.ForiegnOrder;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    public class ForeignOrderController : BaseAdminController
    {
        private readonly IForeignOrder _foreignOrder;
        public ForeignOrderController(IForeignOrder foreignOrder)
        {
            _foreignOrder = foreignOrder;
        }
        public IActionResult Index()
        {
            return View("/Plugins/Orders.ExtendedShipment/Views/ForeignOrder/List.cshtml");
        }
        public IActionResult List(int ServiceId, int ForeignOrderStatus, int OrderId,
            DateTime? OrderDateFrom, DateTime? OrderDateTo, int page, int pageSize)
        {
            int count = 0;
            var reuslt = _foreignOrder.GetForeinOrdersList(ServiceId, ForeignOrderStatus, OrderId, OrderDateFrom, OrderDateTo, out count, page, pageSize);
            var gridModel = new DataSourceResult
            {
                Data = reuslt,
                Total = count
            };
            return Ok(gridModel);
        }
    }
}

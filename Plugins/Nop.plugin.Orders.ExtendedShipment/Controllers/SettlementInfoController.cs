using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.plugin.Orders.ExtendedShipment.Models.SettlementInfoLis;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    public class SettlementInfoController : BaseAdminController
    {
        private readonly IWorkContext _workContext;
        private readonly IAccountingService _accountingService;
        public SettlementInfoController(IWorkContext workContext, IAccountingService accountingService)
        {
            _workContext = workContext;
            _accountingService = accountingService;
        }
        public IActionResult Index()
        {
            if (!_workContext.CurrentCustomer.IsInCustomerRole("Administrators"))
                return AccessDeniedView();
            return View("~/Plugins/Orders.ExtendedShipment/Views/SettlementInfo/SettlementInfoList.cshtml");
        }
        [HttpPost]
        public IActionResult getBulkOrdersList(DataSourceRequest command
            , SettlementInfoLisModel model)
        {
            var dataCont = 0;
            var orders = _accountingService.SettlementList(model.FileId,model.UserName,model.OrderId,
                model.DepositDateFrom,model.DepositDateTo,model.SettlementDateFrom,model.SettlementDateTo,command.Page,command.PageSize,ref dataCont);
            var gridModel = new DataSourceResult
            {
                Data = orders.Select(x => new
                {
                    x.Username,
                    x.Message,
                    x.WalletOprationDate,
                    x.SettlementDate,
                    x.PaymentFileNo,
                    x.OrderId,
                    x.RefrenceCode,
                    x.Value,
                    x.FullName
                }),
                Total = dataCont
            };
            return Json(gridModel);
        }
    }
}

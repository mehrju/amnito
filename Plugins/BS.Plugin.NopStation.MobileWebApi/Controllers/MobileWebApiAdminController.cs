using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Payment;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class MobileWebApiAdminController : BasePublicController
    {
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IWebHelper _webHelper;
        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        public MobileWebApiAdminController(IWorkContext workContext, OrderSettings orderSettings,
            IOrderService orderService, IPaymentService paymentService,
            IWebHelper webHelper, ILogger logger,
            IStoreContext storeContext)
        {
            this._workContext = workContext;
            this._orderSettings = orderSettings;
            this._orderService = orderService;
            this._paymentService = paymentService;
            this._webHelper = webHelper;
            this._logger = logger;
            this._storeContext = storeContext;

        }

        
        public IActionResult Configure()
        {


            return View("~/Plugins/NopStation.MobileWebApi/Views/BsInstagramAdMarket/Configure.cshtml");
        }
    }
}

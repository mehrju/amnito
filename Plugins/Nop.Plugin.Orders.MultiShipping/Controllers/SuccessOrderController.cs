using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Http.Extensions;
using Nop.Core.Plugins;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Factories;
using Nop.Plugin.Orders.MultiShipping.Services;

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
    public class SuccessOrderController : Web.Controllers.CheckoutController
    {
        private readonly IRepository<Shipment> _repositoryTbl_Shipment;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShippingService _shippingService;
        private readonly IPaymentService _paymentService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IWebHelper _webHelper;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly OrderSettings _orderSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly AddressSettings _addressSettings;
        private readonly CustomerSettings _customerSettings;

        public SuccessOrderController(IRepository<Shipment> repositoryTbl_Shipment, ICheckoutModelFactory checkoutModelFactory, IWorkContext workContext, IStoreContext storeContext, IShoppingCartService shoppingCartService, ILocalizationService localizationService, IProductAttributeParser productAttributeParser, IProductService productService, IOrderProcessingService orderProcessingService, ICustomerService customerService, IGenericAttributeService genericAttributeService, ICountryService countryService, IStateProvinceService stateProvinceService, IShippingService shippingService, IPaymentService paymentService, IPluginFinder pluginFinder, ILogger logger, IOrderService orderService, IWebHelper webHelper, IAddressAttributeParser addressAttributeParser, IAddressAttributeService addressAttributeService, OrderSettings orderSettings, RewardPointsSettings rewardPointsSettings, PaymentSettings paymentSettings, ShippingSettings shippingSettings, AddressSettings addressSettings, CustomerSettings customerSettings) : base(checkoutModelFactory, workContext, storeContext, shoppingCartService, localizationService, productAttributeParser, productService, orderProcessingService, customerService, genericAttributeService, countryService, stateProvinceService, shippingService, paymentService, pluginFinder, logger, orderService, webHelper, addressAttributeParser, addressAttributeService, orderSettings, rewardPointsSettings, paymentSettings, shippingSettings, addressSettings, customerSettings)
        {
            this._repositoryTbl_Shipment = repositoryTbl_Shipment;
            this._checkoutModelFactory = checkoutModelFactory;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._shoppingCartService = shoppingCartService;
            this._localizationService = localizationService;
            this._productAttributeParser = productAttributeParser;
            this._productService = productService;
            this._orderProcessingService = orderProcessingService;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._shippingService = shippingService;
            this._paymentService = paymentService;
            this._pluginFinder = pluginFinder;
            this._logger = logger;
            this._orderService = orderService;
            this._webHelper = webHelper;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._orderSettings = orderSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._paymentSettings = paymentSettings;
            this._shippingSettings = shippingSettings;
            this._addressSettings = addressSettings;
            this._customerSettings = customerSettings;
        }

        public override IActionResult Completed(int? orderId)
        {
            //validation
            if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();
            Order order = null;
            if (orderId.HasValue)
            {
                //load order by identifier (if provided)
                order = _orderService.GetOrderById(orderId.Value);
            }
            if (order == null)
            {
                order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                    .FirstOrDefault();
            }
            //if (!Request.HttpContext.Session.Get<bool>("isFromApp"))
            //{
                if (order == null || order.Deleted)// || _workContext.CurrentCustomer.Id != order.CustomerId)
                {
                    return RedirectToRoute("HomePage");
                }
           // }
            //disable "order completed" page?
            //if (_orderSettings.DisableOrderCompletedPage)
            //{
            //    return RedirectToRoute("OrderDetails", new { orderId = order.Id });
            //}

            //model
            var model = new vm_SuccessOrder();
            model.OrderCode = order.Id.ToString();
            model.IsForeign= order.IsOrderForeign();
            model.NameCustomer = _workContext.CurrentCustomer.GetFullName();
            var t = _repositoryTbl_Shipment.Table.Where(p => p.OrderId == order.Id && p.TrackingNumber != null).ToList();
            if (t.Count == 1) { model.StateTrackingCode = true; model.TrackingCode = t.FirstOrDefault().TrackingNumber; }
            if (order.StoreId == 5)
            {
                return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/successOrder.cshtml", model);

            }
            else if (order.StoreId == 3)
            {
                return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Postbar/successOrder.cshtml", model);

            }
            else
            {
                string test = HttpContext.Session.GetString("ComeFrom");
                if (test == "Ap")
                    return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Ap/successOrder.cshtml", model);
                else
                    return Challenge();
            }
        }
    }
}

using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.PrintedReports_Requirements.Models;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Data;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Media;
using Nop.Services.Payments;
using Nop.Core.Domain.Customers;
using Nop.Services.Shipping;
using Nop.Core.Http.Extensions;

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
    public class SaleCartonwrapperController : BasePublicController
    {
        #region Field

        private readonly IShipmentService _shipmentService;
        private readonly IPackingRequestService _packingRequestService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly ICompareProductsService _compareProductsService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IOrderService _orderService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly CatalogSettings _catalogSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly IRewardPointService _rewardPointService;
        private readonly IRepository<ProductAttributeValue> _repositoryTbl_ProductAttributeValue;
        private readonly Nop.Plugin.Orders.MultiShipping.Services.INewCheckout _newCheckout;
        private readonly IApService _apService;
        #endregion

        #region ctor

        public SaleCartonwrapperController
            (
            IPictureService pictureService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            ICompareProductsService compareProductsService,
            IWorkflowMessageService workflowMessageService,
            IOrderService orderService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            CatalogSettings catalogSettings,
            ShoppingCartSettings shoppingCartSettings,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings,
            IRewardPointService rewardPointService,
            IRepository<ProductAttributeValue> repositoryTbl_ProductAttributeValue,
            INewCheckout newCheckout,
            IProductAttributeService productAttributeService,
            IOrderProcessingService orderProcessingService,
            IPaymentService paymentService,
            IApService apService,
            IShipmentService shipmentService,
            IPackingRequestService packingRequestService
            )
        {
            _shipmentService = shipmentService;
            _packingRequestService = packingRequestService;
            _paymentService = paymentService;
            _orderProcessingService = orderProcessingService;
            _pictureService = pictureService;
            _productAttributeService = productAttributeService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _workContext = workContext;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _compareProductsService = compareProductsService;
            _workflowMessageService = workflowMessageService;
            _orderService = orderService;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _permissionService = permissionService;
            _customerActivityService = customerActivityService;
            _eventPublisher = eventPublisher;
            _catalogSettings = catalogSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _localizationSettings = localizationSettings;
            _captchaSettings = captchaSettings;
            _rewardPointService = rewardPointService;
            _repositoryTbl_ProductAttributeValue = repositoryTbl_ProductAttributeValue;
            _newCheckout = newCheckout;
            _apService = apService;
        }

        #endregion

        public bool IsValidCustomer()
        {
            var customer = _workContext.CurrentCustomer;
            if (customer == null || !customer.IsRegistered() || customer.IsGuest() || customer.Username == null)
                return false;
            return true;
        }
        public IActionResult ProductDetail()
        {
            var err = TempData["error"];
            if (!string.IsNullOrEmpty(err?.ToString()))
            {
                ModelState.AddModelError("", err.ToString());
            }
            if (!IsValidCustomer())
                return RedirectToRoute("Login", new { returnUrl = Url.RouteUrl("SaleCartonwrapper") });
            #region product
            int productId = 10430;
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
            {
                if (_storeContext.CurrentStore.Id == 5)
                    return RedirectToRoute("_ShipitoHome");
                else
                {
                    return RedirectToRoute("PostbarHome");
                }
            }


            var notAvailable =
                //published?
                (!product.Published && !_catalogSettings.AllowViewUnpublishedProductPage) ||
                //ACL (access control list) 
                !_aclService.Authorize(product) ||
                //Store mapping
                !_storeMappingService.Authorize(product) ||
                //availability dates
                !product.IsAvailable();
            //Check whether the current user has a "Manage products" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            if (notAvailable)
            {
                if (_storeContext.CurrentStore.Id == 5)
                    return RedirectToRoute("_ShipitoHome");
                else
                {
                    return RedirectToRoute("PostbarHome");
                }
            }
            //save as recently viewed
            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);
            _customerActivityService.InsertActivity("PublicStore.ViewProduct", _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name);

            var pd = _productModelFactory.PrepareProductDetailsModel(product, null, false);
            foreach (var item in pd.ProductAttributes)
            {
                foreach (var itemValue in item.Values)
                {
                    var pictureThumbnailUrl = _pictureService.GetPictureUrl(itemValue.PictureId, 100, false);
                    itemValue.ImageSquaresPictureModel = new Web.Models.Media.PictureModel() { ImageUrl = pictureThumbnailUrl };
                }
            }
            #endregion
            #region list payment
            var PaymentMethod = _newCheckout.getPaymentMethodForSaleCarton();
            #endregion
            var model = new vm_SaleCartonWrapper1()
            {
                ProductDetailsModel = pd,
                PaymentMethods = PaymentMethod
            };
            return View("/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/SaleCartonwrapper/Index.cshtml", model);
        }
        [HttpPost]
        public IActionResult ConfirmAndPaySaleCarton(string data)
        {
            string error = "• ";
            if (string.IsNullOrEmpty(data))
            {
                error = "• اطلاعات وارد شده نا معتبری می باشد";
                TempData["error"] = error;
                return RedirectToRoute("SaleCartonwrapper");
            }
            var _CartonSaleModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CartonSaleModel>(data);
            if (_CartonSaleModel == null)
            {
                error = "• اطلاعات وارد شده نا معتبری می باشد";
                TempData["error"] = error;
                return RedirectToRoute("SaleCartonwrapper");
            }
            var RelatedShipment = _shipmentService.GetShipmentById(_CartonSaleModel.ShipmentId);

            if (RelatedShipment == null)
            {
                error = "• شماره محموله وارد شده نامعتبر می باشد";
                TempData["error"] = error;
                return RedirectToRoute("SaleCartonwrapper");
            }
            if (RelatedShipment.Order.CustomerId != _workContext.CurrentCustomer.Id)
            {
                error = "• شماره محموله وارد شده متعلق به شما نمی باشد";
                TempData["error"] = error;
                return RedirectToRoute("SaleCartonwrapper");
            }
            if (!_packingRequestService.IsRequestedPackingPurchased(_CartonSaleModel.ShipmentId, _CartonSaleModel.List_Sizeitem, out string msg))
            {

                error = msg;
                TempData["error"] = msg;
                //return View("/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/SaleCartonwrapper/Index.cshtml");
                return RedirectToRoute("SaleCartonwrapper");
            }
            HttpContext.Session.Set("isFromApp", _CartonSaleModel.isFromApp);
            _CartonSaleModel.OrderId = RelatedShipment.OrderId;
            if (_CartonSaleModel.UseRewardPoints)
            {
                int rewardPointsBalance =
                       _rewardPointService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
                _CartonSaleModel.Amount += (_CartonSaleModel.Amount * 9 / 100);
                if (_CartonSaleModel.Amount > rewardPointsBalance)
                {
                    error = "• موجودی کیف پول برای این سفارش می بایست حداقل " + Convert.ToInt32(_CartonSaleModel.Amount).ToString("N0")
                                                                                     + " ريال باشد. موجودی فعلی " + rewardPointsBalance.ToString("N0");
                    TempData["error"] = error;
                    return RedirectToRoute("SaleCartonwrapper");
                }
            }

            var result = _newCheckout.ProccessCartonOrder(_CartonSaleModel);
            if (result.Success)
            {
                error = null;
                var order = result.PlacedOrder;
                if (order.OrderStatus == OrderStatus.Cancelled)
                {
                    error = "• سفارش شما کنسل شده و امکان پرداخت برای آن وجود ندارد";
                    return Redirect(Url.RouteUrl("_Sh_BillAndPayment") + "?orderIds[0]=" + order.Id + "&msg=1");
                }

                if (order.PaymentStatus == Core.Domain.Payments.PaymentStatus.Paid)
                    return RedirectToRoute("CheckoutCompleted", new
                    {
                        orderId = order.Id
                    });

                if (_CartonSaleModel.UseRewardPoints)
                {
                    int rewardPointsBalance =
                           _rewardPointService.GetRewardPointsBalance(order.CustomerId, order.StoreId);
                    if (order.OrderTotal > rewardPointsBalance)
                    {
                        error = "• موجودی کیف پول برای این سفارش می بایست حداقل " + Convert.ToInt32(order.OrderTotal).ToString("N0")
                                                                                         + " ريال باشد. موجودی فعلی " + rewardPointsBalance.ToString("N0");
                        TempData["error"] = error;
                        return RedirectToRoute("SaleCartonwrapper");
                    }
                    else
                    {
                        //TODO : reward point
                        _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, -Convert.ToInt32(order.OrderTotal), order.StoreId,
                        string.Format(_localizationService.GetResource("RewardPoints.Message.RedeemedForOrder", order.CustomerLanguageId), order.CustomOrderNumber),
                        order, order.OrderTotal);
                        order.OrderTotal = 0;
                        order.PaymentMethodSystemName = null;
                        _orderProcessingService.MarkOrderAsPaid(order);
                        return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });

                    }
                }
                order.PaymentMethodSystemName = _CartonSaleModel.paymentmethod;
                _orderService.UpdateOrder(order);
                var postProcessPaymentRequest = new PostProcessPaymentRequest
                {
                    Order = order
                };
                _paymentService.PostProcessPayment(postProcessPaymentRequest);

                if (_CartonSaleModel.isFromApp)
                {
                    return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/OpenExternalBrowser.cshtml", HttpContext.Session.Get<string>("redirectUrl"));
                }
                if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                {
                    return Content("Redirected");
                }

                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }
            else
            {
                error = "• موارد زیر را بررسی کنید" + "\r\n" + string.Join("\r\n", result.Errors);
                TempData["error"] = error;
                return RedirectToRoute("SaleCartonwrapper");
            }
        }

        public IActionResult ApProductDetail()
        {
            if (!IsValidCustomer())
            {
                _newCheckout.Log("خطا در زمان اعتبار سنجی کاربر آپ", "");
                return Content("در حال حاضر امکان دسترسی به این سرویس برای شما مقدور نمی باشد. مجددا سعی کنید");
            }
            #region product
            int productId = 10430;
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
            {
                return RedirectToRoute("_ApStartup");
            }

            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);
            _customerActivityService.InsertActivity("PublicStore.ViewProduct", _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name);

            var pd = _productModelFactory.PrepareProductDetailsModel(product, null, false);
            foreach (var item in pd.ProductAttributes)
            {
                foreach (var itemValue in item.Values)
                {
                    var pictureThumbnailUrl = _pictureService.GetPictureUrl(itemValue.PictureId, 300, false);
                    if (!string.IsNullOrEmpty(pictureThumbnailUrl))
                    {
                        pictureThumbnailUrl = pictureThumbnailUrl.Replace("http://localhost:55390", "..");
                        pictureThumbnailUrl = pictureThumbnailUrl.Replace("https://postex.ir", "..");
                        pictureThumbnailUrl = pictureThumbnailUrl.Replace("http://postex.ir", "..");
                    }
                    itemValue.ImageSquaresPictureModel = new Web.Models.Media.PictureModel() { ImageUrl = pictureThumbnailUrl };
                }
            }
            string error = "• ";
            #endregion
            var model = new vm_SaleCartonWrapper1()
            {
                ProductDetailsModel = pd,
            };
            return View("/Plugins/Orders.MultiShipping/Views/NewCheckout/Ap/SaleCartonwrapper/Index.cshtml", model);
        }
        [HttpPost]
        public IActionResult ApConfirmAndPaySaleCarton(string data)
        {
            string error = "• ";
            if (string.IsNullOrEmpty(data))
            {
                error = "• اطلاعات وارد شده نا معتبر می باشد";
                return Json(new { success = false, message = error });

            }
            var _CartonSaleModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CartonSaleModel>(data);
            if (_CartonSaleModel == null)
            {
                error = "• اطلاعات وارد شده نا معتبر می باشد";
                return Json(new { success = false, message = error });
            }
            var RelatedOrder = _orderService.GetOrderById(_CartonSaleModel.OrderId);
            if (RelatedOrder == null)
            {
                error = "• شماره سفارش وارد شده نامعتبر می باشد";
                return Json(new { success = false, message = error });
            }


            var result = _newCheckout.ProccessCartonOrder(_CartonSaleModel);
            if (result.Success)
            {
                var order = result.PlacedOrder;
                error = "• ";
                var _paymentRequest = _apService.CreatePaymentRequestForCatoon(order.Id, out error);
                return Json(new { success = true, message = "", paymentRequest = Newtonsoft.Json.JsonConvert.SerializeObject(_paymentRequest) });
            }
            else
            {
                error = "• موارد زیر را بررسی کنید" + "\r\n" + string.Join("\r\n", result.Errors);
                return Json(new { success = false, message = error });
            }
        }

        public IActionResult getPaymentRequest(int orderId)
        {

            Order O = _orderService.GetOrderById(orderId);
            int catId = O.OrderItems.First().Product.ProductCategories.First().CategoryId;
            if (catId == 720)
            {
                string error = "• ";
                var _paymentRequest = _apService.CreatePaymentRequestForCatoon(orderId, out error);
                return Json(new { success = true, catId = catId, paymentRequest = Newtonsoft.Json.JsonConvert.SerializeObject(_paymentRequest) });
            }
            else
            {
                string error = "• ";
                var _paymentRequest = _apService.CreatePaymentRequest(orderId, out error);
                return Json(new { success = true, catId = catId, paymentRequest = Newtonsoft.Json.JsonConvert.SerializeObject(_paymentRequest) });
            }

        }

        public IActionResult SepGetPaymentRequest(int orderId)
        {
            Order O = _orderService.GetOrderById(orderId);
            //string _paymentRequest = $@"seppay://{12152457}/{O.Id}/0|{Convert.ToInt32(O.OrderTotal)}";
            string _paymentRequest = $@"seppay://{396}/{O.Id}/0|{Convert.ToInt32(O.OrderTotal)}";
            int catId = O.OrderItems.First().Product.ProductCategories.First().CategoryId;
            return Json(new { success = true, catId = catId, paymentRequest = _paymentRequest });
        }

        public IActionResult SepProductDetail()
        {
            if (!IsValidCustomer())
            {
                _newCheckout.Log("خطا در زمان اعتبار سنجی کاربر آپ", "");
                return Content("در حال حاضر امکان دسترسی به این سرویس برای شما مقدور نمی باشد. مجددا سعی کنید");
            }
            #region product
            int productId = 10430;
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
            {
                return RedirectToRoute("_SepStartup");
            }

            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);
            _customerActivityService.InsertActivity("PublicStore.ViewProduct", _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name);

            var pd = _productModelFactory.PrepareProductDetailsModel(product, null, false);
            foreach (var item in pd.ProductAttributes)
            {
                foreach (var itemValue in item.Values)
                {
                    var pictureThumbnailUrl = _pictureService.GetPictureUrl(itemValue.PictureId, 300, false);
                    if (!string.IsNullOrEmpty(pictureThumbnailUrl))
                    {
                        pictureThumbnailUrl = pictureThumbnailUrl.Replace("http://localhost:55390", "..");
                        pictureThumbnailUrl = pictureThumbnailUrl.Replace("https://postex.ir", "..");
                        pictureThumbnailUrl = pictureThumbnailUrl.Replace("http://postex.ir", "..");
                    }
                    itemValue.ImageSquaresPictureModel = new Web.Models.Media.PictureModel() { ImageUrl = pictureThumbnailUrl };
                }
            }
            string error = "• ";
            #endregion
            var model = new vm_SaleCartonWrapper1()
            {
                ProductDetailsModel = pd,
            };
            return View("/Plugins/Orders.MultiShipping/Views/NewCheckout/Sep/SaleCartonwrapper/Index.cshtml", model);
        }
        [HttpPost]
        public IActionResult SepConfirmAndPaySaleCarton(string data)
        {
            string error = "• ";
            if (string.IsNullOrEmpty(data))
            {
                error = "• اطلاعات وارد شده نا معتبری می باشد";
                return Json(new { success = false, message = error });
            }
            var _CartonSaleModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CartonSaleModel>(data);
            if (_CartonSaleModel == null)
            {
                error = "• اطلاعات وارد شده نا معتبری می باشد";
                TempData["error"] = error;
                return Json(new { success = false, message = error });
            }
            var RelatedShipment = _shipmentService.GetShipmentById(_CartonSaleModel.ShipmentId);

            if (RelatedShipment == null)
            {
                error = "• شماره محموله وارد شده نامعتبر می باشد";
                return Json(new { success = false, message = error });
            }
            if (RelatedShipment.Order.CustomerId != _workContext.CurrentCustomer.Id)
            {
                error = "• شماره محموله وارد شده متعلق به شما نمی باشد";
                return Json(new { success = false, message = error });
            }
            if (!_packingRequestService.IsRequestedPackingPurchased(_CartonSaleModel.ShipmentId, _CartonSaleModel.List_Sizeitem, out string msg))
            {

                error = msg;
                return Json(new { success = false, message = error });
            }
            _CartonSaleModel.OrderId = RelatedShipment.OrderId;


            var result = _newCheckout.ProccessCartonOrder(_CartonSaleModel);
            if (result.Success)
            {
                error = null;
                var order = result.PlacedOrder;
                if (order.OrderStatus == OrderStatus.Cancelled)
                {
                    error = "• سفارش شما کنسل شده و امکان پرداخت برای آن وجود ندارد";
                    return Json(new { success = false, message = error });
                }

                if (order.PaymentStatus == Core.Domain.Payments.PaymentStatus.Paid)
                {
                    error = "• سفارش خرید کارتن ولفاف بندی قبلا پرداخت شده";
                    return Json(new { success = false, message = error });
                }

                order.PaymentMethodSystemName = "NopFarsi.Payments.SepShaparak";
                _orderService.UpdateOrder(order);
                var PaymentUrl = $@"seppay://{396}/{order.Id}/0|{Convert.ToInt32(order.OrderTotal)}";
                return Json(new { success = true, message = "", paymentRequest = PaymentUrl });
            }
            else
            {
                error = "• موارد زیر را بررسی کنید" + "\r\n" + string.Join("\r\n", result.Errors);
                return Json(new { success = false, message = error });
            }
        }
    }
}

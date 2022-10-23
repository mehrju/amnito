using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using BS.Plugin.NopStation.MobileWebApi.Factories;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Microsoft.AspNetCore.Mvc;
using Nop.Data;
using Nop.Services.Customers;
using Nop.plugin.Orders.ExtendedShipment.Services;
using BS.Plugin.NopStation.MobileWebApi.Models.Order;
using Nop.Plugin.Orders.BulkOrder.Models;
using Nop.Core.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using BS.Plugin.Orders.ExtendedShipment.Services;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public partial class OrderController : BaseApiController
    {
        #region Fields
        private readonly ISekehService _sekeService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IShipmentTrackingService _shipmentTrackingService;
        private readonly IRepository<ShipmentEventModel> _repository_ShipmentEvent;
        private readonly IRepository<Tbl_ShipmentEventCategory> _repository_ShipmentevenCategory;
        private readonly IDbContext _dbContext;
        private IOrderModelFactoryApi _orderModelFactoryApi;
        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPaymentService _paymentService;
        private readonly ILocalizationService _localizationService;
        private readonly IPdfService _pdfService;
        private readonly IShippingService _shippingService;
        private readonly ICountryService _countryService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IWebHelper _webHelper;
        private readonly IDownloadService _downloadService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IStoreContext _storeContext;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPictureService _pictureService;
        private readonly OrderSettings _orderSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly AddressSettings _addressSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly ICacheManager _cacheManager;
        private readonly ICustomerService _customerService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IOptimeApiService _optimeApiService;
        #endregion

        #region Constructors

        public OrderController(IRewardPointService rewardPointService,
            ICustomerService customerService,
            IDbContext dbContext,
            IOrderModelFactoryApi orderModelFactoryApi,
            IOrderService orderService,
            IShipmentService shipmentService,
            IWorkContext workContext,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            IOrderProcessingService orderProcessingService,
            IDateTimeHelper dateTimeHelper,
            IPaymentService paymentService,
            ILocalizationService localizationService,
            IPdfService pdfService,
            IShippingService shippingService,
            ICountryService countryService,
            IProductAttributeParser productAttributeParser,
            IWebHelper webHelper,
            IDownloadService downloadService,
            IAddressAttributeFormatter addressAttributeFormatter,
            IStoreContext storeContext,
            IOrderTotalCalculationService orderTotalCalculationService,
            CatalogSettings catalogSettings,
            OrderSettings orderSettings,
            TaxSettings taxSettings,
            ShippingSettings shippingSettings,
            AddressSettings addressSettings,
            RewardPointsSettings rewardPointsSettings,
            PdfSettings pdfSettings,
            IPictureService pictureService,
            MediaSettings mediaSettings,
            ICacheManager cacheManager,
            IExtendedShipmentService extendedShipmentService,
            IShipmentTrackingService shipmentTrackingService,
            IRepository<ShipmentEventModel> repository_shipmentEvent,
            IRepository<Tbl_ShipmentEventCategory> repository_shipmentevenCategory,
            ISekehService sekeService, IOptimeApiService optimeApiService)
        {
            _extendedShipmentService = extendedShipmentService;
            _shipmentTrackingService = shipmentTrackingService;
            _repository_ShipmentEvent = repository_shipmentEvent;
            _repository_ShipmentevenCategory = repository_shipmentevenCategory;
            _rewardPointService = rewardPointService;
            _dbContext = dbContext;
            _sekeService = sekeService;
            this._orderModelFactoryApi = orderModelFactoryApi;
            this._orderService = orderService;
            this._shipmentService = shipmentService;
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._dateTimeHelper = dateTimeHelper;
            this._paymentService = paymentService;
            this._localizationService = localizationService;
            this._pdfService = pdfService;
            this._shippingService = shippingService;
            this._countryService = countryService;
            this._productAttributeParser = productAttributeParser;
            this._webHelper = webHelper;
            this._downloadService = downloadService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._storeContext = storeContext;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._customerService = customerService;
            this._catalogSettings = catalogSettings;
            this._orderSettings = orderSettings;
            this._taxSettings = taxSettings;
            this._shippingSettings = shippingSettings;
            this._addressSettings = addressSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._pdfSettings = pdfSettings;
            this._pictureService = pictureService;
            this._mediaSettings = mediaSettings;
            this._cacheManager = cacheManager;
            _optimeApiService = optimeApiService;
        }

        #endregion

        //#region Methods

        ////My account / Orders api
        //[HttpGet]
        //[Route("api/order/customerorders")]
        //public IActionResult CustomerOrders()
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //       return Challenge(HttpStatusCode.Unauthorized.ToString());

        //    var model = _orderModelFactoryApi.PrepareCustomerOrderListModel();
        //    return Ok(model);
        //}


        //[HttpGet]
        //[Route("api/order/details/{orderId}")]
        //public IActionResult Details(int orderId)
        //{
        //    var order = _orderService.GetOrderById(orderId);
        //    if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
        //        return  Challenge(HttpStatusCode.Unauthorized.ToString());

        //    var model = _orderModelFactoryApi.PrepareOrderDetailsModel(order);

        //    return Ok(model);
        //}
        [HttpGet]
        [Route("api/customer/getWalletChargeRate")]
        public IActionResult getWalletChargeRate(int customerId, string mobileNo)
        {
            Customer customer = null;
            if (customerId > 0)
                customer = _customerService.GetCustomerById(customerId);
            else if (!string.IsNullOrEmpty(mobileNo))
                customer = _customerService.GetCustomerByUsername(mobileNo);
            if (customer == null)
                return Json(new { resultCode = 0, message = "کاریری با این شناسه/موبایل یافت نشد" });
            int reqPointsBalance = _rewardPointService.GetRewardPointsBalance(customer.Id, 3);
            return Json(new { resultCode = 0, message = "", walletChargeRate = reqPointsBalance });
        }
        [HttpGet]
        [Route("api/customer/getWalletBalance")]
        public IActionResult _getWalletChargeRate()
        {
            var customer = _workContext.CurrentCustomer;
            if (customer == null)
                return Json(new { resultCode = 0, message = "کاریری با این شناسه/موبایل یافت نشد" });
            int reqPointsBalance = _rewardPointService.GetRewardPointsBalance(customer.Id, 5);
            return Json(new { resultCode = 0, message = "", walletBalance = reqPointsBalance });
        }
        [HttpGet]
        [Route("api/customer/WalletBalance")]
        public IActionResult _getWalletChargeRate_()
        {
            Customer customer = _workContext.CurrentCustomer;
            string _mobileNo = customer.Username;
            if (customer == null)
                return Json(new { resultCode = 0, message = "کاریری با این شناسه/موبایل یافت نشد" });
            int reqPointsBalance = _rewardPointService.GetRewardPointsBalance(customer.Id, 5);
            return Json(new { success = true, message = "", walletBalance = reqPointsBalance });
        }
        [HttpGet]
        [Route("api/order/TrackingNumber/{orderId}")]
        public IActionResult getTrackingNumber(int orderId)
        {
            if(orderId <=0)
            {
                return Json(new { resultCode = 15, message = "شماره سفارش وارد شده نامعتبر می باشد" });
            }
            var order = _orderService.GetOrderById(orderId);
                
            if(order== null || order.Shipments== null || !order.Shipments.Any())
                return Json(new { resultCode = 15, message = "اطلاعات رهگیری در حال حاضر موجود نمی باشد" });
            
            if (!order.Shipments.Any())
                return Json(new { resultCode = 15, message = "اطلاعات رهگیری در حال حاضر موجود نمی باشد" });
            if (order.Shipments.All(p => string.IsNullOrEmpty(p.TrackingNumber)))
                return Json(new { resultCode = 15, message = "اطلاعات رهگیری در حال حاضر موجود نمی باشد" });
            SqlParameter[] prms = new SqlParameter[] {
                new SqlParameter() { ParameterName = "@orderId", SqlDbType = SqlDbType.Int,Value = orderId },
            };
            var orderShipmentTracking = _dbContext.SqlQuery<OrderShipmentTracking>(@"EXECUTE [dbo].[Sp_OrderShipmentTracking] @orderId", prms).ToList();
            return Json(new { resultCode = 0, message = "", data = orderShipmentTracking });
        }

        public class OrderShipmentTracking
        {
            public string TrackingNumber { get; set; }
            public string shipmentStatus { get; set; }
        }
        ////My account / Order details page / PDF invoice
        [HttpGet]
        [Route("api/order/getpdfinvoice/{orderId}")]
        //My account / Order details page / PDF invoice
        public IActionResult GetPdfInvoice(int orderId)
        {
            //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
            {
                return Unauthorized();
            }
            var orders = new List<Order>();
            orders.Add(order);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintOrdersToPdf(stream, orders, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, $"order_{order.Id}.pdf");
        }
        [HttpGet]
        [Route("api/order/getpdfinvoice50M/{orderId}")]
        public virtual IActionResult GetPdfInvoiceMM(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
            {
                return NotFound();
            }


            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _extendedShipmentService.PrintLable50MM(order, stream);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, $"order_{order.Id}.pdf");
        }

        [HttpGet]
        [Route("api/order/GetStatusByCategoryId/{categoryId}")]
        public IActionResult GetStatusByCategoryId([FromRoute] int categoryId)
        {
            var shipmentEventIds = _repository_ShipmentevenCategory.TableNoTracking.Where(p => p.CategoryId == categoryId).Select(p => p.ShipmentEventId).ToList();

            var shipmentevents = _repository_ShipmentEvent.TableNoTracking.Where(p => shipmentEventIds.Contains(p.ShipmentEventId)).Select(p => new
            {
                StatusId = p.ShipmentEventId,
                Name = p.ShipmentEventName
            }).ToList();

            return Ok(shipmentevents);
        }

        [HttpGet]
        [Route("api/order/ChangeStatus")]
        public IActionResult ChangeStatus([FromQuery] int shipmentId, [FromQuery] int statusId, [FromQuery] string description)
        {
            var shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment == null)
            {
                return Ok(new OrderCancelResultModel()
                {
                    Message = ApiMessage.GetErrorMsg(13),
                    ResultCode = 13,
                    Success = false
                });
            }

            if (!_repository_ShipmentEvent.TableNoTracking.Any(p => p.ShipmentEventId == statusId.ToString()))
            {
                return Ok(new OrderCancelResultModel()
                {
                    Message = ApiMessage.GetErrorMsg(25),
                    ResultCode = 25,
                    Success = false
                });
            }

            _shipmentTrackingService.InsertTracking(shipmentId, statusId, description);

            return Ok(new OrderCancelResultModel()
            {
                Message = "عملیات با موفقیت انجام شد",
                ResultCode = 0,
                Success = true
            });
        }

        [HttpGet]
        [Route("api/order/cancel/{orderId}")]
        public IActionResult Cancel([FromRoute] int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null)
            {
                return Ok(new OrderCancelResultModel()
                {
                    Message = ApiMessage.GetErrorMsg(13),
                    ResultCode = 13,
                    Success = false
                });
            }
            if (order.CustomerId != _workContext.CurrentCustomer.Id)
            {
                return Ok(new OrderCancelResultModel()
                {
                    Message = ApiMessage.GetErrorMsg(23),
                    ResultCode = 23,
                    Success = false
                });
            }
            SqlParameter[] prms = new SqlParameter[] {
                new SqlParameter() { ParameterName = "@OrderId", SqlDbType = SqlDbType.Int,Value = orderId },
            };
            var queryResult = _dbContext.SqlQuery<int>("EXECUTE [dbo].[Sp_IsOrderCancelable] @OrderId", prms).FirstOrDefault();
            if (queryResult == 0)
            {
                return Ok(new OrderCancelResultModel()
                {
                    Message = ApiMessage.GetErrorMsg(24),
                    ResultCode = 24,
                    Success = false
                });
            }
            _orderProcessingService.CancelOrder(order, true);
            return Ok(new OrderCancelResultModel()
            {
                Message = "سفارش شما کنسل شد",
                ResultCode = 0,
                Success = true
            });
        }
      
        //////My account / Order details page / re-order
        //[HttpGet]
        //[Route("api/order/reorder/{orderId}")]
        //public IActionResult ReOrder(int orderId)
        //{
        //    var order = _orderService.GetOrderById(orderId);
        //    if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
        //        return Challenge();

        //    var response = new GeneralResponseModel<bool>()
        //    {
        //        Data = true
        //    };
        //    _orderProcessingService.ReOrder(order);

        //    return Ok(response);
        //}


        //[HttpGet]
        //[Route("api/order/shipmentdetails/{shipmentId}")]
        //public IActionResult ShipmentDetails(int shipmentId)
        //{
        //    var shipment = _shipmentService.GetShipmentById(shipmentId);
        //    if (shipment == null)
        //        return Challenge();

        //    var order = shipment.Order;
        //    if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
        //        return Challenge();

        //    var model = _orderModelFactoryApi.PrepareShipmentDetailsModel(shipment);

        //    return Ok(model);
        //}

        //#endregion
    }
}

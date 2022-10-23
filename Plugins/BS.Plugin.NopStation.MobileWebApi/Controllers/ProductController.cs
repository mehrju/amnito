using BS.Plugin.NopStation.MobileWebApi.Models.Product;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.plugin.Orders.ExtendedShipment.Services;
//using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Plugin.Orders.BulkOrder.Models;
using Nop.Plugin.Orders.BulkOrder.Services;
using Nop.Plugin.Orders.MultiShipping.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class ProductController : BaseApiController
    {
        #region Field
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IBulkOrderService _bulkOrderService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IProductServiceApi _productServiceApi;
        private readonly INewCheckout _newCheckout;

        #endregion

        #region Ctor
        public ProductController(
            IWorkContext workContext,
            IStoreContext storeContext,
            IProductServiceApi productServiceApi,
            IBulkOrderService bulkOrderService,
            IExtendedShipmentService extendedShipmentService,
            INewCheckout newCheckout
            )
        {
            _newCheckout = newCheckout;
            this._bulkOrderService = bulkOrderService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._productServiceApi = productServiceApi;
            this._extendedShipmentService = extendedShipmentService;
        }
        #endregion

        #region New Action Method
        [Route("api/getInsuranceList")]
        [HttpGet]
        public IActionResult InsuranceList(uint serviceId)
        {
            var product = _bulkOrderService.DetectProduct((int)serviceId);
            if (product == null)
                return Json(new { resultCode = 5, message = ApiMessage.GetErrorMsg(5) });
            var data = _productServiceApi.getAttributeLit(product.Id, "بیمه");
            return Json(new { resultCode = 0, message = "", data });
        }
        [Route("api/getCartonSizeList")]
        [HttpGet]
        public IActionResult CartonSizeList(uint serviceId)
        {
            var product = _bulkOrderService.DetectProduct((int)serviceId);
            if (product == null)
                return Json(new { resultCode = 5, message = ApiMessage.GetErrorMsg(5) });
            var data = _productServiceApi.getAttributeLit(product.Id, "لفاف");
            return Json(new { resultCode = 0, message = "", data });
        }
        [Route("api/getServiceList")]
        [HttpGet]
        public IActionResult GetServiceList()
        {
            if (_storeContext.CurrentStore.Id != 5)
                return Json(new PostService() { ServiceId = 0, ServiceName = "سرویس مورد نظر با این آدرس درسترس نمی باشد" });
            return Json(_productServiceApi.ListOfService());
        }
        [Route("api/getPrice")]
        [HttpPost]
        public IActionResult CalcPrice(ProductPriceModelApi model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge(HttpStatusCode.Unauthorized.ToString());

            if (model == null)
                return Json(new { resultCode = 1, message = ApiMessage.GetErrorMsg(1), price = 0 });

            string error = "";
            if (!model.IsValid(out error))
            {
                return Json(new { resultCode = 4, message = ApiMessage.GetErrorMsg(4) + " " + error, price = 0 });
            }
            var product = _bulkOrderService.DetectProduct((int)model.ServiceId);
            if (product == null)
                return Json(new { resultCode = 4, message = ApiMessage.GetErrorMsg(5), price = 0 });

            if (!model.IsCod)
            {
                var attrPrice =
                    _bulkOrderService.getCheckoutAttributePrice((int)model.Weight, model.InsuranceName, model.CartonSizeName, model.IsCod,
                        product.Id, model.AccessPrintBill);
                if (!attrPrice.HasValue)
                    return Json(new { resultCode = 1, message = ApiMessage.GetErrorMsg(1), price = 0 });
                int totalPrice = Convert.ToInt32(product.Price + attrPrice.Value);
                totalPrice += ((totalPrice * 9) / 100);
                return Json(new { resultCode = 0, message = "قیمت اعلام شده شامل مالیات می باشد", price = totalPrice });
            }
            else
            {
                error = "";
                int? postType = _productServiceApi.GetPostType((int)model.ServiceId);
                if (!postType.HasValue || postType.Value == 0)
                {
                    return Json(new { resultCode = 2, message = ApiMessage.GetErrorMsg(2), price = 0 });
                }

                int serviceTypeForCod = postType.Value == 11 ? 1 : 0;
                var totalPrice = _bulkOrderService.CalcCodPrice(product, (int)model.Weight, _workContext.CurrentCustomer.Username
                     , (int)model.StateProvinceId, (int)model.CityId, serviceTypeForCod, model.CartonSizeName
                     , model.InsuranceName, (int)model.CodGoodsPrice, model.AccessPrintBill, out error);
                int result = string.IsNullOrEmpty(error) ? 0 : 3;
                error = string.IsNullOrEmpty(error) ? "قیمت اعلام شده شامل مالیات می باشد" : error;
                return Json(new { resultCode = result, message = error, price = totalPrice });
            }
        }

        [Route("api/newGetPrice")]
        [HttpPost]
        public IActionResult CalcPrice([FromBody] OrderPriceInput model)
        {
            //var o = _apiOrderRefrenceCodeService.CheckAndInsertApiOrderRefrenceCode(_workContext.CurrentCustomer.Id, "234342", out Nop.plugin.Orders.ExtendedShipment.Domain.Tbl_ApiOrderRefrenceCode tbl_ApiOrderRefrenceCode);
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge(HttpStatusCode.Unauthorized.ToString());

            if (model == null)
                return Json(new ProductPriceResult { Message = ApiMessage.GetErrorMsg(1), ResultCode = 1 });
            string error = "";
            if (_workContext.CurrentCustomer.IsInCustomerRole("Nasher"))
            {
                if (model.CartonSizeName == "سایز A4(31*22)")
                    model.Weight = 2000;
                else if (model.CartonSizeName == "سایز A3(30*45)")
                    model.Weight = 4900;
                else if (model.CartonSizeName == "سایز 3(15*20*20)")
                    model.Weight = 7000;
                else
                {
                    return Json(new ProductPriceResult { Message = "سایز بسته وارد شده برای شما نا معتبر می با شد", ResultCode = -1 });
                }
                var Dimention = _newCheckout.getDimentionByName(model.CartonSizeName);
                if (Dimention != null)
                {
                    if (model.PackingDimension == null)
                        model.PackingDimension = new PackingDimension();
                    model.PackingDimension.Width = Dimention.Width;
                    model.PackingDimension.Height = (Dimention.Height == 0 ? 2 : Dimention.Height);
                    model.PackingDimension.Length = Dimention.Length;
                }
                else
                {
                    return Json(new ProductPriceResult { Message = "سایز بسته وارد شده برای شما نا معتبر می با شد", ResultCode = -1 });
                }

            }

            if ((model.PackingDimension == null || model.PackingDimension.Height == 0 || model.PackingDimension.Length == 0 || model.PackingDimension.Width == 0)
                && !string.IsNullOrEmpty(model.CartonSizeName) &&  model.CartonSizeName !="کارتن نیاز ندارم.")

            {
                var Dimention = _newCheckout.getDimentionByName(model.CartonSizeName);
                if(Dimention  == null)
                {
                    return Json(new ProductPriceResult { Message = "سایز بسته وارد شده برای شما نا معتبر می با شد", ResultCode = -1 });
                }
                if (model.PackingDimension == null)
                    model.PackingDimension = new PackingDimension();
                model.PackingDimension.Width = Dimention.Width;
                model.PackingDimension.Height = (Dimention.Height == 0 ? 2 : Dimention.Height);
                model.PackingDimension.Length = Dimention.Length;
            }

            if (!model.IsValid(out error))
            {
                return Json(new ProductPriceResult { ResultCode = 4, Message = ApiMessage.GetErrorMsg(4) + " " + error });
            }
            //var product = _bulkOrderService.DetectProduct((int)model.ServiceId);
            //if (product == null)
            //    return Json(new ProductPriceResult { ResultCode = 4, Message = ApiMessage.GetErrorMsg(5) });

            var result = _bulkOrderService.GetCheckoutAttributePriceSeparately(model.Weight,
                 model.InsuranceName,
                 model.PackingDimension.Length,
                 model.PackingDimension.Width,
                 model.PackingDimension.Height,
                 model.IsCod,
                 model.PrintBill,
                 model.SendSms,
                 model.PrintLogo,
                 model.NeedCartoon,
                 model.GoodsValue,
                 model.SenderCityId,
                 model.ReceiverCityId,
                 model.ServiceId,
                 0,
                 model.Address);

            if (result == null)
            {
                return Json(new ProductPriceResult { ResultCode = 31, Message = ApiMessage.GetErrorMsg(31) });
            }

            return Json(new ProductPriceResult
            {
                BillPrintPrice = result.PrintPrice,
                CartoonPrice = result.CartonPrice,
                InsurancePrice = result.InsurancePrice,
                LogoPrice = result.LogoPrice,
                Message = "",
                ResultCode = 0,
                SmsPrice = result.SmsPrice,
                ServicePrices = result.ServicePrices.Select(p => new Models.Product.ServicePrice()
                {
                    ServiceId = p.ServiceId,
                    Price = p.Price,
                    ServiceName = p.ServiceName,
                    TotalPrice = p.TotalPrice,
                    TotalPriceIncludeTax = (p.TotalPrice * 9 / 100) + p.TotalPrice,
                    SLA = string.IsNullOrEmpty(p.SLA)?"" :p.SLA.Split('|')[0],
                    CollectionPrice = p.CollectionPrice
                }).ToList()
            });
        }
        [Route("api/getServices")]
        [HttpPost]
        public async Task<IActionResult> getService(_getServiceInfoModel model)
        {
            return await _getService(model);
        }
        [Route("api/JgetServices")]
        [HttpPost]
        public async Task<IActionResult> JgetService([FromBody] _getServiceInfoModel model)
        {
            return await _getService(model);
        }
        public async Task<IActionResult> _getService(_getServiceInfoModel model)
        {
            try
            {
                //if (_workContext.CurrentCustomer.Id == 2303982)
                //    if (model != null)
                //    {
                string d = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                Nop.plugin.Orders.ExtendedShipment.Services.common.Log("_getService", d);
                //    }
                if (_storeContext.CurrentStore.Id != 5)
                    return Json(new
                    {
                        servicesInfo = new List<ServiceInfoModel>(),
                        errorMessages = new List<string>() { "این سرویس با این آدرس در دسترس نمی باشد" },
                        warningMessages = new List<string>()
                    }); ;

                List<string> _errorMessage = new List<string>();
                List<string> _warningMessage = new List<string>();
                if (_workContext.CurrentCustomer.Id == 2303982)
                    model.ListType = 0;
                if (!model.Isvalid(out _errorMessage, out _warningMessage))
                {
                    return Json(new { servicesInfo = new List<ServiceInfoModel>(), errorMessages = _errorMessage, warningMessages = _warningMessage });
                }
                if (_extendedShipmentService.isInvalidSender(model.senderStateId, model.senderTownId))
                {
                    return Json(new { resultCode = 18, message = ApiMessage.GetErrorMsg(18) });
                }
                string error = "";
                model.customerId = _workContext.CurrentCustomer.Id;
                var result = await _bulkOrderService.getServiceInfo(model);
                return Json(new { servicesInfo = result, errorMessages = new List<string>() { error }, warningMessages = _warningMessage });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new
                {
                    servicesInfo = new List<ServiceInfoModel>(),
                    errorMessages = new List<string>() { "خطا در زمان دریافت لیست قیمت با پشتیبانی فنی تماس بگیرید" },
                    warningMessages = new List<string>()
                });
            }
        }

        [Route("api/getForginCountry")]
        [HttpGet]
        public IActionResult getCountryList()
        {
            return Json(_extendedShipmentService.getForinCountryForApi());
        }
        #endregion

        [Route("api/testapi")]
        [HttpPost]
        public IActionResult TestApi([FromBody] UserModel User)
        {
            return Ok(string.Concat("Hi ", User.Firstname, " " + User.Lastname));
        }
    }
}

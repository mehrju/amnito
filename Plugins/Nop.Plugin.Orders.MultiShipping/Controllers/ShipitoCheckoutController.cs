using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Http.Extensions;
using Nop.plugin.Orders.ExtendedShipment.Domain.PhoneOrder;
using Nop.plugin.Orders.ExtendedShipment.Enums;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.plugin.Orders.ExtendedShipment.Services.PreOrderService;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using Nop.Plugin.Orders.MultiShipping.Model;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
    public class ShipitoCheckoutController : BasePublicController
    {
        #region fields
        private readonly IApService _apService;
        private readonly INewCheckout _newCheckout;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPaymentService _paymentService;
        private readonly IWebHelper _webHelper;
        private readonly IRewardPointService _rewardPointService;
        private readonly IStoreContext _storeContext;
        private readonly IReOrderService _reOrderService;
        private readonly IRepository<Tbl_PhoneOrder> _repository_TblPhoneOrder;
        private readonly ISepService _sepService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly ICustomerService _customerService;
        private readonly IShipmentTrackingService _shipmentTrackingService;
        private readonly IDeclaration_Status_foreign_order_Service _declaration_Status_foreign_order_Service;
        private readonly IRepository<Tbl_PhoneOrder_Order> _repository_PhoneOrderOrder;
        private readonly IApiOrderRefrenceCodeService _apiOrderRefrenceCodeService;
        private readonly IPreOrderService _preOrderService;
        #endregion
        #region ctor
        public ShipitoCheckoutController(
            IApService apService,
            IStateProvinceService stateProvinceService,
            IHostingEnvironment hostingEnvironment
          , IDeclaration_Status_foreign_order_Service declaration_Status_foreign_order_Service
          , ICustomerActivityService customerActivityService
          , IOrderProcessingService orderProcessingService
          , ILocalizationService localizationService
          , IOrderService orderService
          , INewCheckout newCheckout
          , IWorkContext workContext
          , IPaymentService paymentService
          , IWebHelper webHelper
          , IRewardPointService rewardPointService
          , IStoreContext storeContext
            , IRepository<ProductAttributeValue> repositoryTbl_ProductAttributeValue,
            IReOrderService reOrderService,
            IRepository<Tbl_PhoneOrder> repository_TblPhoneOrder,
            ISepService sepService,
            ICustomerService customerService,
            IShipmentTrackingService shipmentTrackingService,
            IExtendedShipmentService extendedShipmentService,
            IRepository<Tbl_PhoneOrder_Order> repository_PhoneOrderOrder,
            IApiOrderRefrenceCodeService apiOrderRefrenceCodeService,
            IPreOrderService preOrderService
          )
        {
            _apService = apService;
            _stateProvinceService = stateProvinceService;
            _declaration_Status_foreign_order_Service = declaration_Status_foreign_order_Service;
            _storeContext = storeContext;
            _extendedShipmentService = extendedShipmentService;
            _webHelper = webHelper;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _workContext = workContext;
            _newCheckout = newCheckout;
            _paymentService = paymentService;
            _rewardPointService = rewardPointService;
            _reOrderService = reOrderService;
            _repository_TblPhoneOrder = repository_TblPhoneOrder;
            _sepService = sepService;
            _customerService = customerService;
            _shipmentTrackingService = shipmentTrackingService;
            _repository_PhoneOrderOrder = repository_PhoneOrderOrder;
            _hostingEnvironment = hostingEnvironment;
            _apiOrderRefrenceCodeService = apiOrderRefrenceCodeService;
            _preOrderService = preOrderService;
        }
        #endregion

        #region MyRegion

        [HttpGet]
        public IActionResult getUbbarTruckType()
        {
            return Json(_newCheckout.getUbbarTruckType());
        }
        [HttpGet]
        public IActionResult getUbbarVechileOPtion(string TruckType)
        {
            return Json(_newCheckout.getUbbarVechileOption(TruckType));
        }
        #endregion
        public bool IsValidCustomer()
        {
            var customer = _workContext.CurrentCustomer;
            if (customer == null || !customer.IsRegistered() || customer.IsGuest() || customer.Username == null)
                return false;
            return true;
        }
        [HttpPost, HttpGet]
        public IActionResult ItSazlogin(string usn)
        {
            try
            {

                //_newCheckout.Log("Input from Sep", encryptedMobileNo ?? "");
                if (string.IsNullOrEmpty(usn))
                {
                    _newCheckout.Log("اطلاعات پست شده از IT Saz نا معتبر می باشد", usn ?? "فیلد اطلاعاتی خالی می باشد");
                    return Content("در حال حاضر امکان اعتبار سنجی وجود ندارد، لطفا مجددا سعی کنید");

                }
                string Mobile = usn;//_securityService.DecryptSepInput(encryptedMobileNo);
                //if (Mobile != "09139064053")
                //    return Content("در حال حاضر امکان دسترسی به این سرویس برای شما مقدور نمی باشد");

                string msg = "";
                var customer = _sepService.AuthenticatItSazUser(out msg, Mobile);
                if (customer == null)
                {
                    _newCheckout.Log(msg, "");
                    return Content("در حال حاضر امکان اعتبار سنجی وجود ندارد، لطفا مجددا سعی کنید");
                }

                //HttpContext.Session.SetString("ApConfigValue", apStartupModel.config);
                ////درخواست اطلاعات کاربر
                vm_Index model = new vm_Index()
                {
                    HideForItSaz = true
                };

                if (!_sepService.IsValidCustomer(customer))
                {
                    _newCheckout.Log("خطا در زمان اعتبار سنجی کاربر sep", "");
                    return Content("در حال حاضر امکان اعتبار سنجی وجود ندارد، لطفا مجددا سعی کنید");
                }
                return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Index.cshtml", model);

            }
            catch (Exception ex)
            {
                LogException(ex);
                return Content("در حال حاضر امکان اعتبار سنجی وجود ندارد، لطفا مجددا سعی کنید");
            }

        }

        [HttpPost, HttpGet]
        public IActionResult Conamlogin(string usn)
        {
            try
            {

                //_newCheckout.Log("Input from Sep", encryptedMobileNo ?? "");
                if (string.IsNullOrEmpty(usn))
                {
                    _newCheckout.Log("اطلاعات پست شده از Conam نا معتبر می باشد", usn ?? "فیلد اطلاعاتی خالی می باشد");
                    return Content("در حال حاضر امکان اعتبار سنجی وجود ندارد، لطفا مجددا سعی کنید");

                }
                string Mobile = usn;//_securityService.DecryptSepInput(encryptedMobileNo);
                //if (Mobile != "09139064053")
                //    return Content("در حال حاضر امکان دسترسی به این سرویس برای شما مقدور نمی باشد");

                string msg = "";
                var customer = _sepService.AuthenticatItSazUser(out msg, Mobile);
                if (customer == null)
                {
                    _newCheckout.Log(msg, "");
                    return Content("در حال حاضر امکان اعتبار سنجی وجود ندارد، لطفا مجددا سعی کنید");
                }

                //HttpContext.Session.SetString("ApConfigValue", apStartupModel.config);
                ////درخواست اطلاعات کاربر
                vm_Index model = new vm_Index()
                {
                    HideForConam = true
                };

                if (!_sepService.IsValidCustomer(customer))
                {
                    _newCheckout.Log("خطا در زمان اعتبار سنجی کاربر conam", "");
                    return Content("در حال حاضر امکان اعتبار سنجی وجود ندارد، لطفا مجددا سعی کنید");
                }
                return RedirectToAction("Index", "ShipitoHome");
                //return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Index.cshtml", model);

            }
            catch (Exception ex)
            {
                LogException(ex);
                return Content("در حال حاضر امکان اعتبار سنجی وجود ندارد، لطفا مجددا سعی کنید");
            }

        }


        public IActionResult Index(string pa, bool isCod)
        {
            if (!IsValidCustomer())
                return RedirectToRoute("Login", new { returnUrl = Url.RouteUrl("_Sh_Checkout") });
            NewCheckoutViewModel model = new NewCheckoutViewModel()
            {
                IsAgent = _workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators"),
                IsInCodRole = _workContext.CurrentCustomer.IsInCustomerRole("COD"),
                postArea = string.IsNullOrEmpty(pa) ? PostArea.Internal : (pa.ToLower() == "f" ? PostArea.Foreign : (pa.ToLower() == "h" ? PostArea.Heavy : PostArea.Internal)),
                IsCod = isCod

            };
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/NewChckOutOrder.cshtml", model);
        }

        public IActionResult Bu_Index(string pa, bool isCod)
        {
            if (!IsValidCustomer())
                return RedirectToRoute("Login", new { returnUrl = Url.RouteUrl("_Sh_Checkout") });
            NewCheckoutViewModel model = new NewCheckoutViewModel()
            {
                IsAgent = _workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators"),
                IsInCodRole = _workContext.CurrentCustomer.IsInCustomerRole("COD"),
                postArea = string.IsNullOrEmpty(pa) ? PostArea.Internal : (pa.ToLower() == "f" ? PostArea.Foreign : (pa.ToLower() == "h" ? PostArea.Heavy : PostArea.Internal)),
                IsCod = isCod
            };
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/NewChckOutOrder.cshtml", model);
        }
        [HttpGet]
        public IActionResult getCountryList()
        {
            return Json(_newCheckout.getCountryList());
        }
        [HttpGet]
        public IActionResult getWeightItems()
        {
            return Json(_newCheckout.getWeightItem());
        }
        [HttpGet]
        public IActionResult getInsuranceItems()
        {
            return Json(_newCheckout.getInsuranceItems());
        }
        [HttpGet]
        public IActionResult getKartonItems()
        {
            return Json(_newCheckout.getKartonItems());
        }
        [HttpGet]
        public IActionResult getForeginCOuntry()
        {
            return Json(_newCheckout.getForginCountry());
        }
        [HttpPost]
        public async Task<ActionResult> getServicesInfo(string _model)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<getServiceInfoModel>(_model);
                return Json(await _newCheckout.GetServiceInfo(model.senderCountry, model.senderState, model.receiverCountry, model.receiverState,
                    model.weightItem, model.AproximateValue, _workContext.CurrentCustomer.Id, model.height, model.length, model.width
                    , dispach_date: (string.IsNullOrEmpty(model.dispatch_date) ? (DateTime?)null : Convert.ToDateTime(model.dispatch_date)),
                    PackingOption: model.UbbarPackingLoad, vechileType: model.UbbraTruckType, VechileOption: model.VechileOptions, content: model.Content
                    , receiver_ForeginCountry: model.receiver_ForeginCountry, receiver_ForeginCountryNameEn: model.receiver_ForeginCountryNameEn, consType: model.boxType, IsCod: model.IsCod, IsFromAp: model.IsFromAp
                    , SenderAddress: model.SenderAddress, ReciverAddress: model.ReciverAddress, IsFromUi: true, IsFromSep: model.IsFromSep));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        [HttpGet]
        public IActionResult GetStatesByCountryId(int countryId, bool isCod = false)
        {
            var stateList = _newCheckout.getStateByCountryId(countryId);
            //if (countryId == 1 && !isCod)
            //{
            //    List<string> ThPostarea = new List<string>() { "4", "579", "580", "581", "582", "583", "584", "585" };
            //    List<SelectListItem> NewStateList = new List<SelectListItem>();
            //    NewStateList.Insert(0, new SelectListItem() { Value = "582", Text = "شهر تهران" });
            //    NewStateList.AddRange(stateList.Where(p => !ThPostarea.Contains(p.Value)).ToList());
            //    return Json(NewStateList.ToList());
            //}
            return Json(stateList);
        }
        [HttpGet]
        public IActionResult GetUbbarStatesByCountryId(int countryId)
        {
            return Json(_newCheckout.getUbbarStateByCountryId(countryId));
        }
        [HttpGet]
        public IActionResult FetchAddress(int countryId, int stateId, string searchtext)
        {
            List<CustomAddressModel> _Addresses = _newCheckout.FetchAddress(_workContext.CurrentCustomer.Id, countryId, stateId, searchtext).ToList();

            //var ThStateId = new List<int> { 4, 579, 580, 581, 582, 583, 584, 585 };
            //if (_Addresses.Any(p => p.CountryId == 1 && ThStateId.Contains(p.StateProvinceId.Value)))
            //{
            //    foreach (var item in _Addresses)
            //    {
            //        if (item.CountryId == 1 && ThStateId.Contains(item.StateProvinceId.Value))
            //        {
            //            item.StateProvinceId = 582;
            //            item.StateProvinceName = "شهر تهران";
            //        }
            //    }
            //}
            var myAddress = _Addresses.Select(p => new
            {
                id = p.Id.ToString(),
                p.text,
                p.FirstName,
                p.LastName,
                p.PhoneNumber,
                p.Company,
                p.Address1,
                p.ZipPostalCode,
                p.Email,
                p.Lat,
                p.Lon,
                p.StateProvinceId,
                p.StateProvinceName,
                CountryName = p.Country,
                p.CountryId
            });
            return Json(new { results = myAddress });
        }

        [HttpPost]
        public IActionResult SaveNewCheckOutBulkOrder(string JsonCheckoutModel, int phoneOrderId, string SenderPhoneorderCustomer)
        {
            if (!IsValidCustomer())
            {
                return Json(new { message = "کاربر گرامی ابتدا وارد سیستم شوید ،سپس اقدام به ثبت سفارش کنید", success = false });
            }
            if (string.IsNullOrEmpty(JsonCheckoutModel))
            {
                _newCheckout.Log("متن ورودی خالی است", "");
                return Json(new { message = "اطلاعات وارد شده نامعتبر می باشد", success = false });
            }
            Customer phoneOrderCustomer = null;
            if (phoneOrderId > 0)
            {
                if (string.IsNullOrEmpty(SenderPhoneorderCustomer))
                {
                    return Json(new { message = "شماره موبایل فرستنده جهت مشخص کردن اکانت مشتری فرستنده نامعتبر میباشد", success = false });
                }
                phoneOrderCustomer = _customerService.GetCustomerByUsername(SenderPhoneorderCustomer);
                if (phoneOrderCustomer == null)
                {
                    string msg = "";
                    phoneOrderCustomer = _apService.Register(SenderPhoneorderCustomer, out msg, 0, true, true);
                    if (phoneOrderCustomer == null)
                    {
                        common.Log("خطا در زمان ثبت فرستنده به عنوان مشتری در سفارش انبوه", msg);
                        return Json(new { message = "امکان ثبت مشتری فرستنده به عنوان مشتری در سیستم وجود ندارد", success = false });
                    }
                }
            }
            var inputModel = new NewCheckout_Sp_Input()
            {
                JsonOrderList = JsonCheckoutModel.Replace("\\\"", "\""),
                JsonData = JsonConvert.SerializeObject(new
                {
                    CustommerId = (phoneOrderCustomer != null ? phoneOrderCustomer.Id : _workContext.CurrentCustomer.Id),
                    CustommerIp = _webHelper.GetCurrentIpAddress(),
                    StoreId = _storeContext.CurrentStore.Id,
                    BulkOrder = true,
                    FileName = "DataTable",
                    SourceId = 14,
                    phonOrderId = phoneOrderId
                })
            };
            try
            {
                var ret = _newCheckout.CheckoutBySp(inputModel, OrderRegistrationMethod.Bulk, 0, false, false);

                return Json(new { message = ret.ErrorMessage, success = ret.ErrorCode == 0, orderIds = ret.orderId });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { message = "خطا در زمان ثبت سفارش لطفا مجدد اقدام به ثبت سفارش کنید", success = false });
            }
        }

        [HttpPost]
        [MultiShipping.Infrastructure.RequestSizeLimit(valueCountLimit: 214748364)]
        public IActionResult SaveNewCheckOutOrder(string JsonCheckoutModel)
        {
            if (!IsValidCustomer())
            {
                return Json(new { message = "کاربر گرامی ابتدا وارد سیستم شوید ،سپس اقدام به ثبت سفارش کنید", success = false });
            }
            if (string.IsNullOrEmpty(JsonCheckoutModel))
            {
                _newCheckout.Log("متن ورودی خالی است", "");
                return Json(new { message = "اطلاعات وارد شده نامعتبر می باشد", success = false });
            }
            Dictionary<string, string> images64 = new Dictionary<string, string>();
            List<NewCheckoutModel> CheckoutModel = new List<NewCheckoutModel>();
            try
            {
                CheckoutModel = JsonConvert.DeserializeObject<List<NewCheckoutModel>>(JsonCheckoutModel);

                if (CheckoutModel == null || CheckoutModel.Count == 0)
                {
                    return Json(new { message = "اطلاعات وارد شده نامعتبر می باشد", success = false });
                }

                if (CheckoutModel[0].getCategoryInfo().IsForeign)
                {
                    for (int i = 0; i < CheckoutModel.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(CheckoutModel[i].imgFile1))
                        {
                            images64.Add("A" + i.ToString(), CheckoutModel[i].imgFile1);
                        }
                        if (!string.IsNullOrEmpty(CheckoutModel[i].imgFile2))
                        {
                            images64.Add("B" + i.ToString(), CheckoutModel[i].imgFile2);
                        }
                        if (!string.IsNullOrEmpty(CheckoutModel[i].imgFile3))
                        {
                            images64.Add("C" + i.ToString(), CheckoutModel[i].imgFile3);
                        }
                        JsonCheckoutModel = JsonConvert.SerializeObject(CheckoutModel);
                    }
                }
            }
            catch (Exception ex)
            {
                _newCheckout.Log(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
                return Json(new { message = "اطلاعات وارد شده نامعتبر می باشد", success = false });
            }


            List<string> str_msg = new List<string>();
            foreach (var item in CheckoutModel)
            {
                string Message = "";
                item.IsValid(out Message);
                if (!string.IsNullOrEmpty(Message))
                {
                    str_msg.Add($@"سفارش شماره {item.TempId} : \r\n" + Message);
                }
            }
            if (str_msg.Any())
                return Json(new { message = string.Join("\r\n", str_msg), success = false });
            if (CheckoutModel.Any(p => p.billingAddressModel.SumAddressValue() != CheckoutModel[0].billingAddressModel.SumAddressValue()))
            {
                return Json(new { message = "در حال حاضر امکان ثبت سفارش از مبدا های متفاوت دریک سفارش فعال نمی باشد", success = false });
            }
            int _orderSourceId = (CheckoutModel.First().IsFromSep ? 10 : (CheckoutModel.First().IsFromAp ? 6 : (CheckoutModel.First().IsFromFava ? 15 : 1)));
            var inputModel = new NewCheckout_Sp_Input()
            {
                JsonOrderList = JsonCheckoutModel,
                JsonData = JsonConvert.SerializeObject(new
                {
                    CustommerId = _workContext.CurrentCustomer.Id,
                    CustommerIp = _webHelper.GetCurrentIpAddress(),
                    StoreId = _storeContext.CurrentStore.Id,
                    SourceId = _orderSourceId
                })
            };
            try
            {
                var ret = _newCheckout.CheckoutBySp(inputModel, OrderRegistrationMethod.NewUi, 0, CheckoutModel.First().IsFromAp, CheckoutModel.First().IsFromSep);
                if (ret.orderId > 0)
                {
                    if (!string.IsNullOrWhiteSpace(CheckoutModel[0].UniqueReferenceNo))
                    {
                        _apiOrderRefrenceCodeService.SetOrderId(_workContext.CurrentCustomer.Id, CheckoutModel[0].UniqueReferenceNo, ret.orderId);
                    }
                    if (_orderSourceId == 15)
                    {
                        bool res = _preOrderService.SetOrderId(CheckoutModel[0].UniqueReferenceNo, ret.orderId);
                        if (!res)
                        {
                            // return StatusCode(500);
                        }
                    }
                    //_reOrderService.InsertOrderJson(ret.orderId, JsonCheckoutModel);
                    try
                    {
                        var catInfo = CheckoutModel[0].getCategoryInfo();
                        if (catInfo != null && catInfo.IsForeign)
                        {
                            string filePath = Path.Combine(_hostingEnvironment.ContentRootPath, @"Plugins\Orders.ExtendedShipment\ForeignOrderImages");

                            if (!System.IO.Directory.Exists(filePath))
                                System.IO.Directory.CreateDirectory(filePath);

                            int i = 1;
                            foreach (var item in images64)
                            {
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    System.IO.File.WriteAllBytes(filePath + "\\" + ret.orderId.ToString() + $"_{i++.ToString()}.jpg", Convert.FromBase64String(item.Value));

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogException(new Exception("Error Saving Images for Foreign Order->" + ex.Message));
                    }
                    return Json(new { success = true, message = "ثبت سفارش با موفقیت انجام شد", orderIds = ret.orderId, });
                }
                else
                {
                    return Json(new { success = false, message = ret.ErrorMessage });

                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { message = "بروز اشکال در زمان ثبت سفارش.لطفا مجدد سعی در ثبت سفارش کنید", success = false });
            }
        }
        public IActionResult ShowBillAndPayment([FromQuery] int[] orderIds, bool? safeOrder)
        {
            if (!IsValidCustomer())
                return RedirectToRoute("Login");
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/PaymentAndBill.cshtml", _newCheckout.GetFactorModel(orderIds.ToList(), safeOrder ?? false));
        }
        public IActionResult _ShowBillAndPayment([FromQuery] int orderId)
        {
            if (!IsValidCustomer())
                return RedirectToRoute("Login");
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/PaymentAndBill.cshtml", _newCheckout.GetFactorModel(new List<int>() { orderId }));
        }
        [HttpPost]
        public IActionResult CancelOrder(int id)
        {
            if ((_workContext.CurrentCustomer == null || _workContext.CurrentCustomer.IsGuest() || _workContext.CurrentCustomer.Username == null))
                return RedirectToRoute("Login", new { returnUrl = Url.RouteUrl("_Sh_BillAndPayment") + "?orderIds[0]=" + id });

            var order = _orderService.GetOrderById(id);
            if (order == null)
                return Json(new { success = false, message = "اطلاعات مربوط به سفارش شما یافت نشد" });
            if (order.OrderStatus == OrderStatus.Cancelled)
                return Json(new { success = false, message = "انصراف از سفارش با موفقیت انجام شده. مجددا سعی نکنید" });
            try
            {
                _orderProcessingService.CancelOrder(order, true);
                LogEditOrder(order.Id);
                if (order.OrderStatus == OrderStatus.Cancelled)
                {
                    return Json(new { success = true, message = "انصراف از سفارش با موفقیت انجام شد" });
                }
                return Json(new { success = false, message = "در حال حاضر امکان انصراف از سفارش وجود ندارد. با پشتیبانی سامانه تماس بگیرید" });
            }
            catch (Exception exc)
            {
                //error
                _newCheckout.InsertOrderNote("خطا در زمان انصراف از سفارش" + "\r\n"
                    + exc.Message + (exc.InnerException != null ? "-->" + exc.InnerException.Message : ""), id);
                return Json(new { success = false, message = "عملیات انصراف از شسفارش شما با مشکل مواجه شد. با پشتیبانی سامانه تماس بگیرید" });
            }
        }
        protected void LogEditOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);

            _customerActivityService.InsertActivity("EditOrder", _localizationService.GetResource("ActivityLog.EditOrder"), order.CustomOrderNumber);
        }
        [HttpPost]
        public IActionResult IsvalidService(int serviceId)
        {
            return Json(new { success = _newCheckout.IsValidServiceForCustomer(serviceId) });
        }
        [HttpPost]
        public IActionResult ConfirmAndPay(int orderId, string paymentmethod, bool UseRewardPoints, bool isFromApp, bool isForeign = false, bool reciverPay = false)
        {
            HttpContext.Session.Set("isFromApp", isFromApp);
            TempData["PaymentMsg"] = null;
            var order = _orderService.GetOrderById(orderId);
            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                TempData["PaymentMsg"] = "سفارش شما کنسل شده و امکان پرداخت برای آن وجود ندارد";
                return Redirect(Url.RouteUrl("_Sh_BillAndPayment") + "?orderIds[0]=" + orderId + "&msg=1");
            }

            if (order.PaymentStatus == Core.Domain.Payments.PaymentStatus.Paid)
                return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });
            var rewardPointsBalance =
                           _rewardPointService.GetRewardPointsBalance(order.CustomerId, order.StoreId);

            var isCod = order.IsOrderCod();
            var isSafeOrder = _extendedShipmentService.IsSafeBuy(order.Id) && !reciverPay;

            if (!isCod && !isForeign && !isSafeOrder)
            {
                var msg = "";
                if (!_newCheckout.CanPayForOrder(order.Id, out msg))
                {
                    return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });
                }
                AddtionalDataForginRequest(order);
                if (UseRewardPoints)
                {
                    if (order.OrderTotal > rewardPointsBalance)
                    {
                        TempData["PaymentMsg"] = "موجودی کیف پول برای این سفارش می بایست حداقل " + Convert.ToInt32(order.OrderTotal).ToString("N0")
                                                                                         + " ريال باشد. موجودی فعلی " + rewardPointsBalance.ToString("N0");
                        return Redirect(Url.RouteUrl("_Sh_BillAndPayment") + "?orderIds[0]=" + orderId + "&msg=2");
                    }
                    else
                    {
                        //TODO : Reward point
                        _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, -Convert.ToInt32(order.OrderTotal), order.StoreId,
                        string.Format(_localizationService.GetResource("RewardPoints.Message.RedeemedForOrder", order.CustomerLanguageId), order.CustomOrderNumber),
                        order, order.OrderTotal);
                        order.OrderTotal = 0;
                        order.PaymentMethodSystemName = null;
                        _orderProcessingService.MarkOrderAsPaid(order);
                        return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });

                    }
                }
                if (string.IsNullOrEmpty(paymentmethod))
                {
                    TempData["PaymentMsg"] = "روش پرداخت انتخابی نامعتبر می باشد";
                    return Redirect(Url.RouteUrl("_Sh_BillAndPayment") + "?orderIds[0]=" + orderId + "&msg=3");

                }
                order.PaymentMethodSystemName = paymentmethod;
                _orderService.UpdateOrder(order);
                var postProcessPaymentRequest = new PostProcessPaymentRequest
                {
                    Order = order
                };
                _paymentService.PostProcessPayment(postProcessPaymentRequest);
                common.Log("قبل از تشخیص نوع درگاه", "");
                if (isFromApp)
                {
                    common.Log("ارسال به صفحه میانی اپ", HttpContext.Session.Get<string>("redirectUrl"));
                    return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/OpenExternalBrowser.cshtml", HttpContext.Session.Get<string>("redirectUrl"));
                }
                if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                {
                    return Content("Redirected");
                }
            }
            else if (isCod && !isForeign)
            {
                order.OrderStatusId = (int)OrderStatus.Processing;
                _orderService.UpdateOrder(order);
            }
            return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });
        }
        public void AddtionalDataForginRequest(Order order)
        {
            //if (_newCheckout.canAddValueToForginRequest(order.Id))
            //{
            //    int orderValue = _newCheckout.getForginAddtionalValue(order.Id);
            //    order.OrderTotal += orderValue;
            //    _orderService.UpdateOrder(order);
            //}
        }
        public IActionResult ChargeWalletIndex()
        {
            if (!IsValidCustomer())
            {
                return RedirectToRoute("Login", new { returnUrl = Url.RouteUrl("Nop.Plugin.Misc.PostbarDashboard.Dashboard") + "#list-up" });
            }
            var model = _newCheckout.getPaymentMethodForChargeWallet();
            return Redirect(Url.RouteUrl("Nop.Plugin.Misc.PostbarDashboard.Dashboard") + "#list-up");
        }
        [HttpPost]
        public IActionResult ChargeWallet(string paymentmethod, int amount, bool isFromApp)
        {
            HttpContext.Session.Set("isFromApp", isFromApp);
            TempData["PaymentMsg"] = null;
            if (!IsValidCustomer())
            {
                TempData["PaymentMsg"] = "ابتدا وارد سیستم شوید سپس اقدام به پرداخت کنید";
                return Redirect(Url.RouteUrl("Nop.Plugin.Misc.PostbarDashboard.Dashboard") + "#list-up");// TODO: باید تغییر کنید
            }
            if (string.IsNullOrEmpty(paymentmethod))
            {
                TempData["PaymentMsg"] = "روش پرداخت انتخابی نامعتبر می باشد";
                return Redirect(Url.RouteUrl("Nop.Plugin.Misc.PostbarDashboard.Dashboard") + "#list-up");// TODO: باید تغییر کنید

            }
            if (amount <= 0)
            {
                TempData["PaymentMsg"] = "مبلغ وارد شده نامعتبر می باشد";
                return Redirect(Url.RouteUrl("Nop.Plugin.Misc.PostbarDashboard.Dashboard") + "#list-up");// TODO: باید تغییر کنید
            }
            var checkoutReult = _newCheckout.ProccessWalletOrder(amount, paymentmethod, _workContext.CurrentCustomer.Id);
            if (!checkoutReult.Success)
            {
                TempData["PaymentMsg"] = string.Join("\r\n", checkoutReult.Errors);
                return Redirect(Url.RouteUrl("Nop.Plugin.Misc.PostbarDashboard.Dashboard") + "#list-up");// TODO: باید تغییر کنید
            }
            var postProcessPaymentRequest = new PostProcessPaymentRequest
            {
                Order = checkoutReult.PlacedOrder
            };
            _paymentService.PostProcessPayment(postProcessPaymentRequest);
            if (isFromApp)
            {
                common.Log("ارسال به صفحه میانی اپ", HttpContext.Session.Get<string>("redirectUrl"));
                return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/OpenExternalBrowser.cshtml", HttpContext.Session.Get<string>("redirectUrl"));
            }
            if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
            {
                return Content("Redirected");
            }


            return RedirectToRoute("CheckoutCompleted", new { orderId = checkoutReult.PlacedOrder.Id });
        }

        public IActionResult PayForOrderIndex(int orderId, bool isFromApp)
        {
            if (orderId <= 0)
            {
                return Json(new { message = "اطلاعات ارسالی ناقص می باشد" });
            }
            var order = _orderService.GetOrderById(orderId);
            if (order.IsOrderForeign())
            {
                if (!_declaration_Status_foreign_order_Service.IsOrderConfirm(orderId))
                {
                    return Json(new { message = "کاربر گرامی سفارش شما هنوز تایید نشده، پس از تایید از طریق پیامک به شما اطلاع رسانی میشود" });
                }
            }
            var paymentMethod = _newCheckout.getPaymentMethod(order);
            if (isFromApp)
            {
                var _paymentMethod = paymentMethod.PaymentMethods.Where(p => p.PaymentMethodSystemName != "Payments.Mellat").ToList();
                paymentMethod.PaymentMethods = _paymentMethod;
            }
            var model = new OrderBillAndPaymentModel()
            {
                order = order,
                PaymentMethods = paymentMethod
            };
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/PayForOrder.cshtml", model);
        }
        public IActionResult PayForSafeBuyOrder(int orderId, bool isFromApp)
        {
            if (orderId <= 0)
            {
                return Json(new { message = "اطلاعات ارسالی ناقص می باشد" });
            }
            var order = _orderService.GetOrderById(orderId);
            if (order == null)
            {
                return Json(new { message = "سفارش یافت نشد" });
            }
            if (order.PaymentStatus == Core.Domain.Payments.PaymentStatus.Paid)
            {
                return Json(new { message = "این سفارش قبلا پرداخت شده است" });
            }
            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                return Json(new { message = "این سفارش لغو شده است" });
            }
            string AuthenticateMsg;
            var adr = _extendedShipmentService.getAddressFromShipment(order.Shipments.First().Id);
            _sepService.AuthenticatSepUser(out AuthenticateMsg, adr.PhoneNumber);
            ViewData["AuthenticateMsg"] = AuthenticateMsg;
            if (order.IsOrderForeign())
            {
                if (!_declaration_Status_foreign_order_Service.IsOrderConfirm(orderId))
                {
                    return Json(new { message = "کاربر گرامی سفارش شما هنوز تایید نشده، پس از تایید از طریق پیامک به شما اطلاع رسانی میشود" });
                }
            }
            var paymentMethod = _newCheckout.getPaymentMethodForSafeBuy(order, _workContext.CurrentCustomer);
            if (isFromApp)
            {
                var _paymentMethod = paymentMethod.PaymentMethods.Where(p => p.PaymentMethodSystemName != "Payments.Mellat").ToList();
                paymentMethod.PaymentMethods = _paymentMethod;
            }
            var model = new OrderBillAndPaymentModel()
            {
                order = order,
                PaymentMethods = paymentMethod
            };
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/AnonymousPayForOrder.cshtml", model);
        }
        public IActionResult PayForSaleCarton(int OrderCode, SendItemSaleKarton List_PP)
        {
            return View("~/Plugins/Misc.PrintedReports_Requirements/Views/SaleCartonwrapper/PayForOrder.cshtml");
        }

        [HttpGet]
        public IActionResult IsInInvalidService(int countryId, int stateId)
        {
            var _IsInvalid = _newCheckout.isInvalidSernder(countryId, stateId);
            return Json(new { isInvalid = _IsInvalid });
        }

        [HttpGet("[controller]/[action]/{phoneOrderId}")]
        public IActionResult SavePhoneOrder([FromRoute] int phoneOrderId)
        {
            var phoneOrder = _repository_TblPhoneOrder.GetById(phoneOrderId);
            if (phoneOrder == null)
            {
                return Json(new { message = "پیش سفارش تلفنی یافت نشد" });
                //return BadRequest();
            }
            var state = _stateProvinceService.GetStateProvinceById(phoneOrder.CityId);

            Address TelOrderSenderAddress = new Address()
            {
                FirstName = phoneOrder.FirstName,
                LastName = phoneOrder.LastName,
                PhoneNumber = phoneOrder.PhoneNumber,
                CountryId = state.CountryId,
                StateProvinceId = phoneOrder.CityId,
                ZipPostalCode = phoneOrder.PostalCode,
                FaxNumber = phoneOrder.TellNumber,
                Address1 = phoneOrder.Address
            };
            Tbl_Address_LatLong Address_LatLong = null;
            if (phoneOrder.HasLocation)
            {
                Address_LatLong = new Tbl_Address_LatLong()
                {
                    Lat = Convert.ToDouble(phoneOrder.Latitude),
                    Long = Convert.ToDouble(phoneOrder.Longitude)
                };
            }
            phoneOrderRegisterOrderModel _model = new phoneOrderRegisterOrderModel()
            {
                GeoPoint = Address_LatLong,
                SenderAddress = TelOrderSenderAddress,
                PhoneOrderId = phoneOrderId,
                ServiceId = phoneOrder.ServiceId,
                description = "",
                CountryName = state.Country.Name,
                StateName = state.Name,
            };
            TempData["PhoneOrdermodel"] = Newtonsoft.Json.JsonConvert.SerializeObject(_model);
            return RedirectToAction("Index_forPhoneOrder", "BulkOrder");
        }

        private NewCheckout_Sp_Output RegisterPhoneOrderOrder(Tbl_PhoneOrder phoneOrder)
        {
            return null;
            //var customer = _customerService.GetCustomerByUsername(phoneOrder.PhoneNumber);
            //if (customer == null)
            //{
            //    customer = _sepService.RegisterForNotCurrentCustomer(phoneOrder.PhoneNumber, out string msg, sendSms: true);
            //    //customer = new Customer() { Username = phoneOrder.PhoneNumber };
            //    //_customerService.InsertCustomer(customer);
            //    if (customer == null)
            //    {
            //        return null;
            //        //return Json(new { message = "customer == null" });
            //    }
            //}
            //var checkOut = new List<NewCheckoutModel>();
            //checkOut.Add(new NewCheckoutModel()
            //{
            //    AgentSaleAmount = 0,
            //    boxType = "پاکت",
            //    CartonSizeName = "کارتن نیاز ندارم.",
            //    CodGoodsPrice = null,
            //    Count = 1,
            //    discountCouponCode = null,
            //    dispatch_date = null,
            //    //getItNow 
            //    GoodsType = "500",
            //    HasAccessToPrinter = false,
            //    hasNotifRequest = false,
            //    height = 2,
            //    width = 11,
            //    length = 22,
            //    InsuranceName = null,
            //    IsCOD = false,
            //    receiver_ForeginCityName = null,
            //    receiver_ForeginCountry = 0,
            //    receiver_ForeginCountryName = null,
            //    receiver_ForeginCountryNameEn = null,
            //    ApproximateValue = 0,
            //    RequestPrintAvatar = false,
            //    ReciverLat = 0,
            //    ReciverLon = 0,
            //    SenderLat = (float)phoneOrder.Latitude,
            //    SenderLon = (float)phoneOrder.Longitude,
            //    UbbarPackingLoad = null,
            //    ServiceId = 724,
            //    _dispatch_date = null,
            //    Weight = 100,
            //    VechileOptions = null,
            //    UbbraTruckType = null,
            //    billingAddressModel = new Core.Domain.Common.Address()
            //    {
            //        Address1 = phoneOrder.Address,
            //        StateProvinceId = phoneOrder.CityId,
            //        CountryId = phoneOrder.id,
            //        PhoneNumber = phoneOrder.TellNumber,
            //        FirstName = phoneOrder.FirstName,
            //        LastName = phoneOrder.LastName
            //    }
            //});


            //var inputModel = new NewCheckout_Sp_Input()
            //{
            //    JsonOrderList = JsonConvert.SerializeObject(checkOut),
            //    JsonData = JsonConvert.SerializeObject(new
            //    {
            //        CustommerId = _workContext.CurrentCustomer.Id,
            //        CustommerIp = _webHelper.GetCurrentIpAddress(),
            //        StoreId = _storeContext.CurrentStore.Id,
            //        IsTell = true,
            //        SourceId = 3
            //    })
            //};

            //var ret = _newCheckout.CheckoutBySp(inputModel, OrderRegistrationMethod.PhoneOrder, 0, false, false);
            //return ret;
        }
    }

}

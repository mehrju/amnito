using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

using Microsoft.CSharp.RuntimeBinder;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.Mellat.ir.shaparak.bpm;
using Nop.Plugin.Payments.Mellat.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Services.Security;
using Nop.Web.Controllers;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Logging;

namespace Nop.Plugin.Payments.Mellat.Controllers
{

    public class NopFarsiPaymentMellatController : BasePublicController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly MellatPaymentSettings _MellatPaymentSettings;
        private readonly IPermissionService _permissionService;
        //public void UpdateOrderId(int? orderId, int? installOrderId)
        //{
        //    MellatPaymentSettings mellatPaymentSettings = new MellatPaymentSettings
        //    {
        //        OrderId = (orderId ?? this._MellatPaymentSettings.OrderId),
        //        TerminalId = this._MellatPaymentSettings.TerminalId,
        //        UserName = this._MellatPaymentSettings.UserName,
        //        UserPassword = this._MellatPaymentSettings.UserPassword,
        //        Toman = this._MellatPaymentSettings.Toman,
        //        AdditionalFee = this._MellatPaymentSettings.AdditionalFee,
        //        AdditionalFeePercentage = this._MellatPaymentSettings.AdditionalFeePercentage,
        //        ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage = this._MellatPaymentSettings.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage
        //    };
        //    this._settingService.SaveSetting<MellatPaymentSettings>(mellatPaymentSettings, 0);
        //}

        public NopFarsiPaymentMellatController(IWorkContext workContext, IStoreService storeService, ISettingService settingService, IOrderService orderService, IOrderProcessingService orderProcessingService, ILogger logger, IWebHelper webHelper, MellatPaymentSettings MellatPaymentSettings, IPermissionService permissionService)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._logger = logger;
            this._webHelper = webHelper;
            this._MellatPaymentSettings = MellatPaymentSettings;
            this._permissionService = permissionService;
        }

        public IActionResult Pay(string result)
        {
            base.ViewBag.result = result;
            return base.View("~/Plugins/NopFarsi.Plugin.Payments.Mellat/Views/PaymentMellat/Pay.cshtml");
        }
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            //whether user has the authority
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            int activeStoreScopeConfiguration = this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
            MellatPaymentSettings mellatPaymentSettings = this._settingService.LoadSetting<MellatPaymentSettings>(activeStoreScopeConfiguration);
            ConfigurationModel configurationModel = new ConfigurationModel
            {
                TerminalId = mellatPaymentSettings.TerminalId,
                UserName = mellatPaymentSettings.UserName,
                UserPassword = mellatPaymentSettings.UserPassword,
                //OrderId = mellatPaymentSettings.OrderId,
                Toman = mellatPaymentSettings.Toman,
                AdditionalFee = mellatPaymentSettings.AdditionalFee,
                AdditionalFeePercentage = mellatPaymentSettings.AdditionalFeePercentage,
                ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage = mellatPaymentSettings.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage,
                ActiveStoreScopeConfiguration = activeStoreScopeConfiguration
            };
            if (activeStoreScopeConfiguration > 0)
            {
                configurationModel.TerminalId_OverrideForStore = this._settingService.SettingExists<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.TerminalId, activeStoreScopeConfiguration);
                configurationModel.UserName_OverrideForStore = this._settingService.SettingExists<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.UserName, activeStoreScopeConfiguration);
                configurationModel.UserPassword_OverrideForStore = this._settingService.SettingExists<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.UserPassword, activeStoreScopeConfiguration);
                //configurationModel.OrderId_OverrideForStore = this._settingService.SettingExists<MellatPaymentSettings, int>(mellatPaymentSettings, (MellatPaymentSettings x) => x.OrderId, activeStoreScopeConfiguration);
                configurationModel.Toman_OverrideForStore = this._settingService.SettingExists<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.Toman, activeStoreScopeConfiguration);
                configurationModel.AdditionalFee_OverrideForStore = this._settingService.SettingExists<MellatPaymentSettings, decimal>(mellatPaymentSettings, (MellatPaymentSettings x) => x.AdditionalFee, activeStoreScopeConfiguration);
                configurationModel.AdditionalFeePercentage_OverrideForStore = this._settingService.SettingExists<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.AdditionalFeePercentage, activeStoreScopeConfiguration);
                configurationModel.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore = this._settingService.SettingExists<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage, activeStoreScopeConfiguration);
            }

            return View("~/Plugins/NopFarsi.Plugin.Payments.Mellat/Views/PaymentMellat/Configure.cshtml", configurationModel);
        }
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost]
        [AdminAntiForgery]
        public IActionResult Configure(ConfigurationModel model)
        {
            //whether user has the authority
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            ActionResult result;
            if (!base.ModelState.IsValid)
            {
                return Configure();
            }
            else
            {
                int activeStoreScopeConfiguration = this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
                MellatPaymentSettings mellatPaymentSettings = this._settingService.LoadSetting<MellatPaymentSettings>(activeStoreScopeConfiguration);
                mellatPaymentSettings.TerminalId = model.TerminalId;
                mellatPaymentSettings.UserName = model.UserName;
                mellatPaymentSettings.UserPassword = model.UserPassword;
                //mellatPaymentSettings.OrderId = model.OrderId;
                mellatPaymentSettings.Toman = model.Toman;
                mellatPaymentSettings.AdditionalFee = model.AdditionalFee;
                mellatPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
                mellatPaymentSettings.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage = model.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage;
                if (model.TerminalId_OverrideForStore || activeStoreScopeConfiguration == 0)
                {
                    this._settingService.SaveSetting<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.TerminalId, activeStoreScopeConfiguration, false);
                }
                else if (activeStoreScopeConfiguration > 0)
                {
                    this._settingService.DeleteSetting<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.TerminalId, activeStoreScopeConfiguration);
                }
                if (model.UserName_OverrideForStore || activeStoreScopeConfiguration == 0)
                {
                    this._settingService.SaveSetting<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.UserName, activeStoreScopeConfiguration, false);
                }
                else if (activeStoreScopeConfiguration > 0)
                {
                    this._settingService.DeleteSetting<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.UserName, activeStoreScopeConfiguration);
                }
                if (model.UserPassword_OverrideForStore || activeStoreScopeConfiguration == 0)
                {
                    this._settingService.SaveSetting<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.UserPassword, activeStoreScopeConfiguration, false);
                }
                else if (activeStoreScopeConfiguration > 0)
                {
                    this._settingService.DeleteSetting<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.UserPassword, activeStoreScopeConfiguration);
                }
                if (model.OrderId_OverrideForStore || activeStoreScopeConfiguration == 0)
                {
                    //this._settingService.SaveSetting<MellatPaymentSettings, int>(mellatPaymentSettings, (MellatPaymentSettings x) => x.OrderId, activeStoreScopeConfiguration, false);
                }
                else if (activeStoreScopeConfiguration > 0)
                {
                    //this._settingService.DeleteSetting<MellatPaymentSettings, int>(mellatPaymentSettings, (MellatPaymentSettings x) => x.OrderId, activeStoreScopeConfiguration);
                }

                if (model.Toman_OverrideForStore || activeStoreScopeConfiguration == 0)
                {
                    this._settingService.SaveSetting<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.Toman, activeStoreScopeConfiguration, false);
                }
                else if (activeStoreScopeConfiguration > 0)
                {
                    this._settingService.DeleteSetting<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.Toman, activeStoreScopeConfiguration);
                }

                if (model.AdditionalFee_OverrideForStore || activeStoreScopeConfiguration == 0)
                {
                    this._settingService.SaveSetting<MellatPaymentSettings, decimal>(mellatPaymentSettings, (MellatPaymentSettings x) => x.AdditionalFee, activeStoreScopeConfiguration, false);
                }
                else if (activeStoreScopeConfiguration > 0)
                {
                    this._settingService.DeleteSetting<MellatPaymentSettings, decimal>(mellatPaymentSettings, (MellatPaymentSettings x) => x.AdditionalFee, activeStoreScopeConfiguration);
                }
                if (model.AdditionalFeePercentage_OverrideForStore || activeStoreScopeConfiguration == 0)
                {
                    this._settingService.SaveSetting<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.AdditionalFeePercentage, activeStoreScopeConfiguration, false);
                }
                else if (activeStoreScopeConfiguration > 0)
                {
                    this._settingService.DeleteSetting<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.AdditionalFeePercentage, activeStoreScopeConfiguration);
                }
                if (model.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore || activeStoreScopeConfiguration == 0)
                {
                    this._settingService.SaveSetting<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage, activeStoreScopeConfiguration, false);
                }
                else if (activeStoreScopeConfiguration > 0)
                {
                    this._settingService.DeleteSetting<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage, activeStoreScopeConfiguration);
                }
                this._settingService.ClearCache();

            }
            base.SuccessNotification("با موفقیت ذخیره شد. تولید شده توسط ناپ فارسی . www.nopfarsi.ir");
            return View("~/Plugins/NopFarsi.Plugin.Payments.Mellat/Views/PaymentMellat/Configure.cshtml", model);
            //return Configure();
        }



        public IActionResult CallBack(string ResCode, int OrderId, string RefId, string saleOrderId, string SaleReferenceId)
        {

            string _result = Newtonsoft.Json.JsonConvert.SerializeObject(new { ResCode, OrderId, RefId, saleOrderId, SaleReferenceId });
            _logger.InsertLog(LogLevel.Information, "کال بک بانک ملت", _result , _workContext.CurrentCustomer);
            string text = string.Format("دسترسی مستقیم به صفحه بازگشت از درگاه ‍‍‍‍‍پرداخت اینترنتی بانک ملت", new object[0]);
            string text2 = ResCode;// base.Request.Params["ResCode"];
            ActionResult result;
            if (text2 == null)
            {
                result = base.RedirectToAction("Index", "Home", new
                {
                    area = ""
                });
            }
            else
            {
                int num = OrderId;//int.Parse(base.Request.QueryString["OrderId"]);
                Order orderById = this._orderService.GetOrderById(num);

                if (!string.IsNullOrEmpty(RefId) && !string.IsNullOrEmpty(saleOrderId) && !string.IsNullOrEmpty(SaleReferenceId))
                {
                    string authorizationTransactionId = RefId;// base.Request.Params["RefId"];
                    long num2 = long.Parse(saleOrderId);//base.Request.Params["saleOrderId"]);
                    long saleReferenceId = long.Parse(SaleReferenceId);//base.Request.Params["SaleReferenceId"]);

                    if (text2 == "0")
                    {
                        if (orderById != null)
                        {
                            try
                            {
                                string terminalId = this._MellatPaymentSettings.TerminalId;
                                string userName = this._MellatPaymentSettings.UserName;
                                string userPassword = this._MellatPaymentSettings.UserPassword;

                                PaymentGatewayImplService paymentGatewayImplService = new PaymentGatewayImplService();
                                paymentGatewayImplService.Credentials = System.Net.CredentialCache.DefaultCredentials; //Test Vakili
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                string text3 = paymentGatewayImplService.bpVerifyRequest(long.Parse(terminalId), userName, userPassword, num2, num2, saleReferenceId);
                                string[] array = text3.Split(new char[]
                                {
                                    ','
                                });
                                orderById.AuthorizationTransactionId = (SaleReferenceId + "-" + saleOrderId);
                                if (!(array[0] == "0"))
                                {
                                    throw new Exception();
                                }
                                string a = paymentGatewayImplService.bpSettleRequest(long.Parse(terminalId), userName, userPassword, num2, num2, saleReferenceId);
                                if (a != "0")
                                {
                                    throw new Exception("Result bpSettleRequest:" + a);
                                }

                                if (this._orderProcessingService.CanMarkOrderAsPaid(orderById))
                                {
                                    orderById.AuthorizationTransactionResult = (string.Format("[ResCode = {0}]", text2));
                                    orderById.AuthorizationTransactionId = (SaleReferenceId + "-" + saleOrderId);
                                    this._orderService.UpdateOrder(orderById);
                                    this._orderProcessingService.MarkOrderAsPaid(orderById);
                                }
                                if (_workContext.CurrentCustomer.Id != orderById.CustomerId)
                                    _workContext.CurrentCustomer = orderById.Customer;
                            }
                            catch (Exception ex)
                            {
                                text = string.Format("Error in Mellat Pay Verify or Pay Settle. OrderId= {0}", num);
                                LoggingExtensions.Error(this._logger, text + " " + ex.Message, ex, null);
                                result = base.RedirectToAction("Error", "PaymentMellat", new
                                {
                                    ErrorCode = text2,
                                    id = num
                                });
                                return result;
                            }


                            return RedirectToRoute("CheckoutCompleted", new { orderId = orderById.Id });
                            result = base.RedirectToRoute("CheckoutCompleted", new
                            {
                                orderId = orderById.Id
                            });
                        }
                        else
                        {
                            text = string.Format("Order Not Found. OrderId= {0}", num);
                            LoggingExtensions.Error(this._logger, text, null, null);
                            result = base.RedirectToAction("Error", "PaymentMellat", new
                            {
                                ErrorCode = text2,
                                id = num
                            });
                        }
                    }
                    else
                    {
                        text = Utility.ErrorCode("bpPayRequest", text2);
                        LoggingExtensions.Warning(this._logger, text, null, null);
                        result = base.RedirectToAction("Error", "PaymentMellat", new
                        {
                            ErrorCode = text2,
                            id = num
                        });
                    }
                }
                else
                {
                    text = Utility.ErrorCode("bpPayRequest", text2);
                    LoggingExtensions.Error(this._logger, text, null, null);
                    result = base.RedirectToAction("Error", "PaymentMellat", new
                    {
                        ErrorCode = text2,
                        id = num
                    });
                }
            }
            return result;
        }



        public ActionResult PaymentResult(string result)
        {
            base.ViewBag.Message = result;
            return base.View("~/Plugins/NopFarsi.Plugin.Payments.Mellat/Views/PaymentMellat/PaymentResult.cshtml");
        }

        public ActionResult Error(string ErrorCode, string resCode, int? id)
        {
            if (string.IsNullOrEmpty(ErrorCode))
                ErrorCode = resCode;
            ViewBag.orderId = id;
            ActionResult result;
            if (ErrorCode == "17")
            {
                result = this.Redirect("/Dashboard/Orders");
            }
            else
            {
                base.ViewBag.ErrorCode = ErrorCode;
                if (ErrorCode == "-2")
                {
                    base.ViewBag.result = "شما مجوز استفاده از ماژول درگاه را ندارید";
                }
                else if (ErrorCode == "-1")
                {
                    base.ViewBag.result = "درگاه پیکربندی نشده است. مقادیر شماره ترمینال ، نام کاربری و رمز عبور را وارد نمایید";

                }
                else if (ErrorCode == "11")
                {
                    base.ViewBag.result = "شماره کارت نا معتبر است";
                }
                else if (ErrorCode == "12")
                {
                    base.ViewBag.result = "موجودی کافی نیست";
                }
                else if (ErrorCode == "13")
                {
                    base.ViewBag.result = "رمز نادرست است";
                }
                else if (ErrorCode == "15")
                {
                    base.ViewBag.result = "کارت نا معتبر است";
                }
                else if (ErrorCode == "18")
                {
                    base.ViewBag.result = "تاریخ انقضای کارت گذشته است";
                }
                else if (ErrorCode == "21")
                {
                    base.ViewBag.result = "پذیرنده نا معتبر است";
                }
                else if (ErrorCode == "41")
                {
                    base.ViewBag.result = "شماره درخواست تکرای است";
                }
                else
                {
                    base.ViewBag.result = Utility.ErrorCode("bpPayRequest", ErrorCode);
                }
                result = base.View("~/Plugins/NopFarsi.Plugin.Payments.Mellat/Views/PaymentMellat/Error.cshtml");
            }
            return result;
        }



    }
}

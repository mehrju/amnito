using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using NopFarsi.Payments.SepShaparak.Models;
using System.Collections.Specialized;
using System.Text;
using Nop.Services.Logging;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Nop.Services.Orders;
using Nop.Core.Domain.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Logging;
using RestSharp;
using Newtonsoft.Json;

namespace NopFarsi.Payments.SepShaparak.Controller
{
    public class SamanPostbarController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPaymentService _paymentService;
        private readonly PaymentSettings _paymentSettings;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly sepsettings _SepPaymentSettings;
        public SamanPostbarController(IWorkContext workContext, IStoreService storeService, ISettingService settingService, ILocalizationService localizationService
            , IPaymentService paymentService, PaymentSettings paymentSettings, ILogger logger, IOrderService orderService, IOrderProcessingService orderProcessingService,
            sepsettings SepPaymentSettings)
        {
            _SepPaymentSettings= SepPaymentSettings;
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._paymentService = paymentService;
            this._paymentSettings = paymentSettings;
            this._logger = logger;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
        }

        public ActionResult Pay(string ResNum, string MID, string Amount, string Token, string RedirectURL)
        {
            PayModel payModel = new PayModel()
            {
                Amount = Amount,
                MerchantId = MID,
                RedirectUrl = RedirectURL,
                ResNum = ResNum,
                Token = Token
            };
            NameValueCollection datacollection = new NameValueCollection();
            //datacollection.Add("Amount", Amount);
            //datacollection.Add("ResNum", ResNum);
            //datacollection.Add("RedirectURL", RedirectURL);
            //datacollection.Add("MID", MID);
            datacollection.Add("Token", Token);
            datacollection.Add("GetMethod", "true");

            var form = PreparePOSTFormSaman(url: "https://sep.shaparak.ir/OnlinePG/OnlinePG", data: datacollection);

            return base.View("~/Plugins/NopFarsi.Payments.SepShaparak/Views/Pay.cshtml", model: form);
        }

        private String PreparePOSTFormSaman(string url, NameValueCollection data)
        {
            //Set a name for the form
            string formID = "PostForm";

            //Build the form using the specified data to be posted.
            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" + formID + "\" action=\"" + url + "\" method=\"POST\">");
            foreach (string key in data)
            {
                strForm.Append("<input type=\"hidden\" name=\"" + key + "\" value=\"" + data[key] + "\">");
            }
            strForm.Append("<input name=\"GetMethod\" type=\"text\" value=\"true\">");
            strForm.Append("</form>");

            //Build the JavaScript which will do the Posting operation.
            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("var v" + formID + " = document." + formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");

            //Return the form and the script concatenated. (The order is important, Form then JavaScript)
            return strForm.ToString() + strScript.ToString();
        }
        
        public ActionResult PaymentInfo()
        {
            return base.View("~/Plugins/NopFarsi.Payments.SepShaparak/Views/PaymentInformation.cshtml");
        }


        public ActionResult Verify()
        {
            var query = HttpContext.Request.Query;
            var state = query["State"].ToString();
            var stateCode = query["stateCode"].ToString();
            var resNum = query["resNum"].ToString();
            var mID = _SepPaymentSettings.MerchantId.ToString();//query["mID"].ToString();
            var refNum = query["refNum"].ToString();
            var cID = query["cID"].ToString();
            var tRACENO = query["tRACENO"].ToString();
            var securePan = query["securePan"].ToString();
            string data = $@"State:{state},stateCode:{stateCode},resNum:{resNum},mID:{mID}
                    ,refNum:{refNum},cID:{cID},tRACENO:{tRACENO},securePan:{securePan}";
            _logger.InsertLog(LogLevel.Information, "اطلاعات بازگشنی بانک سامان", data);
            //if (!stateCode.Equals("0"))
            //{
            //    string str = checkStatusCode(stateCode) + "\n\r";
            //    str = "چنانچه وجهی از حساب شما کسر شده باشد، حداکثر ظرف 72 ساعت به حساب شما بازخواهد گشت";
            //    return base.View("~/Plugins/NopFarsi.Payments.SepShaparak/Views/Verify.cshtml", model: str);
            //}

            var paymentService = new ir.sep.verify.PaymentIFBinding();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var amount = paymentService.verifyTransaction(refNum, mID);

            Order orderById = this._orderService.GetOrderById(int.Parse(resNum));
            _logger.InsertLog(LogLevel.Information,"نتیجه وریفای:"+orderById.Id.ToString(),amount.ToString());

            //وریفای موفق با نتیجه موفق
            if (Convert.ToInt32(orderById.OrderTotal) == Convert.ToInt32(amount))
            {
               // orderById.AuthorizationTransactionResult = (string.Format("[ResCode = {0}]", verifyRes));
                orderById.AuthorizationTransactionId = refNum.ToString();
                this._orderService.UpdateOrder(orderById);
                this._orderProcessingService.MarkOrderAsPaid(orderById);

                return base.RedirectToRoute("CheckoutCompleted", new
                {
                    orderId = orderById.Id
                });
            }
            else
            {
                LoggingExtensions.Information(this._logger, $"Error return Url amount:{amount} Order Id:{orderById.Id} refNum:{refNum}", null, null);
                return base.RedirectToRoute("HomePage");
            }
        }
        public class SepverifyModel
        {
            public string RefNum { get; set; }
            public string MerchantID { get; set; }
            public string  Action { get; set; }
        }
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpGet]
        //[AdminAuthorize, ChildActionOnly]
        public ActionResult Configure()
        {
            int activeStoreScopeConfiguration = this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
            sepsettings paymentSettings = this._settingService.LoadSetting<sepsettings>(activeStoreScopeConfiguration);
            ConfigurationModel configurationModel = new ConfigurationModel
            {
                ActiveStoreScopeConfiguration = activeStoreScopeConfiguration,
                MerchantId = paymentSettings.MerchantId,
                IsTomanForStore = paymentSettings.IsToman,
                DisablePaymentInfoForStore = paymentSettings.DisablePaymentInfo,
                RefundUserName = paymentSettings.RefundUserName,
                RefundPassword = paymentSettings.RefundPassword,
                RefundEmail = paymentSettings.RefundEmail,
                RefundCellPhone = paymentSettings.RefundCellPhone,
                TransactionTermId = paymentSettings.TransactionTermId,
                RefundTermId = paymentSettings.RefundTermId,
            };

            if (activeStoreScopeConfiguration > 0)
            {
                configurationModel.MerchantIdOverride_ForStore = _settingService.SettingExists(paymentSettings, (sepsettings x) => x.MerchantId, activeStoreScopeConfiguration);
                configurationModel.IsTomanOverrideForStore = _settingService.SettingExists(paymentSettings, (sepsettings x) => x.IsToman, activeStoreScopeConfiguration);
                configurationModel.DisablePaymentInfoOverrideForStore = _settingService.SettingExists(paymentSettings, (sepsettings x) => x.DisablePaymentInfo, activeStoreScopeConfiguration);
                configurationModel.RefundUserNameOverride_ForStore = _settingService.SettingExists(paymentSettings, (sepsettings x) => x.RefundUserName, activeStoreScopeConfiguration);
                configurationModel.RefundPasswordOverride_ForStore = _settingService.SettingExists(paymentSettings, (sepsettings x) => x.RefundPassword, activeStoreScopeConfiguration);
                configurationModel.RefundEmailOverride_ForStore = _settingService.SettingExists(paymentSettings, (sepsettings x) => x.RefundEmail, activeStoreScopeConfiguration);
                configurationModel.RefundCellPhoneOverride_ForStore = _settingService.SettingExists(paymentSettings, (sepsettings x) => x.RefundCellPhone, activeStoreScopeConfiguration);
                configurationModel.TransactionTermIdOverride_ForStore = _settingService.SettingExists(paymentSettings, (sepsettings x) => x.TransactionTermId, activeStoreScopeConfiguration);
                configurationModel.RefundTermIdOverride_ForStore = _settingService.SettingExists(paymentSettings, (sepsettings x) => x.RefundTermId, activeStoreScopeConfiguration);
            }
            return base.View("~/Plugins/NopFarsi.Payments.SepShaparak/Views/Configure.cshtml", configurationModel);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost]//, AdminAuthorize, ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }
            int activeStoreScopeConfiguration = this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
            sepsettings zarinpalPaymentSettings = this._settingService.LoadSetting<sepsettings>(activeStoreScopeConfiguration);

            zarinpalPaymentSettings.MerchantId = model.MerchantId;
            zarinpalPaymentSettings.IsToman = model.IsTomanForStore;
            zarinpalPaymentSettings.DisablePaymentInfo = model.DisablePaymentInfoForStore;
            zarinpalPaymentSettings.RefundUserName = model.RefundUserName;
            zarinpalPaymentSettings.RefundPassword = model.RefundPassword;
            zarinpalPaymentSettings.RefundEmail = model.RefundEmail;
            zarinpalPaymentSettings.RefundCellPhone = model.RefundCellPhone;
            zarinpalPaymentSettings.TransactionTermId = model.TransactionTermId;
            zarinpalPaymentSettings.RefundTermId = model.RefundTermId;

            _settingService.SaveSettingOverridablePerStore(zarinpalPaymentSettings, (sepsettings x) => x.MerchantId, model.MerchantIdOverride_ForStore, activeStoreScopeConfiguration, false);
            _settingService.SaveSettingOverridablePerStore(zarinpalPaymentSettings, (sepsettings x) => x.IsToman, model.IsTomanForStore, activeStoreScopeConfiguration, false);
            _settingService.SaveSettingOverridablePerStore(zarinpalPaymentSettings, (sepsettings x) => x.DisablePaymentInfo, model.DisablePaymentInfoForStore, activeStoreScopeConfiguration, false);
            _settingService.SaveSettingOverridablePerStore(zarinpalPaymentSettings, (sepsettings x) => x.RefundUserName, model.RefundUserNameOverride_ForStore, activeStoreScopeConfiguration, false);
            _settingService.SaveSettingOverridablePerStore(zarinpalPaymentSettings, (sepsettings x) => x.RefundPassword, model.RefundPasswordOverride_ForStore, activeStoreScopeConfiguration, false);
            _settingService.SaveSettingOverridablePerStore(zarinpalPaymentSettings, (sepsettings x) => x.RefundEmail, model.RefundEmailOverride_ForStore, activeStoreScopeConfiguration, false);
            _settingService.SaveSettingOverridablePerStore(zarinpalPaymentSettings, (sepsettings x) => x.RefundCellPhone, model.RefundCellPhoneOverride_ForStore, activeStoreScopeConfiguration, false);
            _settingService.SaveSettingOverridablePerStore(zarinpalPaymentSettings, (sepsettings x) => x.TransactionTermId, model.TransactionTermIdOverride_ForStore, activeStoreScopeConfiguration, false);
            _settingService.SaveSettingOverridablePerStore(zarinpalPaymentSettings, (sepsettings x) => x.RefundTermId, model.RefundTermIdOverride_ForStore, activeStoreScopeConfiguration, false);
            _settingService.ClearCache();
            base.SuccessNotification("با موفقیت ذخیره شد");
            return base.View("~/Plugins/NopFarsi.Payments.SepShaparak/Views/Configure.cshtml", model);
        }

        private string checkStatusCode(string statusCode)
        {
            switch (statusCode)
            {
                case "-1":
                    return "تراکنش توسط خریدار کنسل شده است.";
                case "79":
                    return "مبلغ سند برگشتی، از مبلغ تراکنش اصلی بیشتر است.";
                case "12":
                    return "درخواست برگشت یک تراکنش رسیده است، در حالی کهتراکنش اصلی پیدا نتی شود";
                case "14":
                    return "شماره کارت نامعتبر است";
                case "15":
                    return "چنین صادر کننده کارتی وجود ندارد.";
                case "33":
                    return "از تاریخ انقضای کارت گذشته است و کارت دیگر معتبر نیست";
                case "38":
                    return "رمز کارت ) PIN ) 3 مرتبه اشتباه وارد شده است در نتیجه کارت غیر فعال خواهد شد.";
                case "55":
                    return "خریدار رمز کارت ) PIN ( را اشتباه وارد کرده است.";
                case "61":
                    return "مبلغ بیش از سقف برداشت می باشد";
                case "93":
                    return "تراکنش Authorize شده است )شتاره PIN و PAN درست هستند(ولی امکان سند خوردن وجود ندارد.";
                case "68":
                    return "تراکنش در شبکه بانکی Timeout خورده است.";
                case "34":
                    return "خریدار یا فیلد CVV2 و یا فیلد ExpDate را اشتباه وارد کرده است )یا اصلا وارد نکرده است(.";
                case "51":
                    return "موجودی حساب خریدار، کافی نیست.";
                case "84":
                    return "سیستم بانک صادر کننده کارت خریدار، در وضعیت عتلیاتی نیست";
                case "96":
                    return "کلیه خطاهای دیگر بانکی باعث ایجاد چنین خطایی می گردد.";
            }
            return "";
        }
    }
}
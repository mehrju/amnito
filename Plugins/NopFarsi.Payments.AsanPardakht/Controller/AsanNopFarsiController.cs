using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using NopFarsi.Payments.AsanPardakht.Models;
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
using NopFarsi.Payments.AsanPardakht.Service;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Logging;

namespace NopFarsi.Payments.AsanPardakht.Controller
{

    public class AsanNopFarsiController : BasePaymentController
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
        private readonly IStoreContext _storeContext;
        public AsanNopFarsiController(IStoreContext storeContext, IWorkContext workContext, IStoreService storeService, ISettingService settingService, ILocalizationService localizationService, IPaymentService paymentService, PaymentSettings paymentSettings, ILogger logger, IOrderService orderService, IOrderProcessingService orderProcessingService)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._paymentService = paymentService;
            this._paymentSettings = paymentSettings;
            this._logger = logger;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            _storeContext = storeContext;
        }


        //[ValidateInput(false)]
        public ActionResult Pay(string ResNum, string MID, string Amount, string RedirectURL)
        {
            try
            {
                var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
                var paymentSettings = _settingService.LoadSetting<AsanPardakhtSettings>(storeScope);
                var sett = Newtonsoft.Json.JsonConvert.SerializeObject(paymentSettings);

                string encryptedRequest = string.Join(",",
                     1,
                     paymentSettings.UserNameMerchent.Trim(),
                     paymentSettings.PassMerchent,
                     ResNum + "" + DateTime.Now.Second,
                     Amount,
                     DateTime.Now.Year + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + DateTime.Now.ToString("HHMMss"),
                     "",
                     RedirectURL,
                     "0"
                     );
                AES2 aesProvider = new AES2(paymentSettings.Key, paymentSettings.VectorEncriptor);
                bool decryptionIsSuccessful = aesProvider.Encrypt(encryptedRequest, out encryptedRequest);

                string token;
                token = (new asanpardakht.services.merchantservices()).RequestOperation(paymentSettings.ConfigMerchentId, encryptedRequest); ;
                EngineContext.Current.Resolve<ILogger>().InsertLog(LogLevel.Information, "Result AsabPardakht: " + token);
                var result = token.Split(',');
                if (result[0] == "0")
                {
                    NameValueCollection datacollection = new NameValueCollection();
                    datacollection.Add("RefId", result[1]);
                    string text31 = PreparePOSTFormSaman("https://asan.shaparak.ir", datacollection);
                    return base.View("~/Plugins/NopFarsi.Payments.AsanPardakht/Views/Pay.cshtml", model: text31);
                }
                else
                {
                    Log("خطا در زمان پرداخت با درگاه آسان پرداخت", "خطا در پرداخت. کد:" + result[0] + "|" + ResNum);
                    return base.View("~/Plugins/NopFarsi.Payments.AsanPardakht/Views/Pay.cshtml", model: "خطا در پرداخت. کد:" + result[0] + "|" + ResNum);

                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                return base.View("~/Plugins/NopFarsi.Payments.AsanPardakht/Views/Pay.cshtml", model: "از بانک پذیرنده پاسخی دریافت نشد" + "|" + ResNum);
            }
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
        //[ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            return base.View("~/Plugins/NopFarsi.Payments.AsanPardakht/Views/PaymentInformation.cshtml");
        }


        //[ValidateInput(false)]
        public ActionResult Verify(int orderId, string ReturningParams)
        {
            string returningParamsString = ReturningParams.Replace(" ", "+");
            Log("آسان پرداخت", ReturningParams);
            string decryptedReturningParamsString = string.Empty;
            int activeStoreScopeConfiguration = _storeContext.CurrentStore.Id;//this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
            AsanPardakhtSettings paymentSettings = this._settingService.LoadSetting<AsanPardakhtSettings>(activeStoreScopeConfiguration);
            AES2 aesProvider = new AES2(paymentSettings.Key, paymentSettings.VectorEncriptor);
            Log("آسان پرداخت", paymentSettings.Key + "|" + paymentSettings.VectorEncriptor);
            bool decryptionIsSuccessful = aesProvider.Decrypt(returningParamsString, out decryptedReturningParamsString);
            Log("آسان پرداخت", decryptionIsSuccessful.ToString());
            if (!decryptionIsSuccessful)
            {
                string str = "تراکنشی یافت نشد. ";
                str += "چنانچه وجهی از حساب شما کسر شده باشد، حداکثر ظرف 72 ساعت به حساب شما بازخواهد گشت";
                //str += "--->" +decryptionIsSuccessful .ToString()+"-->"+ returningParamsString;
                return base.View("~/Plugins/NopFarsi.Payments.AsanPardakht/Views/Verify.cshtml", model: str);

            }
            AsanPardakhtPGResultDescriptor trxResultDescriptor = AsanPardakhtPGResultDescriptor.AsanPardakhtTrxResultDescriptorFromString(decryptedReturningParamsString);
            Log("آسان پرداخت", (trxResultDescriptor == null ? "trxResultDescriptor is null" : trxResultDescriptor.PreInvoiceID.ToString()));
            if (trxResultDescriptor == null)
            {
                string str = "تراکنشی یافت نشد. ";
                str += "چنانچه وجهی از حساب شما کسر شده باشد، حداکثر ظرف 72 ساعت به حساب شما بازخواهد گشت";
                //str += "--->" + decryptionIsSuccessful.ToString() + "-!->" + (decryptedReturningParamsString ?? "***");
                return base.View("~/Plugins/NopFarsi.Payments.AsanPardakht/Views/Verify.cshtml", model: str);
            }

            int iPreInvoiceID;
            if (!int.TryParse(trxResultDescriptor.PreInvoiceID, out iPreInvoiceID) || iPreInvoiceID < 1)
            {
                return base.View("~/Plugins/NopFarsi.Payments.AsanPardakht/Views/Verify.cshtml", model: "تراکنشی یافت نشد");

            }
            if (trxResultDescriptor.ResCode == "911")
            {
                string str = "شما از انجام تراکنش منصرف شدید. ";
                str += "در صورت تمایل می توانید دوباره خرید خود را انجام دهید";
                return base.View("~/Plugins/NopFarsi.Payments.AsanPardakht/Views/Verify.cshtml", model: str);
            }
            // حال لازم است چک نهایی روی شماره پیگیری پرداخت را انجام دهید
            ulong refNumb;
            if (!ulong.TryParse(trxResultDescriptor.PayGateTranID, out refNumb))
            {
                string str = "به دلیل بروز مشکلی امکان ثبت پرداخت شما وجود ندارد. ";
                str += "چنانچه وجهی از حساب شما کسر شده باشد، حداکثر ظرف 72 ساعت به حساب شما بازخواهد گشت";
                return base.View("~/Plugins/NopFarsi.Payments.AsanPardakht/Views/Verify.cshtml", model: str);
            }

            // در حالت عادی شما بعد از دریافت نتیجه قطعی پرداخت موفق، فرآیندهای تسویه یا عودت وجه را بصورت خودکار انجام می دهید
            // اما در این پروژه آموزشی شماره پیگیری تراکنش ذخیره می شود تا با فشار دادن دکمه های متناظر فرآیند های تکمیلی را بصورت دستی انجام دهید

            {
                AsanPaymentProcessor sepPaymentProcessor = this._paymentService.LoadPaymentMethodBySystemName("NopFarsi.Payments.AsanPardakht") as AsanPaymentProcessor;
                if (sepPaymentProcessor == null || !PaymentExtensions.IsPaymentMethodActive(sepPaymentProcessor, this._paymentSettings) || !sepPaymentProcessor.PluginDescriptor.Installed)
                {
                    throw new NopException("module cannot be loaded");
                }
                //if (!zarinpalPaymentProcessor.VerifyPayment(authority, status, out orderId))
                //{
                //    return base.RedirectToRoute("HomePage");
                //}
                var paymentService = new asanpardakht.services.merchantservices();//shaparak.AsanPardakht.PaymentIFBinding();

                ServicePointManager.ServerCertificateValidationCallback =
                               delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                // فراخوانی وب سرویس 
                AsanPardakhtProvider asanPardakhtProvider = new AsanPardakhtProvider(paymentSettings.MerchantId, paymentSettings.ConfigMerchentId, paymentSettings.UserNameMerchent, paymentSettings.PassMerchent, paymentSettings.Key, paymentSettings.VectorEncriptor);
                string verifyRes = string.Empty;
                bool verified = asanPardakhtProvider.VerifyTrx(refNumb, out verifyRes);

                //وریفای موفق با نتیجه موفق
                if (verified && verifyRes == "500")
                {
                    Order orderById = this._orderService.GetOrderById(orderId);
                    orderById.AuthorizationTransactionResult = (string.Format("[ResCode = {0}]", verifyRes));
                    orderById.AuthorizationTransactionId = (refNumb.ToString());
                    this._orderService.UpdateOrder(orderById);
                    this._orderProcessingService.MarkOrderAsPaid(orderById);

                    return base.RedirectToRoute("CheckoutCompleted", new
                    {
                        orderId
                    });
                }
                else
                {
                    LoggingExtensions.Information(this._logger, "Error return Url Sep: " + verifyRes + " Order Id:" + orderId, null, null);
                    return base.RedirectToRoute("HomePage");
                }

            }
            return base.RedirectToRoute("HomePage");
        }



        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpGet]
        //[AdminAuthorize, ChildActionOnly]
        public ActionResult Configure()
        {
            int activeStoreScopeConfiguration = this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
            AsanPardakhtSettings paymentSettings = this._settingService.LoadSetting<AsanPardakhtSettings>(activeStoreScopeConfiguration);
            ConfigurationModel configurationModel = new ConfigurationModel
            {
                ActiveStoreScopeConfiguration = activeStoreScopeConfiguration,
                MerchantId = paymentSettings.MerchantId,
                ConfigMerchentId = paymentSettings.ConfigMerchentId,
                Key = paymentSettings.Key,
                PassMerchent = paymentSettings.PassMerchent,
                UserNameMerchent = paymentSettings.UserNameMerchent,
                VectorEncriptor = paymentSettings.VectorEncriptor,
                IsTomanForStore = paymentSettings.IsToman,
                DisablePaymentInfoForStore = paymentSettings.DisablePaymentInfo,
                NopFarsi = "طراحی و تولید توسط ناپ فارسی"
            };

            if (activeStoreScopeConfiguration > 0)
            {
                configurationModel.MerchantIdOverride_ForStore = this._settingService.SettingExists<AsanPardakhtSettings, int>(paymentSettings, (AsanPardakhtSettings x) => x.MerchantId, activeStoreScopeConfiguration);
                configurationModel.ConfigMerchentId_ForStore = this._settingService.SettingExists<AsanPardakhtSettings, int>(paymentSettings, (AsanPardakhtSettings x) => x.ConfigMerchentId, activeStoreScopeConfiguration);
                configurationModel.UserNameMerchent_ForStore = this._settingService.SettingExists<AsanPardakhtSettings, string>(paymentSettings, (AsanPardakhtSettings x) => x.UserNameMerchent, activeStoreScopeConfiguration);
                configurationModel.PassMerchent_ForStore = this._settingService.SettingExists<AsanPardakhtSettings, string>(paymentSettings, (AsanPardakhtSettings x) => x.PassMerchent, activeStoreScopeConfiguration);
                configurationModel.Key_ForStore = this._settingService.SettingExists<AsanPardakhtSettings, string>(paymentSettings, (AsanPardakhtSettings x) => x.Key, activeStoreScopeConfiguration);
                configurationModel.VectorEncriptor_ForStore = this._settingService.SettingExists<AsanPardakhtSettings, string>(paymentSettings, (AsanPardakhtSettings x) => x.VectorEncriptor, activeStoreScopeConfiguration);
                configurationModel.IsTomanOverrideForStore = this._settingService.SettingExists<AsanPardakhtSettings, bool>(paymentSettings, (AsanPardakhtSettings x) => x.IsToman, activeStoreScopeConfiguration);
                configurationModel.DisablePaymentInfoOverrideForStore = this._settingService.SettingExists<AsanPardakhtSettings, bool>(paymentSettings, (AsanPardakhtSettings x) => x.DisablePaymentInfo, activeStoreScopeConfiguration);

            }
            return base.View("~/Plugins/NopFarsi.Payments.AsanPardakht/Views/Configure.cshtml", configurationModel);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost]//, AdminAuthorize, ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!base.ModelState.IsValid)
            {
                return this.Configure();
            }
            if (model.NopFarsi != "طراحی و تولید توسط ناپ فارسی")
            {
                return base.Content("خطای 1054 رخ داده است. لطفا با پشتیبانی تماس حاصل شود.");
            }
            int activeStoreScopeConfiguration = this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
            AsanPardakhtSettings asanPardakhtSettings = this._settingService.LoadSetting<AsanPardakhtSettings>(activeStoreScopeConfiguration);


            asanPardakhtSettings.MerchantId = model.MerchantId;
            asanPardakhtSettings.ConfigMerchentId = model.ConfigMerchentId;
            asanPardakhtSettings.UserNameMerchent = model.UserNameMerchent;
            asanPardakhtSettings.PassMerchent = model.PassMerchent;
            asanPardakhtSettings.Key = model.Key;
            asanPardakhtSettings.VectorEncriptor = model.VectorEncriptor;

            asanPardakhtSettings.IsToman = model.IsTomanForStore;
            asanPardakhtSettings.DisablePaymentInfo = model.DisablePaymentInfoForStore;

            this._settingService.SaveSettingOverridablePerStore<AsanPardakhtSettings, int>(asanPardakhtSettings, (AsanPardakhtSettings x) => x.MerchantId, model.MerchantIdOverride_ForStore, activeStoreScopeConfiguration, false);
            this._settingService.SaveSettingOverridablePerStore<AsanPardakhtSettings, int>(asanPardakhtSettings, (AsanPardakhtSettings x) => x.ConfigMerchentId, model.ConfigMerchentId_ForStore, activeStoreScopeConfiguration, false);
            this._settingService.SaveSettingOverridablePerStore<AsanPardakhtSettings, string>(asanPardakhtSettings, (AsanPardakhtSettings x) => x.UserNameMerchent, model.UserNameMerchent_ForStore, activeStoreScopeConfiguration, false);
            this._settingService.SaveSettingOverridablePerStore<AsanPardakhtSettings, string>(asanPardakhtSettings, (AsanPardakhtSettings x) => x.PassMerchent, model.PassMerchent_ForStore, activeStoreScopeConfiguration, false);
            this._settingService.SaveSettingOverridablePerStore<AsanPardakhtSettings, string>(asanPardakhtSettings, (AsanPardakhtSettings x) => x.Key, model.Key_ForStore, activeStoreScopeConfiguration, false);
            this._settingService.SaveSettingOverridablePerStore<AsanPardakhtSettings, string>(asanPardakhtSettings, (AsanPardakhtSettings x) => x.VectorEncriptor, model.VectorEncriptor_ForStore, activeStoreScopeConfiguration, false);

            this._settingService.SaveSettingOverridablePerStore<AsanPardakhtSettings, bool>(asanPardakhtSettings, (AsanPardakhtSettings x) => x.IsToman, model.IsTomanForStore, activeStoreScopeConfiguration, false);
            this._settingService.SaveSettingOverridablePerStore<AsanPardakhtSettings, bool>(asanPardakhtSettings, (AsanPardakhtSettings x) => x.DisablePaymentInfo, model.DisablePaymentInfoForStore, activeStoreScopeConfiguration, false);
            this._settingService.ClearCache();
            base.SuccessNotification("با موفقیت ذخیره شد. تولید شده توسط ناپ فارسی . www.nopfarsi.ir");
            return base.View("~/Plugins/NopFarsi.Payments.AsanPardakht/Views/Configure.cshtml", model);
        }

        public void Log(string header, string Message)
        {
            var logger =
                (DefaultLogger)EngineContext.Current
                    .Resolve<ILogger>();
            logger.InsertLog(LogLevel.Information, header, Message, null);
        }
    }
}

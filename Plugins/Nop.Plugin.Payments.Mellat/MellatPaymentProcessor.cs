using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Text;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.Mellat.Controllers;
using Nop.Plugin.Payments.Mellat.ir.shaparak.bpm;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Tax;
using Nop.Core.Domain.Payments;
using Microsoft.AspNetCore.Http;
using Nop.Services.Logging;
using Nop.Core.Http.Extensions;
using Newtonsoft.Json;
using System.Net;

namespace Nop.Plugin.Payments.Mellat
{
    public class MellatPaymentProcessor : BasePlugin, IPaymentMethod, IPlugin
    {
        private readonly ICurrencyService _currencySettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ISettingService _settingService;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly MellatPaymentSettings _MellatPaymentSettings;
        private readonly ILogger _logger;
        public MellatPaymentProcessor(ICurrencyService currencySettings, //HttpContextBase httpContext, 
            ICheckoutAttributeParser checkoutAttributeParser,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ISettingService settingService,
            ITaxService taxService,
            IHttpContextAccessor httpContextAccessor,
            IWebHelper webHelper, ILogger logger,
            MellatPaymentSettings mellatPaymentSettings)
        {
            this._currencySettings = currencySettings;
            this._httpContextAccessor = httpContextAccessor;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._currencyService = currencyService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._settingService = settingService;
            this._taxService = taxService;
            this._webHelper = webHelper;
            this._MellatPaymentSettings = mellatPaymentSettings;
            this._logger = logger;
        }

        private string ToTwoDigit(int i)
        {
            string arg = "";
            if (i < 10)
            {
                arg = "0";
            }
            return arg + i;
        }

        private string LocalDate
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(DateTime.Now.Year);
                stringBuilder.Append(this.ToTwoDigit(DateTime.Now.Month));
                stringBuilder.Append(this.ToTwoDigit(DateTime.Now.Day));
                return stringBuilder.ToString();
            }
        }

        private string LocalTime
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(this.ToTwoDigit(DateTime.Now.Hour));
                stringBuilder.Append(this.ToTwoDigit(DateTime.Now.Minute));
                stringBuilder.Append(this.ToTwoDigit(DateTime.Now.Second));
                return stringBuilder.ToString();
            }
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            ProcessPaymentResult processPaymentResult = new ProcessPaymentResult();
            processPaymentResult.NewPaymentStatus = PaymentStatus.Pending;
            return processPaymentResult;
        }

        long LongRandom(long min, long max, Random rand)
        {
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //if (!this._webHelper.GetStoreLocation().Contains("monadi"))
            //{
            //    throw new Exception();
            //}
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            {
                decimal value = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);
                long OrderId = (long)postProcessPaymentRequest.Order.Id;
                string terminalId = this._MellatPaymentSettings.TerminalId;
                string userName = this._MellatPaymentSettings.UserName;
                string userPassword = this._MellatPaymentSettings.UserPassword;
                int num2 = 1;
                //if (this._MellatPaymentSettings.OrderId.ToString() != "")
                //{
                //    num2 = this._MellatPaymentSettings.OrderId;
                //}
                long orderId = LongRandom(0, long.MaxValue, new Random());// Convert.ToInt64(this._MellatPaymentSettings.OrderId);
                //this._MellatPaymentSettings.OrderId = num2 + 1;
                var _session = _httpContextAccessor.HttpContext.Session;
                bool isFromApp = SessionExtensions.Get<bool>(_session, "isFromApp");
                this._settingService.SaveSetting<MellatPaymentSettings>(this._MellatPaymentSettings, 0);
                if (terminalId == "" || userName == "" || userPassword == "")
                {
                    this._httpContextAccessor.HttpContext.Response.Redirect(this._webHelper.GetStoreLocation() + "Plugins/PaymentMellat/Error?resCode=-1&Id=" + OrderId);
                }
                else
                {
                    long orderTotal = Convert.ToInt64(value);
                    if (this._MellatPaymentSettings.Toman)
                    {
                        orderTotal *= 10L;
                    }
                    string additionalData = "خرید از فروشگاه اینترنتی " + OrderId;
                    string callBackUrl = this._webHelper.GetStoreLocation() + "Plugins/PaymentMellat/CallBack?OrderId=" + OrderId + "&isFromApp=" + isFromApp; ;
                    string localDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
                    string localTime = DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');
                    PaymentGatewayImplService paymentGatewayImplService = new PaymentGatewayImplService();
                    paymentGatewayImplService.Credentials = System.Net.CredentialCache.DefaultCredentials; //Test Vakili

                    string text = paymentGatewayImplService.bpPayRequest(long.Parse(terminalId), userName, userPassword, OrderId, orderTotal, localDate
                        , localTime, additionalData, callBackUrl, long.Parse("0"));
                   
                    string[] array = text.Split(new char[]
                    {
                        ','
                    });
                    if (array[0] == "0")
                    {
                        string redirectUrl = this._webHelper.GetStoreLocation() + "Plugins/PaymentMellat/Pay?result=" + array[1];
                        if (!isFromApp)
                        {
                            this._httpContextAccessor.HttpContext.Response.Redirect(redirectUrl);
                        }
                        else
                        {
                            SessionExtensions.Set<string>(_session, "redirectUrl", redirectUrl);
                        }
                    }
                    else
                    {
                        this._httpContextAccessor.HttpContext.Response.Redirect(string.Concat(new string[]
                        {
                            this._webHelper.GetStoreLocation(),
                            "Plugins/PaymentMellat/Error?resCode=",
                            text,
                            "&id=",
                            orderId.ToString()
                        }));
                    }
                }
            }
        }

        //public void UpdateOrderId(int? orderId, int? installOrderId)
        //{
        //    MellatPaymentSettings mellatPaymentSettings = new MellatPaymentSettings
        //    {
        //        OrderId = (orderId ?? this._MellatPaymentSettings.OrderId),
        //        TerminalId = this._MellatPaymentSettings.TerminalId,
        //        UserName = this._MellatPaymentSettings.UserName,
        //        UserPassword = this._MellatPaymentSettings.UserPassword,
        //        AdditionalFee = this._MellatPaymentSettings.AdditionalFee,
        //        AdditionalFeePercentage = this._MellatPaymentSettings.AdditionalFeePercentage,
        //        ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage = this._MellatPaymentSettings.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage
        //    };
        //    this._settingService.SaveSetting<MellatPaymentSettings>(mellatPaymentSettings, 0);
        //}

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return PaymentExtensions.CalculateAdditionalFee(this, this._orderTotalCalculationService, cart, this._MellatPaymentSettings.AdditionalFee, this._MellatPaymentSettings.AdditionalFeePercentage);
        }




        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException("order");
            }
            return (DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds >= 5.0;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            if (this.PluginDescriptor.Author != "NopFarsi.ir  (MJ Vakili)")
                throw new Exception();
            return _webHelper.GetStoreLocation() + "Admin/NopFarsiPaymentMellat/Configure";
        }

        private void log(string shortMessage, string fullMessage)
        {
            _logger.InsertLog(Core.Domain.Logging.LogLevel.Information, shortMessage, fullMessage);
        }
        public override void Install()
        {

            MellatPaymentSettings mellatPaymentSettings = new MellatPaymentSettings
            {
                TerminalId = "1",
                UserName = "1",
                UserPassword = "1",
                //OrderId = 1,
                Toman = true
            };
            this._settingService.SaveSetting<MellatPaymentSettings>(mellatPaymentSettings, 0);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.OrderId", "شماره درخواست بانک ملت", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "plugins.payments.mellat.paymentmethoddescription", "پرداخت توسط کلیه بانک های عضو شتاب", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.OrderId.Hint", "شماره ای غیر تکراری که در هر بار قبل از رفتن به درگاه بانک ارسال می شود", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.TerminalId", "شماره ترمینال", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.TerminalId.Hint", "شماره ترمینال بانک ملت", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.UserName", "نام کاربری", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.UserName.Hint", "نام کاربری", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.UserPassword", "رمز عبور", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.UserPassword.Hint", "رمز عبور", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.ActivationCode", "کد فعالسازی", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.ActivationCode.Hint", "کد فعالسازی", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.SystemCode", "کد سیستم", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.SystemCode.Hint", "کد سیستم", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.Toman", "استفاده از تومان", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.Toman.Hint", "در صورتی که قیمت کالاها بر حسب تومان می باشد ، این گزینه را تیک بزنید", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.RedirectionTip", "برای تکمیل سفارش به درگاه پرداخت هدایت خواهید شد", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.BusinessEmail", "Business Email", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.BusinessEmail.Hint", "Specify your Mellat business email.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.AdditionalFee", "هزینه های اضافی", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.AdditionalFee.Hint", "هزینه های اضافی برای مطالبه از مشتریان خود وارد نمایید", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.AdditionalFeePercentage", "هزینه اضافی. از درصد استفاده نمایید", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.AdditionalFeePercentage.Hint", "تعیین اینکه آیا  درصد هزینه های اضافی به کل سفارش اعمال شود. اگر فعال نشود ، یک مقدار ثابت استفاده می شود", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage", "بازگشت به صفحه جزئیات سفارش", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage.Hint", "Enable if a customer should be redirected to the order details page when he clicks \"return to store\" link on Mellat site WITHOUT completing a payment", null);
            base.Install();
        }

        public override void Uninstall()
        {
            this._settingService.DeleteSetting<MellatPaymentSettings>();
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.OrderId");
            LocalizationExtensions.DeletePluginLocaleResource(this, "plugins.payments.mellat.paymentmethoddescription");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.TerminalId");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.UserName");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.UserPassword");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.ActivationCode");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.SystemCode");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.Toman");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.RedirectionTip");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.BusinessEmail");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.BusinessEmail.Hint");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.AdditionalFee");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.AdditionalFee.Hint");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.AdditionalFeePercentage");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.AdditionalFeePercentage.Hint");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.Payments.Mellat.Fields.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage.Hint");
            base.Uninstall();
        }

        public IList<string> ValidatePaymentForm(Microsoft.AspNetCore.Http.IFormCollection form)
        {
            if (form == null)
                throw new ArgumentException("");//nameof(form));

            StringValues errorsString;
            //try to get errors
            if (form.TryGetValue("Errors", out errorsString) && !StringValues.IsNullOrEmpty(errorsString))
                return (new[] { errorsString.ToString() });

            return new List<string>();
        }

        public ProcessPaymentRequest GetPaymentInfo(Microsoft.AspNetCore.Http.IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "PaymentInfo";
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            throw new NotImplementedException();
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return 0;
            }
        }

        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Redirection;
            }
        }

        public bool SkipPaymentInfo
        {
            get
            {
                return true;
            }
        }

        public string PaymentMethodDescription
        {
            get
            {
                return this._localizationService.GetResource("Plugins.Payments.Mellat.PaymentMethodDescription");
            }
        }

    }
}

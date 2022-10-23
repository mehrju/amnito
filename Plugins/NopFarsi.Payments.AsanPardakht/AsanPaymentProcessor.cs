using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Web;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Data;
//using NopFarsi.Payments.AsanPardakht.Controller;
//using NopFarsi.Payments.AsanPardakht.SepPeymentService;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Logging;

using System.Collections.Specialized;
using System.Text;
using NopFarsi.Payments.AsanPardakht.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Http.Extensions;

namespace NopFarsi.Payments.AsanPardakht
{

    public class AsanPaymentProcessor : BasePlugin, IPaymentMethod, IPlugin
    {

        private const string GenericAttributeAuthority = "SepAuthority";
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AsanPardakhtSettings _SepPaymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        public AsanPaymentProcessor(
            IHttpContextAccessor httpContextAccessor,
            AsanPardakhtSettings SepPaymentSettings,
            ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper,
            IGenericAttributeService genericAttributeService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            ILogger logger)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._SepPaymentSettings = SepPaymentSettings;
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._genericAttributeService = genericAttributeService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._logger = logger;
           
        }
    
        private string TranslateStatus(int statusCode)
        {
            string resource = this._localizationService.GetResource(string.Format("NopFarsi.AsanPardakht.StatusCode.{0}", statusCode));
            if (!string.IsNullOrWhiteSpace(resource))
            {
                return resource;
            }
            return statusCode.ToString();
        }


        private BasicHttpBinding Binding
        {
            get
            {
                return new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            }
        }


        private EndpointAddress EndPoint
        {
            get
            {
                return new EndpointAddress(this._SepPaymentSettings.WebServiceUrl);
            }
        }


        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            ProcessPaymentResult expr_05 = new ProcessPaymentResult();
            expr_05.NewPaymentStatus = (Nop.Core.Domain.Payments.PaymentStatus)10;
            return expr_05;
        }


        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            decimal value = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);
            long num = (long)postProcessPaymentRequest.Order.Id;

            long num3 = Convert.ToInt64(value);
            if (this._SepPaymentSettings.IsToman)
            {
                num3 *= 10L;
            }

            var _session = _httpContextAccessor.HttpContext.Session;
            bool isFromApp = SessionExtensions.Get<bool>(_session, "isFromApp");

            string callBackUrl = this._webHelper.GetStoreLocation(false) + "Plugins/AsanPardakhtNopFarsi/Verify?OrderId=" + num + "&isFromApp=" + isFromApp;

            NameValueCollection datacollection = new NameValueCollection();

            datacollection.Add("ResNum", postProcessPaymentRequest.Order.Id.ToString());

            datacollection.Add("MID", this._SepPaymentSettings.MerchantId.ToString());


            datacollection.Add("RedirectURL", callBackUrl);

            datacollection.Add("Amount", num3.ToString());




            var redirectUrl = this._webHelper.GetStoreLocation(false) + "Plugins/AsanPardakhtNopFarsi/Pay?ResNum=" + postProcessPaymentRequest.Order.Id.ToString() + "&MID=" + this._SepPaymentSettings.MerchantId.ToString() +
                "&Amount=" + num3.ToString() +
                "&RedirectURL=" + callBackUrl;

            if (isFromApp)
            {
                SessionExtensions.Set<string>(_session, "redirectUrl", redirectUrl);
            }
            else
            {
                this._httpContextAccessor.HttpContext.Response.Redirect(redirectUrl);
            }
        }


        //this._httpContext.Response.Write(text31);

        //this._httpContext.Response.Redirect(string.Concat(new string[]
        //        {
        //            this._webHelper.GetStoreLocation(false),
        //            "Plugins/PaymentMellat/Error?resCode=",
        //            //text,
        //            "&id=",
        //            //orderId.ToString()
        //        }));



        //string text = string.Empty;
        //ServicePointManager.Expect100Continue = false;
        //PaymentGatewayImplementationServicePortTypeClient service = new PaymentGatewayImplementationServicePortTypeClient(this.Binding, this.EndPoint);
        //string callbackURL = this._webHelper.GetStoreLocation(false) + "Plugins/SepNopFarsi/Verify";
        //decimal value = Math.Round(postProcessPaymentRequest.Order.OrderTotal / 10m, 0);
        //if (this._SepPaymentSettings.IsToman)
        //{
        //    value *= 10;
        //}
        //string text2;
        //int num = service.PaymentRequest(this._SepPaymentSettings.MerchantId, (int)value * 10, string.Format(this._localizationService.GetResource("NopFarsi.AsanPardakht.Payment.Description"), postProcessPaymentRequest.Order.Id.ToString("#,#")), postProcessPaymentRequest.Order.BillingAddress.Email, postProcessPaymentRequest.Order.BillingAddress.PhoneNumber, callbackURL, out text2);
        //if (num == 100)
        //{
        //    this._genericAttributeService.SaveAttribute<string>(postProcessPaymentRequest.Order, "SepAuthority", text2, 0);
        //    text = string.Format(this._SepPaymentSettings.PayementUrl, text2);
        //}
        //if (string.IsNullOrWhiteSpace(text))
        //{
        //    throw new Exception(string.Format(this._localizationService.GetResource("NopFarsi.AsanPardakht.Payment.FailureMessage"), this.TranslateStatus(num)));
        //}
        //this._httpContext.Response.Redirect(text);




        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;// !ShoppingCartExtensions.RequiresShipping(cart);
        }


        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return PaymentExtensions.CalculateAdditionalFee(this, this._orderTotalCalculationService, cart, 0, false);
        }


        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            CapturePaymentResult capture = new CapturePaymentResult();
            capture.AddError("Capture method not supported.");
            return capture;
        }


        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            RefundPaymentResult refund = new RefundPaymentResult();
            refund.AddError("Refund method not supported.");
            return refund;
        }


        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            VoidPaymentResult voidPay = new VoidPaymentResult();
            voidPay.AddError("Void method not supported.");
            return voidPay;
        }


        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            ProcessPaymentResult p = new ProcessPaymentResult();
            p.AddError("Recurring payment not supported.");
            return p;
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
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            CancelRecurringPaymentResult c = new CancelRecurringPaymentResult();
            c.AddError("Recurring payment not supported.");
            return c;
        }


        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException("order");
            }
            return (DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds >= 5.0;
        }

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/AsanNopFarsi/Configure";
        }
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "AsanNopFarsi";
            routeValues = new RouteValueDictionary
			{
				{
					"Namespaces",
					"NopFarsi.Payments.AsanPardakht.Controllers"
				},
				{
					"area",
					null
				}
			};
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "AsanPaymentInfo";
        }

        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        //public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        //{
        //    actionName = "PaymentInfo";
        //    controllerName = "SepNopFarsi";
        //    routeValues = new RouteValueDictionary
        //    {
        //        {
        //            "Namespaces",
        //            "NopFarsi.Payments.AsanPardakht.Controllers"
        //        },
        //        {
        //            "area",
        //            null
        //        }
        //    };
        //}


        public Type GetControllerType()
        {
            return typeof(AsanNopFarsiController);
        }


        public override void Install()
        {
            AsanPardakhtSettings SepPaymentSettings = new AsanPardakhtSettings
            {
                MerchantId = 0,
                //PayementUrl = "https://www.AsanPardakht.com/pg/StartPay/{0}",
                //WebServiceUrl = "https://www.AsanPardakht.com/pg/services/WebGate/service",
                IsToman = false
            };
            this._settingService.SaveSetting<AsanPardakhtSettings>(SepPaymentSettings, 0);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.Fields.RedirectionTip", " به درگاه آسان پرداخت متصل خواهید شد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.MerchantId", "کد درگاه پرداخت", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.MerchantId.Hint", "كدي يكتا است كه آسان پرداخت به ازاي هر درخواست درگاه پرداخت به پذيرنده اختصاص می‌دهد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.ConfigMerchentId", "کد پیکربندی پذیرنده", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.UserNameMerchent", "نام کاربری", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.PassMerchent", "رمز عبور", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.Key", "کلید", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.VectorEncriptor", "وکتور رمز نگاری", null);

            //LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.Fields.PayementUrl", "آدرس درگاه پرداخت", null);
            //LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.Fields.PayementUrl.Hint", "آدرس اینترنتی‌ای که مشتری برای پرداخت مبلغ فاکتور به آن هدایت می‌شود.", null);
            //LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.Fields.WebServiceUrl", "آدرس وب سرویس", null);
            //LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.Fields.WebServiceUrl.Hint", "آدرس وب سرویس آسان پرداخت برای ازتباط با درگاه پرداخت.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.IsToman", "محاسبه بر اساس تومان", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.IsToman.Hint", "اگر واحد پول شما بر اساس تومان است این تیک را بزنید و اگر بر اساس ریال است این تیک را بردارید.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.DisablePaymentInfo", "عدم نمایش صفحه اطلاعات روش پرداخت", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.DisablePaymentInfo.Hint", "با انتخاب این گزینه صفحه مربوط به اطلاعات روش پرداخت به کاربر نمایش داده نمی شود.(به درگاه آسان پرداخت متصل خواهید شد)", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.PaymentMethodDescription", "پرداخت توسط درگاه آسان پرداخت", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.Verify.SuccessMessage", "صورتحساب با موفقیت پرداخت گردید.\r\nکد پیگیری : {0}", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.Verify.FailureMessage", "پردخت  ناموفق بود.\r\nشرح خطا : {0}", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.Payment.Description", "بابت خرید فاکتور شماه {0}", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.Payment.FailureMessage", "برقراری ارتباط با سرور آسان پرداخت امکان پذیر نمی‌باشد.\r\nشرح خطا : {0}", null);

            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-1", "اطلاعات ارسالی ناقص است.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-2", "آی‌پی و یا مرچنت کد،  صحیح نیست.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-3", "با توجه به محدودیت‌های شاپرک امکان پرداخت با رقم درخواست شده میسر نمی‌باشد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-4", "سطح تایید پذیرنده پایین‌تر از سطح نقره‌ای است.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-11", "درخواست مورد نظر یافت نشد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-12", "امکان ویرایش درخواست میسر نمی‌باشد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-21", "هیچ نوع عملیات مالی برای این تراکنش یافت نشد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-22", "تراکنش ناموفق می‌باشد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-33", "رقم تراکنش با رقم پرداخت شده مطابقت ندارد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-34", "سقف تقسیم تراکنش از لحاظ تعداد یا رقم، عبور نموده است.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-40", "اجازه دسترسی به متد مربوطه وجود ندارد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-41", "اطلاعات ارسال شده مربوط به AdditionalData غیرمعتبر می‌باشد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-42", "مدت زمان معتبر طول عمر شناسه پرداخت باید بین  30 دقیه تا  45 روز می‌باشد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-54", "درخواست مورد نظر آرشیو شده است.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.100", "عملیات با موفقیت انجام گردیده است.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.101", "عملیات پرداخت موفق بوده و قبلاً PaymentVerification تراکنش انجام شده است.", null);
            base.Install();
        }


        public override void Uninstall()
        {
            this._settingService.DeleteSetting<AsanPardakhtSettings>();
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.Fields.RedirectionTip");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.Fields.MerchantId");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.Fields.MerchantId.Hint");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.Fields.PayementUrl");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.Fields.PayementUrl.Hint");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.PaymentMethodDescription");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.Payment.Description");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.Verify.SuccessMessage");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.Payment.FailureMessage");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-1");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-2");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-3");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-4");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-11");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-12");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-21");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-22");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-33");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-34");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-40");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-41");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-42");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.-54");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.100");
            LocalizationExtensions.DeletePluginLocaleResource(this, "NopFarsi.AsanPardakht.StatusCode.101");
            base.Uninstall();
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
                return (PaymentMethodType)15;
            }
        }


        public bool SkipPaymentInfo
        {
            get
            {
                if (_SepPaymentSettings.DisablePaymentInfo)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public string PaymentMethodDescription
        {
            get
            {
                return this._localizationService.GetResource("NopFarsi.AsanPardakht.PaymentMethodDescription");
            }
        }



    }
}

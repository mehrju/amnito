using System;
using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Logging;
using NopFarsi.Payments.SepShaparak.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Payments;
using NopFarsi.Payments.SepShaparak.Service;
using NopFarsi.Payments.SepShaparak.ir.sep.srtm;
using NopFarsi.Payments.SepShaparak.Data;
using NopFarsi.Payments.SepShaparak.Domain;
using Nop.Core.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NopFarsi.Payments.SepShaparak.Models;
using RestSharp;
using Newtonsoft.Json;
using System.Net;

namespace NopFarsi.Payments.SepShaparak
{

    public class SamanPaymentProcessor : BasePlugin, IPaymentMethod, IPlugin
    {
        private const string GenericAttributeAuthority = "SepAuthority";
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly sepsettings _SepPaymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderRefundStatusService _orderRefundStatusService;
        private readonly OrderRefundStatusObjectContext _objectContext;
        private readonly IWorkContext _workContext;
        private ISession _session;

        public SamanPaymentProcessor(
            IHttpContextAccessor httpContextAccessor,
            sepsettings SepPaymentSettings,
            ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper,
            IGenericAttributeService genericAttributeService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            ILogger logger,
            IOrderRefundStatusService orderRefundStatusService,
            OrderRefundStatusObjectContext objectContext,
            IWorkContext workContext
            )
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
            this._orderRefundStatusService = orderRefundStatusService;
            this._objectContext = objectContext;
            this._session = _httpContextAccessor.HttpContext.Session;
            this._workContext = workContext;
            //if (!webHelper.GetStoreLocation().Contains("falahshop"))
            {
                ///throw new Exception();
            }
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Pending };
            return result;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            decimal orderTotalDecimal = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);
            long orderId = postProcessPaymentRequest.Order.Id;
            string MID = _SepPaymentSettings.MerchantId.ToString();
            long orderTotal = Convert.ToInt64(orderTotalDecimal);
            long Amount = orderTotal;

            if (_SepPaymentSettings.IsToman)
            {
                orderTotal *= 10L;
            }
            bool isFromApp = SessionExtensions.Get<bool>(_session, "isFromApp");
            string callBackUrl = _webHelper.GetStoreLocation(true) + "Sep/Verify?isFromApp=" + isFromApp;
            var tokenResult = SendViaToken(new SepTokenModel
                                                { 
                                                    TerminalId = MID,
                                                    Amount = Amount,
                                                    RedirectUrl = callBackUrl,
                                                    ResNum = orderId.ToString(),
                                                    CellNumber = long.Parse(_workContext.CurrentCustomer.Username)
                                                }
            );
            _logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Debug,"نتیجه توکن بانک سامان",Newtonsoft.Json.JsonConvert.SerializeObject(tokenResult));
            if(tokenResult== null || tokenResult.status == -1)
            {
                throw new Exception("اعتبار سنجی برای ورود به درگاه بانک سامان انجام نشد");
            }

            string redirectUrl = _webHelper.GetStoreLocation(true) +
                "Plugins/SamanPostbar/Pay?ResNum=" + orderId.ToString() + "&MID=" + MID +
                "&Amount=" + Amount.ToString() +"&="+tokenResult.token+ "&RedirectURL=" + callBackUrl;

            
            _logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Information, "آدرس ارسال به درگاه سامان", redirectUrl);
            if (isFromApp)
            {
                SessionExtensions.Set<string>(_session, "redirectUrl", redirectUrl);
                //_logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Information, "آدرس ارسال به درگاه سامان", redirectUrl);
                return;
            }

            _httpContextAccessor.HttpContext.Response.Redirect(redirectUrl);
        }

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
            var resNum = refundPaymentRequest.Order.Id.ToString();
            var refNum = refundPaymentRequest.Order.AuthorizationTransactionId;
            var requestId = new Random().Next();

            srvRefund srvRefund = new srvRefund();

            LoggingExtensions.Information(_logger, $"Refund_Reg with Params userName:{_SepPaymentSettings.RefundUserName}, password:{_SepPaymentSettings.RefundPassword}," +
                $" refNum:{refNum}, resNum:{resNum}, transactionTermId:{_SepPaymentSettings.TransactionTermId}, refundTermId:{_SepPaymentSettings.RefundTermId}, " +
                $" amount:{refundPaymentRequest.Order.OrderTotal}, requestId:{requestId}, exeTime: 1, email:{_SepPaymentSettings.RefundEmail}," +
                $" cellNumber:{_SepPaymentSettings.RefundCellPhone} Time:{DateTime.Now}", null, null);

            var refundRegResult = srvRefund.Refund_Reg(
                 userName: _SepPaymentSettings.RefundUserName,
                 password: _SepPaymentSettings.RefundPassword,
                 refNum: refNum,
                 resNum: resNum,
                 transactionTermIdSpecified: true,
                 refundTermIdSpecified: true,
                 transactionTermId: _SepPaymentSettings.TransactionTermId,
                 refundTermId: _SepPaymentSettings.RefundTermId,
                 amountSpecified: true,
                 exeTimeSpecified: true,
                 amount: (long)refundPaymentRequest.Order.OrderTotal,
                 requestId: requestId,
                 requestIdSpecified: true,
                 exeTime: 1,
                 email: _SepPaymentSettings.RefundEmail,
                 cellNumber: _SepPaymentSettings.RefundCellPhone);

            LoggingExtensions.Information(_logger, $"Refund_Reg Result ActionName:{refundRegResult.ActionName} Description: {refundRegResult.Description} " +
                $"ErrorCode:{refundRegResult.ErrorCode} ErrorMessage:{refundRegResult.ErrorMessage} RequestStatus:{(RefundStatus)refundRegResult.RequestStatus} " +
                $"ReferenceId:{refundRegResult.ReferenceId}", null, null);

            RefundPaymentResult refund = new RefundPaymentResult();
            if (refundRegResult.ErrorCode != ErrCode.Success)
            {
                refund.AddError($"Refund_Reg -> ErrorMessage:{refundRegResult.ErrorMessage} Description: {refundRegResult.Description}");
            }
            else
            {
                //var resultRefundExec = srvRefund.Refund_Exec(
                //    userName: _SepPaymentSettings.RefundUserName,
                //    password: _SepPaymentSettings.RefundPassword,
                //    partialRefundId: refundRegResult.ReferenceId,
                //    partialRefundIdSpecified: true,
                //    typRefundAction: typeRefundAction.Approve,
                //    typRefundActionSpecified: true,
                //    termId: _SepPaymentSettings.TransactionTermId,
                //    termIdSpecified: true);
                //LoggingExtensions.Information(_logger, $"Refund_Exec -> ActionName:{resultRefundExec.ActionName} Description: {resultRefundExec.Description} ErrorCode:{resultRefundExec.ErrorCode} ErrorMessage:{resultRefundExec.ErrorMessage} RequestStatus:{(RefundStatus)resultRefundExec.RequestStatus}", null, null);

                //if (resultRefundExec.ErrorCode != ErrCode.Success)
                //{
                //    refund.AddError($"Refund_Exec -> ErrorMessage:{resultRefundExec.ErrorMessage} Description: {resultRefundExec.Description}");
                //}
                //else
                //{
                _orderRefundStatusService.InsertOrderRefundStatus(new Domain.OrderRefundStatus
                    {
                        OrderId = refundPaymentRequest.Order.Id,
                        RefundRefrenceId = refundRegResult.ReferenceId,
                        RefundStatus = Domain.RefundStatus.Start,
                    });
                //}
            }
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
            return _webHelper.GetStoreLocation() + "Admin/SamanPostbar/Configure";
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "SamanPostbar";
            routeValues = new RouteValueDictionary
			{
				{
					"Namespaces",
                    "NopFarsi.Payments.SepShaparak.Controllers"
                },
				{
					"area",
					null
				}
			};
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "SamanPaymentInfo";
        }

        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        public Type GetControllerType()
        {
            return typeof(SamanPostbarController);
        }

        public override void Install()
        {
            _objectContext.Install();

            sepsettings SepPaymentSettings = new sepsettings
            {
                MerchantId = 0,
                IsToman = false
            };
            this._settingService.SaveSetting<sepsettings>(SepPaymentSettings, 0);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.Fields.RedirectionTip", " به درگاه سامان متصل خواهید شد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.MerchantId", "کد درگاه پرداخت", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.MerchantId.Hint", "كدي يكتا است كه سامان به ازاي هر درخواست درگاه پرداخت به پذيرنده اختصاص می‌دهد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.ConfigMerchentId", "کد پیکربندی پذیرنده", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.UserNameMerchent", "نام کاربری", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.PassMerchent", "رمز عبور", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.IsToman", "محاسبه بر اساس تومان", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.IsToman.Hint", "اگر واحد پول شما بر اساس تومان است این تیک را بزنید و اگر بر اساس ریال است این تیک را بردارید.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.DisablePaymentInfo", "عدم نمایش صفحه اطلاعات روش پرداخت", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.DisablePaymentInfo.Hint", "با انتخاب این گزینه صفحه مربوط به اطلاعات روش پرداخت به کاربر نمایش داده نمی شود.(به درگاه سامان متصل خواهید شد)", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.PaymentMethodDescription", "پرداخت توسط درگاه سامان", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.Verify.SuccessMessage", "صورتحساب با موفقیت پرداخت گردید.\r\nکد پیگیری : {0}", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.Verify.FailureMessage", "پردخت  ناموفق بود.\r\nشرح خطا : {0}", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.Payment.Description", "بابت خرید فاکتور شماه {0}", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.Payment.FailureMessage", "برقراری ارتباط با سرور سامان امکان پذیر نمی‌باشد.\r\nشرح خطا : {0}", null);

            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.-1", "اطلاعات ارسالی ناقص است.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.-2", "آی‌پی و یا مرچنت کد،  صحیح نیست.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.-3", "با توجه به محدودیت‌های شاپرک امکان پرداخت با رقم درخواست شده میسر نمی‌باشد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.-4", "سطح تایید پذیرنده پایین‌تر از سطح نقره‌ای است.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.-11", "درخواست مورد نظر یافت نشد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.-12", "امکان ویرایش درخواست میسر نمی‌باشد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.-21", "هیچ نوع عملیات مالی برای این تراکنش یافت نشد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.-22", "تراکنش ناموفق می‌باشد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.-33", "رقم تراکنش با رقم پرداخت شده مطابقت ندارد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.-34", "سقف تقسیم تراکنش از لحاظ تعداد یا رقم، عبور نموده است.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.-40", "اجازه دسترسی به متد مربوطه وجود ندارد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.-41", "اطلاعات ارسال شده مربوط به AdditionalData غیرمعتبر می‌باشد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.-42", "مدت زمان معتبر طول عمر شناسه پرداخت باید بین  30 دقیه تا  45 روز می‌باشد.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.-54", "درخواست مورد نظر آرشیو شده است.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.100", "عملیات با موفقیت انجام گردیده است.", null);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Postbar.Saman.StatusCode.101", "عملیات پرداخت موفق بوده و قبلاً PaymentVerification تراکنش انجام شده است.", null);
            base.Install();
        }

        public override void Uninstall()
        {
            this._settingService.DeleteSetting<sepsettings>();
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.Fields.RedirectionTip");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.Fields.MerchantId");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.Fields.MerchantId.Hint");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.Fields.PayementUrl");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.Fields.PayementUrl.Hint");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.PaymentMethodDescription");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.Payment.Description");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.Verify.SuccessMessage");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.Payment.FailureMessage");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.-1");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.-2");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.-3");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.-4");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.-11");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.-12");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.-21");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.-22");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.-33");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.-34");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.-40");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.-41");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.-42");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.-54");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.100");
            LocalizationExtensions.DeletePluginLocaleResource(this, "Postbar.Saman.StatusCode.101");
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
                return true;
            }
        }

        public bool SupportRefund
        {
            get
            {
                return true;
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
                return this._localizationService.GetResource("Postbar.Saman.PaymentMethodDescription");
            }
        }

        public SepTokenResultModel SendViaToken(SepTokenModel token)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string urlAddress = "https://sep.shaparak.ir/OnlinePG/OnlinePG";
            var restClient = new RestClient(urlAddress);
            var request = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json,
                OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
            };
            //to request a token you should set Actio property as token.
            token.Action = "token";
            request.AddBody(token);
            var sepResult = restClient.Execute(request);
            //var jsonSerializer = new JavaScriptSerializer();
            return JsonConvert.DeserializeObject<SepTokenResultModel>(sepResult.Content);
        }
    }
}
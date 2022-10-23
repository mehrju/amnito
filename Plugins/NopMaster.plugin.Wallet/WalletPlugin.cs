using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Nop.Core.Domain.Payments;

namespace NopMaster.Plugin.Payments.Wallet
{
	public class WalletPlugin : BasePlugin, IConsumer<OrderPaidEvent>, IPlugin, IPaymentMethod
    {
		public IStoreContext _storeContext;

		public ICustomerService _customerService;

		public IRewardPointService _rewardPointService;

		public ISettingService _settingService;

		private IWebHelper iwebHelper_0;

		private ILocalizationService _localizationService;

		private IOrderTotalCalculationService iorderTotalCalculationService_0;

        public bool SkipPaymentInfo => false;
        public string PaymentMethodDescription => _localizationService.GetResource("NopMaster.Wallet.PaymentMethodDescription");

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;
        public PaymentMethodType PaymentMethodType => Nop.Services.Payments.PaymentMethodType.Standard;

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public WalletPlugin(IStoreContext storeContext
            , ICustomerService customerService
            , IRewardPointService rewardPointService
            , ISettingService settingService
            , IWebHelper webHelper
            , IOrderTotalCalculationService orderTotalCalculationService
            , ILocalizationService localizationService)
		{
            
                this._storeContext = storeContext;
                this._customerService = customerService;
                this._rewardPointService = rewardPointService;
                this._settingService = settingService;
                this.iwebHelper_0 = webHelper;
                this.iorderTotalCalculationService_0 = orderTotalCalculationService;
                this._localizationService = localizationService;
        }

		public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
		{
            CancelRecurringPaymentResult cancelRecurringPaymentResult = new CancelRecurringPaymentResult();
            cancelRecurringPaymentResult.AddError("Recurring payment not supported.");
            return cancelRecurringPaymentResult;
        }

		public bool CanRePostProcessPayment(Order order)
		{
            if (order == null)
            {
                throw new ArgumentNullException("order");
            }
            TimeSpan utcNow = DateTime.UtcNow - order.CreatedOnUtc;
            return utcNow.TotalSeconds >= 1;
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return 0;
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
		{
            CapturePaymentResult capturePaymentResult = new CapturePaymentResult();
            capturePaymentResult.AddError("Capture method not supported.");
            return capturePaymentResult;
        }

		public override string GetConfigurationPageUrl()
		{
           
            return string.Concat(this.iwebHelper_0.GetStoreLocation(), "Admin/WalletAdmin/Configure");
        }

		public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
		{
			return new ProcessPaymentRequest();
		}

		public void GetPublicViewComponent(out string viewComponentName)
		{
            viewComponentName = "PaymentInfo";
        }

		public void HandleEvent(OrderPaidEvent eventMessage)
		{
            try
            {
                Order order = eventMessage.Order;
                if (order.PaymentStatus == PaymentStatus.Paid)
                {
                    Setting setting = this._settingService.GetSetting("NopMaster.Wallet_ProductId", this._storeContext.CurrentStore.Id, false);
                    List<OrderItem> list = (
                        from x in order.OrderItems
                        where x.ProductId == int.Parse(setting.Value)
                        select x).ToList();
                    if (list.Any())
                    {
                        Customer customerById = this._customerService.GetCustomerById(order.CustomerId);
                        if (customerById != null)
                        {
                            RewardPointsSettings rewardPointsSetting = this._settingService.LoadSetting<RewardPointsSettings>(this._storeContext.CurrentStore.Id);
                            foreach (OrderItem orderItem in list)
                            {
                                //TODO : Reward point
                                int priceInclTax = (int)((orderItem.PriceInclTax / rewardPointsSetting.ExchangeRate) * orderItem.Quantity);
                                this._rewardPointService.AddRewardPointsHistoryEntry(customerById, priceInclTax, this._storeContext.CurrentStore.Id, string.Concat("افزایش مبلغ کیف پول شماره سفارش:", order.Id));
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                LoggingExtensions.Error(EngineContext.Current.Resolve<ILogger>(), "خطا در بروزرسانی کیف پول", exception, null);
            }
        }

		public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
		{
			return true;
		}

		public override void Install()
		{
            LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "NopMaster.Wallet.PaymentMethodDescription", "پرداخت با کیف پول", null);
            base.Install();
		}

		public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
		{
			throw new NotImplementedException();
		}

		public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
		{
            ProcessPaymentResult processPaymentResult = new ProcessPaymentResult();
            processPaymentResult.NewPaymentStatus = PaymentStatus.Pending;
            return processPaymentResult;
        }

		public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
		{
            ProcessPaymentResult processPaymentResult = new ProcessPaymentResult();
            processPaymentResult.AddError("Recurring payment not supported.");
            return processPaymentResult;
        }

		public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
		{
            RefundPaymentResult refundPaymentResult = new RefundPaymentResult();
            refundPaymentResult.AddError("Refund method not supported.");
            return refundPaymentResult;
        }

		public IList<string> ValidatePaymentForm(IFormCollection form)
		{
            if (form == null)
                throw new ArgumentException(nameof(form));

            //try to get errors
            if (form.TryGetValue("Errors", out StringValues errorsString) && !StringValues.IsNullOrEmpty(errorsString))
                return new[] { errorsString.ToString() }.ToList();

            return new List<string>();
        }

		public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
		{
            VoidPaymentResult voidPaymentResult = new VoidPaymentResult();
            voidPaymentResult.AddError("Void method not supported.");
            return voidPaymentResult;
        }
	}
}
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Factories;
using Nop.Web.Models.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Nop.Core.Domain.Configuration;
using Nop.Services.Configuration;

namespace NopMaster.Plugin.Payments.Wallet
{
    public class CheckoutModelFactoryNf : CheckoutModelFactory
    {
        
        private readonly IWorkContext _workContext;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPaymentService _paymentService;
        private readonly PaymentSettings _paymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly ITaxService _taxService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRewardPointService _rewardPointService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        public CheckoutModelFactoryNf(IAddressModelFactory addressModelFactory
            , IWorkContext workContext
            , IStoreContext storeContext
            , IStoreMappingService storeMappingService
            , ILocalizationService localizationService
            , ITaxService taxService
            , ICurrencyService currencyService
            , IPriceFormatter priceFormatter
            , IOrderProcessingService orderProcessingService
            , IProductAttributeParser productAttributeParser
            , IProductService productService
            , IGenericAttributeService genericAttributeService
            , ICountryService countryService
            , IStateProvinceService stateProvinceService
            , IShippingService shippingService
            , IPaymentService paymentService
            , IOrderTotalCalculationService orderTotalCalculationService
            , IRewardPointService rewardPointService
            , IWebHelper webHelper, CommonSettings commonSettings
            , OrderSettings orderSettings
            , RewardPointsSettings rewardPointsSettings
            , PaymentSettings paymentSettings
            , ShippingSettings shippingSettings
            , AddressSettings addressSettings, ISettingService settingService) : base(addressModelFactory, workContext, storeContext, storeMappingService, localizationService, taxService, currencyService, priceFormatter, orderProcessingService, productAttributeParser, productService, genericAttributeService, countryService, stateProvinceService, shippingService, paymentService, orderTotalCalculationService, rewardPointService, webHelper, commonSettings, orderSettings, rewardPointsSettings, paymentSettings, shippingSettings, addressSettings)
        {
            _settingService = settingService;
            this._rewardPointService = rewardPointService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._paymentService = paymentService;
            this._paymentSettings = paymentSettings;
            this._localizationService = localizationService;
            this._taxService = taxService;
            this._webHelper = webHelper;
            this._genericAttributeService = genericAttributeService;
            this._rewardPointsSettings = rewardPointsSettings;
        }

        public override CheckoutPaymentMethodModel PreparePaymentMethodModel(IList<ShoppingCartItem> cart, int filterByCountryId)
        {
            var model = new CheckoutPaymentMethodModel();
            if (this._rewardPointsSettings.Enabled && !cart.IsRecurring())
            {
                int rewardPointsBalance = this._rewardPointService.GetRewardPointsBalance(this._workContext.CurrentCustomer.Id, this._storeContext.CurrentStore.Id);
                decimal amount = this._orderTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
                decimal num = this._currencyService.ConvertFromPrimaryStoreCurrency(amount, this._workContext.WorkingCurrency);
                if (num > new decimal(0) && this._orderTotalCalculationService.CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                {
                    model.DisplayRewardPoints = true;
                    model.RewardPointsAmount = this._priceFormatter.FormatPrice(num, true, false);
                    model.RewardPointsBalance = rewardPointsBalance;
                    model.RewardPointsEnoughToPayForOrder = !this._orderProcessingService.IsPaymentWorkflowRequired(cart, new bool?(true));
                }
            }
            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id, filterByCountryId)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();
            foreach (var pm in paymentMethods)
            {
                if (cart.IsRecurring() && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                var pmModel = new CheckoutPaymentMethodModel.PaymentMethodModel
                {
                    Name = pm.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id),
                    Description = _paymentSettings.ShowPaymentMethodDescriptions ? pm.PaymentMethodDescription : string.Empty,
                    PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
                    LogoUrl = pm.PluginDescriptor.GetLogoUrl(_webHelper)
                };
                var paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, pm.PluginDescriptor.SystemName);
                var rateBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                var rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                if (rate > decimal.Zero)
                    pmModel.Fee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate, true);

                model.PaymentMethods.Add(pmModel);
            }
            var selectedPaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                SystemCustomerAttributeNames.SelectedPaymentMethod,
                _genericAttributeService, _storeContext.CurrentStore.Id);
            if (!string.IsNullOrEmpty(selectedPaymentMethodSystemName))
            {
                var paymentMethodToSelect = model.PaymentMethods.ToList()
                    .Find(pm => pm.PaymentMethodSystemName.Equals(selectedPaymentMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }
            if (model.PaymentMethods.FirstOrDefault(so => so.Selected) == null)
            {
                var paymentMethodToSelect = model.PaymentMethods.FirstOrDefault();
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }

            if (_workContext.CurrentCustomer.ShoppingCartItems.Any(p => p.Product.Name.Contains("پس کرایه")))
            {
                model.DisplayRewardPoints = false;
            }
            Setting setting = this._settingService.GetSetting("NopMaster.Wallet_ProductId", this._storeContext.CurrentStore.Id, false);
            if (_workContext.CurrentCustomer.ShoppingCartItems.Any(p => p.Product.Id == int.Parse(setting.Value)))
            {
                model.DisplayRewardPoints = false;
            }
            if (model.PaymentMethods.Count == 1 && model.PaymentMethods.FirstOrDefault()?.Name == "Payments.CashOnDelivery")
            {
                model.DisplayRewardPoints = false;
            }
            return model;
        }
    }
}
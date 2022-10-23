using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoxNetSoft.Plugin.Payments.PaymentRules.Domain;
using FoxNetSoft.Plugin.Payments.PaymentRules.Logger;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Services
{
    public class AdvancedPaymentService : PaymentService
    {
        private readonly IAclService _aclService;
        private readonly IOrderService _orderService;
        private readonly IPaymentRulesService _paymentRulesService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly FNSLogger _fnsLogger;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly PaymentRulesSettings _paymentRulesSettings;
        private readonly PaymentSettings _paymentSettings0;
        private readonly bool _showDebugInfo;

        public AdvancedPaymentService(PaymentSettings paymentSettings
            , IAclService aclService
            , IPluginFinder pluginFinder
            , ISettingService settingService
            , ShoppingCartSettings shoppingCartSettings
            , IStaticCacheManager cacheManager
            , PaymentRulesSettings paymentRulesSettings
            , IPaymentRulesService paymentRulesService
            , IStoreMappingService storeMappingService
            , IOrderService orderService) : base(paymentSettings, pluginFinder, settingService, shoppingCartSettings)
        {
            _paymentRulesService = paymentRulesService;
            _paymentSettings0 = paymentSettings;
            _staticCacheManager = cacheManager;
            _paymentRulesSettings = paymentRulesSettings;
            _showDebugInfo = _paymentRulesSettings.showDebugInfo;
            _storeMappingService = storeMappingService;
            _aclService = aclService;
            _fnsLogger = new FNSLogger(_showDebugInfo);
            _orderService = orderService;
        }

        protected bool CompareRequirementValue(string value, string requirementValue, string requirementOperator)
        {
            if (_showDebugInfo)
                LogMsg(string.Format(
                    "CompareRequirementValue. value={0}, requirementValue={1}, requirementOperator={2}", value,
                    requirementValue, requirementOperator));
            value = value ?? "";
            requirementValue = requirementValue ?? "";
            if (string.IsNullOrWhiteSpace(requirementOperator)) return false;
            if (requirementOperator == "==")
                return value.Equals(requirementValue, StringComparison.InvariantCultureIgnoreCase);
            if (requirementOperator == "!=")
                return !value.Equals(requirementValue, StringComparison.InvariantCultureIgnoreCase);
            if (requirementOperator != "contain") return false;
            return value.ToLower().Contains(requirementValue.ToLower());
        }

        protected bool CompareRequirementValue(decimal value, string requirementValue, string requirementOperator)
        {
            if (_showDebugInfo)
                LogMsg(string.Format(
                    "CompareRequirementValue. value={0}, requirementValue={1}, requirementOperator={2}", value,
                    requirementValue, requirementOperator));
            requirementValue = requirementValue ?? "";
            if (string.IsNullOrWhiteSpace(requirementOperator)) return false;
            var num = new decimal();
            if (!decimal.TryParse(requirementValue, out num)) return false;
            if (requirementOperator == "==") return value == num;
            if (requirementOperator == "!=") return value != num;
            if (requirementOperator == ">") return value > num;
            if (requirementOperator == ">=") return value >= num;
            if (requirementOperator == "<") return value < num;
            if (requirementOperator != "<=") return false;
            return value <= num;
        }

        protected bool CompareRequirementValue(int value, string requirementValue, string requirementOperator)
        {
            if (_showDebugInfo)
                LogMsg(string.Format(
                    "CompareRequirementValue. value={0}, requirementValue={1}, requirementOperator={2}", value,
                    requirementValue, requirementOperator));
            requirementValue = requirementValue ?? "";
            if (string.IsNullOrWhiteSpace(requirementOperator)) return false;
            var num = 0;
            if (!int.TryParse(requirementValue, out num)) return false;
            if (requirementOperator == "==") return value == num;
            if (requirementOperator == "!=") return value != num;
            if (requirementOperator == ">") return value > num;
            if (requirementOperator == ">=") return value >= num;
            if (requirementOperator == "<") return value < num;
            if (requirementOperator != "<=") return false;
            return value <= num;
        }

        public override decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart, string paymentMethodSystemName)
        {
            var additionalHandlingFee = base.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
            var enabled = _paymentRulesSettings.Enabled;
            return additionalHandlingFee;
        }

        protected IList<RequirementForCaching> GetReqirementsForCaching(
            IEnumerable<PaymentRuleRequirement> requirements)
        {
            return (
                from paymentRuleRequirement_0 in requirements
                select new RequirementForCaching
                {
                    Id = paymentRuleRequirement_0.Id,
                    ExternalId = paymentRuleRequirement_0.PaymentRuleId,
                    IsGroup = paymentRuleRequirement_0.IsGroup,
                    InteractionType = paymentRuleRequirement_0.InteractionType,
                    ChildRequirements = GetReqirementsForCaching(paymentRuleRequirement_0.ChildRequirements),
                    RequirementCategory = paymentRuleRequirement_0.RequirementCategory,
                    RequirementProperty = paymentRuleRequirement_0.RequirementProperty,
                    RequirementOperator = paymentRuleRequirement_0.RequirementOperator,
                    RequirementValue = paymentRuleRequirement_0.RequirementValue
                }).ToList();
        }

        protected bool GetValidationResult(IEnumerable<RequirementForCaching> requirements
            , GroupInteractionType groupInteractionType
            , Customer customer
            , int storeId)
        {
            var isValid = false;
            var stringBuilder = new StringBuilder();
            var str1 = "";
            foreach (var requirement in requirements)
            {
                if (_showDebugInfo)
                    stringBuilder.AppendLine(string.Format(
                        "GetValidationResult. ExternalId={0}, requirement.Id={1}, IsGroup={2}, RequirementCategory='{3}', RequirementProperty='{4}', RequirementOperator='{5}', RequirementValue='{6}'",
                        (object) requirement.ExternalId, (object) requirement.Id, (object) requirement.IsGroup,
                        (object) requirement.RequirementCategory, (object) requirement.RequirementProperty,
                        (object) requirement.RequirementOperator, (object) requirement.RequirementValue));
                if (requirement.IsGroup)
                {
                    var groupInteractionType1 = requirement.InteractionType.GetValueOrDefault(0);
                    isValid = GetValidationResult(requirement.ChildRequirements, groupInteractionType1, customer,
                        storeId);
                }
                else
                {
                    RequirementCategory requirementCategory;
                    try
                    {
                        requirementCategory = (RequirementCategory) Enum.Parse(typeof(RequirementCategory),
                            requirement.RequirementCategory);
                    }
                    catch (Exception ex)
                    {
                        if (_showDebugInfo)
                            LogMsg(string.Format(
                                "GetValidationResult. ExternalId={0}, requirement.Id={1} (wrong RequirementCategory={2}) Error={3}.",
                                (object) requirement.ExternalId, (object) requirement.Id,
                                (object) requirement.RequirementCategory, (object) ex.Message));
                        requirementCategory = RequirementCategory.DefaultRequirementGroup;
                    }

                    if (requirementCategory == RequirementCategory.BillingAddress ||
                        requirementCategory == RequirementCategory.ShippingAddress)
                    {
                        var address = customer.ShippingAddress;
                        if (requirementCategory == RequirementCategory.BillingAddress)
                            address = customer.BillingAddress;
                        var result = ShippingAddressOperators.CountryIsoCode;
                        if (address == null && Enum.TryParse(requirement.RequirementProperty, out result))
                        {
                            var str2 = (string) null;
                            switch (result)
                            {
                                case ShippingAddressOperators.ZipCode:
                                    str2 = address.ZipPostalCode;
                                    break;
                                case ShippingAddressOperators.CountryIsoCode:
                                    if (address.Country != null) str2 = address.Country.TwoLetterIsoCode;
                                    break;
                                case ShippingAddressOperators.CountryName:
                                    if (address.Country != null) str2 = address.Country.Name;
                                    break;
                                case ShippingAddressOperators.City:
                                    str2 = address.City;
                                    break;
                                case ShippingAddressOperators.ProvinceName:
                                    if (address.StateProvince != null) str2 = address.StateProvince.Name;
                                    break;
                                case ShippingAddressOperators.ProvinceAbbreviation:
                                    if (address.StateProvince != null) str2 = address.StateProvince.Abbreviation;
                                    break;
                                case ShippingAddressOperators.Country:
                                    str2 = address.CountryId.ToString();
                                    break;
                                case ShippingAddressOperators.Province:
                                    str2 = address.StateProvinceId.ToString();
                                    break;
                            }

                            if (str2 != null)
                                isValid = CompareRequirementValue(str2, requirement.RequirementValue,
                                    requirement.RequirementOperator);
                            str1 = str2;
                        }
                    }

                    if (requirementCategory == RequirementCategory.OrderTotals)
                    {
                        var result = OrderTotalOperators.OrderTotal;
                        if (Enum.TryParse(requirement.RequirementProperty, out result))
                        {
                            var nullable = new decimal?();
                            switch (result)
                            {
                                case OrderTotalOperators.OrderTotal:
                                    var list1 = customer.ShoppingCartItems
                                        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                                        .LimitPerStore(storeId).ToList();
                                    if (list1.Any())
                                    {
                                        var shoppingCartTotal =
                                            EngineContext.Current.Resolve<IOrderTotalCalculationService>().GetShoppingCartTotal(list1, false, false);
                                        if (shoppingCartTotal.HasValue) nullable = shoppingCartTotal.Value;
                                    }

                                    break;
                                case OrderTotalOperators.OrderSubTotal:
                                    var ShoppingCartItems = customer.ShoppingCartItems
                                        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                                        .LimitPerStore(storeId).ToList();
                                    if (ShoppingCartItems.Any())
                                    {
                                        decimal discountAmount;
                                        List<DiscountForCaching> discountForCachingList;
                                        decimal subTotalWithoutDiscount;
                                        decimal subTotalWithDiscount;
                                        EngineContext.Current.Resolve<IOrderTotalCalculationService>().GetShoppingCartSubTotal(ShoppingCartItems, true,
                                            out discountAmount, out discountForCachingList, out subTotalWithoutDiscount,
                                            out subTotalWithDiscount);
                                        nullable = subTotalWithDiscount;
                                    }

                                    break;
                                case OrderTotalOperators.AllOrderTotal:
                                    nullable = _orderService.SearchOrders(storeId, 0, customer.Id,
                                            osIds: new List<int> {30}, pageIndex: 0, pageSize: int.MaxValue)
                                        .Sum(o => o.OrderTotal);
                                    break;
                                case OrderTotalOperators.NumberOfOrders:
                                    _orderService.SearchOrders(storeId, 0, customer.Id, osIds: new List<int> {30},
                                        pageIndex: 0, pageSize: int.MaxValue).Count();
                                    break;
                            }

                            if (nullable.HasValue)
                                isValid = CompareRequirementValue(nullable.Value, requirement.RequirementValue,
                                    requirement.RequirementOperator);
                            str1 = string.Format("{0}", nullable);
                        }
                    }

                    if (requirementCategory == RequirementCategory.CheckoutAttributes)
                    {
                        var attribute =
                            customer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, storeId);
                        if (!string.IsNullOrEmpty(attribute) &&
                            !string.IsNullOrWhiteSpace(requirement.RequirementProperty))
                        {
                            var icheckoutAttributeParser = EngineContext.Current.Resolve<ICheckoutAttributeParser>();
                            var attributeService = EngineContext.Current.Resolve<ICheckoutAttributeService>();
                            var attrId = 0;
                            if (int.TryParse(requirement.RequirementProperty, out attrId))
                            {
                                var checkoutAttribute = icheckoutAttributeParser.ParseCheckoutAttributes(attribute)
                                    .FirstOrDefault(c => c.Id == attrId);
                                if (checkoutAttribute != null)
                                {
                                    var values = icheckoutAttributeParser.ParseValues(attribute, checkoutAttribute.Id);
                                    for (var index = 0; index < values.Count; ++index)
                                    {
                                        var s = values[index];
                                        str1 = s ?? "";
                                        if (!checkoutAttribute.ShouldHaveValues())
                                        {
                                            if (isValid = CompareRequirementValue(s, requirement.RequirementValue,
                                                requirement.RequirementOperator))
                                                break;
                                        }
                                        else
                                        {
                                            int result;
                                            if (int.TryParse(s, out result))
                                            {
                                                var attributeValueById =
                                                    attributeService.GetCheckoutAttributeValueById(result);
                                                if (attributeValueById != null && (isValid = CompareRequirementValue(
                                                        attributeValueById.Name
                                                        , requirement.RequirementValue,
                                                        requirement.RequirementOperator)))
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (requirementCategory == RequirementCategory.SpecificationAttributes)
                    {
                        var ShoppingCartItems = customer.ShoppingCartItems
                            .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).LimitPerStore(storeId)
                            .ToList();
                        if (ShoppingCartItems.Any())
                            foreach (var cartItem in ShoppingCartItems)
                                if (cartItem.Product != null && cartItem.Product.ProductSpecificationAttributes.Any())
                                {
                                    foreach (var psaItem in cartItem.Product.ProductSpecificationAttributes)
                                        if (psaItem.SpecificationAttributeOption != null)
                                        {
                                            var specificationAttribute = psaItem.SpecificationAttributeOption
                                                .SpecificationAttribute;
                                            if (specificationAttribute != null)
                                            {
                                                var result = 0;
                                                if (int.TryParse(requirement.RequirementProperty, out result) &&
                                                    specificationAttribute.Id == result)
                                                {
                                                    str1 = psaItem.SpecificationAttributeOption.Name ?? "";
                                                    if (isValid = CompareRequirementValue(
                                                        psaItem.SpecificationAttributeOption.Id,
                                                        requirement.RequirementValue, requirement.RequirementOperator))
                                                        break;
                                                }
                                            }
                                        }

                                    if (isValid)
                                        break;
                                }
                    }

                    if (requirementCategory == RequirementCategory.ShippingMethods)
                    {
                        var result = ShipmentMethodOperators.ShippingProvider;
                        if (Enum.TryParse(requirement.RequirementProperty, out result))
                        {
                            var attribute =
                                customer.GetAttribute<ShippingOption>(
                                    SystemCustomerAttributeNames.SelectedShippingOption, storeId);
                            if (attribute != null)
                            {
                                var str2 = (string) null;
                                switch (result)
                                {
                                    case ShipmentMethodOperators.ShippingProvider:
                                        str2 = attribute.ShippingRateComputationMethodSystemName;
                                        break;
                                    case ShipmentMethodOperators.ShippingName:
                                        str2 = attribute.Name;
                                        break;
                                }

                                if (str2 != null)
                                    isValid = CompareRequirementValue(str2, requirement.RequirementValue,
                                        requirement.RequirementOperator);
                                str1 = str2 ?? "";
                            }
                        }
                    }

                    if (requirementCategory == RequirementCategory.CustomerAttributes)
                    {
                        var attribute =
                            customer.GetAttribute<string>(SystemCustomerAttributeNames.CustomCustomerAttributes,
                                storeId);
                        if (!string.IsNullOrEmpty(attribute) &&
                            !string.IsNullOrWhiteSpace(requirement.RequirementProperty))
                        {
                            var icustomerAttributeParser = EngineContext.Current.Resolve<ICustomerAttributeParser>();
                            var attributeService = EngineContext.Current.Resolve<ICustomerAttributeService>();
                            var attrId = 0;
                            if (int.TryParse(requirement.RequirementProperty, out attrId))
                            {
                                var customerAttribute = icustomerAttributeParser.ParseCustomerAttributes(attribute)
                                    .FirstOrDefault(c => c.Id == attrId);
                                if (customerAttribute != null)
                                {
                                    var values = icustomerAttributeParser.ParseValues(attribute, customerAttribute.Id);
                                    for (var index = 0; index < values.Count; ++index)
                                    {
                                        var s = values[index];
                                        str1 = s ?? "";
                                        if (!customerAttribute.ShouldHaveValues())
                                        {
                                            if (isValid = CompareRequirementValue(s, requirement.RequirementValue,
                                                requirement.RequirementOperator))
                                                break;
                                        }
                                        else
                                        {
                                            int result;
                                            if (int.TryParse(s, out result))
                                            {
                                                var attributeValueById =
                                                    attributeService.GetCustomerAttributeValueById(result);
                                                if (attributeValueById != null && (isValid =
                                                        CompareRequirementValue(attributeValueById.Name,
                                                            requirement.RequirementValue,
                                                            requirement.RequirementOperator)))
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (requirementCategory == RequirementCategory.Product)
                    {
                        var result = ProductOperators.Category;
                        if (Enum.TryParse(requirement.RequirementProperty, out result))
                        {
                            var ShoppingCartItems = customer.ShoppingCartItems
                                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart);
                                //.LimitPerStore(storeId).ToList();
                            if (ShoppingCartItems.Any())
                                foreach (var cartItem in ShoppingCartItems)
                                    if (cartItem.Product != null)
                                    {
                                        var str2 = (string) null;
                                        switch (result)
                                        {
                                            case ProductOperators.Category:
                                                if (cartItem.Product.ProductCategories.Any())
                                                {
                                                    foreach (var category in cartItem.Product.ProductCategories)
                                                    {
                                                        str2 = category.Category.Id.ToString();
                                                        if (isValid = CompareRequirementValue(str2,
                                                            requirement.RequirementValue,
                                                            requirement.RequirementOperator))
                                                            break;
                                                    }

                                                    break;
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            case ProductOperators.Manufacturer:
                                                if (cartItem.Product.ProductManufacturers.Any())
                                                {
                                                    foreach (var item in cartItem.Product.ProductManufacturers)
                                                    {
                                                        str2 = item.ManufacturerId.ToString();
                                                        if (isValid = CompareRequirementValue(str2,
                                                            requirement.RequirementValue,
                                                            requirement.RequirementOperator))
                                                            break;
                                                    }

                                                    break;
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            case ProductOperators.Vendor:
                                                str2 = cartItem.Product.VendorId.ToString();
                                                isValid = CompareRequirementValue(str2, requirement.RequirementValue,
                                                    requirement.RequirementOperator);
                                                break;
                                        }

                                        str1 = str2;
                                        if (isValid)
                                            break;
                                    }
                        }
                    }
                }

                if (_showDebugInfo)
                {
                    stringBuilder.AppendLine(string.Format("    result={0} , current value = '{1}'", isValid, str1));
                    LogMsg(stringBuilder.ToString());
                }

                if (!isValid && groupInteractionType == GroupInteractionType.And ||
                    isValid && groupInteractionType == GroupInteractionType.Or)
                    return isValid;
            }

            return isValid;
        }


        public override IList<IPaymentMethod> LoadActivePaymentMethods(Customer customer = null, int storeId = 0,
            int filterByCountryId = 0)
        {
            var list = LoadAllPaymentMethods(customer, storeId, filterByCountryId)
                .Where(p => _paymentSettings0.ActivePaymentMethodSystemNames.Contains(p.PluginDescriptor.SystemName))
                .ToList();


            if (customer == null || storeId == 0)
                return list;
            if (!_paymentRulesSettings.Enabled)
                return list;
            var allPaymentRules = _paymentRulesService.GetAllPaymentRules(false);
            if (!allPaymentRules.Any())
                return list;
            var ipaymentMethodList = new List<IPaymentMethod>();
            foreach (var paymentRule in allPaymentRules)
            {
                if (_showDebugInfo)
                    LogMsg(string.Format("LoadActivePaymentMethods. paymentRule.Id={0} (before)", paymentRule.Id));
                if (!ValidatePaymentRule(paymentRule, customer, storeId))
                {
                    if (_showDebugInfo)
                        LogMsg(string.Format("LoadActivePaymentMethods. paymentRule.Id={0} (not valid)",
                            paymentRule.Id));
                }
                else
                {
                    if (_showDebugInfo)
                        LogMsg(string.Format("LoadActivePaymentMethods. paymentRule.Id={0} (valid)", paymentRule.Id));
                   
                    if (string.IsNullOrWhiteSpace(paymentRule.Payments))
                        return ipaymentMethodList;
                    var payments = paymentRule.Payments;
                    var separator = new char[1] {','};
                    foreach (var item in payments.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var ipaymentMethod = list.FirstOrDefault(p => p.PluginDescriptor.SystemName.Equals(item));
                        if (ipaymentMethod != null)
                            ipaymentMethodList.Add(ipaymentMethod);
                    }
                }
            }

            return ipaymentMethodList;
        }

        private void LogMsg(string string_0)
        {
            if (_showDebugInfo) _fnsLogger.LogMessage(string_0);
        }

        public virtual bool ValidatePaymentRule(PaymentRule paymentRule, Customer customer, int storeId)
        {
            if (paymentRule == null) throw new ArgumentNullException("paymentRule");
            if (customer == null) throw new ArgumentNullException("customer");

            var isValid = false;
            var utcNow = DateTime.UtcNow;
            if (paymentRule.StartDateTimeUtc.HasValue &&
                DateTime.SpecifyKind(paymentRule.StartDateTimeUtc.Value, DateTimeKind.Utc).CompareTo(utcNow) > 0)
            {
                if (_showDebugInfo)
                    LogMsg(string.Format(
                        "ValidatePaymentRule. paymentRule.Id={0} (novalid) Sorry, this payment method rule is not started yet.",
                        paymentRule.Id));
                return isValid;
            }

            if (paymentRule.EndDateTimeUtc.HasValue &&
                DateTime.SpecifyKind(paymentRule.EndDateTimeUtc.Value, DateTimeKind.Utc).CompareTo(utcNow) < 0)
            {
                if (_showDebugInfo)
                    LogMsg(string.Format(
                        "ValidatePaymentRule. paymentRule.Id={0} (novalid) Sorry, this payment method rule is expired",
                        paymentRule.Id));
                return isValid;
            }

            if (paymentRule.LimitedToStores && !_storeMappingService.Authorize(paymentRule, storeId))
            {
                if (_showDebugInfo)
                    LogMsg(string.Format(
                        "ValidatePaymentRule. paymentRule.Id={0} (novalid) Sorry, this payment method rule is not for this store",
                        paymentRule.Id));
                return isValid;
            }

            if (!_aclService.Authorize(paymentRule, customer))
            {
                if (_showDebugInfo)
                    LogMsg(string.Format(
                        "ValidatePaymentRule. paymentRule.Id={0} (novalid) Sorry, this payment method rule is not for this customer's role",
                        paymentRule.Id));
                return isValid;
            }

            var str = string.Format("FoxNetSoft.PaymentRules.requirements-{0}", paymentRule.Id);
            var requirementForCachings = _staticCacheManager.Get(str, () =>
            {
                var requirementsByPaymentRuleId =
                    _paymentRulesService.GetRequirementsByPaymentRuleId(paymentRule.Id, false);
                return GetReqirementsForCaching(requirementsByPaymentRuleId);
            });
            var requirementForCaching =
                requirementForCachings.FirstOrDefault(r => r.RequirementCategory.Equals("DefaultRequirementGroup"));
            if (requirementForCaching != null &&
                (!requirementForCaching.IsGroup || requirementForCaching.ChildRequirements.Any()) &&
                requirementForCaching.InteractionType.HasValue)
            {
                var interactionType = requirementForCaching.InteractionType;
                var validationResult =
                    GetValidationResult(requirementForCachings, interactionType.Value, customer, storeId);
                isValid = validationResult;
                return validationResult;
            }

            if (_showDebugInfo)
                LogMsg(string.Format(
                    "ValidatePaymentRule. paymentRule.Id={0} (valid) there are no requirements, so payment rule is valid",
                    paymentRule.Id));
            isValid = true;
            return true;
        }

        [Serializable]
        public class RequirementForCaching
        {
            public RequirementForCaching()
            {
                ChildRequirements = new List<RequirementForCaching>();
            }

            public IList<RequirementForCaching> ChildRequirements { get; set; }

            public int ExternalId { get; set; }

            public int Id { get; set; }

            public GroupInteractionType? InteractionType { get; set; }

            public bool IsGroup { get; set; }

            public string RequirementCategory { get; set; }

            public string RequirementOperator { get; set; }

            public string RequirementProperty { get; set; }

            public string RequirementValue { get; set; }
        }
    }
}
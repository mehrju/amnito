using System;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Domain
{
	public enum RequirementCategory
	{
		DefaultRequirementGroup = 0,
		AddGroup = 1,
		BillingAddress = 10,
		ShippingAddress = 11,
		CheckoutAttributes = 12,
		OrderTotals = 13,
		SpecificationAttributes = 14,
		ShippingMethods = 15,
		CustomerAttributes = 16,
		Product = 17
	}
}
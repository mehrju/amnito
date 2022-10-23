using FoxNetSoft.Plugin.Payments.PaymentRules.Domain;
using System;
using System.Collections.Generic;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Services
{
	public interface IPaymentRulesService
    {
		void DeletePaymentRule(PaymentRule paymentRule);

		void DeletePaymentRuleRequirement(PaymentRuleRequirement paymentRuleRequirement);

		IList<PaymentRule> GetAllPaymentRules(bool showHidden = false);

		PaymentRule GetPaymentRuleById(int paymentRuleId);

		PaymentRuleRequirement GetPaymentRuleRequirementById(int paymentRuleRequirementId);

		IList<PaymentRuleRequirement> GetRequirementsByPaymentRuleId(int paymentRuleId, bool topLevelOnly = false);

		void InsertPaymentRule(PaymentRule paymentRule);

		void InsertPaymentRuleRequirement(PaymentRuleRequirement paymentRuleRequirement);

		void UpdatePaymentRule(PaymentRule paymentRule);

		void UpdatePaymentRuleRequirement(PaymentRuleRequirement paymentRuleRequirement);
	}
}
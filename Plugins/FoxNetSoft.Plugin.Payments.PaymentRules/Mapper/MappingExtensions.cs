using FoxNetSoft.Plugin.Payments.PaymentRules.Domain;
using FoxNetSoft.Plugin.Payments.PaymentRules.Models;
using Nop.Web.Areas.Admin.Extensions;
using System;
using System.Runtime.CompilerServices;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Mapper
{
	public static class MappingExtensions
	{
		public static PaymentRule ToEntity(this PaymentRuleModel model)
		{
			return Nop.Web.Areas.Admin.Extensions.MappingExtensions.MapTo<PaymentRuleModel, PaymentRule>(model);
		}

		public static PaymentRule ToEntity(this PaymentRuleModel model, PaymentRule destination)
		{
			return Nop.Web.Areas.Admin.Extensions.MappingExtensions.MapTo<PaymentRuleModel, PaymentRule>(model, destination);
		}

		public static PaymentRuleModel ToModel(this PaymentRule entity)
		{
			return Nop.Web.Areas.Admin.Extensions.MappingExtensions.MapTo<PaymentRule, PaymentRuleModel>(entity);
		}
	}
}
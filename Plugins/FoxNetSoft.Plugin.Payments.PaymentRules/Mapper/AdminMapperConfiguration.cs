using AutoMapper;
using FoxNetSoft.Plugin.Payments.PaymentRules.Domain;
using FoxNetSoft.Plugin.Payments.PaymentRules.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Infrastructure.Mapper;
using Nop.Web.Framework.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Mapper
{
	public class AdminMapperConfiguration : Profile, IMapperProfile
	{
		public int Order
		{
			get
			{
				return 0;
			}
		}

		public AdminMapperConfiguration()
		{
			IMappingExpression<PaymentRule, PaymentRuleModel> mappingExpression = base.CreateMap<PaymentRule, PaymentRuleModel>();
			ParameterExpression parameterExpression = Expression.Parameter(typeof(PaymentRuleModel), "dest");
			IMappingExpression<PaymentRule, PaymentRuleModel> mappingExpression1 = mappingExpression.ForMember<Dictionary<string, object>>(Expression.Lambda<Func<PaymentRuleModel, Dictionary<string, object>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(BaseNopModel).GetMethod("get_CustomProperties").MethodHandle)), new ParameterExpression[] { parameterExpression }), (IMemberConfigurationExpression<PaymentRule, PaymentRuleModel, Dictionary<string, object>> mo) => mo.Ignore());
			parameterExpression = Expression.Parameter(typeof(PaymentRuleModel), "dest");
			IMappingExpression<PaymentRule, PaymentRuleModel> mappingExpression2 = mappingExpression1.ForMember<string>(Expression.Lambda<Func<PaymentRuleModel, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(PaymentRuleModel).GetMethod("get_RequirementsHTML").MethodHandle)), new ParameterExpression[] { parameterExpression }), (IMemberConfigurationExpression<PaymentRule, PaymentRuleModel, string> mo) => mo.Ignore());
			parameterExpression = Expression.Parameter(typeof(PaymentRuleModel), "dest");
			IMappingExpression<PaymentRule, PaymentRuleModel> mappingExpression3 = mappingExpression2.ForMember<string>(Expression.Lambda<Func<PaymentRuleModel, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(PaymentRuleModel).GetMethod("get_PaymentMethodsHTML").MethodHandle)), new ParameterExpression[] { parameterExpression }), (IMemberConfigurationExpression<PaymentRule, PaymentRuleModel, string> mo) => mo.Ignore());
			parameterExpression = Expression.Parameter(typeof(PaymentRuleModel), "dest");
			IMappingExpression<PaymentRule, PaymentRuleModel> mappingExpression4 = mappingExpression3.ForMember<string[]>(Expression.Lambda<Func<PaymentRuleModel, string[]>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(PaymentRuleModel).GetMethod("get_CheckedPaymentMethods").MethodHandle)), new ParameterExpression[] { parameterExpression }), (IMemberConfigurationExpression<PaymentRule, PaymentRuleModel, string[]> mo) => mo.Ignore());
			parameterExpression = Expression.Parameter(typeof(PaymentRuleModel), "dest");
			IMappingExpression<PaymentRule, PaymentRuleModel> mappingExpression5 = mappingExpression4.ForMember<IList<SelectListItem>>(Expression.Lambda<Func<PaymentRuleModel, IList<SelectListItem>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(PaymentRuleModel).GetMethod("get_AvailablePaymentMethods").MethodHandle)), new ParameterExpression[] { parameterExpression }), (IMemberConfigurationExpression<PaymentRule, PaymentRuleModel, IList<SelectListItem>> mo) => mo.Ignore());
			parameterExpression = Expression.Parameter(typeof(PaymentRuleModel), "dest");
			IMappingExpression<PaymentRule, PaymentRuleModel> mappingExpression6 = mappingExpression5.ForMember<IList<int>>(Expression.Lambda<Func<PaymentRuleModel, IList<int>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(PaymentRuleModel).GetMethod("get_SelectedCustomerRoleIds").MethodHandle)), new ParameterExpression[] { parameterExpression }), (IMemberConfigurationExpression<PaymentRule, PaymentRuleModel, IList<int>> mo) => mo.Ignore());
			parameterExpression = Expression.Parameter(typeof(PaymentRuleModel), "dest");
			IMappingExpression<PaymentRule, PaymentRuleModel> mappingExpression7 = mappingExpression6.ForMember<IList<SelectListItem>>(Expression.Lambda<Func<PaymentRuleModel, IList<SelectListItem>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(PaymentRuleModel).GetMethod("get_AvailableCustomerRoles").MethodHandle)), new ParameterExpression[] { parameterExpression }), (IMemberConfigurationExpression<PaymentRule, PaymentRuleModel, IList<SelectListItem>> mo) => mo.Ignore());
			parameterExpression = Expression.Parameter(typeof(PaymentRuleModel), "dest");
			IMappingExpression<PaymentRule, PaymentRuleModel> mappingExpression8 = mappingExpression7.ForMember<IList<int>>(Expression.Lambda<Func<PaymentRuleModel, IList<int>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(PaymentRuleModel).GetMethod("get_SelectedStoreIds").MethodHandle)), new ParameterExpression[] { parameterExpression }), (IMemberConfigurationExpression<PaymentRule, PaymentRuleModel, IList<int>> mo) => mo.Ignore());
			parameterExpression = Expression.Parameter(typeof(PaymentRuleModel), "dest");
			mappingExpression8.ForMember<IList<SelectListItem>>(Expression.Lambda<Func<PaymentRuleModel, IList<SelectListItem>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(PaymentRuleModel).GetMethod("get_AvailableStores").MethodHandle)), new ParameterExpression[] { parameterExpression }), (IMemberConfigurationExpression<PaymentRule, PaymentRuleModel, IList<SelectListItem>> mo) => mo.Ignore());
			base.CreateMap<PaymentRuleModel, PaymentRule>();
		}
	}
}
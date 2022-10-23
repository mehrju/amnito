using Autofac;
using Autofac.Builder;
using Autofac.Core;
using FoxNetSoft.Plugin.Payments.PaymentRules.Data;
using FoxNetSoft.Plugin.Payments.PaymentRules.Domain;
using FoxNetSoft.Plugin.Payments.PaymentRules.Logger;
using FoxNetSoft.Plugin.Payments.PaymentRules.Services;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Services.Payments;
using Nop.Web.Framework.Infrastructure;
using System;

namespace FoxNetSoft.Plugin.Payments.PaymentRules
{
	public class DependencyRegistrar : IDependencyRegistrar
	{
		public int Order
		{
			get
			{
				return 100;
			}
		}

		public DependencyRegistrar()
		{
		}

		public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
		{
			DependencyRegistrarExtensions.RegisterPluginDataContext<PaymentRulesObjectContext>(this, builder, "nop_object_context_foxnetsoft_paymentrules", null, false);
			builder.RegisterType<PaymentRulesService>().As<IPaymentRulesService>().InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<PaymentRule>>()
                .As<IRepository<PaymentRule>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_foxnetsoft_paymentrules"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<PaymentRuleRequirement>>()
                .As<IRepository<PaymentRuleRequirement>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_foxnetsoft_paymentrules"))
                .InstancePerLifetimeScope();

			builder.RegisterType<AdvancedPaymentService>().As<IPaymentService>().InstancePerLifetimeScope();
		}
	}
}
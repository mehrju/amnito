using Autofac;
using Autofac.Builder;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using System;
using Nop.Web.Factories;
using RegistrationExtensions = Autofac.Features.AttributeFilters.RegistrationExtensions;

namespace NopMaster.Plugin.Payments.Wallet
{
    //
	public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => Int32.MaxValue;

		public DependencyRegistrar()
		{
		}

		public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
		{
            builder.RegisterType<CheckoutModelFactoryNf>().As<ICheckoutModelFactory>().InstancePerLifetimeScope();
        }
	}
}
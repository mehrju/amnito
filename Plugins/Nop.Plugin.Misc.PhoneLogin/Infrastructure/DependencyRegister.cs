using Autofac;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Misc.PhoneLogin.Factories;
using Nop.Plugin.Misc.PhoneLogin.Services;

namespace Nop.Plugin.Misc.PhoneLogin.Infrastructure
{
    class DependencyRegistrar : IDependencyRegistrar
    {

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<PhoneLoginModelFactory>().As<IPhoneLoginModelFactory>()
                .InstancePerLifetimeScope();

            //builder.RegisterType<CustomerLoginAttribute>().As<IActionFilter>().InstancePerRequest();
            //builder.RegisterType<CustomerLoginFilterProvider>().As<IFilterProvider>().InstancePerRequest();
            builder.RegisterType<PhoneLoginPlugin>().As<PhoneLoginPlugin>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerServices>().As<ICustomerServices>().InstancePerLifetimeScope();
            
        }

        public int Order
        {
            get { return 10; }
        }
    }
}

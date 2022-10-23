using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Orders.BulkOrder.Models;
using Nop.Plugin.Orders.BulkOrder.Services;
using Nop.Web.Framework.Infrastructure;
using System;
using Nop.Core;
using Nop.Services.Orders;

namespace Nop.Plugin.Orders.BulkOrder.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<BulkOrderService>().As<IBulkOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<Xtn_ShoppingCartService>().As<IShoppingCartService>().InstancePerLifetimeScope();
            
            this.RegisterPluginDataContext<BulkOrderObjectContext>(builder, "nop_object_context_BulkOrder");

            builder.RegisterType<EfRepository<BulkOrderModel>>()
                .As<IRepository<BulkOrderModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_BulkOrder"))
                .InstancePerLifetimeScope();

            
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => Int32.MaxValue;
    }

    
}

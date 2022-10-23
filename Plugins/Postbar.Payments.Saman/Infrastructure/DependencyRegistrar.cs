using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using NopFarsi.Payments.SepShaparak.Service;
using Autofac;
using Autofac.Core;
using Nop.Core.Data;
using Nop.Data;
using Nop.Web.Framework.Infrastructure;
using NopFarsi.Payments.SepShaparak.Data;
using NopFarsi.Payments.SepShaparak.Domain;

namespace NopFarsi.Payments.SepShaparak.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_order_refund_status";

        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<OrderRefundStatusService>().As<IOrderRefundStatusService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<OrderRefundStatusObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<OrderRefundStatus>>()
            .As<IRepository<OrderRefundStatus>>()
            .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
            .InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}

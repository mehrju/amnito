using Autofac;
using Autofac.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Misc.PostbarDashboard.Domain;
using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Plugin.Misc.PostbarDashboard.Services;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.PostbarDashboard.Infrastructure
{
    class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_PostbarDashboard";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            this.RegisterPluginDataContext<PostbarDashboardObjectContext>(builder, CONTEXT_NAME);

            builder.RegisterType<OrderServices>().As<IOrderServices>().InstancePerLifetimeScope();
            builder.RegisterType<DashboardService>().As<IDashboardService>().InstancePerLifetimeScope();
            builder.RegisterType<RewardPointServices>().As<IRewardPointServices>().InstancePerLifetimeScope();
            builder.RegisterType<OrderExcelService>().As<IOrderExcelService>().InstancePerLifetimeScope();
            builder.RegisterType<CODRequestService>().As<ICODRequestService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderPdfService>().As<IOrderPdfService>().InstancePerLifetimeScope();
            builder.RegisterType<ServiceTypeService>().As<IServiceTypeService>().InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_CheckAvatarCustomer>>()
                .As<IRepository<Tbl_CheckAvatarCustomer>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_ViewVideoCustomer>>()
                .As<IRepository<Tbl_ViewVideoCustomer>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
            
                 builder.RegisterType<EfRepository<Tbl_ServiceProviderDashboard>>()
                .As<IRepository<Tbl_ServiceProviderDashboard>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();



            builder.RegisterType<EfRepository<Tbl_Carousel_slideshow>>()
              .As<IRepository<Tbl_Carousel_slideshow>>()
              .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
              .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_CODRequestLog>>()
             .As<IRepository<Tbl_CODRequestLog>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();

        }

        public int Order
        {
            get { return 1; }
        }
    }
}

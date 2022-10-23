using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using BS.Plugin.NopStation.MobileWebApi.Data;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "bs_object_context_nopstation_mobilewebapi";
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<BsNopMobilePluginService>().As<IBsNopMobilePluginService>().InstancePerLifetimeScope();
            builder.RegisterType<ContentManagementService>().As<IContentManagementService>().InstancePerLifetimeScope();
            builder.RegisterType<ContentManagementTemplateService>().As<IContentManagementTemplateService>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryIconService>().As<ICategoryIconService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<MobileWebApiObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<BS_FeaturedProducts>>()
                .As<IRepository<BS_FeaturedProducts>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<BS_ContentManagement>>()
             .As<IRepository<BS_ContentManagement>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<BS_ContentManagementTemplate>>()
             .As<IRepository<BS_ContentManagementTemplate>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<BS_CategoryIcons>>()
             .As<IRepository<BS_CategoryIcons>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}

using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Misc.PrintedReports_Requirements.Controllers;
using Nop.Plugin.Misc.PrintedReports_Requirements.Service;
using Nop.Plugin.Misc.PrintedReports_Requirements.Service.Interface;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        //private const string CONTEXT_NAME = "nop_object_context_PrintedReports_Requirements";

        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //register Service
            //as self
            //builder.RegisterType<Customer>().As<AvatarController>().InstancePerLifetimeScope();
            builder.RegisterType<GetByteAvatarCustomer_Service>().As<IGetByteAvatarCustomer_Service>().InstancePerLifetimeScope();
            builder.RegisterType<IndexPageService>().As<I_IndexPageService>().InstancePerLifetimeScope();








            //data context
            //this.RegisterPluginDataContext<PrintedReports_RequirementsObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            //builder.RegisterType<EfRepository<Tbl_ServicesProviders>>()
            //    .As<IRepository<Tbl_ServicesProviders>>()
            //    .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
            //    .InstancePerLifetimeScope();



        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 1; }
        }
    }
}
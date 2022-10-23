using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Misc.Ticket.Data;
using Nop.Plugin.Misc.Ticket.Domain;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.Ticket.Infrastructure
{
    /// <summary>
    /// Dependency registrarm
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_Ticket";

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
            //builder.RegisterType<PDE_Service>().As<IPDE_Service>().InstancePerLifetimeScope();




            //data context
            this.RegisterPluginDataContext<TicketObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<Tbl_Ticket_Department>>()
                .As<IRepository<Tbl_Ticket_Department>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_Ticket_Priority>>()
               .As<IRepository<Tbl_Ticket_Priority>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
               .InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tbl_Ticket_Staff_Department>>()
               .As<IRepository<Tbl_Ticket_Staff_Department>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
               .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_Ticket>>()
              .As<IRepository<Tbl_Ticket>>()
              .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
              .InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tbl_Ticket_Detail>>()
              .As<IRepository<Tbl_Ticket_Detail>>()
              .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
              .InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tbl_FAQCategory>>()
             .As<IRepository<Tbl_FAQCategory>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tbl_FAQ>>()
           .As<IRepository<Tbl_FAQ>>()
           .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
           .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_PatternAnswer_Ticket_Damages_RequestCOD>>()
           .As<IRepository<Tbl_PatternAnswer_Ticket_Damages_RequestCOD>>()
           .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
           .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_CategoryTicket>>()
           .As<IRepository<Tbl_CategoryTicket>>()
           .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
           .InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tbl_TrainingAcademyTopic>>()
           .As<IRepository<Tbl_TrainingAcademyTopic>>()
           .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
           .InstancePerLifetimeScope();



            builder.RegisterType<EfRepository<Tbl_Damages>>()
          .As<IRepository<Tbl_Damages>>()
          .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
          .InstancePerLifetimeScope();



            builder.RegisterType<EfRepository<Tbl_Damages_Detail>>()
          .As<IRepository<Tbl_Damages_Detail>>()
          .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
          .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_LogSMS>>()
         .As<IRepository<Tbl_LogSMS>>()
         .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
         .InstancePerLifetimeScope();

          

            builder.RegisterType<EfRepository<Tbl_RequestCODCustomer>>()
      .As<IRepository<Tbl_RequestCODCustomer>>()
      .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
      .InstancePerLifetimeScope();
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
using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Misc.ShippingSolutions.Data;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using Nop.Plugin.Misc.ShippingSolutions.Services;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.ShippingSolutions.Infrastructure
{
    /// <summary>
    /// Dependency registrarm
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_Shipping_Solutions";

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
            builder.RegisterType<PaterPricingPolicyService>().As<IPaterPricingPolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<LocationService>().As<ILocationService>().InstancePerLifetimeScope();
            builder.RegisterType<PDE_Service>().As<IPDE_Service>().InstancePerLifetimeScope();
            builder.RegisterType<YarBox_Service>().As<IYarBox_Service>().InstancePerLifetimeScope();
            builder.RegisterType<Ubaar_Service>().As<IUbaar_Service>().InstancePerLifetimeScope();
            builder.RegisterType<Tinex_Service>().As<ITinex_Service>().InstancePerLifetimeScope();
            builder.RegisterType<Safiran_Service>().As<ISafiran_Service>().InstancePerLifetimeScope();
            builder.RegisterType<TPG_Service>().As<ITPG_Service>().InstancePerLifetimeScope();
            builder.RegisterType<Chapar_Service>().As<IChapar_Service>().InstancePerLifetimeScope();
            builder.RegisterType<Taroff_Service>().As<ITaroff_Service>().InstancePerLifetimeScope();
            builder.RegisterType<Snappbox_Service>().As<ISnappbox_Service>().InstancePerLifetimeScope();
            builder.RegisterType<Persain_Service>().As<IPersian_Service>().InstancePerLifetimeScope();
            builder.RegisterType<SkyBlue_Service>().As<ISkyBlue_Service>().InstancePerLifetimeScope();
            builder.RegisterType<CheckRegionDesVijePost>().As<ICheckRegionDesVijePost>().InstancePerLifetimeScope();
            builder.RegisterType<Mahex_Service>().As<IMahex_Service>().InstancePerLifetimeScope();
            builder.RegisterType<kalaResan_Service>().As<IkalaResan_Service>().InstancePerLifetimeScope();


            


            //data context
            this.RegisterPluginDataContext<ShippingSolutionsObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<Tbl_ServicesProviders>>()
                .As<IRepository<Tbl_ServicesProviders>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_ServicesTypes>>()
                .As<IRepository<Tbl_ServicesTypes>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_ServiceTypesProvider>>()
               .As<IRepository<Tbl_ServiceTypesProvider>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
               .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_Dealer>>()
               .As<IRepository<Tbl_Dealer>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
               .InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tbl_Offices>>()
               .As<IRepository<Tbl_Offices>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
               .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_WorkingTime>>()
               .As<IRepository<Tbl_WorkingTime>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
               .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_Collectors>>()
              .As<IRepository<Tbl_Collectors>>()
              .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
              .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_ProviderStores>>()
              .As<IRepository<Tbl_ProviderStores>>()
              .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
              .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_CollectoreStores>>()
             .As<IRepository<Tbl_CollectoreStores>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_CollectorsServiceProvider>>()
             .As<IRepository<Tbl_CollectorsServiceProvider>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_Dealer_Customer_ServiceProvider>>()
             .As<IRepository<Tbl_Dealer_Customer_ServiceProvider>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_PricingPolicy>>()
             .As<IRepository<Tbl_PricingPolicy>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
            .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_PatternPricingPolicy>>()
            .As<IRepository<Tbl_PatternPricingPolicy>>()
            .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
           .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_CarriersTarof>>()
           .As<IRepository<Tbl_CarriersTarof>>()
           .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
          .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<Tbl_Ostantarof>>()
           .As<IRepository<Tbl_Ostantarof>>()
           .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
          .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<Tbl_CityTarof>>()
           .As<IRepository<Tbl_CityTarof>>()
           .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))

          .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<Tbl_PaymentMethodsTarof>>()
           .As<IRepository<Tbl_PaymentMethodsTarof>>()
           .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
          .InstancePerLifetimeScope();




            builder.RegisterType<EfRepository<Tbl_Address_LatLong>>()
         .As<IRepository<Tbl_Address_LatLong>>()
         .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
        .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_Product_PatternPricing>>()
        .As<IRepository<Tbl_Product_PatternPricing>>()
        .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
       .InstancePerLifetimeScope();




            builder.RegisterType<EfRepository<Tbl_CountryISO3166>>()
        .As<IRepository<Tbl_CountryISO3166>>()
        .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
       .InstancePerLifetimeScope();



            builder.RegisterType<EfRepository<Tbl_ParcelTypeSkyBlue>>()
        .As<IRepository<Tbl_ParcelTypeSkyBlue>>()
        .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
       .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_DiscountPlan_AgentCustomer>>()
     .As<IRepository<Tbl_DiscountPlan_AgentCustomer>>()
     .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
    .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_SalesPartnersPercent>>()
     .As<IRepository<Tbl_SalesPartnersPercent>>()
     .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
    .InstancePerLifetimeScope();



            builder.RegisterType<EfRepository<Tbl_RelationOstanCityVijePost>>()
.As<IRepository<Tbl_RelationOstanCityVijePost>>()
.WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
.InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tbl_AffiliateToCustomer>>()
             .As<IRepository<Tbl_AffiliateToCustomer>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();



            builder.RegisterType<EfRepository<Tbl_IncentivePlanCustomer>>()
.As<IRepository<Tbl_IncentivePlanCustomer>>()
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
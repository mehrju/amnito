using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Web.Framework.Infrastructure;
using System;
using Nop.Services.Orders;
using Nop.Plugin.Orders.MultiShipping.Models.Tbl_Extra_Status_Field_Shipment;
using Nop.Plugin.Orders.MultiShipping.Models.RSVP;
using Nop.Plugin.Orders.MultiShipping.Models.GeoPoint;
using Nop.Plugin.Orders.MultiShipping.Domain;
using Nop.Plugin.Orders.MultiShipping.Services.Comment;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http.Features;

namespace Nop.Plugin.Orders.MultiShipping.Infrastructure
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
            this.RegisterPluginDataContext<ExnShippmentObjectContext> (builder, "nop_object_context_ExnShippment");
            builder.RegisterType<NewCheckout>().As<INewCheckout>().InstancePerLifetimeScope();
            builder.RegisterType<Mus_OrderProcessingService>().As<IExtnOrderProcessingService>().InstancePerLifetimeScope();
            builder.RegisterType<HagheMaghar>().As<IHagheMaghar>().InstancePerLifetimeScope();
            builder.RegisterType<XtnOrderProcessingService>().As<IOrderProcessingService>().InstancePerLifetimeScope();
            builder.RegisterType<ApService>().As<IApService>().InstancePerLifetimeScope();
            builder.RegisterType<PackingRequestService>().As<IPackingRequestService>();
            builder.RegisterType<OrderCostService>().As<IOrderCostService>();
            builder.RegisterType<ReOrderService>().As<IReOrderService>();
            builder.RegisterType<TicketService>().As<ITicketService>();
            builder.RegisterType<SepService>().As<ISepService>();
            builder.RegisterType<CommentService>().As<ICommentService>();



            builder.RegisterType<EfRepository<ExnShippmentModel>>()
                .As<IRepository<ExnShippmentModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ExnShippment"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<FirstOrderModel>>()
                .As<IRepository<FirstOrderModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ExnShippment"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<HagheMagharModel>>()
                .As<IRepository<HagheMagharModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ExnShippment"))
                .InstancePerLifetimeScope();
            
                  builder.RegisterType<EfRepository<TblExtraFiledShipment>>()
                .As<IRepository<TblExtraFiledShipment>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ExnShippment"))
                .InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tbl_RSVP_Webhook>>()
              .As<IRepository<Tbl_RSVP_Webhook>>()
              .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ExnShippment"))
              .InstancePerLifetimeScope();



            builder.RegisterType<EfRepository<Tb_StateProvinceGeoPoints>>()
             .As<IRepository<Tb_StateProvinceGeoPoints>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ExnShippment"))
             .InstancePerLifetimeScope();



            builder.RegisterType<EfRepository<Tb_CountryGeoPoints>>()
             .As<IRepository<Tb_CountryGeoPoints>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ExnShippment"))
             .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_ShipmentPackingRequest>>()
            .As<IRepository<Tbl_ShipmentPackingRequest>>()
            .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ExnShippment"))
            .InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tbl_OrderJson>>()
            .As<IRepository<Tbl_OrderJson>>()
            .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ExnShippment"))
            .InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tbl_Comment>>()
           .As<IRepository<Tbl_Comment>>()
           .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ExnShippment"))
           .InstancePerLifetimeScope();

        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => Int32.MaxValue;
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequestSizeLimitAttribute : Attribute, IAuthorizationFilter, IOrderedFilter
    {
        private readonly FormOptions _formOptions;

        public RequestSizeLimitAttribute(int valueCountLimit)
        {
            _formOptions = new FormOptions()
            {
                // tip: you can use different arguments to set each properties instead of single argument
                KeyLengthLimit = valueCountLimit,
                ValueCountLimit = valueCountLimit,
                ValueLengthLimit = valueCountLimit

                // uncomment this line below if you want to set multipart body limit too
                // MultipartBodyLengthLimit = valueCountLimit
            };
        }

        public int Order { get; set; }

        // taken from /a/38396065
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var contextFeatures = context.HttpContext.Features;
            var formFeature = contextFeatures.Get<IFormFeature>();

            if (formFeature == null || formFeature.Form == null)
            {
                // Setting length limit when the form request is not yet being read
                contextFeatures.Set<IFormFeature>(new FormFeature(context.HttpContext.Request, _formOptions));
            }
        }
    }
}

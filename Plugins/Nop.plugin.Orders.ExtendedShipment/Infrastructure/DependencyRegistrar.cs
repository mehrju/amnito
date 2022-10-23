using Autofac;
using Autofac.Core;
using BS.Plugin.Orders.ExtendedShipment.Services;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Domain.PhoneOrder;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Models.NotificationModel;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.plugin.Orders.ExtendedShipment.Services.ForiegnOrder;
using Nop.plugin.Orders.ExtendedShipment.Services.Messages;
using Nop.plugin.Orders.ExtendedShipment.Services.PhoneOrder;
using Nop.plugin.Orders.ExtendedShipment.Services.PreOrderService;
using Nop.plugin.Orders.ExtendedShipment.Services.Tozico;
using Nop.Services.Common;
using Nop.Web.Framework.Infrastructure;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;
using Nop.Plugin.Misc.ShippingSolutions.Services;
using System;

namespace Nop.plugin.Orders.ExtendedShipment.Infrastructure
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
            builder.RegisterType<ForeignOrder>().As<IForeignOrder>().InstancePerLifetimeScope();
            builder.RegisterType<CollectorService>().As<ICollectorService>().InstancePerLifetimeScope();
            builder.RegisterType<ShipmentTrackingService>().As<IShipmentTrackingService>().InstancePerLifetimeScope();
            builder.RegisterType<CodService>().As<ICodService>().InstancePerLifetimeScope();
            builder.RegisterType<ExtendedShipmentService>().As<IExtendedShipmentService>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<ExtnOrderModelFactory>().As<IExtnOrderModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<OrderStatusTrackingService>().As<IOrderStatusTrackingService>().InstancePerLifetimeScope();
            builder.RegisterType<ExtendedPdfService>().As<IPdfService>().InstancePerLifetimeScope();
            builder.RegisterType<UserStatesService>().As<IUserStatesService>().InstancePerLifetimeScope();
            builder.RegisterType<AgentAmountRuleService>().As<IAgentAmountRuleService>().InstancePerLifetimeScope();
            builder.RegisterType<AccountingService>().As<IAccountingService>().InstancePerLifetimeScope();
            builder.RegisterType<SecurityService>().As<ISecurityService>().InstancePerLifetimeScope();
            builder.RegisterType<Collection_SnapboxTarof_Service>().As<ICollection_SnapboxTarof_Service>().InstancePerLifetimeScope();
            builder.RegisterType<Declaration_Status_foreign_order_Service>().As<IDeclaration_Status_foreign_order_Service>().InstancePerLifetimeScope();
            builder.RegisterType<RelatedOrders_Service>().As<IRelatedOrders_Service>().InstancePerLifetimeScope();
            builder.RegisterType<CalcPriceOrderItem_Service>().As<ICalcPriceOrderItem_Service>().InstancePerLifetimeScope();
            builder.RegisterType<TrackingUbaarOrder_Service>().As<ITrackingUbaarOrder_Service>().InstancePerLifetimeScope();
            builder.RegisterType<TozicoService>().As<ITozicoService>().InstancePerLifetimeScope();
            builder.RegisterType<PostexEmailSender>().As<IPostexEmailSender>().InstancePerLifetimeScope();
            builder.RegisterType<OrderItemsRecordService>().As<IOrderItemsRecordService>().InstancePerLifetimeScope();
            builder.RegisterType<AttributeEditorService>().As<IAttributeEditorService>().InstancePerLifetimeScope();
            builder.RegisterType<CartonService>().As<ICartonService>().InstancePerLifetimeScope();
            builder.RegisterType<PostBarCodeService>().As<IPostBarCodeService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerSettingService>().As<ICustomerSettingService>().InstancePerLifetimeScope();
            builder.RegisterType<ChargeWalletFailService>().As<IChargeWalletFailService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerVehicleService>().As<ICustomerVehicleService>().InstancePerLifetimeScope();
            builder.RegisterType<ApiOrderRefrenceCodeService>().As<IApiOrderRefrenceCodeService>().InstancePerLifetimeScope();
            builder.RegisterType<RewardPointCashoutService>().As<IRewardPointCashoutService>().InstancePerLifetimeScope();
            builder.RegisterType<PhoneOrderService>().As<IPhoneOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomeCookieAuthenticationService>().As<ICustomAuthenticationService>().InstancePerLifetimeScope();
            builder.RegisterType<OptimeApiService>().As<IOptimeApiService>().InstancePerLifetimeScope();
            builder.RegisterType<AgentConfigService>().As<IAgentConfigService>().InstancePerLifetimeScope();
            builder.RegisterType<PreOrderService>().As<IPreOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<Mahex_Service>().As<IMahex_Service>().InstancePerLifetimeScope();
            builder.RegisterType<SekehService>().As<ISekehService>().InstancePerLifetimeScope();
            builder.RegisterType<ContractService>().As<IContractService>().InstancePerLifetimeScope();

            this.RegisterPluginDataContext<ShipmentAppointmenObjectContext>(builder, "nop_object_context_ShipmentAppointmen");


            builder.RegisterType<EfRepository<ChargeWalletHistoryModel>>()
                .As<IRepository<ChargeWalletHistoryModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<NotifItemsModel>>()
               .As<IRepository<NotifItemsModel>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
               .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<NotifTitleModel>>()
               .As<IRepository<NotifTitleModel>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
               .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<NotifTypeModel>>()
               .As<IRepository<NotifTypeModel>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
               .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<OrderItemAttributeValueModel>>()
                .As<IRepository<OrderItemAttributeValueModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<TaskHistoryModel>>()
                .As<IRepository<TaskHistoryModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<ShipmentAppointmentModel>>()
                .As<IRepository<ShipmentAppointmentModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<AssignAgentAmountRuleModel>>()
                .As<IRepository<AssignAgentAmountRuleModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<AgentAmountRuleModel>>()
                .As<IRepository<AgentAmountRuleModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<CountryCodeModel>>()
                .As<IRepository<CountryCodeModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<StateCodemodel>>()
                .As<IRepository<StateCodemodel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<CateguryPostTypeModel>>()
                .As<IRepository<CateguryPostTypeModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<BarcodeRepositoryModel>>()
                .As<IRepository<BarcodeRepositoryModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<PostCoordinationModel>>()
               .As<IRepository<PostCoordinationModel>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
               .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<OrderStatusTrackingModel>>()
                .As<IRepository<OrderStatusTrackingModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<ShipmentTrackingModel>>()
                .As<IRepository<ShipmentTrackingModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<ShipmentEventModel>>()
                .As<IRepository<ShipmentEventModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<CODShipmentEventModel>>()
                .As<IRepository<CODShipmentEventModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<CodShipmentFinancialModel>>()
                .As<IRepository<CodShipmentFinancialModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<PayForCodLogModel>>()
                .As<IRepository<PayForCodLogModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<UserStetesModel>>()
                .As<IRepository<UserStetesModel>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
                .InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tbl_Collection_Snappbaox_Tarof>>()
               .As<IRepository<Tbl_Collection_Snappbaox_Tarof>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
               .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_CancelReason_Order>>()
               .As<IRepository<Tbl_CancelReason_Order>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
               .InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tbl_CancellReason>>()
              .As<IRepository<Tbl_CancellReason>>()
              .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
              .InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tbl_TrackingForeignOrder>>()
              .As<IRepository<Tbl_TrackingForeignOrder>>()
              .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
              .InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tb_RelatedOrders>>()
              .As<IRepository<Tb_RelatedOrders>>()
              .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
              .InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tb_CalcPriceOrderItem>>()
          .As<IRepository<Tb_CalcPriceOrderItem>>()
          .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
          .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_TrackingUbaarOrder>>()
             .As<IRepository<Tbl_TrackingUbaarOrder>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
             .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_PopupNotification>>()
            .As<IRepository<Tbl_PopupNotification>>()
            .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
            .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_CustomerDepositCode>>()
            .As<IRepository<Tbl_CustomerDepositCode>>()
            .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
            .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tb_ShippingServiceLog>>()
            .As<IRepository<Tb_ShippingServiceLog>>()
            .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
            .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_ChargeWalletFails>>()
           .As<IRepository<Tbl_ChargeWalletFails>>()
           .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
           .InstancePerLifetimeScope();


            builder.RegisterType<EfRepository<Tbl_CartonInfo>>()
           .As<IRepository<Tbl_CartonInfo>>()
           .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
           .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_CustomerVehicle>>()
           .As<IRepository<Tbl_CustomerVehicle>>()
           .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
           .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_ApiOrderRefrenceCode>>()
           .As<IRepository<Tbl_ApiOrderRefrenceCode>>()
           .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
           .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_RewritePaths>>()
           .As<IRepository<Tbl_RewritePaths>>()
           .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
           .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_PhoneOrder>>()
          .As<IRepository<Tbl_PhoneOrder>>()
          .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
          .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_PhoneOrder_Order>>()
         .As<IRepository<Tbl_PhoneOrder_Order>>()
         .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
         .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_ShipmentEventCategory>>()
         .As<IRepository<Tbl_ShipmentEventCategory>>()
         .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
         .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_OpTimeList>>()
         .As<IRepository<Tbl_OpTimeList>>()
         .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
         .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tbl_CollectorOptimeUser>>()
         .As<IRepository<Tbl_CollectorOptimeUser>>()
         .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
         .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Tb_preOrder>>()
         .As<IRepository<Tb_preOrder>>()
         .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
         .InstancePerLifetimeScope();
            
            builder.RegisterType<EfRepository<Tbl_Contract>>()
         .As<IRepository<Tbl_Contract>>()
         .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
         .InstancePerLifetimeScope();
            
            builder.RegisterType<EfRepository<Tbl_ContractItems>>()
         .As<IRepository<Tbl_ContractItems>>()
         .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
         .InstancePerLifetimeScope();
            
            builder.RegisterType<EfRepository<Tbl_ContractItemDetail>>()
         .As<IRepository<Tbl_ContractItemDetail>>()
         .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_ShipmentAppointmen"))
         .InstancePerLifetimeScope();

        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => Int32.MaxValue;
    }
}

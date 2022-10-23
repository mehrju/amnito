using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Nop.Data;
using Nop.Core;

using System.Data.Entity.Infrastructure;
using Nop.plugin.Orders.ExtendedShipment.Models.NotificationModel;
using Nop.plugin.Orders.ExtendedShipment.Data;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class ShipmentAppointmenObjectContext : DbContext, IDbContext
    {
        public ShipmentAppointmenObjectContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        #region Implementation of IDbContext

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<ShipmentAppointmenObjectContext>(null);
            modelBuilder.Configurations.Add(new ShipmentAppointmentMap());
            modelBuilder.Configurations.Add(new CountryCodeMap());
            modelBuilder.Configurations.Add(new StateCodeMap());
            modelBuilder.Configurations.Add(new CateguryPostTypeMap());
            modelBuilder.Configurations.Add(new BarcodeRepositoryMap());
            modelBuilder.Configurations.Add(new PostCoordinationMap());
            modelBuilder.Configurations.Add(new OrderStatusTrackingMap());
            modelBuilder.Configurations.Add(new ShipmentTrackingMap());
            modelBuilder.Configurations.Add(new ShipmentEventMap());
            modelBuilder.Configurations.Add(new CODShipmentEventMap());
            modelBuilder.Configurations.Add(new CodShipmentFinancialMap());
            modelBuilder.Configurations.Add(new PayForCodLogMap());
            modelBuilder.Configurations.Add(new UserStatesMap());
            modelBuilder.Configurations.Add(new TaskHistoryMap());
            modelBuilder.Configurations.Add(new AssignAgentAmountRuleMap());
            modelBuilder.Configurations.Add(new AgentAmountRuleMap());
            modelBuilder.Configurations.Add(new ChargeWalletHistoryMap());
            modelBuilder.Configurations.Add(new OrderItemAttributeValueMap());
            modelBuilder.Configurations.Add(new NotifItemsMap());
            modelBuilder.Configurations.Add(new NotifTitleMap());
            modelBuilder.Configurations.Add(new NotifTypeMap());
            modelBuilder.Configurations.Add(new Tbl_Collection_Snappbaox_TarofMap());
            modelBuilder.Configurations.Add(new Tbl_CancelReason_OrderMap());
            modelBuilder.Configurations.Add(new Tbl_CancellReasonMap());
            modelBuilder.Configurations.Add(new Tbl_TrackingForeignOrderMap());
            modelBuilder.Configurations.Add(new Tb_RelatedOrdersMap());
            modelBuilder.Configurations.Add(new Tb_CalcPriceOrderItemMap());
            modelBuilder.Configurations.Add(new Tbl_TrackingUbaarOrderMap());
            modelBuilder.Configurations.Add(new Tbl_PopupNotificationMap());
            modelBuilder.Configurations.Add(new Tbl_CustomerDepositCodeMap());
            modelBuilder.Configurations.Add(new Tbl_ChargeWalletFailsMap());
            modelBuilder.Configurations.Add(new Tb_ShippingServiceLogMap());
            modelBuilder.Configurations.Add(new Tbl_CartonInfoMap());
            modelBuilder.Configurations.Add(new Tbl_CustomerVehicleMap());
            modelBuilder.Configurations.Add(new Tbl_ApiOrderRefrenceCodeMap());
			modelBuilder.Configurations.Add(new Tbl_RewritePathsMap());
            modelBuilder.Configurations.Add(new Tbl_PhoneOrderMap());
            modelBuilder.Configurations.Add(new Tbl_PhoneOrder_OrderMap());
            modelBuilder.Configurations.Add(new Tbl_OpTimeListMap());
            modelBuilder.Configurations.Add(new Tbl_OptimeListDetailesMap());
            modelBuilder.Configurations.Add(new Tbl_ShipmentEventCategoryMap());
            modelBuilder.Configurations.Add(new Tbl_CollectorOptimeUserMap());
            modelBuilder.Configurations.Add(new Tb_preOrderMap());
            modelBuilder.Configurations.Add(new Tbl_ContractMap());
            modelBuilder.Configurations.Add(new Tbl_ContractItemsMap());
            modelBuilder.Configurations.Add(new Tbl_ContractItemDetailMap());

            base.OnModelCreating(modelBuilder);
        }

        public string CreateDatabaseInstallationScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public void Install()
        {
            //It's required to set initializer to null (for SQL Server Compact).
            //otherwise, you'll get something like "The model backing the 'your context name' context has changed since the database was created. Consider using Code First Migrations to update the database"
            Database.SetInitializer<ShipmentAppointmenObjectContext>(null);
            Database.ExecuteSqlCommand(CreateDatabaseInstallationScript());
            SaveChanges();
        }

        public void Uninstall()
        {
            //var dbScript = "DROP TABLE ShipmentAppointment";
            //Database.ExecuteSqlCommand(dbScript);
            //dbScript = "DROP TABLE CountryCode";
            //Database.ExecuteSqlCommand(dbScript);
            //dbScript = "DROP TABLE StateCode";
            //Database.ExecuteSqlCommand(dbScript);
            //dbScript = "IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[BarcodeRepository]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1) DROP TABLE BarcodeRepository";
            //Database.ExecuteSqlCommand(dbScript);
            //dbScript = "IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[CateguryPostType]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1) DROP TABLE CateguryPostType";
            //Database.ExecuteSqlCommand(dbScript);
            //dbScript = "IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[OrderStatusTracking]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1) DROP TABLE OrderStatusTracking";
            //Database.ExecuteSqlCommand(dbScript);
            //dbScript = "IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[ShipmentTracking]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1) DROP TABLE ShipmentTracking";
            //Database.ExecuteSqlCommand(dbScript);
            //dbScript = "IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Tb_ShipmentEvent]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1) DROP TABLE Tb_ShipmentEvent";
            //Database.ExecuteSqlCommand(dbScript);
            //dbScript = "IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Tb_CODShipmentEvent]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1) DROP TABLE Tb_CODShipmentEvent";
            //Database.ExecuteSqlCommand(dbScript);
            //dbScript = "IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[ShipmentAppointment]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1) DROP TABLE ShipmentAppointment";
            //Database.ExecuteSqlCommand(dbScript);
            //dbScript = "IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[PayForCodLog]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1) DROP TABLE PayForCodLog";
            //Database.ExecuteSqlCommand(dbScript);
            //SaveChanges();
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public System.Collections.Generic.IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Detach an entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Detach(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }



        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether proxy creation setting is enabled (used in EF)
        /// </summary>
        public virtual bool ProxyCreationEnabled
        {
            get
            {
                return this.Configuration.ProxyCreationEnabled;
            }
            set
            {
                this.Configuration.ProxyCreationEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto detect changes setting is enabled (used in EF)
        /// </summary>
        public virtual bool AutoDetectChangesEnabled
        {
            get
            {
                return this.Configuration.AutoDetectChangesEnabled;
            }
            set
            {
                this.Configuration.AutoDetectChangesEnabled = value;
            }
        }
        #endregion

    }
}

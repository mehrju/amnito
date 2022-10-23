using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Nop.Data;
using Nop.Core;
using System.Data.Entity.Infrastructure;
using Nop.Plugin.Orders.MultiShipping.Models.Extra_Status_Field_Shipment;
using Nop.Plugin.Orders.MultiShipping.Models.RSVP;
using Nop.Plugin.Orders.MultiShipping.Models.GeoPoint;
using Nop.Plugin.Orders.MultiShipping.Data;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class ExnShippmentObjectContext : DbContext, IDbContext
    {
        public ExnShippmentObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString) { }

        #region Implementation of IDbContext

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<ExnShippmentObjectContext>(null);
            modelBuilder.Configurations.Add(new ExnShippmentMap());
            modelBuilder.Configurations.Add(new FirstOrderMap());
            modelBuilder.Configurations.Add(new HagheMagharMap());
            modelBuilder.Configurations.Add(new TblExtraFiledShipmentMap());
            modelBuilder.Configurations.Add(new Tbl_RSVP_WebhookMap());
            modelBuilder.Configurations.Add(new Tb_CountryGeoPointsMap());
            modelBuilder.Configurations.Add(new Tb_StateProvinceGeoPointsMap());
            modelBuilder.Configurations.Add(new Tbl_ShipmentPackingRequestMap()); 
            modelBuilder.Configurations.Add(new Tbl_OrderJsonMap());
            modelBuilder.Configurations.Add(new Tbl_CommentMap());

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
            Database.SetInitializer<ExnShippmentObjectContext>(null);
            Database.ExecuteSqlCommand(CreateDatabaseInstallationScript());
            SaveChanges();
        }

        public void Uninstall()
        {
            var dbScript = "IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[BarcodeRepository]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1) DROP TABLE XtnShippment";
            Database.ExecuteSqlCommand(dbScript);
            dbScript = "IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FirstOrder]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1) DROP TABLE FirstOrder";
            Database.ExecuteSqlCommand(dbScript);
            SaveChanges();
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

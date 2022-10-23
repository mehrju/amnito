using Nop.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class ShippingSolutionsObjectContext : DbContext, IDbContext

    {
        public ShippingSolutionsObjectContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        public bool ProxyCreationEnabled
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
        public bool AutoDetectChangesEnabled
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


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<ShippingSolutionsObjectContext>(null);
            modelBuilder.Configurations.Add(new Tbl_CollectorsMap());
            modelBuilder.Configurations.Add(new Tbl_CollectoreStoresMap());
            modelBuilder.Configurations.Add(new Tbl_OfficesMap());
            modelBuilder.Configurations.Add(new Tbl_ServicesProvidersMap());
            modelBuilder.Configurations.Add(new Tbl_ServicesTypesMap());
            modelBuilder.Configurations.Add(new Tbl_ServiceTypesProviderMap());
            modelBuilder.Configurations.Add(new Tbl_DealerMap());
            modelBuilder.Configurations.Add(new Tbl_WorkingTimeMap());
            modelBuilder.Configurations.Add(new Tbl_ProviderStoresMap());
            modelBuilder.Configurations.Add(new Tbl_CollectorsServiceProviderMap());
            modelBuilder.Configurations.Add(new Tbl_Dealer_Customer_ServiceProviderMap());
            modelBuilder.Configurations.Add(new Tbl_PricingPolicyMap());
            modelBuilder.Configurations.Add(new Tbl_PatternPricingPolicyMap());
            modelBuilder.Configurations.Add(new Tbl_CarriersTarofMap());
            modelBuilder.Configurations.Add(new Tbl_OstantarofMap());
            modelBuilder.Configurations.Add(new Tbl_CityTarofMap());
            modelBuilder.Configurations.Add(new Tbl_PaymentMethodsTarofMap());
            modelBuilder.Configurations.Add(new Tbl_Address_LatLongMap());
            modelBuilder.Configurations.Add(new Tbl_Product_PatternPricingMap());
            modelBuilder.Configurations.Add(new Tbl_CountryISO3166Map());
            modelBuilder.Configurations.Add(new Tbl_ParcelTypeSkyBlueMap());
            modelBuilder.Configurations.Add(new Tbl_DiscountPlan_AgentCustomerMap());
            modelBuilder.Configurations.Add(new Tbl_SalesPartnersPercentMap());
            modelBuilder.Configurations.Add(new Tbl_RelationOstanCityVijePostMap());
            modelBuilder.Configurations.Add(new Tbl_IncentivePlanCustomerMap());
            modelBuilder.Configurations.Add(new Tbl_AffiliateToCustomerMap());








            base.OnModelCreating(modelBuilder);
        }

        public string CreateDatabaseInstallationScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public void Install()
        {
            Database.SetInitializer<ShippingSolutionsObjectContext>(null);
            Database.ExecuteSqlCommand(CreateDatabaseInstallationScript());
            SaveChanges();
        }

        public void Uninstall()
        {
            //this.DropPluginTable("Tbl_Collectors");
            //this.DropPluginTable("Tbl_CollectoreStores");
            //this.DropPluginTable("Tbl_Offices");
            //this.DropPluginTable("Tbl_ServicesProviders");
            //this.DropPluginTable("Tbl_ServicesTypes");
            //this.DropPluginTable("Tbl_ServiceTypesProvider");
            //this.DropPluginTable("Tbl_Dealer");
            //this.DropPluginTable("Tbl_WorkingTime");
            //this.DropPluginTable("Tbl_ProviderStores");
            //this.DropPluginTable("Tbl_CollectorsServiceProvider");
            //this.DropPluginTable("Tbl_Dealer_Customer_ServiceProvider");
            //this.DropPluginTable("Tbl_PricingPolicy");


        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : Core.BaseEntity
        {
            return base.Set<TEntity>();
        }


        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : Core.BaseEntity, new()
        {
            throw new NotImplementedException();
        }


        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Detach(object entity)
        {
            throw new NotImplementedException();
        }
    }
}

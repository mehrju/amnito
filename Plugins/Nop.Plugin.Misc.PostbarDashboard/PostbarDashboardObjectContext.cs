using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.PostbarDashboard.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard
{
    public class PostbarDashboardObjectContext : DbContext, IDbContext
    {
        public PostbarDashboardObjectContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

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
            Database.SetInitializer<PostbarDashboardObjectContext>(null);
            modelBuilder.Configurations.Add(new Tbl_CheckAvatarCustomerMap());
            modelBuilder.Configurations.Add(new Tbl_ViewVideoCustomerMAP());
            modelBuilder.Configurations.Add(new Tbl_ServiceProviderDashboardMap());
            modelBuilder.Configurations.Add(new Tbl_Carousel_slideshowMap());
            modelBuilder.Configurations.Add(new Tbl_CODRequestLogMap());




            base.OnModelCreating(modelBuilder);
        }

        public string CreateDatabaseInstallationScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public void Install()
        {
            Database.SetInitializer<PostbarDashboardObjectContext>(null);
            Database.ExecuteSqlCommand(CreateDatabaseInstallationScript());
            SaveChanges();
        }

        public void Uninstall()
        {
            //this.DropPluginTable("Tbl_CheckAvatarCustomer");


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

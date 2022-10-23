using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Data
{
    public class TicketObjectContext : DbContext, IDbContext
    {
        public TicketObjectContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

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
            Database.SetInitializer<TicketObjectContext>(null);
            modelBuilder.Configurations.Add(new Tbl_Ticket_DepartmentMap());
            modelBuilder.Configurations.Add(new Tbl_Ticket_PriorityMap());
            modelBuilder.Configurations.Add(new Tbl_Ticket_Staff_DepartmentMap());
            modelBuilder.Configurations.Add(new Tbl_TicketMap());
            modelBuilder.Configurations.Add(new Tbl_Ticket_DetailMap());
            modelBuilder.Configurations.Add(new Tbl_FAQCategoryMap());
            modelBuilder.Configurations.Add(new Tbl_FAQMap());
            modelBuilder.Configurations.Add(new Tbl_PatternAnswer_Ticket_Damages_RequestCODMAP());
            modelBuilder.Configurations.Add(new Tbl_CategoryTicketMAP());
            modelBuilder.Configurations.Add(new Tbl_TrainingAcademyTopicMAP());
            modelBuilder.Configurations.Add(new Tbl_DamagesMap());
            modelBuilder.Configurations.Add(new Tbl_Damages_DetailMap());
            modelBuilder.Configurations.Add(new Tbl_LogSMSMap());
            modelBuilder.Configurations.Add(new Tbl_RequestCODCustomerMap());






            base.OnModelCreating(modelBuilder);
        }

        public string CreateDatabaseInstallationScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public void Install()
        {
            Database.SetInitializer<TicketObjectContext>(null);
            Database.ExecuteSqlCommand(CreateDatabaseInstallationScript());
            SaveChanges();
        }

        public void Uninstall()
        {
            //this.DropPluginTable("Tbl_Collectors");


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

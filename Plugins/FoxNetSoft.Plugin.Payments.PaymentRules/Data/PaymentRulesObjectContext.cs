using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Text;
using FoxNetSoft.Plugin.Payments.PaymentRules.Logger;
using Nop.Core;
using Nop.Data;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Data
{
    public class PaymentRulesObjectContext : DbContext, IDbContext
    {
        public PaymentRulesObjectContext(string nameOrConnectionString): base(nameOrConnectionString) { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<PaymentRulesObjectContext>(null);

            modelBuilder.Configurations.Add(new PaymentRuleMap());
            modelBuilder.Configurations.Add(new PaymentRuleRequirementMap());
            base.OnModelCreating(modelBuilder);
        }
        public string CreateDatabaseInstallationScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }
        public void Install()
        {
            DropTables();
            Database.ExecuteSqlCommand(CreateDatabaseInstallationScript(), Array.Empty<object>());
            method_0();
            SaveChanges();
        }
        public void Uninstall()
        {
            DropTables();
            unIstallPaymentRules();
            SaveChanges();
        }

        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null,
            params object[] parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Detach an entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Detach(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            ((IObjectContextAdapter) this).ObjectContext.Detach(entity);
        }

        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : BaseEntity, new()
        {
            throw new NotImplementedException();
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }



        private void method_0()
        {
            var str = string.Concat(PluginLog.Folder, "Install/");
            var strs = new List<string>();
            strs.AddRange(ParseCommands(CommonHelper.MapPath(string.Concat(str, "SqlServer.PaymentRules.sql")), false));
            ExecuteSqlCommandExt(strs.ToArray());
        }

        private void unIstallPaymentRules()
        {
            var str = string.Concat(PluginLog.Folder, "Install/");
            var strs = new List<string>();
            strs.AddRange(
                ParseCommands(CommonHelper.MapPath(string.Concat(str, "UnSqlServer.PaymentRules.sql")), false));
            ExecuteSqlCommandExt(strs.ToArray());
        }

        private void ExecuteSqlCommandExt(string[] string_0)
        {
            if (string_0 != null && string_0.Length != 0)
            {
                var string0 = string_0;
                for (var i = 0; i < string0.Length; i++)
                {
                    var str = string0[i];
                    if (!string.IsNullOrWhiteSpace(str)) Database.ExecuteSqlCommand(str);
                }
            }
        }


        protected virtual string[] ParseCommands(string filePath, bool throwExceptionIfNonExists)
        {
            if (!File.Exists(filePath))
            {
                if (throwExceptionIfNonExists)
                    throw new ArgumentException(string.Format("Specified file doesn't exist - {0}", filePath));
                return new string[0];
            }

            var strs = new List<string>();
            using (var fileStream = File.OpenRead(filePath))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    var str = "";
                    while (true)
                    {
                        var str1 = readNextStatementFromStream(streamReader);
                        str = str1;
                        if (str1 == null) break;
                        strs.Add(str);
                    }
                }
            }

            return strs.ToArray();
        }

        protected virtual string readNextStatementFromStream(StreamReader reader)
        {
            var stringBuilder = new StringBuilder();
            while (true)
            {
                var str = reader.ReadLine();
                if (str == null)
                {
                    if (stringBuilder.Length <= 0) return null;
                    return stringBuilder.ToString();
                }

                if (str.TrimEnd(Array.Empty<char>()).ToUpper() == "GO") break;
                stringBuilder.Append(string.Concat(str, Environment.NewLine));
            }

            return stringBuilder.ToString();
        }

        private void DropTables()
        {
            var str =
                "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'FNS_PaymentRuleRequirement') AND type in (N'U')) DROP TABLE FNS_PaymentRuleRequirement IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'FNS_PaymentRule') AND type in (N'U')) DROP TABLE FNS_PaymentRule delete from LocaleStringResource where ResourceName like '%FoxNetSoft.PaymentRules%'";
            Database.ExecuteSqlCommand(str);
        }


        public void UpdateInstallationScript(int version)
        {
            var strs = new List<string>();
            var str = string.Concat(PluginLog.Folder, "Install/");
            if (version == 111)
                strs.AddRange(ParseCommands(CommonHelper.MapPath(string.Concat(str, "Update100.PaymentRules.sql")),
                    false));
            if (strs.Count > 0) ExecuteSqlCommandExt(strs.ToArray());
        }

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether proxy creation setting is enabled (used in EF)
        /// </summary>
        public virtual bool ProxyCreationEnabled
        {
            get => Configuration.ProxyCreationEnabled;
            set => Configuration.ProxyCreationEnabled = value;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether auto detect changes setting is enabled (used in EF)
        /// </summary>
        public virtual bool AutoDetectChangesEnabled
        {
            get => Configuration.AutoDetectChangesEnabled;
            set => Configuration.AutoDetectChangesEnabled = value;
        }

        #endregion
    }
}
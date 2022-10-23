using System;
using System.Data.Entity.ModelConfiguration;
using System.Linq.Expressions;
using System.Reflection;
using FoxNetSoft.Plugin.Payments.PaymentRules.Domain;
using Nop.Core;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Data
{
    public class PaymentRuleMap : EntityTypeConfiguration<PaymentRule>
    {
        public PaymentRuleMap()
        {
            this.ToTable("FNS_PaymentRule", "dbo");
            HasKey(p => p.Id);
            Property(m => m.Name).HasMaxLength(new int?(400));
            Property(m => m.Payments).HasMaxLength(new int?(2000));
            Property(m => m.AdminComment).HasMaxLength(new int?(1000));
            Property(m => m.DisplayOrder);
            Property(m => m.EndDateTimeUtc);
            Property(m => m.IsActive);
            Property(m => m.LimitedToStores);
            Property(m => m.StartDateTimeUtc);
            Property(m => m.SubjectToAcl);
        }

    }
}
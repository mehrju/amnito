using System.Data.Entity.ModelConfiguration;
using FoxNetSoft.Plugin.Payments.PaymentRules.Domain;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Data
{
    public class PaymentRuleRequirementMap : EntityTypeConfiguration<PaymentRuleRequirement>
    {
        public PaymentRuleRequirementMap()
        {
            ToTable("FNS_PaymentRuleRequirement", "dbo");
            HasKey(p => p.Id);
            HasRequired(p => p.PaymentRule).WithMany(p => p.Requirements).HasForeignKey(p => p.PaymentRuleId);
            HasMany(p => p.ChildRequirements).WithOptional().HasForeignKey(p => p.ParentId);
            // Property(p => p.PaymentRuleId);
            // Property(p => p.ParentId);
            Property(p => p.InteractionTypeId);
            Property(p => p.IsGroup);
            Property(p => p.RequirementCategory);
            Property(p => p.RequirementOperator);
            Property(p => p.RequirementProperty);
            Property(p => p.RequirementValue).HasMaxLength(400);
        }
    }
}
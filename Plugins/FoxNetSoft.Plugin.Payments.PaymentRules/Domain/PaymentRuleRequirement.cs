using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Core;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Domain
{
    public class PaymentRuleRequirement : BaseEntity
    {
        private ICollection<PaymentRuleRequirement> icollection_0;

        public virtual ICollection<PaymentRuleRequirement> ChildRequirements
        {
            get
            {
                var icollection0 = icollection_0;
                if (icollection0 == null)
                {
                    var paymentRuleRequirements = new List<PaymentRuleRequirement>();
                    ICollection<PaymentRuleRequirement> paymentRuleRequirements1 = paymentRuleRequirements;
                    icollection_0 = paymentRuleRequirements;
                    icollection0 = paymentRuleRequirements1;
                }

                return icollection0;
            }
            protected set => icollection_0 = value;
        }

        public GroupInteractionType? InteractionType
        {
            get
            {
                var interactionTypeId = InteractionTypeId;
                if (!interactionTypeId.HasValue)
                    return new GroupInteractionType?();
                return (GroupInteractionType) interactionTypeId.GetValueOrDefault();
            }
            set => InteractionTypeId = (int?) value;
        }

        public int? InteractionTypeId { get; set; }

        public bool IsGroup { get; set; }

        public int? ParentId { get; set; }

        [JsonIgnore] public virtual PaymentRule PaymentRule { get; set; }

        public int PaymentRuleId { get; set; }

        public string RequirementCategory { get; set; }

        public string RequirementOperator { get; set; }

        public string RequirementProperty { get; set; }

        public string RequirementValue { get; set; }
    }
}
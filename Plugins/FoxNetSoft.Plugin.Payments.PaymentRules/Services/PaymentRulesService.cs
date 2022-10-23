using System;
using System.Collections.Generic;
using System.Linq;
using FoxNetSoft.Plugin.Payments.PaymentRules.Domain;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Services.Events;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Services
{
    public class PaymentRulesService : IPaymentRulesService
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<PaymentRule> _repositoryPaymentRule;
        private readonly IRepository<PaymentRuleRequirement> _repositoryPaymentRuleRequirement;
        private readonly IStaticCacheManager _staticCacheManager;

        public PaymentRulesService(IRepository<PaymentRule> paymentRuleMapRepository
            , IRepository<PaymentRuleRequirement> paymentRuleRequirementMapRepository
            , IEventPublisher eventPublisher
            , IStaticCacheManager cacheManager)
        {
            _repositoryPaymentRule = paymentRuleMapRepository;
            _repositoryPaymentRuleRequirement = paymentRuleRequirementMapRepository;
            _eventPublisher = eventPublisher;
            _staticCacheManager = cacheManager;
        }

        public virtual void DeletePaymentRule(PaymentRule paymentRule)
        {
            if (paymentRule == null) throw new ArgumentNullException(nameof(paymentRule));
            _repositoryPaymentRule.Delete(paymentRule);
            _staticCacheManager.RemoveByPattern("FoxNetSoft.PaymentRules.");
            _eventPublisher.EntityDeleted(paymentRule);
        }

        public virtual void DeletePaymentRuleRequirement(PaymentRuleRequirement paymentRuleRequirement)
        {
            if (paymentRuleRequirement == null) throw new ArgumentNullException(nameof(paymentRuleRequirement));
            _repositoryPaymentRuleRequirement.Delete(paymentRuleRequirement);
            _staticCacheManager.RemoveByPattern("FoxNetSoft.PaymentRules.");
            _eventPublisher.EntityDeleted(paymentRuleRequirement);
        }

        public virtual IList<PaymentRule> GetAllPaymentRules(bool showHidden = false)
        {
            var table = _repositoryPaymentRule.Table;
            if (!showHidden)
                table =
                    from c in table
                    where c.IsActive
                    select c;
            table =
                from c in table
                orderby c.DisplayOrder
                select c;
            return table.ToList();
        }

        public virtual PaymentRule GetPaymentRuleById(int paymentRuleId)
        {
            if (paymentRuleId == 0) return null;
            return _repositoryPaymentRule.GetById(paymentRuleId);
        }

        public virtual PaymentRuleRequirement GetPaymentRuleRequirementById(int paymentRuleRequirementId)
        {
            if (paymentRuleRequirementId == 0) return null;
            return _repositoryPaymentRuleRequirement.GetById(paymentRuleRequirementId);
        }

        public virtual IList<PaymentRuleRequirement> GetRequirementsByPaymentRuleId(int paymentRuleId,
            bool topLevelOnly = false)
        {
            if (paymentRuleId == 0)
                return new List<PaymentRuleRequirement>();
            var source = _repositoryPaymentRuleRequirement.Table;
            if (paymentRuleId > 0)
                source = source.Where(requirement => requirement.PaymentRuleId == paymentRuleId);
            if (topLevelOnly)
                source = source.Where(requirement => !requirement.ParentId.HasValue);
            return source.OrderBy(p => p.Id).ToList();
        }

        public virtual void InsertPaymentRule(PaymentRule paymentRule)
        {
            if (paymentRule == null) throw new ArgumentNullException("paymentRule");
            _repositoryPaymentRule.Insert(paymentRule);
            _staticCacheManager.RemoveByPattern("FoxNetSoft.PaymentRules.");
            _eventPublisher.EntityInserted(paymentRule);
        }

        public virtual void InsertPaymentRuleRequirement(PaymentRuleRequirement paymentRuleRequirement)
        {
            if (paymentRuleRequirement == null) throw new ArgumentNullException("paymentRuleRequirement");
            _repositoryPaymentRuleRequirement.Insert(paymentRuleRequirement);
            _staticCacheManager.RemoveByPattern("FoxNetSoft.PaymentRules.");
            _eventPublisher.EntityInserted(paymentRuleRequirement);
        }

        public virtual void UpdatePaymentRule(PaymentRule paymentRule)
        {
            if (paymentRule == null) throw new ArgumentNullException("paymentRule");
            _repositoryPaymentRule.Update(paymentRule);
            _staticCacheManager.RemoveByPattern("FoxNetSoft.PaymentRules.");
            _eventPublisher.EntityUpdated(paymentRule);
        }

        public virtual void UpdatePaymentRuleRequirement(PaymentRuleRequirement paymentRuleRequirement)
        {
            if (paymentRuleRequirement == null) throw new ArgumentNullException("paymentRuleRequirement");
            _repositoryPaymentRuleRequirement.Update(paymentRuleRequirement);
            _staticCacheManager.RemoveByPattern("FoxNetSoft.PaymentRules.");
            _eventPublisher.EntityUpdated(paymentRuleRequirement);
        }
    }
}
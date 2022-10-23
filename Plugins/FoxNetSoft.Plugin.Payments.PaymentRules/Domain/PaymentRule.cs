using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Domain
{
	[Serializable]
	public class PaymentRule : BaseEntity, IStoreMappingSupported, IAclSupported
	{
		private ICollection<PaymentRuleRequirement> icollection_0;

		public string AdminComment
		{
			get;
			set;
		}

		public int DisplayOrder
		{
			get;
			set;
		}

		public DateTime? EndDateTimeUtc
		{
			get;
			set;
		}

		public bool IsActive
		{
			get;
			set;
		}

		public bool LimitedToStores
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string Payments
		{
			get;
			set;
		}

		[JsonIgnore]
		public virtual ICollection<PaymentRuleRequirement> Requirements
		{
			get
			{
				ICollection<PaymentRuleRequirement> icollection0 = this.icollection_0;
				if (icollection0 == null)
				{
					List<PaymentRuleRequirement> paymentRuleRequirements = new List<PaymentRuleRequirement>();
					ICollection<PaymentRuleRequirement> paymentRuleRequirements1 = paymentRuleRequirements;
					this.icollection_0 = paymentRuleRequirements;
					icollection0 = paymentRuleRequirements1;
				}
				return icollection0;
			}
			protected set
			{
				this.icollection_0 = value;
			}
		}

		public DateTime? StartDateTimeUtc
		{
			get;
			set;
		}

		public bool SubjectToAcl
		{
			get;
			set;
		}

		public PaymentRule()
		{
			
		}
	}
}
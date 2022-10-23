using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Models
{
	public class PaymentRuleModel : BaseNopEntityModel
	{
		[NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.Requirements.PaymentRequirementType")]
		public string AddRequirement
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.PaymentRule.Fields.AdminComment")]
		public string AdminComment
		{
			get;
			set;
		}

		public IList<SelectListItem> AvailableCustomerRoles
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.PaymentRule.Fields.AvailablePaymentMethods")]
		public IList<SelectListItem> AvailablePaymentMethods
		{
			get;
			set;
		}

		public IList<SelectListItem> AvailableRequirementGroups
		{
			get;
			set;
		}

		public IList<SelectListItem> AvailableRequirements
		{
			get;
			set;
		}

		public IList<SelectListItem> AvailableStores
		{
			get;
			set;
		}

		public string[] CheckedPaymentMethods
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.PaymentRule.Fields.DisplayOrder")]
		public int DisplayOrder
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.PaymentRule.Fields.EndDateTimeUtc")]
		[UIHint("DateTimeNullable")]
		public DateTime? EndDateTimeUtc
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.Requirements.GroupName")]
		public string GroupName
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.PaymentRule.Fields.IsActive")]
		public bool IsActive
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.PaymentRule.Fields.Name")]
		public string Name
		{
			get;
			set;
		}

		public string PaymentMethodsHTML
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.Requirements.RequirementGroup")]
		public int RequirementGroupId
		{
			get;
			set;
		}

		public string RequirementsHTML
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.PaymentRule.Fields.AclCustomerRoles")]
		[UIHint("MultiSelect")]
		public IList<int> SelectedCustomerRoleIds
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.PaymentRule.Fields.LimitedToStores")]
		[UIHint("MultiSelect")]
		public IList<int> SelectedStoreIds
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.PaymentRule.Fields.StartDateTimeUtc")]
		[UIHint("DateTimeNullable")]
		public DateTime? StartDateTimeUtc
		{
			get;
			set;
		}

		public PaymentRuleModel()
		{
			
			this.SelectedCustomerRoleIds = new List<int>();
			this.AvailableCustomerRoles = new List<SelectListItem>();
			this.SelectedStoreIds = new List<int>();
			this.AvailableStores = new List<SelectListItem>();
			this.AvailableRequirements = new List<SelectListItem>();
			this.AvailablePaymentMethods = new List<SelectListItem>();
			this.AvailableRequirementGroups = new List<SelectListItem>();
		}

		public class RequirementMetaInfo : BaseNopModel
		{
			public SelectList AvailableInteractionTypes
			{
				get;
				set;
			}

			public IList<PaymentRuleModel.RequirementMetaInfo> ChildRequirements
			{
				get;
				set;
			}

			public string ConfigurationUrl
			{
				get;
				set;
			}

			public int InteractionTypeId
			{
				get;
				set;
			}

			public bool IsGroup
			{
				get;
				set;
			}

			public bool IsLastInGroup
			{
				get;
				set;
			}

			public int? ParentId
			{
				get;
				set;
			}

			public int RequirementId
			{
				get;
				set;
			}

			public string RequirementName
			{
				get;
				set;
			}

			public RequirementMetaInfo()
			{
				this.ChildRequirements = new List<PaymentRuleModel.RequirementMetaInfo>();
			}
		}
	}
}
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Models
{
	public class RequirementModel : BaseNopModel
	{
		public IList<SelectListItem> AvailableOperators
		{
			get;
			set;
		}

		public IList<SelectListItem> AvailableProperties
		{
			get;
			set;
		}

		public IList<SelectListItem> AvailableValues
		{
			get;
			set;
		}

		public int ExternalId
		{
			get;
			set;
		}

		public bool IsValueComboBox
		{
			get;
			set;
		}

		public string RequirementCategory
		{
			get;
			set;
		}

		public int RequirementId
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.Requirements.Operator")]
		public string RequirementOperator
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.Requirements.Property")]
		public string RequirementProperty
		{
			get;
			set;
		}

		[NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.Requirements.Value")]
		public string RequirementValue
		{
			get;
			set;
		}

		public RequirementModel()
		{
			
			this.AvailableProperties = new List<SelectListItem>();
			this.AvailableOperators = new List<SelectListItem>();
			this.AvailableValues = new List<SelectListItem>();
		}
	}
}
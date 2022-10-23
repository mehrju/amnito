using System;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Payments.Mellat.Models
{
	public class ConfigurationModel : BaseNopModel
	{
		[NopResourceDisplayName("Plugins.Payments.Mellat.Fields.OrderId")]
		public int OrderId
		{
			get;
			set;
		}

		public bool OrderId_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("Plugins.Payments.Mellat.Fields.TerminalId")]
		public string TerminalId
		{
			get;
			set;
		}

		public bool TerminalId_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("Plugins.Payments.Mellat.Fields.UserName")]
		public string UserName
		{
			get;
			set;
		}

		public bool UserName_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("Plugins.Payments.Mellat.Fields.UserPassword")]
		public string UserPassword
		{
			get;
			set;
		}

		public bool UserPassword_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("Plugins.Payments.Mellat.Fields.ActivationCode")]
		public string ActivationCode
		{
			get;
			set;
		}

		public bool ActivationCode_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("Plugins.Payments.Mellat.Fields.SystemCode")]
		public string SystemCode
		{
			get;
			set;
		}

		public bool SystemCode_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("Plugins.Payments.Mellat.Fields.Toman")]
		public bool Toman
		{
			get;
			set;
		}

		public bool Toman_OverrideForStore
		{
			get;
			set;
		}

		

		public bool BusinessEmail_OverrideForStore
		{
			get;
			set;
		}

		public int ActiveStoreScopeConfiguration
		{
			get;
			set;
		}

		[NopResourceDisplayName("Plugins.Payments.Mellat.Fields.AdditionalFee")]
		public decimal AdditionalFee
		{
			get;
			set;
		}

		public bool AdditionalFee_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("Plugins.Payments.Mellat.Fields.AdditionalFeePercentage")]
		public bool AdditionalFeePercentage
		{
			get;
			set;
		}

		public bool AdditionalFeePercentage_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("Plugins.Payments.Mellat.Fields.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage")]
		public bool ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage
		{
			get;
			set;
		}

		public bool ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore
		{
			get;
			set;
		}
	}
}

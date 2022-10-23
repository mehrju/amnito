using Nop.Core.Configuration;
using System;
using System.Runtime.CompilerServices;

namespace FoxNetSoft.Plugin.Payments.PaymentRules
{
	public class PaymentRulesSettings : ISettings
	{
		public bool Enabled
		{
			get;
			set;
		}

		public string SerialNumber
		{
			get;
			set;
		}

		public bool showDebugInfo
		{
			get;
			set;
		}

		public int Version
		{
			get;
			set;
		}

		public PaymentRulesSettings()
		{
		}
	}
}
using System;
using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Mellat
{
	public class MellatPaymentSettings : ISettings
	{
		public string TerminalId
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public string UserPassword
		{
			get;
			set;
		}

        //public int OrderId
        //{
        //    get;
        //    set;
        //}

		public bool Toman
		{
			get;
			set;
		}

		public bool AdditionalFeePercentage
		{
			get;
			set;
		}

		public decimal AdditionalFee
		{
			get;
			set;
		}

		public bool ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage
		{
			get;
			set;
		}
	}
}

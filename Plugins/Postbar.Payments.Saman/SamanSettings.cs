using System;

using Nop.Core.Configuration;

namespace NopFarsi.Payments.SepShaparak
{
	
	public class sepsettings : ISettings
	{
		public int MerchantId { get; set; }

        public string PayementUrl
        {
            get { return "https://sep.shaparak.ir/payment.aspx"; }
        }
       
        public bool IsToman { get; set; }

        public bool DisablePaymentInfo { get; set; }

        public string RefundUserName { get; set; }

        public string RefundPassword { get; set; }

        public string RefundEmail { get; set; }

        public string RefundCellPhone { get; set; }

        public long TransactionTermId { get; set; }

        public long RefundTermId { get; set; }

    }
}
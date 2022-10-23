using System;

using Nop.Core.Configuration;

namespace NopFarsi.Payments.AsanPardakht
{
	
	public class AsanPardakhtSettings : ISettings
	{
		public int MerchantId
		{
			get;
			set;
		}
        public int ConfigMerchentId { get; set; }
        public string UserNameMerchent { get; set; }
        public string PassMerchent { get; set; }
        public string Key { get; set; }
        public string VectorEncriptor { get; set; }

        public string PayementUrl
        {
            get { return "https://sep.shaparak.ir/payment.aspx"; }
            //set;
        }

        
        public string WebServiceUrl
        {
            get { return ""; }

            //set;
        }

        public bool IsToman
        {
            get;
            set;
        }

        public bool DisablePaymentInfo { get; set; }
        
	}
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrotelServiceLibrary.Input
{
    public class CheckPayInput : BaseInput
    {
        //  در هنگام برگشت خریدار از درگاه بانکی دو پارامتر _i و sb نیز به آدرس callback شما پاس داده می شود که برای بررسی پرداخت نیاز به این پارامتر ها دارید.

        [JsonProperty("paymentId")]
        public int PaymentId { get; set; } // شناسه پرداخت (پارامتر _i)

        [JsonProperty("ref")]
        public string Ref { get; set; } // کد ارجاع(پارامتر sb)	

        [JsonProperty("factor")]
        public string FactorId { get; set; }

    }
}

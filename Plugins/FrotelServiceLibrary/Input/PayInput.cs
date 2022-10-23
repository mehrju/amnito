using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrotelServiceLibrary.Input
{
    public class PayInput : BaseInput
    {
        [JsonProperty("factor")]
        public int FactorId { get; set; }

        [JsonProperty("bank")]
        public int BankId { get; set; }

        [JsonProperty("callback")]
        public string CallbackUrl { get; set; } //آدرس برگشت از سايت بانک به سايت فروشنده	

    }
}
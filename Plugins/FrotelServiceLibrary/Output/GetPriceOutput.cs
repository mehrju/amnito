using FrotelServiceLibrary.Enum;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FrotelServiceLibrary.Output
{
    public class GetPricesOutput : BaseOutput
    {
        public GetPricesOutput()
        {
            Result = new List<GetPricesOutputResult>();
        }

        public List<GetPricesOutputResult> Result { get; set; }
    }

    public class GetPricesOutputResult
    {
        public SendType SendType { get; set; }

        public BuyType BuyType { get; set; }

        [JsonProperty("post")]
        public int PostPrice { get; set; } // هزینه ارسال

        [JsonProperty("tax")]
        public int TaxPrice { get; set; } // مالیات ارسال

        [JsonProperty("frotel_service")]
        public int FrotelServicePrice { get; set; } // هزینه خدمات فروتل

        [JsonProperty("packing")]
        public int PackingPrice { get; set; }

        public int CalcTotalPrice()
        {
            return PostPrice + TaxPrice + FrotelServicePrice + PackingPrice;
        }
    }
}
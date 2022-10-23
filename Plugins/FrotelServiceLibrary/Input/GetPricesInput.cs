using FrotelServiceLibrary.Enum;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FrotelServiceLibrary.Input
{
    public class GetPricesInput : BaseInput
    {
        [JsonProperty("price")]
        public int Price { get; set; } //جمع کل هزینه سفارش به ریال	

        [JsonProperty("weight")]
        public int Weight { get; set; } //وزن کل سفارش به گرم	

        [JsonProperty("des_city")]
        public int DestinationCityId { get; set; } //شناسه شهر مقصد	

        [JsonProperty("send_type")]
        public List<SendType> SendTypes { get; set; } //لیست روش های ارسال

        [JsonProperty("buy_type")]
        public List<BuyType> BuyTypes { get; set; } //لیست روش های پرداخت
    }
}
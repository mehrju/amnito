using FrotelServiceLibrary.Enum;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FrotelServiceLibrary.Output
{
    public class RegisterOrderOutput : BaseOutput
    {
        [JsonProperty("result")]
        public RegisterOrderOutputResult Result { get; set; }
    }

    public class RegisterOrderOutputResult
    {
        [JsonProperty("factor")]
        public Factor Factro { get; set; }

        [JsonProperty("items")]
        public Items[] Items { get; set; }

    }

    public class Factor
    {
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// کدهای HTML فاکتور
        /// </summary>
        [JsonProperty("view")]
        public string View { get; set; }

        /// <summary>
        /// جمع کل سفارش
        /// </summary>
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("banks")]
        public Bank[] Banks { get; set; } // اگر پرداخت نقدی باشد این مقدار دارد
    }

    public class Items
    {
        [JsonProperty("product_id")]
        public string ProductId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("num")]
        public int Num { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("pro_code")]
        public string ProCode { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }

        [JsonProperty("option")]
        public string Option { get; set; }
    }

    public class Bank
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

    }
}
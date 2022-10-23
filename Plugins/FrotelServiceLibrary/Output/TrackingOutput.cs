using FrotelServiceLibrary.Enum;
using Newtonsoft.Json;

namespace FrotelServiceLibrary.Output
{
    public class TrackingOutput : BaseOutput
    {
        [JsonProperty("result")]
        public TrackingOutputResult Result { get; set; }
    }

    public class TrackingOutputResult
    {
        /// <summary>
        /// اطلاعات خریدار
        /// </summary>
        [JsonProperty("customer")]
        public string Customer { get; set; }


        [JsonProperty("order")]
        public Order Order { get; set; }
    }

    public class Order
    {
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        [JsonProperty("factor")]
        public string FactorId { get; set; }

        /// <summary>
        /// بارکد رهگیری پست
        /// </summary>
        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        /// <summary>
        /// وضعیت فعلی سفارش
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// تاریخ ثبت سفارش
        /// </summary>
        [JsonProperty("date")]
        public string RegisterOrderDate { get; set; }

        /// <summary>
        /// تاریخ آخرین تغییر وضعیت
        /// </summary>
        [JsonProperty("change_date")]
        public string LastChangeStatusDate { get; set; }

        /// <summary>
        /// جمع کل سفارش
        /// </summary>
        [JsonProperty("price")]
        public int Price { get; set; }

        /// <summary>
        /// نوع پرداخت
        /// </summary>
        [JsonProperty("buy_type")]
        public TrackingBuyType TrackingBuyType { get; set; }

        /// <summary>
        /// تاریخ آخرین تغییر وضعیت
        /// </summary>
        [JsonProperty("desc")]
        public string Description { get; set; }

        /// <summary>
        /// نوع سفارش
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; set; }
    }
}
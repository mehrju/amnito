using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Snappbox
{
    /// <summary>
    /// مدل ورودی استعلام قیمت
    /// <para> تمامی موارد زیر اجباری هستند</para>
    /// <para>وزن کالا برای موتور سیکلت باید حداکثر ۲۵ کیلوگرم و حداکثر ابعاد ۴۵*۴۵*۴۵ باشد.</para>
    /// <para>Customer Id از تنظیمات تکمیل میگردد</para>
    /// <para>1Params_Snappbox_Get_Price کلیه فیلد های کلاس </para>
    /// <para>deliveryCategory:'bike' | 'bike-without-box' | 'van' | 'van-heavy' | 'carbox' | 'passenger</para>
    /// <para>deliveryFarePaymentType: 'prepaid' | 'cod' پیش پرداخت یا امانی</para>
    /// <para>isReturn: false | true</para>
    /// <para>sequenceNumberDeliveryCollection: 1 | 2، یک یعنی مبدا دو مقصد</para>
    /// <para>مثال</para>
    /// { 
    ///  "city":"tehran",
    ///  "deliveryCategory":"bike",
    ///  "deliveryFarePaymentType":"cod",
    ///  "isReturn":false,
    ///  "pricingId":null,
    ///  "sequenceNumberDeliveryCollection":1,
    ///  "totalFare":null,
    ///  "voucherCode":null,
    ///  "waitingTime":0,
    ///  "customerWalletType":null,
    ///  "terminals":[
    ///     { 
    ///        "id":null,
    ///        "contactName":"نجمه اسماعیلی",
    ///        "address":"جردن - پارک ملت، سید رضا سعیدی، بعد از مهرداد، اسنپ تریپ",
    ///        "contactPhoneNumber":"09355270322",
    ///        "plate":"",
    ///        "sequenceNumber":1,
    ///        "unit":"",
    ///        "comment":"",
    ///        "latitude":35.77325759103725,
    ///        "longitude":51.418590545654304,
    ///        "type":"pickup",
    ///        "collectCash":"no",
    ///        "paymentType":"prepaid",
    ///        "cashOnPickup":0,
    ///        "cashOnDelivery":0,
    ///        "isHub":null,
    ///        "vendorId":null
    ///     },
    ///     { 
    ///        "id":null,
    ///        "contactName":"نجمه اسماعیلی",
    ///        "address":"دبستان - مجیدیه، حسین زاده، تقاطع ظفر",
    ///        "contactPhoneNumber":"02122391452",
    ///        "plate":"",
    ///        "sequenceNumber":2,
    ///        "unit":"",
    ///        "comment":"",
    ///        "latitude":35.74188663727774,
    ///        "longitude":51.45718002633658,
    ///        "type":"drop",
    ///        "collectCash":"no",
    ///        "paymentType":"prepaid",
    ///        "cashOnPickup":0,
    ///        "cashOnDelivery":0,
    ///        "isHub":null,
    ///        "vendorId":null
    ///     }
    ///  ],
    ///  "id":null,
    ///  "customerId":"2074188"
    ///
    /// </summary>
    /// 
    public class Params_Snappbox_Get_Price
    {
        public string city { get; set; }
        public string deliveryCategory { get; set; }
        public string deliveryFarePaymentType { get; set; }
        public bool isReturn { get; set; }
        public object pricingId { get; set; }
        public int sequenceNumberDeliveryCollection { get; set; }
        public object totalFare { get; set; }
        public object voucherCode { get; set; }
        public int waitingTime { get; set; }
        public object customerWalletType { get; set; }
        public List<Terminal> terminals { get; set; }
        public object id { get; set; }
        public string customerId { get; set; }
    }

    /// <summary>
    /// موارد اجباری
    /// <para>type: 'pickup' | 'dropoff</para>
    /// <para>contactName: string</para>
    /// <para>contactPhoneNumber: string</para>
    /// <para>address: string</para>
    /// <para>latitude: number</para>
    /// <para>longitude: number</para>
    /// <para>sequenceNumber: number</para>
    /// <para>paymentType: 'prepaid', // always set to 'prepaid</para>
    /// 
    /// 
    /// </summary>
    public class Terminal
    {
        public object id { get; set; }
        public string contactName { get; set; }
        public string address { get; set; }
        public string contactPhoneNumber { get; set; }
        public string plate { get; set; }
        public int sequenceNumber { get; set; }
        public string unit { get; set; }
        public string comment { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string type { get; set; }
        public string collectCash { get; set; }
        public string paymentType { get; set; }
        public int cashOnPickup { get; set; }
        public int cashOnDelivery { get; set; }
        public object isHub { get; set; }
        public object vendorId { get; set; }
    }
}

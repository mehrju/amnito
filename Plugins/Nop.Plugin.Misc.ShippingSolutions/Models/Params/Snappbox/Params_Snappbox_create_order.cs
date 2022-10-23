using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Snappbox
{
    /// <summary>
    /// مدل اصلی ثبت سفارش
    /// <para>data مدل دیتا که به هر کدام از زیر مجموعهها  مراجعه شود</para>
    /// <para>customerId کد مشتری که از تنظیمات تکمیل میگردد</para>
    /// 
    /// </summary>
    public class Params_Snappbox_create_order
    {
        public Params_Snappbox_create_order()
        {
            data = new Data();
            data.orderDetails = new OrderDetails();
            data.timeSlotDTO = new TimeSlotDTO();
            data.dropOffDetails = new List<DropOffDetail>();
            data.itemDetails = new List<ItemDetail>();
            data.pickUpDetails = new List<PickUpDetail>();
        }

        public Data data { get; set; }
        public string customerId { get; set; }
    }
    /// <summary>
    /// خالی ارسال شود
    ///     این مربوط به ثبت پیش سفارش یا سفارش زمانبندی هست
    /// </summary>
    public class TimeSlotDTO
    {
        public string startTimeSlot { get; set; }
        public string endTimeSlot { get; set; }
    }

    /// <summary>
    /// مدل هر یک از آیتم های سفارش
    /// <para>pickedUpSequenceNumber شماره ترتیب جمع اوری</para>
    /// <para>dropOffSequenceNumber شماره ترتیب تحویل</para>
    /// <para>name نام کالا</para>
    /// <para>quantity تعدد</para>
    /// <para>quantityMeasuringUnit واحد unit,kg</para>
    /// <para>packageValue ارزش کالا</para>
    /// <para>createdAt زمان ثبت کالا</para>
    /// <para>updatedAt زمان ویرایش</para>
    /// <para>2 مورد بالا یک مقدار داشته باشند</para>
    /// 
    /// </summary>
    public class ItemDetail
    {
        public int pickedUpSequenceNumber { get; set; }
        public int dropOffSequenceNumber { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public string quantityMeasuringUnit { get; set; }
        public int packageValue { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
    }

    /// <summary>
    /// مدل جزییات سفارش
    /// <para>city نام شهر براساس لیست ارائه شده در توضیحات سرویس</para>
    /// <para>deliveryCategory نوع وسلیه</para>
    /// <para>deliveryFarePaymentType نوع پرداخت:پیش پرداخت یا...</para>
    /// <para>isReturn ایا برگشت داشته باشد</para>
    /// <para>pricingId کد دریافتی از متد استعلام قیمت میبشاد فعلی خالی بگذار</para>
    /// <para>totalFare مبلغ استعلام قیمت</para>
    /// <para>sequenceNumberDeliveryCollection==1</para>
    /// <para>customerRefId کد سفارش جهت پیگیری</para>
    /// <para>"voucherCode": null,</para>
    /// <para>"waitingTime": 0</para>
    /// 
    /// </summary>
    public class OrderDetails
    {
        public double packageSize { get; set; }
        public string city { get; set; }
        public string deliveryCategory { get; set; }
        public string deliveryFarePaymentType { get; set; }
        public bool isReturn { get; set; }
        public string pricingId { get; set; }
        public int sequenceNumberDeliveryCollection { get; set; }
        public int totalFare { get; set; }
        public string customerRefId { get; set; }
        public object voucherCode { get; set; }
        public int waitingTime { get; set; }
    }

    /// <summary>
    /// مدل برداشت  کالا به مقصد
    /// <para>موارد اجباری</para>
    /// <para>  "contactName": "kiana",</para>
    /// <para>  "address": "تهران، محله شهرک هما",</para>
    /// <para> "contactPhoneNumber": "09203704100",</para>
    /// <para> "sequenceNumber": 1, مبدا هستش</para>
    /// <para> "latitude": 35.74014673099287,</para>
    /// <para> "longitude": 51.33300125598908,</para>
    /// <para>  "type": "pickup", نوع دریافت است</para>
    /// <para> "collectCash": "no",</para>
    /// <para> "paymentType": "prepaid", پیش پرداخت</para>
    /// <para>"cashOnPickup": 0,</para>
    /// <para> "cashOnDelivery": 0,</para>
    /// <para> "isHub": null,</para>
    /// <para>"vendorId": null</para>
    /// 
    /// </summary>
    public class PickUpDetail
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

    /// <summary>
    /// مدل تحویل کالا به مقصد
    /// <para>موارد اجباری</para>
    /// <para>"customerRefId": "YOUR-ORDER-ID",</para>
    /// <para>  "contactName": "kiana",</para>
    /// <para>  "address": "تهران، محله شهرک هما",</para>
    /// <para> "contactPhoneNumber": "09203704100",</para>
    /// <para> "sequenceNumber": 2, مقصد هستش</para>
    /// <para> "latitude": 35.74014673099287,</para>
    /// <para> "longitude": 51.33300125598908,</para>
    /// <para>  "type": "drop", نوع تحویل است</para>
    /// <para> "collectCash": "no",</para>
    /// <para> "paymentType": "prepaid", پیش پرداخت</para>
    /// <para>"cashOnPickup": 0,</para>
    /// <para> "cashOnDelivery": 0,</para>
    /// <para> "isHub": null,</para>
    /// <para>"vendorId": null</para>
    /// 
    /// </summary>
    public class DropOffDetail
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

    /// <summary>
    /// <para>itemDetails لیستی از کالاهای داخل سفارش</para>
    /// <para>orderDetails جزییات سفارش</para>
    /// <para>pickUpDetails لیست مکانهایی که بسته ها باید جمع اوری بشه</para>
    /// <para>dropOffDetails لیستی از نکان های که بسته ها باید تحویل داده بشه</para>
    /// 
    /// </summary>
    public class Data
    {
        public TimeSlotDTO timeSlotDTO { get; set; }
        public List<ItemDetail> itemDetails { get; set; }
        public OrderDetails orderDetails { get; set; }
        public List<PickUpDetail> pickUpDetails { get; set; }
        public List<DropOffDetail> dropOffDetails { get; set; }
    }
}

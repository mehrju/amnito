using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class PostBarcodeGeneratorInputModel
    {
        /// <summary>
        /// کد قرارداد
        /// </summary>
        public int ContractCode { get; set; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// رمز عبور
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// ﮐﺪ رﻫﮕﯿﺮي
        /// </summary>
        [StringLength(24)]
        public string PostNodeCode { get; set; }
        /// <summary>
        /// کد نوع مرسوله با توجه به داکیومنت
        /// </summary>
        public int TypeCode { get; set; }
        /// <summary>
        /// نوع سرویس 
        /// پیشتاز : 1
        /// سفارشی : 2
        /// ویژه : 3
        /// </summary>
        public int? ServiceType { get; set; }
        /// <summary>
        /// نوع مرسوله با توجه به داکیومنت
        /// </summary>
        public int ParcelType { get; set; }
        /// <summary>
        /// کد شهر مبدا
        /// </summary>
        public int SourceCode { get; set; }
        /// <summary>
        /// کد شهر مقصد
        /// </summary>
        public int DestCode { get; set; }
        /// <summary>
        /// نام فرستنده
        /// </summary>
        public string SenderName { get; set; }
        /// <summary>
        /// نام گیرنده
        /// </summary>
        public string ReceiverName { get; set; }
        /// <summary>
        /// کد پستی فرستنده
        /// </summary>
        public string SenderPostalCode { get; set; }
        /// <summary>
        /// کد پستی گیرنده
        /// </summary>
        public string ReceiverPostalCode { get; set; }
        /// <summary>
        /// وزن
        /// </summary>
        public int Weight { get; set; }
        /// <summary>
        /// نوع اصلی مخزن کرایه پستی
        /// با الصاق تمبر : 1
        /// بدون الصاق تمبر : 2
        /// نقش تمبر : 3
        /// </summary>
        public int PostalCostCategoryId { get; set; }
        /// <summary>
        /// نوع زیرگروه مخزن کرایه پستی با توجه به داکیومنت
        /// </summary>
        public string PostalCostTypeFlag { get; set; }
        /// <summary>
        /// تکمیلی
        /// </summary>
        public string Relationalkey { get; set; }
        /// <summary>
        /// کد ملی فرستنده
        /// </summary>
        public string SenderId { get; set; }
        /// <summary>
        /// کد ملی گیرنده
        /// </summary>
        public string ReceiverId { get; set; }
        /// <summary>
        /// موبایل فرستنده
        /// </summary>
        public string SenderMobile { get; set; }
        /// <summary>
        /// موبایل گیرنده
        /// </summary>
        public string ReceiverMobile { get; set; }
        /// <summary>
        /// آدرس فرستنده
        /// </summary>
        public string SenderAddress { get; set; }
        /// <summary>
        /// آدرس گیرند ه
        /// </summary>
        public string ReceiverAddress { get; set; }
        /// <summary>
        /// نوع بیمه  با توجه به داکیومنت
        /// </summary>
        public int InsuranceType { get; set; }
        /// <summary>
        /// مبلغ اضهار شد ه
        /// </summary>
        public long InsuranceAmount { get; set; }
        /// <summary>
        /// کد نوع مقصد مرسولات ویژه
        /// درون استان ی : 1
        /// استانی همجورا : 2
        /// بین استانی : 3
        /// </summary>
        public int SpsDestinationType { get; set; }
        /// <summary>
        /// نوع زمان دریافت مرسولات ویژه با توجه به داکیومنت
        /// </summary>
        public int SpsReceiverTimeType { get; set; }
        /// <summary>
        /// نوع مرسولات ویژ ه
        /// نامه : 1
        /// بسته : 2
        /// </summary>
        public int? SpsParcelType { get; set; }
        /// <summary>
        /// نوع زمان ارسا ل
        /// در ساعت ادار ي : 0
        /// بعد از ساعت ادار ي : 1
        /// شبان : 3
        /// </summary>
        public int TlsServiceType { get; set; }
        /// <summary>
        /// دو قبضه
        /// </summary>
        public bool TwoReceiptant { get; set; }
        /// <summary>
        /// رسید الکترونیکی
        /// </summary>
        public bool ElectroReceiptant { get; set; }
        /// <summary>
        /// کرایه در مقصد
        /// </summary>
        public bool IsCot { get; set; }
        /// <summary>
        /// سرویس پیام ک
        /// </summary>
        public bool? SmsService { get; set; }
        /// <summary>
        /// غیر استاندار د
        /// </summary>
        public bool IsNonStandard { get; set; }
        /// <summary>
        /// حق مقر با توجه به داکیومنت
        /// </summary>
        public int SendPlaceType { get; set; }
    }

    public class PostBarcodeGeneratorOutputModel
    {
        public PostBarcodeData Data { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public string BarCode { get; set; }
    }

    public class PostBarcodeData
    {
        /// <summary>
        /// مجموع قیمت
        /// </summary>
        public long TotalPrice { get; set; }
        /// <summary>
        /// حق السهم پست
        /// </summary>
        public long PostPrice { get; set; }
        /// <summary>
        /// کرایه پستی
        /// </summary>
        public long PostFare { get; set; }
        /// <summary>
        /// بیمه
        /// </summary>
        public long InsurancePrice { get; set; }
        /// <summary>
        /// مالیات
        /// </summary>
        public int Tax { get; set; }

    }
}

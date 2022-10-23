using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Ubbar
{
    /// <summary>مدل خروجی تابع استعلام قیمت
    /// <para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///<para>DetailUbaarPriceInquiry جزییات خروجی</para>
    /// 
    /// </summary>
    public class Result_Ubaar_priceenquiry
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public string EnMessage { get; set; }
        public DetailUbaarPriceInquiry DetailUbaarPriceInquiry { get; set; }

    }
    /// <summary>
    /// <para>order_data یک مدل</para>
    /// <para>success_flag وضعیت خروجی صفر تابع خطا داره و یک تابع اوکی بوده است</para>
    /// <para>warning_messages لیست هشدار ها</para>
    /// </summary>
    public class DetailUbaarPriceInquiry
    {
        public order_data order_data { get; set; }
        public int success_flag { get; set; }
        public List<warning_messages> warning_messages { get; set; }
        public List<error_messages> error_messages { get; set; }
    }
    /// <summary> خروجی استعلام قیمت
    /// <para>predicted_price          قیمت پیش بینی شده</para>
    /// <para>tomorrow_predict_price   قیمت پیشبینی شده فردا </para>
    /// <para>tomorrow_discount_percent درصد تخفیف فردا</para>
    /// <para>discount                 مبلغ تخفیف</para>
    /// <para>driver_income            </para>
    /// <para>tracking_code             کد پیگیری، باید این کد را برای ثبت سفارش ارسال کنید</para>
    /// <para>min_acceptable_price     حداقل قیمت</para>
    /// <para>max_acceptable_price     حداکثر قیمت قابل قبول</para>
    /// <para>order_credit             </para>
    /// <para>status_farsi             </para>
    /// <para>status                    </para>
    /// <para>tavafoghi_flag           فلگ توافقی بودن</para>
    /// </summary>
    public class order_data
    {
        public string predicted_price { get; set; }
        public string tomorrow_predict_price { get; set; }
        public string tomorrow_discount_percent { get; set; }
        public string discount { get; set; }
        public string driver_income { get; set; }
        public string tracking_code { get; set; }
        public string min_acceptable_price { get; set; }
        public string max_acceptable_price { get; set; }
        public string order_credit { get; set; }
        public string status_farsi { get; set; }
        public string status { get; set; }
        public string tavafoghi_flag { get; set; }
    }

}

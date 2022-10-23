using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Ubbar
{
    /// <summary>خروجی تابع ویرایش سفارش -
    /// <para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///<para>DetailUbaarmodifyorder جزییات خروجی </para>
    /// 
    /// </summary>
    public class Result_Ubaar_modifyorder
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public string EnMessage { get; set; }
        public DetailUbaarmodifyorder DetailUbaarmodifyorder { get; set; }

    }
    /// <summary>
    /// <para>order_data مدل خروجی</para>
    /// <para>success_flag وضعیت تابع صفر تابع با خطا روبه رو بوده است و یک تابع اوکی بوده است</para>
    /// <para>warning_messages لیست هشدارها</para>
    /// </summary>
    public class DetailUbaarmodifyorder
    {
        public order_data_order order_data { get; set; }
        public int success_flag { get; set; }
        public List<warning_messages> warning_messages { get; set; }
        public List<error_messages> error_messages { get; set; }
    }
    /// <summary>مدل خروجی
    /// <para>tracking_code          کد پیگیری </para>
    /// <para>destination_lat       </para>
    /// <para>destination_lng       </para>
    /// <para>source_lat            </para>
    /// <para>source_lng            </para>
    /// <para>address_id_destination</para>
    /// <para>address_id_source     </para>
    /// <para>price                 قیمت</para>
    /// <para>status_farsi          متن پیام بازگشتی فارسی</para>
    /// <para>status                متن پیام بازگشتی : انگلیسی</para>
    /// <para></para>
    /// 
    /// </summary>
    public class order_data_order
    {

        public string tracking_code { get; set; }
        public string destination_lat { get; set; }
        public string destination_lng { get; set; }
        public string source_lat { get; set; }
        public string source_lng { get; set; }
        public string address_id_destination { get; set; }
        public string address_id_source { get; set; }
        public string price { get; set; }
        public string status_farsi { get; set; }
        public string status { get; set; }
    }
}

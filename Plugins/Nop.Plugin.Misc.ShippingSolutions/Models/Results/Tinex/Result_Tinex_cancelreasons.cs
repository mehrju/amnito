using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Tinex
{
    ///<summary> 
    ///<para>خروجی توابع در کلاس Result_Tinex_cancelreasons</para>
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///<para> ListItemsTinexCancelReasons جزییات خروجی برگشتی</para>
    ///</summary>
    ///

    public class Result_Tinex_cancelreasons
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
       public List< ItemTinexCancelReasons> ListItemsTinexCancelReasons { get; set; }
    }
    /// <summary> خروجی تابع لیست دلایل کنسل کردن سفارش
    /// <para>id</para>
    /// <para>title عنوان دلیل</para>
    /// <para>description توضیحات</para>
    /// 
    /// </summary>
    public class ItemTinexCancelReasons
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }
}

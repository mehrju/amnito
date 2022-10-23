using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Tinex
{
    ///<summary> 
    ///<para>خروجی توابع در کلاس Result_Tinex_insert</para>
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///<para> Detail_TinexInsert جزییات خروجی برگشتی</para>
    ///</summary>
    ///

    public class Result_Tinex_insert
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }

       public Detail_TinexInsert Detail_TinexInsert { get; set; }
    }
    /// <summary> مدل خروجی تابع ثبت سفارش
    /// <para>status وضعیت خروجی، صفر تابع اوکی بوده است</para>
    /// <para>result</para>
    /// <para>message</para>
    /// 
    /// </summary>
    public class Detail_TinexInsert
    {
        public int status { get; set; }
        public string result { get; set; }
        public string message { get; set; }
    }
}

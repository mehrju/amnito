using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Tinex
{
    ///<summary> 
    ///<para>خروجی توابع در کلاس Result_Tinex_cancel</para>
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///<para> DetailTinexCancel جزییات خروجی برگشتی</para>
    ///</summary>
    ///
    public class Result_Tinex_cancel
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
       public DetailTinexCancel DetailTinexCancel { get; set; }
    }
    /// <summary> مدل خروجی تابع کنسل کردن سفارش
    /// <para>status صفر تابعم اوکی بوده است</para>
    /// <para>404 منبع یافت نشد</para>
    /// <para>406 سفارش قابل کنسل نیست</para>
    /// <para>400 خطای ناشناخته</para>
    /// 
    /// </summary>
    public class DetailTinexCancel
    {
        public int status { get; set; }
        public string result { get; set; }
        public string message { get; set; }
    }
}

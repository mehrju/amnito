using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Tinex
{
    ///<summary> 
    ///<para>خروجی توابع در کلاس Result_Tinex_insertcommit</para>
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///<para> DetailTinexInsertCommit جزییات خروجی برگشتی</para>
    ///</summary>
    ///

    public class Result_Tinex_insertcommit
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public DetailTinexInsertCommit DetailTinexInsertCommit { get; set; }

    }
    /// <summary>خزوجی تابع تایید سفارش
    /// <para>status صفر تابع اوکی بوده است</para>
    /// <para>در غیراین صورت طبق ارورهای زیر خطا میدهد</para>
    /// <para>404 منبع یافت نشد</para>
    /// <para>406 سفارش قبلا ثبت شده است</para>
    /// <para>400 ارور ناشناخته</para>
    /// </summary>
    public class DetailTinexInsertCommit
    {
        public int status { get; set; }
        public string result { get; set; }
        public string message { get; set; }
    }
}

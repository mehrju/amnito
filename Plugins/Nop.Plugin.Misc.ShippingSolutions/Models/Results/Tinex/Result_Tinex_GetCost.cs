using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Tinex
{
    ///<summary> 
    ///<para>خروجی توابع در کلاس Result_Tinex_GetCost</para>
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///<para> Detail_TinexGetCost جزییات خروجی برگشتی</para>
    ///</summary>
    ///

    public class Result_Tinex_GetCost
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public Detail_TinexGetCost Detail_TinexGetCost { get; set; }

    }
    /// <summary>مدل خروجی تابع استعلام قیمت
    /// <para>status اگر صفر باشد استعلام اوکی بوده است</para>
    /// <para>cost هزینه</para>
    /// <para>deliver_time زمان تحویل</para>
    /// <para>errors لیست ارورها</para>
    /// 
    /// </summary>
    public class Detail_TinexGetCost
    {
        public int status { get; set; }
        public double cost { get; set; }
        public Delivertime deliver_time { get; set; }
        public Errors errors { get; set; }
    }
    public class Delivertime
    {
        public string date { get; set; }
        public TimeSpan timespan_since { get; set; }
        public TimeSpan timespan_til { get; set; }
    }

    public class Errors
    {
        public string Message { get; set; }
    }
}

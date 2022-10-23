using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Taroff
{
    /// <summary>
    /// مدل خروجی متد ثبت گروهی بارنامه مخصوص مشتری
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// <para>objects آبجکت خروجی</para>
    /// </summary>
    public class Result_Taroff_CreateOrder
    {

        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public DetailResult_CreateOrder DetailResult_CreateOrder { get; set; }
    }
    /// <summary>
    /// مدل جزییات ثبت سفارش
    /// <para>statuse وضعیت</para>
    /// <para>کدپیگری سفارش id </para>
    /// </summary>
    public class DetailResult_CreateOrder
    {
        public string status { get; set; }
        public int id { get; set; }
        public int shipping { get; set; }
        public int tax { get; set; }
    }
}

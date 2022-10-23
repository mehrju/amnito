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
    public class Result_Taroff_GetStateOrder
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public DetailResult_Taroff_GetStateOrder DetailResult_Taroff_GetStateOrder { get; set; }
    }
    public class DetailResult_Taroff_GetStateOrder
    {
        public string state { get; set; }
        public object barcode { get; set; }
        public int stateid { get; set; }
        public string statetitle { get; set; }
        public object shopshare { get; set; }
        public object taroffshare { get; set; }
        public object postshare { get; set; }
        public object peikname { get; set; }
        public object peikphone { get; set; }
    }
}

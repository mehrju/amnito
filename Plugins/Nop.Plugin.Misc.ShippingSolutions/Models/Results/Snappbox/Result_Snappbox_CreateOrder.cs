using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Snappbox
{
    /// <summary>
    /// مدل خروچی ثبت سفارش
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// </summary>
    public class Result_Snappbox_CreateOrder
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public DetailResult_Snappbox_CreateOrder DetailResult_Snappbox_CreateOrder { get; set; }
    }
    /// <summary>
    /// <para>api_status وضعیت </para>
    /// <para>status_code 201</para>
    /// <para>key=ORDER_CREATED</para>
    /// <para>message متن</para>
    /// <para>data</para>
    /// 
    /// </summary>
    public class DetailResult_Snappbox_CreateOrder
    {
        public string api_status { get; set; }
        public string status_code { get; set; }
        public string key { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }
    /// <summary>
    /// <para>allocationTimeout زمانی که ما دنبال یک پیک میگردیم و خب timeout اش به معنی زمانی هست که سرچ ما برای بایکر تموم میشه</para>
    /// <para>orderId شماره سفارش</para>
    /// 
    /// </summary>
    public class Data
    {
        public int allocationTimeout { get; set; }
        public int orderId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Snappbox
{
    /// <summary>
    /// مدل ورودی دریافت چزییات و پیگیری سفارش
    /// <para>customerId  کد مشتری که از تنظیمات پر خواهد شد</para>
    /// <para>orderId  کد شفارش که از خرجی متد ثبت سفارش درافت خواهید کرد</para>
    /// 
    /// </summary>
    public class Params_Snappbox_Get_Order_Details
    {
        public string customerId { get; set; }
        public string orderId { get; set; }
    }
}

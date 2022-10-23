using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Snappbox
{
    /// <summary>
    /// مدل ورودی کنسل کردن سفارش
    /// <para>customerId کد مشتری که از تنظمیات تکمیل خواهد شد</para>
    /// <para>orderId کد سفارش</para>
    /// 
    /// </summary>
    public class Params_Snappbox_Cancel_Order
    {
        public string customerId { get; set; }
        public string orderId { get; set; }
    }
}

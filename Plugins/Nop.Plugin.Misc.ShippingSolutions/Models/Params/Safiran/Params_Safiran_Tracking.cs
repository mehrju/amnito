using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran
{
    /// <summary>
    /// مدل ورودی ترک کردن محموله یا بارنامه
    /// <para> طی صحبت انجام شده کاربر میتواند براساس شماره بارنامه که از خروجی متد ثبت بارنامه گرفته میشود</para>
    /// <para>یا کد کالا که در مدل ورودی متد ثبت بارنامه به سرویس ارسال میشود قابل پیگیری است</para>
    /// <para>OrderTracking مدل ورودی سفارش یا بارنامه</para>
    /// <para></para>
    /// 
    /// </summary>
    public class Params_Safiran_Tracking
    {
        public OrderTracking order { get; set; }
    }
    /// <summary>
    /// مدل ورودی سفارش در متد ترک کردن محموله یا کالا
    /// <para>reference کد بارنامه یا کد 11 رقمی بسته(کالا)</para>
    /// <para>lang =fa </para>
    /// <para></para>
    /// 
    /// </summary>
    public class OrderTracking
    {
        public string reference { get; set; }
        public string lang { get; set; }
    }
}

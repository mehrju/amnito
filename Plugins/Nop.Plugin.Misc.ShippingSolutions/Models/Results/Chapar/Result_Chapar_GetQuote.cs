using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Chapar
{
    /// <summary>
    /// مدل خروجی متد استعلام قیمت
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// <para>objects آبجکت خروجی</para>
    /// </summary>
    public class Result_Chapar_GetQuote
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public ObjectsGetQuote objects { get; set; }
    }
    /// <summary>
    /// مدل اصلی برگشتی از سرویس که مپ شد به مدل خروجی 
    /// </summary>
    public class Temp_Result_Chapar_GetQuote
    {
        public bool result { get; set; }
        public string message { get; set; }
        public ObjectsGetQuote objects { get; set; }
    }
    /// <summary>
    /// <para>quote مبلغ کرایه</para>
    /// <para>currency واحد پول کشور</para>
    /// </summary>
    public class OrderGetQuote
    {
        public decimal quote { get; set; }
        public string currency { get; set; }
        public _Price price { get; set; }
    }
    /// <summary>
    /// مدل سفارش
    /// </summary>
    public class ObjectsGetQuote
    {
        public OrderGetQuote order { get; set; }
    }
    public class _Price
    {
        public string Zone { get; set; }
        public int fld_Manual_Cost { get; set; }
        public int fld_Pack_Cost { get; set; }
        public int fld_Charge_Cost { get; set; }
        public int fld_Manual_Insurance { get; set; }
        public int fld_Lab_Cost { get; set; }
        public int fld_Manual_VAT { get; set; }
        public int fld_Total_Cost { get; set; }
        public string price_list { get; set; }
        public string min_ins { get; set; }
    }
}

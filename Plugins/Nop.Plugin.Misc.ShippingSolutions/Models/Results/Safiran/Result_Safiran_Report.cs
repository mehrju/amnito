using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Safiran
{
    /// <summary>
    /// مدل خروجی متد گزارش عملکرد
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// <para>objects آبجکت خروجی</para>
    /// </summary>
    public class Result_Safiran_Report
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public ObjectsReport objects { get; set; }
    }
    public class Temp_Result_Safiran_Report
    {
        public bool result { get; set; }
        public string message { get; set; }
        public ObjectsReport objects { get; set; }
    }
    public class CnReport
    {
        public string tracking { get; set; }
        public string date { get; set; }
        public string assinged_pieces { get; set; }
        public string weight { get; set; }
        public string service { get; set; }
        public string value { get; set; }
        public string payment_term { get; set; }
        public string last_status { get; set; }
        public string delivery_date { get; set; }
        public string delivery_person { get; set; }
    }

    /// <summary>
    /// مدل دریافت کننده
    /// <para>person نام دریافت کننده</para>
    /// <para>company</para>
    /// <para>city</para>
    /// <para>telephone</para>
    /// <para>mobile</para>
    /// <para>address</para>
    /// <para>post_code</para>
    /// 
    /// </summary>
    public class ReceiverReport
    {
        public string person { get; set; }
        public string company { get; set; }
        public string city { get; set; }
        public string telephone { get; set; }
        public string mobile { get; set; }
        public string address { get; set; }
        public string post_code { get; set; }
    }
    /// <summary>
    /// مدل مالی
    /// 
    /// </summary>
    public class FinancialReport
    {
        public string delivery_charge { get; set; }
        public string insurance_charge { get; set; }
        public string vat_charge { get; set; }
        public string pack_charge { get; set; }
        public string extra_charge { get; set; }
        public int cod_value { get; set; }
        public int total_charge { get; set; }
    }

    /// <summary>
    /// مدل یک بارنامه
    /// <para>CnReport مدل بارنامه </para>
    /// <para>ReceiverReport دریافت کننده</para>
    /// <para>FinancialReport مدل مالی</para>
    /// <para>List history لیستی از عملیات های انجام شده برروی محموله-بارنامه</para>
    /// </summary>
    public class ConReport
    {
        public CnReport cn { get; set; }
        public ReceiverReport receiver { get; set; }
        public FinancialReport financial { get; set; }
        public List<object> history { get; set; }
    }
    /// <summary>
    /// مدل لیستی از بارنامهها
    /// </summary>
    public class ObjectsReport
    {
        public List<ConReport> cons { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Chapar
{
    /// <summary>
    /// مدل خروجی متد ثبت گروهی بارنامه مخصوص مشتری
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// <para>objects آبجکت خروجی</para>
    /// </summary>
    public class Result_Chapar_Bulkimport
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public Objectsimport objects { get; set; }
    }
    public class Temp_Result_Chapar_Bulkimport
    {
        public bool result { get; set; }
        public object message { get; set; }
        public Objectsimport objects { get; set; }
    }
    /// <summary>
    /// مدل نتیجه برگشتی برای هر یک از محموله ها
    /// <para>tracking کد پیگیری محموله-بارنامه</para>
    /// <para>package کد 11 رقمی کالا،که در این متد خود سیتم تولید میکتند</para>
    /// <para>reference شناسه محلی است است که در موقع ارسال ، به سریس فرستاده شده</para>
    /// <para>status به خاطر اینکه در این متد شما به صورت گروهی محموله ثبت میکند ممکن است مواردی از محموله ها به هردلیل امکان ثبت نداشته باشد که با توجه به این مقدار مشخص میشود</para>
    /// <para> اگر اوکی باشد یعنی عملیات ثبت با موفقیت انجام شده  است و بعلکس هم که مشخص است</para>
    /// <para>error ایرادها</para>
    /// 
    /// </summary>
    public class Result
    {
        public string tracking { get; set; }
        public string[] package { get; set; }
        public int reference { get; set; }
        public bool status { get; set; }
        public string error { get; set; }
    }
    /// <summary>
    /// لیست مدل خروجی برای هر یک از محموله های ارسالی
    /// </summary>
    public class Objectsimport
    {
        public List<Result> result { get; set; }
    }
}

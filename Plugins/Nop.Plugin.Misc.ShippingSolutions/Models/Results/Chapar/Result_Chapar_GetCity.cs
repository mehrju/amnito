using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Chapar
{
    /// <summary>
    /// مدل خروجی متد لیست شهرهای یک استان
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// <para>objects آبجکت خروجی</para>
    /// </summary>
    public class Result_Chapar_GetCity
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public ObjectsGetCity objects { get; set; }
    }
    public class Temp_Result_Chapar_GetCity
    {

        public bool result { get; set; }
        public string message { get; set; }
        public ObjectsGetCity objects { get; set; }
    }
    /// <summary>
    /// مدل یک شهر
    /// <para>state_no کد استان</para>
    /// <para>no کد شهر</para>
    /// <para>name نام شهر</para>
    /// </summary>
    public class City
    {
        public string state_no { get; set; }
        public string no { get; set; }
        public string name { get; set; }
    }
    /// <summary>
    /// مدل لیست شهرها
    /// <para>city لیست شهرها</para>
    /// </summary>
    public class ObjectsGetCity
    {
        public List<City> city { get; set; }
        public object selected { get; set; }
    }
}

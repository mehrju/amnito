using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Chapar
{
    /// <summary>
    /// مدل خروجی متد لیست استان ها
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// <para>objects آبجکت خروجی</para>
    /// </summary>
    public class Result_Chapar_GetState
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public ObjectsGetState objects { get; set; }
    }
    public class Temp_Result_Chapar_GetState
    {
        public bool result { get; set; }
        public string message { get; set; }
        public ObjectsGetState objects { get; set; }
    }
    /// <summary>
    /// مدل یک استان
    /// <para>no کد استان</para>
    /// <para>name نام استان</para>
    /// 
    /// </summary>
    public class State
    {
        public string no { get; set; }
        public string name { get; set; }
    }
    /// <summary>
    /// مدل لیست استان ها
    /// </summary>
    public class ObjectsGetState
    {
        public List<State> state { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Safiran
{
    /// <summary>
    /// مدل خروجی متد ثبت بارنامه
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// <para>objects آبجکت خروجی</para>
    /// </summary>
    public class Result_Safiran_PickupRequest
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public ObjectsPickupRequest objects { get; set; }
    }
    public class Temp_Result_Safiran_PickupRequest
    {
        public bool result { get; set; }
        public string message { get; set; }
        public ObjectsPickupRequest objects { get; set; }
    }
    /// <summary>
    /// شماره بارنامه برگشتی
    /// </summary>
    public class ObjectsPickupRequest
    {
        public string cons { get; set; }
    }
}

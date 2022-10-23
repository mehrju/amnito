using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.YarBox
{
    /// <summary>
    /// مدل خروجی ای پی ای نهایی کردن سفارش
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///این ای پی ای خروجی دیگری ندارد
    /// </summary>
    public class Result_YarBox_accept
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
    }
}

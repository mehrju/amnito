using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.YarBox
{
    /// <summary>
    /// مدل خروجی استعلام قیمت
    /// ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// واحد مبلغ برگشتی به تومان میباشد
    /// </summary>
    public class Result_Yarbox_Qute
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public Detail_Result_Yarbox_Qute Detail_Result_Yarbox_Qute{get;set;}
    }
    public class Detail_Result_Yarbox_Qute
    {
        public int price { get; set; }
    }
}

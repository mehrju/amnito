using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Tinex
{
    ///<summary> 
    ///<para>خروجی توابع در کلاس Result_Tinex_Token</para>
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///<para> DetailTinexToken جزییات خروجی برگشتی</para>
    ///</summary>
    ///
    public class Result_Tinex_Token
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public DetailTinexToken DetailTinexToken { get; set; }
    }
    public class DetailTinexToken
    {
        public string token_type { get; set; }
        public long expires_in { get; set; }
        public string access_token { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.YarBox
{
    /// <summary>
    /// مدل خروجی ای پی ای نوع بسته
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///<para>itemsApTypes لیستی از انوع بسته مثلا از صفر تا یک کیلو و.... </para>
    /// </summary>
    public class Result_YarBox_ApType
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public List<ItemsApType> itemsApTypes{ get; set; }

}
    public class ItemsApType
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}

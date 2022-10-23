using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.YarBox
{
    /// <summary>
    /// مدل خروجی ای پی ای نوع بسته بندی
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///<para>Items لیستی از انواع بسته بندی مثلا کارتن سایز یک و.... </para>
    /// </summary>
    /// 
    public class Result_YarBox_PackingType
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public List<ItemsPackingType> Items { get; set; }
       
    }
    /// <summary>
    /// <para>name نام آیتم</para>
    /// <para>id آی دی</para>
    /// </summary>
    public class ItemsPackingType
    {
        public string name { get; set; }
        public int id { get; set; }
    }
}

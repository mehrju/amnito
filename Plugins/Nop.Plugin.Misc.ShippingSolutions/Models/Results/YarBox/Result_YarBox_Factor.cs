using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.YarBox
{
    /// <summary>
    /// مدل خروجی ای پی ای ثبت فاکتور
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///<para>detailFactor مدل خروجی یا نمایش اطلاعات فاکتور، دقیقا همان مواردی است که در ثبت بسته به سرویس ارسال شده بود</para>
    /// </summary>
    /// 
    public class Result_YarBox_Factor
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public DetailFactor detailFactor { get; set; }
    }
    /// <summary> 
    /// مدل خروجی ای  پی ای ثبت بسته
    /// طبق داکیومنت تمام فیلد ها باید پر شوند
    /// در این ای پی ای نام شهر مبدا و مقصد به صورت متنی ارسال میشود
    /// <para>count تعداد</para>
    /// <para>origin شهر مبدا به صورت متنی</para>
    /// <para>destination شهر مقصد به صورت متنی</para>
    /// <para>apPackingTypeId نوع بسته بندی مثلا کارتن با سایز یک: این مورد باید از ای پی ای دیگری گرفته شود</para>
    /// <para>apTypeId نوع بسته مثلا از صفر تا یک کیلو : این مورد باید از ای پی ای دیگری گرفته شود</para>
    /// <para>insurance بیمه</para>
    /// <para>receiveType حتما برابر صفر باشد</para>
    /// <para>senderPhone موبایل فرستنده</para>
    /// <para>receiverPhone موبایل گیرنده</para>
    /// <para>latitudeعرض جغرافیایی مبدا</para>
    /// <para>longitude طول جغرافیایی مبدا</para>
    /// <para>destinationAddress ادرس مقصد</para>
    /// <para>originAddress ادرس مبدا</para>
    /// <para></para>
    /// 
    /// </summary>
    public class DetailFactor
    {
        public int id { get; set; }
        public int count { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public int apPackingTypeId { get; set; }
        public int apTypeId { get; set; }
        public int insurance { get; set; }
        public int receiveType { get; set; }
        public string senderPhone { get; set; }
        public string receiverPhone { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string destinationAddress { get; set; }
        public string originAddress { get; set; }
    }
}

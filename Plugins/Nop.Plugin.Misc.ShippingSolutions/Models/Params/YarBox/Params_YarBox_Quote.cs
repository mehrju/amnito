using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.YarBox
{
    /// <summary>
    /// مدل ورودی استعلام قیمت
    /// همه موارد اجباری هستند
    /// <para>count تعداد بسته:باید بزرگتر از صفر باشد</para>
    /// <para>مبدا تهران میباشد</para>
    /// <para>destination city  نام شهر مقصد</para>
    /// <para>destination State  نام استان مقصد</para>
    /// <para>apTypeId نوع بسته مثلا از صفر تا یک کیلو : این مورد باید از ای پی ای دیگری گرفته شود</para>
    /// <para>apPackingTypeId نوع بسته بندی مثلا کارتن با سایز یک: این مورد باید از ای پی ای دیگری گرفته شود</para>
    /// </summary>
    public class Params_YarBox_Quote
    {
        public int count { get; set; }
        public string destination_State { get; set; }
        public string destination_City { get; set; }
        public int apPackingTypeId { get; set; }
        public int apTypeId { get; set; }
        public int cityId { get; set; }
    }


    public class _Params_YarBox_Quote
    {
        public int count { get; set; }
        public string destination { get; set; }
        public int apPackingTypeId { get; set; }
        public int apTypeId { get; set; }
        public int cityId { get; set; }
    }
}

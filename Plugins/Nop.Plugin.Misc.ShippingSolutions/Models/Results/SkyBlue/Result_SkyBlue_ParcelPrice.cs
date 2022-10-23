using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Snappbox
{
    /// <summary>
    /// مدل خروچی لیست سفارشات
    /// <para> Status وضعیت متد</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///<para>DetailParcelPrice  مدل خروجی متد </para>
    /// </summary>
    public class Result_SkyBlue_ParcelPrice
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public DetailParcelPrice DetailParcelPrice { get; set; }
    }
    public class DetailParcelPrice
    {
        public int Weight { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Countrycode { get; set; }
        public int ParcelType { get; set; }
        public int Price { get; set; }
        public string errorMessage { get; set; }
    }
}

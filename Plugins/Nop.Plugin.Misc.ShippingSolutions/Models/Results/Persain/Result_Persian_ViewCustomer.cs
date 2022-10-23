using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Persain
{
    /// <summary>
    /// مدل خروجی متد پیگیری
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// <para>objects آبجکت خروجی</para>
    /// </summary>
    public class Result_Persian_ViewCustomer
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public Deatil_Result_Persian_ViewCustomer Deatil_Result_Persian_ViewCustomer { get; set; }
    }
    public class Deatil_Result_Persian_ViewCustomer
    {
        public string status { get; set; }
        public DataViewCustomer data { get; set; }
    }
    public class DataViewCustomer
    {
        public int id { get; set; }
        public string status { get; set; }
        public object delivery_status { get; set; }
    }
}

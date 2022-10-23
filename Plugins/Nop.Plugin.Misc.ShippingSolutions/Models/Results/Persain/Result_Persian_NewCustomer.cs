using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Persain
{
    /// <summary>
    /// مدل خروجی متد ثبت کالا
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// <para>objects آبجکت خروجی</para>
    /// </summary>
    public class Result_Persian_NewCustomer
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public Detail_Result_Persian_NewCustomer Detail_Result_Persian_NewCustomer { get; set; }
    }
    public class Detail_Result_Persian_NewCustomer
    {
        public string status { get; set; }
        public Data data { get; set; }
        public string message { get; set; }
    }
    public class Data
    {
        public string id { get; set; }
    }
}

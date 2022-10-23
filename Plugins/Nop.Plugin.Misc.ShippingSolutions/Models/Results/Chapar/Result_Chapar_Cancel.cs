using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Chapar
{
    /// <summary>
    /// مدل خروجی متد ثبت گروهی بارنامه مخصوص مشتری
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// <para>Detail_Chapar_Cancel آبجکت خروجی</para>
    /// </summary>
    public class Result_Chapar_Cancel
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public Detail_Chapar_Cancel Detail_Chapar_Cancel { get; set; }

    }
    public class Detail_Chapar_Cancel
    {
        public bool result { get; set; }
        public string message { get; set; }
        public Objects1 objects { get; set; }
    }
    public class Objects1
    {
        public List<object> order { get; set; }
    }
}

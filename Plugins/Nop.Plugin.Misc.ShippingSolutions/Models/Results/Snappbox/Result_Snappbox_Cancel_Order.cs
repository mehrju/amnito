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
    ///<para>DetailResult_Snappbox_Get_Order_List  مدل خروجی متد </para>
    /// </summary>
    public class Result_Snappbox_Cancel_Order
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public DetailResult_Snappbox_Cancel_Order DetailResult_Snappbox_Cancel_Order { get; set; }

    }
    public class DetailResult_Snappbox_Cancel_Order
    {
        public string api_status { get; set; }
        public string status_code { get; set; }
        public string key { get; set; }
        public string message { get; set; }
    }
}

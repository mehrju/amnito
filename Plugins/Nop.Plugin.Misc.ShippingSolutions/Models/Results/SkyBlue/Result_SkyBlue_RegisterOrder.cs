using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.SkyBlue
{
    /// <summary>
    /// مدل خروچی لیست سفارشات
    /// <para> Status وضعیت متد</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///<para>DetailRegisterOrder  مدل خروجی متد </para>
    /// </summary>
    public class Result_SkyBlue_RegisterOrder
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public DetailRegisterOrder DetailRegisterOrder { get; set; }
    }

    public class DetailRegisterOrder
    {
        public string SenderName { get; set; }
        public string SenderAddress { get; set; }
        public object SenderPostCode { get; set; }
        public string SenderPhone { get; set; }
        public object SenderEmail { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverAddress { get; set; }
        public object ReceiverPostCode { get; set; }
        public string ReceiverPhone { get; set; }
        public object ReceiverEmail { get; set; }
        public object Content { get; set; }
        public int ContentValue { get; set; }
        public int Weight { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Countrycode { get; set; }
        public int ParcelType { get; set; }
        public int Price { get; set; }
        public string OrderNumber { get; set; }
        public string errorMessage { get; set; }
       
    }
}

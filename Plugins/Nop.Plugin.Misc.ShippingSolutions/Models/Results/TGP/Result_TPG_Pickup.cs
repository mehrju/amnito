using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.TGP
{
    /// <summary>
    /// مدل خروجی متد ثبت گروهی بارنامه مخصوص مشتری
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// <para>Deatil_TPG_Pickup آبجکت خروجی</para>
    /// <para> RequestId کد پیگیری درخواست</para>
    /// 
    /// </summary>
    public class Result_TPG_Pickup
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public string RequestId { get; set; }

        public Deatil_TPG_Pickup Deatil_TPG_Pickup { get; set; }
    }
    /// <summary>
    /// مدل خروجی متد pickup
    /// <para>CN کد پیگیری سفارش</para>
    /// <para>PickupId</para>
    /// <para>Code وضعیت پذیرش سفارش: اگر201 باشه اوکی هستش</para>
    /// <para>TotalCost کل هزینه</para>
    /// 
    /// </summary>
    public class Deatil_TPG_Pickup
    {
        public string ContractCode { get; set; }
        public int ContractId { get; set; }
        public int ClientId { get; set; }
        public int ContactId { get; set; }
        public int PickupId { get; set; }
        public int CN { get; set; }
        public double TotalCost { get; set; }
        public string PickupCode { get; set; }
        public int ScheduleId { get; set; }
        public string CorrelatorId { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public int ResultId { get; set; }
        public string ResultData { get; set; }
    }
}

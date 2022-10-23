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
    /// <para>Deatil_Result_TGP_Compute آبجکت خروجی</para>
    /// <para> RequestId کد پیگیری درخواست</para>
    /// </summary>
    public class Result_TGP_Compute
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public string RequestId { get; set; }
        public Deatil_Result_TGP_Compute Deatil_Result_TGP_Compute { get; set; }
    }
    /// <summary>
    /// مدل خروجی
    /// <para>CanDo وضعیت قبول کردن سرویس: </para>
    /// <para>Coverage وضعیت تحت پوشش</para>
    /// <para></para>
    /// <para></para>
    /// <para></para>
    /// <para></para>
    /// 
    /// </summary>
    public class Deatil_Result_TGP_Compute
    {

        public int CanDo { get; set; }
        public bool Coverage { get; set; }
        public int TaxiCost { get; set; }
        public string Title { get; set; }
        public int TimeEstimate { get; set; }
        public int Price { get; set; }
        public int ExtraPrice { get; set; }
        public object ExtraAmount { get; set; }
        public int Total { get; set; }
        public object TotalWithRatio { get; set; }
        public object ExtraDivider { get; set; }
        public bool HasInsurance { get; set; }
        public int InsuranceCost { get; set; }
        public int PackingCost { get; set; }
        public int WeightSpace { get; set; }
        public bool isPas { get; set; }
        public int pasCost { get; set; }
        public int TaxiType { get; set; }
        public object SecondaryRegionId { get; set; }
        public object SecondaryRegionName { get; set; }
        public int VAT { get; set; }
    }
}

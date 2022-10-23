using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.YarBox
{
    /// <summary>
    /// مدل خروجی ای پی ای لاگین و گرفتن توکن جدید
    /// <para>Detail_YarBox_Verify توکن جدید</para>
    /// </summary>
    public class Result_YarBox_Verify
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public Detail_YarBox_Verify detail_yarBox_verify { get; set; }
    }
    /// <summary>
    /// مدل توکن جدید
    /// <para>access_token توکن دریافتی</para>
    /// <para>token_type نوع توکن beare</para>
    /// <para>expires_in انتقاء: در تنظیمات ناپ کارمس دیگه شد</para>
    /// <para>refresh_token</para>
    /// 
    /// </summary>
    public class Detail_YarBox_Verify
    {
        public object access_token { get; set; }
        public object token_type { get; set; }
        public int expires_in { get; set; }
        public object refresh_token { get; set; }
    }
}

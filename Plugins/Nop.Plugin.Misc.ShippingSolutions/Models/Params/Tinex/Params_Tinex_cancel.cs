using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Tinex
{
    /// <summary>مدل ورودی تابع کنسل کردن سفارش
    /// <para>order_no  اجباری شماره سفارش</para>
    /// <para>reason_id کد علت اجباری </para>
    /// <para>reason_description توضیحات</para>
    /// </summary>
    public class Params_Tinex_cancel
    {
        public string order_no { get; set; }
        public string reason_id { get; set; }
        public string reason_description { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Snappbox
{
    /// <summary>
    /// مدل دریافت لیست سفارشات
    /// <para>customerId کد مشتری</para>
    /// <para>ongoing وضعیت در دست اقدام</para>
    /// <para>pageNumber شماره  صفحه</para>
    /// <para>pageSize تعداد صفحه/رکورد</para>
    /// </summary>
    public class Params_Snappbox_Get_Order_List
    {
        public string customerId { get; set; }
        public bool ongoing { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }
}

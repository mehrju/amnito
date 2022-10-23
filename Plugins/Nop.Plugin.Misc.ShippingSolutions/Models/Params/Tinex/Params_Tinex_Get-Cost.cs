using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Tinex
{
    /// <summary> مدل وردی تابع استعلام قیمت
    /// <para>sub_orders لیستی از وزن کالا</para>
    /// <para>مجموع وزن کالاها باید کمتر از 20 کیلو باشد</para>
    /// 
    /// </summary>
    public class Params_Tinex_Get_Cost
    {
        public List<sub> sub_orders { get; set; }
    }
    /// <summary>مدل وزن هر کالا
    /// <para>weight اجباری </para>
    /// </summary>
    public class sub
    {
        public float weight { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Ubbar
{
    /// <summary> مدل ورودی تابع لیست مناطق
    /// <para>3 پرامتر وردی میتواند خالی باشد</para>
    /// <para>region_state نام استان</para>
    /// <para>region_city  نام شهر</para>
    /// <para>region_name  نام منطقه</para>
    /// 
    /// </summary>
    public class Params_Ubaar_regionlist
    {
        public string region_state { get; set; }
        public string region_city { get; set; }
        public string region_name { get; set; }
    }
}

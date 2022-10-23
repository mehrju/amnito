using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Tinex
{
    /// <summary> مدل ورودی تابع تایید سفارش
    /// <param> order_no      اجباری شماره سفارش</param>
    /// </summary>
    public class Params_Tinex_insertcommit
    {
        public string order_no { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.YarBox
{/// <summary>
/// مدل ورودی ای پی ای نهایی کردن سفارش
/// <para>id که در قصمت فاکتور گرفته اید</para>
/// </summary>
    public class Params_YarBox_accept
    {
        //ای دی است که در قسمت فاکتور گرفته اید
        public Int64 Id { get; set; }
    }
}

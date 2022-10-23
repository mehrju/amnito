using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran
{
    /// <summary>
    /// مدل ورودی لیست شهر های استان
    /// <para>State استان </para>
    /// </summary>
    public class Params_Safiran_GetCity
    {
        public State state { get; set; }
    }
    /// <summary>
    /// مدل ورودی استان
    /// <para> no کد شهر دریافتی از سرویس لیست استانهاست:اجباری</para>
    /// </summary>
    public class State
    {
        public int no { get; set; }
    }
}

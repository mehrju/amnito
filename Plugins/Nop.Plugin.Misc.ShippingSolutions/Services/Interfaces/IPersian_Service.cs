using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Persain;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Persain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces
{
    /// <summary>
    /// <para>NewCustomer ای پی ای ثبت محموله</para>
    /// <para>ViewCustomer متد پیگیری محموله</para>
    /// </summary>
    public interface IPersian_Service
    {
        Result_Persian_NewCustomer NewCustomer(Params_Persian_NewCustomer param);
        Result_Persian_ViewCustomer ViewCustomer(Params_Persian_ViewCustomer param);
    }
}

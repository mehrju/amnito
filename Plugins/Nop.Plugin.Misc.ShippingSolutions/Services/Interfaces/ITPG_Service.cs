using Nop.Plugin.Misc.ShippingSolutions.Models.Params.TPG;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.TGP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces
{
    /// <summary>
    /// <para>Compute استعلام قیمت</para>
    /// <para>Pickup ثبت بارنامه</para>
    /// <para>Pickup پیگیری بارنامه</para>
    /// 
    /// </summary>
    public interface ITPG_Service
    {
        Task<Result_TGP_Compute> Compute(Params_TPG_Compute param);
        Task<Result_TPG_Pickup> Pickup(Params_TPG_Pickup param);
        Result_TPG_Tracking Tracking(Params_TPG_Tracking param);
    }
}

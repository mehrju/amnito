using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Ubbar;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Ubbar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces
{
   public interface IUbaar_Service
    {
        Result_Ubaar_regionlist Regionlist(Params_Ubaar_regionlist param);
        Task<Result_Ubaar_priceenquiry> Priceenquiry(Params_Ubaar_priceenquiry param);
        Task<Result_Ubaar_modifyorder> Modifyorder(Params_Ubaar_modifyorder param);
        Result_Ubbar_Tracking Tracking(Params_Ubaar_Tracking param);
    }
}

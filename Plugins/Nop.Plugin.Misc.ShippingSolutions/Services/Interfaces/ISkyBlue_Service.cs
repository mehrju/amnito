using Nop.Plugin.Misc.ShippingSolutions.Models.Params.SkyBlue;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.SkyBlue;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Snappbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces
{
   public interface ISkyBlue_Service
    {
        Task<Result_SkyBlue_ParcelPrice> GetParcelPrice(Params_SkyBlue_ParcelPrice param);
        Task<Result_SkyBlue_RegisterOrder> RegisterOrder(Params_SkyBlue_RegisterOrder param);
        Task<Result_SkyBlue_CheckService> CheckService(Params_SkyBlue_CheckService param);
        Task<Result_SkyBlue_Cancell> Cancell(Params_SkyBlue_Cancel_Tracking param);
        Result_SkyBlue_Tracking Tracking(Params_SkyBlue_Cancel_Tracking param);

    }
}

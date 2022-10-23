using Nop.Plugin.Misc.ShippingSolutions.Models.Params.YarBox;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.YarBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services
{
    public interface IYarBox_Service
    {
        Result_YarBox_PackingType GetPackingType();
        Result_YarBox_ApType GetAPType();
        Task<Result_YarBox_AddPostPacks> AddPostPacks(Params_YarBox_AddPostPacks param);
        Task<Result_YarBox_Factor> Factor(Params_YarBox_Factor param);
        Task<Result_YarBox_accept> accept(Params_YarBox_accept param);
        Task<Result_Yarbox_Qute> Qute(Params_YarBox_Quote param);
        Result_YarBox_Tracking Tracking(Params_YarBox_Tracking param);
    }
}

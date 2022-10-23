using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Tinex;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Tinex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces
{
    public interface ITinex_Service
    {
        Result_Tinex_cancelreasons cancelreasons();
        Result_Tinex_GetCost GetCost(Params_Tinex_Get_Cost param);
        Result_Tinex_insert Insert(Params_Tinex_insert param);
        Result_Tinex_insertcommit InsertCommit(Params_Tinex_insertcommit param);
        Result_Tinex_cancel Cancel(Params_Tinex_cancel param);
    }
}

using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Chapar;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Mahex;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Chapar;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Mahex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces
{
    public interface IMahex_Service
    {

        List<Result_Mahex_GetCities> GetState();
        Result_Mahex_GetCities GetCityCode(int StateId);
        Task<Result_Mahex_GetQuote> GetQuote(Params_Mahex_GetPrices param);
        Task<Result_Mahex_Bulkimport> Bulkimport(Params_Mahex_createShipment param);
        GetShipmentResult UpdateShipmentTracking(string UUID);
        VoidShipmentResult VoidShipment(string waybill_number);
        Result_Mahex_Tracking Tracking(Params_Mahex_Tracking param);
    }
}

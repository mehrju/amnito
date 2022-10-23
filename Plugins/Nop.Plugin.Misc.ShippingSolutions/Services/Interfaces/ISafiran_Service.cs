using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Safiran;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces
{
    public interface ISafiran_Service
    {
        Result_Safiran_Tracking Tracking(Params_Safiran_Tracking param);
        Task<Result_Safiran_PickupRequest> Pickup(Params_Safiran_PickupRequest param);
        Result_Safiran_GetState GetState();
        Result_Safiran_GetCity GetCity(Params_Safiran_GetCity param);
        Task<Result_Safiran_GetQuote> GetQuote(Params_Safiran_GetQuote param);
        Result_Safiran_Report Report(Params_Safiran_Report param);
        Result_Safiran_Bulkimport Bulkimport(Params_Safiran_BulkImport param);
        Result_Safiran_BulkHistoryReport BulkHistoryReport(Params_Safiran_BulkHistoryReport param);
        Task<Result_Safiran_Cancel> Cancel(Params_Safiran_Cancel param);
    }
}

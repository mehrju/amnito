using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Chapar;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Chapar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces
{
    public interface IChapar_Service
    {
        
        Result_Chapar_GetState GetState();
        Result_Chapar_GetCity GetCity(Params_Chapar_GetCity param);
        Task<Result_Chapar_GetQuote> GetQuote(Params_Chapar_GetQuote param);
        Result_Chapar_Report Report(Params_Chapar_Report param);
        Task<Result_Chapar_Bulkimport> Bulkimport(Params_Chapar_BulkImport param);
        Result_Chapar_BulkHistoryReport BulkHistoryReport(Params_Chapar_BulkHistoryReport param);
        Result_Chapar_Tracking Tracking(Params_Chapar_Tracking param);
    }
}

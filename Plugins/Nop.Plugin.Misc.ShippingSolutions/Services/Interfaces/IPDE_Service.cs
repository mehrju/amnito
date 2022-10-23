using Nop.Plugin.Misc.ShippingSolutions.Models.Params;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.PDE;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.PDE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services
{
    public interface IPDE_Service
    {
        Result_PDE_CountriesGET Countries();
        Result_PDE_CitiesGET cities();
        Task<Result_PDE_Calculator> IntenationalCalculator(Params_PDE_IntenationalCalculator Param);
        Task<Result_PDE_Calculator> DomesticCalculator(Params_PDE_DomesticCalculator Param);
        Result_PDE_TrackingParcels TrackingParcels(Params_PDE_TrackingParcels param);
        Task<Result_PDE_RegisterOrder> RegisterInternationalOrder(Params_PDE_RegisterInternationalOrder Param);
        Task<Result_PDE_RegisterOrder> RegisterDomesticOrder(Params_PDE_RegisterDomesticOrder Param);
    }
}

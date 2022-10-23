using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Chapar;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.PDE;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Chapar;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.PDE;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Safiran;
using Nop.Plugin.Misc.ShippingSolutions.Services;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class TrackingController : BasePublicController
    {
        private readonly IPDE_Service _PDE;
        private readonly IChapar_Service _chapar;
        private readonly ISafiran_Service _safiran;

        public TrackingController
            (
            IPDE_Service PDE, IChapar_Service chapar, ISafiran_Service safiran
            )
        {
            _PDE = PDE;
            _chapar = chapar;
            _safiran = safiran;
        }

        [Route("api/Tracking/GetState")]
        [HttpPost]
        public JsonResult GetState(int IdCategury,string reference)
        {
            var result = (dynamic)null;
            //pde
            if (IdCategury ==707 || IdCategury == 708)
            {
                Params_PDE_TrackingParcels p = new Params_PDE_TrackingParcels();
                p.IdOrder = Convert.ToInt64(reference);
                Result_PDE_TrackingParcels r= _PDE.TrackingParcels(p);
                result = r;
            }
           //safiran
            if (IdCategury == 705 || IdCategury == 706 || IdCategury == 703 || IdCategury == 699)
            {
                Params_Safiran_Tracking x = new Params_Safiran_Tracking();
                Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran.OrderTracking t = new Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran.OrderTracking();
                t.reference = reference;
                t.lang = "fa";
                x.order = t;
                Result_Safiran_Tracking r = _safiran.Tracking(x);

            }
            //chapar
            if (IdCategury == 712 || IdCategury == 713 || IdCategury == 714 || IdCategury == 715)
            {
                Params_Chapar_Tracking x = new Params_Chapar_Tracking();
                Nop.Plugin.Misc.ShippingSolutions.Models.Params.Chapar.OrderTracking t = new Nop.Plugin.Misc.ShippingSolutions.Models.Params.Chapar.OrderTracking();
                t.reference = reference;
                t.lang = "fa";
                x.order = t;
                Result_Chapar_Tracking r = _chapar.Tracking(x);
                
            }
            return Json(result);
        }
    }
}

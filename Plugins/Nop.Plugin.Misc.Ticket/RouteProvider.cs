using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket
{
    public partial class RouteProvider : IRouteProvider
    {

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            

            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.Ticket.ManageCODCustomer",
              "Admin/CODCustomer/{Id:regex(\\d*)}",
              new { controller = "ManageRouteProvider", action = "GetCOD" }
          );
            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.Ticket.ManageDamagesCustomer",
              "Admin/DamagesCustomer/{Id}",
              new { controller = "ManageRouteProvider", action = "GetDamages" }
          );
            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.Ticket.ReadFilePdf",
            "EmbedePdf/ReadFilePdf",
            new { controller = "EmbedePdf", action = "ReadFilePdf" }
        );

        }

        public int Priority
        {
            get
            {
                return 16;
            }
        }

     
    }
}

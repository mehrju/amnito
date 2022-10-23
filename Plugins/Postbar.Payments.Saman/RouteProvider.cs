using Microsoft.AspNetCore.Builder;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace NopFarsi.Payments.SepShaparak
{
	
	public class RouteProvider : IRouteProvider
	{

        public void RegisterRoutes(IRouteBuilder routeBuilder)
		{
            routeBuilder.MapLocalizedRoute("NopFarsi.Payments.SepShaparak.Pay", "Plugins/SamanPostbar/Pay/",
                new { controller = "SamanPostbar", action = "Pay" });

            routeBuilder.MapLocalizedRoute("NopFarsi.Payments.SepShaparak.Verify", "Sep/Verify/",
                new { controller = "SamanPostbar", action = "Verify" });
		}
		
		public int Priority
		{
			get
			{
				return 0;
			}
		}
	}
}

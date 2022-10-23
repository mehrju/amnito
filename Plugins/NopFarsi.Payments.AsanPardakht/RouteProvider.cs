using Microsoft.AspNetCore.Builder;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace NopFarsi.Payments.AsanPardakht
{
	
	public class RouteProvider : IRouteProvider
	{

        public void RegisterRoutes(IRouteBuilder routeBuilder)
		{
            routeBuilder.MapLocalizedRoute("NopFarsi.Payments.AsanPardakht.Pay", "Plugins/AsanPardakhtNopFarsi/Pay/",
                new { controller = "AsanNopFarsi", action = "Pay" });

            routeBuilder.MapLocalizedRoute("NopFarsi.Payments.AsanPardakht.Verify", "Plugins/AsanPardakhtNopFarsi/Verify/",
                new { controller = "AsanNopFarsi", action = "Verify" });
            //RouteCollectionExtensions.MapRoute(routes, "NopFarsi.Payments.AsanPardakht.Pay", "Plugins/AsanPardakhtNopFarsi/Pay", new
            //{
            //    controller = "SepNopFarsi",
            //    action = "Pay",
            //    id = UrlParameter.Optional
            //}, new string[]
            //{
            //    "NopFarsi.Payments.AsanPardakht.Controllers"
            //});
            //RouteCollectionExtensions.MapRoute(routes, "NopFarsi.Payments.AsanPardakht.Verify", "Plugins/AsanPardakhtNopFarsi/Verify", new
            //{
            //    controller = "SepNopFarsi",
            //    action = "Verify"
            //}, new string[]
            //{
            //    "NopFarsi.Payments.AsanPardakht.Controllers"
            //});
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

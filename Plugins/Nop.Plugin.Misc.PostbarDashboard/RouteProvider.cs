
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;
using System.Collections;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.PostbarDashboard
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            #region Dashboard postex & postbar
            //routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.Dashboard",
            //        "Dashboard",
            //        new { controller = "PostbarDashboard", action = "Dashboard" }
            //    );
            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.Dashboard",
                    "Dashboard",
                    new { controller = "PostbarDashboard", action = "Dashboard2" }
                );
            routeBuilder.MapLocalizedRoute("CustomerWallet",
                "Dashboard/Wallet",
                new { controller = "PostbarDashboard", action = "Wallet" }
            );
			routeBuilder.MapLocalizedRoute("CustomerChargeWalletHistory",
				"Dashboard/CustomerChargeWalletHistory",
				new { controller = "PostbarDashboard", action = "CustomerChargeWalletHistory" }
			);
			routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.WalletPaged",
                 "Plugins/PostbarDashboard/WalletPaged",
                 new { controller = "PostbarDashboard", action = "WalletPaged" }
            );

            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.Orders",
                "Dashboard/Orders",
                new { controller = "PostbarDashboard", action = "Orders" }
            );

            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.OrdersPaged",
                 "Plugins/PostbarDashboard/OrdersPaged",
                 new { controller = "PostbarDashboard", action = "OrdersPaged" }
            );
            ///*******************************
            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.CustomerConfirmOrderUbaar",
               "Dashboard/CustomerConfirmOrderUbaar/{OrderItem:regex(\\d*)}",
               new { controller = "PostbarDashboard", action = "CustomerConfirmOrderUbaar" }
           );
            //routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.ManageAvatarCustomer",
            // "Dashboard/AvatarCustomer/{Id:regex(\\d*)}",
            //  new { controller = "PostbarDashboard", action = "CustomerConfirmOrderUbaar" }
            // );

            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.support",
                "Dashboard/support",
                new { controller = "PostbarDashboard", action = "support" }
            );
            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.Damages",
                "Dashboard/Damages",
                new { controller = "PostbarDashboard", action = "Damages" }
            );
            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.RequestCOD",
              "Dashboard/RequestCOD",
              new { controller = "PostbarDashboard", action = "RequestCOD" }
          );
            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.FAQ",
               "Dashboard/FAQ",
               new { controller = "PostbarDashboard", action = "FAQ" }
           );
            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.supportPaged",
                 "Dashboard/supportPaged",
                 new { controller = "PostbarDashboard", action = "supportPaged" }
            );
            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.DamagesPaged",
                "Dashboard/DamagesPaged",
                new { controller = "PostbarDashboard", action = "DamagesPaged" }
           );
            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.RequestCODPaged",
               "Dashboard/RequestCODPaged",
               new { controller = "PostbarDashboard", action = "RequestCODPaged" }
          );

            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.AddSupport",
                "Dashboard/AddSupport",
                new { controller = "PostbarDashboard", action = "AddSupport" }
           );
            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.AddDamages",
                "Dashboard/AddDamages",
                new { controller = "PostbarDashboard", action = "AddDamages" }
           );
            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.AddRequestCOD",
                "Dashboard/AddRequestCOD",
                new { controller = "PostbarDashboard", action = "AddRequestCOD" }
           );

            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.DetailSupport",
               "Dashboard/DetailSupport",
               new { controller = "PostbarDashboard", action = "DetailSupport" }
           );

            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.DetailDamages",
           "Dashboard/DetailDamages",
           new { controller = "PostbarDashboard", action = "DetailDamages" }
       );
            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.DetailRequestCOD",
              "Dashboard/DetailRequestCOD",
              new { controller = "PostbarDashboard", action = "DetailRequestCOD" }
          );

            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.Services",
                "Dashboard/Services",
                new { controller = "PostbarDashboard", action = "Services" }
            );


            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.Customersubset",
               "Dashboard/Customersubset",
               new { controller = "PostbarDashboard", action = "Customersubset" }
           );
            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.CustomersubsetPaged",
                 "Dashboard/CustomersubsetPaged",
                 new { controller = "PostbarDashboard", action = "CustomersubsetPaged" }
            );


            ///*****************************
            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.CustomerInfo",
                 "Dashboard/CustomerInfo",
                 new { controller = "PostbarDashboard", action = "CustomerInfo" }
            );

            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.ChangePassword",
                 "Dashboard/ChangePass",
                 new { controller = "PostbarDashboard", action = "ChangePass" }
            );

            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.Avatar",
                 "Dashboard/Avatar",
                 new { controller = "PostbarDashboard", action = "Avatar" }
            );

            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PostbarDashboard.PostalAddress",
                 "Dashboard/PostalAddress",
                 new { controller = "PostbarDashboard", action = "PostalAddress" }
            );

            routeBuilder.MapLocalizedRoute("CustomerSetting",
               "Dashboard/Setting",
               new { controller = "PostbarDashboard", action = "CustomerSetting" }
           );
            #endregion
            #region Dashboard Ap
            routeBuilder.MapLocalizedRoute("Ap.Dashboard",
                    "ApDashboard",
                    new { controller = "ApDashboard", action = "Dashboard" }
                );

            //routeBuilder.MapLocalizedRoute("ApCustomerWallet",
            //    "ApDashboard/Wallet",
            //    new { controller = "ApDashboard", action = "Wallet" }
            //);

            //routeBuilder.MapLocalizedRoute("Ap.WalletPaged",
            //     "Ap/WalletPaged",
            //     new { controller = "ApDashboard", action = "WalletPaged" }
            //);

            routeBuilder.MapLocalizedRoute("Ap.Orders",
                "ApDashboard/Orders",
                new { controller = "ApDashboard", action = "Orders" }
            );

            routeBuilder.MapLocalizedRoute("Ap.OrdersPaged",
                 "Ap/OrdersPaged",
                 new { controller = "ApDashboard", action = "OrdersPaged" }
            );
            ///*******************************
            routeBuilder.MapLocalizedRoute("Ap.support",
                "ApDashboard/support",
                new { controller = "ApDashboard", action = "support" }
            );
            routeBuilder.MapLocalizedRoute("Ap.FAQ",
               "ApDashboard/FAQ",
               new { controller = "ApDashboard", action = "FAQ" }
           );
            routeBuilder.MapLocalizedRoute("Ap.supportPaged",
                 "Ap/supportPaged",
                 new { controller = "ApDashboard", action = "supportPaged" }
            );


            routeBuilder.MapLocalizedRoute("Ap.AddSupport",
                "ApDashboard/AddSupport",
                new { controller = "ApDashboard", action = "AddSupport" }
           );

            routeBuilder.MapLocalizedRoute("Ap.DetailSupport",
               "ApDashboard/DetailSupport",
               new { controller = "ApDashboard", action = "DetailSupport" }
           );

            routeBuilder.MapLocalizedRoute("Ap.Services",
                "ApDashboard/Services",
                new { controller = "ApDashboard", action = "Services" }
            );
            ///*****************************
            routeBuilder.MapLocalizedRoute("Ap.CustomerInfo",
                 "ApDashboard/CustomerInfo",
                 new { controller = "ApDashboard", action = "CustomerInfo" }
            );

            routeBuilder.MapLocalizedRoute("Ap.ChangePassword",
                 "ApDashboard/ChangePass",
                 new { controller = "ApDashboard", action = "ChangePass" }
            );

            routeBuilder.MapLocalizedRoute("Ap.Avatar",
                 "ApDashboard/Avatar",
                 new { controller = "ApDashboard", action = "Avatar" }
            );

            routeBuilder.MapLocalizedRoute("Ap.PostalAddress",
                 "ApDashboard/PostalAddress",
                 new { controller = "ApDashboard", action = "PostalAddress" }
            );


            routeBuilder.MapLocalizedRoute("Ap.AddRequestCOD",
                "ApDashboard/AddRequestCOD",
                new { controller = "ApDashboard", action = "AddRequestCOD" }
           );
            #endregion

            #region Dashboard Sep
            routeBuilder.MapLocalizedRoute("Sep.Dashboard",
                    "SepDashboard",
                    new { controller = "SepDashboard", action = "Dashboard" }
                );

            //routeBuilder.MapLocalizedRoute("SepCustomerWallet",
            //    "SepDashboard/Wallet",
            //    new { controller = "SepDashboard", action = "Wallet" }
            //);

            //routeBuilder.MapLocalizedRoute("Sep.WalletPaged",
            //     "Sep/WalletPaged",
            //     new { controller = "SepDashboard", action = "WalletPaged" }
            //);

            routeBuilder.MapLocalizedRoute("Sep.Orders",
                "SepDashboard/Orders",
                new { controller = "SepDashboard", action = "Orders" }
            );

            routeBuilder.MapLocalizedRoute("Sep.OrdersPaged",
                 "Sep/OrdersPaged",
                 new { controller = "SepDashboard", action = "OrdersPaged" }
            );
            ///*******************************
            routeBuilder.MapLocalizedRoute("Sep.support",
                "SepDashboard/support",
                new { controller = "SepDashboard", action = "support" }
            );
            routeBuilder.MapLocalizedRoute("Sep.FAQ",
               "SepDashboard/FAQ",
               new { controller = "SepDashboard", action = "FAQ" }
           );
            routeBuilder.MapLocalizedRoute("Sep.supportPaged",
                 "Sep/supportPaged",
                 new { controller = "SepDashboard", action = "supportPaged" }
            );


            routeBuilder.MapLocalizedRoute("Sep.AddSupport",
                "SepDashboard/AddSupport",
                new { controller = "SepDashboard", action = "AddSupport" }
           );

            routeBuilder.MapLocalizedRoute("Sep.DetailSupport",
               "SepDashboard/DetailSupport",
               new { controller = "SepDashboard", action = "DetailSupport" }
           );

            routeBuilder.MapLocalizedRoute("Sep.Services",
                "SepDashboard/Services",
                new { controller = "SepDashboard", action = "Services" }
            );
            ///*****************************
            routeBuilder.MapLocalizedRoute("Sep.CustomerInfo",
                 "SepDashboard/CustomerInfo",
                 new { controller = "SepDashboard", action = "CustomerInfo" }
            );

            routeBuilder.MapLocalizedRoute("Sep.ChangePassword",
                 "SepDashboard/ChangePass",
                 new { controller = "SepDashboard", action = "ChangePass" }
            );

            routeBuilder.MapLocalizedRoute("Sep.Avatar",
                 "SepDashboard/Avatar",
                 new { controller = "SepDashboard", action = "Avatar" }
            );

            routeBuilder.MapLocalizedRoute("Sep.PostalAddress",
                 "SepDashboard/PostalAddress",
                 new { controller = "SepDashboard", action = "PostalAddress" }
            );


            routeBuilder.MapLocalizedRoute("Sep.AddRequestCOD",
                "SepDashboard/AddRequestCOD",
                new { controller = "SepDashboard", action = "AddRequestCOD" }
           );
            #endregion

        }

        public int Priority
        {
            get
            {
                return 10;
            }
        }

        private bool findAndRemoveRoute(IList<IRouter> list, string routeName)
        {
            Route findedRoute = null;
            foreach (Route item in list)
            {
                if (string.Equals(item.Name, routeName, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    findedRoute = item;
                    break;
                }
            }
            if (findedRoute != null)
            {
                list.Remove(findedRoute);
                return true;
            }
            return false;
        }
    }
}
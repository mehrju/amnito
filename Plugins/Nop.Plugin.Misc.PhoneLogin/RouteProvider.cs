
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;
using System.Collections;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.PhoneLogin
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //ViewEngines.Engines.Insert(0, new CustomViewEngine());

            findAndRemoveRoute(routeBuilder.Routes, "login");
            findAndRemoveRoute(routeBuilder.Routes, "Register");
            findAndRemoveRoute(routeBuilder.Routes, "nfregister");
            findAndRemoveRoute(routeBuilder.Routes, "PasswordRecovery");
            findAndRemoveRoute(routeBuilder.Routes, "Plugin.Misc.NopshopCalendar.Customer.Register");
            findAndRemoveRoute(routeBuilder.Routes, "Plugin.Misc.NopshopCalendar.Customer.Register2");


            routeBuilder.MapLocalizedRoute("Login",
                "Login",
                new { controller = "PhoneLogin", action = "Login" }
            );
            
            routeBuilder.MapLocalizedRoute("Register",
                    "register/",
                    new { controller = "PhoneLogin", action = "Register" }
            );
          
            routeBuilder.MapLocalizedRoute("PasswordRecovery",
                "PasswordRecovery",
                new { controller = "PhoneLogin", action = "PasswordRecovery" }                
           );

            routeBuilder.MapLocalizedRoute("Plugin.Misc.PhoneLogin.RetriveActivationCode",
                 "Plugins/PhoneLogin/RetriveActivationCode",
                 new { controller = "PhoneLogin", action = "RetriveActivationCode" }
            );

            routeBuilder.MapLocalizedRoute("Plugin.Misc.PhoneLogin.ResendActivationCode",
                 "Plugins/PhoneLogin/ResendActivationCode",
                 new { controller = "PhoneLogin", action = "ResendActivationCode" }
            );

        }

        public int Priority
        {
            get
            {
                return -2;
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
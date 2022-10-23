using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Localization;
using System;

namespace Nop.plugin.Orders.ExtendedShipment
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            var dataTokens = new RouteValueDictionary() {
                { "Namespaces",new[] { "Nop.plugin.Orders.ExtendedShipment.Controllers" } }
            };




            //routeBuilder.MapLocalizedRoute("ShoppingCart1"
            //    , "cart1/"
            //    , new { controller = "ExtendedOrderCustomer", action = "ChangeRoutOfCart" }
            //    , constraints: null
            //    , dataTokens: dataTokens
            //);

            routeBuilder.MapLocalizedRoute(
                "GetOrderPdf58mInvoice"
                , "ExtendedOrderOrder/PdfInvoice58m"
                , new { controller = "ExtendedOrder", action = "GetPdf58mInvoice", area = "admin" }
                , constraints: null
                , dataTokens: dataTokens
            );

            routeBuilder.MapLocalizedRoute(
                "GetPrintLable50MMInvoice"
                , "printLable/{orderId}"
                , new { controller = "ExtendedOrder", action = "PrintLable50MM" }
                , constraints: null
                , dataTokens: dataTokens
            );

            routeBuilder.MapLocalizedRoute(
            "Rph_Cashout",
            "Rph/Cashout",
                        new { controller = "ExtendedCustomer", action = "RewardPointsHistoryCashout", area = "admin" }
            , constraints: null
                , dataTokens: dataTokens);

            routeBuilder.MapLocalizedRoute(
            "Rph_CashoutExcel",
            "Rph/CashoutExcel",
                        new { controller = "ExtendedCustomer", action = "ExcelRewardPointsHistory", area = "admin" }
            , constraints: null
                , dataTokens: dataTokens);

            routeBuilder.MapLocalizedRoute(
            "Dashboard_Messaging",
            "Dashboard/Messaging",
                        new { controller = "ExtendedCustomer", action = "DashboardMessaging", area = "admin" }
            , constraints: null
                , dataTokens: dataTokens);

            routeBuilder.MapLocalizedRoute(
                "Dashboard_RequestFactor",
                "Dashboard/RequestFactor",
                            new { controller = "ExtendedCustomer", action = "RequestFactor", area = "admin" }
                , constraints: null
                    , dataTokens: dataTokens);

            //login

            Route LogoutRoute = null;
            Route LoginRoute = null;
            Route CustomerOrdersRute = null;
            Route OrderDetails = null;
            foreach (Route item in routeBuilder.Routes)
            {
                if (item.Name == "CustomerOrders")
                {
                    CustomerOrdersRute = item;
                    break;
                }
            }
            foreach (Route item in routeBuilder.Routes)
            {
                if (item.Name == "Logout")
                {
                    LogoutRoute = item;
                    break;
                }
            }
            foreach (Route item in routeBuilder.Routes)
            {
                if (item.Name == "Login")
                {
                    LoginRoute = item;
                    break;
                }
            }
            //foreach (Route item in routeBuilder.Routes)
            //{
            //    if (item.Name == "OrderDetails")
            //    {
            //        OrderDetails = item;
            //        break;
            //    }
            //}
            if (CustomerOrdersRute != null) routeBuilder.Routes.Remove(CustomerOrdersRute);
            if (LogoutRoute != null) routeBuilder.Routes.Remove(LogoutRoute);
            if (LoginRoute != null) routeBuilder.Routes.Remove(LoginRoute);
            //  if(OrderDetails != null) routeBuilder.Routes.Remove(OrderDetails);

            routeBuilder.MapLocalizedRoute(
               "CustomerOrders"
               , "order/history"
               , new { controller = "ExtendedOrder", action = "CustomerOrders" }
               , constraints: null
               , dataTokens: dataTokens
               );

            routeBuilder.MapLocalizedRoute("Login", "login/",
               new { controller = "ExtendedPublicCustomer", action = "Login" });

            routeBuilder.MapLocalizedRoute("Logout", "logout/",
                new { controller = "ExtendedPublicCustomer", action = "Logout" });

            routeBuilder.MapLocalizedRoute("CommentList", "managecomment/List", new { controller = "ManageComment", action = "List", area = "admin" }, constraints: null
                , dataTokens: dataTokens);


            //routeBuilder.MapLocalizedRoute("OrderDetails", "orderdetails/{orderId:min(0)}",
            //    new { controller = "ExtendedOrderCustomer", action = "Details" });

            routeBuilder.MapLocalizedRoute("NewShipmentList", "Admin/Order/NewShipmentList",
            new { controller = "ExtendedOrder", action = "NewShipmentList", area = "admin" });

            routeBuilder.MapLocalizedRoute("Shipments", "Admin/Order/Shipments",
            new { controller = "Shipments", action = "Index", area = "admin" });

            routeBuilder.MapLocalizedRoute("eventcategory", "Admin/Order/EventCategory",
             new { controller = "ExtendedOrder", action = "EventCategory", area = "admin" });

            routeBuilder.MapLocalizedRoute("GetAllCategories", "Admin/Order/GetAllCategories",
            new { controller = "ExtendedOrder", action = "GetAllCategories", area = "admin" });

            routeBuilder.MapLocalizedRoute("GetCategoriesByStatus", "Admin/Order/GetCategoriesByStatus",
            new { controller = "ExtendedOrder", action = "GetCategoriesByStatus", area = "admin" });
            
            routeBuilder.MapLocalizedRoute("UpdateCategoryStatus", "Admin/Order/UpdateCategoryStatus",
            new { controller = "ExtendedOrder", action = "UpdateCategoryStatus", area = "admin" });
            
            routeBuilder.MapLocalizedRoute("GetOrderStatusByCategoryId", "Admin/Order/GetOrderStatusByCategoryId",
            new { controller = "ExtendedOrder", action = "GetOrderStatusByCategoryId", area = "admin" });

            routeBuilder.MapLocalizedRoute("IsAllowedToChangeStatus", "Admin/Order/IsAllowedToChangeStatus",
            new { controller = "ExtendedOrder", action = "IsAllowedToChangeStatus", area = "admin" });

        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => -1;

    }
}

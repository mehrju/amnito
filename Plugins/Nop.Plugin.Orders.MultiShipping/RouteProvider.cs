using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Localization;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Orders.MultiShipping
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
				{ "Namespaces",new[] { "Nop.Plugin.Orders.MultiShipping.Controllers" } }
			};
			findAndRemoveRoute(routeBuilder.Routes, "CheckoutCompleted");
			routeBuilder.MapLocalizedRoute("CheckoutCompleted", "checkout/completed/{orderId:regex(\\d*)}",
				new { controller = "SuccessOrder", action = "Completed" });

			#region HomePage
			Route homePageRoute = null;

			foreach (Route item in routeBuilder.Routes)
			{
				if (item.Name == "HomePage")
				{
					homePageRoute = item;
					break;
				}
			}
			if (homePageRoute != null)
			{
				routeBuilder.Routes.Remove(homePageRoute);
			}

			routeBuilder.MapLocalizedRoute("HomePage", "",
			   new { controller = "Dispacher", action = "Index" });


			routeBuilder.MapLocalizedRoute("HomePageIndex", "Home/Index",
			   new { controller = "Dispacher", action = "Index" });


			#endregion

			#region Postbar

			routeBuilder.MapLocalizedRoute("PostbarHome", "Home",
			 new { controller = "NewHome", action = "Index" });

			#region Wallet
			routeBuilder.MapLocalizedRoute("_PayForCharge", "PayForCharge",
			   new { controller = "NewCheckout", action = "ChargeWallet" });


			routeBuilder.MapLocalizedRoute(
			   "_ChargeWallet"
			   , "ChargeWallet"
			   , new { controller = "NewCheckout", action = "ChargeWalletIndex" }
			   , constraints: null
			   , dataTokens: dataTokens
			   );
			#endregion

			#region Checkout and Pay
			routeBuilder.MapLocalizedRoute(
					"NewCheckout"
					, "Order/NewCheckout"
					, new { controller = "NewCheckout", action = "Index" }
					, constraints: null
					, dataTokens: dataTokens
					);
			routeBuilder.MapLocalizedRoute(
					"Bu_NewCheckout"
					, "Order/BuCheckout"
					, new { controller = "ShipitoCheckout", action = "Bu_Index" }
					, constraints: null
					, dataTokens: dataTokens
					);
			routeBuilder.MapLocalizedRoute(
				"BillAndPayment"
				, "order/billpayment"
				, new { controller = "NewCheckout", action = "ShowBillAndPayment" }
				, constraints: null
				, dataTokens: dataTokens
				);
			routeBuilder.MapLocalizedRoute(
			 "ConfirmAndPay"
			 , "ConfirmAndPay"
			 , new { controller = "NewCheckout", action = "ConfirmAndPay" }
			 , constraints: null
			 , dataTokens: dataTokens
			 );
			#endregion
			#endregion

			#region Ap

			routeBuilder.MapLocalizedRoute("ApBulkCheckout", "Ap/BulkCheckout",
			   new { controller = "Ap_NewCheckout", action = "BulkIndex" });

			routeBuilder.MapLocalizedRoute("ApCheckoutCompleted", "ApCheckout/completed",
			   new { controller = "ApSuccessOrder", action = "Completed" });

			routeBuilder.MapLocalizedRoute("ApTracking", "Ap/Tracking",
			   new { controller = "Ap_NewCheckout", action = "IndexTracking" });

			routeBuilder.MapLocalizedRoute(
						 "ApStartup"
						 , "Ap/Startup"
						 , new { controller = "Ap_NewHome", action = "Index" }
						 , constraints: null
						 , dataTokens: dataTokens
						 );

			routeBuilder.MapLocalizedRoute(
			   "_ApStartup"
			   , "Ap/_Startupt"
			   , new { controller = "Ap_NewHome", action = "_Index" }
			   , constraints: null
			   , dataTokens: dataTokens
			   );

			routeBuilder.MapLocalizedRoute(
				  "ApCheckout"
				  , "Ap/ApCheckout"
				  , new { controller = "Ap_NewCheckout", action = "Index" }
				  , constraints: null
				  , dataTokens: dataTokens
				  );

			routeBuilder.MapLocalizedRoute(
					"ApBillAndPayment"
					, "order/ApBillpayment"
					, new { controller = "Ap_NewCheckout", action = "ShowBillAndPayment" }
					, constraints: null
					, dataTokens: dataTokens
					);

			routeBuilder.MapLocalizedRoute(
				"_ApPDF"
				, "ShipitoHome/GetPDF/{orderId:regex(\\d*)}"
				, new { controller = "ShipitoHome", action = "GetPDF" }
				, constraints: null
				, dataTokens: dataTokens
				);

			routeBuilder.MapLocalizedRoute(
				"ApSaleCartonwrapper"
				, "Ap/CartonSale"
				, new { controller = "SaleCartonwrapper", action = "ApProductDetail" }
				, constraints: null
				, dataTokens: dataTokens
			  );

			routeBuilder.MapLocalizedRoute(
				"Ap_Sh_ConfirmAndPaySaleCarton"
				, "Ap/CartonSalePay"
				, new { controller = "SaleCartonwrapper", action = "ApConfirmAndPaySaleCarton" }
				, constraints: null
				, dataTokens: dataTokens
				);

			#endregion

			#region Sep


			routeBuilder.MapLocalizedRoute("SepCheckoutCompleted", "SepCheckout/completed",
			   new { controller = "SepSuccessOrder", action = "Completed" });

			routeBuilder.MapLocalizedRoute("SepBulkCheckout", "Sep/BulkCheckout",
			   new { controller = "Ap_NewCheckout", action = "BulkIndex" });

			routeBuilder.MapLocalizedRoute(
				 "SepBillAndPayment"
				 , "order/SepBillpayment"
				 , new { controller = "Sep_NewCheckout", action = "ShowBillAndPayment" }
				 , constraints: null
				 , dataTokens: dataTokens
				 );
			routeBuilder.MapLocalizedRoute(
					"_SepIPGStartup"
					, "Sep/_Startup"
					, new { controller = "Sep_NewHome", action = "MainIndex" }
					, constraints: null
					, dataTokens: dataTokens
				);
			routeBuilder.MapLocalizedRoute(
					"_SepIPGIndex"
					, "Sep/_Index"
					, new { controller = "Sep_NewHome", action = "Index" }
					, constraints: null
					, dataTokens: dataTokens
				);
			routeBuilder.MapLocalizedRoute(
				   "_SepLogin"
				   , "Sep/login"
				   , new { controller = "Sep_NewHome", action = "login" }
				   , constraints: null
				   , dataTokens: dataTokens
			   );
			routeBuilder.MapLocalizedRoute(
			   "_SepStartup"
			   , "Sep/_Startupt"
			   , new { controller = "Sep_NewHome", action = "_Index" }
			   , constraints: null
			   , dataTokens: dataTokens
			   );
			routeBuilder.MapLocalizedRoute(
			  "SepCheckout"
			  , "Sep/SepCheckout"
			  , new { controller = "Sep_NewCheckout", action = "Index" }
			  , constraints: null
			  , dataTokens: dataTokens
			  );



			routeBuilder.MapLocalizedRoute(
				"_SepPDF"
				, "ShipitoHome/GetPDF/{orderId:regex(\\d*)}"
				, new { controller = "ShipitoHome", action = "GetPDF" }
				, constraints: null
				, dataTokens: dataTokens
				);

			routeBuilder.MapLocalizedRoute(
				"SepSaleCartonwrapper"
				, "Sep/CartonSale"
				, new { controller = "SaleCartonwrapper", action = "SepProductDetail" }
				, constraints: null
				, dataTokens: dataTokens
			  );

			routeBuilder.MapLocalizedRoute(
				"Sep_ConfirmAndPaySaleCarton"
				, "Sep/CartonSalePay"
				, new { controller = "SaleCartonwrapper", action = "SepConfirmAndPaySaleCarton" }
				, constraints: null
				, dataTokens: dataTokens
				);

			#endregion
			
			routeBuilder.MapLocalizedRoute("PostexLanding", "Home/About-us", new { controller = "NewHome", action = "Postex" });
			routeBuilder.MapLocalizedRoute("PostexLandingfa", "Home/درباره ما", new { controller = "NewHome", action = "Postex" });

			routeBuilder.MapLocalizedRoute("DomesticPost", "Home/DomesticPost", new { controller = "NewHome", action = "DomesticPost" });
			routeBuilder.MapLocalizedRoute("ForeignPost", "Home/ForeignPost", new { controller = "NewHome", action = "ForeignPost" });
			routeBuilder.MapLocalizedRoute("PeykhubLanding", "Home/Peykhub", new { controller = "NewHome", action = "Peykhub" });

			routeBuilder.MapLocalizedRoute("OnlineOrder", "Home/OnlineOrder", new { controller = "NewHome", action = "OnlineOrder" });
			routeBuilder.MapLocalizedRoute("CodOrder", "Home/CodOrder", new { controller = "NewHome", action = "CodOrder" });
			routeBuilder.MapLocalizedRoute("AmnitoOrder", "Home/AmnitoOrder", new { controller = "NewHome", action = "AmnitoOrder" });
			routeBuilder.MapLocalizedRoute("HomeBulkOrder", "Home/BulkOrder", new { controller = "NewHome", action = "BulkOrder" });
			routeBuilder.MapLocalizedRoute("OrganizationOrder", "Home/OrganizationOrder", new { controller = "NewHome", action = "OrganizationOrder" });
			routeBuilder.MapLocalizedRoute("Shopex", "Home/Shopex", new { controller = "NewHome", action = "Shopex" });
			routeBuilder.MapLocalizedRoute("HomeWallet", "Home/Wallet", new { controller = "NewHome", action = "Wallet" });
			routeBuilder.MapLocalizedRoute("Agencies", "Home/Agencies", new { controller = "NewHome", action = "Agencies" });
			routeBuilder.MapLocalizedRoute("Packing", "Home/Packing", new { controller = "NewHome", action = "Packing" });
			routeBuilder.MapLocalizedRoute("ApiRequest", "Home/Api-request", new { controller = "NewHome", action = "ApiRequest" });

			//routeBuilder.MapLocalizedRoute("PostbarLanding", "Home/partners/postbar", new { controller = "NewHome", action = "Postbar" });

			routeBuilder.MapLocalizedRoute("ChaparLanding", "Home/partners/chapar", new { controller = "NewHome", action = "Chapar" });
			routeBuilder.MapLocalizedRoute("MahexLanding", "Home/partners/mahex", new { controller = "NewHome", action = "Mahex" });
			routeBuilder.MapLocalizedRoute("YarboxLanding", "Home/partners/yarbox", new { controller = "NewHome", action = "Yarbox" });

			routeBuilder.MapLocalizedRoute("PDELanding", "Home/partners/pde", new { controller = "NewHome", action = "PDE" });
			routeBuilder.MapLocalizedRoute("RaheAsemanLanding", "Home/partners/rahe-aseman", new { controller = "NewHome", action = "RaheAsemanAbi" });
			routeBuilder.MapLocalizedRoute("TPGLanding", "Home/partners/tpg", new { controller = "NewHome", action = "TPG" });
			
			routeBuilder.MapLocalizedRoute("SnappBoxLanding", "Home/partners/snappbox", new { controller = "NewHome", action = "SnappBox" });
			routeBuilder.MapLocalizedRoute("PersiaExpressLanding", "Home/partners/persia-express", new { controller = "NewHome", action = "PersiaExpress" });
			
			routeBuilder.MapLocalizedRoute("ObarLanding", "Home/partners/obar", new { controller = "NewHome", action = "Obar" });

			routeBuilder.MapLocalizedRoute("KadoxLanding", "Home/Kadox", new { controller = "NewHome", action = "Kadox" });
			routeBuilder.MapLocalizedRoute("ShipitoPlusLanding", "Home/ShipitoPlus", new { controller = "NewHome", action = "ShipitoPlus" });
			routeBuilder.MapLocalizedRoute("PostexYarLanding", "Home/PostexYar", new { controller = "NewHome", action = "PostexYar" });
			routeBuilder.MapLocalizedRoute("ColdexLanding", "Home/Coldex", new { controller = "NewHome", action = "Coldex" });
			routeBuilder.MapLocalizedRoute("SubexLanding", "Home/Subex", new { controller = "NewHome", action = "Subex" });
			routeBuilder.MapLocalizedRoute("HeavyPostLanding", "Home/HeavyPost", new { controller = "NewHome", action = "HeavyPost" });





			routeBuilder.MapLocalizedRoute("PtxPreOrderInfo",
			   "PreOrderInfo",
			   new { controller = "Ptx_Checkout", action = "ShowPreOrderInfo" });

			routeBuilder.MapLocalizedRoute("ItSazLogin",
			   "user/login-partners",
			   new { controller = "ShipitoCheckout", action = "ItSazlogin" });

			routeBuilder.MapLocalizedRoute("Conamlogin",
			   "eghabz",
			   new { controller = "ShipitoCheckout", action = "Conamlogin" });
			routeBuilder.MapLocalizedRoute("Conamlogin1",
			   "conam",
			   new { controller = "ShipitoCheckout", action = "Conamlogin" });

			routeBuilder.MapLocalizedRoute("SaleCartonwrapper",
			   "CartonSale",
			   new { controller = "SaleCartonwrapper", action = "ProductDetail" });

			routeBuilder.MapLocalizedRoute("_Sh_ConfirmAndPaySaleCarton",
			   "CartonSalePay",
			   new { controller = "SaleCartonwrapper", action = "ConfirmAndPaySaleCarton" });

			#region shipito

			routeBuilder.MapLocalizedRoute(
				 "_Sh_Checkout"
				, "Order/Sh_Checkout"
				,
			new { controller = "Ptx_Checkout", action = "Index" });

			routeBuilder.MapLocalizedRoute("_ShipitoHome", "Index",
			 new { controller = "ShipitoHome", action = "Index" });

			//routeBuilder.MapLocalizedRoute("_SekeHome", "Sekeh/_Startup",
			//new { controller = "ShipitoHome", action = "Index" });

			routeBuilder.MapLocalizedRoute("_ApLogin", "ApLogin",
			 new { controller = "ShipitoHome", action = "Login" });

			//routeBuilder.MapLocalizedRoute("CheckoutCompleted", "Completed",
			//new { controller = "SuccessOrder", action = "Completed" });

			#region Wallet
			routeBuilder.MapLocalizedRoute("_Sh_PayForCharge", "Sh_PayForCharge",
			   new { controller = "ShipitoCheckout", action = "ChargeWallet" });


			routeBuilder.MapLocalizedRoute(
			   "_Sh_ChargeWallet"
			   , "Sh_ChargeWallet"
			   , new { controller = "ShipitoCheckout", action = "ChargeWalletIndex" }
			   , constraints: null
			   , dataTokens: dataTokens
			   );


			#endregion

			#region Topic
			routeBuilder.MapLocalizedRoute("TopicCollection", "TopicCollection",
			 new { controller = "TopicCollection", action = "Index" });
			#endregion

			#region Checkout and Pay
			//routeBuilder.MapLocalizedRoute(
			//        "_Sh_Checkout"
			//        , "Order/Sh_Checkout"
			//        , new { controller = "ShipitoCheckout", action = "Index" }
			//        , constraints: null
			//        , dataTokens: dataTokens
			//        );

			routeBuilder.MapLocalizedRoute(
				"_Sh_BillAndPayment"
				, "order/Sh_billpayment"
				, new { controller = "ShipitoCheckout", action = "ShowBillAndPayment" }
				, constraints: null
				, dataTokens: dataTokens
				);
			routeBuilder.MapLocalizedRoute(
				"_Sh_BillAndPayment_"
				, "OrderPayment"
				, new { controller = "ShipitoCheckout", action = "_ShowBillAndPayment" }
				, constraints: null
				, dataTokens: dataTokens
				);
			routeBuilder.MapLocalizedRoute(
				"SafeBuyPayForOrderIndex"
				, "order/SafeBuyPayForOrderIndex"
				, new { controller = "ShipitoCheckout", action = "PayForSafeBuyOrder" }
				, constraints: null
				, dataTokens: dataTokens
				);


			routeBuilder.MapLocalizedRoute(
			 "_Sh_ConfirmAndPay"
			 , "Sh_ConfirmAndPay"
			 , new { controller = "ShipitoCheckout", action = "ConfirmAndPay" }
			 , constraints: null
			 , dataTokens: dataTokens
			 );
			#endregion
			#endregion

			#region hyperjet
			routeBuilder.MapLocalizedRoute(
					"hyperJetStartup"
					, "hyperJet/Startup"
					, new { controller = "ShipitoHome", action = "hyperJetIndex" }
					, constraints: null
					, dataTokens: dataTokens
					);

			#endregion

			#region multiShiping
			routeBuilder.MapLocalizedRoute(
				   "CheckoutShipment"
				   , "checkout/Shipment"
				   , new { controller = "MultiShippingCheckout", action = "Shipment" }
				   , constraints: null
				   , dataTokens: dataTokens
				   );
			routeBuilder.MapLocalizedRoute(
			   "mus_selectBillingAddress"
			   , "checkout/Mus_selectBillingAddress"
			   , new { controller = "MultiShippingCheckout", action = "Mus_SelectBillingAddress" }
			   , constraints: null
			   , dataTokens: dataTokens
			   );
			routeBuilder.MapLocalizedRoute(
			  "mus_SaveShipmentNumbers"
			  , "checkout/SaveShipmentNumbers"
			  , new { controller = "MultiShippingCheckout", action = "SaveShipmentNumbers" }
			  , constraints: null
			  , dataTokens: dataTokens
			  );
			routeBuilder.MapLocalizedRoute(
			 "mus_SaveShipmentMethods"
			 , "checkout/SaveShipmentMethods"
			 , new { controller = "MultiShippingCheckout", action = "SaveShipmentMethods" }
			 , constraints: null
			 , dataTokens: dataTokens
			 );
			routeBuilder.MapLocalizedRoute(
			 "mus_SaveShipmentAddresses"
			 , "checkout/SaveShipmentAddresses"
			 , new { controller = "MultiShippingCheckout", action = "SaveShipmentAddresses" }
			 , constraints: null
			 , dataTokens: dataTokens
			 );
			routeBuilder.MapLocalizedRoute(
			 "Mus_PaymentMethod"
			 , "checkout/Mus_PaymentMethod"
			 , new { controller = "MultiShippingCheckout", action = "PaymentMethod" }
			 , constraints: null
			 , dataTokens: dataTokens
			 );
			routeBuilder.MapLocalizedRoute(
			"Mus_CheckoutPaymentInfo"
			, "checkout/Mus_CheckoutPaymentInfo"
			, new { controller = "MultiShippingCheckout", action = "PaymentInfo" }
			, constraints: null
			, dataTokens: dataTokens
			);
			routeBuilder.MapLocalizedRoute(
			"Mus_CheckoutConfirm"
			, "checkout/Mus_CheckoutConfirm"
			, new { controller = "MultiShippingCheckout", action = "Confirm" }
			, constraints: null
			, dataTokens: dataTokens
			);


			#endregion

			#region comments
			routeBuilder.MapLocalizedRoute(
		   "Comments"
		   , "Comments/Index"
		   , new { controller = "Comments", action = "Index" }
		   , constraints: null
		   , dataTokens: dataTokens
		   );

			routeBuilder.MapLocalizedRoute(
			"CommentsPost"
			, "Comments/Post"
			, new { controller = "Comments", action = "Post" }
			, constraints: null
			, dataTokens: dataTokens
			);

			routeBuilder.MapLocalizedRoute(
			"CommentsList"
			, "Comments/List"
			, new { controller = "Comments", action = "List" }
			, constraints: null
			, dataTokens: dataTokens
			);
			#endregion

			routeBuilder.MapLocalizedRoute("AndroidApp",
			   "App/Login",
			   new { controller = "ShipitoHome", action = "LoginApp" });
		}
		/// <summary>
		/// Gets a priority of route provider
		/// </summary>
		public int Priority => -2;
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

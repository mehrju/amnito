using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Plugin.Misc.ShippingSolutions.Data;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;
using System.Linq;

namespace Nop.Plugin.Misc.ShippingSolutions
{

    public class ShippingSolutionsPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ShippingSolutionsObjectContext _context;

        public ShippingSolutionsPlugin(ISettingService settingService, IWebHelper webHelper, ShippingSolutionsObjectContext context)
        {
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._context = context;
        }


        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/Shipping/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {

            #region 
            #region Set settings
            var settings = new ShippingSettings
            {
                //pde
                PDE_Authorization = "Basic QXBpQHBkZXhwLmNvbTpjYzM0NGRkYzgzN2FmMzNlZDA4NDE5OWJiZjBhZGNkMw==",
                PDE_Password = "2fKaR0CX",
                PDE_Ccode = 101,
                PDE_ClientId = 23553,
                PDE_URLTrackingParcels = "https://www.pdexp.com/api/statusupdate",
                PDE_URLListOfCountries = "https://www.pdexp.com/api/countries   ",
                PDE_URLListOfCities = "https://www.pdexp.com/api/cities",
                PDE_URLIntenationalCalculator = "https://www.pdexp.com/api/PriceCalculator/PostPrice",
                PDE_URLDomesticCalculator = "https://www.pdexp.com/api/DomesticPriceCalculator/PostPrice",
                PDE_URLRegisterInternationalOrder = "https://www.pdexp.com/api/RegisterOrders/",
                PDE_URLRegisterDomesticOrder = "https://www.pdexp.com/api/RegisterOrders/",


                //yarbox
                //YarBox_Token= "bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiMDkxMzIwOTUyMzQiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3NlcmlhbG51bWJlciI6ImExOWE0ZmEzZTdkNDQ1MjU5YWNjYWZkOWIzMjk2YWQxIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy91c2VyZGF0YSI6IjUxODc4Y2U1LTI1ZjMtZTkxMS1hOTVkLTkwMWIwZWU1NTViMyIsIm5iZiI6MTU4MzMwMDc0NSwiZXhwIjoxNTkxMDQwNzQ1LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0LyIsImF1ZCI6IkFueSJ9.prLWDB_RL5jPskKr4qf6fOuy5NJKIettgwmeuS72v8c",
                YarBox_verifyCode = "arghasemyan1368@gmail.com",
                YarBox_phoneNumber = "09132095234",
                YarBox_playerId = "company",
                YarBox_ExpiresDayToken = 29,
                YarBox_URLAp_Verify = "http://api.yarbox.co/api/vv2/account/verify",
                YarBox_URLApPackingType = "http://api.yarbox.co/api/vv2/Ap-PackingType/Get",
                YarBox_URLAp_Type = "http://api.yarbox.co/api/vv2/Ap-Type/Get",
                YarBox_URLApPostPacks = "http://api.yarbox.co/api/vv2/" + " Ap-PostPacks/add",
                YarBox_URLfactor = "http://api.yarbox.co/api/vv2/" + " Ap-PostPacks/add",
                YarBox_URLAp_PostPacks_accept = "http://api.yarbox.co/api/vv2/" + " Ap-PostPacks/accept",
                YarBox_URLAp_Qute = "http://api.yarbox.co/api/vv2/" + " Ap-PostPacks/assessment-price",
                YarBox_URL_Tracking = "http://api.yarbox.co/api/vv2/" + " Ap-PostPacks/get-status/{0}",
                //Ubaar
                Ubaar_APIToken = "AC2zToa6934",
                Ubaar_USERToken = "03485bf2258070e8142ce57e2eaf19",
                Ubaar_Urlregionlist = "https://dev.ubaar.ir/api/caravan/region_list",
                Ubaar_Urlpriceenquiry = "https://dev.ubaar.ir/api/caravan/price_enquiry",
                Ubaar_Urlmodifyorder = "https://dev.ubaar.ir/api/caravan/modify_order",
                Ubaar_UrlTracking = "https://dev.ubaar.ir/api/caravan/order_details",
                //Tinex
                Tinex_grant_type = "client_credentials",
                Tinex_client_id = "5",
                Tinex_client_secret = "pqOQSBHcLJNId7AWFVT764vtWF6S5kgf",
                Tinex_scope = "order_insert track_status",
                Tinex_ExpireDayToken = 1,
                Tinex_UrlToken = "5.201.128.139:8022/api/oauth/token",
                Tinex_UrlGetCost = "5.201.128.139:8022/api/customer/v1/order/get-cost",
                Tinex_Urlinsert = "5.201.128.139:8022/api/customer/v1/order/insert",
                Tinex_Urlinsertcommit = "5.201.128.139:8022/api/customer/v1/order/insert-commit",
                Tinex_Urlcancel = "5.201.128.139:8022/api/customer/v1/{0}/cancel",
                Tinex_Urlcancelreasons = "5.201.128.139:8022/api/customer/v1/order/cancel-reasons",
                //Safiran
                Safiran_APP_AUTH = "aW9zX2N1c3RvbWVyX2FwcDpUUFhAMjAxNg==",
                Safiran_PickupMan = "8000",
                Safiran_SenderCode = "10825",
                Safiran_UserName = "postwaan",
                Safiran_Password = "POPO2019",
                Safiran_URL_GetState = "http://api.dtsxp.com/get_state?input=null",
                Safiran_URL_City = "http://api.dtsxp.com/get_city",
                Safiran_URL_TRACKING = "http://api.dtsxp.com/tracking",
                Safiran_URL_GetQuote = "http://api.dtsxp.com/get_quote",
                Safiran_URL_PickupRequest = "http://api.dtsxp.com/pickup_request",
                Safiran_URL_BulkImport = "http://api.dtsxp.com/bulk_import",
                Safiran_URL_BulkHistoryReport = "http://api.dtsxp.com/bulk_history_report?input={0}",
                Safiran_URL_HistoryReport = "http://api.dtsxp.com/report?input={0}",
                Safiran_URL_Cancel = "http://api.dtsxp.com/cancel_pickup",
                //persain
                Persain_UserName = "1",
                Persain_Password = "1",
                Persain_URL_NewCustomer = "1",
                Persain_URL_ViewCustomer = "1",

                //TPG
                TPG_UserName = "postbarapi",
                TPG_Password = "vv?%{8SLLc=3w>96I-/p",
                TPG_Url_compute = "https://api.tpg.ir/api/tarrif/compute",
                TPG_Url_Pickup = "https://api.tpg.ir/api/pickup/order",
                TPG_ContractCode = "D1789249 ",
                TPG_ContractId = 37640,
                TPG_UserId = 62195,
                TPG_Url_Receipt = "https://api.tpg.ir/api/v3/receipt/{0}/61976/1/7",
                TPG_Url_Tracking = "https://api.tpg.ir/api/v3/receipt/{0}/62195/1/7",
                //Chapar
                Chapar_APP_AUTH = "aW9zX2N1c3RvbWVyX2FwcDpUUFhAMjAxNg==",
                //Chapar_PickupMan = "8000",
                //Chapar_SenderCode = "29863",
                Chapar_UserName = "Postkhone",
                Chapar_Password = "postkhone",
                Chapar_URL_GetState = "http://api.chaparnet.com/get_state?input=null",
                Chapar_URL_City = "http://api.chaparnet.com/get_city",
                Chapar_URL_TRACKING = "http://api.chaparnet.com/tracking",
                Chapar_URL_GetQuote = "http://api.chaparnet.com/get_quote",
                //Chapar_URL_PickupRequest = "http://api.chaparnet.com/pickup_request",
                Chapar_URL_BulkImport = "http://api.chaparnet.com/bulk_import",
                Chapar_URL_BulkHistoryReport = "http://api.chaparnet.com/bulk_history_report?input={0}",
                Chapar_URL_HistoryReport = "http://api.chaparnet.com/report?input={0}",
                // Chapar_URL_Cancel = "http://api.chaparnet.com/cancel_pickup",
                Mahex_Username = "obtEuZpQ3Ha4MnfL4kYFnGp23hqntcZUP72cF7AhVvyckCwa",
                Mahex_Password = "",
                Mahex_RateUrl = " http://api.mahex.com/v2/rates",
                Mahex_CreateShipmentUrl = "http://api.mahex.com/v2/shipments",
                Mahex_Url_TrackShipmentByUUID = " http://api.mahex.com/v2/track/",
                //mahex setting

                //mahex

                //Tarof
                taroff_APP_AUTH = "0d4dd5762d3d412cbc13342bbb3aca3794425cef74a5424b941d9a9daf68860e",
                taroff_URL_GetCity = "http://service.taroff.ir/city/Cities",
                taroff_URL_GetProvinces = "http://service.taroff.ir/city/provinces",
                taroff_URL_GetListPaymentMethods = "http://service.taroff.ir/Order/ListPaymentMethods",
                taroff_URL_GetListCarriers = "http://service.taroff.ir/Order/ListCarriers",
                taroff_URL_CreateOrder = "http://service.taroff.ir/Order/CreateOrder",
                taroff_URL_GetStateOrder = "http://service.taroff.ir/Order/GetState",
                taroff_URL_SetStateReady = "http://service.taroff.ir/Order/SetStateReady",
                taroff_URL_SetStateCancel = "http://service.taroff.ir/Order/SetStateCancel",
                taroff_URL_Report = "http://service.taroff.ir/Reports/OrderInvoiceOnePdf",

                //Snapbox
                Snappbox_APP_AUTH = "eyJhbGciOiJIUzUxMiJ9.eyJjaWQiOjMzMDM1NTEsImNyaWQiOiI2NTY1OTQ3IiwiZSI6ImJlaHJhbmcuYWdoaWxpQGdtYWlsLmNvbSIsIndlIjp0cnVlLCJzdWIiOiIwOTEyMzg5ODA1OCIsImF1dGgiOiJST0xFX0NVU1RPTUVSIiwidHlwZSI6ImN1c3RvbWVyIn0.JOEVcl7_kNftwcsXEi3Gol8KB-W8Flc71OuRj-Caj87DFgq_XDC_Wk6NWgglH2jmn4sCVSoj02d3tShDhW8N0g",
                Snappbox_CustomerId = "6565947",
                Snappbox_URL_Login = "http://customer.snapp-box.com/v1/customer/auth",
                Snappbox_URL_Get_Account_Balance = "http://customer.snapp-box.com/v1/customer/current_balance",
                Snappbox_URL_Create_Order = "http://customer.snapp-box.com/v1/customer/create_order",
                Snappbox_URL_Get_Order_Details = "http://customer.snapp-box.com/v1/customer/order_details",
                Snappbox_URL_Cancel_Order = "http://customer.snapp-box.com/v1/customer/cancel_order",
                Snappbox_URL_Get_Order_List = "http://customer.snapp-box.com/v1/customer/order_history",
                Snappbox_URL_Get_Price = "http://customer.snapp-box.com/v1/customer/order/pricing",

                //skyblue
                SkyBlue_APP_AUTH= "FB1EA060-75FC-420D-96E5-CAF54B2FA37A",
                SkyBlue_URL_Get_Country= "http://cargobsw.com/postex/api/AjaxAPI/Country",
                SkyBlue_URL_Get_ParcelType= "http://cargobsw.com/postex/api/AjaxAPI/ParcelType",
                SkyBlue_URL_ParcelPrice= "http://cargobsw.com/postex/api/AjaxAPI/ParcelPrice",
                SkyBlue_URL_RegisterOrder = "http://cargobsw.com/postex/api/AjaxAPI/RegisterOrder",
                SkyBlue_URL_Cancel= "http://cargobsw.com/postex/api/AjaxAPI/CancelOrder",
                SkyBlue_URL_Tracking= "http://cargobsw.com/postex/api/AjaxAPI/OrderTracking",
                SkyBlue_URL_CheckService= "http://cargobsw.com/postex/api/AjaxAPI/CheckService",




            };
            _settingService.SaveSetting(settings);
            #endregion

            #region Locale Resource Services
            //pde
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_Authorization", "PDE_Authorization");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_Password", "PDE_Password");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_Ccode", "PDE_Ccode");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_ClientId", "PDE_ClientId");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_URLTrackingParcels", "PDE_URLTrackingParcels");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_URLListOfCountries", "PDE_URLListOfCountries");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_URLListOfCities", "PDE_URLListOfCities");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_URLIntenationalCalculator", "PDE_URLIntenationalCalculator");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_URLDomesticCalculator", "PDE_URLDomesticCalculator");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_URLRegisterInternationalOrder", "PDE_URLRegisterInternationalOrder");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_URLRegisterDomesticOrder", "PDE_URLRegisterDomesticOrder");

            //yarbox
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_verifyCode", "YarBox_verifyCode");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_phoneNumber", "YarBox_phoneNumber");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_playerId", "YarBox_playerId");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_ExpiresDayToken", "YarBox_ExpiresDayToken");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLAp_Verify", "YarBox_URLAp_Verify");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLApPackingType", "YarBox_URLApPackingType");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLAp_Type", "YarBox_URLAp_Type");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLApPostPacks", "YarBox_URLApPostPacks");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLfactor", "YarBox_URLfactor");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLAp_PostPacks_accept", "YarBox_URLAp_PostPacks_accept");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLAp_Qute", "YarBox_URLAp_Qute");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URL_Tracking", "YarBox_URL_Tracking");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_Token", "YarBox_Token");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_DateToken", "YarBox_DateToken");

            
            //Ubaar
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Ubaar_APIToken", "Ubaar_APIToken");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Ubaar_USERToken", "Ubaar_USERToken");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Ubaar_Urlregionlist", "Ubaar_Urlregionlist");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Ubaar_Urlpriceenquiry", "Ubaar_Urlpriceenquiry");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Ubaar_Urlmodifyorder", "Ubaar_Urlmodifyorder");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Ubaar_UrlTracking", "Ubaar_UrlTracking");

            //Tinex
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_grant_type", "Tinex_grant_type");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_client_id", "Tinex_client_id");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_client_secret", "Tinex_client_secret");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_scope", "Tinex_scope");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_ExpireDayToken", "Tinex_ExpireDayToken");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_UrlToken", "Tinex_UrlToken");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_UrlGetCost", "Tinex_UrlGetCost");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_Urlinsert", "Tinex_Urlinsert");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_Urlinsertcommit", "Tinex_Urlinsertcommit");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_Urlcancel", "Tinex_Urlcancel");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_Urlcancelreasons", "Tinex_Urlcancelreasons");
            //Safiran
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_APP_AUTH", "Safiran_APP_AUTH");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_PickupMan", "Safiran_PickupMan");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_SenderCode", "Safiran_SenderCode");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_UserName", "Safiran_UserName");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_Password", "Safiran_Password");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_GetState", "Safiran_URL_GetState");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_City", "Safiran_URL_City");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_TRACKING", "Safiran_URL_TRACKING");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_GetQuote", "Safiran_URL_GetQuote");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_PickupRequest", "Safiran_URL_PickupRequest");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_BulkImport", "Safiran_URL_BulkImport");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_BulkHistoryReport", "Safiran_URL_BulkHistoryReport");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_HistoryReport", "Safiran_URL_HistoryReport");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_Cancel", "Safiran_URL_Cancel");


            //persain
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Persain_UserName", "Persain_UserName");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Persain_Password", "Persain_Password");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Persain_URL_NewCustomer", "Persain_URL_NewCustomer");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Persain_URL_ViewCustomer", "Persain_URL_ViewCustomer");

            //TPG
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_UserName", "TPG_UserName");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_Password", "TPG_Password");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_Url_compute", "TPG_Url_compute");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_Url_Pickup", "TPG_Url_Pickup");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_ContractCode", "TPG_ContractCode");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_ContractId", "TPG_ContractId");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_UserId", "TPG_UserId");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_Url_Receipt", "TPG_Url_Receipt");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_Url_Tracking", "TPG_Url_Tracking");


            //Chapar
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_APP_AUTH", "Chapar_APP_AUTH");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_PickupMan", "Chapar_PickupMan");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_SenderCode", "Chapar_SenderCode");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_UserName", "Chapar_UserName");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_Password", "Chapar_Password");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_GetState", "Chapar_URL_GetState");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_City", "Chapar_URL_City");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_TRACKING", "Chapar_URL_TRACKING");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_GetQuote", "Chapar_URL_GetQuote");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_PickupRequest", "Chapar_URL_PickupRequest");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_BulkImport", "Chapar_URL_BulkImport");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_BulkHistoryReport", "Chapar_URL_BulkHistoryReport");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_HistoryReport", "Chapar_URL_HistoryReport");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_Cancel", "Chapar_URL_Cancel");


            //taroff.ir/
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_APP_AUTH", "taroff_APP_AUTH");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_GetCity", "taroff_URL_GetCity");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_GetProvinces", "taroff_URL_GetProvinces");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_GetListPaymentMethods", "taroff_URL_GetListPaymentMethods");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_GetListCarriers", "taroff_URL_GetListCarriers");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_CreateOrder", "taroff_URL_CreateOrder");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_GetStateOrder", "taroff_URL_GetStateOrder");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_SetStateReady", "taroff_URL_SetStateReady");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_SetStateCancel", "taroff_URL_SetStateCancel");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_Report", "taroff_URL_Report");

            //snapbox
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_APP_AUTH", "Snappbox_APP_AUTH");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Login", "Snappbox_URL_Login");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Get_Account_Balance", "Snappbox_URL_Get_Account_Balance");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Create_Order", "Snappbox_URL_Create_Order");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Get_Order_Details", "Snappbox_URL_Get_Order_Details");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Cancel_Order", "Snappbox_URL_Cancel_Order");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Get_Order_List", "Snappbox_URL_Get_Order_List");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Get_Price", "Snappbox_URL_Get_Price");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_CustomerId", "Snappbox_CustomerId");


            //sky
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_APP_AUTH", "SkyBlue_APP_AUTH");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_URL_Get_Country", "SkyBlue_URL_Get_Country");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_URL_Get_ParcelType", "SkyBlue_URL_Get_ParcelType");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_URL_ParcelPrice", "SkyBlue_URL_ParcelPrice");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_URL_RegisterOrder", "SkyBlue_URL_RegisterOrder");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_URL_Cancel", "SkyBlue_URL_Cancel");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_URL_Tracking", "SkyBlue_URL_Tracking");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_URL_CheckService", "SkyBlue_URL_CheckService");



            //#endregion

            //#region set Local Resource Update/Delete/Create/Duplicate/
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.MessageDisable", "عملیات غیرفعال سازی با موفقیت انجام شد");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.MessageDisable", "عملیات فعال سازی با موفقیت انجام شد");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate", "رکورد تکراری میباشد");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Create", "عملیات ثبت با موفقیت انجام شد");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate", "عملیات ویرایش با موفقیت انجام شد");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.BackToList", "برگشت به لیست");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TabInfo", "مشخصات");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Active", "فعال سازی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Disable", "غیرفعالسازی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Limitation", "محدودیت ها");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PricingPolicy", "قیمت گذاری");

            //#endregion

            //#region set Local Resource Page Manage Provider
            //page create
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.ServiceProvider_pagetitle", "عنوان بالایی صفحه ");
            //page info
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMaxOrder", "حداکثر سفارش روزانه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderName", "نام سرویس دهنده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderAgentName", "نام عامل");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderServiceTypeId", "انتخاب سرویس");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderCategoryId", "انتخاب دسته بندی");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMaxWeight", "حداکثر وزن بسته");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMinWeight", "حداقل وزن بسته");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMaxTimeDeliver", "حداکثر زمان تحویل");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProvideradvancefreight", "پیشکرایه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderfreightforward", "پسکرایه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProvidercod", "امانی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsPishtaz", "پیشتاز");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsSefareshi", "سفارشی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsVIje", "ویژه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsNromal", "نرمال");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsDroonOstani", "درون استانی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsAdjoining", "Adjoining");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsNotAdjacent", "NotAdjacent");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsHeavyTransport", "سنگین");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsForeign", "خارجی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsInCity", "شهری");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsTwoStep", "دومرحله ای");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewHasHagheMaghar", "HasHagheMaghar");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsAmanat", "امانت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewTransportationTypeId", "انتخاب روش های حمل و نقل");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewStoreId", "انتخاب فروشگاهها");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMaxMaxlength", "حدکثر طول");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMaxMaxwidth", "حداکثر عرض");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMaxheight", "حداکثرارتفاع");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMaxbillingamountCOD", "حداکثر مبلغ امانی");




            //page edit
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Provider_Update", "ویرایش سرویس دهنده");

            //search box
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchServicesProviderName", "نام سرویس دهنده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchServicesProviderAgentName", "نام عامل");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchCategoryId", "دسته بندی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchIsActive", "وضعیت(فعال)");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchMaxOrder", "حداکثر تعداد سفارشات روز");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchMaxWeight", "حداکثر وزن هر بسته");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchMinWeight", "حداقل وزن هر بسته");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchMaxTimeDeliver", "حداکثر زمان تحویل محموله ");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Searchadvancefreight", "پیش کرایه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Searchfreightforward", "پس کرایه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Searchcod", "امانی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchOfficeId", "شهر");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchServiceTypeId", "نوع سرویس");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchTransportationId", "روش حمل و نقل");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchStartTime", "ساعت شروع کار");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchEndTime", "ساعت پایان کار");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchStoreId", "فروشگاه");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchIsPishtaz", "پیشتاز");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchIsSefareshi", "سفارشی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchIsVIje", "ویژه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchIsNromal", "نرمال");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchIsDroonOstani", "درون استانی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchIsAdjoining", "Adjoining");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchIsNotAdjacent", "NotAdjacent");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchIsHeavyTransport", "سنگین");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchIsForeign", "خارجی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchIsInCity", "درون شهری");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchIsAmanat", "امانت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchIsTwoStep", "دومرحله ای");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchHasHagheMaghar", "HasHagheMaghar");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchMaxlength", "حدکثر طول");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchMaxwidth", "حداکثر عرض");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchMaxheight", "حداکثرارتفاع");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchMaxbillingamountCOD", "حداکثر مبلغ امانی");








            //grid
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_AgentName", "نام سرویس دهنده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_ServiceTypeName", "نام سرویس");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_Name", "نام سرویس دهنده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_CategoryName", "دسته بندی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_MaxOrder", "حداکثر سفارش روزانه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_MaxTimeDeliver", "حداکثر زمان تحویل");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_MaxWeight", "حداکثر وزن محموله");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_MinWeight", "حداقل وزن محموله");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_advancefreight", "پیشکرایه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_freightforward", "پسکرایه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_cod", "امانی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsActive", "وضعیت");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsPishtaz", "پیشتاز");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsSefareshi", "سفارشی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsVIje", "ویژه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsNromal", "نرمال");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsDroonOstani", "درون استانی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsAdjoining", "Adjoining");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsNotAdjacent", "NotAdjacent");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsHeavyTransport", "سنگین");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsForeign", "خارجی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsInCity", "درون شهری");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsAmanat", "امانت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsTwoStep", "دومرحله ای");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_HasHagheMaghar", "HasHagheMaghar");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_DateInsert", "تاریخ ثبت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_DateUpdate", "اخرین تاریخ ویرایش");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_UserInsert", "کاربرثبت کننده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_UserUpdate", "کاربر ویرایش کننده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_Maxlength", "حدکثر طول");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_Maxwidth", "حداکثر عرض");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_Maxheight", "حداکثرارتفاع");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_MaxbillingamountCOD", "حداکثر مبلغ امانی");

            //#endregion


            //#region Set Local Resource Page ServiceTypes
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.ServiceType_pagetitle", "عنوان بالایی صفحه ");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.ServiceType_title", "لیست سرویس ها");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.ServiceType_new", "سرویس جدید");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.ServiceType_Update", "ویرایش سرویس");


            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Search_ServiceTypes_Name", "نام");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Search_ServiceTypes_IsActive", "وضعیت(فعال)");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Service_Name", "نام سرویس");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Service_IsActive", "وضعیت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Service_DateInsert", "تاریخ ثبت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Service_DateUpdate", "آخرین تاریخ ویرایش");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Service_UserInsert", "کاربر ثبت کننده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Service_UserUpdate", "کاربر ویرایش کننده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewServiceType", "نام سرویس:");




            //#endregion

            //#region Set Local Resource Page Dealer
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Dealer_pagetitle", "عنوان بالایی صفحه ");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Dealer_title", "لیست واسطه ها");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Search_Dealer_Name", "نام");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Search_Dealer_IsActive", "وضعیت(فعال)");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Dealer_Name", "نام واسطه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Dealer_IsActive", "وضعیت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Dealer_DateInsert", "تاریخ ثبت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Dealer_DateUpdate", "آخرین تاریخ ویرایش");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Dealer_UserInsert", "کاربر ثبت کننده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Dealer_UserUpdate", "کاربر ویرایش کننده");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Dealer_new", "واسطه جدید");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Dealer_Update", "ویرایش واسطه");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TabInfo", "مشخصات");
            //table
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.DealerName", "نام واسطه:");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.DealerProviderId", "انتخاب سرویس  دهنده ها");


            //#endregion

            //#region Set Local Resource Store



            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProviderStore_Name", "نام فروشگاه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProviderStore_IsActive", "وضعیت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProviderStore_DateInsert", "تاریخ ثبت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProviderStore_DateUpdate", "آخرین تاریخ ویرایش");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProviderStore_UserInsert", "کاربر ثبت کننده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProviderStore_UserUpdate", "کاربر ویرایش کننده");


            //#endregion

            //Collector

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PageSearchCollector_title", "لیست جمع آور کننده ها");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Collector_new", " جمع آور کننده جدید");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Collector_pagetitle", " جمع آور کننده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Collector_Update", " ویرایش جمع آور کننده");


            //#region set Local Resource Page Search collector
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchName", "نام جمع آور کننده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.SearchUserName", "نام مسئول");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsActive", "فعال");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchMaxPath", "حداکثر تعداد مسیر");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchMaxWeight", "حداکثر وزن");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchMinWeight", "حداقل وزن");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchadvancefreight", "پیشکرایه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchfreightforward", "پسکرایه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchcod", "امانی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsPishtaz", "پیشتاز");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsSefareshi", "سفارشی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsVIje", "ویژه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsNromal", "نرمال");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsDroonOstani", "درون استانی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsAdjoining", "Adjoining");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsNotAdjacent", "NotAdjacent");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsHeavyTransport", "سنگین");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsForeign", "خارجی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsInCity", "درون شهری");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsAmanat", "امانت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsTwoStep", "دو مرحله ای");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchHasHagheMaghar", "HasHagheMaghar");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchStartTime", "شروع ساعت کار");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchEndTime", "پایان ساعت کار");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchStoreId", "فروشگاه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchOfficeId", "شهر");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchProviderId", "سرویس دهنده");

            //#endregion
            //#region Set Resource Grid Collectors
            //grid

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_Name", "نام جمع آورکننده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_UserName", "نام مسئول");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_MaxPath", "حداکثر مسیر روزانه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_MaxWeight", "حداکثر وزن");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_MinWeight", "حداقل وزن");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_advancefreight", "پیشکرایه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_freightforward", "پسکرایه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_cod", "امانی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsActive", "وضعیت");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsPishtaz", "پیشتاز");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsSefareshi", "سفارشی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsVIje", "ویژه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsNromal", "نرمال");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsDroonOstani", "درون استانی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsAdjoining", "Adjoining");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsNotAdjacent", "NotAdjacent");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsHeavyTransport", "سنگین");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsForeign", "خارجی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsInCity", "درون شهری");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsAmanat", "امانت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsTwoStep", "دومرحله ای");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_HasHagheMaghar", "HasHagheMaghar");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_UserUpdate", "کاربر ویرایش کننده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_UserInsert", "کاربرثبت کننده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_DateUpdate", "اخرین تاریخ ویرایش");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_DateInsert", "تاریخ ثبت");







            //#endregion


            //#region set Local Resource Page NEW collector
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorName", "نام جمع آور کننده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorUserId", "نام مسئول");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorMaxPath", "حداکثر تعداد مسیر");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorMaxWeight", "حداکثر وزن");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorMinWeight", "حداقل وزن");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectoradvancefreight", "پیشکرایه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorfreightforward", "پسکرایه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorcod", "امانی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsPishtaz", "پیشتاز");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsSefareshi", "سفارشی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsVIje", "ویژه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsNromal", "نرمال");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsDroonOstani", "درون استانی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsAdjoining", "Adjoining");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsNotAdjacent", "NotAdjacent");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsHeavyTransport", "سنگین");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsForeign", "خارجی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsInCity", "درون شهری");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsAmanat", "امانت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsTwoStep", "دو مرحله ای");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorHasHagheMaghar", "HasHagheMaghar");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorStoreId", "انتخاب فروشگاه ها");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorProviderId", "انتخاب سرویس دهنده ها");

            //#endregion


            //#region Set Local Resource Grid Offies

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorStateProvinceId", "انتخاب شهر دفاتر");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_NameCountry", "نام استان");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_NameProvinces", "نام شهر");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_IdStateMaping", "آی دی استان مپ شده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_IdCityMaping", "آی دی شهر مپ شده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_NameStateMaping", "نام استان مپ شده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_NameCityMaping", "نام شهر مپ شده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_IsActive", "وضعبت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_DateInsert", "تاربخ ثبت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_DateUpdate", "تاریخ ویرایش");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_UserInsert", "کاربر ثبت کننده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_UserUpdate", "کاربر ویرایش کننده");

            //#endregion

            //#region Set Resource Manege Office
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Office_pagetitle", "مدیریت دفاتر");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Office_Update", "ویرایش دفاتر");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Office_Monday", "دوشنبه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Office_Tuesday", "سه شنبه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Office_Wednesday", "چهارشنبه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Office_Thursday", "پنجشنبه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Office_Friday", "جمعه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Office_Saturday", "شنبه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Office_Sunday", "یکشنبه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Office_StartTime", "شروع زمان کاری");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Office_EndTime", "پایان زمان کاری");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.OfficeCustomerID", "مسئول دفتر");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.OfficeWarehouseState", "وضعیت موجود بودن انبار");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.OfficeAddressId", "آدرس انبار");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.OfficeLat", "عرض جغرافیایی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.OfficeLong", "طول جغرافیایی");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.OfficeHolidaysState", "باز بودن در روزهای تعطیل");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.OfficeIdState", "کد استان مپ شده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.OfficeIdCity", "کد شهر مپ شده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.OfficeNameState", "نام استان مپ شده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.OfficeNameCity", "نام شهر مپ شده");



            //#endregion


            //#region Set Local Resource Page Dealer
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Dealer_pagetitle", "عنوان بالایی صفحه ");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Dealer_title", "لیست واسطه ها");

            //#endregion
            //#region set Local Resource Page Index Customer
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Page_Titel_List_Customer", "لیست مشتریان");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Titel_List_Customer", "لیست مشتریان-سیاست قیمت گذاری");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Customer_new", "مشتری جدید");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Customer_Update", "ویرایش مشتری");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.StateNegative_credit_amount", "وضعیت اعتبار منفی");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Negative_credit_amount", "مبلغ اعتبار منفی");


            //#endregion

            //#region Set Local Resource Page DealerCustomer ServiceProvider
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.DealerCustomerStateMonth_Day", "محدودیت براساس ماه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.DealerCustomerMaxCountpackage", "حداکثر تعداد بسته");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.DealerCustomerMaxWeight", "حداکثر وزن");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.DealerCustomerListCustomer", " مشتریان");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.DealerCustomer_ProviderId", "انتخاب سرویس دهنده ها");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.ListStateApplyPricingPolicy", "نحوه تخصیص سیاست های قیمت گذاری");


            //#endregion
            //#region Set Local Resource Page PricingPolicy
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TabInfoPricingPoliy", "لیست سیاست ها و ایجاد سیاست قیمت گذاری جدید");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Page_Titel_PricingPoliy", "مدیریت سیاست قیمت گذاری");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Update_PricingPoliy", "ویرایش سیاست قیمت گذاری");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PricingPoliyMinWeight", "از وزن");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PricingPoliyMaxWeight", "تا وزن");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PricingPoliyPercent", "درصد");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PricingPoliyMablagh", "مبلغ");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PricingPoliyTashim", "مبلغ تسهیم");
            //grid
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_NameDealer", "نام واسطه");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_NameCustomer", "نام مشتری");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_NameProvider", "نام سرویس دهنده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_MinWeight", "از وزن");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_MaxWeight", "تا وزن");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_Percent", "درصد");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_Mablagh", "مبلغ");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_Tashim", "مبلغ تسهیم");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_IsActive", "وضعیت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_DateInsert", "تاریخ ثبت");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_DateUpdate", "اخریت تاریخ ویرایش");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_UserInsert", "کاربر ثبت کننده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_UserUpdate", "کابر ویرایش کننده");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PricingPolicy_NameCountry", "نام استان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PricingPoliyPercentTashim", "درصد تسهیم");



            #endregion

            #region Set Local Resource Page Pattern PricingPolicy
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TabInfoPatternPricingPoliy", "مشخصات");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Page_Titel_PatternPricingPoliy", "مدیریت پیش نویس سیاست قیمت گذاری");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Update_PatternPricingPoliy", "ویرایش پیش نویس سیاست قیمت گذاری");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.New_PatternPricingPoliy", " پیش نویس سیاست قیمت گذاری جدید");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.List_PatternPricingPoliy", "لیست پیش نویس ها براساس دسته بندی");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMinCount", "از تعداد");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMaxCount", "تا تعداد");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMinWeight", "از وزن");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMaxWeight", "تا وزن");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyPercent", "درصد");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyMablagh", "مبلغ");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyTashim", "مبلغ تسهیم");
            //create
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyListCategory", "انتخاب دسته بندی");

            //grid pattern category
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_Category_Name", "نام دسته بندی");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_IsActive", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_DateInsert", "تاریخ ثبت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_DateUpdate", "تاریخ اخرین ویرایش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_UserInsert", "کاربر ثبت کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_PatternPricingPolicy_UserUpdate", "اخرین کاربر ویرایش کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyNamePattern", "نام پیش نویس");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PatternPricingPoliyPercentTashim", "درصد تسهیم");


            //Product Pattern Pricing
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TabInfoProductPatternPricing", "مشخصات");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Page_Titel_ProductPatternPricing", "مدیریت طرح های تخفیف");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Update_ProductPatternPricing", "ویرایش طر تخفیف");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.New_ProductPatternPricing", " طرح تخفیف جدید");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.List_ProductPatternPricing", "لیست طرح های تخفیف");

            //search
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Search_ProductPatternPricing_ProductName", "نام محصول");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Search_ProductPatternPricing_IsActive", "وضعیت(فعال)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Search_ProductPatternPricing_IdPatternPricing", "الگوی سایت قیمت گاری");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_ProductName", "نام محصول");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_PatternNames", "نام سیاست های قیمت گذاری");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_IsActive", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_DateInsert", "تاریخ ثبت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_DateUpdate", "آخرین تاریخ ویرایش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_UserInsert", "کاربر ثبت کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_UserUpdate", "آخرین کاربر ویرایش کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.ProductPatternPricingListProduct", "انتخاب محصول");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.ProductPatternPricingListStateApplyPricingPolicy", "انتخاب وضعیت تخصیص سیاست ها");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.ProductPatternPricingListPatternPricing", "انتخاب الگوهای سیاست های قیمت گذاری");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Search_ProductPatternPricing_IdStateApplyPricingPolicy", "انتخاب وضعیت تخصیص سیاست ها");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.ProductPatternPricingPrice", "قیمت فروش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_Price", "قیمت فروش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_StateApplyPricingPolicy", "انتخاب وضعیت تخصیص سیاست ها");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_ProductPatternPricing_StateClaculateMonth", "وضعیت محاسبه -پیش فرض بعد ا سفارش-انتخاب ماهانه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.ProductPatternPricingListStateClaculateMonth", "وضعیت محاسبه -پیش فرض بعد ا سفارش-انتخاب ماهانه");


            //discountplan
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Page_Titel_DiscountPlan", "مدیریت طرح های تخفیف مشتریان و نمایندگان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Update_DiscountPlan", "ویرایش طرح تخفیف");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.New_DiscountPlan", " طرح تخفیف جدید");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.List_DiscountPlan", "لیست طرح های تخفیف مشتریان و نمایندگان و شارژ کیف پول");



            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.DiscountPlan_AgentCustomer_Name", "نام طرح");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.DiscountPlan_AgentCustomer_OfAmount", "از مبلغ");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.DiscountPlan_AgentCustomer_UpAmount", "تا مبلغ");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.DiscountPlan_AgentCustomer_IsAgentt", "مشتری/نماینده(پیش فرش مشتری)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.DiscountPlan_AgentCustomer_ExpireDay", "مهلت استفاده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_Name", "نام طرح");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_OfAmount", "از مبلغ");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_UpAmount", "تا مبلغ");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_IsAgent", "مشتری/نماینده(پیش فرش مشتری)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_ExpireDay", "مهلت استفاده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_IsActive", "وضعیت(فعال)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_DateInsert", "تاریخ ثبت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_DateUpdate", "آخرین تاریخ ویرایش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_UserInsert", "کاربر ثبت کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_UserUpdate", "آخرین کاریر ویرایش کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Search_DiscountPlan_Name", "نام طرح");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Search_DiscountPlan_IsActive", "وضعیت(فعال)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Search_DiscountPlan_IsAgent", "مشتری/نماینده(پیش فرش مشتری)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Grid_DiscountPlan_Percent", "درصد شارژ کیف پول");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.DiscountPlan_AgentCustomer_Percent", "درصد شارژ کیف پول");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.", "");

            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.", "");




            #endregion
            #endregion
            base.Install();
            _context.Install();
            PluginManager.MarkPluginAsInstalled(this.PluginDescriptor.SystemName);
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {

            SiteMapNode siteMapNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Configuration");

            #region _ServiceMangement
            SiteMapNode _ServiceMangement = new SiteMapNode
            {
                SystemName = "Opr.Services",
                Title = "سرویس ها",
                Visible = true,
                IconClass = "fa fa-dot-circle-o",
            };
            siteMapNode?.ChildNodes.Add(_ServiceMangement);

            SiteMapNode itemProvider = new SiteMapNode
            {
                SystemName = "Opr.Services.ManageServiceProvider",
                Title = "سرویس های پستی",
                ControllerName = "ManageServiceProvider",
                ActionName = "Index",
                Visible = true,
                IconClass = "fa fa-dot-circle-o",
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary
                {
                    {
                        "area",
                        "admin"
                    }
                }
            };
            _ServiceMangement?.ChildNodes.Add(itemProvider);

            SiteMapNode itemCollector = new SiteMapNode
            {
                SystemName = "Opr.Services.ManageCollector",
                Title = "سرویس های جمع آوری",
                ControllerName = "ManageCollector",
                ActionName = "Index",
                Visible = true,
                IconClass = "fa fa-dot-circle-o",
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary
                {
                    {
                        "area",
                        "admin"
                    }
                }
            };
            _ServiceMangement?.ChildNodes.Add(itemCollector);
            #endregion

            #region سیاست قیمت گزاری

            SiteMapNode _PricingPolicy = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Sale.PricingPolicy");
            if (_PricingPolicy == null)
            {
                _PricingPolicy = new SiteMapNode
                {
                    SystemName = "Sale.PricingPolicy",
                    Title = "سیاست های مالی",
                    Visible = true,
                    IconClass = "fa fa-dot-circle-o",
                };
                siteMapNode?.ChildNodes.Add(_PricingPolicy);
            }
            SiteMapNode itemPatternPricingPolicy = new SiteMapNode
            {
                SystemName = "Sale.PricingPolicy.ManagePatternPricingPolicy",
                Title = "پیش نویس",
                ControllerName = "ManagePatternPricingPolicy",
                ActionName = "Index",
                Visible = true,
                IconClass = "fa fa-dot-circle-o",
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary
                {
                    {
                        "area",
                        "admin"
                    }
                }
            };
            _PricingPolicy?.ChildNodes.Add(itemPatternPricingPolicy);
            SiteMapNode itemDealer = new SiteMapNode
            {
                SystemName = "Sale.PricingPolicy.ManageDealer",
                Title = "درگاه ها",
                ControllerName = "ManageDealer",
                ActionName = "Index",
                Visible = true,
                IconClass = "fa fa-dot-circle-o",
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary
                {
                    {
                        "area",
                        "admin"
                    }
                }
            };
            _PricingPolicy?.ChildNodes.Add(itemDealer);
            SiteMapNode itemCustomer = new SiteMapNode
            {
                SystemName = "Sale.PricingPolicy.ManageCustomerServiceProvider",
                Title = "مشتری خاص",
                ControllerName = "ManageCustomerServiceProvider",
                ActionName = "Index",
                Visible = true,
                IconClass = "fa fa-dot-circle-o",
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary
                {
                    {
                        "area",
                        "admin"
                    }
                }
            };
            _PricingPolicy?.ChildNodes.Add(itemCustomer);

            SiteMapNode itemProductPatternPricing = new SiteMapNode
            {
                SystemName = "Sale.PricingPolicy.ManageProductPatternPricing",
                Title = "پلن های مالی نمایشی",
                ControllerName = "ManageProductPatternPricing",
                ActionName = "Index",
                Visible = true,
                IconClass = "fa fa-dot-circle-o",
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary
                {
                    {
                        "area",
                        "admin"
                    }
                }
            };
            _PricingPolicy?.ChildNodes.Add(itemProductPatternPricing);
            
            SiteMapNode itemDiscountPlan = new SiteMapNode
            {
                SystemName = "Sale.PricingPolicy.ManageDiscountPlan_AgentCustomer",
                Title = "طرح تشویق",
                ControllerName = "ManageDiscountPlan_AgentCustomer",
                ActionName = "Index",
                Visible = true,
                IconClass = "fa fa-dot-circle-o",
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary
                {
                    {
                        "area",
                        "admin"
                    }
                }
            };
            _PricingPolicy?.ChildNodes.Add(itemDiscountPlan);

            SiteMapNode itemSalesPartnersPercent = new SiteMapNode
            {
                SystemName = "Sale.PricingPolicy.ManageSalesPartnersPercent",
                Title = "همکاران فروش",
                ControllerName = "ManageSalesPartnersPercent",
                ActionName = "Index",
                Visible = true,
                IconClass = "fa fa-dot-circle-o",
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary
                {
                    {
                        "area",
                        "admin"
                    }
                }
            };
            _PricingPolicy?.ChildNodes.Add(itemSalesPartnersPercent);



            #endregion

        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            #region settings
            _settingService.DeleteSetting<ShippingSettings>();
            #region Delete Plugin LocaleResource services
            //locales
            //pde
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_Authorization");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_Password");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_Ccode");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_ClientId");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_URLTrackingParcels");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_URLListOfCountries");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_URLListOfCities");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_URLIntenationalCalculator");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_URLDomesticCalculator");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_URLRegisterInternationalOrder");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_URLRegisterDomesticOrder");
            //yarbox
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_verifyCode");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_phoneNumber");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_playerId");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_ExpiresDayToken");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLAp_Verify");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLApPackingType");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLAp_Type");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLApPostPacks");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLfactor");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLAp_PostPacks_accept");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLAp_Qute");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.YarBox_URL_Tracking");


            //Ubaar
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Ubaar_APIToken");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Ubaar_USERToken");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Ubaar_Urlregionlist");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Ubaar_Urlpriceenquiry");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Ubaar_Urlmodifyorder");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Ubaar_UrlTracking");


            //Tinex
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_grant_type");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_client_id");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_client_secret");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_scope");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_ExpireDayToken");

            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_UrlToken");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_UrlGetCost");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_Urlinsert");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_Urlinsertcommit");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_Urlcancel");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Tinex_Urlcancelreasons");

            //safiran
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_APP_AUTH");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_PickupMan");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_SenderCode");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_Safiran_UserName");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_Safiran_Password");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_GetState");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_City");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_TRACKING");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_GetQuote");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_PickupRequest");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_BulkImport");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_BulkHistoryReport");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_HistoryReport");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_Cancel");

            //persian
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Persain_UserName");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Persain_Password");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Persain_URL_NewCustomer");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Persain_URL_ViewCustomer");
            //TPG
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_UserName");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_Password");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_Url_compute");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_Url_Pickup");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_ContractCode");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_ContractId");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_UserId");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.TPG_Url_Receipt");
            //Chapar
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_APP_AUTH");
            //this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_PickupMan");
            //this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_SenderCode");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_Safiran_UserName");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_Safiran_Password");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_GetState");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_City");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_TRACKING");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_GetQuote");
            //this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_PickupRequest");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_BulkImport");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_BulkHistoryReport");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_HistoryReport");
            //this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_Cancel");

            //taroff
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_APP_AUTH");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_GetCity");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_GetProvinces");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_GetListPaymentMethods");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_GetListCarriers");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_CreateOrder");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_GetStateOrder");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_SetStateReady");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_SetStateCancel");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_Report");

            //snapbox
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_APP_AUTH");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Login");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Get_Account_Balance");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Create_Order");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Get_Order_Details");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Cancel_Order");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Get_Order_List");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Get_Price");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.Snappbox_CustomerId");




            #endregion

            #endregion


            PluginManager.MarkPluginAsUninstalled(this.PluginDescriptor.SystemName);
            _context.Uninstall();
            base.Uninstall();
        }
    }
}

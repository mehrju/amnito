using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Misc.ShippingSolutions.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.ShippingSolutions.Controllers
{
    [Area(AreaNames.Admin)]
    public class ShippingController : BasePluginController
    {

        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public ShippingController(IWorkContext workContext,
            IStoreService storeService,
            IPermissionService permissionService,
            ISettingService settingService,
            ICacheManager cacheManager,
            ILocalizationService localizationService)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var Settings = _settingService.LoadSetting<ShippingSettings>(storeScope);

            var model = new ConfigurationModel
            {
                //pde
                PDE_Authorization = Settings.PDE_Authorization,
                PDE_Password = Settings.PDE_Password,
                PDE_Ccode = Settings.PDE_Ccode,
                PDE_ClientId = Settings.PDE_ClientId,
                PDE_URLTrackingParcels = Settings.PDE_URLTrackingParcels,
                PDE_URLListOfCountries = Settings.PDE_URLListOfCountries,
                PDE_URLListOfCities = Settings.PDE_URLListOfCities,
                PDE_URLIntenationalCalculator = Settings.PDE_URLIntenationalCalculator,
                PDE_URLDomesticCalculator = Settings.PDE_URLDomesticCalculator,
                PDE_URLRegisterInternationalOrder = Settings.PDE_URLRegisterInternationalOrder,
                PDE_URLRegisterDomesticOrder = Settings.PDE_URLRegisterDomesticOrder,

                //yarbox
                //YarBox_Token = Settings.YarBox_Token,
                //YarBox_DateToken = Settings.YarBox_DateToken,
                YarBox_verifyCode = Settings.YarBox_verifyCode,
                YarBox_phoneNumber = Settings.YarBox_phoneNumber,
                YarBox_playerId = Settings.YarBox_playerId,
                YarBox_ExpiresDayToken = Settings.YarBox_ExpiresDayToken,
                YarBox_URLAp_Verify = Settings.YarBox_URLAp_Verify,
                YarBox_URLApPackingType = Settings.YarBox_URLApPackingType,
                YarBox_URLAp_Type = Settings.YarBox_URLAp_Type,
                YarBox_URLApPostPacks = Settings.YarBox_URLApPostPacks,
                YarBox_URLfactor = Settings.YarBox_URLfactor,
                YarBox_URLAp_PostPacks_accept = Settings.YarBox_URLAp_PostPacks_accept,
                YarBox_URLAp_Qute = Settings.YarBox_URLAp_Qute,
                YarBox_URL_Tracking = Settings.YarBox_URL_Tracking,
                //Ubaar
                Ubaar_APIToken = Settings.Ubaar_APIToken,
                Ubaar_USERToken = Settings.Ubaar_USERToken,
                Ubaar_Urlregionlist = Settings.Ubaar_Urlregionlist,
                Ubaar_Urlpriceenquiry = Settings.Ubaar_Urlpriceenquiry,
                Ubaar_Urlmodifyorder = Settings.Ubaar_Urlmodifyorder,
                Ubaar_UrlTracking = Settings.Ubaar_UrlTracking,
                //Tinex
                Tinex_grant_type = Settings.Tinex_grant_type,
                Tinex_client_id = Settings.Tinex_client_id,
                Tinex_client_secret = Settings.Tinex_client_secret,
                Tinex_scope = Settings.Tinex_scope,
                Tinex_ExpireDayToken = Settings.Tinex_ExpireDayToken,
                Tinex_UrlToken = Settings.Tinex_UrlToken,
                Tinex_UrlGetCost = Settings.Tinex_UrlGetCost,
                Tinex_Urlinsert = Settings.Tinex_Urlinsert,
                Tinex_Urlinsertcommit = Settings.Tinex_Urlinsertcommit,
                Tinex_Urlcancel = Settings.Tinex_Urlcancel,
                Tinex_Urlcancelreasons = Settings.Tinex_Urlcancelreasons,

                //safiran
                Safiran_APP_AUTH = Settings.Safiran_APP_AUTH,
                Safiran_PickupMan = Settings.Safiran_PickupMan,
                Safiran_SenderCode = Settings.Safiran_SenderCode,
                Safiran_UserName = Settings.Safiran_UserName,
                Safiran_Password = Settings.Safiran_Password,
                Safiran_URL_GetState = Settings.Safiran_URL_GetState,
                Safiran_URL_City = Settings.Safiran_URL_City,
                Safiran_URL_TRACKING = Settings.Safiran_URL_TRACKING,
                Safiran_URL_GetQuote = Settings.Safiran_URL_GetQuote,
                Safiran_URL_PickupRequest = Settings.Safiran_URL_PickupRequest,
                Safiran_URL_BulkImport = Settings.Safiran_URL_BulkImport,
                Safiran_URL_BulkHistoryReport = Settings.Safiran_URL_BulkHistoryReport,
                Safiran_URL_HistoryReport = Settings.Safiran_URL_HistoryReport,
                Safiran_URL_Cancel = Settings.Safiran_URL_Cancel,
                //Persian
                Persain_UserName = Settings.Persain_UserName,
                Persain_Password = Settings.Persain_Password,
                Persain_URL_NewCustomer = Settings.Persain_URL_NewCustomer,
                Persain_URL_ViewCustomer = Settings.Persain_URL_ViewCustomer,

                //TPG
                TPG_UserName = Settings.TPG_UserName,
                TPG_Password = Settings.TPG_Password,
                TPG_Url_compute = Settings.TPG_Url_compute,
                TPG_Url_Pickup = Settings.TPG_Url_Pickup,
                TPG_ContractCode = Settings.TPG_ContractCode,
                TPG_ContractId = Settings.TPG_ContractId,
                TPG_UserId = Settings.TPG_UserId,
                TPG_Url_Receipt = Settings.TPG_Url_Receipt,
                TPG_Url_Tracking=Settings.TPG_Url_Tracking,
                //Chapar
                Chapar_APP_AUTH = Settings.Chapar_APP_AUTH,
                //Chapar_PickupMan = Settings.Chapar_PickupMan,
                //Chapar_SenderCode = Settings.Chapar_SenderCode,
                Chapar_UserName = Settings.Chapar_UserName,
                Chapar_Password = Settings.Chapar_Password,
                Chapar_URL_GetState = Settings.Chapar_URL_GetState,
                Chapar_URL_City = Settings.Chapar_URL_City,
                Chapar_URL_TRACKING = Settings.Chapar_URL_TRACKING,
                Chapar_URL_GetQuote = Settings.Chapar_URL_GetQuote,
                //Chapar_URL_PickupRequest = Settings.Chapar_URL_PickupRequest,
                Chapar_URL_BulkImport = Settings.Chapar_URL_BulkImport,
                Chapar_URL_BulkHistoryReport = Settings.Chapar_URL_BulkHistoryReport,
                Chapar_URL_HistoryReport = Settings.Chapar_URL_HistoryReport,
                //Chapar_URL_Cancel = Settings.Safiran_URL_Cancel,


                //taroff
                taroff_APP_AUTH = Settings.taroff_APP_AUTH,
                taroff_URL_GetCity = Settings.taroff_URL_GetCity,
                taroff_URL_GetProvinces = Settings.taroff_URL_GetProvinces,
                taroff_URL_GetListPaymentMethods = Settings.taroff_URL_GetListPaymentMethods,
                taroff_URL_GetListCarriers = Settings.taroff_URL_GetListCarriers,
                taroff_URL_CreateOrder = Settings.taroff_URL_CreateOrder,
                taroff_URL_GetStateOrder = Settings.taroff_URL_GetStateOrder,
                taroff_URL_SetStateReady = Settings.taroff_URL_SetStateReady,
                taroff_URL_SetStateCancel = Settings.taroff_URL_SetStateCancel,
                taroff_URL_Report = Settings.taroff_URL_Report,
                //sanpbx
                Snappbox_APP_AUTH = Settings.Snappbox_APP_AUTH,
                Snappbox_URL_Login = Settings.Snappbox_URL_Login,
                Snappbox_URL_Get_Account_Balance = Settings.Snappbox_URL_Get_Account_Balance,
                Snappbox_URL_Create_Order = Settings.Snappbox_URL_Create_Order,
                Snappbox_URL_Get_Order_Details = Settings.Snappbox_URL_Get_Order_Details,
                Snappbox_URL_Cancel_Order = Settings.Snappbox_URL_Cancel_Order,
                Snappbox_URL_Get_Order_List = Settings.Snappbox_URL_Get_Order_List,
                Snappbox_URL_Get_Price = Settings.Snappbox_URL_Get_Price,
                Snappbox_CustomerId = Settings.Snappbox_CustomerId,
                //pde
                SkyBlue_APP_AUTH = Settings.Safiran_APP_AUTH,
                SkyBlue_URL_Get_Country = Settings.SkyBlue_URL_Get_Country,
                SkyBlue_URL_Get_ParcelType = Settings.SkyBlue_URL_Get_ParcelType,
                SkyBlue_URL_ParcelPrice = Settings.SkyBlue_URL_ParcelPrice,
                SkyBlue_URL_RegisterOrder = Settings.SkyBlue_URL_RegisterOrder,
                SkyBlue_URL_Cancel = Settings.SkyBlue_URL_Cancel,
                SkyBlue_URL_Tracking = Settings.SkyBlue_URL_Tracking,
                SkyBlue_URL_CheckService=Settings.SkyBlue_URL_CheckService,












                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                //pde
                model.PDE_Authorization_OverrideForStore = _settingService.SettingExists(Settings, x => x.PDE_Authorization, storeScope);
                model.PDE_Password_OverrideForStore = _settingService.SettingExists(Settings, x => x.PDE_Password, storeScope);
                model.PDE_Ccode_OverrideForStore = _settingService.SettingExists(Settings, x => x.PDE_Ccode, storeScope);
                model.PDE_ClientId_OverrideForStore = _settingService.SettingExists(Settings, x => x.PDE_ClientId, storeScope);
                model.PDE_URLTrackingParcels_OverrideForStore = _settingService.SettingExists(Settings, x => x.PDE_URLTrackingParcels, storeScope);
                model.PDE_URLListOfCountries_OverrideForStore = _settingService.SettingExists(Settings, x => x.PDE_URLListOfCountries, storeScope);
                model.PDE_URLListOfCities_OverrideForStore = _settingService.SettingExists(Settings, x => x.PDE_URLListOfCities, storeScope);
                model.PDE_URLIntenationalCalculator_OverrideForStore = _settingService.SettingExists(Settings, x => x.PDE_URLIntenationalCalculator, storeScope);
                model.PDE_URLDomesticCalculator_OverrideForStore = _settingService.SettingExists(Settings, x => x.PDE_URLDomesticCalculator, storeScope);
                model.PDE_URLRegisterInternationalOrder_OverrideForStore = _settingService.SettingExists(Settings, x => x.PDE_URLRegisterInternationalOrder, storeScope);
                model.PDE_URLRegisterDomesticOrder_OverrideForStore = _settingService.SettingExists(Settings, x => x.PDE_URLRegisterDomesticOrder, storeScope);


                //yarbox
                
                //model.YarBox_verifyCode_OverrideForStore = _settingService.SettingExists(Settings, x => x.YarBox_Token, storeScope);
                //model.YarBox_DateToken_OverrideForStore = _settingService.SettingExists(Settings, x => x.YarBox_DateToken, storeScope);
                model.YarBox_verifyCode_OverrideForStore = _settingService.SettingExists(Settings, x => x.YarBox_verifyCode, storeScope);
                model.YarBox_phoneNumber_OverrideForStore = _settingService.SettingExists(Settings, x => x.YarBox_phoneNumber, storeScope);
                model.YarBox_playerId_OverrideForStore = _settingService.SettingExists(Settings, x => x.YarBox_playerId, storeScope);
                model.YarBox_ExpiresDayToken_OverrideForStore = _settingService.SettingExists(Settings, x => x.YarBox_ExpiresDayToken, storeScope);
                model.YarBox_URLAp_Verify_OverrideForStore = _settingService.SettingExists(Settings, x => x.YarBox_URLAp_Verify, storeScope);
                model.YarBox_URLApPackingType_OverrideForStore = _settingService.SettingExists(Settings, x => x.YarBox_URLApPackingType, storeScope);
                model.YarBox_URLApPostPacks_OverrideForStore = _settingService.SettingExists(Settings, x => x.YarBox_URLApPostPacks, storeScope);
                model.YarBox_URLAp_PostPacks_accept_OverrideForStore = _settingService.SettingExists(Settings, x => x.YarBox_URLAp_PostPacks_accept, storeScope);
                model.YarBox_URLAp_Type_OverrideForStore = _settingService.SettingExists(Settings, x => x.YarBox_URLAp_Type, storeScope);
                model.YarBox_URLfactor_OverrideForStore = _settingService.SettingExists(Settings, x => x.YarBox_URLfactor, storeScope);
                model.YarBox_URLAp_Qute_OverrideForStore = _settingService.SettingExists(Settings, x => x.YarBox_URLAp_Qute, storeScope);
                model.YarBox_URL_Tracking_OverrideForStore = _settingService.SettingExists(Settings, x => x.YarBox_URL_Tracking, storeScope);

                //Ubaar
                model.Ubaar_APIToken_OverrideForStore = _settingService.SettingExists(Settings, x => x.Ubaar_APIToken, storeScope);
                model.Ubaar_USERToken_OverrideForStore = _settingService.SettingExists(Settings, x => x.Ubaar_USERToken, storeScope);
                model.Ubaar_Urlregionlist_OverrideForStore = _settingService.SettingExists(Settings, x => x.Ubaar_Urlregionlist, storeScope);
                model.Ubaar_Urlpriceenquiry_OverrideForStore = _settingService.SettingExists(Settings, x => x.Ubaar_Urlpriceenquiry, storeScope);
                model.Ubaar_Urlmodifyorder_OverrideForStore = _settingService.SettingExists(Settings, x => x.Ubaar_Urlmodifyorder, storeScope);
                model.Ubaar_UrlTracking_OverrideForStore = _settingService.SettingExists(Settings, x => x.Ubaar_UrlTracking, storeScope);

                
                //Tinex
                model.Tinex_grant_type_OverrideForStore = _settingService.SettingExists(Settings, x => x.Tinex_grant_type, storeScope);
                model.Tinex_client_id_OverrideForStore = _settingService.SettingExists(Settings, x => x.Tinex_client_id, storeScope);
                model.Tinex_client_secret_OverrideForStore = _settingService.SettingExists(Settings, x => x.Tinex_client_secret, storeScope);
                model.Tinex_scope_OverrideForStore = _settingService.SettingExists(Settings, x => x.Tinex_scope, storeScope);
                model.Tinex_ExpireDayToken_OverrideForStore = _settingService.SettingExists(Settings, x => x.Tinex_ExpireDayToken, storeScope);

                model.Tinex_UrlToken_OverrideForStore = _settingService.SettingExists(Settings, x => x.Tinex_UrlToken, storeScope);
                model.Tinex_UrlGetCost_OverrideForStore = _settingService.SettingExists(Settings, x => x.Tinex_UrlGetCost, storeScope);
                model.Tinex_Urlinsert_OverrideForStore = _settingService.SettingExists(Settings, x => x.Tinex_Urlinsert, storeScope);
                model.Tinex_Urlinsertcommit_OverrideForStore = _settingService.SettingExists(Settings, x => x.Tinex_Urlinsertcommit, storeScope);
                model.Tinex_Urlcancel_OverrideForStore = _settingService.SettingExists(Settings, x => x.Tinex_Urlcancel, storeScope);
                model.Tinex_Urlcancelreasons_OverrideForStore = _settingService.SettingExists(Settings, x => x.Tinex_Urlcancelreasons, storeScope);
                //safiran
                model.Safiran_APP_AUTH_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_APP_AUTH, storeScope);
                model.Safiran_PickupMan_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_PickupMan, storeScope);
                model.Safiran_SenderCode_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_SenderCode, storeScope);
                model.Safiran_UserName_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_UserName, storeScope);
                model.Safiran_Password_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_Password, storeScope);
                model.Safiran_URL_GetState_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_URL_GetState, storeScope);
                model.Safiran_URL_City_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_URL_City, storeScope);
                model.Safiran_URL_TRACKING_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_URL_TRACKING, storeScope);
                model.Safiran_URL_GetQuote_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_URL_GetQuote, storeScope);
                model.Safiran_URL_PickupRequest_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_URL_PickupRequest, storeScope);
                model.Safiran_URL_BulkImport_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_URL_BulkImport, storeScope);
                model.Safiran_URL_BulkHistoryReport_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_URL_BulkHistoryReport, storeScope);
                model.Safiran_URL_HistoryReport_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_URL_HistoryReport, storeScope);

                model.Safiran_URL_Cancel_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_URL_Cancel, storeScope);

                //Persain
                model.Persain_UserName_OverrideForStore = _settingService.SettingExists(Settings, x => x.Persain_UserName, storeScope);
                model.Persain_Password_OverrideForStore = _settingService.SettingExists(Settings, x => x.Persain_Password, storeScope);
                model.Persain_URL_NewCustomer_OverrideForStore = _settingService.SettingExists(Settings, x => x.Persain_URL_NewCustomer, storeScope);
                model.Persain_URL_ViewCustomer_OverrideForStore = _settingService.SettingExists(Settings, x => x.Persain_URL_ViewCustomer, storeScope);
                //TPG
                model.TPG_UserName_OverrideForStore = _settingService.SettingExists(Settings, x => x.TPG_UserName, storeScope);
                model.TPG_Password_OverrideForStore = _settingService.SettingExists(Settings, x => x.TPG_Password, storeScope);
                model.TPG_Url_compute_OverrideForStore = _settingService.SettingExists(Settings, x => x.TPG_Url_compute, storeScope);
                model.TPG_Url_Pickup_OverrideForStore = _settingService.SettingExists(Settings, x => x.TPG_Url_Pickup, storeScope);
                model.TPG_ContractCode_OverrideForStore = _settingService.SettingExists(Settings, x => x.TPG_ContractCode, storeScope);
                model.TPG_ContractId_OverrideForStore = _settingService.SettingExists(Settings, x => x.TPG_ContractId, storeScope);
                model.TPG_UserId_OverrideForStore = _settingService.SettingExists(Settings, x => x.TPG_UserId, storeScope);
                model.TPG_Url_Receipt_OverrideForStore = _settingService.SettingExists(Settings, x => x.TPG_Url_Receipt, storeScope);
                model.TPG_Url_Tracking_OverrideForStore = _settingService.SettingExists(Settings, x => x.TPG_Url_Tracking, storeScope);

                
                //chapar
                model.Chapar_APP_AUTH_OverrideForStore = _settingService.SettingExists(Settings, x => x.Chapar_APP_AUTH, storeScope);
                //model.Chapar_PickupMan_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_PickupMan, storeScope);
                //model.Chapar_SenderCode_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_SenderCode, storeScope);
                model.Chapar_UserName_OverrideForStore = _settingService.SettingExists(Settings, x => x.Chapar_UserName, storeScope);
                model.Chapar_Password_OverrideForStore = _settingService.SettingExists(Settings, x => x.Chapar_Password, storeScope);
                model.Chapar_URL_GetState_OverrideForStore = _settingService.SettingExists(Settings, x => x.Chapar_URL_GetState, storeScope);
                model.Chapar_URL_City_OverrideForStore = _settingService.SettingExists(Settings, x => x.Chapar_URL_City, storeScope);
                model.Chapar_URL_TRACKING_OverrideForStore = _settingService.SettingExists(Settings, x => x.Chapar_URL_TRACKING, storeScope);
                model.Chapar_URL_GetQuote_OverrideForStore = _settingService.SettingExists(Settings, x => x.Chapar_URL_GetQuote, storeScope);
                //model.Chapar_URL_PickupRequest_OverrideForStore = _settingService.SettingExists(Settings, x => x.Safiran_URL_PickupRequest, storeScope);
                model.Chapar_URL_BulkImport_OverrideForStore = _settingService.SettingExists(Settings, x => x.Chapar_URL_BulkImport, storeScope);
                model.Chapar_URL_BulkHistoryReport_OverrideForStore = _settingService.SettingExists(Settings, x => x.Chapar_URL_BulkHistoryReport, storeScope);
                model.Chapar_URL_HistoryReport_OverrideForStore = _settingService.SettingExists(Settings, x => x.Chapar_URL_HistoryReport, storeScope);

                //tarrof
                model.taroff_APP_AUTH_OverrideForStore = _settingService.SettingExists(Settings, x => x.taroff_APP_AUTH, storeScope);
                model.taroff_URL_GetCity_OverrideForStore = _settingService.SettingExists(Settings, x => x.taroff_URL_GetCity, storeScope);
                model.taroff_URL_GetProvinces_OverrideForStore = _settingService.SettingExists(Settings, x => x.taroff_URL_GetProvinces, storeScope);
                model.taroff_URL_GetListPaymentMethods_OverrideForStore = _settingService.SettingExists(Settings, x => x.taroff_URL_GetListPaymentMethods, storeScope);
                model.taroff_URL_GetListCarriers_OverrideForStore = _settingService.SettingExists(Settings, x => x.taroff_URL_GetListCarriers, storeScope);
                model.taroff_URL_CreateOrder_OverrideForStore = _settingService.SettingExists(Settings, x => x.taroff_URL_CreateOrder, storeScope);
                model.taroff_URL_GetStateOrder_OverrideForStore = _settingService.SettingExists(Settings, x => x.taroff_URL_GetStateOrder, storeScope);
                model.taroff_URL_SetStateReady_OverrideForStore = _settingService.SettingExists(Settings, x => x.taroff_URL_SetStateReady, storeScope);
                model.taroff_URL_SetStateCancel_OverrideForStore = _settingService.SettingExists(Settings, x => x.taroff_URL_SetStateCancel, storeScope);
                model.taroff_URL_Report_OverrideForStore = _settingService.SettingExists(Settings, x => x.taroff_URL_Report, storeScope);

                //snapbox
                model.Snappbox_APP_AUTH_OverrideForStore = _settingService.SettingExists(Settings, x => x.Snappbox_APP_AUTH, storeScope);
                model.Snappbox_URL_Login_OverrideForStore = _settingService.SettingExists(Settings, x => x.Snappbox_URL_Login, storeScope);
                model.Snappbox_URL_Get_Account_Balance_OverrideForStore = _settingService.SettingExists(Settings, x => x.Snappbox_URL_Get_Account_Balance, storeScope);
                model.Snappbox_URL_Create_Order_OverrideForStore = _settingService.SettingExists(Settings, x => x.Snappbox_URL_Create_Order, storeScope);
                model.Snappbox_URL_Get_Order_Details_OverrideForStore = _settingService.SettingExists(Settings, x => x.Snappbox_URL_Get_Order_Details, storeScope);
                model.Snappbox_URL_Cancel_Order_OverrideForStore = _settingService.SettingExists(Settings, x => x.Snappbox_URL_Cancel_Order, storeScope);
                model.Snappbox_URL_Get_Order_List_OverrideForStore = _settingService.SettingExists(Settings, x => x.Snappbox_URL_Get_Order_List, storeScope);
                model.Snappbox_URL_Get_Price_OverrideForStore = _settingService.SettingExists(Settings, x => x.Snappbox_URL_Get_Price, storeScope);
                model.Snappbox_CustomerId_OverrideForStore = _settingService.SettingExists(Settings, x => x.Snappbox_CustomerId, storeScope);


                //sky
                model.SkyBlue_APP_AUTH_OverrideForStore = _settingService.SettingExists(Settings, x => x.SkyBlue_APP_AUTH, storeScope);
                model.SkyBlue_URL_Get_Country_OverrideForStore = _settingService.SettingExists(Settings, x => x.SkyBlue_URL_Get_Country, storeScope);
                model.SkyBlue_URL_Get_ParcelType_OverrideForStore = _settingService.SettingExists(Settings, x => x.SkyBlue_URL_Get_ParcelType, storeScope);
                model.SkyBlue_URL_ParcelPrice_OverrideForStore = _settingService.SettingExists(Settings, x => x.SkyBlue_URL_ParcelPrice, storeScope);
                model.SkyBlue_URL_RegisterOrder_OverrideForStore = _settingService.SettingExists(Settings, x => x.SkyBlue_URL_RegisterOrder, storeScope);
                model.SkyBlue_URL_Cancel_OverrideForStore = _settingService.SettingExists(Settings, x => x.SkyBlue_URL_Cancel, storeScope);
                model.SkyBlue_URL_Tracking_OverrideForStore = _settingService.SettingExists(Settings, x => x.SkyBlue_URL_Tracking, storeScope);
                model.SkyBlue_URL_CheckService_OverrideForStore = _settingService.SettingExists(Settings, x => x.SkyBlue_URL_CheckService, storeScope);

                



            }

            return View("~/Plugins/Misc.ShippingSolutions/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var Settings = _settingService.LoadSetting<ShippingSettings>(storeScope);


            //pde
            Settings.PDE_Authorization = model.PDE_Authorization;
            Settings.PDE_Password = model.PDE_Password;
            Settings.PDE_Ccode = model.PDE_Ccode;
            Settings.PDE_ClientId = model.PDE_ClientId;
            Settings.PDE_URLTrackingParcels = model.PDE_URLTrackingParcels;
            Settings.PDE_URLListOfCountries = model.PDE_URLListOfCountries;
            Settings.PDE_URLListOfCities = model.PDE_URLListOfCities;
            Settings.PDE_URLIntenationalCalculator = model.PDE_URLIntenationalCalculator;
            Settings.PDE_URLDomesticCalculator = model.PDE_URLDomesticCalculator;
            Settings.PDE_URLRegisterInternationalOrder = model.PDE_URLRegisterInternationalOrder;
            Settings.PDE_URLRegisterDomesticOrder = model.PDE_URLRegisterDomesticOrder;

            //yarbox
            //Settings.YarBox_Token = model.YarBox_Token;
            //Settings.YarBox_DateToken = model.YarBox_DateToken;
            Settings.YarBox_verifyCode = model.YarBox_verifyCode;
            Settings.YarBox_phoneNumber = model.YarBox_phoneNumber;
            Settings.YarBox_playerId = model.YarBox_playerId;
            Settings.YarBox_ExpiresDayToken = model.YarBox_ExpiresDayToken;
            Settings.YarBox_URLAp_Verify = model.YarBox_URLAp_Verify;
            Settings.YarBox_URLAp_PostPacks_accept = model.YarBox_URLAp_PostPacks_accept;
            Settings.YarBox_URLfactor = model.YarBox_URLfactor;
            Settings.YarBox_URLAp_Type = model.YarBox_URLAp_Type;
            Settings.YarBox_URLApPostPacks = model.YarBox_URLApPostPacks;
            Settings.YarBox_URLApPackingType = model.YarBox_URLApPackingType;
            Settings.YarBox_URLAp_Qute = model.YarBox_URLAp_Qute;
            Settings.YarBox_URL_Tracking = model.YarBox_URL_Tracking;
            //Ubaar
            Settings.Ubaar_APIToken = model.Ubaar_APIToken;
            Settings.Ubaar_USERToken = model.Ubaar_USERToken;
            Settings.Ubaar_Urlregionlist = model.Ubaar_Urlregionlist;
            Settings.Ubaar_Urlpriceenquiry = model.Ubaar_Urlpriceenquiry;
            Settings.Ubaar_Urlmodifyorder = model.Ubaar_Urlmodifyorder;
            Settings.Ubaar_UrlTracking = model.Ubaar_UrlTracking;

            
            //Tinex
            Settings.Tinex_grant_type = model.Tinex_grant_type;
            Settings.Tinex_client_id = model.Tinex_client_id;
            Settings.Tinex_client_secret = model.Tinex_client_secret;
            Settings.Tinex_scope = model.Tinex_scope;
            Settings.Tinex_ExpireDayToken = model.Tinex_ExpireDayToken;
            Settings.Tinex_UrlToken = model.Tinex_UrlToken;
            Settings.Tinex_UrlGetCost = model.Tinex_UrlGetCost;
            Settings.Tinex_Urlinsert = model.Tinex_Urlinsert;
            Settings.Tinex_Urlinsertcommit = model.Tinex_Urlinsertcommit;
            Settings.Tinex_Urlcancel = model.Tinex_Urlcancel;
            Settings.Tinex_Urlcancelreasons = model.Tinex_Urlcancelreasons;

            //safiran
            Settings.Safiran_APP_AUTH = model.Safiran_APP_AUTH;
            Settings.Safiran_PickupMan = model.Safiran_PickupMan;
            Settings.Safiran_SenderCode = model.Safiran_SenderCode;
            Settings.Safiran_UserName = model.Safiran_UserName;
            Settings.Safiran_Password = model.Safiran_Password;
            Settings.Safiran_URL_GetState = model.Safiran_URL_GetState;
            Settings.Safiran_URL_City = model.Safiran_URL_City;
            Settings.Safiran_URL_TRACKING = model.Safiran_URL_TRACKING;
            Settings.Safiran_URL_GetQuote = model.Safiran_URL_GetQuote;
            Settings.Safiran_URL_PickupRequest = model.Safiran_URL_PickupRequest;
            Settings.Safiran_URL_BulkImport = model.Safiran_URL_BulkImport;
            Settings.Safiran_URL_BulkHistoryReport = model.Safiran_URL_BulkHistoryReport;
            Settings.Safiran_URL_HistoryReport = model.Safiran_URL_HistoryReport;
            Settings.Safiran_URL_Cancel = model.Safiran_URL_Cancel;
            // persain
            Settings.Persain_UserName = model.Persain_UserName;
            Settings.Persain_Password = model.Persain_Password;
            Settings.Persain_URL_NewCustomer = model.Persain_URL_NewCustomer;
            Settings.Persain_URL_ViewCustomer = model.Persain_URL_ViewCustomer;

            //TPG
            Settings.TPG_UserName = model.TPG_UserName;
            Settings.TPG_Password = model.TPG_Password;
            Settings.TPG_Url_compute = model.TPG_Url_compute;
            Settings.TPG_Url_Pickup = model.TPG_Url_Pickup;
            Settings.TPG_ContractCode = model.TPG_ContractCode;
            Settings.TPG_ContractId = model.TPG_ContractId;
            Settings.TPG_UserId = model.TPG_UserId;
            Settings.TPG_Url_Receipt = model.TPG_Url_Receipt;
            Settings.TPG_Url_Tracking = model.TPG_Url_Tracking;

            //Chapar

            Settings.Chapar_APP_AUTH = model.Chapar_APP_AUTH;
            //Settings.Chapar_PickupMan = model.Chapar_PickupMan;
            //Settings.Chapar_SenderCode = model.Chapar_SenderCode;
            Settings.Chapar_UserName = model.Chapar_UserName;
            Settings.Chapar_Password = model.Chapar_Password;
            Settings.Chapar_URL_GetState = model.Chapar_URL_GetState;
            Settings.Chapar_URL_City = model.Chapar_URL_City;
            Settings.Chapar_URL_TRACKING = model.Chapar_URL_TRACKING;
            Settings.Chapar_URL_GetQuote = model.Chapar_URL_GetQuote;
            //Settings.Chapar_URL_PickupRequest = model.Chapar_URL_PickupRequest;
            Settings.Chapar_URL_BulkImport = model.Chapar_URL_BulkImport;
            Settings.Chapar_URL_BulkHistoryReport = model.Chapar_URL_BulkHistoryReport;
            Settings.Chapar_URL_HistoryReport = model.Chapar_URL_HistoryReport;
            //Settings.Chapar_URL_Cancel = model.Chapar_URL_Cancel;

            //taroff
            Settings.taroff_APP_AUTH = model.taroff_APP_AUTH;
            Settings.taroff_URL_GetCity = model.taroff_URL_GetCity;
            Settings.taroff_URL_GetProvinces = model.taroff_URL_GetProvinces;
            Settings.taroff_URL_GetListPaymentMethods = model.taroff_URL_GetListPaymentMethods;
            Settings.taroff_URL_GetListCarriers = model.taroff_URL_GetListCarriers;
            Settings.taroff_URL_CreateOrder = model.taroff_URL_CreateOrder;
            Settings.taroff_URL_GetStateOrder = model.taroff_URL_GetStateOrder;
            Settings.taroff_URL_SetStateReady = model.taroff_URL_SetStateReady;
            Settings.taroff_URL_SetStateCancel = model.taroff_URL_SetStateCancel;
            Settings.taroff_URL_Report = model.taroff_URL_Report;


            //snapbox
            Settings.Snappbox_APP_AUTH = model.Snappbox_APP_AUTH;
            Settings.Snappbox_URL_Login = model.Snappbox_URL_Login;
            Settings.Snappbox_URL_Get_Account_Balance = model.Snappbox_URL_Get_Account_Balance;
            Settings.Snappbox_URL_Create_Order = model.Snappbox_URL_Create_Order;
            Settings.Snappbox_URL_Get_Order_Details = model.Snappbox_URL_Get_Order_Details;
            Settings.Snappbox_URL_Cancel_Order = model.Snappbox_URL_Cancel_Order;
            Settings.Snappbox_URL_Get_Order_List = model.Snappbox_URL_Get_Order_List;
            Settings.Snappbox_URL_Get_Price = model.Snappbox_URL_Get_Price;
            Settings.Snappbox_CustomerId = model.Snappbox_CustomerId;



            //sky
            Settings.SkyBlue_APP_AUTH = model.SkyBlue_APP_AUTH;
            Settings.SkyBlue_URL_Get_Country = model.SkyBlue_URL_Get_Country;
            Settings.SkyBlue_URL_Get_ParcelType = model.SkyBlue_URL_Get_ParcelType;
            Settings.SkyBlue_URL_ParcelPrice = model.SkyBlue_URL_ParcelPrice;
            Settings.SkyBlue_URL_RegisterOrder = model.SkyBlue_URL_RegisterOrder;
            Settings.SkyBlue_URL_Cancel = model.SkyBlue_URL_Cancel;
            Settings.SkyBlue_URL_Tracking = model.SkyBlue_URL_Tracking;
            Settings.SkyBlue_URL_CheckService = model.SkyBlue_URL_CheckService;


            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */



            //pde
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.PDE_Authorization, model.PDE_Authorization_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.PDE_Password, model.PDE_Password_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.PDE_Ccode, model.PDE_Ccode_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.PDE_ClientId, model.PDE_ClientId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.PDE_URLTrackingParcels, model.PDE_URLTrackingParcels_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.PDE_URLListOfCountries, model.PDE_URLListOfCountries_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.PDE_URLListOfCities, model.PDE_URLListOfCities_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.PDE_URLIntenationalCalculator, model.PDE_URLIntenationalCalculator_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.PDE_URLDomesticCalculator, model.PDE_URLDomesticCalculator_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.PDE_URLRegisterInternationalOrder, model.PDE_URLRegisterInternationalOrder_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.PDE_URLRegisterDomesticOrder, model.PDE_URLRegisterDomesticOrder_OverrideForStore, storeScope, false);



            //yarbox
            
            //_settingService.SaveSettingOverridablePerStore(Settings, x => x.YarBox_Token, model.YarBox_Token_OverrideForStore, storeScope, false);
            //_settingService.SaveSettingOverridablePerStore(Settings, x => x.YarBox_DateToken, model.YarBox_DateToken_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.YarBox_verifyCode, model.YarBox_verifyCode_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.YarBox_phoneNumber, model.YarBox_phoneNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.YarBox_playerId, model.YarBox_playerId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.YarBox_ExpiresDayToken, model.YarBox_ExpiresDayToken_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.YarBox_URLAp_Verify, model.YarBox_URLAp_Verify_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.YarBox_URLApPackingType, model.YarBox_URLApPackingType_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.YarBox_URLApPostPacks, model.YarBox_URLApPostPacks_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.YarBox_URLAp_PostPacks_accept, model.YarBox_URLAp_PostPacks_accept_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.YarBox_URLAp_Type, model.YarBox_URLAp_Type_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.YarBox_URLfactor, model.YarBox_URLfactor_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.YarBox_URLAp_Qute, model.YarBox_URLAp_Qute_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.YarBox_URL_Tracking, model.YarBox_URL_Tracking_OverrideForStore, storeScope, false);

            //Ubaar
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Ubaar_APIToken, model.Ubaar_APIToken_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Ubaar_USERToken, model.Ubaar_USERToken_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Ubaar_Urlregionlist, model.Ubaar_Urlregionlist_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Ubaar_Urlpriceenquiry, model.Ubaar_Urlpriceenquiry_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Ubaar_Urlmodifyorder, model.Ubaar_Urlmodifyorder_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Ubaar_UrlTracking, model.Ubaar_UrlTracking_OverrideForStore, storeScope, false);

            
            //Tinex
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Tinex_grant_type, model.Tinex_grant_type_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Tinex_client_id, model.Tinex_client_id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Tinex_client_secret, model.Tinex_client_secret_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Tinex_scope, model.Tinex_scope_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Tinex_ExpireDayToken, model.Tinex_ExpireDayToken_OverrideForStore, storeScope, false);

            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Tinex_UrlToken, model.Tinex_UrlToken_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Tinex_UrlGetCost, model.Tinex_UrlGetCost_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Tinex_Urlinsert, model.Tinex_Urlinsert_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Tinex_Urlinsertcommit, model.Tinex_Urlinsertcommit_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Tinex_Urlcancel, model.Tinex_Urlcancel_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Tinex_Urlcancelreasons, model.Tinex_Urlcancelreasons_OverrideForStore, storeScope, false);

            //safiarn
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Safiran_APP_AUTH, model.Safiran_APP_AUTH_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Safiran_PickupMan, model.Safiran_PickupMan_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Safiran_SenderCode, model.Safiran_SenderCode_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Safiran_UserName, model.Safiran_UserName_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Safiran_Password, model.Safiran_Password_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Safiran_URL_GetState, model.Safiran_URL_GetState_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Safiran_URL_City, model.Safiran_URL_City_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Safiran_URL_TRACKING, model.Safiran_URL_TRACKING_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Safiran_URL_GetQuote, model.Safiran_URL_GetQuote_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Safiran_URL_PickupRequest, model.Safiran_URL_PickupRequest_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Safiran_URL_BulkImport, model.Safiran_URL_BulkImport_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Safiran_URL_BulkHistoryReport, model.Safiran_URL_BulkHistoryReport_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Safiran_URL_HistoryReport, model.Safiran_URL_HistoryReport_OverrideForStore, storeScope, false);

            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Safiran_URL_Cancel, model.Safiran_URL_Cancel_OverrideForStore, storeScope, false);

            //persain
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Persain_UserName, model.Persain_UserName_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Persain_Password, model.Persain_Password_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Persain_URL_NewCustomer, model.Persain_URL_NewCustomer_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Persain_URL_ViewCustomer, model.Persain_URL_ViewCustomer_OverrideForStore, storeScope, false);


            //TPG
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.TPG_UserName, model.TPG_UserName_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.TPG_Password, model.TPG_Password_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.TPG_Url_compute, model.TPG_Url_compute_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.TPG_Url_Pickup, model.TPG_Url_Pickup_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.TPG_ContractCode, model.TPG_ContractCode_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.TPG_ContractId, model.TPG_ContractId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.TPG_UserId, model.TPG_UserId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.TPG_Url_Receipt, model.TPG_Url_Receipt_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.TPG_Url_Tracking, model.TPG_Url_Tracking_OverrideForStore, storeScope, false);


            
            //Chapar
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Chapar_APP_AUTH, model.Chapar_APP_AUTH_OverrideForStore, storeScope, false);
            //_settingService.SaveSettingOverridablePerStore(Settings, x => x.Chapar_PickupMan, model.Chapar_PickupMan_OverrideForStore, storeScope, false);
            //_settingService.SaveSettingOverridablePerStore(Settings, x => x.Chapar_SenderCode, model.Chapar_SenderCode_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Chapar_UserName, model.Chapar_UserName_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Chapar_Password, model.Chapar_Password_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Chapar_URL_GetState, model.Chapar_URL_GetState_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Chapar_URL_City, model.Chapar_URL_City_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Chapar_URL_TRACKING, model.Chapar_URL_TRACKING_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Chapar_URL_GetQuote, model.Chapar_URL_GetQuote_OverrideForStore, storeScope, false);
            //_settingService.SaveSettingOverridablePerStore(Settings, x => x.Chapar_URL_PickupRequest, model.Chapar_URL_PickupRequest_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Chapar_URL_BulkImport, model.Chapar_URL_BulkImport_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Chapar_URL_BulkHistoryReport, model.Chapar_URL_BulkHistoryReport_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Chapar_URL_HistoryReport, model.Chapar_URL_HistoryReport_OverrideForStore, storeScope, false);
            // _settingService.SaveSettingOverridablePerStore(Settings, x => x.Chapar_URL_Cancel, model.Chapar_URL_Cancel_OverrideForStore, storeScope, false);


            //tarof
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.taroff_APP_AUTH, model.taroff_APP_AUTH_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.taroff_URL_GetCity, model.taroff_URL_GetCity_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.taroff_URL_GetProvinces, model.taroff_URL_GetProvinces_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.taroff_URL_GetListPaymentMethods, model.taroff_URL_GetListPaymentMethods_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.taroff_URL_GetListCarriers, model.taroff_URL_GetListCarriers_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.taroff_URL_CreateOrder, model.taroff_URL_CreateOrder_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.taroff_URL_GetStateOrder, model.taroff_URL_GetStateOrder_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.taroff_URL_SetStateReady, model.taroff_URL_SetStateReady_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.taroff_URL_SetStateCancel, model.taroff_URL_SetStateCancel_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.taroff_URL_Report, model.taroff_URL_Report_OverrideForStore, storeScope, false);


            //snapbox
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Snappbox_APP_AUTH, model.Snappbox_APP_AUTH_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Snappbox_URL_Login, model.Snappbox_URL_Login_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Snappbox_URL_Get_Account_Balance, model.Snappbox_URL_Get_Account_Balance_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Snappbox_URL_Create_Order, model.Snappbox_URL_Create_Order_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Snappbox_URL_Get_Order_Details, model.Snappbox_URL_Get_Order_Details_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Snappbox_URL_Cancel_Order, model.Snappbox_URL_Cancel_Order_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Snappbox_URL_Get_Order_List, model.Snappbox_URL_Get_Order_List_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Snappbox_URL_Get_Price, model.Snappbox_URL_Get_Price_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.Snappbox_CustomerId, model.Snappbox_CustomerId_OverrideForStore, storeScope, false);

            //sky

            _settingService.SaveSettingOverridablePerStore(Settings, x => x.SkyBlue_APP_AUTH, model.SkyBlue_APP_AUTH_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.SkyBlue_URL_Get_Country, model.SkyBlue_URL_Get_Country_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.SkyBlue_URL_Get_ParcelType, model.SkyBlue_URL_Get_ParcelType_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.SkyBlue_URL_ParcelPrice, model.SkyBlue_URL_ParcelPrice_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.SkyBlue_URL_RegisterOrder, model.SkyBlue_URL_RegisterOrder_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.SkyBlue_URL_Cancel, model.SkyBlue_URL_Cancel_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.SkyBlue_URL_Tracking, model.SkyBlue_URL_Tracking_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(Settings, x => x.SkyBlue_URL_CheckService, model.SkyBlue_URL_CheckService_OverrideForStore, storeScope, false);


            //now clear settings cache
            _settingService.ClearCache();
            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }


    }
}

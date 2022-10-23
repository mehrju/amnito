using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using System;
using System.Globalization;

namespace Nop.Plugin.Misc.ShippingSolutions.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }


        #region PDE
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PDE_Authorization")]
        public string PDE_Authorization { get; set; }
        public bool PDE_Authorization_OverrideForStore { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PDE_Password")]
        public string PDE_Password { get; set; }
        public bool PDE_Password_OverrideForStore { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PDE_Ccode")]
        public int PDE_Ccode { get; set; }
        public bool PDE_Ccode_OverrideForStore { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PDE_ClientId")]
        public int PDE_ClientId { get; set; }
        public bool PDE_ClientId_OverrideForStore { get; set; }



        //1
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PDE_URLTrackingParcels")]
        public string PDE_URLTrackingParcels { get; set; }
        public bool PDE_URLTrackingParcels_OverrideForStore { get; set; }
        //2
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PDE_URLListOfCountries")]
        public string PDE_URLListOfCountries { get; set; }
        public bool PDE_URLListOfCountries_OverrideForStore { get; set; }
        //3
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PDE_URLListOfCities")]
        public string PDE_URLListOfCities { get; set; }
        public bool PDE_URLListOfCities_OverrideForStore { get; set; }
        //4
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PDE_URLIntenationalCalculator")]
        public string PDE_URLIntenationalCalculator { get; set; }
        public bool PDE_URLIntenationalCalculator_OverrideForStore { get; set; }
        //5
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PDE_URLDomesticCalculator")]
        public string PDE_URLDomesticCalculator { get; set; }
        public bool PDE_URLDomesticCalculator_OverrideForStore { get; set; }
        //6
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PDE_URLRegisterInternationalOrder")]
        public string PDE_URLRegisterInternationalOrder { get; set; }
        public bool PDE_URLRegisterInternationalOrder_OverrideForStore { get; set; }
        //7
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.PDE_URLRegisterDomesticOrder")]
        public string PDE_URLRegisterDomesticOrder { get; set; }
        public bool PDE_URLRegisterDomesticOrder_OverrideForStore { get; set; }

        #endregion
        #region YarBOx
        //YarBox
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.YarBox_Token")]
        //public string YarBox_Token { get; set; }
        //public bool YarBox_Token_OverrideForStore { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.YarBox_DateToken")]
        //public DateTime YarBox_DateToken { get; set; }
        //public bool YarBox_DateToken_OverrideForStore { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.YarBox_verifyCode")]
        public string YarBox_verifyCode { get; set; }
        public bool YarBox_verifyCode_OverrideForStore { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.YarBox_phoneNumber")]
        public string YarBox_phoneNumber { get; set; }
        public bool YarBox_phoneNumber_OverrideForStore { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.YarBox_playerId")]
        public string YarBox_playerId { get; set; }
        public bool YarBox_playerId_OverrideForStore { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.YarBox_ExpiresDayToken")]
        public int YarBox_ExpiresDayToken { get; set; }
        public bool YarBox_ExpiresDayToken_OverrideForStore { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLAp_Verify")]
        public string YarBox_URLAp_Verify { get; set; }
        public bool YarBox_URLAp_Verify_OverrideForStore { get; set; }

        //1
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLApPackingType")]
        public string YarBox_URLApPackingType { get; set; }
        public bool YarBox_URLApPackingType_OverrideForStore { get; set; }

        //2
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLAp_Type")]
        public string YarBox_URLAp_Type { get; set; }
        public bool YarBox_URLAp_Type_OverrideForStore { get; set; }
        //3
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLApPostPacks")]
        public string YarBox_URLApPostPacks { get; set; }
        public bool YarBox_URLApPostPacks_OverrideForStore { get; set; }
        //4
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLfactor")]
        public string YarBox_URLfactor { get; set; }
        public bool YarBox_URLfactor_OverrideForStore { get; set; }
        //5
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLAp_PostPacks_accept")]
        public string YarBox_URLAp_PostPacks_accept { get; set; }
        public bool YarBox_URLAp_PostPacks_accept_OverrideForStore { get; set; }
        //6
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.YarBox_URLAp_Qute")]
        public string YarBox_URLAp_Qute { get; set; }
        public bool YarBox_URLAp_Qute_OverrideForStore { get; set; }
        //7
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.YarBox_URL_Tracking")]
        public string YarBox_URL_Tracking { get; set; }
        public bool YarBox_URL_Tracking_OverrideForStore { get; set; }
        
        #endregion
        #region Ubaar
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Ubaar_APIToken")]
        public string Ubaar_APIToken { get; set; }
        public bool Ubaar_APIToken_OverrideForStore { get; set; }
        //1
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Ubaar_USERToken")]
        public string Ubaar_USERToken { get; set; }
        public bool Ubaar_USERToken_OverrideForStore { get; set; }
        //2
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Ubaar_Urlregionlist")]
        public string Ubaar_Urlregionlist { get; set; }
        public bool Ubaar_Urlregionlist_OverrideForStore { get; set; }
        //3
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Ubaar_Urlpriceenquiry")]
        public string Ubaar_Urlpriceenquiry { get; set; }
        public bool Ubaar_Urlpriceenquiry_OverrideForStore { get; set; }
        //4
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Ubaar_Urlmodifyorder")]
        public string Ubaar_Urlmodifyorder { get; set; }
        public bool Ubaar_Urlmodifyorder_OverrideForStore { get; set; }
        //5//Ubaar_UrlTracking
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Ubaar_UrlTracking")]
        public string Ubaar_UrlTracking { get; set; }
        public bool Ubaar_UrlTracking_OverrideForStore { get; set; }
        #endregion
        #region Tinex
        //1
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Tinex_grant_type")]
        public string Tinex_grant_type { get; set; }
        public bool Tinex_grant_type_OverrideForStore { get; set; }
        //2
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Tinex_client_id")]
        public string Tinex_client_id { get; set; }
        public bool Tinex_client_id_OverrideForStore { get; set; }
        //3
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Tinex_client_secret")]
        public string Tinex_client_secret { get; set; }
        public bool Tinex_client_secret_OverrideForStore { get; set; }
        //4
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Tinex_scope")]
        public string Tinex_scope { get; set; }
        public bool Tinex_scope_OverrideForStore { get; set; }
        //4-2
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Tinex_ExpireDayToken")]
        public int Tinex_ExpireDayToken { get; set; }
        public bool Tinex_ExpireDayToken_OverrideForStore { get; set; }
        //5
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Tinex_UrlToken")]
        public string Tinex_UrlToken { get; set; }
        public bool Tinex_UrlToken_OverrideForStore { get; set; }
        //6
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Tinex_UrlGetCost")]
        public string Tinex_UrlGetCost { get; set; }
        public bool Tinex_UrlGetCost_OverrideForStore { get; set; }
        //7
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Tinex_Urlinsert")]
        public string Tinex_Urlinsert { get; set; }
        public bool Tinex_Urlinsert_OverrideForStore { get; set; }
        //8
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Tinex_Urlinsertcommit")]
        public string Tinex_Urlinsertcommit { get; set; }
        public bool Tinex_Urlinsertcommit_OverrideForStore { get; set; }
        //9
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Tinex_Urlcancel")]
        public string Tinex_Urlcancel { get; set; }
        public bool Tinex_Urlcancel_OverrideForStore { get; set; }
        //10
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Tinex_Urlcancelreasons")]
        public string Tinex_Urlcancelreasons { get; set; }
        public bool Tinex_Urlcancelreasons_OverrideForStore { get; set; }
        #endregion
        #region Safiran
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Safiran_APP_AUTH")]
        public string Safiran_APP_AUTH { get; set; }
        public bool Safiran_APP_AUTH_OverrideForStore { get; set; }
        //
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Safiran_PickupMan")]
        public string Safiran_PickupMan { get; set; }
        public bool Safiran_PickupMan_OverrideForStore { get; set; }
        //
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Safiran_SenderCode")]
        public string Safiran_SenderCode { get; set; }
        public bool Safiran_SenderCode_OverrideForStore { get; set; }
        //
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Safiran_UserName")]
        public string Safiran_UserName { get; set; }
        public bool Safiran_UserName_OverrideForStore { get; set; }
        //
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Safiran_Password")]
        public string Safiran_Password { get; set; }
        public bool Safiran_Password_OverrideForStore { get; set; }
        //
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_GetState")]
        public string Safiran_URL_GetState { get; set; }
        public bool Safiran_URL_GetState_OverrideForStore { get; set; }
        //
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_City")]
        public string Safiran_URL_City { get; set; }
        public bool Safiran_URL_City_OverrideForStore { get; set; }
        //1
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_TRACKING")]
        public string Safiran_URL_TRACKING { get; set; }
        public bool Safiran_URL_TRACKING_OverrideForStore { get; set; }
        //2
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_GetQuote")]
        public string Safiran_URL_GetQuote { get; set; }
        public bool Safiran_URL_GetQuote_OverrideForStore { get; set; }
        //3
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_PickupRequest")]
        public string Safiran_URL_PickupRequest { get; set; }
        public bool Safiran_URL_PickupRequest_OverrideForStore { get; set; }
        //4
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_BulkImport")]
        public string Safiran_URL_BulkImport { get; set; }
        public bool Safiran_URL_BulkImport_OverrideForStore { get; set; }
        //5
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_BulkHistoryReport")]
        public string Safiran_URL_BulkHistoryReport { get; set; }
        public bool Safiran_URL_BulkHistoryReport_OverrideForStore { get; set; }
        //6
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_HistoryReport")]
        public string Safiran_URL_HistoryReport { get; set; }
        public bool Safiran_URL_HistoryReport_OverrideForStore { get; set; }
        //7

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Safiran_URL_Cancel")]
        public string Safiran_URL_Cancel { get; set; }
        public bool Safiran_URL_Cancel_OverrideForStore { get; set; }
        #endregion
        #region Persian Box
        //
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Persain_UserName")]
        public string Persain_UserName { get; set; }
        public bool Persain_UserName_OverrideForStore { get; set; }
        //
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Persain_Password")]
        public string Persain_Password { get; set; }
        public bool Persain_Password_OverrideForStore { get; set; }
        //
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Persain_URL_NewCustomer")]
        public string Persain_URL_NewCustomer { get; set; }
        public bool Persain_URL_NewCustomer_OverrideForStore { get; set; }
        //
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Persain_URL_ViewCustomer")]
        public string Persain_URL_ViewCustomer { get; set; }
        public bool Persain_URL_ViewCustomer_OverrideForStore { get; set; }
        #endregion
        #region TPG
        //1
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.TPG_UserName")]
        public string TPG_UserName { get; set; }
        public bool TPG_UserName_OverrideForStore { get; set; }
        //2
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.TPG_Password")]
        public string TPG_Password { get; set; }
        public bool TPG_Password_OverrideForStore { get; set; }
        //3
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.TPG_ContractCode")]
        public string TPG_ContractCode { get; set; }
        public bool TPG_ContractCode_OverrideForStore { get; set; }
        //4
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.TPG_ContractId")]
        public int TPG_ContractId { get; set; }
        public bool TPG_ContractId_OverrideForStore { get; set; }
        //5
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.TPG_UserId")]
        public int TPG_UserId { get; set; }
        public bool TPG_UserId_OverrideForStore { get; set; }
        //6
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.TPG_Url_compute")]
        public string TPG_Url_compute { get; set; }
        public bool TPG_Url_compute_OverrideForStore { get; set; }
        //7
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.TPG_Url_Pickup")]
        public string TPG_Url_Pickup { get; set; }
        public bool TPG_Url_Pickup_OverrideForStore { get; set; }
        //8
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.TPG_Url_Receipt")]
        public string TPG_Url_Receipt { get; set; }
        public bool TPG_Url_Receipt_OverrideForStore { get; set; }
        //9
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.TPG_Url_Tracking")]
        public string TPG_Url_Tracking { get; set; }
        public bool TPG_Url_Tracking_OverrideForStore { get; set; }
        #endregion
        //Chapar
        #region Chapar
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Chapar_APP_AUTH")]
        public string Chapar_APP_AUTH { get; set; }
        public bool Chapar_APP_AUTH_OverrideForStore { get; set; }
        //
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Chapar_PickupMan")]
        //public string Chapar_PickupMan { get; set; }
        //public bool Chapar_PickupMan_OverrideForStore { get; set; }
        //
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Chapar_SenderCode")]
        //public string Chapar_SenderCode { get; set; }
        //public bool Chapar_SenderCode_OverrideForStore { get; set; }
        //
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Chapar_UserName")]
        public string Chapar_UserName { get; set; }
        public bool Chapar_UserName_OverrideForStore { get; set; }
        //
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Chapar_Password")]
        public string Chapar_Password { get; set; }
        public bool Chapar_Password_OverrideForStore { get; set; }
        //
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_GetState")]
        public string Chapar_URL_GetState { get; set; }
        public bool Chapar_URL_GetState_OverrideForStore { get; set; }
        //
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_City")]
        public string Chapar_URL_City { get; set; }
        public bool Chapar_URL_City_OverrideForStore { get; set; }
        //1
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_TRACKING")]
        public string Chapar_URL_TRACKING { get; set; }
        public bool Chapar_URL_TRACKING_OverrideForStore { get; set; }
        //2
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_GetQuote")]
        public string Chapar_URL_GetQuote { get; set; }
        public bool Chapar_URL_GetQuote_OverrideForStore { get; set; }
        //3
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_PickupRequest")]
        //public string Chapar_URL_PickupRequest { get; set; }
        //public bool Chapar_URL_PickupRequest_OverrideForStore { get; set; }
        //4
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_BulkImport")]
        public string Chapar_URL_BulkImport { get; set; }
        public bool Chapar_URL_BulkImport_OverrideForStore { get; set; }
        //5
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_BulkHistoryReport")]
        public string Chapar_URL_BulkHistoryReport { get; set; }
        public bool Chapar_URL_BulkHistoryReport_OverrideForStore { get; set; }
        //6
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_HistoryReport")]
        public string Chapar_URL_HistoryReport { get; set; }
        public bool Chapar_URL_HistoryReport_OverrideForStore { get; set; }
        //7

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Chapar_URL_Cancel")]
        //public string Chapar_URL_Cancel { get; set; }
        //public bool Chapar_URL_Cancel_OverrideForStore { get; set; }
        #endregion

        #region taroff
            //1
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.taroff_APP_AUTH")]
        public string taroff_APP_AUTH { get; set; }
        public bool taroff_APP_AUTH_OverrideForStore { get; set; }

        //2
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_GetCity")]
        public string taroff_URL_GetCity { get; set; }
        public bool taroff_URL_GetCity_OverrideForStore { get; set; }
        //3
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_GetProvinces")]
        public string taroff_URL_GetProvinces { get; set; }
        public bool taroff_URL_GetProvinces_OverrideForStore { get; set; }
        //4
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_GetListPaymentMethods")]
        public string taroff_URL_GetListPaymentMethods { get; set; }
        public bool taroff_URL_GetListPaymentMethods_OverrideForStore { get; set; }
        //5
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_GetListCarriers")]
        public string taroff_URL_GetListCarriers { get; set; }
        public bool taroff_URL_GetListCarriers_OverrideForStore { get; set; }
        //6
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_CreateOrder")]
        public string taroff_URL_CreateOrder { get; set; }
        public bool taroff_URL_CreateOrder_OverrideForStore { get; set; }
        //7
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_GetStateOrder")]
        public string taroff_URL_GetStateOrder { get; set; }
        public bool taroff_URL_GetStateOrder_OverrideForStore { get; set; }
        //8
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_SetStateReady")]
        public string taroff_URL_SetStateReady { get; set; }
        public bool taroff_URL_SetStateReady_OverrideForStore { get; set; }
        //9
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_SetStateCancel")]
        public string taroff_URL_SetStateCancel { get; set; }
        public bool taroff_URL_SetStateCancel_OverrideForStore { get; set; }
        //10
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.taroff_URL_Report")]
        public string taroff_URL_Report { get; set; }
        public bool taroff_URL_Report_OverrideForStore { get; set; }










        #endregion


        #region snapbox
        //1
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Snappbox_APP_AUTH")]
        public string Snappbox_APP_AUTH { get; set; }
        public bool Snappbox_APP_AUTH_OverrideForStore { get; set; }
        //1-2
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Snappbox_CustomerId")]
        public string Snappbox_CustomerId { get; set; }
        public bool Snappbox_CustomerId_OverrideForStore { get; set; }
        //2
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Login")]
        public string Snappbox_URL_Login { get; set; }
        public bool Snappbox_URL_Login_OverrideForStore { get; set; }
        //3
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Get_Account_Balance")]
        public string Snappbox_URL_Get_Account_Balance { get; set; }
        public bool Snappbox_URL_Get_Account_Balance_OverrideForStore { get; set; }
        //4
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Create_Order")]
        public string Snappbox_URL_Create_Order { get; set; }
        public bool Snappbox_URL_Create_Order_OverrideForStore { get; set; }
        //5
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Get_Order_Details")]
        public string Snappbox_URL_Get_Order_Details { get; set; }
        public bool Snappbox_URL_Get_Order_Details_OverrideForStore { get; set; }
        //6
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Cancel_Order")]
        public string Snappbox_URL_Cancel_Order { get; set; }
        public bool Snappbox_URL_Cancel_Order_OverrideForStore { get; set; }
        //7
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Get_Order_List")]
        public string Snappbox_URL_Get_Order_List { get; set; }
        public bool Snappbox_URL_Get_Order_List_OverrideForStore { get; set; }
        //8
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Snappbox_URL_Get_Price")]
        public string Snappbox_URL_Get_Price { get; set; }
        public bool Snappbox_URL_Get_Price_OverrideForStore { get; set; }
        #endregion



        #region SkyBlue
        //1
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_APP_AUTH")]
        public string SkyBlue_APP_AUTH { get; set; }
        public bool SkyBlue_APP_AUTH_OverrideForStore { get; set; }
        //2
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_URL_Get_Country")]
        public string SkyBlue_URL_Get_Country { get; set; }
        public bool SkyBlue_URL_Get_Country_OverrideForStore { get; set; }
        //3
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_URL_Get_ParcelType")]
        public string SkyBlue_URL_Get_ParcelType { get; set; }
        public bool SkyBlue_URL_Get_ParcelType_OverrideForStore { get; set; }
        //4
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_URL_ParcelPrice")]
        public string SkyBlue_URL_ParcelPrice { get; set; }
        public bool SkyBlue_URL_ParcelPrice_OverrideForStore { get; set; }
        //5
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_URL_RegisterOrder")]
        public string SkyBlue_URL_RegisterOrder { get; set; }
        public bool SkyBlue_URL_RegisterOrder_OverrideForStore { get; set; }
        //6
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_URL_Cancel")]
        public string SkyBlue_URL_Cancel { get; set; }
        public bool SkyBlue_URL_Cancel_OverrideForStore { get; set; }
        //7
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_URL_Tracking")]
        public string SkyBlue_URL_Tracking { get; set; }
        public bool SkyBlue_URL_Tracking_OverrideForStore { get; set; }
        //8
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SkyBlue_URL_CheckService")]
        public string SkyBlue_URL_CheckService { get; set; }
        public bool SkyBlue_URL_CheckService_OverrideForStore { get; set; }


        


        #endregion
    }
}
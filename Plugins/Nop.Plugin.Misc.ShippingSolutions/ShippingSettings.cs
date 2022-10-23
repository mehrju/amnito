using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions
{
    public class ShippingSettings : ISettings
    {
        #region Setting PDE

        public string PDE_Authorization { get; set; }
        public string PDE_Password { get; set; }
        public int PDE_Ccode { get; set; }
        public int PDE_ClientId { get; set; }
        public string PDE_URLTrackingParcels { get; set; }
        public string PDE_URLListOfCountries { get; set; }
        public string PDE_URLListOfCities { get; set; }
        public string PDE_URLIntenationalCalculator { get; set; }
        public string PDE_URLDomesticCalculator { get; set; }
        public string PDE_URLRegisterInternationalOrder { get; set; }
        public string PDE_URLRegisterDomesticOrder { get; set; }
        #endregion
        #region Setting YarBox
        //public string YarBox_Token { get; set; }
        //public DateTime YarBox_DateToken { get; set; }
        public string YarBox_verifyCode { get; set; }
        public string YarBox_phoneNumber { get; set; }
        public string YarBox_playerId { get; set; }
        public int YarBox_ExpiresDayToken { get; set; }
        public string YarBox_URLAp_Verify { get; set; }
        public string YarBox_URLApPackingType { get; set; }
        public string YarBox_URLAp_Type { get; set; }
        public string YarBox_URLApPostPacks { get; set; }
        public string YarBox_URLfactor { get; set; }
        public string YarBox_URLAp_PostPacks_accept { get; set; }
        public string YarBox_URLAp_Qute { get; set; }
        public string YarBox_URL_Tracking { get; set; }

        #endregion
        #region setting Ubaar
        public string Ubaar_APIToken { get; set; }
        public string Ubaar_USERToken { get; set; }
        public string Ubaar_Urlregionlist { get; set; }
        public string Ubaar_Urlpriceenquiry { get; set; }
        public string Ubaar_Urlmodifyorder { get; set; }
        public string Ubaar_UrlTracking { get; set; }
        #endregion
        #region Tinex
        public string Tinex_grant_type { get; set; }
        public string Tinex_client_id { get; set; }
        public string Tinex_client_secret { get; set; }
        public string Tinex_scope { get; set; }
        public int Tinex_ExpireDayToken { get; set; }
        public string Tinex_UrlToken { get; set; }
        public string Tinex_UrlGetCost { get; set; }
        public string Tinex_Urlinsert { get; set; }
        public string Tinex_Urlinsertcommit { get; set; }
        public string Tinex_Urlcancel { get; set; }
        public string Tinex_Urlcancelreasons { get; set; }
        #endregion
        #region Safiran
        public string Safiran_APP_AUTH { get; set; }
        public string Safiran_PickupMan { get; set; }
        public string Safiran_SenderCode { get; set; }
        public string Safiran_UserName { get; set; }
        public string Safiran_Password { get; set; }
        public string Safiran_URL_GetState { get; set; }
        public string Safiran_URL_City { get; set; }
        public string Safiran_URL_TRACKING { get; set; }
        public string Safiran_URL_GetQuote { get; set; }
        public string Safiran_URL_PickupRequest { get; set; }
        public string Safiran_URL_BulkImport { get; set; }
        public string Safiran_URL_BulkHistoryReport { get; set; }
        public string Safiran_URL_HistoryReport { get; set; }
        public string Safiran_URL_Cancel { get; set; }
        #endregion
        #region  Persain Box
        public string Persain_UserName { get; set; }
        public string Persain_Password { get; set; }
        public string Persain_URL_NewCustomer { get; set; }
        public string Persain_URL_ViewCustomer { get; set; }

        #endregion
        #region TPG
        public string TPG_UserName { get; set; }
        public string TPG_Password { get; set; }
        public string TPG_Url_compute { get; set; }
        public string TPG_Url_Pickup { get; set; }
        public int TPG_UserId { get; set; }
        public string TPG_Url_Receipt { get; set; }
        public string TPG_ContractCode { get; set; }
        public int TPG_ContractId { get; set; }
        public string TPG_Url_Tracking { get; set; }

        #endregion
        #region Chapar
        public string Chapar_APP_AUTH { get; set; }
        //public string Chapar_PickupMan { get; set; }
        // public string Chapar_SenderCode { get; set; }
        public string Chapar_UserName { get; set; }
        public string Chapar_Password { get; set; }
        public string Chapar_URL_GetState { get; set; }
        public string Chapar_URL_City { get; set; }
        public string Chapar_URL_TRACKING { get; set; }
        public string Chapar_URL_GetQuote { get; set; }
        //public string Chapar_URL_PickupRequest { get; set; }
        public string Chapar_URL_BulkImport { get; set; }
        public string Chapar_URL_BulkHistoryReport { get; set; }
        public string Chapar_URL_HistoryReport { get; set; }
        // public string Chapar_URL_Cancel { get; set; }
        #endregion

        #region Mahex
        public string Mahex_Username { get; set; }
        public string Mahex_Password { get; set; }
        public string Mahex_RateUrl { get; set; }
        public string Mahex_CreateShipmentUrl { get; set; }
        public string Mahex_Url_TrackShipmentByUUID { get; set; }
        //public string Mahex_Url_TrackShipmentByUUID { get; set; }
        //public string Mahex_Url_TrackShipmentByUUID { get; set; }


        #endregion

        #region taroff
        public string taroff_APP_AUTH { get; set; }
        public string taroff_URL_GetProvinces { get; set; }
        public string taroff_URL_GetCity { get; set; }
        public string taroff_URL_GetListPaymentMethods { get; set; }
        public string taroff_URL_GetListCarriers { get; set; }
        public string taroff_URL_CreateOrder { get; set; }
        public string taroff_URL_GetStateOrder { get; set; }
        public string taroff_URL_SetStateReady { get; set; }
        public string taroff_URL_SetStateCancel { get; set; }
        public string taroff_URL_Report { get; set; }
        #endregion

        #region Snappbox
        public string Snappbox_APP_AUTH { get; set; }
        public string Snappbox_URL_Login { get; set; }
        public string Snappbox_URL_Get_Account_Balance { get; set; }
        public string Snappbox_URL_Create_Order { get; set; }
        public string Snappbox_URL_Get_Order_Details { get; set; }
        public string Snappbox_URL_Cancel_Order { get; set; }
        public string Snappbox_URL_Get_Order_List { get; set; }
        public string Snappbox_URL_Get_Price { get; set; }
        public string Snappbox_CustomerId { get; set; }


        #endregion


        #region SkyBlue
        public string SkyBlue_APP_AUTH           { get; set; }
        public string SkyBlue_URL_Get_Country    { get; set; }
        public string SkyBlue_URL_Get_ParcelType { get; set; }
        public string SkyBlue_URL_ParcelPrice    { get; set; }
        public string SkyBlue_URL_RegisterOrder  { get; set; }
        public string SkyBlue_URL_Cancel         { get; set; }
        public string SkyBlue_URL_Tracking       { get; set; }
        public string SkyBlue_URL_CheckService { get; set; }

        
        #endregion
    }
}

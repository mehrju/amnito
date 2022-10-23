using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.plugin.Orders.ExtendedShipment.Enums;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using static Nop.plugin.Orders.ExtendedShipment.Services.ExtendedShipmentService;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface IExtendedShipmentService

    {
        byte[] getBarocdeImage(string TrackingNumber);
        int GetdistributionValue(OrderItem orderitem);
        Task<string> RegisterKalaresan(Order order);
        Task<GetPriceResult> getKalaresanPrice(getPriceInputModel model);
         bool GetFreePost(OrderItem item);
        bool HasAgentInCity(int StateId);
        List<ForgeinCountry> getForinCountryForApi();
        AddressDetailes getAddressData(int CustomerId);
        List<SelectListItem> getCustomerByRoleId(int roleId);
        int GetGatwayPriceCountryCode(int countryId);
        string processShipmentAddressForCOD(int addressId, int orderId, out string StateCode, out string CountryCode);
        string processShipmentAddressForCODPrice(int senderCountrId, int SenderStateId, out string StateCode, out string CountryCode);
        CodGetPriceRersult CalcGatewayPrice(Customer customer, string ReciverCountryCode, string ReciverCityCode, bool IsCOD, int ItemWeight, string postType
           , int SenderStateId, OrderItem orderItem, int _haghemagharForshipment, int _approximateValue = 0, List<string> ForIgnorUserName = null, int ServiceId = 0
           , bool isFreePost = false);
        bool IsSafeBuy(int OrderId);
        //List<Attr> getAttrList(int shipmentId);
        int getInsertedHagheMaghar(Order order);
        bool IsOrderForeign(Order order);
        void PrintLable50MM(Order order, Stream stream);
        bool isInvalidSender(int SenderCountry, int senderState, int serviceId);
        (int, int, int) getDimensions(int orderItemId);
        (int, int, int) getDimensions(OrderItem item);
        int getItemWeight_V(OrderItem OI);
        int getItemWeight_V(int orderitemId);
        bool IsShipmentCollected(int shipmentId);
        List<int> getorderBySource(int Source);
        void InsertOrderSource(int OrderId, int SourceId);
        string MakeMahexVoid(string waybill_number);
        bool IsIsland(int StateId);
        bool IsWallet(Order order);
        int RequestSendSmsNotifPrice(OrderItem item);
        bool HasRequestSendSmsNotif(Shipment shipment);
        bool HasRequestSendSmsNotif(Order order);
        bool IsInValidDiscountPeriod();
        bool CanUseFirstOrderDiscount(Order order);
        bool IsInValidDiscountPeriod(Order order);
        bool isInvalidSender(int SenderCountry, int senderState);
        bool hasPostCordination(int orderId);
        bool CancelSappbox_Order(Order order, out string strError);
        float getFistOrderDiscount();
        int GetOrdersByCustomerId(int customerId);
        List<KeyValuePair<int, int>> getOrderByStateCount();
        void SavePostCoordination(int orderId, string Desc);
        int getHagheMaghar(int orderItemId, int SenderCountryId, int ServiceId, out int postHagheMaghar);
        bool IsPostService(int ServiceId);
        List<_ServiceInfo> GetBasPriceAndSlA(
            int senderCountry
            , int senderState
            , int receiverCountry
            , int receiverState
            , int weightItem
            , int storeId
            , int customerId
            , int receiver_ForeginCountry
            , bool? IsCod
            , int serviceId
            , bool ShowPrivatePost = true
            , bool ShowDistributer = true
            );
        Address getAddressFromShipment(int shipmentId);
        int GetItemWeightFromAttr(int orderitemId);
        List<node> ShipmentTreeView(int orderState);
        void RestartStopwatch(System.Diagnostics.Stopwatch watch, string logNote, ref long Total);
        int RequestPrintLogoPrice(OrderItem item);
        bool? hasValidAvatar(int customerId);
        bool HasRequestPrintAvatar(OrderItem item);
        List<node> OrdersOverView(int orderState);
        getPriceInputModel PriceInpuModelFactory(int senderCountry
            , int senderState
            , int receiverCountry
            , int receiverState
            , int weightItem
            , int AproximateValue
            , out string error
            , out bool canSelect
            , int height = 0
            , int length = 0
            , int width = 0
            , byte consType = 0
            , string content = ""
            , DateTime? dispach_date = null
            , string PackingOption = ""
            , string vechileType = ""
            , string VechileOption = ""
            , int receiver_ForeginCountry = 0
            , int ServiceId = 0
            , CustomAddressModel SenderAddress = null
            , CustomAddressModel ReciverAddress = null
            , string receiver_ForeginCountryNameEn = null
            , int quantity = 0
            );
        YarboxStateModel getYarBoxState(int stateId);
        UbbarRegionModel getUbarState(int ObaarRegionId);
        TPGStateModel getTpgState(int stateId);
        int getDtsStateId(int StateId);
        Task<string> RegisterChapar_Order(Order order);
        Task<GetPriceResult> getChaparPrice(getPriceInputModel model, int SenderCode, int TotalPrice = 0);
        Task<GetPriceResult> getmahexPrice(getPriceInputModel model);
        Task<string> RegisterMahex_Order(Order order);

        Task<string> RegisterUbbarOrder(Order order);
        Task<string> RegisterBlueSky_Order(Order order);
        Task<string> RegisterSnappbox_Order(Order order);
        Task<string> Registerpeykhub_Order(Order order);
        Task<GetPriceResult> getUbarPrice(getPriceInputModel model);
        Task<string> registerTPGOrder(Order order);
        Task<string> registerYarboxOrder(Order order);
        Task<GetPriceResult> getTPGPrice(getPriceInputModel model, bool minPrice = false);
        Task<GetPriceResult> getYarboxPrice(getPriceInputModel model);
        Task<GetPriceResult> getsnappboxPrice(getPriceInputModel model);
        Task<GetPriceResult> getBlueSkyPrice(getPriceInputModel model);
        //int getInsertedHagheMaghar(int orderItemId);
        Task<string> RegisterDomesticPDEOrder(Order order);
        Task<string> RegisterInternationalPDEOrder(Order order);
        List<SelectListItem> getForinCountry();
        Task<GetPriceResult> getPDEInternational_Price(getPriceInputModel model);
        Task<GetPriceResult> getPDE_Price(getPriceInputModel model);
        Task<GetPriceResult> peykhubGetPrice(getPriceInputModel model);
        PDEStateModel getPDEState(int StateId);
        int getHagheSabt(int orderItemId);
        ApiOrderItemPrice getEngAndPostPrice(OrderItem orderitem);
        bool CheckHasValidPrice(Order order);
        int getHagheSabt(Order order);
        int CalcHagheSabet(int customerId, int ServiceId, int OrderId = 0);
        void UpdateOrderTotalByApiPrice(Order order, int? DealerId = null);
        CategoryInfoModel GetCategoryInfo(Nop.Core.Domain.Catalog.Product product);
        int GetAgentAddValue(int orderItemId);
        void ChangeOrderState(Order order, OrderStatus orderStatus, string msg);
        int getApproximateValue(int orderItemId);
        int getOrderTotalbyIncomeApiPrice(int IncomePrice, int orderitemId, int serviceId, int Weight = 0, int? DealerId = null);
        Task<GetPriceResult> getDtsPrice(getPriceInputModel model, int SenderCode);
        Task<string> RegisterDTS_Order(Order order);
        void LockThisToSendToPost(OrderStatusToPost state, int orderId);
        void UnLockThisOrder(OrderStatusToPost state, int orderId);
        int getGoodsPrice(OrderItem orderitem);
        void EditCollectDate(int shipmentId, DateTime? CoordinationDate);
        void LogException(Exception exception);
        void EditPostCoordination(int orderId, DateTime? CoordinationDate);
        int getCodbasePrice(int customerId, int serviceId, int weight, int countryId, int stateId, int SenderStateId, out string error
            , bool IsFromAp = false, OrderItem orderItem = null, bool IsCOD = true, int approximateValue = 0);
        bool getDefulteSenderState(int customerId, out int senderStateId);
        CategoryInfoModel GetCategoryInfo(int categoryId);
        bool IsLockThisToSendToPost(OrderStatusToPost state, int orderId);
        void MarkOrder(OrderRegistrationMethod orderMethod, Order order);
        OrderRegistrationMethod GetOrderRegistrationMethod(Order order);
        string GetBase64Image(string TrackingNumber);
        bool _SendDataToPost(Order order, out string strError);
        void _GenerateBarcodes(Order order, out List<string> strError);
        int GetValueAddedbyAgent(OrderItem orderitem);
        List<SelectListItem> ListOfService();
        int[] getCODCost(int shipmentId);
        CoardinationStatisticModel GetOrderStateStatistic(string OrderIds, int customerId);
        string getOrderItemContent(OrderItem orderitem);
        int GetItemWeightFromAttr(OrderItem orderitem);
        string getOrderItemWehghtName(OrderItem orderitem, bool ignoreBime = false);
        int GetItemPriceFromAttr(OrderItem orderitem);
        int GetItemCostFromAttr(OrderItem orderitem);
        bool setDataCollect(int shipmentId);
        void SavePostCoordination(List<int> orderIds, string Desc);
        int getAddtionalFee(Customer customer, int? shipmentId);
        //decimal getProductPrice(Core.Domain.Catalog.Product product, Customer customer, int storeId);
        void CleanToSendDataToPostAgain(List<int> LstOrderId);
        bool IsMultiShippment(Order order);
        MultiShipmentModel getShipmentFromMultiShipment(OrderItem orderitem, int i);
        List<OrderShipmentModel> getOrderShipment(out int count, int orderId, int pageIndex, int pageSize);
        OrderAverageReportLine GetOrderAverageReportLine(int storeId = 0,
            int vendorId = 0, int billingCountryId = 0,
            int orderId = 0, string paymentMethodSystemName = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingEmail = null, string billingLastName = "", string orderNotes = null,
            int SenderStateProvinceId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
            string ReciverName = null, string SenderName = null);
        decimal ProfitReport(int storeId = 0, int vendorId = 0,
            int billingCountryId = 0, int orderId = 0, string paymentMethodSystemName = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingEmail = null, string billingLastName = "", string orderNotes = null,
           int SenderStateProvinceId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
           string ReciverName = null, string SenderName = null);

        List<ExtendedShipmentModel> GetAllShipments(out int count, int vendorId = 0,
                int warehouseId = 0,
                int billingCountryId = 0,
                int billingStateId = 0,
                string billingCity = null,
                int shippingCountryId = 0,
                int shippingStateId = 0,
                string shippingCity = null,
                string trackingNumber = null,
                bool loadNotShipped = false,
                DateTime? createdFromUtc = null,
                DateTime? createdToUtc = null,
                int pageIndex = 0,
                int pageSize = int.MaxValue,
                int PostmanId = 0,
                string ReciverName = null,
                string SenderName = null,
                int orderId = 0,
                int ignorCod = 1,
                int ShipmentState = 0,
                int ShipmentState2 = 0,
                int CODShipmentEventId = 0,
                int CodTrackingDayCoun = 0,
                DateTime? CodEndTime = null,
                bool HasGoodsPrice = false,
                int OrderCustomerId = 0
            );

        List<ExtendedShipmentModel> GetAllShipmentsByStatus(out int count, int vendorId = 0,
                int warehouseId = 0,
                int billingCountryId = 0,
                int billingStateId = 0,
                string billingCity = null,
                int shippingCountryId = 0,
                int shippingStateId = 0,
                string shippingCity = null,
                string trackingNumber = null,
                bool loadNotShipped = false,
                DateTime? createdFromUtc = null,
                DateTime? createdToUtc = null,
                int pageIndex = 0,
                int pageSize = int.MaxValue,
                int PostmanId = 0,
                string ReciverName = null,
                string SenderName = null,
                int orderId = 0,
                int ignorCod = 1,
                int ShipmentState = 0,
                int ShipmentState2 = 0,
                int CODShipmentEventId = 0,
                int CodTrackingDayCoun = 0,
                DateTime? CodEndTime = null,
                bool HasGoodsPrice = false,
                int OrderCustomerId = 0,
                int statusId = 0
            );
        void ProcessPhoneOrder(Order order);
        List<CustomOrder> SearchOrders(out CoardinationStatisticModel CoardinationStatistic, out int count, int storeId = 0,
           int vendorId = 0, int customerId = 0,
           int productId = 0, int affiliateId = 0, int warehouseId = 0,
           int billingCountryId = 0, string paymentMethodSystemName = null,
           DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
           List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
           string billingEmail = null, string billingLastName = "",
           string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue,
           int SenderStateProvinceId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
            string ReciverName = null, string SenderName = null, int OrderId = 0, bool IsOrderOutDate = false, int orderState = 0);

        List<SelectListItem> getUserInRole(int RoleId, int? country, int? state, string city);
        void ChossePostMan(int postMan, int PostAdmin, int ShipmentId);

        void SetPersuitCodeMode(int ShipmentId, bool IsPersuitCodeAuto);

        //string getBarcode(int ShipmentId);
        //string GenerateBarcodeFromPost(int shipmentId, int DefulteSenderStateId, int? shippingAddressId = null);

        byte[] getBarocdeImage(Shipment shipment);
        void Log(string header, string Message);
        void SendSmsForCustomerAndAdmin(Shipment model);
        void UpdateFromPost(Shipment shipment, ExtendedShipmentSetting setting = null);
        string getPhoneNumber(int customerId);
        string SendDataToPost(int shipmentId);
        bool ReadExcelFile(MemoryStream stream);
        List<BarcodeRepositoryModel> readBarcodeRepository(string barcode, int shipmentId);
        string SetBarcodeIsUsed(string barcode, int shipmentId);
        int GetBieme(OrderItem orderItem);
        int GetBiemeDiff(OrderItem orderItem);
        int GetkarotnPrice(OrderItem orderItem);
        void SaveCateguryPostType(string categuryName, int categuryId, int postType);
        List<CateguryPostTypeModel> ReadCategoryPostType();
        bool DeleteCateguryPostType(int id);
        int GetPostType(List<int> categuryIds);
        OrderBill orderTotal(Order order);
        void SendToCod(Order order, bool IsCOD);
        void ProcessOrderForPost(Order order);
        OrderBill orderItemTotal(OrderItem orderitem);
        void Print_58mToPdf(IList<Order> orders, Stream stream);
        OrderBill CODorderItemTotal(OrderItem orderitem);
        void InsertOrderNote(string note, int orderId);
        List<node> ShipmentTreeViewByStatus(int orderState, OrderShipmentStatusEnum shipmentStatusEnum, int categoryId);

        int CalcCODPriceApi(Core.Domain.Catalog.Product product
            , int weight
            , string attributesXml
            , string userName
            , int countryId
            , int stateId
            , int postType
            , out string error);
        int getTotalWeight(Order order);
        Tbl_Address_LatLong GetAddressLocation(int AddressId);
        void InsertAddressLocation(int AddressId, float Lat, float Lon);
        PostBarcodeGeneratorOutputModel NewGenerateBarcodeFromPost(int shipmentId, int SenderStateId, int? shippingAddressId = null);
        int getSourceByOrder(int OrderId);
    }
}
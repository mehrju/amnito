using System;
using System.Collections.Generic;
using Nop.Plugin.Orders.BulkOrder.Models;
using Nop.Services.Orders;
using System.IO;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Orders.MultiShipping.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Plugin.Orders.BulkOrder.Services
{
    public interface IBulkOrderService
    {
        List<SelectListItem> getcategoryFileType(int FileType, bool ispeyk);
        List<SelectListItem> getForinCountry();
        bool ReadExcelFile_PostKhone(MemoryStream stream
            , string fileName
            , int customerId
            , string discountCouponCode
            , bool PrintLogo
            , bool SendSms
            , int ServiceSort//1 = cheapest , 2 = fastest
            , int FileType//1 = domestic online pay,2 = domestic CashOnDelivery,3 = International
            , bool HasAccessToPrinter
            , out BulkOrderModel bulkOrdermodel,
            string Sender_FirstName,
            string Sender_LastName,
            string Sender_mobile,
            string Sender_Country,
            string Sender_State,
            string Sender_City,
            string Sender_PostCode,
            string Sender_Address,
            string Sender_Email,
             string Sender_lat,
            string Sender_long,
             int ServiceId = 0
            );
        Task<List<ServiceInfoModel>> getServiceInfo(_getServiceInfoModel model);
        List<PlaceOrderResult> ProcessOrderList_PostKhones(BulkOrderModel bulkOrder = null, List<CheckoutItemApi> apiModel = null,
        int customerId = 0, string discountCouponCode = null);

        bool ReadExcelFile(MemoryStream stream, string fileName, int customerId, bool IsCod, string discountCouponCode);
        bool DeleteBulkOrder(int Id, int customerId, out string msg);
        PlaceOrderResult ProcessOrderList(BulkOrderModel bulkOrder = null, List<CheckoutItemApi> apiModel = null,
             int customerId = 0, string discountCouponCode = null);
        string GetUniqueFileName(string fileName);

        List<BulkOrderModel> getBulkOrderList(out int count, int pageIndex = 0, int pageSize = 999999, int customerId = 0,
            string CustomerName = null, DateTime? createDateFrom = null, DateTime? createDateTo = null, int OrderStatusId = 0, int PaymentStatusId = 0);

        void InsertBulkOrder(BulkOrderModel model);
        void UpdateBulkOrder(BulkOrderModel model);
        BulkOrderModel GetBulkOrder(int bulkOrderId);
        void ChangeBulkOrderStatus(int bulkOrderId, OrderStatus orderStatus);
        Product DetectProduct(int wehightG, string serviceType);
        Product DetectProduct(int serviceType);
        int? getCheckoutAttributePrice(int Wehight_g, string Insurance, string Carton, bool isCod, int productId, bool AccessPrintBill);

        int CalcCodPrice(Product product
            , int weight
            , string userName
            , int countryId
            , int steteId
            , int postType
            , string cartonSizeName
            , string insuranceName
            , int goodsPrice
            , bool AccessPrintBill
            , out string error);
        bool _SendDataToPost(Order order, out string strError);

        OrderPriceModel GetCheckoutAttributePriceSeparately(int Weight,
            string Insurance,
            decimal length,
            decimal width,
            decimal height,
            bool isCod,
            bool printBill,
            bool hasSms,
            bool hasLogo,
            bool needCarton,
            int approximateValue,
            int senderCityId,
            int receiverCityId,
            int? productId,
            int receiverCountryId = 0,
            string Address = "");
    }
}

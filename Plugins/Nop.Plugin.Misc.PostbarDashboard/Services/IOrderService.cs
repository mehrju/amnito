using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Services.Localization;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Services
{
    public interface IOrderServices
    {
        int GetCustomerOrderCount(int customerId);

        OrderModel GetCustomerLastOrder(int customerId);
        string getSubMarketFromUrl();

        IPagedList<Order> SearchOrders(int storeId = 0,
           int vendorId = 0, int customerId = 0,
           int productId = 0, int affiliateId = 0, int warehouseId = 0,
           int billingCountryId = 0, string paymentMethodSystemName = null,
           DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
           List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
           string billingEmail = null, string billingLastName = "",
           string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue
            , int SenderStateProvinceId = 0, int SenderCountryId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
           string ReciverName = null, string SenderName = null, int OrderIdFrom = 0, int OrderIdTo = 0, bool IsOrderOutDate = false, int orderState = 0, List<int> serviceTypes = null);

        IList<OrderBillDetail> SearchOrderBillDetail(int pageIndex = 0, int pageSize = int.MaxValue,
            int customerId = 0, int SenderStateProvinceId = 0, int SenderCountryId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
           string ReciverName = null, int OrderIdFrom = 0, int OrderIdTo = 0, int PaymentStatus = 0, int OrderStatus = 0,
           DateTime? createdFromUtc = null, DateTime? createdToUtc = null, List<int> ServiceTypes = null);

        IList<OrderTrackingBarcode> SearchOrderBarcode(int pageIndex = 0, int pageSize = int.MaxValue,
            int customerId = 0, int SenderStateProvinceId = 0, int SenderCountryId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
           string ReciverName = null, int OrderIdFrom = 0, int OrderIdTo = 0, int PaymentStatus = 0, int OrderStatus = 0,
           DateTime? createdFromUtc = null, DateTime? createdToUtc = null, List<int> ServiceTypes = null);

        OrdersSum GetOrdersSumByCustomer(int customerId);
        IList<SearchedOrder> SearchOrders(int pageIndex = 0, int pageSize = int.MaxValue,
          int customerId = 0, int SenderStateProvinceId = 0, int SenderCountryId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
         string ReciverName = null, int OrderIdFrom = 0, int OrderIdTo = 0, int PaymentStatus = 0, int OrderStatus = 0,
         DateTime? createdFromUtc = null, DateTime? createdToUtc = null, List<int> ServiceTypes = null);
    }
}

using Nop.Web.Factories;
using Nop.Web.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface IExtnOrderModelFactory: IOrderModelFactory
    {
        CustomerOrderListModel ExnPrepareCustomerOrderListModel(int storeId = 0,
           int vendorId = 0, int customerId = 0,
           int productId = 0, int affiliateId = 0, int warehouseId = 0,
           int billingCountryId = 0, string paymentMethodSystemName = null,
           DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
           List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
           string billingEmail = null, string billingLastName = "",
           string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue,
           int SenderStateProvinceId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
           string ReciverName = null, string SenderName = null, List<int> OrderStatusIds = null
            , List<int> paymentStatusIds = null, List<int> shippingStatusIds = null, int ProductId = 0,int OrderId = 0);
    }
}

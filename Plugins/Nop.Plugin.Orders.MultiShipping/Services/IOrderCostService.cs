using Nop.Plugin.Orders.MultiShipping.Models;
using System.Collections.Generic;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public interface IOrderCostService
    {
        IEnumerable<OrderItemCostInfoViewModel> GetOrderItemsCost(int orderId = 0, int shipmentId = 0);
    }
}
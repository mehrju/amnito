using Nop.plugin.Orders.ExtendedShipment.Models.PreOrderModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services.PreOrderService
{
    public interface IPreOrderService
    {
        int InsertPreOrder(CheckoutParcellModel Model, int CustomerId, out string Error, out string url);
        CheckoutParcellModel PreOrderCheckout(int PreOrderId);
        bool SetOrderId(string UniqueReferenceNo, int OrderId);
    }
}

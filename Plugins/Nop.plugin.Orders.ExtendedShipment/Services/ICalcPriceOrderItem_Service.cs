using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface ICalcPriceOrderItem_Service
    {
        int Get_IncomePrice_by_OrderItemId(int OrderItem);
        bool Update_IncomePrice_by_OrderItemId(int OrderItem, int price);
    }
}

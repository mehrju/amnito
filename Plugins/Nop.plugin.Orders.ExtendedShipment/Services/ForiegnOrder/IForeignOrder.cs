using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.plugin.Orders.ExtendedShipment.Services.ForiegnOrder;

namespace Nop.plugin.Orders.ExtendedShipment.Services.ForiegnOrder
{
    public interface IForeignOrder
    {
        List<ForeignOrder.ForeinOrderModel>  GetForeinOrdersList(int ServiceId,int ForeignOrderStatus,int OrderId,
            DateTime? OrderDateFrom,DateTime? OrderDateTo,out int Count,int pageIndex = 0, int pageSize = int.MaxValue);
    }
}

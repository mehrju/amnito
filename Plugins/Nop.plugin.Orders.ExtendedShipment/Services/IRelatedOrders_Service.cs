using Nop.plugin.Orders.ExtendedShipment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface IRelatedOrders_Service
    {
        bool Insert(int ParentOrderId, int ChildOrderId);
        Tb_RelatedOrders GetTb_RelatedOrders_By_ParentOrderId(int ParentOrderId);
    }
}

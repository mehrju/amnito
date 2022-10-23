using Nop.Core;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Domain.PhoneOrder;
using Nop.plugin.Orders.ExtendedShipment.Models.PhoneOrder;
using System.Collections.Generic;

namespace Nop.plugin.Orders.ExtendedShipment.Services.PhoneOrder
{
    public interface IPhoneOrderService
    {
        void InsertPhoneOrder(PhoneOrderModel model);
        PagedList<PhoneOrderModel> GetPagedPhoneOrders(string fromDate, string toDate, int pageIndex = 0, int pageSize = int.MaxValue);
        Tbl_PhoneOrder GetPhoneOrderById(int id);
        void UpdatePhoneOrder(Tbl_PhoneOrder entity);
        List<PostService> ListOfService();
        int? CanRegisterPhoneOrder(int stateId);
    }
}
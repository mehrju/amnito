using Nop.plugin.Orders.ExtendedShipment.Domain;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface IApiOrderRefrenceCodeService
    {
        string getRefrenceCode(int orderId);
        bool CheckAndInsertApiOrderRefrenceCode(int customerId, string refrenceNo, out Tbl_ApiOrderRefrenceCode obj);
        void SetOrderId(int customerId, string refrenceNo, int orderId);
        int getOrderId(string RefrenceCode);
    }
}
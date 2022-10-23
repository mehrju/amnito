namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public interface IReOrderService
    {
        void InsertOrderJson(int orderId, string orderJson, bool isWebApi = false);
    }
}
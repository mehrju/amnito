namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface IOrderItemsRecordService
    {
        int ShipmentHasPacking(int orderItemId);
    }
}
using Nop.plugin.Orders.ExtendedShipment.Models;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface ICustomerSettingService
    {
		CustomerSetting GetCustomerSetting(int customerId = 0);
    }
}
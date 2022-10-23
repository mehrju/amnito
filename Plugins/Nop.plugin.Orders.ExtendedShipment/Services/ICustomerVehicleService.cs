using Nop.plugin.Orders.ExtendedShipment.Domain;
using System.Collections.Generic;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface ICustomerVehicleService
    {
        void SaveCustomerVehicle(Tbl_CustomerVehicle model);
        List<Tbl_CustomerVehicle> GetCustomerVehicles(int customerId);
        Tbl_CustomerVehicle GetCustomervehicleById(int Id);
        void RemoveCustomerVehicle(Tbl_CustomerVehicle entity);
    }
}
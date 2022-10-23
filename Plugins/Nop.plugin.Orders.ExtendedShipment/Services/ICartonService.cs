using Nop.plugin.Orders.ExtendedShipment.Domain;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface ICartonService
    {
        int CalculateSize9Price(decimal length, decimal width, decimal height);
        string GetRequiredCartonBySize(decimal length, decimal width, decimal height);
        Tbl_CartonInfo getcartonInfoBySizeName(string name);
    }
}
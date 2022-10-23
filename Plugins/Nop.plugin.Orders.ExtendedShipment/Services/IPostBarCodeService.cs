using Nop.plugin.Orders.ExtendedShipment.Models;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface IPostBarCodeService
    {
        PostBarcodeGeneratorOutputModel GenerateAndGetBarcode(PostBarcodeGeneratorInputModel model);
    }
}
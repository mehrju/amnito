using Nop.Plugin.Misc.ShippingSolutions.Models.kalaResan;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services
{
    /// <summary>
    ///  سرویس کالا رسان
    /// </summary>
    public interface IkalaResan_Service
    {
        Task<RegisterParcelOutoutModel> RegisterShipment(RegisterParcelInputModel model);
        Task<TrackingOutputtModel> TrackShipment(TrackingInputModel model);
        Task<CancelOrderOutputModel> CancelOrder(TrackingInputModel model);
        Task<GetPriceOutputModel> GetPrice(GetPriceInputModel model);
    }
}
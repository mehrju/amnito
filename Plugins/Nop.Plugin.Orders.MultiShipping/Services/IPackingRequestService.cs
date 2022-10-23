using Nop.Plugin.Orders.MultiShipping.Domain;
using Nop.Plugin.Orders.MultiShipping.Enums;
using System.Collections.Generic;

namespace Nop.Plugin.Orders.MultiShipping.Services
{

    public interface IPackingRequestService
    {
        bool ShipmentHasPackingRequest(int shipmentId);
        void InsertPackingRequest(Tbl_ShipmentPackingRequest shipmentPackingRequest);
        void UpdatePackingRequest(Tbl_ShipmentPackingRequest shipmentPackingRequest);
        Tbl_ShipmentPackingRequest GetPackingRequestById(int id);
        IEnumerable<Tbl_ShipmentPackingRequest> SearchPackingRequests(int shipmentId = 0, string kartonSizeItemName = null,
            int? length = null, int? width = null, int? height = null,
            ShipmentPackingRequestStatus? packingRequestStatus = null, bool? isSmsSent = null);

        bool IsRequestedPackingPurchased(int shipmentId, List<CartonSizeSelected> cartonSizes, out string msg);
    }
}
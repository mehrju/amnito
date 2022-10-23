using Nop.Core.Domain.Orders;
using Nop.plugin.Orders.ExtendedShipment.Models.distOrCollect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nop.plugin.Orders.ExtendedShipment.Services.CollectorService;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface ICollectorService
    {
        int assignToCollector(int shipmentId);
        bool ChooseCollector(int shipmentId, int CollectorId, string desc);
        List<CollectorList> GetAllCollectorList();
        bool HasCollectorInState(int StateId);
        List<CollectorList> GetBikers();
        PriceResponse CalculatePrice(int SenderCountry, int SenderState, string Address, int CustomerId, int ServiceId, int orderId,
            ServicesType _servicesType);
        void assignToDistributer(Order order);
        bool SaveCollectingPrices(PriceResponse pricesData,int orderId);
        void SaveCollectingPrices(PriceResponse collectingInfo);
    }
}

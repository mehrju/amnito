using Nop.Core.Domain.Shipping;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public static class ExtendedShipmentModelExtensions
    {
        /// <summary>
        /// Get the tracker of the shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="shippingService">Shipping service</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <returns>Shipment tracker</returns>
        public static IShipmentTracker GetShipmentTracker(this ExtendedShipmentModel shipment, IShippingService shippingService, ShippingSettings shippingSettings)
        {
            if (!shipment.Order.PickUpInStore)
            {
                var shippingRateComputationMethod = shippingService.LoadShippingRateComputationMethodBySystemName(shipment.Order.ShippingRateComputationMethodSystemName);
                if (shippingRateComputationMethod != null &&
                    shippingRateComputationMethod.PluginDescriptor.Installed)
                    //shippingRateComputationMethod.IsShippingRateComputationMethodActive(shippingSettings))
                    return shippingRateComputationMethod.ShipmentTracker;
            }
            else
            {
                var pickupPointProvider = shippingService.LoadPickupPointProviderBySystemName(shipment.Order.ShippingRateComputationMethodSystemName);
                if (pickupPointProvider != null &&
                    pickupPointProvider.PluginDescriptor.Installed)
                    //pickupPointProvider.IsPickupPointProviderActive(shippingSettings))
                    return pickupPointProvider.ShipmentTracker;
            }

            return null;
        }
    }
}

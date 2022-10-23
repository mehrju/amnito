using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Order;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Orders;

namespace BS.Plugin.NopStation.MobileWebApi.Factories
{
    /// <summary>
    /// Represents the interface of the order model factory
    /// </summary>
    public partial interface IOrderModelFactoryApi
    {
        /// <summary>
        /// Prepare the customer order list model
        /// </summary>
        /// <returns>Customer order list model</returns>
        CustomerOrderListResponseModel PrepareCustomerOrderListModel();

        /// <summary>
        /// Prepare the order details model
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Order details model</returns>
        OrderDetailsResponseModel PrepareOrderDetailsModel(Order order);

        /// <summary>
        /// Prepare the shipment details model
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>Shipment details model</returns>
        ShipmentDetailsResponseModel PrepareShipmentDetailsModel(Shipment shipment);

        /// <summary>
        /// Prepare the customer reward points model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>Customer reward points model</returns>
        //CustomerRewardPointsModel PrepareCustomerRewardPoints(int? page);
    }
}

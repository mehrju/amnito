using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.plugin.Orders.ExtendedShipment.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nop.Web.Areas.Admin.Models.Orders.ShipmentModel;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class ExtendedShipmentModel: BaseEntity
    {
        private ICollection<ShipmentItem> _shipmentItems;

        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the tracking number of this shipment
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets the total weight of this shipment
        /// It's nullable for compatibility with the previous version of nopCommerce where was no such property
        /// </summary>
        public decimal? TotalWeight { get; set; }

        /// <summary>
        /// Gets or sets the shipped date and time
        /// </summary>
        public DateTime? ShippedDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the delivery date and time
        /// </summary>
        public DateTime? DeliveryDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets the entity creation date
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets the order
        /// </summary>
        public Order Order { get; set; }

        /// <summary>
        /// Gets or sets the shipment items
        /// </summary>
        public ICollection<ShipmentItem> ShipmentItems
        {
            get { return _shipmentItems ?? (_shipmentItems = new List<ShipmentItem>()); }
            protected set { _shipmentItems = value; }
        }
        public string postManName { get; set; }
        public decimal orderPrice { get; set; }
        public string HasDiffrentWeight { get; set; }
        public string ShippedDate { get; set; }
        public string strTotalWeight { get; set; }
        public bool CanShip { get; set; }
        public string DeliveryDate { get; set; }
        public bool CanDeliver { get; set; }
        public string CustomOrderNumber { get; set; }
        public List<ShipmentItemModel> Items { get; set; }
        public string TrackingNumberUrl { get; set; }
        public int? LatestEventId { get; set; } 
        [NotMapped]
        public string LatestEvent
        {
            get
            {
                return ((OrderShipmentStatusEnum)(LatestEventId.HasValue ? LatestEventId.Value : 113)).GetDisplayName();
            }
        }

        public IList<ShipmentStatusEventModel> ShipmentStatusEvents { get; set; }
        public DateTime? DataCollect { get; set; }
        public int BillingAddressId { get; set; }
        public int BillingCountryId { get; set; }
        public int BillingStateProvinceId { get; set; }
        public string BillingCountryName { get; set; }
        public string BillingStateProvinceName { get; set; }
        public string Address1 { get; set; }
        public string FullAddress { get; set; }
        public string HasBime { get; set; }
        public int GoodsPrice { get; set; }

        public string LastState { get; set; }
        public bool vaizShode { get; set; }
        public string IsBulk { get; set; }
        public  int CanChargWallet { get; set; }
        public bool showWalletbnt { get; set; }
        public int Count { get; set; }
        public int delayShippedDate { get; set; }
        public string LastShipmentState { get; set; }
        public string LastShipmentStateDate { get; set; }

        public int delayDataCollect { get; set; }
        public DateTime? CoordinationDate { get; set; }
        public bool NeedPrinter { get; set; }
        public bool NeedCarton { get; set; }
        public string CartonSizeName { get; set; }
        public string PostManMobile { get; set; }

        public int IdCategory { get; set; }
        public bool IsForeign { get; set; }

    }
}

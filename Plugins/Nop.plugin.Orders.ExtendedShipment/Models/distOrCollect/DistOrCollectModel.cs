using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.distOrCollect
{
    public class ParcelBasket
    {
        public List<BoxSizePricingInfo> Parcels { get; set; }
        public int CityId { get; set; }

        public ParcelBasket()
        {
            Parcels = new List<BoxSizePricingInfo>();
        }
    }
    public class BoxSizePricingInfo 
    {
        public int shipmnetId { get; set; }
        public int CollectingPrice { get; set; }
        public int Volume { get; set; }
    }
    public class PriceResponse
    {
        public int NewItemCollectingPrice { get; set; }
        public int PrevItemCollectingPrice { get; set; }
    }
}

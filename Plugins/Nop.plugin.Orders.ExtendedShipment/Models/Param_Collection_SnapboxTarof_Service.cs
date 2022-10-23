using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class Param_Collection_SnapboxTarof_Service
    {
        //ItemCollection Item { get; set; }
        public List<ItemCollection> Items { get; set; }
    }

    public class ItemCollection
    {
        public int Type { get; set; }
        public int shipmentid { get; set; }

       // public List<int> shipmentid { get; set; }
       // public Place Origin { get; set; }
        public Place Destination { get; set; }

    }
    public class Place
    {
        public String contactName { get; set; }
        public String address { get; set; }
        public String contactPhoneNumber { get; set; }
        public String comment { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }

    }
   
}

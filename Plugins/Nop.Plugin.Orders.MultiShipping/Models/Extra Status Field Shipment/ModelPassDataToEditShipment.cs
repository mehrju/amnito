using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models.Extra_Status_Field_Shipment
{
    public class ModelPassDataToEditShipment
    {
        public int shipmentId { get; set; }
        public string CollectDate { get; set; }
        public  string ShippedDate { get; set; }
        public string DeliveryDate { get; set; }
        //1
        public int Value_mafgho { get; set; }
        public string Des_mafgho { get; set; }
        //2
        public int Value_khesarat { get; set; }
        public string Des_khesarat { get; set; }
        //3
        public int Value_gheramatmafghod { get; set; }
        public string Des_gheramatmafghod { get; set; }
        //4
        public int Value_takhir { get; set; }
        public string Des_takhir { get; set; }
        //5
        public int Value_shekayat { get; set; }
        public string Des_shekayat { get; set; }


        public string Weight { get; set; }

    }
}

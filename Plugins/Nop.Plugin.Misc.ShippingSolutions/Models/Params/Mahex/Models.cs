using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Mahex
{
    public class Params_Mahex_GetPrices
    {
        public Params_Mahex_GetPrices()
        {
            from_address = new to_address();
            to_address = new to_address();

            parcels = new List<parcels>();

        }
        public to_address from_address { get; set; }
        public to_address to_address { get; set; }

        public List<parcels> parcels { get; set; }
        public string package_type { get; set; }
        public string declared_value { get; set; }
        
    }
    public class to_address
    {
        public string city_code { get; set; }
        public string postal_code { get; set; }
        public string Street { get; set; }

    }
    //public class parcels
    //{
    //    public decimal weight { get; set; }
    //}




    public class Params_Mahex_createShipment
    {
        public Params_Mahex_createShipment()
        {
            from_address = new address();
            to_address = new address();
            parcels = new List<parcels>();
            price_items = new List<price_items>();

        }
        public address from_address { get; set; }
        public address to_address { get; set; }
        public List<parcels> parcels { get; set; }
        public List<price_items> price_items { get; set; }

        public string reference { get; set; }
        public decimal cod_amount { get; set; }
        public string charge_party { get; set; }
        public string payment_method_type { get; set; }
        public string delivery_date { get; set; }

        public string delivery_time_from { get; set; }
        public string delivery_time_to { get; set; }
    }
    public class address
    {
        public string client_id { get; set; }
        public string type { get; set; }
        public string gender { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string organization { get; set; }
        public string national_id { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
        public string city_code { get; set; }
        public string postal_code { get; set; }
        public string street { get; set; }
        public string remarks { get; set; }

    }
    public class parcels
    {
        public string id { get; set; }
        public decimal length { get; set; }
        public decimal width { get; set; }
        public decimal height { get; set; }
        public decimal weight { get; set; }
        public string content { get; set; }
        public string package_type { get; set; }
        public int declared_value { get; set; }
    }
    public class price_items
    {
        public int amount { get; set; }
        public string code { get; set; }
    }
    public class Params_Mahex_Tracking
    {
        //string as  http://api.mahex.com/v2/track/partnumber/{part_number}
        //or string as  http://api.mahex.com/v2/track/{waybill_number}
        public string waybill_number { get; set; }
        public string reference { get; set; }
        public string part_number { get; set; }
    }
}

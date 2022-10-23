using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Mahex
{
    public class Result_Mahex_GetQuote
    {
        public Result_Mahex_GetQuote()
        {
            data = new data();
            status = new status();
        }
        public data data { get; set; }
        public status status { get; set; }

    }
    public class data
    {
        public data()
        {
            rate = new rate();
        }
        public rate rate { get; set; }
        public string delivery_window { get; set; }
    }
    public class rate
    {
        public product product { get; set; }
        public decimal amount { get; set; }
    }
    public class product
    {
        public string code { get; set; }
        public int version { get; set; }

    }
    public class status
    {
        public decimal code { get; set; }
        public string state { get; set; }
        public string invalid_field { get; set; }
        public string message { get; set; }

    }
    public class Result_Mahex_GetCities
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }




    public class Result_Mahex_Bulkimport
    {
        public Result_Mahex_Bulkimport()
        {
            data = new data2();
            status = new status();
        }
        public data2 data { get; set; }
        public status status { get; set; }

    }
    public class data2
    {
        public string shipment_uuid { get; set; }
    }


    public class Result_Mahex_Tracking
    {
        public string waybill_number { get; set; }
        public string uuid { get; set; }
        public string reference { get; set; }
        public string create_date { get; set; }
        public string update_date { get; set; }
        public int total_parts { get; set; }
        public string current_state { get; set; }
        public parcels parcels { get; set; }
        public status status { get; set; }
    }

    public class parcels
    {
        public string id { get; set; }
        public float length { get; set; }
        public float width { get; set; }
        public float height { get; set; }
        public float weight { get; set; }
        public string content { get; set; }
        public string current_state { get; set; }
        public string current_location { get; set; }
    }




    public class GetShipmentResult
    {
        public ShipmentResultdata data { get; set; }
        public status status { get; set; }

    }
    public class ShipmentResultdata 
    {
        public string waybill_number { get; set; }
        public string uuid { get; set; }
        public string charge_party { get; set; }
        public string reference { get; set; }
        public address from_address { get; set; }
        public address to_address { get; set; }
        public List<parcels3> parcels { get; set; }
        public double cod_amount { get; set; }
        public string package_type { get; set; }
        public string declared_value { get; set; }
    }


    public class address
    {
        public string client_id { get; set; }
        public string national_id { get; set; }
        public string type { get; set; }
        public string prefix { get; set; }
        public string gender { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone { get; set; }
        public string city_code { get; set; }
        public string province_code { get; set; }
        public string country_code { get; set; }
        public string postal_code { get; set; }

    }
    public class parcels3
    {
        public string id { get; set; }
        public decimal length { get; set; }
        public decimal width { get; set; }
        public decimal height { get; set; }
        public decimal weight { get; set; }
        public string content { get; set; }

    }



    public class VoidShipmentResult
    {
        public status status { get; set; }
    }
}

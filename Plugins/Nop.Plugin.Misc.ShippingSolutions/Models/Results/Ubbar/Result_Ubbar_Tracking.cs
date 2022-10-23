using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Ubbar
{
    /// <summary>خروجی تابع رهگیری سفارش -
    /// <para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///<para>DetailResiltTracking جزییات خروجی </para>
    /// 
    /// </summary>
    public class Result_Ubbar_Tracking
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public string EnMessage { get; set; }
        public DetailResiltTracking DetailResiltTracking { get; set; }

    }
    public class DetailResiltTracking
    {
        public int success_flag { get; set; }
        public OrderDetails order_details { get; set; }
        public List<object> warning_messages
        {
            get; set;
        }
    }
    public class SourceRegionCoordinate
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class DestinationRegionCoordinate
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class OrderDetails
    {
        public int order_id { get; set; }
        public string order_code { get; set; }
        public string load_type { get; set; }
        public string source_city { get; set; }
        public string destination_city { get; set; }
        public string source_state { get; set; }
        public string destination_state { get; set; }
        public string dispatch_hour { get; set; }
        public double distance { get; set; }
        public string dispatch_date { get; set; }
        public bool is_part_loads { get; set; }
        public int less_4_hours_left { get; set; }
        public int order_credit { get; set; }
        public string status_farsi { get; set; }
        public string status { get; set; }
        public string baarbari_name { get; set; }
        public object bearing { get; set; }
        public string importance { get; set; }
        public bool is_modified { get; set; }
        public string cancellation_reason { get; set; }
        public string cancellation_reason_comment { get; set; }
        public string baarnameh_photo { get; set; }
        public string vehicle_type { get; set; }
        public string vehicle_options { get; set; }
        public string vehicle_type_farsi { get; set; }
        public string vehicle_options_farsi { get; set; }
        public double load_value { get; set; }
        public double weight { get; set; }
        public double transport_price { get; set; }
        public double surplus_costs { get; set; }
        public double discount { get; set; }
        public double driver_income { get; set; }
        public string announce_type { get; set; }
        public string delivery_confirmation_code { get; set; }
        public double dispatch_date_ms { get; set; }
        public string creation_date { get; set; }
        public string unload_option { get; set; }
        public string source_neighborhood { get; set; }
        public string destination_neighborhood { get; set; }
        public double volume { get; set; }
        public double length { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public string description { get; set; }
        public string payment_type { get; set; }
        public string payment_type_farsi { get; set; }
        public string packaging_type_english { get; set; }
        public string packaging_type { get; set; }
        public string packaging_type_farsi { get; set; }
        public string assignment_date { get; set; }
        public int assignment_date_ms { get; set; }
        public string pickup_date { get; set; }
        public int pickup_date_ms { get; set; }
        public string delivery_date { get; set; }
        public int delivery_date_ms { get; set; }
        public string cancellation_date { get; set; }
        public double cancellation_date_ms { get; set; }
        public string baarnameh_options { get; set; }
        public string baarnameh { get; set; }
        public string baarnameh_type { get; set; }
        public string baarnameh_options_persian { get; set; }
        public string vehicle_suspension_type { get; set; }
        public string source_lat { get; set; }
        public string source_lng { get; set; }
        public string source_address { get; set; }
        public string sender_mobile_phone { get; set; }
        public string sender_phone { get; set; }
        public string sender_name { get; set; }
        public string sender_company { get; set; }
        public string source_region_id { get; set; }
        public string source_region_name { get; set; }
        public SourceRegionCoordinate source_region_coordinate { get; set; }
        public string address_id_source { get; set; }
        public object source_address_title { get; set; }
        public string source_address_phone { get; set; }
        public string source_address_mobile_no { get; set; }
        public object source_address_company_name { get; set; }
        public object source_address_person_name { get; set; }
        public string destination_address { get; set; }
        public string receiver_phone { get; set; }
        public string receiver_mobile_phone { get; set; }
        public string receiver_name { get; set; }
        public string destination_lat { get; set; }
        public string destination_lng { get; set; }
        public string receiver_company { get; set; }
        public string destination_region_id { get; set; }
        public string destination_region_name { get; set; }
        public DestinationRegionCoordinate destination_region_coordinate { get; set; }
        public string address_id_destination { get; set; }
        public object destination_address_title { get; set; }
        public string destination_address_phone { get; set; }
        public string destination_address_mobile_no { get; set; }
        public object destination_address_company_name { get; set; }
        public object destination_address_person_name { get; set; }
        public bool postpone_order { get; set; }
        public string driver_name { get; set; }
        public string driver_cellphone { get; set; }
        public string driver_id { get; set; }
        public string number_plate { get; set; }
        public string driver_photo { get; set; }
        public string driver_score { get; set; }
        public string driver_score_no_jobs { get; set; }
        public string driver_status { get; set; }
        public string driver_status_farsi { get; set; }
        public double sender_score { get; set; }
        public int sender_score_no_jobs { get; set; }
        public double receiver_score { get; set; }
        public double receiver_score_no_jobs { get; set; }
        public bool payment_flag { get; set; }
        public string online_payment_link { get; set; }
        public string online_payment_link_msg { get; set; }
        public string payment_method { get; set; }
        public string payment_status { get; set; }
        public string payment_status_english { get; set; }
    }
}

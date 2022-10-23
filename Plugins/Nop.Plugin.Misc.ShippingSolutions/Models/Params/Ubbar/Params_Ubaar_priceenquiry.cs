using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Ubbar
{
    /// <summary>مدل ورودی استعلام قیمت
    /// <para>weight اجباری: وزن بار واحد تن</para>               
    /// <para>source_city  نام شهرمبدا</para>          
    /// <para>destination_city  نام شهر مقصد</para>     
    /// <para>source_region_id اجباری: کد شهر مبدا</para>     
    /// <para>destination_region_id اجباری: کد شهر مقصد</para>
    /// <para>load_type اجباری</para>            
    /// <para>vehicle_type اجباری: نوع ماشین </para>         
    /// <para>vehicle_options اجباری : نوع ماشین</para>      
    /// <para>suspention_type</para>      
    /// <para>load_value </para>           
    /// <para>package_options اجباری انواعبسته بندی: </para>  
    /// karton','pallet','roll','falleh','bandil','kiseh_gooni
    /// <para>baarnameh اجباری</para>    
    /// 'khavar','tak','joft','treili',''
    /// <para>baarnameh_options  </para> 
    /// 'kompressi','otaghdar','baghaldar','kafi','hichkodam'
    /// <para> dispatch_date اجباری</para>        
    /// <para>dispatch_hour اجباری</para>    
    /// </summary>
    public class Params_Ubaar_priceenquiry
    {
        public double weight { get; set; }
        public string source_city { get; set; }
        public string destination_city { get; set; }
        public int source_region_id { get; set; }
        public int destination_region_id { get; set; }
        public string load_type { get; set; }
        public string vehicle_type { get; set; }
        public string vehicle_options { get; set; }
        public string suspention_type { get; set; }
        public double load_value { get; set; }
        public string package_options { get; set; }
        public string baarnameh { get; set; }
        public string baarnameh_options { get; set; }
        public string dispatch_date { get; set; }
        public string dispatch_hour { get; set; }
        public string announce_type { get; set; }
        //public string description { get; set; }
        //public string baarnameh_type { get; set; }
       // public string unload_option { get; set; }
        //public string order_code { get; set; }
        //public string channel_submit_type { get; set; }
        //public string source_address { get; set; }
        //public string destination_address { get; set; }
        //public string sender_name { get; set; }
        //public string sender_phone { get; set; }
        //public string sender_company { get; set; }
        //public string sender_mobile_phone { get; set; }
        //public string receiver_name { get; set; }
        //public string receiver_phone { get; set; }
        //public string receiver_company { get; set; }
        //public string receiver_mobile_phone { get; set; }
        //public string payment_type { get; set; }


        public (bool, String) IsValidParamParams_Ubaar_priceenquiry()
        {
            bool result = true;
            string Message = "";


            if (weight <= 0
                 || source_region_id <= 0
                 || destination_region_id < 0
                 || string.IsNullOrEmpty((load_type ?? "").Trim())
                 || string.IsNullOrEmpty((vehicle_type ?? "").Trim())
                 || string.IsNullOrEmpty((vehicle_options ?? "").Trim())
                 || string.IsNullOrEmpty((package_options ?? "").Trim())
                 //|| string.IsNullOrEmpty((baarnameh ?? "").Trim())
                 || string.IsNullOrEmpty((dispatch_date ?? "").Trim())
                 || string.IsNullOrEmpty((dispatch_hour ?? "").Trim())
                  )
            {
                result = false;
                Message = "Mandatory ters must be filled";

            }


            return (result, Message);
        }
    }
}

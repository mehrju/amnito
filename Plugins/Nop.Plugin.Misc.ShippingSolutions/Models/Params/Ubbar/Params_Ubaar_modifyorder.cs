using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Ubbar
{
    /// <summary>مدل ورودی تابه سفارش
    /// <para>weight                وزن بار: تن</para>
    /// <para>price                  قیمت کرایه</para>
    /// <para>source_region_id       کد منطقه مبدا</para>
    /// <para>destination_region_id  کد منطقه مقصد</para>
    /// <para>load_type             </para>
    /// <para>vehicle_type          </para>
    /// <para>vehicle_options       نوع ماشین</para>
    /// 'khavar','tak','joft','treili',''
    /// <para>suspention_type       نوع ماشین</para>
    /// 'kompressi','otaghdar','baghaldar','kafi','hichkodam'
    /// <para>load_value            </para>
    /// <para>description           </para>
    /// <para>package_options       </para>
    /// 'karton','pallet','roll','falleh','bandil','kiseh_gooni','hichkodam'
    /// <para>baarnameh_type        نوع بارنامه</para>
    /// 'baarnameh','havaleh', 'bijak' or 'hichkodam'
    /// <para>unload_option         </para>
    /// day or night
    /// <para>announce_type         </para>
    /// <para>dispatch_date         </para>
    /// <para>dispatch_hour         </para>
    /// <para>order_code            :اجباری</para>
    /// <para>channel_submit_type   </para>
    /// <para>source_address        اردس مبدا</para>
    /// <para>destination_address   اردس مقصد</para>
    /// <para>sender_name           نام فرستنده</para>
    /// <para>sender_phone          شماره فرستنده</para>
    /// <para>sender_company        شرکت فرشتنده</para>
    /// <para>sender_mobile_phone   موبایل فرستنده</para>
    /// <para>receiver_name         نام گیرنده</para>
    /// <para>receiver_phone        شماره تلفن کیرنده</para>
    /// <para>receiver_company      شرکت گیرنده</para>
    /// <para>receiver_mobile_phone موبایل گیرنده</para>
    /// <para>payment_type          نوع پرداخت کرایه :اجباری</para>
    /// "sender" or "receiver"
    /// </summary>
    public class Params_Ubaar_modifyorder
    {
        public double weight { get; set; }
        public double price { get; set; }
        public int source_region_id { get; set; }
        public int destination_region_id { get; set; }
        public string load_type { get; set; }
        public string vehicle_type { get; set; }
        public string vehicle_options { get; set; }
        public string suspention_type { get; set; }
        public double load_value { get; set; }
        public string description { get; set; }
        public string package_options { get; set; }
        public string baarnameh_type { get; set; }
        public string unload_option { get; set; }
        public string announce_type { get; set; }
        public string dispatch_date { get; set; }
        public string dispatch_hour { get; set; }
        public string order_code { get; set; }
        public string channel_submit_type { get; set; }
        public string source_address { get; set; }
        public string destination_address { get; set; }
        public string sender_name { get; set; }
        public string sender_phone { get; set; }
        public string sender_company { get; set; }
        public string sender_mobile_phone { get; set; }
        public string receiver_name { get; set; }
        public string receiver_phone { get; set; }
        public string receiver_company { get; set; }
        public string receiver_mobile_phone { get; set; }
        public string payment_type { get; set; }


        public (bool, string) IsValidParamsUbaarmodifyorder()
        {
            bool result = true;
            string Message = "";

            if (string.IsNullOrEmpty((order_code ?? "").Trim())
                || string.IsNullOrEmpty((payment_type ?? "").Trim())
              )
            {
                result = false;
                Message = "Mandatory parameters must be filled";

            }
            if (payment_type != "sender" && payment_type != "receiver")
            {
                result = false;
                Message = "payment_type Must have a value of sender or receiver";

            }
            if (unload_option != null && unload_option != "day" && unload_option != "night")
            {

                result = false;
                Message = "unload_option Must have a value of day or night ";

            }



            return (result, Message);
        }
    }
}

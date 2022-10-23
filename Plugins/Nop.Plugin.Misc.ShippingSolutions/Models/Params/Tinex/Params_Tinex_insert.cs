using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Tinex
{
    /// <summary>مدل ورودی تابع ثبت سفارش
    /// <para>order_no اجباری : شماره سقارش</para>
    /// <para>pre_paid وجه پیش سفارش دهنده </para>
    /// <para> remaining_pay_type  نوع پرداخت</para>
    /// <para>remaining_source_t ype منبع دریافت باقیمانده حساب</para>
    /// <para>customer_identify_ id اجباری: کد یکتای سفارش دهنده</para>
    /// <para>customer_first_name اجباری: نام سفارش دهنده</para>
    /// <para>customer_last_nam اجباری</para>
    /// <para>customer_phone_no اجباری</para>
    /// <para>customer_email</para>
    /// <para>customer_address  اجباری</para>
    /// <para>customer_postal_code</para>
    /// <para> sender_first_name  اجباری: نام فرستنده</para>
    /// <para> sender_last_name  اجباری</para>
    /// <para>sender_national_code</para>
    /// <para> sender_phone_no  اجباری</para>
    /// <para> sender_address  اجباری</para>
    /// <para> sender_lat</para>
    /// <para>sender_lng </para>
    /// <para>sender_plate_no</para>
    /// <para>receiver_first_name اجباری نام تحویل گیرنده</para>
    /// <para> receiver_last_name  اجباری</para>
    /// <para>receiver_national_ code</para>
    /// <para>receiver_phone_no  اجباری</para>
    /// <para> receiver_address اجباری</para>
    /// <para> receiver_lat </para>
    /// <para> receiver_lng</para>
    /// <para>receiver_plate_no </para>
    /// <para> cash_on_delivery</para>
    /// <para> package_value </para>
    /// <para>weight اجباری لیستی از وزن کالاها</para>
    ///
    /// </summary>
    public class Params_Tinex_insert
    {
        public string order_no { get; set; }
        public int pre_paid { get; set; }
        public string remaining_pay_type { get; set; }
        public string remaining_source_type { get; set; }
        public string customer_identify_id { get; set; }
        public string customer_first_name { get; set; }
        public string customer_last_name { get; set; }
        public string customer_phone_no { get; set; }
        public string customer_email { get; set; }
        public string customer_address { get; set; }
        public string customer_postal_code { get; set; }
        public string sender_first_name { get; set; }
        public string sender_last_name { get; set; }
        public string sender_national_code { get; set; }
        public string sender_phone_no { get; set; }
        public string sender_address { get; set; }
        public float sender_lat { get; set; }
        public float sender_lng { get; set; }
        public string sender_plate_no { get; set; }
        public string receiver_first_name { get; set; }
        public string receiver_last_name { get; set; }
        public string receiver_national_code { get; set; }
        public string receiver_phone_no { get; set; }
        public string receiver_address { get; set; }
        public float receiver_lat { get; set; }
        public float receiver_lng { get; set; }
        public string receiver_plate_no { get; set; }
        public List<SubOrders> sub_orders { get; set; }

        /// <summary>
        /// تابع بررسی کننده ورودی ها
        /// </summary>
        /// <returns></returns>
        public (bool, string) IsValidParams_Tinex_insert()
        {
            bool Status = true;
            string Message = "";

            if (remaining_pay_type != "credit" && remaining_pay_type != "cash")
            {
                Status = false;
                Message = "remaining_pay_type Must have a value of credit or cash";

            }
            if (remaining_source_type != "orderer" && remaining_source_type != "receiver" && remaining_source_type != "sender")
            {

                Status = false;
                Message = "remaining_source_type Must have a value of orderer or receiver  or sender";

            }

            if (   string.IsNullOrEmpty((order_no ?? "").Trim())
                || string.IsNullOrEmpty((customer_identify_id ?? "").Trim())
                || string.IsNullOrEmpty((customer_first_name ?? "").Trim())
                || string.IsNullOrEmpty((customer_last_name ?? "").Trim())
                || string.IsNullOrEmpty((customer_phone_no ?? "").Trim())
                || string.IsNullOrEmpty((customer_address ?? "").Trim())
                || string.IsNullOrEmpty((sender_first_name ?? "").Trim())
                || string.IsNullOrEmpty((sender_last_name ?? "").Trim())
                || string.IsNullOrEmpty((sender_phone_no ?? "").Trim())
                || string.IsNullOrEmpty((sender_address ?? "").Trim())
                || string.IsNullOrEmpty((receiver_first_name ?? "").Trim())
                || string.IsNullOrEmpty((receiver_last_name ?? "").Trim())
                || string.IsNullOrEmpty((receiver_phone_no ?? "").Trim())
                || string.IsNullOrEmpty((receiver_address ?? "").Trim())
                 )
            {
                Status = false;
                Message = "Mandatory parameters must be filled";

            }
            float TotalW = 0;
            if (sub_orders.Count > 0)
            {
                foreach (var item in sub_orders)
                {
                    TotalW += item.weight;
                }
            }
            else
            {
                Status = false;
                Message = "Weight should be between zero and 20 kg";

            }

            if (TotalW > 20)
            {
                Status = false;
                Message = "Weight should be between zero and 20 kg";

            }


            return (Status, Message);
        }

    }
    public class SubOrders
    {

        public int cash_on_delivery { get; set; }
        public int package_value { get; set; }
        public float weight { get; set; }
    }
}

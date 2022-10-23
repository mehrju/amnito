using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Chapar
{
    public class Params_Chapar_GetQuote
    {
        /// <summary>
        /// مدل ورودی استعلام قیمت
        /// <para> order سفارش</para>
        /// </summary>
        public Order order { get; set; }

        public (bool, string) IsValidParamsGetQuote()
        {
            bool State = true;
            string Message = "";
            if (string.IsNullOrEmpty((order.origin ?? "").Trim())
                || string.IsNullOrEmpty((order.destination ?? "").Trim()))
            {
                State = false;
                Message = "All mandatory requirements must be completed in accordance with the description ";
            }

            if (order.weight > 100 || order.weight < 0)
            {
                State = false;
                Message = "حداکثر وزن مجاز برای این سرویس 100 کیلو گرم می باشد";
            }
            //if (order.method != 1 && order.method != 6 && order.method != 11)
            //{
            //    State = false;
            //    Message = "The cn.service field  Must be Between 1:Ground 6:Air 11:Post , Postman Must Use 1 ";
            //}

            return (State, Message);
        }
    }
    /// <summary>
    /// مدل ورودی سفارش برای استعلام قیمت 
    /// <para>همه موارد اجباری هستند</para>
    /// <para>origin کد شهر مبدا</para>
    /// <para>destination کد شهر مقصد</para>
    /// <para>weight وزن محموله : کمتر یا مساوی 20 کیلوگرم</para>
    /// <para>یک محموله میتواند شامل چندین بسته باشد</para>
    /// <para>اگر یک محموله بیش از 20 کیلو شد باید با توجه به وزن در چندین بارنامه ثبت شود</para>
    /// <para>value ارزش محموله ریال</para>
    /// <para>method روش انتقال محموله </para>
    /// <para>عدد 1 زمینی(عادی) عدد 6 هوایی(اکپرس) و عدد 11 پستی: قرار شد شرکت پست بار عدد یک را وارد کند</para>
    /// <para>sender_code کد فرستنده شرکت پست بار 10825</para>
    /// <para>receiver_code خالی بگذارید</para>
    /// <para>cod روش دریافت مبلغ کالا از دیفات کننده که در قیمت پست تاثثیر میگذارد</para>
    /// <para>که مقادیر ان صفر و یک میباشد اگر یک بگذارید کالا به صورت امانی در اختیار سرویس سفیران هست و باید ارزش کالا را از مشتری بگیرد</para>
    /// </summary>
    public class Order
    {
        public string origin { get; set; }
        public string destination { get; set; }
        public decimal weight { get; set; }
        public int value { get; set; }
        public string method { get; set; }
        public int sender_code { get; set; }
        public string receiver_code { get; set; }
        public int cod { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int length { get; set; }
        public int payment_terms { get; set; }
        public int inv_value { get; set; }
        //public string randomstring
        //{
        //    get
        //    {
        //        Random rand = new Random();
        //        const string pool = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        //        var chars = Enumerable.Range(0, 20)
        //            .Select(x => pool[rand.Next(0, pool.Length)]);
        //        return new string(chars.ToArray());
        //    }
        //}
    }
}

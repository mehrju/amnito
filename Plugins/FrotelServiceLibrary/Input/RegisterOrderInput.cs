using FrotelServiceLibrary.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrotelServiceLibrary.Input
{
    public class RegisterOrderInput : BaseInput
    {
        /// <summary>
        /// نام خریدار
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } 

        /// <summary>
        /// نام خانوادگی خریدار
        /// </summary>
        [JsonProperty("family")]
        public string Family { get; set; } 	

        /// <summary>
        /// تلفن خریدار
        /// </summary>
        [JsonProperty("phone")]
        public string Phone { get; set; }

        /// <summary>
        /// موبایل خریدار
        /// </summary>
        [JsonProperty("mobile")]
        public string Mobile { get; set; } 

        /// <summary>
        /// جنسیت خریدار
        /// </summary>
        [JsonProperty("gender")]
        public Gender Gender { get; set; }

        /// <summary>
        /// ایمیل خریدار
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; } 	

        /// <summary>
        /// آدرس خریدار ( آدرس محل دریافت سفارش )	
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }  

        /// <summary>
        /// کد پستی
        /// </summary>
        [JsonProperty("code_posti")]
        public string PostalCode { get; set; }  	

        /// <summary>
        /// شناسه استان مقصد
        /// </summary>
        [JsonProperty("province")]
        public int DestinationProvinceId { get; set; }	

        /// <summary>
        /// شناسه شهر مقصد
        /// </summary>
        [JsonProperty("city")]
        public int DestinationCityId { get; set; } 	

        /// <summary>
        /// روش پرداخت
        /// </summary>
        [JsonProperty("buy_type")]
        public BuyType BuyType { get; set; }

        /// <summary>
        /// روش ارسال
        /// </summary>
        [JsonProperty("send_type")]
        public SendType SendType { get; set; }

        /// <summary>
        /// آي پي خريدار در لحظه ثبت سفارش	
        /// اختیاری
        /// </summary>
        [JsonProperty("ip")]
        public string IP { get; set; }

        /// <summary>
        /// پیام خریدار		
        /// اختیاری
        /// </summary>
        [JsonProperty("pm")]
        public string Pm { get; set; }

        /// <summary>
        /// ارسال رایگان		
        /// اختیاری
        /// </summary>
        [JsonProperty("free_send")]
        public FreeSend FreeSend { get; set; }


        /// <summary>
        /// اطلاعات محصولات داخل سبد خرید	
        /// </summary>
        [JsonProperty("basket")]
        public List<RegisterOrderBasket> BasketItems { get; set; }

    }

    public class RegisterOrderBasket
    {
        [JsonProperty("pro_code")]
        public string ProductCode { get; set; }

        [JsonProperty("name")]
        public string ProductName { get; set; }

        [JsonProperty("price")]
        public int ProductPrice { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }

        /// <summary>
        /// پورسانت محصول به درصد	
        /// اختیاری
        /// </summary>
        [JsonProperty("porsant")]
        public float Porsant { get; set; }

        /// <summary>
        /// شناسه بازاریاب	
        /// اختیاری
        /// </summary>
        [JsonProperty("bazaryab")]
        public int Bazaryab { get; set; }

        /// <summary>
        /// میزان تخفیف به درصد	
        /// اختیاری
        /// </summary>
        [JsonProperty("discount")]
        public float Discount { get; set; }

        /// <summary>
        /// ارسال رایگان		
        /// اختیاری
        /// </summary>
        [JsonProperty("free_send")]
        public FreeSend FreeSend { get; set; }

        /// <summary>
        /// مالیات کالا به درصد			
        /// اختیاری
        /// </summary>
        [JsonProperty("tax")]
        public int Tax { get; set; }

    }
}

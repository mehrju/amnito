using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Persain
{
    /// <summary>
    /// مدل ورودی متد ثبت کالا
    /// <para>fullname": "Requirement/",</para>
    /// <para>mobile": "Requirement/",</para>
    /// <para>address": "Requirement/",</para>
    /// <para>vcustom_barcode": "Optional//Default:NULL",</para>
    /// <para>tel": "Optional/Number/Default:NULL",</para>
    /// <para>product_name": "Optional/String/Default:NULL",</para>
    /// <para>send_time_range": "Optional/String/Default:NULL",</para>
    /// <para>description": "Optional/String/Default:NULL",</para>
    /// <para>price": "Optional/Number/Default:0",</para>
    /// <para>username":"Requirement/String", از طرف تنظیمات تکمیل میگردد</para>
    /// <para>password":"Requirement/String" از طرف تنظیمات تکمیل میگردد</para>
    /// 
    /// </summary>
    public class Params_Persian_NewCustomer
    {
        public String fullname { get; set; }
        public int mobile { get; set; }
        public String address { get; set; }
        public String vcustom_barcode { get; set; }
        public int tel { get; set; }
        public String product_name { get; set; }
        public String send_time_range { get; set; }
        public String description { get; set; }
        public int price { get; set; }
        public String username { get; set; }
        public String password { get; set; }

        public (bool,string) IsValidParamsNewCustomer()
        {
            bool Result = true;
            string Message = "";



            return (Result, Message);
        }
    }
}

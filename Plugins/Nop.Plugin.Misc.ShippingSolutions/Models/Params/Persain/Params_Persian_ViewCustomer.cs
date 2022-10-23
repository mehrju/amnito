using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Persain
{/// <summary>
 /// مدل ورودی 
 /// <para>id": "Optional/Number/Requirement",</para>
 /// <para>username":"Requirement/String", از طرف تنظیمات تکمیل میگردد</para>
 /// <para>password":"Requirement/String" از طرف تنظیمات تکمیل میگردد</para>
 /// </summary>
    public class Params_Persian_ViewCustomer
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}

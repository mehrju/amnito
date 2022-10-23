using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Tinex
{
    /// <summary>
    /// مدل ورودی تابع دریافت توکن
    /// </summary>
   public class Params_Tinex_Token
    {
        public string Tinex_grant_type { get; set; }
        public string Tinex_client_id { get; set; }
        public string Tinex_client_secret { get; set; }
        public string Tinex_scope { get; set; }
    }
}

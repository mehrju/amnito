using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Taroff
{
    /// <summary>
    /// مدل خروجی متد لیست استانها
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// <para>Category_GetCity لیست</para>
    /// </summary>
    public class Result_Taroff_GetProvinces
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public List<Category> categories { get; set; }
    }

    public class DetailResult_GetProvinces
    {
        public string state { get; set; }
        public List<Category> categories { get; set; }
    }
    public class Category
    {
        public int id { get; set; }
        public string title { get; set; }
    }
}

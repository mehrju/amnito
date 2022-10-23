using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.PDE
{
    ///<summary> 
    ///<para>خروجی توابع در کلاس ActionResult</para>
    ///<para>در خروجی توابع قسمت عمومی وجود دارد که نشان میدهد که وضعیت خروجی به چه گونه هست</para>
    ///<para> و یک قسمت خاص که برای هر تابع یک خروجی منحصربهفرد با توجه به داکیومنت طراحی شده است</para>
    ///<para>Status اگر true باشد تابع به درستی انجام شده است در غیر این صورت تابع به هر دلیل خطا داده است</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///</summary>
    ///
    public class Result_PDE_CountriesGET
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
       public List<ItemsCountries> ItemsCountries { get; set; }
    }
    public class ItemsCountries
    {
        public int Id { get; set; }
        public string FaTitle { get; set; }
        public string Title { get; set; }
    }
}

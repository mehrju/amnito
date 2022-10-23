using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Search
{
    public class Search_Dealer_Model
    {
        public bool Search_Dealer_ActiveSearch { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Search_Dealer_Name")]
        public string Search_Dealer_Name { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Search_Dealer_IsActive")]
        public bool Search_Dealer_IsActive { get; set; }
    }
}

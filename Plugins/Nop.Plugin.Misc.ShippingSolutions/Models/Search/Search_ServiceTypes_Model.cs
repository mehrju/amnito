using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Search
{
    public class Search_ServiceTypes_Model
    {

        public bool Search_ServiceTypes_ActiveSearch { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Search_ServiceTypes_Name")]
        public string Search_ServiceTypes_Name { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Search_ServiceTypes_IsActive")]
        public bool Search_ServiceTypes_IsActive { get; set; }
    }
}

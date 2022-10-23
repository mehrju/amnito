using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Grid
{
    public class Grid_Provider_In_Collector
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_Name")]
        public string Grid_Provider_Name { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsActive")]
        public bool Grid_Provider_IsActive { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_DateInsert")]
        public DateTime Grid_Provider_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_DateUpdate")]
        public DateTime? Grid_Provider_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_UserInsert")]
        public string Grid_Provider_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_UserUpdate")]
        public string Grid_Provider_UserUpdate { get; set; }
    }
}

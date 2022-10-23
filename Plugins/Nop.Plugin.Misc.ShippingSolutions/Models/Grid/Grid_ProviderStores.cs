using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Grid
{
    public class Grid_ProviderStores
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_ProviderStore_Name")]
        public string Grid_ProviderStore_Name { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_ProviderStore_IsActive")]
        public bool Grid_ProviderStore_IsActive { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_ProviderStore_DateInsert")]
        public DateTime Grid_ProviderStore_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_ProviderStore_DateUpdate")]
        public DateTime? Grid_ProviderStore_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_ProviderStore_UserInsert")]
        public string Grid_ProviderStore_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_ProviderStore_UserUpdate")]
        public string Grid_ProviderStore_UserUpdate { get; set; }
    }
}

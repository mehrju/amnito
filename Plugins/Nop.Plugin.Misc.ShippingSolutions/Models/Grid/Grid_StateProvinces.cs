using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Grid
{
    public class Grid_StateProvinces
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_NameCountry")]
        public string Grid_StateProvinces_NameCountry { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_NameProvinces")]
        public string Grid_StateProvinces_NameProvinces { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_IdStateMaping")]
        public int Grid_StateProvinces_IdStateMaping { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_IdCityMaping")]
        public int Grid_StateProvinces_IdCityMaping { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_NameStateMaping")]
        public string Grid_StateProvinces_NameStateMaping { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_NameCityMaping")]
        public string Grid_StateProvinces_NameCityMaping { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_IsActive")]
        public bool Grid_StateProvinces_IsActive { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_DateInsert")]
        public DateTime Grid_StateProvinces_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_DateUpdate")]
        public DateTime? Grid_StateProvinces_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_UserInsert")]
        public string Grid_StateProvinces_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_StateProvinces_UserUpdate")]
        public string Grid_StateProvinces_UserUpdate { get; set; }
    }
}

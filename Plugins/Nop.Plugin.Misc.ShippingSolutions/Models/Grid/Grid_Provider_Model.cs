using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Grid
{
    public class Grid_Provider_Model
    {

        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_Name")]
        public string Grid_Provider_Name { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_CategoryName")]
        public string Grid_Provider_CategoryName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_ServiceTypeName")]
        public string Grid_Provider_ServiceTypeName { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_AgentName")]
        public string Grid_Provider_AgentName { get; set; }




        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsActive")]
        public bool Grid_Provider_IsActive { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_MaxOrder")]
        public string Grid_Provider_MaxOrder { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_MaxWeight")]
        public string Grid_Provider_MaxWeight { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_MinWeight")]
        public string Grid_Provider_MinWeight { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_MaxTimeDeliver")]
        public string Grid_Provider_MaxTimeDeliver { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_advancefreight")]
        public bool Grid_Provider_advancefreight { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_freightforward")]
        public bool Grid_Provider_freightforward { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_cod")]
        public bool Grid_Provider_cod { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_DateInsert")]
        public DateTime Grid_Provider_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_DateUpdate")]
        public DateTime? Grid_Provider_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_UserInsert")]
        public string Grid_Provider_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_UserUpdate")]
        public string Grid_Provider_UserUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsPishtaz")]
        public bool Grid_Provider_IsPishtaz { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsSefareshi")]
        public bool Grid_Provider_IsSefareshi { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsVIje")]
        public bool Grid_Provider_IsVIje { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsNromal")]
        public bool Grid_Provider_IsNromal { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsDroonOstani")]
        public bool Grid_Provider_IsDroonOstani { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsAdjoining")]
        public bool Grid_Provider_IsAdjoining { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsNotAdjacent")]
        public bool Grid_Provider_IsNotAdjacent { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsHeavyTransport")]
        public bool Grid_Provider_IsHeavyTransport { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsForeign")]
        public bool Grid_Provider_IsForeign { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsInCity")]
        public bool Grid_Provider_IsInCity { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsAmanat")]
        public bool Grid_Provider_IsAmanat { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_IsTwoStep")]
        public bool Grid_Provider_IsTwoStep { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_HasHagheMaghar")]
        public bool Grid_Provider_HasHagheMaghar { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_Maxlength")]
        public int Grid_Provider_Maxlength { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_Maxwidth")]
        public int Grid_Provider_Maxwidth { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_Maxheight")]
        public int Grid_Provider_Maxheight { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Provider_MaxbillingamountCOD")]
        public double Grid_Provider_MaxbillingamountCOD { get; set; }

    }
}

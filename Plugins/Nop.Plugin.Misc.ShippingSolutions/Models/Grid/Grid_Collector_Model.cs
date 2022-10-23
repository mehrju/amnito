using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Grid
{
    public class Grid_Collector_Model
    {

        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_Name")]
        public string Grid_Collector_Name { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_UserName")]
        public string Grid_Collector_UserName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsActive")]
        public bool Grid_Collector_IsActive { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_MaxPath")]
        public string Grid_Collector_MaxPath { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_MaxWeight")]
        public string Grid_Collector_MaxWeight { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_MinWeight")]
        public string Grid_Collector_MinWeight { get; set; }

      
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_advancefreight")]
        //public bool Grid_Collector_advancefreight { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_freightforward")]
        //public bool Grid_Collector_freightforward { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_cod")]
        //public bool Grid_Collector_cod { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_DateInsert")]
        public DateTime Grid_Collector_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_DateUpdate")]
        public DateTime? Grid_Collector_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_UserInsert")]
        public string Grid_Collector_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_UserUpdate")]
        public string Grid_Collector_UserUpdate { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsPishtaz")]
        //public bool Grid_Collector_IsPishtaz { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsSefareshi")]
        //public bool Grid_Collector_IsSefareshi { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsVIje")]
        //public bool Grid_Collector_IsVIje { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsNromal")]
        //public bool Grid_Collector_IsNromal { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsDroonOstani")]
        //public bool Grid_Collector_IsDroonOstani { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsAdjoining")]
        //public bool Grid_Collector_IsAdjoining { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsNotAdjacent")]
        //public bool Grid_Collector_IsNotAdjacent { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsHeavyTransport")]
        //public bool Grid_Collector_IsHeavyTransport { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsForeign")]
        //public bool Grid_Collector_IsForeign { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsInCity")]
        //public bool Grid_Collector_IsInCity { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsAmanat")]
        //public bool Grid_Collector_IsAmanat { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_IsTwoStep")]
        //public bool Grid_Collector_IsTwoStep { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Grid_Collector_HasHagheMaghar")]
        //public bool Grid_Collector_HasHagheMaghar { get; set; }
    }
}

using Nop.Core;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.ShippingSolutions.Domain
{
    public class Tbl_ServicesTypes : BaseEntity
    {
        //[ScaffoldColumn(false)]
        //public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewServiceType")]
        [DataType(DataType.Text)]
        public String Name { get; set; }

        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? DateUpdate { get; set; }

        [ScaffoldColumn(false)]
        public int IdUserInsert { get; set; }

        [ScaffoldColumn(false)]
        public int? IdUserUpdate { get; set; }

        public List<Tbl_ServiceTypesProvider> ServiceTypesProvider { get; set; }
    }
}

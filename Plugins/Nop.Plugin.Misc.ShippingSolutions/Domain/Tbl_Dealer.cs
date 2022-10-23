using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Domain
{
    public class Tbl_Dealer : BaseEntity
    {
        public Tbl_Dealer()
        {
            ListProvider = new List<SelectListItem>();
            ListStateApplyPricingPolicy = new List<SelectListItem>();
        }

        //public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.DealerName")]
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

        [NotMapped]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.DealerProviderId")]
        public IList<int> ProviderId { get; set; }
        [NotMapped]
        public IList<SelectListItem> ListProvider { get; set; }

        [ScaffoldColumn(false)]
        public int StateApplyPricingPolicy { get; set; }
        [NotMapped]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.ListStateApplyPricingPolicy")]
        public IList<SelectListItem> ListStateApplyPricingPolicy { get; set; }
    }
}

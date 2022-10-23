using Nop.Core;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class Tbl_ServiceProviderDashboard : BaseEntity
    {
        [NopResourceDisplayName("Nop.Plugin.Misc.PostbarDashboard.TitleServiceProvider")]
        public string TitleServiceProviderDashboard { get; set; }

        [ScaffoldColumn(false)]
        public String UrlImage { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PostbarDashboard.UrlPageDiscreption")]
        public String UrlPageDiscreption { get; set; }

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
    }
}

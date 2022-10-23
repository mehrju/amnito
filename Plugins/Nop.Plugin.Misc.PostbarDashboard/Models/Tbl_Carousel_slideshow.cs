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

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class Tbl_Carousel_slideshow : BaseEntity
    {
        public Tbl_Carousel_slideshow()
        {
            
            ListStores = new List<SelectListItem>();
        }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.PostbarDashboard.Title_Carousel_slideshow")]
        public string Title_Carousel_slideshow { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.PostbarDashboard.Discrition_Carousel_slideshow")]
        public string Discrition_Carousel_slideshow { get; set; }


        [ScaffoldColumn(false)]
        public String UrlImage { get; set; }

        [ScaffoldColumn(false)]
        public String UrlImageMobile { get; set; }



        [NopResourceDisplayName("Nop.Plugin.Misc.PostbarDashboards.UrlPageDiscreption")]
        public String UrlPage { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PostbarDashboards.IsVideo")]
        public bool IsVideo { get; set; }



        [NopResourceDisplayName("Nop.Plugin.Misc.PostbarDashboards.TimeInterval")]
        public int TimeInterval { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PostbarDashboards.DateStart")]
        public DateTime DateStart { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.PostbarDashboards.DateExpire")]
        public DateTime? DateExpire { get; set; }


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
        
        public string LimitedStore { get; set; }

        [NotMapped]
        [NopResourceDisplayName("Nop.Plugin.Misc.PostbarDashboards.StoreId")]
        public IList<int> StoreId { get; set; }
        [NotMapped]
        public IList<SelectListItem> ListStores { get; set; }


    }
}

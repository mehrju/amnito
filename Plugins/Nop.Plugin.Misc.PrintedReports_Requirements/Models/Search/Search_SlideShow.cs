using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Models.Search
{
    public class Search_SlideShow
    {
        public Search_SlideShow()
        {
            ListStores = new List<SelectListItem>();
        }

        public bool Search_SlideShow_ActiveSearch { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Search_SlideShow_Title")]
        public string Search_SlideShow_Title { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Search_SlideShow_IsActive")]
        public bool Search_SlideShow_IsActive { get; set; }


        
        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Search_SlideShow_IsVideo")]
        public bool Search_SlideShow_IsVideo { get; set; }


        
        [NopResourceDisplayName("Nop.Plugin.Misc.PostbarDashboards.StoreId")]
        public int Search_StoreId { get; set; }
        public IList<SelectListItem> ListStores { get; set; }
    }
}

using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Models.Grid
{
    public class Grid_SlideShow
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_UrlImage")]
        public string Grid_SlideShow_UrlImage { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_Title")]
        public string Grid_SlideShow_Title { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_Dis")]
        public string Grid_SlideShow_Dis { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_UrlPage")]
        public string Grid_SlideShow_UrlPage { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_IsActive")]
        public bool Grid_SlideShow_IsActive { get; set; }

        
        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_IsVideo")]
        public bool Grid_SlideShow_IsVideo { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_DateInsert")]
        public DateTime Grid_SlideShow_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_DateUpdate")]
        public DateTime? Grid_SlideShow_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_UserInsert")]
        public string Grid_SlideShow_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_UserUpdate")]
        public string Grid_SlideShow_UserUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_NameStore")]
        public string Grid_SlideShow_NameStore { get; set; }
    }
}

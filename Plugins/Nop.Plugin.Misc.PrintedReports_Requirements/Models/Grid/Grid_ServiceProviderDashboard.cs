using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Models.Grid
{
    public class Grid_ServiceProviderDashboard
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_ServiceProviderDashboard_UrlImage")]
        public string Grid_ServiceProviderDashboard_UrlImage { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_ServiceProviderDashboard_Title")]
        public string Grid_ServiceProviderDashboard_Title { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_ServiceProviderDashboard_UrlPage")]
        public string Grid_ServiceProviderDashboard_UrlPage { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_ServiceProviderDashboard_IsActive")]
        public bool Grid_ServiceProviderDashboard_IsActive { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_ServiceProviderDashboard_DateInsert")]
        public DateTime Grid_ServiceProviderDashboard_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_ServiceProviderDashboard_DateUpdate")]
        public DateTime? Grid_ServiceProviderDashboard_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_ServiceProviderDashboard_UserInsert")]
        public string Grid_ServiceProviderDashboard_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_ServiceProviderDashboard_UserUpdate")]
        public string Grid_ServiceProviderDashboard_UserUpdate { get; set; }

    }
}

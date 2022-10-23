using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Models.Search
{
    public class Search_ServiceProviderDashboard
    {
        public Search_ServiceProviderDashboard()
        {
            
        }

        public bool Search_ServiceProviderDashboard_ActiveSearch { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Search_ServiceProviderDashboard_Title")]
        public string Search_ServiceProviderDashboard_Title { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.PrintedReports_Requirements.Search_ServiceProviderDashboard_IsActive")]
        public bool Search_ServiceProviderDashboard_IsActive { get; set; }

       
    }
}

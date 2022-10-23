
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public class CategoryIconsModel : BaseNopModel
    {
        public CategoryIconsModel()
        {
            AvailableSubCategories = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Willusedefaultnopcategory")]
        public bool Willusedefaultnopcategory { get; set; }
        public bool Willusedefaultnopcategory_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.SubCategory")]
        public int SubCategoryId { get; set; }
        public bool SubCategoryId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.CategoryIcon")]
        //[UIHint("Picture")]
        public string Extension { get; set; }
        public bool Extension_OverrideForStore { get; set; }

        public List<SelectListItem> AvailableSubCategories { get; set; }

    }
}
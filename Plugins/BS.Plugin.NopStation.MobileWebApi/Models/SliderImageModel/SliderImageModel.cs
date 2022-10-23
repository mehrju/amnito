using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace BS.Plugin.NopStation.MobileWebApi.Models.SliderImageModel
{
    public class SliderImageModel : BaseNopEntityModel
    {
        public SliderImageModel()
        {
            ImageFor = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.HomePageProduct.Fields.Add")]
        public int Add { get; set; }

        [NopResourceDisplayName("Admin.Catalog.HomePageProduct.Fields.Delete")]
        public int Delete { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.PictureUrl")]
        public string PictureUrl { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.OverrideAltAttribute")]
      
        public string OverrideAltAttribute { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.OverrideTitleAttribute")]
    
        public string OverrideTitleAttribute { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.ShortDescription")]
      
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Slider.Products.Fields.StartDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? SliderActiveStartDate { get; set; }


        [NopResourceDisplayName("Slider.Products.Fields.EndDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? SliderActiveEndDate { get; set; }

        [NopResourceDisplayName("Slider.Products.Fields.CampaignType")]
        public int CampaignType { get; set; }

        public string CampaignTypeText { get; set; }

        [NopResourceDisplayName("Slider.Products.Fields.ImageFor")]
        public IList<SelectListItem> ImageFor { get; set; }

        public string ImageIdFor { get; set; }

        /// <summary>
        /// Under properties for kendogrid dropdown
        /// </summary>
        public string ImageIdDropDownFor { get; set; }
        public string ImageTextDropDownFor { get; set; }

        public DateTime? SliderActiveStartDateInGrid { get; set; }

        public DateTime? SliderActiveEndDateInGrid { get; set; }

        public string SliderActiveStartDateInGridString { get; set; }

        public string SliderActiveEndDateInGridString { get; set; }

        [NopResourceDisplayName("Slider.Products.Fields.IsProduct")]
        public bool IsProduct { get; set; }
        [NopResourceDisplayName("Slider.Products.Fields.ProductOrCategory")]
        public int ProductOrCategory { get; set; }
    }
}

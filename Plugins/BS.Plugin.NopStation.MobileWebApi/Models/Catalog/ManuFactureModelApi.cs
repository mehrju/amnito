using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Models.Media;
using Nop.Core.Domain.Catalog;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class ManuFactureModelApi :CategoryBaseModelApi
    {
        public ManuFactureModelApi()
        {
            PictureModel = new PictureModel();
            FeaturedProducts = new List<ProductOverViewModelApi>();
            Products = new List<ProductOverViewModelApi>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
            
        }

        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public IList<SelectListItem> AvailableSortOptions { get; set; }
        public PictureModel PictureModel { get; set; }

        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<ProductOverViewModelApi> FeaturedProducts { get; set; }
        public IList<ProductOverViewModelApi> Products { get; set; }

        public PriceRange PriceRange { get; set; }
    }
}

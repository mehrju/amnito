using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Models.Media;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog
{
    public class VendorDetailProductResponseModel : BaseResponse
    {
        public VendorDetailProductResponseModel()
        {
            LogoModel = new PictureModel();
            PictureModel = new PictureModel();
            Products = new List<ProductOverViewModelApi>();
            NotFilteredItems = new List<CatalogPagingFilteringModel.SpecificationFilterItem>();
            AlreadyFilteredItems = new List<CatalogPagingFilteringModel.SpecificationFilterItem>();
            AvailableSortOptions = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public bool AllowCustomersToContactVendors { get; set; }

        public PictureModel PictureModel { get; set; }

        public IList<ProductOverViewModelApi> Products { get; set; }
        public IList<CatalogPagingFilteringModel.SpecificationFilterItem> NotFilteredItems { get; set; }
        public IList<CatalogPagingFilteringModel.SpecificationFilterItem> AlreadyFilteredItems { get; set; }
        public int TotalPages { get; set; }
        public IList<SelectListItem> AvailableSortOptions { get; set; }

        public PictureModel LogoModel { get; set; }
    }
}

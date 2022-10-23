using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Domain;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public partial interface IProductServiceApi
    {
        List<CheckoutAttribute> getAttributeLit(int productId, string attributeName);
        int? GetPostType(int categoryId);
        List<PostService> ListOfService();
        PriceRange SearchProductsPrice(
           int pageIndex = 0,
           int pageSize = int.MaxValue,
           IList<int> categoryIds = null,
           int manufacturerId = 0,
           int storeId = 0,
           int vendorId = 0,
           int warehouseId = 0,
           ProductType? productType = null,
           bool visibleIndividuallyOnly = false,
           bool? featuredProducts = null,
           decimal? priceMin = null,
           decimal? priceMax = null,
           int productTagId = 0,
           string keywords = null,
           bool searchDescriptions = false,
           bool searchSku = true,
           bool searchProductTags = false,
           int languageId = 0,
           IList<int> filteredSpecs = null,
           ProductSortingEnum orderBy = ProductSortingEnum.Position,
           bool showHidden = false,
           bool? overridePublished = null);
        IList<Product> GetProductsOnlyOfParentCategory(int categoryId, int itemsNumber);
        IList<Product> GetProductsByCategoryId(List<int> categoryIds, int itemsNumber);

        IList<int> GetPreviousAndNextProducts(int categoryId, int productId);

        IList<BS_FeaturedProducts> GetFeaturedProducts();

        IPagedList<Product> SearchOnSaleProducts(
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
            IList<int> categoryIds = null,
            int manufacturerId = 0,
            int storeId = 0,
            int vendorId = 0,
            int warehouseId = 0,
            ProductType? productType = null,
            bool visibleIndividuallyOnly = false,
            bool markedAsNewOnly = false,
            bool? featuredProducts = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int productTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchSku = true,
            bool searchProductTags = false,
            int languageId = 0,
            IList<int> filteredSpecs = null,
            ProductSortingEnum orderBy = ProductSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null,
            bool specialPrice = true,
            bool hasTierPrice = true,
            bool hasDiscountApplied = true);

        IList<Product> GetTopDealsProductsByIds(int[] productIds);
    }

    
}

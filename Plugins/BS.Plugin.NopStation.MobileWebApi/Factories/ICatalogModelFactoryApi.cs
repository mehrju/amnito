using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Web.Models.Catalog;

namespace BS.Plugin.NopStation.MobileWebApi.Factories
{
    public partial interface ICatalogModelFactoryApi
    {
        #region Common

        /// <summary>
        /// Prepare sorting options
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        void PrepareSortingOptions(Models.Catalog.CatalogPagingFilteringModel pagingFilteringModel, int orderBy);

        /// <summary>
        /// Prepare view modes
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        void PrepareViewModes(Models.Catalog.CatalogPagingFilteringModel pagingFilteringModel, Models.Catalog.CatalogPagingFilteringModel command);

        /// <summary>
        /// Prepare page size options
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <param name="allowCustomersToSelectPageSize">Are customers allowed to select page size?</param>
        /// <param name="pageSizeOptions">Page size options</param>
        /// <param name="fixedPageSize">Fixed page size</param>
        void PreparePageSizeOptions(Models.Catalog.CatalogPagingFilteringModel pagingFilteringModel, Models.Catalog.CatalogPagingFilteringModel command,
            bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize);

        #endregion

        #region Categories

        /// <summary>
        /// Prepare CategoryNavigation model
        /// </summary>
        /// <returns>List of CategoryNavigation model</returns>
        IList<CategoryNavigationModelApi> PrepareCategoriesModel();

        /// <summary>
        /// Prepare Category model
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Category model</returns>
        CategoryModelApi PrepareCategoryModel(Category category, int pageNumber, int orderBy);

        /// <summary>
        /// Prepare category template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Category template view path</returns>
        string PrepareCategoryTemplateViewPath(int templateId);

        /// <summary>
        /// Prepare category navigation model
        /// </summary>
        /// <param name="currentCategoryId">Current category identifier</param>
        /// <param name="currentProductId">Current product identifier</param>
        /// <returns>Category navigation model</returns>
        //CategoryNavigationModelApi PrepareCategoryNavigationModel(int currentCategoryId,
        //    int currentProductId);

        /// <summary>
        /// Prepare top menu model
        /// </summary>
        /// <returns>Top menu model</returns>
        //TopMenuModel PrepareTopMenuModel();

        /// <summary>
        /// Prepare homepage category models
        /// </summary>
        /// <returns>List of homepage category models</returns>
        List<CategoryOverViewModelApi> PrepareHomepageCategoryModels(int? thumbPictureSize);

        /// <summary>
        /// Prepare homepage category models with product
        /// </summary>
        /// <returns>List of homepage category models with product</returns>
        List<CatalogFeaturedCategoryWithProduct> PrepareHomepageCategoryModelsWithProduct(int? thumbPictureSize);

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <returns>List of category (simple) models</returns>
        List<CategorySimpleModel> PrepareCategorySimpleModels();

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <param name="allCategories">All available categories; pass null to load them internally</param>
        /// <returns>List of category (simple) models</returns>
        List<CategorySimpleModel> PrepareCategorySimpleModels(int rootCategoryId,
            bool loadSubCategories = true, IList<Category> allCategories = null);

        CategoryModelApi CategoryFeaturedProductAndSubCategory(Category category);

        CategoryDetailProductResponseModel PrepareCategoryDetailProductResponseModel(CategoryModelApi model);

        CategoryDetailFeaturedProductAndSubcategoryResponseModel
            PrepareCategoryDetailFeaturedProductAndSubcategoryResponseModel(CategoryModelApi categoryModelApi);

        #endregion

        #region Manufacturers

        /// <summary>
        /// Prepare manufacturer model
        /// </summary>
        /// <param name="manufacturer">Manufacturer identifier</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Manufacturer model</returns>
        ManuFactureModelApi PrepareManufacturerModel(Manufacturer manufacturer, int pageNumber, int orderBy);

        /// <summary>
        /// Prepare manufacturer template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Manufacturer template view path</returns>
        string PrepareManufacturerTemplateViewPath(int templateId);

        /// <summary>
        /// Prepare manufacturer all models
        /// </summary>
        /// <returns>List of manufacturer models</returns>
        List<MenufactureOverViewModelApi> PrepareManufacturerAllModels(int? thumbPictureSize);

        ManuFactureModelApi PrepareManufacturerFeaturedProduct(Manufacturer manufacturer);

        SearchModelApi PrepareSearchModelApi(SearchModelApi model, int pageNumber);

        TagModelApi PrepareTagModelApi(ProductTag productTag, int pageNumber, int orderBy);

        ManufactureDetailProductResponseModel PrepareManufactureDetailProductResponseModel(ManuFactureModelApi model);

        SearchProductResponseModel PrepareSearchProductResponseModel(SearchModelApi model);

        ProductTagDetailResponseModel PrepareProductTagDetailResponseModel(TagModelApi model);

        ManufacturerDetailFeaturedProductResponseModel PrepareManufacturerDetailFeaturedProductResponseModel(
            ManuFactureModelApi manuFactureModelApi);

        /// <summary>
        /// Prepare manufacturer navigation model
        /// </summary>
        /// <param name="currentManufacturerId">Current manufacturer identifier</param>
        /// <returns>Manufacturer navigation model</returns>
        //ManufacturerNavigationModel PrepareManufacturerNavigationModel(int currentManufacturerId);

        #endregion

        #region Vendors

        /// <summary>
        /// Prepare vendor model
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Vendor model</returns>
        VendorDetailProductResponseModel PrepareVendorModel(Vendor vendor);

        /// <summary>
        /// Prepare vendor all models
        /// </summary>
        /// <returns>List of vendor models</returns>
        List<Models.Vendor.VendorModel> PrepareVendorAllModels();

        /// <summary>
        /// Prepare vendor navigation model
        /// </summary>
        /// <returns>Vendor navigation model</returns>
        Models.Vendor.VendorNavigationModel PrepareVendorNavigationModel();

        #endregion

        #region CategoriesAndManufacturers
        CategoriesAndManufacturersModelApi PrepareCategoriesAndManufacturersModel();
        #endregion
    }
}

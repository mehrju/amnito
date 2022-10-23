using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Framework.Events;
using Nop.Core.Domain.Localization;
using System.Globalization;
using BS.Plugin.NopStation.MobileWebApi.Factories;
using Nop.Web.Models.Media;
using BS.Plugin.NopStation.MobileWebApi.Models.Vendor;
using BS.Plugin.NopStation.MobileWebApi.Models.Product;
using Nop.Core.Domain.Tax;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    //[Route("api/[controller]")]
    public class CatalogController : BaseApiController
    {
        //#region Fields

        //private int pageSize = 6;

        //private readonly ICatalogModelFactoryApi _catalogModelFactoryApi;
        //private readonly ICommonModelFactoryApi _commonModelFactoryApi;
        //private readonly ICategoryService _categoryService;
        //private readonly IWorkContext _workContext;
        //private readonly IStoreContext _storeContext;
        //private readonly IWebHelper _webHelper;
        //private readonly ICacheManager _cacheManager;
        //private readonly IPictureService _pictureService;
        //private readonly MediaSettings _mediaSettings;
        //private readonly IManufacturerService _manufacturerService;
        //private readonly IPermissionService _permissionService;
        //private readonly IAclService _aclService;
        //private readonly IStoreMappingService _storeMappingService;
        //private readonly ICurrencyService _currencyService;
        //private readonly CatalogSettings _catalogSettings;
        //private readonly IPriceFormatter _priceFormatter;
        //private readonly IProductService _productService;
        //private readonly ISpecificationAttributeService _specificationAttributeService;
        //private readonly ICustomerActivityService _customerActivityService;
        //private readonly ILocalizationService _localizationService;
        //private readonly IPriceCalculationService _priceCalculationService;
        //private readonly ITaxService _taxService;
        //private readonly IProductServiceApi _productServiceApi;
        //private readonly IGenericAttributeService _genericAttributeService;
        //private readonly ISearchTermService _searchTermService;
        //private readonly IEventPublisher _eventPublisher;
        //private readonly IProductTagService _productTagService;
        //private readonly IVendorService _vendorService;
        //private readonly VendorSettings _vendorSettings;
        //private readonly ICategoryIconService _categoryIconService;
        //private readonly LocalizationSettings _localizationSettings;
        //private readonly ILanguageService _languageService;
        //private readonly ICustomerServiceApi _customerServiceApi;
        //private readonly IOrderReportService _orderReportService;
        //private readonly TaxSettings _taxSettings;
        //private readonly ShoppingCartSettings _shoppingCartSettings;
        //#endregion

        //#region Ctor
        //public CatalogController(ICatalogModelFactoryApi catalogModelFactoryApi,
        //    ICommonModelFactoryApi commonModelFactoryApi,
        //    ICategoryService categoryService,
        //    IWorkContext workContext,
        //    IStoreContext storeContext,
        //    IWebHelper webHelper,
        //    ICacheManager cacheManager,
        //    IPictureService pictureService,
        //    MediaSettings mediaSettings,
        //    IManufacturerService manufacturerService,
        //    IPermissionService permissionService,
        //    IAclService aclService,
        //    IStoreMappingService storeMappingService,
        //    ICurrencyService currencyService,
        //    CatalogSettings catalogSettings,
        //    IPriceFormatter priceFormatter,
        //    IProductService productService,
        //    ISpecificationAttributeService specificationAttributeService,
        //    ICustomerActivityService customerActivityService,
        //    ILocalizationService localizationService,
        //    IPriceCalculationService priceCalculationService,
        //    ITaxService taxService,
        //    IProductServiceApi productServiceApi,
        //    IGenericAttributeService genericAttributeService,
        //    ISearchTermService searchTermService,
        //    IEventPublisher eventPublisher,
        //    IProductTagService productTagService,
        //    IVendorService vendorService,
        //    VendorSettings vendorSettings,
        //    ICategoryIconService categoryIconService,
        //    LocalizationSettings localizationSettings,
        //    ILanguageService languageService,
        //    ICustomerServiceApi customerServiceApi,
        //    IOrderReportService orderReportService,
        //    TaxSettings taxSettings,
        //    ShoppingCartSettings shoppingCartSettings
        //    )
        //{
        //    this._catalogModelFactoryApi = catalogModelFactoryApi;
        //    this._commonModelFactoryApi = commonModelFactoryApi;
        //    this._categoryService = categoryService;
        //    this._workContext = workContext;
        //    this._storeContext = storeContext;
        //    this._webHelper = webHelper;
        //    this._cacheManager = cacheManager;
        //    this._pictureService = pictureService;
        //    this._mediaSettings = mediaSettings;
        //    this._manufacturerService = manufacturerService;
        //    this._permissionService = permissionService;
        //    this._aclService = aclService;
        //    this._storeMappingService = storeMappingService;
        //    this._currencyService = currencyService;
        //    this._catalogSettings = catalogSettings;
        //    this._priceFormatter = priceFormatter;
        //    this._productService = productService;
        //    this._specificationAttributeService = specificationAttributeService;
        //    this._customerActivityService = customerActivityService;
        //    this._localizationService = localizationService;
        //    this._priceCalculationService = priceCalculationService;
        //    this._taxService = taxService;
        //    this._productServiceApi = productServiceApi;
        //    this._genericAttributeService = genericAttributeService;
        //    this._searchTermService = searchTermService;
        //    this._eventPublisher = eventPublisher;
        //    this._productTagService = productTagService;
        //    this._vendorService = vendorService;
        //    this._vendorSettings = vendorSettings;
        //    this._categoryIconService = categoryIconService;
        //    this._localizationSettings = localizationSettings;
        //    this._languageService = languageService;
        //    this._customerServiceApi = customerServiceApi;
        //    this._orderReportService = orderReportService;
        //    this._taxSettings = taxSettings;
        //    this._shoppingCartSettings = shoppingCartSettings;
        //}
        //#endregion

        //#region Utility
        ////[System.Web.Http.NonAction]
        ////protected virtual IEnumerable<ProductOverViewModelApi> PrepareProductOverviewModels(IEnumerable<Product> products,
        ////    bool preparePriceModel = true, bool preparePictureModel = true,
        ////    int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
        ////    bool forceRedirectionAfterAddingToCart = false)
        ////{
        ////    return products.PrepareProductOverviewModels(_workContext,
        ////        _storeContext, _categoryService, _productService,
        ////        _priceCalculationService, _priceFormatter, _permissionService,
        ////        _localizationService, _taxService, _currencyService,
        ////        _pictureService, _webHelper, _cacheManager,
        ////        _catalogSettings, _mediaSettings,
        ////        preparePriceModel, preparePictureModel,
        ////        productThumbPictureSize, prepareSpecificationAttributes);
        ////}

        ////[System.Web.Http.NonAction]
        ////protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        ////{
        ////    string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY,
        ////        parentCategoryId,
        ////        string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
        ////        _storeContext.CurrentStore.Id);
        ////    return _cacheManager.Get(cacheKey, () =>
        ////    {
        ////        var categoriesIds = new List<int>();
        ////        var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
        ////        foreach (var category in categories)
        ////        {
        ////            categoriesIds.Add(category.Id);
        ////            categoriesIds.AddRange(GetChildCategoryIds(category.Id));
        ////        }
        ////        return categoriesIds;
        ////    });
        ////}


        ////[System.Web.Http.NonAction]
        ////protected virtual void PrepareSortingOptions(Models.Catalog.CatalogPagingFilteringModel pagingFilteringModel, int orderBy)
        ////{
        ////    if (pagingFilteringModel == null)
        ////        throw new ArgumentNullException("pagingFilteringModel");


        ////    pagingFilteringModel.AllowProductSorting = _catalogSettings.AllowProductSorting;
        ////    if (pagingFilteringModel.AllowProductSorting)
        ////    {
        ////        foreach (ProductSortingEnum enumValue in Enum.GetValues(typeof(ProductSortingEnum)))
        ////        {
        ////            var currentPageUrl = _webHelper.GetThisPageUrl(true);
        ////            //var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "orderby=" + ((int)enumValue).ToString(), null);

        ////            var sortValue = enumValue.GetLocalizedEnum(_localizationService, _workContext);
        ////            pagingFilteringModel.AvailableSortOptions.Add(new SelectListItem
        ////            {
        ////                Text = sortValue,
        ////                Value = string.Concat((int)enumValue),
        ////                Selected = enumValue == (ProductSortingEnum)orderBy
        ////            });
        ////        }
        ////    }
        ////}

        ////protected IList<Product> GetProductsByCategoryId(int categoryId, int itemsNumber)
        ////{
        ////    var categoryIds = new List<int> { categoryId };
        ////    if (_catalogSettings.ShowProductsFromSubcategories)
        ////    {
        ////        //include subcategories
        ////        categoryIds.AddRange(GetChildCategoryIds(categoryId));
        ////    }
        ////    //products
        ////    var products = new List<Product>();
        ////    var featuredProducts = _productService.SearchProducts(
        ////               categoryIds: new List<int> { categoryId },
        ////               storeId: _storeContext.CurrentStore.Id,
        ////               visibleIndividuallyOnly: true,
        ////               featuredProducts: true);
        ////    products.AddRange(featuredProducts);
        ////    int remainingProducts = itemsNumber - products.Count();
        ////    if (remainingProducts > 0)
        ////    {
        ////        IList<int> filterableSpecificationAttributeOptionIds;
        ////        var extraProucts = _productService.SearchProducts(out filterableSpecificationAttributeOptionIds, false,
        ////        categoryIds: categoryIds,
        ////        storeId: _storeContext.CurrentStore.Id,
        ////        visibleIndividuallyOnly: true,
        ////        featuredProducts: false,
        ////        orderBy: (ProductSortingEnum)15,
        ////        pageSize: itemsNumber,
        ////        pageIndex: 0);
        ////        products.AddRange(extraProucts);
        ////    }
        ////    return products;
        ////}

        ////protected IList<Product> GetAllProductsByCategoryId(int categoryId)
        ////{
        ////    var categoryIds = new List<int> { categoryId };
        ////    if (_catalogSettings.ShowProductsFromSubcategories)
        ////    {
        ////        //include subcategories
        ////        categoryIds.AddRange(GetChildCategoryIds(categoryId));
        ////    }
        ////    //products
        ////    var products = _productService.SearchProducts(
        ////               categoryIds: categoryIds,
        ////               storeId: _storeContext.CurrentStore.Id,
        ////               visibleIndividuallyOnly: true);

        ////    return products;
        ////}

        ////[System.Web.Http.NonAction]
        ////protected virtual IEnumerable<SubCategoryModelApi> PrepareCategoryFilterOnSale(IEnumerable<Product> products, int pictureSize)
        ////{
        ////    List<SubCategoryModelApi> categoryList = new List<SubCategoryModelApi>();

        ////    foreach (var item in products)
        ////    {
        ////        var cList = _categoryService.GetProductCategoriesByProductId(item.Id).ToList().Select(s => new SubCategoryModelApi()
        ////        {
        ////            Id = s.CategoryId,
        ////            Name = s.Category.Name,
        ////            PictureModel = new PictureModel()
        ////            {
        ////                FullSizeImageUrl = _pictureService.GetPictureUrl(s.Category.PictureId),
        ////                ImageUrl = _pictureService.GetPictureUrl(s.Category.PictureId, pictureSize),
        ////                Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), s.Category.Name),
        ////                AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), s.Category.Name)
        ////            }

        ////        });

        ////        categoryList = categoryList.Union(cList).ToList();
        ////    }

        ////    return categoryList;
        ////}


        //public IList<CategoryNavigationModelApi> FlatToHierarchy(IEnumerable<CategoryNavigationModelApi> list, int parentId = 0)
        //{
        //    return (from i in list
        //            where i.ParentCategoryId == parentId
        //            select new CategoryNavigationModelApi
        //            {
        //                Id = i.Id,
        //                ParentCategoryId = i.ParentCategoryId,
        //                IconPath = i.IconPath,
        //                Name = i.Name,
        //                Extension = i.Extension,
        //                //ColorSquaresRgb = i.ColorSquaresRgb,
        //                ProductCount = i.ProductCount,
        //                DisplayOrder = i.DisplayOrder,
        //                Children = FlatToHierarchy(list, i.Id)
        //            }).ToList();
        //}
        //#endregion

        //#region Action Method

        //#region category
        ///// <summary>
        ///// Get all categories, languages, currencies
        ///// </summary>
        ///// <returns></returns>
        //[Route("api/categories")]
        //[HttpGet]
        //public IActionResult Categories()
        //{
        //    var model = _catalogModelFactoryApi.PrepareCategoriesModel();

        //    int count = _workContext.CurrentCustomer.ShoppingCartItems
        //               .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //               .LimitPerStore(_storeContext.CurrentStore.Id)
        //               .ToList()
        //               .GetTotalProducts();

        //    //Tax settings
        //    bool displayTaxInOrderSummary = !(_taxSettings.HideTaxInOrderSummary && _workContext.TaxDisplayType == TaxDisplayType.IncludingTax);
        //    //Discount Box Settings
        //    bool showDiscountBox = _shoppingCartSettings.ShowDiscountBox;
        //    // Language Selector
        //    var langNavModel = _commonModelFactoryApi.PrepareLanguageSelectorModel();
        //    // Currency Selector
        //    var currNavModel = _commonModelFactoryApi.PrepareCurrencySelectorModel();

        //    var result = new AllResponseModel
        //    {
        //        Data = model,
        //        Count = count,
        //        DisplayTaxInOrderSummary = displayTaxInOrderSummary,
        //        ShowDiscountBox = showDiscountBox,
        //        Language = langNavModel,
        //        Currency = currNavModel
        //    };

        //    return Ok(result);
        //}

        //[Route("api/v1/categories")]
        //[HttpGet]
        //public IActionResult V1Categories()
        //{
        //    var model = _catalogModelFactoryApi.PrepareCategoriesModel();

        //    int count = _workContext.CurrentCustomer.ShoppingCartItems
        //               .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //               .LimitPerStore(_storeContext.CurrentStore.Id)
        //               .ToList()
        //               .GetTotalProducts();

        //    model = FlatToHierarchy(model, 0);

        //    // Language Selector
        //    var langNavModel = _commonModelFactoryApi.PrepareLanguageSelectorModel();
        //    // Currency Selector
        //    var currNavModel = _commonModelFactoryApi.PrepareCurrencySelectorModel();

        //    var result = new AllResponseModel
        //    {
        //        Data = model,
        //        Count = count,
        //        Language = langNavModel,
        //        Currency = currNavModel
        //    };

        //    return Ok(result);
        //}

        

        //[Route("api/homepagecategories")]
        //[HttpGet]
        //public IActionResult HomepageCategories(int? thumbPictureSize = null)
        //{
        //    var model = _catalogModelFactoryApi.PrepareHomepageCategoryModels(thumbPictureSize);

        //    var result = new GeneralResponseModel<List<CategoryOverViewModelApi>>()
        //    {
        //        Data = model
        //    };
        //    return Ok(result);
        //}

        //[Route("api/catalog/homepagecategorieswithproduct")]
        //[HttpGet]
        //public IActionResult HomepageCategoriesWithProduct(int? thumbPictureSize = null)
        //{
        //    var model = _catalogModelFactoryApi.PrepareHomepageCategoryModelsWithProduct(thumbPictureSize);

        //    var result = new GeneralResponseModel<List<CatalogFeaturedCategoryWithProduct>>()
        //    {
        //        Data = model
        //    };
        //    return Ok(result);
        //}

        //[Route("api/Category/{categoryId}")]
        //[HttpGet]
        //public IActionResult Category(int categoryId, int pageNumber = 1, int orderBy = 0)
        //{
        //    var category = _categoryService.GetCategoryById(categoryId);
        //    if (category == null || category.Deleted)
        //        return NotFound();

        //    //Check whether the current user has a "Manage catalog" permission
        //    //It allows him to preview a category before publishing
        //    if (!category.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
        //        return NotFound();

        //    //ACL (access control list)
        //    if (!_aclService.Authorize(category))
        //        return NotFound();

        //    //Store mapping
        //    if (!_storeMappingService.Authorize(category))
        //        return NotFound();

        //    //model
        //    var model = _catalogModelFactoryApi.PrepareCategoryModel(category, pageNumber, orderBy);

        //    var result = _catalogModelFactoryApi.PrepareCategoryDetailProductResponseModel(model);
        //    return Ok(result);
        //}

        //[Route("api/categoryfeaturedproductandsubcategory/{categoryId}")]
        //[HttpGet]
        //public IActionResult CategoryFeaturedProductAndSubCategory(int categoryId)
        //{
        //    var category = _categoryService.GetCategoryById(categoryId);
        //    if (category == null || category.Deleted)
        //        return NotFound();

        //    //Check whether the current user has a "Manage catalog" permission
        //    //It allows him to preview a category before publishing
        //    if (!category.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
        //        return NotFound();

        //    //ACL (access control list)
        //    if (!_aclService.Authorize(category))
        //        return NotFound();

        //    //Store mapping
        //    if (!_storeMappingService.Authorize(category))
        //        return NotFound();

        //    var model = _catalogModelFactoryApi.CategoryFeaturedProductAndSubCategory(category);

        //    //need to change mapping instead of mapping extension with as nop web current structure
        //    //not done due to lack of time
        //    var result = _catalogModelFactoryApi.PrepareCategoryDetailFeaturedProductAndSubcategoryResponseModel(model);
        //    return Ok(result);
        //}
        //#endregion

        //#region manufacture
        //[Route("api/homepagemanufacture")]
        //[HttpGet]
        //public IActionResult ManufacturerAll(int? thumbPictureSize = null)
        //{
        //    var model = _catalogModelFactoryApi.PrepareManufacturerAllModels(thumbPictureSize);

        //    var result = new GeneralResponseModel<List<MenufactureOverViewModelApi>>()
        //    {
        //        Data = model
        //    };
        //    return Ok(result);
        //}

        //[Route("api/Manufacturer/{manufacturerId}")]
        //[HttpGet]
        //public IActionResult Manufacturer(int manufacturerId, int pageNumber = 1, int orderBy = 0)
        //{
        //    var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
        //    if (manufacturer == null || manufacturer.Deleted)
        //        return NotFound();

        //    //Check whether the current user has a "Manage catalog" permission
        //    //It allows him to preview a manufacturer before publishing
        //    if (!manufacturer.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
        //        return NotFound();

        //    //ACL (access control list)
        //    if (!_aclService.Authorize(manufacturer))
        //        return NotFound();

        //    //Store mapping
        //    if (!_storeMappingService.Authorize(manufacturer))
        //        return NotFound();

        //    //'Continue shopping' URL
        //    //_genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
        //    //    SystemCustomerAttributeNames.LastContinueShoppingPage,
        //    //    _webHelper.GetThisPageUrl(false),
        //    //    _storeContext.CurrentStore.Id);

        //    //model
        //    var model = _catalogModelFactoryApi.PrepareManufacturerModel(manufacturer, pageNumber, orderBy);

        //    var result = _catalogModelFactoryApi.PrepareManufactureDetailProductResponseModel(model);
        //    return Ok(result);
        //}

        //[Route("api/manufacturerfeaturedproduct/{manufacturerId}")]
        //[HttpGet]
        //public IActionResult ManufacturerFeaturedProduct(int manufacturerId)
        //{
        //    var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
        //    if (manufacturer == null || manufacturer.Deleted)
        //        return NotFound();

        //    //Check whether the current user has a "Manage catalog" permission
        //    //It allows him to preview a manufacturer before publishing
        //    if (!manufacturer.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
        //        return NotFound();

        //    //ACL (access control list)
        //    if (!_aclService.Authorize(manufacturer))
        //        return NotFound();

        //    //Store mapping
        //    if (!_storeMappingService.Authorize(manufacturer))
        //        return NotFound();

        //    var model = _catalogModelFactoryApi.PrepareManufacturerFeaturedProduct(manufacturer);

        //    var result = _catalogModelFactoryApi.PrepareManufacturerDetailFeaturedProductResponseModel(model);
        //    return Ok(result);
        //}

        //[Route("api/catalog/search")]
        //[HttpPost]
        //public IActionResult Search(SearchModelApi model, int pageNumber = 1)
        //{
        //    //'Continue shopping' URL
        //    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
        //        SystemCustomerAttributeNames.LastContinueShoppingPage,
        //        _webHelper.GetThisPageUrl(false),
        //        _storeContext.CurrentStore.Id);

        //    var searchModel = _catalogModelFactoryApi.PrepareSearchModelApi(model, pageNumber);
        //    var result = _catalogModelFactoryApi.PrepareSearchProductResponseModel(searchModel);
        //    return Ok(result);
        //}

        //[Route("api/Tag/{productTagId}")]
        //[HttpGet]
        //public IActionResult ProductsByTag(int productTagId, int pageNumber = 1, int orderBy = 0)
        //{
        //    var productTag = _productTagService.GetProductTagById(productTagId);
        //    if (productTag == null)
        //        return NotFound();

        //    var model = _catalogModelFactoryApi.PrepareTagModelApi(productTag, pageNumber, orderBy);
        //    var result = _catalogModelFactoryApi.PrepareProductTagDetailResponseModel(model);
        //    return Ok(result);
        //}

        //#endregion

        //#region Vendors

        //[Route("api/vendor/{vendorId}")]
        //[HttpGet]
        //public IActionResult Vendor(int vendorId)
        //{
        //    var vendor = _vendorService.GetVendorById(vendorId);

        //    //'Continue shopping' URL
        //    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
        //        SystemCustomerAttributeNames.LastContinueShoppingPage,
        //        _webHelper.GetThisPageUrl(false),
        //        _storeContext.CurrentStore.Id);

        //    //model
        //    var model = _catalogModelFactoryApi.PrepareVendorModel(vendor);

        //    return Ok(model);
        //}

        //[Route("api/vendorall")]
        //[HttpGet]
        //public IActionResult VendorAll()
        //{
        //    var result = new GeneralResponseModel<List<VendorModel>>();
        //    var model = _catalogModelFactoryApi.PrepareVendorAllModels();

        //    result.Data = model;

        //    return Ok(result);
        //}

        //[Route("api/vendornav")]
        //[HttpGet]
        //public IActionResult VendorNavigation()
        //{
        //    var model = _catalogModelFactoryApi.PrepareVendorNavigationModel();

        //    return Ok(model);
        //}

        //#endregion

       
        //#region categories&manufactures

        ///// <summary>
        ///// Get all categories and manufacturers
        ///// </summary>
        ///// <returns></returns>
        //[Route("api/categoriesNmanufactures/search")]
        //[HttpGet]
        ////[TokenAuthorize]
        //public IActionResult CategoriesAndManufacturesSearch()
        //{
        //    var model = _catalogModelFactoryApi.PrepareCategoriesAndManufacturersModel();

        //    return Ok(model);
        //}
        //#endregion

        //#endregion
    }
}

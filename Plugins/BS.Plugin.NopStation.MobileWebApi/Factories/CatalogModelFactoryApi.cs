using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Services.Seo;
using Nop.Services.Customers;
using Nop.Web.Framework.Events;
using Nop.Web.Models.Media;
using Nop.Web.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Services;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Customers;

namespace BS.Plugin.NopStation.MobileWebApi.Factories
{
    public partial class CatalogModelFactoryApi : ICatalogModelFactoryApi
    {
        #region Fields
        private int pageSize = 6;
        private readonly IProductModelFactoryApi _productModelFactoryApi;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IProductServiceApi _productServiceApi;
        private readonly IVendorService _vendorService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWebHelper _webHelper;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IProductTagService _productTagService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ITopicService _topicService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ISearchTermService _searchTermService;
     
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly ICategoryIconService _categoryIconService;
        private readonly ICustomerServiceApi _customerServiceApi;
        private readonly BlogSettings _blogSettings;
        private readonly ForumSettings _forumSettings;
        private readonly ICacheManager _cacheManager;
        private readonly DisplayDefaultMenuItemSettings _displayDefaultMenuItemSettings;

        #endregion

        #region Constructors

        public CatalogModelFactoryApi(IProductModelFactoryApi productModelFactoryApi,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IProductService productService,
            IProductServiceApi productServiceApi,
            IVendorService vendorService,
            ICategoryTemplateService categoryTemplateService,
            IManufacturerTemplateService manufacturerTemplateService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICurrencyService currencyService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IPriceFormatter priceFormatter,
            IWebHelper webHelper,
            ISpecificationAttributeService specificationAttributeService,
            IProductTagService productTagService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            ITopicService topicService,
            IEventPublisher eventPublisher,
            ISearchTermService searchTermService,
            IHttpContextAccessor httpContext,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            VendorSettings vendorSettings,
            ICategoryIconService categoryIconServic,
            ICustomerServiceApi customerServiceApi,
            BlogSettings blogSettings,
            ForumSettings forumSettings,
            ICacheManager cacheManager,
            DisplayDefaultMenuItemSettings displayDefaultMenuItemSettings)
        {
            this._productModelFactoryApi = productModelFactoryApi;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._productServiceApi = productServiceApi;
            this._vendorService = vendorService;
            this._categoryTemplateService = categoryTemplateService;
            this._manufacturerTemplateService = manufacturerTemplateService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._priceFormatter = priceFormatter;
            this._webHelper = webHelper;
            this._specificationAttributeService = specificationAttributeService;
            this._productTagService = productTagService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._topicService = topicService;
            this._eventPublisher = eventPublisher;
            this._searchTermService = searchTermService;
            this._httpContextAccessor = httpContext;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._vendorSettings = vendorSettings;
            this._categoryIconService = categoryIconServic;
            this._customerServiceApi = customerServiceApi;
            this._blogSettings = blogSettings;
            this._forumSettings = forumSettings;
            this._cacheManager = cacheManager;
            this._displayDefaultMenuItemSettings = displayDefaultMenuItemSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get child category identifiers
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <returns>List of child category identifiers</returns>
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY,
                parentCategoryId,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () =>
            {
                var categoriesIds = new List<int>();
                var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
                foreach (var category in categories)
                {
                    categoriesIds.Add(category.Id);
                    categoriesIds.AddRange(GetChildCategoryIds(category.Id));
                }
                return categoriesIds;
            });
        }

        protected IList<Product> GetAllProductsByCategoryId(int categoryId)
        {
            var categoryIds = new List<int> { categoryId };
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(categoryId));
            }
            //products
            var products = _productService.SearchProducts(
                       categoryIds: categoryIds,
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true);

            return products;
        }

        protected IList<Product> GetProductsByCategoryId(int categoryId, int itemsNumber)
        {
            var categoryIds = new List<int> { categoryId };
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(categoryId));
            }
            //products
            var products = new List<Product>();
            var featuredProducts = _productService.SearchProducts(
                       categoryIds: new List<int> { categoryId },
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true);
            products.AddRange(featuredProducts);
            int remainingProducts = itemsNumber - products.Count();
            if (remainingProducts > 0)
            {
                IList<int> filterableSpecificationAttributeOptionIds;
                var extraProucts = _productService.SearchProducts(out filterableSpecificationAttributeOptionIds, false,
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                featuredProducts: false,
                orderBy: (ProductSortingEnum)15,
                pageSize: itemsNumber,
                pageIndex: 0);
                products.AddRange(extraProucts);
            }
            return products;
        }

        protected IList<CategoryNavigationModelApi> PrepareCategoryNavigationModelApi(IPagedList<Category> categories)
        {
            var categoriesNavigationModelApi = new List<CategoryNavigationModelApi>();

            foreach (var category in categories)
            {
                var catIds = new List<int>();
                catIds.Add(category.Id);

                var categoryNavigationModelApi = new CategoryNavigationModelApi
                {
                    Id = category.Id,
                    ParentCategoryId = category.ParentCategoryId,
                    Name = category.Name,
                    //Extension = category.Extension,
                    ProductCount = _productService.SearchProducts(categoryIds: catIds).Count,
                    DisplayOrder = category.DisplayOrder
                };
                categoriesNavigationModelApi.Add(categoryNavigationModelApi);
            }

            return categoriesNavigationModelApi;
        }
        #endregion

        #region Common

        /// <summary>
        /// Prepare sorting options
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        public virtual void PrepareSortingOptions(Models.Catalog.CatalogPagingFilteringModel pagingFilteringModel, int orderBy)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            pagingFilteringModel.AllowProductSorting = _catalogSettings.AllowProductSorting;
            if (pagingFilteringModel.AllowProductSorting)
            {
                foreach (ProductSortingEnum enumValue in Enum.GetValues(typeof(ProductSortingEnum)))
                {
                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    //var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "orderby=" + ((int)enumValue).ToString(), null);

                    var sortValue = enumValue.GetLocalizedEnum(_localizationService, _workContext);
                    pagingFilteringModel.AvailableSortOptions.Add(new SelectListItem
                    {
                        Text = sortValue,
                        Value = string.Concat((int)enumValue),
                        Selected = enumValue == (ProductSortingEnum)orderBy
                    });
                }
            }
        }

        /// <summary>
        /// Prepare view modes
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        public virtual void PrepareViewModes(Models.Catalog.CatalogPagingFilteringModel pagingFilteringModel, Models.Catalog.CatalogPagingFilteringModel command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            pagingFilteringModel.AllowProductViewModeChanging = _catalogSettings.AllowProductViewModeChanging;

            var viewMode = !string.IsNullOrEmpty(command.ViewMode)
                ? command.ViewMode
                : _catalogSettings.DefaultViewMode;
            pagingFilteringModel.ViewMode = viewMode;
            if (pagingFilteringModel.AllowProductViewModeChanging)
            {
                var currentPageUrl = _webHelper.GetThisPageUrl(true);
                //grid
                pagingFilteringModel.AvailableViewModes.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Catalog.ViewMode.Grid"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=grid", null),
                    Selected = viewMode == "grid"
                });
                //list
                pagingFilteringModel.AvailableViewModes.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Catalog.ViewMode.List"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=list", null),
                    Selected = viewMode == "list"
                });
            }

        }

        /// <summary>
        /// Prepare page size options
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <param name="allowCustomersToSelectPageSize">Are customers allowed to select page size?</param>
        /// <param name="pageSizeOptions">Page size options</param>
        /// <param name="fixedPageSize">Fixed page size</param>
        public virtual void PreparePageSizeOptions(Models.Catalog.CatalogPagingFilteringModel pagingFilteringModel, Models.Catalog.CatalogPagingFilteringModel command,
            bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            if (command.PageNumber <= 0)
            {
                command.PageNumber = 1;
            }
            pagingFilteringModel.AllowCustomersToSelectPageSize = false;
            if (allowCustomersToSelectPageSize && pageSizeOptions != null)
            {
                var pageSizes = pageSizeOptions.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (pageSizes.Any())
                {
                    // get the first page size entry to use as the default (category page load) or if customer enters invalid value via query string
                    if (command.PageSize <= 0 || !pageSizes.Contains(command.PageSize.ToString()))
                    {
                        int temp;
                        if (int.TryParse(pageSizes.FirstOrDefault(), out temp))
                        {
                            if (temp > 0)
                            {
                                command.PageSize = temp;
                            }
                        }
                    }

                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "pagesize={0}", null);
                    sortUrl = _webHelper.RemoveQueryString(sortUrl, "pagenumber");

                    foreach (var pageSize in pageSizes)
                    {
                        int temp;
                        if (!int.TryParse(pageSize, out temp))
                        {
                            continue;
                        }
                        if (temp <= 0)
                        {
                            continue;
                        }

                        pagingFilteringModel.PageSizeOptions.Add(new SelectListItem
                        {
                            Text = pageSize,
                            Value = String.Format(sortUrl, pageSize),
                            Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        });
                    }

                    if (pagingFilteringModel.PageSizeOptions.Any())
                    {
                        pagingFilteringModel.PageSizeOptions = pagingFilteringModel.PageSizeOptions.OrderBy(x => int.Parse(x.Text)).ToList();
                        pagingFilteringModel.AllowCustomersToSelectPageSize = true;

                        if (command.PageSize <= 0)
                        {
                            command.PageSize = int.Parse(pagingFilteringModel.PageSizeOptions.FirstOrDefault().Text);
                        }
                    }
                }
            }
            else
            {
                //customer is not allowed to select a page size
                command.PageSize = fixedPageSize;
            }

            //ensure pge size is specified
            if (command.PageSize <= 0)
            {
                command.PageSize = fixedPageSize;
            }
        }

        #endregion

        #region Categories

        /// <summary>
        /// Prepare CategoryNavigation model
        /// </summary>
        /// <returns>List of CategoryNavigation model</returns>
        public virtual IList<CategoryNavigationModelApi> PrepareCategoriesModel()
        {
            string categoryCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_MENU_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var cachedCategoriesModel = _cacheManager.Get(categoryCacheKey, () => _categoryService.GetAllCategories());

            var model = PrepareCategoryNavigationModelApi(cachedCategoriesModel);
            foreach (var cat in model)
            {
                string fileName = "defaultIcon.png";
                var catIcon = _categoryIconService.GetIconExtentionByCategoryId(cat.Id);
                if (catIcon != null)
                {
                    fileName = string.Format("{0}{1}", cat.Id.ToString(), _categoryIconService.GetIconExtentionByCategoryId(cat.Id).Extension);
                    var iconId = catIcon.Id.ToString();
                    cat.IconPath = fileName.IconImagePath(_webHelper) + "?id=" + iconId;
                }

                else
                {
                    cat.IconPath = fileName.IconImagePath(_webHelper);
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare Category model
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Category model</returns>
        public virtual CategoryModelApi PrepareCategoryModel(Category category, int pageNumber, int orderBy)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            var model = new CategoryModelApi
            {
                Id = category.Id,
                Name = category.GetLocalized(x => x.Name),
                Description = category.GetLocalized(x => x.Description)
            };

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, orderBy);

            var selectedPriceRange = model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRange(_webHelper, category.PriceRanges);
            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;
            if (selectedPriceRange != null)
            {
                if (selectedPriceRange.From.HasValue)
                    minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.From.Value, _workContext.WorkingCurrency);

                if (selectedPriceRange.To.HasValue)
                    maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.To.Value, _workContext.WorkingCurrency);
            }            

            var categoryIds = new List<int>();
            categoryIds.Add(category.Id);
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(category.Id));
            }
            //products
            IList<int> alreadyFilteredSpecOptionIds = model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIds(_webHelper);
            IList<int> filterableSpecificationAttributeOptionIds;
            var products = _productService.SearchProducts(out filterableSpecificationAttributeOptionIds,
                true,
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
                priceMin: minPriceConverted,
                priceMax: maxPriceConverted,
                filteredSpecs: alreadyFilteredSpecOptionIds,
                orderBy: (ProductSortingEnum)orderBy,
                pageIndex: pageNumber - 1,
                pageSize: this.pageSize);

            var price = _productServiceApi.SearchProductsPrice(
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
                priceMin: minPriceConverted,
                priceMax: maxPriceConverted,
                filteredSpecs: alreadyFilteredSpecOptionIds,
                orderBy: (ProductSortingEnum)orderBy,
                pageSize: this.pageSize,
                pageIndex: pageNumber - 1);

            model.Products = _productModelFactoryApi.PrepareProductOverviewModels(products).ToList();
            model.PagingFilteringContext.LoadPagedList(products);
            model.PriceRange = price;

            //specs
            model.PagingFilteringContext.SpecificationFilter.PrepareSpecsFilters(alreadyFilteredSpecOptionIds,
                filterableSpecificationAttributeOptionIds != null ? filterableSpecificationAttributeOptionIds.ToArray() : null,
                _specificationAttributeService,
                _webHelper,
                _workContext);

            return model;
        }        

        /// <summary>
        /// Prepare category template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Category template view path</returns>
        public virtual string PrepareCategoryTemplateViewPath(int templateId)
        {
            var templateCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_TEMPLATE_MODEL_KEY, templateId);
            var templateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _categoryTemplateService.GetCategoryTemplateById(templateId);
                if (template == null)
                    template = _categoryTemplateService.GetAllCategoryTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });

            return templateViewPath;
        }

        /// <summary>
        /// Prepare category navigation model
        /// </summary>
        /// <param name="currentCategoryId">Current category identifier</param>
        /// <param name="currentProductId">Current product identifier</param>
        /// <returns>Category navigation model</returns>
        //public virtual CategoryNavigationModelApi PrepareCategoryNavigationModel(int currentCategoryId, int currentProductId)
        //{
        //    //get active category
        //    int activeCategoryId = 0;
        //    if (currentCategoryId > 0)
        //    {
        //        //category details page
        //        activeCategoryId = currentCategoryId;
        //    }
        //    else if (currentProductId > 0)
        //    {
        //        //product details page
        //        var productCategories = _categoryService.GetProductCategoriesByProductId(currentProductId);
        //        if (productCategories.Any())
        //            activeCategoryId = productCategories[0].CategoryId;
        //    }

        //    var cachedCategoriesModel = PrepareCategorySimpleModels();
        //    var model = new CategoryNavigationModelApi
        //    {
        //        CurrentCategoryId = activeCategoryId,
        //        Categories = cachedCategoriesModel
        //    };

        //    return model;
        //}

        /// <summary>
        /// Prepare top menu model
        /// </summary>
        /// <returns>Top menu model</returns>
        //public virtual TopMenuModel PrepareTopMenuModel()
        //{
        //    //categories
        //    var cachedCategoriesModel = PrepareCategorySimpleModels();

        //    //top menu topics
        //    string topicCacheKey = string.Format(ModelCacheEventConsumer.TOPIC_TOP_MENU_MODEL_KEY,
        //        _workContext.WorkingLanguage.Id,
        //        _storeContext.CurrentStore.Id,
        //        string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
        //    var cachedTopicModel = _cacheManager.Get(topicCacheKey, () =>
        //        _topicService.GetAllTopics(_storeContext.CurrentStore.Id)
        //        .Where(t => t.IncludeInTopMenu)
        //        .Select(t => new TopMenuModel.TopMenuTopicModel
        //        {
        //            Id = t.Id,
        //            Name = t.GetLocalized(x => x.Title),
        //            SeName = t.GetSeName()
        //        })
        //        .ToList()
        //    );
        //    var model = new TopMenuModel
        //    {
        //        Categories = cachedCategoriesModel,
        //        Topics = cachedTopicModel,
        //        NewProductsEnabled = _catalogSettings.NewProductsEnabled,
        //        BlogEnabled = _blogSettings.Enabled,
        //        ForumEnabled = _forumSettings.ForumsEnabled,
        //        DisplayHomePageMenuItem = _displayDefaultMenuItemSettings.DisplayHomePageMenuItem,
        //        DisplayNewProductsMenuItem = _displayDefaultMenuItemSettings.DisplayNewProductsMenuItem,
        //        DisplayProductSearchMenuItem = _displayDefaultMenuItemSettings.DisplayProductSearchMenuItem,
        //        DisplayCustomerInfoMenuItem = _displayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem,
        //        DisplayBlogMenuItem = _displayDefaultMenuItemSettings.DisplayBlogMenuItem,
        //        DisplayForumsMenuItem = _displayDefaultMenuItemSettings.DisplayForumsMenuItem,
        //        DisplayContactUsMenuItem = _displayDefaultMenuItemSettings.DisplayContactUsMenuItem
        //    };
        //    return model;
        //}

        /// <summary>
        /// Prepare homepage category models
        /// </summary>
        /// <returns>List of homepage category models</returns>
        public virtual List<CategoryOverViewModelApi> PrepareHomepageCategoryModels(int? thumbPictureSize)
        {
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            string categoriesCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_HOMEPAGE_KEY,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                pictureSize,
                _storeContext.CurrentStore.Id,
                _workContext.WorkingLanguage.Id,
                _webHelper.IsCurrentConnectionSecured());

            var model = _cacheManager.Get(categoriesCacheKey, () =>
                _categoryService.GetAllCategoriesDisplayedOnHomePage()
                .Select(category =>
                {
                    var catModel = new CategoryOverViewModelApi
                    {
                        Id = category.Id,
                        Name = category.GetLocalized(x => x.Name)
                    };

                    //prepare picture model
                    if (thumbPictureSize.HasValue)
                    {
                        pictureSize = thumbPictureSize.GetValueOrDefault();
                    }

                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, category.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                    catModel.DefaultPictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(category.PictureId);
                        var pictureModel = new Models.DashboardModel.PictureModel
                        {
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        };
                        return pictureModel;
                    });
                    catModel.ProductCount = GetAllProductsByCategoryId(catModel.Id).Count;
                    return catModel;
                })
                .ToList()
            );

            return model;
        }

        /// <summary>
        /// Prepare homepage category models with product
        /// </summary>
        /// <returns>List of homepage category models with product</returns>
        public virtual List<CatalogFeaturedCategoryWithProduct> PrepareHomepageCategoryModelsWithProduct(int? thumbPictureSize)
        {
            string categoriesCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_HOMEPAGE_KEY_PRODUCT_CATEGORY,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id,
                _workContext.WorkingLanguage.Id,
                _webHelper.IsCurrentConnectionSecured());

            var model = _cacheManager.Get(categoriesCacheKey, () =>
                _categoryService.GetAllCategoriesDisplayedOnHomePage()
                .Select(x =>
                {
                    var catWithProduct = new CatalogFeaturedCategoryWithProduct();
                    var subCategory = _categoryService.GetAllCategoriesByParentCategoryId(x.Id).Select(c => new CatalogFeaturedCategoryWithProduct.SubCategoriesWithNameAndId() { Id = c.Id, Name = c.Name, IconPath = _categoryIconService.GetIconExtentionByCategoryId(c.Id) != null ? string.Format("{0}{1}", c.Id.ToString(), _categoryIconService.GetIconExtentionByCategoryId(c.Id)).IconImagePath(_webHelper) : ("defaultIcon.png").IconImagePath(_webHelper) }).ToList();
                    //var categoryIds = subCategory.Select(c => c.Id).ToList();
                    //var catModel = x.MapTo<Category, CategoryOverViewModelApi>();
                    var catModel = new CategoryOverViewModelApi
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name)
                    };
                    int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                    //prepare picture model
                    if (thumbPictureSize.HasValue)
                    {
                        pictureSize = thumbPictureSize.GetValueOrDefault();
                    }

                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                    catModel.DefaultPictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(x.PictureId);
                        var pictureModel = new Models.DashboardModel.PictureModel
                        {
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        };
                        return pictureModel;
                    });
                    catWithProduct.SubCategory = subCategory;
                    var products = GetProductsByCategoryId(x.Id, 6);
                    catWithProduct.Product = _productModelFactoryApi.PrepareProductOverviewModels(products);
                    catModel.ProductCount = products.Count;
                    catWithProduct.Category = catModel;
                    return catWithProduct;
                })
                .ToList()
            );
            return model;
        }

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <returns>List of category (simple) models</returns>
        public virtual List<CategorySimpleModel> PrepareCategorySimpleModels()
        {
            //load and cache them
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_ALL_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () => PrepareCategorySimpleModels(0));
        }

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <param name="allCategories">All available categories; pass null to load them internally</param>
        /// <returns>List of category (simple) models</returns>
        public virtual List<CategorySimpleModel> PrepareCategorySimpleModels(int rootCategoryId,
            bool loadSubCategories = true, IList<Category> allCategories = null)
        {
            var result = new List<CategorySimpleModel>();

            //little hack for performance optimization.
            //we know that this method is used to load top and left menu for categories.
            //it'll load all categories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once
            //if you don't like this implementation if you can uncomment the line below (old behavior) and comment several next lines (before foreach)
            //var categories = _categoryService.GetAllCategoriesByParentCategoryId(rootCategoryId);
            if (allCategories == null)
            {
                //load categories if null passed
                //we implemeneted it this way for performance optimization - recursive iterations (below)
                //this way all categories are loaded only once
                allCategories = _categoryService.GetAllCategories(storeId: _storeContext.CurrentStore.Id);
            }
            var categories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).ToList();
            foreach (var category in categories)
            {
                var categoryModel = new CategorySimpleModel
                {
                    Id = category.Id,
                    Name = category.GetLocalized(x => x.Name),
                    SeName = category.GetSeName(),
                    IncludeInTopMenu = category.IncludeInTopMenu
                };

                //number of products in each category
                if (_catalogSettings.ShowCategoryProductNumber)
                {
                    string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_NUMBER_OF_PRODUCTS_MODEL_KEY,
                        string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                        _storeContext.CurrentStore.Id,
                        category.Id);
                    categoryModel.NumberOfProducts = _cacheManager.Get(cacheKey, () =>
                    {
                        var categoryIds = new List<int>();
                        categoryIds.Add(category.Id);
                        //include subcategories
                        if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                            categoryIds.AddRange(GetChildCategoryIds(category.Id));
                        return _productService.GetNumberOfProductsInCategory(categoryIds, _storeContext.CurrentStore.Id);
                    });
                }

                if (loadSubCategories)
                {
                    var subCategories = PrepareCategorySimpleModels(category.Id, loadSubCategories, allCategories);
                    categoryModel.SubCategories.AddRange(subCategories);
                }
                result.Add(categoryModel);
            }

            return result;
        }

        public virtual CategoryModelApi CategoryFeaturedProductAndSubCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            var model = new CategoryModelApi
            {
                Id = category.Id,
                Name = category.GetLocalized(x => x.Name),
                Description = category.GetLocalized(x => x.Description)
            };

            //subcategories
            string subCategoriesCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_SUBCATEGORIES_KEY,
                category.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id,
                _workContext.WorkingLanguage.Id,
                _webHelper.IsCurrentConnectionSecured());
            model.SubCategories = _cacheManager.Get(subCategoriesCacheKey, () =>
                _categoryService.GetAllCategoriesByParentCategoryId(category.Id)
                .Select(x =>
                {
                    var subCatModel = new SubCategoryModelApi
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name)
                    };

                    //prepare picture model
                    int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                    subCatModel.PictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(x.PictureId);
                        var pictureModel = new PictureModel
                        {
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize)
                        };
                        return pictureModel;
                    });

                    return subCatModel;
                })
                .ToList()
            );

            //featured products
            if (!_catalogSettings.IgnoreFeaturedProducts)
            {
                //We cache a value indicating whether we have featured products
                IPagedList<Product> featuredProducts = null;
                string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_HAS_FEATURED_PRODUCTS_KEY, category.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()), _storeContext.CurrentStore.Id);
                var hasFeaturedProductsCache = _cacheManager.Get<bool?>(cacheKey);
                if (!hasFeaturedProductsCache.HasValue)
                {
                    //no value in the cache yet
                    //let's load products and cache the result (true/false)
                    featuredProducts = _productService.SearchProducts(
                       categoryIds: new List<int> { category.Id },
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true);
                    hasFeaturedProductsCache = featuredProducts.TotalCount > 0;
                    _cacheManager.Set(cacheKey, hasFeaturedProductsCache, 60);
                }
                if (hasFeaturedProductsCache.Value && featuredProducts == null)
                {
                    //cache indicates that the category has featured products
                    //let's load them
                    featuredProducts = _productService.SearchProducts(
                       categoryIds: new List<int> { category.Id },
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true);
                }
                if (featuredProducts != null)
                {
                    model.FeaturedProducts = _productModelFactoryApi.PrepareProductOverviewModels(featuredProducts).ToList();
                }
            }

            return model;
        }

        public virtual CategoryDetailProductResponseModel PrepareCategoryDetailProductResponseModel(CategoryModelApi model)
        {
            if (model == null)
                return null;

            var categoryDetailProductResponseModel = new CategoryDetailProductResponseModel
            {
                Name = model.Name,
                Products = model.Products,
                TotalPages = model.PagingFilteringContext.TotalPages,
                NotFilteredItems = model.PagingFilteringContext.SpecificationFilter.NotFilteredItems,
                AlreadyFilteredItems = model.PagingFilteringContext.SpecificationFilter.AlreadyFilteredItems,
                PriceRange = model.PriceRange,
                AvailableSortOptions = model.PagingFilteringContext.AvailableSortOptions

            };
            return categoryDetailProductResponseModel;
        }

        public virtual CategoryDetailFeaturedProductAndSubcategoryResponseModel
            PrepareCategoryDetailFeaturedProductAndSubcategoryResponseModel(CategoryModelApi categoryModelApi)
        {
            if (categoryModelApi == null)
                return null;

            var responseModel = new CategoryDetailFeaturedProductAndSubcategoryResponseModel
            {
                SubCategories = categoryModelApi.SubCategories,
                FeaturedProducts = categoryModelApi.FeaturedProducts
            };

            return responseModel;
        }
        #endregion

        #region Manufacturers

        /// <summary>
        /// Prepare manufacturer model
        /// </summary>
        /// <param name="manufacturer">Manufacturer identifier</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Manufacturer model</returns>
        public virtual ManuFactureModelApi PrepareManufacturerModel(Manufacturer manufacturer, int pageNumber, int orderBy)
        {
            if (manufacturer == null)
                throw new ArgumentNullException("manufacturer");

            var model = new ManuFactureModelApi
            {
                Id = manufacturer.Id,
                Name = manufacturer.GetLocalized(x => x.Name),
                Description = manufacturer.GetLocalized(x => x.Description)
            };

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, orderBy);

            var selectedPriceRange = model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRange(_webHelper, manufacturer.PriceRanges);
            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;
            if (selectedPriceRange != null)
            {
                if (selectedPriceRange.From.HasValue)
                    minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.From.Value, _workContext.WorkingCurrency);

                if (selectedPriceRange.To.HasValue)
                    maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.To.Value, _workContext.WorkingCurrency);
            }           

            //products
            IList<int> filterableSpecificationAttributeOptionIds;
            var products = _productService.SearchProducts(out filterableSpecificationAttributeOptionIds, true,
                manufacturerId: manufacturer.Id,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
                priceMin: minPriceConverted,
                priceMax: maxPriceConverted,
                orderBy: (ProductSortingEnum)orderBy,
                pageIndex: pageNumber - 1,
                pageSize: this.pageSize);
            var price = _productServiceApi.SearchProductsPrice(
              manufacturerId: manufacturer.Id,
              storeId: _storeContext.CurrentStore.Id,
              visibleIndividuallyOnly: true,
              featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
              priceMin: minPriceConverted,
              priceMax: maxPriceConverted,
              orderBy: (ProductSortingEnum)orderBy,
              pageSize: this.pageSize,
              pageIndex: pageNumber - 1);

            model.Products = _productModelFactoryApi.PrepareProductOverviewModels(products).ToList();
            model.PagingFilteringContext.LoadPagedList(products);
            model.PriceRange = price;

            return model;
        }

        /// <summary>
        /// Prepare manufacturer template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Manufacturer template view path</returns>
        public virtual string PrepareManufacturerTemplateViewPath(int templateId)
        {
            var templateCacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURER_TEMPLATE_MODEL_KEY, templateId);
            var templateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _manufacturerTemplateService.GetManufacturerTemplateById(templateId);
                if (template == null)
                    template = _manufacturerTemplateService.GetAllManufacturerTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });

            return templateViewPath;
        }

        /// <summary>
        /// Prepare manufacturer all models
        /// </summary>
        /// <returns>List of manufacturer models</returns>
        public virtual List<MenufactureOverViewModelApi> PrepareManufacturerAllModels(int? thumbPictureSize)
        {
            var model = new List<MenufactureOverViewModelApi>();
            var manufacturers = _manufacturerService.GetAllManufacturers(storeId: _storeContext.CurrentStore.Id);
            foreach (var manufacturer in manufacturers)
            {
                var modelMan = new MenufactureOverViewModelApi
                {
                    Id = manufacturer.Id,
                    Name = manufacturer.GetLocalized(x => x.Name)
                };

                //prepare picture model
                int pictureSize = _mediaSettings.ManufacturerThumbPictureSize;
                if (thumbPictureSize.HasValue)
                {
                    pictureSize = thumbPictureSize.GetValueOrDefault();
                }            
                var manufacturerPictureCacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURER_PICTURE_MODEL_KEY, manufacturer.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                modelMan.DefaultPictureModel = _cacheManager.Get(manufacturerPictureCacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(manufacturer.PictureId);
                    var pictureModel = new Models.DashboardModel.PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize)
                    };
                    return pictureModel;
                });
                model.Add(modelMan);
            }

            return model;
        }

        public virtual ManuFactureModelApi PrepareManufacturerFeaturedProduct(Manufacturer manufacturer)
        {
            var model = new ManuFactureModelApi
            {
                Id = manufacturer.Id,
                Name = manufacturer.GetLocalized(x => x.Name),
                Description = manufacturer.Description
            };

            //featured products
            if (!_catalogSettings.IgnoreFeaturedProducts)
            {
                IPagedList<Product> featuredProducts = null;

                //We cache a value indicating whether we have featured products
                string cacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURER_HAS_FEATURED_PRODUCTS_KEY,
                    manufacturer.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id);
                var hasFeaturedProductsCache = _cacheManager.Get<bool?>(cacheKey);
                if (!hasFeaturedProductsCache.HasValue)
                {
                    //no value in the cache yet
                    //let's load products and cache the result (true/false)
                    featuredProducts = _productService.SearchProducts(
                       manufacturerId: manufacturer.Id,
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true);
                    hasFeaturedProductsCache = featuredProducts.TotalCount > 0;
                    _cacheManager.Set(cacheKey, hasFeaturedProductsCache, 60);
                }
                if (hasFeaturedProductsCache.Value && featuredProducts == null)
                {
                    //cache indicates that the manufacturer has featured products
                    //let's load them
                    featuredProducts = _productService.SearchProducts(
                       manufacturerId: manufacturer.Id,
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true);
                }
                if (featuredProducts != null)
                {
                    model.FeaturedProducts = _productModelFactoryApi.PrepareProductOverviewModels(featuredProducts).ToList();
                }
            }

            return model;
        }

        public virtual SearchModelApi PrepareSearchModelApi(SearchModelApi model, int pageNumber)
        {
            var price = new PriceRange();
            if (model == null)
            {
                model = new SearchModelApi();
            }

            var searchTerms = model.q;
            if (searchTerms == null)
                searchTerms = "";
            searchTerms = searchTerms.Trim();

            IPagedList<Product> products = new PagedList<Product>(new List<Product>(), 0, 1);
            // only search if query string search keyword is set (used to avoid searching or displaying search term min length error message on /search page load)

            if (searchTerms.Length < _catalogSettings.ProductSearchTermMinimumLength)
            {
                model.Warning = string.Format(_localizationService.GetResource("Search.SearchTermMinimumLengthIsNCharacters"), _catalogSettings.ProductSearchTermMinimumLength);
            }
            else
            {
                var categoryIds = new List<int>();
                int manufacturerId = 0;
                decimal? minPriceConverted = null;
                decimal? maxPriceConverted = null;
                bool searchInDescriptions = false;

                if (model.adv)
                {
                    //advanced search
                    var categoryId = model.cid;
                    if (categoryId > 0)
                    {
                        categoryIds.Add(categoryId);
                        if (model.isc)
                        {
                            //include subcategories
                            categoryIds.AddRange(GetChildCategoryIds(categoryId));
                        }
                    }

                    manufacturerId = model.mid;

                    //min price
                    if (!string.IsNullOrEmpty(model.pf))
                    {
                        decimal minPrice;
                        if (decimal.TryParse(model.pf, out minPrice))
                            minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(minPrice, _workContext.WorkingCurrency);
                    }
                    //max price
                    if (!string.IsNullOrEmpty(model.pt))
                    {
                        decimal maxPrice;
                        if (decimal.TryParse(model.pt, out maxPrice))
                            maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(maxPrice, _workContext.WorkingCurrency);
                    }

                    searchInDescriptions = model.sid;
                }

                //var searchInProductTags = false;
                var searchInProductTags = searchInDescriptions;

                //products
                products = _productService.SearchProducts(
                    categoryIds: categoryIds,
                    manufacturerId: manufacturerId,
                    storeId: _storeContext.CurrentStore.Id,
                    visibleIndividuallyOnly: true,
                    priceMin: minPriceConverted,
                    priceMax: maxPriceConverted,
                    keywords: searchTerms,
                    searchDescriptions: searchInDescriptions,
                    searchSku: searchInDescriptions,
                    searchProductTags: searchInProductTags,
                    languageId: _workContext.WorkingLanguage.Id,
                    orderBy: (ProductSortingEnum)0,
                    pageIndex: pageNumber - 1,
                    pageSize: this.pageSize);

                price = _productServiceApi.SearchProductsPrice(
                   categoryIds: categoryIds,
                   manufacturerId: manufacturerId,
                   storeId: _storeContext.CurrentStore.Id,
                   visibleIndividuallyOnly: true,
                   priceMin: minPriceConverted,
                   priceMax: maxPriceConverted,
                   keywords: searchTerms,
                   searchDescriptions: searchInDescriptions,
                   searchSku: searchInDescriptions,
                   searchProductTags: searchInProductTags,
                   languageId: _workContext.WorkingLanguage.Id,
                   orderBy: (ProductSortingEnum)0,
                   pageIndex: pageNumber - 1,
                   pageSize: this.pageSize);

                model.Products = _productModelFactoryApi.PrepareProductOverviewModels(products).ToList();

                model.NoResults = !model.Products.Any();

                //search term statistics
                if (!String.IsNullOrEmpty(searchTerms))
                {
                    var searchTerm = _searchTermService.GetSearchTermByKeyword(searchTerms, _storeContext.CurrentStore.Id);
                    if (searchTerm != null)
                    {
                        searchTerm.Count++;
                        _searchTermService.UpdateSearchTerm(searchTerm);
                    }
                    else
                    {
                        searchTerm = new SearchTerm
                        {
                            Keyword = searchTerms,
                            StoreId = _storeContext.CurrentStore.Id,
                            Count = 1
                        };
                        _searchTermService.InsertSearchTerm(searchTerm);
                    }
                }

                //event
                _eventPublisher.Publish(new ProductSearchEvent
                {
                    SearchTerm = searchTerms,
                    SearchInDescriptions = searchInDescriptions,
                    CategoryIds = categoryIds,
                    ManufacturerId = manufacturerId,
                    WorkingLanguageId = _workContext.WorkingLanguage.Id
                });
            }

            model.PagingFilteringContext.LoadPagedList(products);
            model.PriceRange = price;

            return model;
        }

        public virtual TagModelApi PrepareTagModelApi(ProductTag productTag, int pageNumber, int orderBy)
        {
            var model = new TagModelApi()
            {
                Id = productTag.Id,
                Name = productTag.GetLocalized(x => x.Name)
            };
            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, orderBy);
            
            //products
            var products = _productService.SearchProducts(
                storeId: _storeContext.CurrentStore.Id,
                productTagId: productTag.Id,
                visibleIndividuallyOnly: true,
                orderBy: (ProductSortingEnum)orderBy,
                pageIndex: pageNumber - 1,
                pageSize: this.pageSize);

            model.Products = _productModelFactoryApi.PrepareProductOverviewModels(products).ToList();

            model.PagingFilteringContext.LoadPagedList(products);

            return model;
        }

        public virtual ManufactureDetailProductResponseModel PrepareManufactureDetailProductResponseModel(ManuFactureModelApi model)
        {
            if (model == null)
                return null;

            var manufactureDetailProductResponseModel = new ManufactureDetailProductResponseModel
            {
                Name = model.Name,
                Products = model.Products,
                TotalPages = model.PagingFilteringContext.TotalPages,
                NotFilteredItems = model.PagingFilteringContext.SpecificationFilter.NotFilteredItems,
                AlreadyFilteredItems = model.PagingFilteringContext.SpecificationFilter.AlreadyFilteredItems,
                PriceRange = model.PriceRange,
                AvailableSortOptions = model.PagingFilteringContext.AvailableSortOptions

            };
            return manufactureDetailProductResponseModel;
        }

        public virtual SearchProductResponseModel PrepareSearchProductResponseModel(SearchModelApi model)
        {
            if (model == null)
                return null;

            var searchProductResponseModel = new SearchProductResponseModel()
            {
                Products = model.Products,
                TotalPages = model.PagingFilteringContext.TotalPages,
                PriceRange = model.PriceRange,
                AvailableSortOptions = model.PagingFilteringContext.AvailableSortOptions,

            };
            return searchProductResponseModel;
        }

        public virtual ProductTagDetailResponseModel PrepareProductTagDetailResponseModel(TagModelApi model)
        {
            if (model == null)
                return null;

            var productTagDetailResponseModel = new ProductTagDetailResponseModel
            {
                Name = model.Name,
                Products = model.Products,
                TotalPages = model.PagingFilteringContext.TotalPages,
                NotFilteredItems = model.PagingFilteringContext.SpecificationFilter.NotFilteredItems,
                AlreadyFilteredItems = model.PagingFilteringContext.SpecificationFilter.AlreadyFilteredItems,
                AvailableSortOptions = model.PagingFilteringContext.AvailableSortOptions

            };
            return productTagDetailResponseModel;
        }

        public virtual ManufacturerDetailFeaturedProductResponseModel
            PrepareManufacturerDetailFeaturedProductResponseModel(ManuFactureModelApi manuFactureModelApi)
        {
            if (manuFactureModelApi == null)
                return null;

            var responseModel = new ManufacturerDetailFeaturedProductResponseModel
            {
                FeaturedProducts = manuFactureModelApi.FeaturedProducts
            };

            return responseModel;
        }

        
        #endregion

        #region Vendors

        /// <summary>
        /// Prepare vendor model
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Vendor model</returns>
        public virtual VendorDetailProductResponseModel PrepareVendorModel(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException("vendor");

            var model = new VendorDetailProductResponseModel
            {
                Id = vendor.Id,
                Name = vendor.GetLocalized(x => x.Name),
                Description = vendor.GetLocalized(x => x.Description),
                MetaKeywords = vendor.GetLocalized(x => x.MetaKeywords),
                MetaDescription = vendor.GetLocalized(x => x.MetaDescription),
                MetaTitle = vendor.GetLocalized(x => x.MetaTitle),
                SeName = vendor.GetSeName(),
                AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors
            };

            //prepare picture model
            int pictureSize = _mediaSettings.VendorThumbPictureSize;
            var pictureCacheKey = string.Format(ModelCacheEventConsumer.VENDOR_PICTURE_MODEL_KEY, vendor.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
            model.PictureModel = _cacheManager.Get(pictureCacheKey, () =>
            {
                var picture = _pictureService.GetPictureById(vendor.PictureId);
                var pictureModel = new PictureModel
                {
                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                    Title = string.Format(_localizationService.GetResource("Media.Vendor.ImageLinkTitleFormat"), model.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Vendor.ImageAlternateTextFormat"), model.Name)
                };
                return pictureModel;
            });

            var customerVendor = _customerServiceApi.GetCustomerByVendorId(vendor.Id);
            model.LogoModel = _cacheManager.Get(pictureCacheKey, () =>
            {
                var picture = _pictureService.GetPictureById(customerVendor.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
                var pictureModel = new PictureModel
                {
                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                    Title = string.Format(_localizationService.GetResource("Media.Vendor.ImageLinkTitleFormat"), model.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Vendor.ImageAlternateTextFormat"), model.Name)
                };
                return pictureModel;
            });

            //products
            IList<int> filterableSpecificationAttributeOptionIds;
            var products = _productService.SearchProducts(out filterableSpecificationAttributeOptionIds, true,
                vendorId: vendor.Id,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true);

            model.Products = _productModelFactoryApi.PrepareProductOverviewModels(products).ToList();

            return model;
        }

        /// <summary>
        /// Prepare vendor all models
        /// </summary>
        /// <returns>List of vendor models</returns>
        public virtual List<Models.Vendor.VendorModel> PrepareVendorAllModels()
        {
            var model = new List<Models.Vendor.VendorModel>();
            var vendors = _vendorService.GetAllVendors();
            foreach (var vendor in vendors)
            {
                var vendorModel = new Models.Vendor.VendorModel
                {
                    Id = vendor.Id,
                    Name = vendor.GetLocalized(x => x.Name),
                    Description = vendor.GetLocalized(x => x.Description),
                    MetaKeywords = vendor.GetLocalized(x => x.MetaKeywords),
                    MetaDescription = vendor.GetLocalized(x => x.MetaDescription),
                    MetaTitle = vendor.GetLocalized(x => x.MetaTitle),
                    SeName = vendor.GetSeName(),
                    AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors
                };

                //prepare picture model
                int pictureSize = _mediaSettings.VendorThumbPictureSize;
                var pictureCacheKey = string.Format(ModelCacheEventConsumer.VENDOR_PICTURE_MODEL_KEY, vendor.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                vendorModel.PictureModel = _cacheManager.Get(pictureCacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(vendor.PictureId);
                    var pictureModel = new Models.DashboardModel.PictureModel
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Vendor.ImageLinkTitleFormat"), vendorModel.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Vendor.ImageAlternateTextFormat"), vendorModel.Name)
                    };
                    return pictureModel;
                });

                var customerVendor = _customerServiceApi.GetCustomerByVendorId(vendor.Id);
                vendorModel.LogoModel = _cacheManager.Get(pictureCacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(customerVendor.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
                    var pictureModel = new Models.DashboardModel.PictureModel
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Vendor.ImageLinkTitleFormat"), vendorModel.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Vendor.ImageAlternateTextFormat"), vendorModel.Name)
                    };
                    return pictureModel;
                });

                model.Add(vendorModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare vendor navigation model
        /// </summary>
        /// <returns>Vendor navigation model</returns>
        public virtual Models.Vendor.VendorNavigationModel PrepareVendorNavigationModel()
        {
            string cacheKey = ModelCacheEventConsumer.VENDOR_NAVIGATION_MODEL_KEY;
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var vendors = _vendorService.GetAllVendors(pageSize: _vendorSettings.VendorsBlockItemsToDisplay);
                var model = new Models.Vendor.VendorNavigationModel
                {
                    TotalVendors = vendors.TotalCount
                };

                foreach (var vendor in vendors)
                {
                    model.Vendors.Add(new Models.Vendor.VendorBriefInfoModel
                    {
                        Id = vendor.Id,
                        Name = vendor.GetLocalized(x => x.Name),
                        SeName = vendor.GetSeName(),
                    });
                }
                return model;
            });

            return cachedModel;
        }

        #endregion

        #region CategoriesAndManufacturers
        public virtual CategoriesAndManufacturersModelApi PrepareCategoriesAndManufacturersModel()
        {
            var model = new CategoriesAndManufacturersModelApi();

            string cacheKey = string.Format(ModelCacheEventConsumer.SEARCH_CATEGORIES_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);

            var categories = _cacheManager.Get(cacheKey, () =>
            {
                var categoriesModel = new List<CatalogSearchyModelApi>();
                //all categories
                var allCategories = _categoryService.GetAllCategories(storeId: _storeContext.CurrentStore.Id);
                foreach (var c in allCategories)
                {
                    categoriesModel.Add(new CatalogSearchyModelApi
                    {
                        Id = c.Id,
                        Name = c.Name
                    });
                }
                return categoriesModel;
            });

            if (categories.Any())
            {
                foreach (var category in categories)
                {
                    model.Categories.Add(new CatalogSearchyModelApi
                    {
                        Id = category.Id,
                        Name = category.Name
                    });
                }
            }

            var manufacturers = _manufacturerService.GetAllManufacturers(storeId: _storeContext.CurrentStore.Id);

            if (manufacturers.Any())
            {
                foreach (var manufacturer in manufacturers)
                {
                    model.Manufacturers.Add(new CatalogSearchyModelApi
                    {
                        Id = manufacturer.Id,
                        Name = manufacturer.Name
                    });
                }
            }

            return model;
        }
        #endregion
    }
}

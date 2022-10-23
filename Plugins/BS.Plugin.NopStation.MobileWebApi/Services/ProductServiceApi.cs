using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Nop.Data;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public class ProductServiceApi : IProductServiceApi
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly CatalogSettings _catalogSettings;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly IRepository<BS_FeaturedProducts> _featuredProdRepository;
        private readonly IDbContext _dbContext;

        public ProductServiceApi(IRepository<Product> productRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            CatalogSettings catalogSettings,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
             ILanguageService languageService,
            IWorkContext workContext,
            IRepository<BS_FeaturedProducts> featuredProdRepository,
            IDbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._productRepository = productRepository;
            this._localizedPropertyRepository = localizedPropertyRepository;
            this._catalogSettings = catalogSettings;
            this._aclRepository = aclRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._languageService = languageService;
            this._workContext = workContext;
            this._featuredProdRepository = featuredProdRepository;
        }


        public List<CheckoutAttribute> getAttributeLit(int productId, string attributeName)
        {
            string query = @"SELECT
	                            PAV.Name,
	                            PAv.Id
                            FROM
	                            dbo.Product AS P
	                            INNER JOIN dbo.Product_ProductAttribute_Mapping AS PPAM ON PPAM.ProductId = P.Id
	                            INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = PPAM.ProductAttributeId
	                            INNER JOIN dbo.ProductAttributeValue AS PAV ON PAV.ProductAttributeMappingId = PPAM.Id
                            WHERE
	                            p.id = " + productId + @"
	                            AND pa.Name LIKE N'%" + attributeName + @"%'";
            return _dbContext.SqlQuery<CheckoutAttribute>(query, new object[0]).AsEnumerable<CheckoutAttribute>().ToList();
        }
        public List<PostService> ListOfService()
        {
            string query = @"SELECT
	                            DISTINCT C.Name AS ServiceName
	                            ,C.Id AS ServiceId
                            FROM
	                            dbo.Category AS C
	                            INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.CategoryId = C.Id
	                            INNER JOIN dbo.StoreMapping AS SM ON sm.EntityId = C.Id AND sm.EntityName ='Category'
	                            INNER JOIN dbo.Tb_CategoryInfo AS TCI ON TCI.CategoryId = C.Id
                            WHERE	
	                             C.Deleted = 0
	                            AND C.Published = 1
	                            AND SM.StoreId = 5
                            ORDER BY C.Name";
            return _dbContext.SqlQuery<PostService>(query, new object[0]).AsEnumerable<PostService>().ToList();
        }
        public int? GetPostType(int categoryId)
        {
            string query = @"SELECT
	                            CPT.PostType
                            FROM
	                            dbo.CateguryPostType AS CPT
                            WHERE
	                            CPT.CateguryId = " + categoryId;
            return _dbContext.SqlQuery<int>(query, new object[0]).FirstOrDefault();
        }
        public PriceRange SearchProductsPrice(int pageIndex = 0,
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
            bool? overridePublished = null)
        {
            bool searchLocalizedValue = false;
            if (languageId > 0)
            {
                if (showHidden)
                {
                    searchLocalizedValue = true;
                }
                else
                {
                    //ensure that we have at least two published languages
                    var totalPublishedLanguages = _languageService.GetAllLanguages().Count;
                    searchLocalizedValue = totalPublishedLanguages >= 2;
                }
            }

            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            //Access control list. Allowed customer roles
            var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
            var query = _productRepository.Table;
            query = query.Where(p => !p.Deleted);
            if (!overridePublished.HasValue)
            {
                //process according to "showHidden"
                if (!showHidden)
                {
                    query = query.Where(p => p.Published);
                }
            }
            else if (overridePublished.Value)
            {
                //published only
                query = query.Where(p => p.Published);
            }
            else if (!overridePublished.Value)
            {
                //unpublished only
                query = query.Where(p => !p.Published);
            }
            if (visibleIndividuallyOnly)
            {
                query = query.Where(p => p.VisibleIndividually);
            }
            if (productType.HasValue)
            {
                var productTypeId = (int)productType.Value;
                query = query.Where(p => p.ProductTypeId == productTypeId);
            }



            //searching by keyword
            if (!String.IsNullOrWhiteSpace(keywords))
            {
                query = from p in query
                        join lp in _localizedPropertyRepository.Table on p.Id equals lp.EntityId into p_lp
                        from lp in p_lp.DefaultIfEmpty()
                        from pt in p.ProductTags.DefaultIfEmpty()
                        where (p.Name.Contains(keywords)) ||
                              (searchDescriptions && p.ShortDescription.Contains(keywords)) ||
                              (searchDescriptions && p.FullDescription.Contains(keywords)) ||
                              (searchProductTags && pt.Name.Contains(keywords)) ||
                              //localized values
                              (searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Product" && lp.LocaleKey == "Name" && lp.LocaleValue.Contains(keywords)) ||
                              (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Product" && lp.LocaleKey == "ShortDescription" && lp.LocaleValue.Contains(keywords)) ||
                              (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Product" && lp.LocaleKey == "FullDescription" && lp.LocaleValue.Contains(keywords))
                        select p;
            }

            if (!showHidden && !_catalogSettings.IgnoreAcl)
            {
                //ACL (access control list)
                query = from p in query
                        join acl in _aclRepository.Table
                        on new { c1 = p.Id, c2 = "Product" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into p_acl
                        from acl in p_acl.DefaultIfEmpty()
                        where !p.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select p;
            }

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                //Store mapping
                query = from p in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = p.Id, c2 = "Product" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into p_sm
                        from sm in p_sm.DefaultIfEmpty()
                        where !p.LimitedToStores || storeId == sm.StoreId
                        select p;
            }

            //search by specs
            if (filteredSpecs != null && filteredSpecs.Count > 0)
            {
                query = from p in query
                        where !filteredSpecs
                                   .Except(
                                       p.ProductSpecificationAttributes.Where(psa => psa.AllowFiltering).Select(
                                           psa => psa.SpecificationAttributeOptionId))
                                   .Any()
                        select p;
            }

            //category filtering
            if (categoryIds != null && categoryIds.Count > 0)
            {
                query = from p in query
                        from pc in p.ProductCategories.Where(pc => categoryIds.Contains(pc.CategoryId))
                        where (!featuredProducts.HasValue || featuredProducts.Value == pc.IsFeaturedProduct)
                        select p;
            }

            //manufacturer filtering
            if (manufacturerId > 0)
            {
                query = from p in query
                        from pm in p.ProductManufacturers.Where(pm => pm.ManufacturerId == manufacturerId)
                        where (!featuredProducts.HasValue || featuredProducts.Value == pm.IsFeaturedProduct)
                        select p;
            }

            //vendor filtering
            if (vendorId > 0)
            {
                query = query.Where(p => p.VendorId == vendorId);
            }

            //warehouse filtering
            if (warehouseId > 0)
            {
                var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;
                query = query.Where(p =>
                    //"Use multiple warehouses" enabled
                    //we search in each warehouse
                    (p.ManageInventoryMethodId == manageStockInventoryMethodId &&
                     p.UseMultipleWarehouses &&
                     p.ProductWarehouseInventory.Any(pwi => pwi.WarehouseId == warehouseId))
                    ||
                    //"Use multiple warehouses" disabled
                    //we use standard "warehouse" property
                    ((p.ManageInventoryMethodId != manageStockInventoryMethodId ||
                      !p.UseMultipleWarehouses) &&
                      p.WarehouseId == warehouseId));
            }

            //related products filtering
            //if (relatedToProductId > 0)
            //{
            //    query = from p in query
            //            join rp in _relatedProductRepository.Table on p.Id equals rp.ProductId2
            //            where (relatedToProductId == rp.ProductId1)
            //            select p;
            //}

            //tag filtering
            if (productTagId > 0)
            {
                query = from p in query
                        from pt in p.ProductTags.Where(pt => pt.Id == productTagId)
                        select p;
            }

            //only distinct products (group by ID)
            //if we use standard Distinct() method, then all fields will be compared (low performance)
            //it'll not work in SQL Server Compact when searching products by a keyword)
            query = from p in query
                    group p by p.Id
                        into pGroup
                    orderby pGroup.Key
                    select pGroup.FirstOrDefault();
            decimal? maxPrice = 0;
            decimal? minPrice = 0;
            try
            {
                maxPrice = query.Max(p => p.Price);
                minPrice = query.Min(p => p.Price);
            }
            catch (Exception)
            {
                maxPrice = 0;
                minPrice = 0;
            }
            var price = new PriceRange()
            {
                From = minPrice,
                To = maxPrice
            };
            return price;
            // var products = new PagedList<Product>(query, pageIndex, pageSize);

            //get filterable specification attribute option identifier
            //if (loadFilterableSpecificationAttributeOptionIds)
            //{
            //    var querySpecs = from p in query
            //                     join psa in _productSpecificationAttributeRepository.Table on p.Id equals psa.ProductId
            //                     where psa.AllowFiltering
            //                     select psa.SpecificationAttributeOptionId;
            //    //only distinct attributes
            //    filterableSpecificationAttributeOptionIds = querySpecs
            //        .Distinct()
            //        .ToList();
            //}

            //return products

        }



        public IList<Product> GetProductsByCategoryId(List<int> categoryIds, int itemsNumber)
        {
            var query = _productRepository.Table;
            query = (from p in query
                     from pc in p.ProductCategories.Where(pc => categoryIds.Contains(pc.CategoryId))
                     orderby pc.IsFeaturedProduct descending
                     select p).Take(itemsNumber);
            return query.ToList();//where (pc.IsFeaturedProduct.Equals(true))
        }

        public IList<Product> GetProductsOnlyOfParentCategory(int categoryId, int itemsNumber)
        {
            var query = _productRepository.Table;
            query = (from p in query
                     from pc in p.ProductCategories.Where(pc => pc.CategoryId == categoryId)
                     orderby pc.IsFeaturedProduct descending
                     select p).Take(itemsNumber);
            return query.ToList();
        }

        public IList<int> GetPreviousAndNextProducts(int categoryId, int productId)
        {
            var query = _productRepository.Table;
            query = from p in query
                    from pc in p.ProductCategories.Where(pc => pc.CategoryId == categoryId)
                    select p;
            query = query.OrderBy(p => p.ProductCategories.FirstOrDefault(pc => pc.CategoryId == categoryId).DisplayOrder);
            var queryIds = query.Select(p => p.Id);
            var nextProduct = queryIds.FirstOrDefault(p => p > productId);
            var prevProduct = queryIds.FirstOrDefault(p => p < productId);

            return new List<int>()
            {
                nextProduct,
                prevProduct
            };
        }

        public IList<BS_FeaturedProducts> GetFeaturedProducts()
        {
            var query = from p in _featuredProdRepository.Table
                        select p;

            var products = query.ToList();
            return products;
        }

        /// <summary>
        /// Search products
        /// </summary>
        /// <param name="filterableSpecificationAttributeOptionIds">The specification attribute option identifiers applied to loaded products (all pages)</param>
        /// <param name="loadFilterableSpecificationAttributeOptionIds">A value indicating whether we should load the specification attribute option identifiers applied to loaded products (all pages)</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier; 0 to load all records</param>
        /// <param name="productType">Product type; 0 to load all records</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only products marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="markedAsNewOnly">A values indicating whether to load only products marked as "new"; "false" to load all records; "true" to load "marked as new" only</param>
        /// <param name="featuredProducts">A value indicating whether loaded products are marked as featured (relates only to categories and manufacturers). 0 to load featured products only, 1 to load not featured products only, null to load all products</param>
        /// <param name="priceMin">Minimum price; null to load all records</param>
        /// <param name="priceMax">Maximum price; null to load all records</param>
        /// <param name="productTagId">Product tag identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in product descriptions</param>
        /// <param name="searchSku">A value indicating whether to search by a specified "keyword" in product SKU</param>
        /// <param name="searchProductTags">A value indicating whether to search by a specified "keyword" in product tags</param>
        /// <param name="languageId">Language identifier (search for text searching)</param>
        /// <param name="filteredSpecs">Filtered product specification identifiers</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" products
        /// false - load only "Unpublished" products
        /// </param>
        /// <returns>Products</returns>
        public IPagedList<Product> SearchOnSaleProducts(
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
            bool hasDiscountApplied = true)
        {
            //search by keyword
            bool searchLocalizedValue = false;
            if (languageId > 0)
            {
                if (showHidden)
                {
                    searchLocalizedValue = true;
                }
                else
                {
                    //ensure that we have at least two published languages
                    var totalPublishedLanguages = _languageService.GetAllLanguages().Count;
                    searchLocalizedValue = totalPublishedLanguages >= 2;
                }
            }

            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            //Access control list. Allowed customer roles
            var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();

            #region Search products

            //products
            var query = _productRepository.Table;
            query = query.Where(p => !p.Deleted);
            if (!overridePublished.HasValue)
            {
                //process according to "showHidden"
                if (!showHidden)
                {
                    query = query.Where(p => p.Published);
                }
            }
            else if (overridePublished.Value)
            {
                //published only
                query = query.Where(p => p.Published);
            }
            else if (!overridePublished.Value)
            {
                //unpublished only
                query = query.Where(p => !p.Published);
            }
            if (visibleIndividuallyOnly)
            {
                query = query.Where(p => p.VisibleIndividually);
            }
            //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
            //That's why we pass the date value
            var nowUtc = DateTime.UtcNow;
            if (markedAsNewOnly)
            {
                query = query.Where(p => p.MarkAsNew);
                query = query.Where(p =>
                    (!p.MarkAsNewStartDateTimeUtc.HasValue || p.MarkAsNewStartDateTimeUtc.Value < nowUtc) &&
                    (!p.MarkAsNewEndDateTimeUtc.HasValue || p.MarkAsNewEndDateTimeUtc.Value > nowUtc));
            }
            if (productType.HasValue)
            {
                var productTypeId = (int)productType.Value;
                query = query.Where(p => p.ProductTypeId == productTypeId);
            }

            if (priceMin.HasValue)
            {
                //min price
                //query = query.Where(p =>
                //    //special price (specified price and valid date range)
                //                    ((p.SpecialPrice.HasValue &&
                //                      ((!p.SpecialPriceStartDateTimeUtc.HasValue ||
                //                        p.SpecialPriceStartDateTimeUtc.Value < nowUtc) &&
                //                       (!p.SpecialPriceEndDateTimeUtc.HasValue ||
                //                        p.SpecialPriceEndDateTimeUtc.Value > nowUtc))) &&
                //                     (p.SpecialPrice >= priceMin.Value))
                //                    ||
                //                        //regular price (price isn't specified or date range isn't valid)
                //                    ((!p.SpecialPrice.HasValue ||
                //                      ((p.SpecialPriceStartDateTimeUtc.HasValue &&
                //                        p.SpecialPriceStartDateTimeUtc.Value > nowUtc) ||
                //                       (p.SpecialPriceEndDateTimeUtc.HasValue &&
                //                        p.SpecialPriceEndDateTimeUtc.Value < nowUtc))) &&
                //                     (p.Price >= priceMin.Value)));

                query = query.Where(p => p.Price >= priceMin.Value);
            }
            if (priceMax.HasValue)
            {
                //max price
                //query = query.Where(p =>
                //    //special price (specified price and valid date range)
                //                    ((p.SpecialPrice.HasValue &&
                //                      ((!p.SpecialPriceStartDateTimeUtc.HasValue ||
                //                        p.SpecialPriceStartDateTimeUtc.Value < nowUtc) &&
                //                       (!p.SpecialPriceEndDateTimeUtc.HasValue ||
                //                        p.SpecialPriceEndDateTimeUtc.Value > nowUtc))) &&
                //                     (p.SpecialPrice <= priceMax.Value))
                //                    ||
                //                        //regular price (price isn't specified or date range isn't valid)
                //                    ((!p.SpecialPrice.HasValue ||
                //                      ((p.SpecialPriceStartDateTimeUtc.HasValue &&
                //                        p.SpecialPriceStartDateTimeUtc.Value > nowUtc) ||
                //                       (p.SpecialPriceEndDateTimeUtc.HasValue &&
                //                        p.SpecialPriceEndDateTimeUtc.Value < nowUtc))) &&
                //                     (p.Price <= priceMax.Value)));

                query = query.Where(p => p.Price <= priceMax.Value);
            }
            if (!showHidden)
            {
                //available dates
                query = query.Where(p =>
                    (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < nowUtc) &&
                    (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > nowUtc));
            }

            //searching by keyword
            if (!String.IsNullOrWhiteSpace(keywords))
            {
                query = from p in query
                        join lp in _localizedPropertyRepository.Table on p.Id equals lp.EntityId into p_lp
                        from lp in p_lp.DefaultIfEmpty()
                        from pt in p.ProductTags.DefaultIfEmpty()
                        where (p.Name.Contains(keywords)) ||
                              (searchDescriptions && p.ShortDescription.Contains(keywords)) ||
                              (searchDescriptions && p.FullDescription.Contains(keywords)) ||
                              (searchProductTags && pt.Name.Contains(keywords)) ||
                              //localized values
                              (searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Product" && lp.LocaleKey == "Name" && lp.LocaleValue.Contains(keywords)) ||
                              (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Product" && lp.LocaleKey == "ShortDescription" && lp.LocaleValue.Contains(keywords)) ||
                              (searchDescriptions && searchLocalizedValue && lp.LanguageId == languageId && lp.LocaleKeyGroup == "Product" && lp.LocaleKey == "FullDescription" && lp.LocaleValue.Contains(keywords))
                        select p;
            }

            if (!showHidden && !_catalogSettings.IgnoreAcl)
            {
                //ACL (access control list)
                query = from p in query
                        join acl in _aclRepository.Table
                        on new { c1 = p.Id, c2 = "Product" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into p_acl
                        from acl in p_acl.DefaultIfEmpty()
                        where !p.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                        select p;
            }

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                //Store mapping
                query = from p in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = p.Id, c2 = "Product" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into p_sm
                        from sm in p_sm.DefaultIfEmpty()
                        where !p.LimitedToStores || storeId == sm.StoreId
                        select p;
            }

            //search by specs
            if (filteredSpecs != null && filteredSpecs.Count > 0)
            {
                query = from p in query
                        where !filteredSpecs
                                   .Except(
                                       p.ProductSpecificationAttributes.Where(psa => psa.AllowFiltering).Select(
                                           psa => psa.SpecificationAttributeOptionId))
                                   .Any()
                        select p;
            }

            //category filtering
            if (categoryIds != null && categoryIds.Count > 0)
            {
                query = from p in query
                        from pc in p.ProductCategories.Where(pc => categoryIds.Contains(pc.CategoryId))
                        where (!featuredProducts.HasValue || featuredProducts.Value == pc.IsFeaturedProduct)
                        select p;
            }

            //manufacturer filtering
            if (manufacturerId > 0)
            {
                query = from p in query
                        from pm in p.ProductManufacturers.Where(pm => pm.ManufacturerId == manufacturerId)
                        where (!featuredProducts.HasValue || featuredProducts.Value == pm.IsFeaturedProduct)
                        select p;
            }

            //vendor filtering
            if (vendorId > 0)
            {
                query = query.Where(p => p.VendorId == vendorId);
            }

            //warehouse filtering
            if (warehouseId > 0)
            {
                var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;
                query = query.Where(p =>
                    //"Use multiple warehouses" enabled
                    //we search in each warehouse
                    (p.ManageInventoryMethodId == manageStockInventoryMethodId &&
                     p.UseMultipleWarehouses &&
                     p.ProductWarehouseInventory.Any(pwi => pwi.WarehouseId == warehouseId))
                    ||
                    //"Use multiple warehouses" disabled
                    //we use standard "warehouse" property
                    ((p.ManageInventoryMethodId != manageStockInventoryMethodId ||
                      !p.UseMultipleWarehouses) &&
                      p.WarehouseId == warehouseId));
            }

            //related products filtering
            //if (relatedToProductId > 0)
            //{
            //    query = from p in query
            //            join rp in _relatedProductRepository.Table on p.Id equals rp.ProductId2
            //            where (relatedToProductId == rp.ProductId1)
            //            select p;
            //}

            //tag filtering
            if (productTagId > 0)
            {
                query = from p in query
                        from pt in p.ProductTags.Where(pt => pt.Id == productTagId)
                        select p;
            }

            // On Sale Filtering

            if (hasTierPrice && hasDiscountApplied)
            {
                query = from p in query
                        where (/*p.SpecialPrice.HasValue &&*/ (!p.MarkAsNewStartDateTimeUtc.HasValue || p.MarkAsNewStartDateTimeUtc.Value < nowUtc) &&
                        (!p.MarkAsNewEndDateTimeUtc.HasValue || p.MarkAsNewEndDateTimeUtc.Value > nowUtc)) || p.HasTierPrices || p.HasDiscountsApplied
                        select p;
            }
            else
            {
                DateTime weekUtc = nowUtc.AddDays(7);
                query = from p in query
                        where (/*p.SpecialPrice.HasValue &&*/ (!p.MarkAsNewStartDateTimeUtc.HasValue || p.MarkAsNewStartDateTimeUtc.Value < weekUtc) &&
                        (!p.MarkAsNewEndDateTimeUtc.HasValue || p.MarkAsNewEndDateTimeUtc.Value > nowUtc))
                        select p;
            }

            //only distinct products (group by ID)
            //if we use standard Distinct() method, then all fields will be compared (low performance)
            //it'll not work in SQL Server Compact when searching products by a keyword)
            query = from p in query
                    group p by p.Id
                        into pGroup
                    orderby pGroup.Key
                    select pGroup.FirstOrDefault();

            //sort products
            if (orderBy == ProductSortingEnum.Position && categoryIds != null && categoryIds.Count > 0)
            {
                //category position
                var firstCategoryId = categoryIds[0];
                query = query.OrderBy(p => p.ProductCategories.FirstOrDefault(pc => pc.CategoryId == firstCategoryId).DisplayOrder);
            }
            else if (orderBy == ProductSortingEnum.Position && manufacturerId > 0)
            {
                //manufacturer position
                query =
                    query.OrderBy(p => p.ProductManufacturers.FirstOrDefault(pm => pm.ManufacturerId == manufacturerId).DisplayOrder);
            }
            else if (orderBy == ProductSortingEnum.Position)
            {
                //otherwise sort by name
                query = query.OrderBy(p => p.Name);
            }
            else if (orderBy == ProductSortingEnum.NameAsc)
            {
                //Name: A to Z
                query = query.OrderBy(p => p.Name);
            }
            else if (orderBy == ProductSortingEnum.NameDesc)
            {
                //Name: Z to A
                query = query.OrderByDescending(p => p.Name);
            }
            else if (orderBy == ProductSortingEnum.PriceAsc)
            {
                //Price: Low to High
                query = query.OrderBy(p => p.Price);
            }
            else if (orderBy == ProductSortingEnum.PriceDesc)
            {
                //Price: High to Low
                query = query.OrderByDescending(p => p.Price);
            }
            else if (orderBy == ProductSortingEnum.CreatedOn)
            {
                //creation date
                query = query.OrderByDescending(p => p.CreatedOnUtc);
            }
            else
            {
                //actually this code is not reachable
                query = query.OrderBy(p => p.Name);
            }



            var products = new PagedList<Product>(query, pageIndex, pageSize);

            //return products
            return products;

            #endregion
        }

        public virtual IList<Product> GetTopDealsProductsByIds(int[] productIds)
        {
            if (productIds == null || productIds.Length == 0)
                return new List<Product>();

            var query = from p in _productRepository.Table
                        where productIds.Contains(p.Id)
                        select p;

            var nowUtc = DateTime.UtcNow;
            query = from p in query
                    where (/*p.SpecialPrice.HasValue &&*/ (!p.MarkAsNewStartDateTimeUtc.HasValue || p.MarkAsNewStartDateTimeUtc.Value < nowUtc) &&
                    (!p.MarkAsNewEndDateTimeUtc.HasValue || p.MarkAsNewEndDateTimeUtc.Value > nowUtc)) || p.HasTierPrices || p.HasDiscountsApplied
                    select p;

            var products = query.ToList();
            //sort by passed identifiers
            var sortedProducts = new List<Product>();
            foreach (int id in productIds)
            {
                var product = products.Find(x => x.Id == id);
                if (product != null)
                    sortedProducts.Add(product);
            }
            return sortedProducts;
        }
    }
    public class PostService
    {
        public string ServiceName { get; set; }
        public int ServiceId { get; set; }
    }

    public class CheckoutAttribute
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

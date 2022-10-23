using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Controllers
{
    public class ManageProductController :  BasePublicController
    {
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly ICompareProductsService _compareProductsService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IOrderService _orderService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly CatalogSettings _catalogSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;


        public ManageProductController(
            IProductModelFactory productModelFactory, 
            IProductService productService,
            IWorkContext workContext, 
            IStoreContext storeContext, 
            ILocalizationService localizationService, 
            IWebHelper webHelper, 
            IRecentlyViewedProductsService recentlyViewedProductsService, 
            ICompareProductsService compareProductsService, 
            IWorkflowMessageService workflowMessageService, 
            IOrderService orderService,
            IAclService aclService, 
            IStoreMappingService storeMappingService, 
            IPermissionService permissionService, 
            ICustomerActivityService customerActivityService, 
            IEventPublisher eventPublisher, 
            CatalogSettings catalogSettings, 
            ShoppingCartSettings shoppingCartSettings, 
            LocalizationSettings localizationSettings, 
            CaptchaSettings captchaSettings) 
            
        {
            _productModelFactory = productModelFactory;
            _productService = productService;
            _workContext = workContext;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _compareProductsService = compareProductsService;
            _workflowMessageService = workflowMessageService;
            _orderService = orderService;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _permissionService = permissionService;
            _customerActivityService = customerActivityService;
            _eventPublisher = eventPublisher;
            _catalogSettings = catalogSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _localizationSettings = localizationSettings;
            _captchaSettings = captchaSettings;
        }

        public  IActionResult ProductDetail(int productId, int updatecartitemid = 0)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                (!product.Published && !_catalogSettings.AllowViewUnpublishedProductPage) ||
                //ACL (access control list) 
                !_aclService.Authorize(product) ||
                //Store mapping
                !_storeMappingService.Authorize(product) ||
                //availability dates
                !product.IsAvailable();
            //Check whether the current user has a "Manage products" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            if (notAvailable && !_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return InvokeHttp404();

            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct == null)
                    return RedirectToRoute("HomePage");

                return RedirectToRoute("Product", new { SeName = parentGroupedProduct.GetSeName() });
            }

            //update existing shopping cart or wishlist  item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return RedirectToRoute("Product", new { SeName = product.GetSeName() });
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    return RedirectToRoute("Product", new { SeName = product.GetSeName() });
                }
            }

            //save as recently viewed
            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            ////display "edit" (manage) link
            //if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) &&
            //    _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
            //{
            //    //a vendor should have access only to his products
            //    if (_workContext.CurrentVendor == null || _workContext.CurrentVendor.Id == product.VendorId)
            //    {
            //        DisplayEditLink(Url.Action("Edit", "Product", new { id = product.Id, area = AreaNames.Admin }));
            //    }
            //}

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewProduct", _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name);

            //model
            var model = _productModelFactory.PrepareProductDetailsModel(product, updatecartitem, false);
            
            return View("/Plugins/Misc.PrintedReports_Requirements/Views/Product/ProductTemplate.Simple.cshtml", model);
        }


        public  IActionResult CustomerProductReviews(int? pageNumber)
        {
            if (_workContext.CurrentCustomer.IsGuest())
                return Challenge();

            if (!_catalogSettings.ShowProductReviewsTabOnAccountPage)
            {
                return RedirectToRoute("CustomerInfo");
            }

            var model = _productModelFactory.PrepareCustomerProductReviewsModel(pageNumber);
            return View("/Plugins/Misc.PrintedReports_Requirements/Views/Product/CustomerProductReviews.cshtml", model);
        }

        [HttpsRequirement(SslRequirement.No)]
        public  IActionResult ProductReviews(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return RedirectToRoute("HomePage");

            var model = new ProductReviewsModel();
            model = _productModelFactory.PrepareProductReviewsModel(model, product);
            //only registered users can leave reviews
            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));

            if (_catalogSettings.ProductReviewPossibleOnlyAfterPurchasing)
            {
                var hasCompletedOrders = _orderService.SearchOrders(customerId: _workContext.CurrentCustomer.Id,
                    productId: productId,
                    osIds: new List<int> { (int)OrderStatus.Complete },
                    pageSize: 1).Any();
                if (!hasCompletedOrders)
                    ModelState.AddModelError(string.Empty, _localizationService.GetResource("Reviews.ProductReviewPossibleOnlyAfterPurchasing"));
            }

            //default value
            model.AddProductReview.Rating = _catalogSettings.DefaultProductRatingValue;
            return View("/Plugins/Misc.PrintedReports_Requirements/Views/Product/ProductReviews.cshtml", model);
        }
    }
}

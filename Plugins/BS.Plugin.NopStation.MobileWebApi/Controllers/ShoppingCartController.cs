using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Factories;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ShoppingCart;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Media;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class ShoppingCartController : BaseApiController
    {
        //#region Field

        //private readonly IShoppingCartModelFactoryApi _shoppingCartModelFactoryApi;
        //private readonly IProductService _productService;
        //private readonly ShoppingCartSettings _shoppingCartSettings;
        //private readonly IWorkContext _workContext;
        //private readonly IStoreContext _storeContext;
        //private readonly ICurrencyService _currencyService;
        //private readonly IShoppingCartService _shoppingCartService;
        //private readonly ICustomerService _customerService;
        //private readonly ILocalizationService _localizationService;
        //private readonly IProductAttributeService _productAttributeService;
        //private readonly IProductAttributeParser _productAttributeParser;
        //private readonly IDownloadService _downloadService;
        //private readonly IPriceCalculationService _priceCalculationService;
        //private readonly IPriceFormatter _priceFormatter;
        //private readonly ITaxService _taxService;
        //private readonly ICheckoutAttributeService _checkoutAttributeService;
        //private readonly IPermissionService _permissionService;
        //private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        //private readonly IProductAttributeFormatter _productAttributeFormatter;
        //private readonly IGenericAttributeService _genericAttributeService;
        //private readonly IPictureService _pictureService;
        //private readonly IWebHelper _webHelper;
        //private readonly ICacheManager _cacheManager;
        //private readonly MediaSettings _mediaSettings;
        //private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        //private readonly IGiftCardService _giftCardService;
        //private readonly IDiscountService _discountService;
        //private readonly GenericAttributeServiceApi _genericAttributeServiceApi;
        //private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        //private readonly IPaymentService _paymentService;
        //private readonly RewardPointsSettings _rewardPointsSettings;
        //private readonly TaxSettings _taxSettings;
        //private readonly CatalogSettings _catalogSettings;
        //private readonly AddressSettings _addressSettings;
        //private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        //private readonly ShippingSettings _shippingSettings;
        //#endregion

        //#region Ctor
        //public ShoppingCartController(IShoppingCartModelFactoryApi shoppingCartModelFactoryApi,
        //    IProductService productService,
        //    ShoppingCartSettings shoppingCartSettings,
        //    IWorkContext workContext,
        //    IStoreContext storeContext,
        //    ICurrencyService currencyService,
        //    IShoppingCartService shoppingCartService,
        //    ICustomerService customerService,
        //    ILocalizationService localizationService,
        //    IProductAttributeService productAttributeService,
        //    IProductAttributeParser productAttributeParser,
        //    IDownloadService downloadService,
        //    IPriceCalculationService priceCalculationService,
        //    IPriceFormatter priceFormatter,
        //    ITaxService taxService,
        //    ICheckoutAttributeService checkoutAttributeService,
        //    IPermissionService permissionService,
        //    ICheckoutAttributeParser checkoutAttributeParser,
        //    IGenericAttributeService genericAttributeService,
        //    IProductAttributeFormatter productAttributeFormatter,
        //    IPictureService pictureService,
        //    IWebHelper webHelper,
        //     ICacheManager cacheManager,
        //    MediaSettings mediaSettings,
        //    ICheckoutAttributeFormatter checkoutAttributeFormatter,
        //    IGiftCardService giftCardService,
        //    IDiscountService discountService,
        //    IOrderTotalCalculationService orderTotalCalculationService,
        //    //GenericAttributeServiceApi genericAttributeServiceApi,
        //    TaxSettings taxSettings,
        //    IPaymentService paymentService,
        //    RewardPointsSettings rewardPointsSettings,
        //    CatalogSettings catalogSettings,
        //    AddressSettings addressSettings,
        //    IAddressAttributeFormatter addressAttributeFormatter,
        //    ShippingSettings shippingSettings
        //    )
        //{
        //    this._shoppingCartModelFactoryApi = shoppingCartModelFactoryApi;
        //    this._productService = productService;
        //    this._shoppingCartSettings = shoppingCartSettings;
        //    this._workContext = workContext;
        //    this._storeContext = storeContext;
        //    this._currencyService = currencyService;
        //    this._shoppingCartService = shoppingCartService;
        //    this._customerService = customerService;
        //    this._localizationService = localizationService;
        //    this._productAttributeService = productAttributeService;
        //    this._downloadService = downloadService;
        //    this._productAttributeParser = productAttributeParser;
        //    this._priceCalculationService = priceCalculationService;
        //    this._priceFormatter = priceFormatter;
        //    this._taxService = taxService;
        //    this._checkoutAttributeService = checkoutAttributeService;
        //    this._permissionService = permissionService;
        //    this._checkoutAttributeParser = checkoutAttributeParser;
        //    this._genericAttributeService = genericAttributeService;
        //    this._productAttributeFormatter = productAttributeFormatter;
        //    this._pictureService = pictureService;
        //    this._webHelper = webHelper;
        //    this._cacheManager = cacheManager;
        //    this._mediaSettings = mediaSettings;
        //    this._checkoutAttributeFormatter = checkoutAttributeFormatter;
        //    this._giftCardService = giftCardService;
        //    this._discountService = discountService;
        //    this._orderTotalCalculationService = orderTotalCalculationService;
        //    this._taxSettings = taxSettings;
        //    this._paymentService = paymentService;
        //    this._rewardPointsSettings = rewardPointsSettings;
        //    this._catalogSettings = catalogSettings;
        //    this._shippingSettings = shippingSettings;
        //    this._addressSettings = addressSettings;
        //    this._addressAttributeFormatter = addressAttributeFormatter;
        //    //this._genericAttributeServiceApi = genericAttributeServiceApi;
        //}
        //#endregion

        //#region Utility

        

        ///// <summary>
        ///// Parse product attributes on the product details page
        ///// </summary>
        ///// <param name="product">Product</param>
        ///// <param name="form">Form</param>
        ///// <returns>Parsed attributes</returns>
        //[NonAction]
        //protected virtual string ParseProductAttributes(Product product, NameValueCollection form)
        //{
        //    string attributesXml = "";

        //    #region Product attributes
        //    var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
        //    foreach (var attribute in productAttributes)
        //    {
        //        string controlId = string.Format("product_attribute_{0}_{1}_{2}", attribute.ProductId, attribute.ProductAttributeId, attribute.Id);
        //        switch (attribute.AttributeControlType)
        //        {
        //            case AttributeControlType.DropdownList:
        //            case AttributeControlType.RadioList:
        //            case AttributeControlType.ColorSquares:
        //                {
        //                    var ctrlAttributes = form[controlId];
        //                    if (!String.IsNullOrEmpty(ctrlAttributes))
        //                    {
        //                        int selectedAttributeId = int.Parse(ctrlAttributes);
        //                        if (selectedAttributeId > 0)
        //                            attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
        //                                attribute, selectedAttributeId.ToString());
        //                    }
        //                }
        //                break;
        //            case AttributeControlType.Checkboxes:
        //                {
        //                    var ctrlAttributes = form[controlId];
        //                    if (!String.IsNullOrEmpty(ctrlAttributes))
        //                    {
        //                        foreach (var item in ctrlAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        //                        {
        //                            int selectedAttributeId = int.Parse(item);
        //                            if (selectedAttributeId > 0)
        //                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
        //                                    attribute, selectedAttributeId.ToString());
        //                        }
        //                    }
        //                }
        //                break;
        //            case AttributeControlType.ReadonlyCheckboxes:
        //                {
        //                    //load read-only (already server-side selected) values
        //                    var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
        //                    foreach (var selectedAttributeId in attributeValues
        //                        .Where(v => v.IsPreSelected)
        //                        .Select(v => v.Id)
        //                        .ToList())
        //                    {
        //                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
        //                            attribute, selectedAttributeId.ToString());
        //                    }
        //                }
        //                break;
        //            case AttributeControlType.TextBox:
        //            case AttributeControlType.MultilineTextbox:
        //                {
        //                    var ctrlAttributes = form[controlId];
        //                    if (!String.IsNullOrEmpty(ctrlAttributes))
        //                    {
        //                        string enteredText = ctrlAttributes.Trim();
        //                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
        //                            attribute, enteredText);
        //                    }
        //                }
        //                break;
        //            case AttributeControlType.Datepicker:
        //                {
        //                    var day = form[controlId + "_day"];
        //                    var month = form[controlId + "_month"];
        //                    var year = form[controlId + "_year"];
        //                    DateTime? selectedDate = null;
        //                    try
        //                    {
        //                        selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
        //                    }
        //                    catch { }
        //                    if (selectedDate.HasValue)
        //                    {
        //                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
        //                            attribute, selectedDate.Value.ToString("D"));
        //                    }
        //                }
        //                break;
        //            case AttributeControlType.FileUpload:
        //                {
        //                    Guid downloadGuid;
        //                    Guid.TryParse(form[controlId], out downloadGuid);
        //                    var download = _downloadService.GetDownloadByGuid(downloadGuid);
        //                    if (download != null)
        //                    {
        //                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
        //                                attribute, download.DownloadGuid.ToString());
        //                    }
        //                }
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    #endregion

        //    #region Gift cards

        //    if (product.IsGiftCard)
        //    {
        //        string recipientName = "";
        //        string recipientEmail = "";
        //        string senderName = "";
        //        string senderEmail = "";
        //        string giftCardMessage = "";
        //        foreach (string formKey in form.AllKeys)
        //        {
        //            if (formKey.Equals(string.Format("giftcard_{0}.RecipientName", product.Id), StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                recipientName = form[formKey];
        //                continue;
        //            }
        //            if (formKey.Equals(string.Format("giftcard_{0}.RecipientEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                recipientEmail = form[formKey];
        //                continue;
        //            }
        //            if (formKey.Equals(string.Format("giftcard_{0}.SenderName", product.Id), StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                senderName = form[formKey];
        //                continue;
        //            }
        //            if (formKey.Equals(string.Format("giftcard_{0}.SenderEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                senderEmail = form[formKey];
        //                continue;
        //            }
        //            if (formKey.Equals(string.Format("giftcard_{0}.Message", product.Id), StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                giftCardMessage = form[formKey];
        //                continue;
        //            }
        //        }

        //        attributesXml = _productAttributeParser.AddGiftCardAttribute(attributesXml,
        //            recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
        //    }

        //    #endregion

        //    return attributesXml;
        //}

        //[NonAction]
        //protected virtual void ParseAndSaveCheckoutAttributes(List<ShoppingCartItem> cart, NameValueCollection form)
        //{
        //    if (cart == null)
        //        throw new ArgumentNullException("cart");

        //    if (form == null)
        //        throw new ArgumentNullException("form");

        //    string attributesXml = "";
        //    var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !cart.RequiresShipping());
        //    foreach (var attribute in checkoutAttributes)
        //    {
        //        string controlId = string.Format("checkout_attribute_{0}", attribute.Id);
        //        switch (attribute.AttributeControlType)
        //        {
        //            case AttributeControlType.DropdownList:
        //            case AttributeControlType.RadioList:
        //            case AttributeControlType.ColorSquares:
        //                {
        //                    var ctrlAttributes = form[controlId];
        //                    if (!String.IsNullOrEmpty(ctrlAttributes))
        //                    {
        //                        int selectedAttributeId = int.Parse(ctrlAttributes);
        //                        if (selectedAttributeId > 0)
        //                            attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
        //                                attribute, selectedAttributeId.ToString());
        //                    }
        //                }
        //                break;
        //            case AttributeControlType.Checkboxes:
        //                {
        //                    var cblAttributes = form[controlId];
        //                    if (!String.IsNullOrEmpty(cblAttributes))
        //                    {
        //                        foreach (var item in cblAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        //                        {
        //                            int selectedAttributeId = int.Parse(item);
        //                            if (selectedAttributeId > 0)
        //                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
        //                                    attribute, selectedAttributeId.ToString());
        //                        }
        //                    }
        //                }
        //                break;
        //            case AttributeControlType.ReadonlyCheckboxes:
        //                {
        //                    //load read-only (already server-side selected) values
        //                    var attributeValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
        //                    foreach (var selectedAttributeId in attributeValues
        //                        .Where(v => v.IsPreSelected)
        //                        .Select(v => v.Id)
        //                        .ToList())
        //                    {
        //                        attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
        //                                    attribute, selectedAttributeId.ToString());
        //                    }
        //                }
        //                break;
        //            case AttributeControlType.TextBox:
        //            case AttributeControlType.MultilineTextbox:
        //                {
        //                    var ctrlAttributes = form[controlId];
        //                    if (!String.IsNullOrEmpty(ctrlAttributes))
        //                    {
        //                        string enteredText = ctrlAttributes.Trim();
        //                        attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
        //                            attribute, enteredText);
        //                    }
        //                }
        //                break;
        //            case AttributeControlType.Datepicker:
        //                {
        //                    var date = form[controlId + "_day"];
        //                    var month = form[controlId + "_month"];
        //                    var year = form[controlId + "_year"];
        //                    DateTime? selectedDate = null;
        //                    try
        //                    {
        //                        selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(date));
        //                    }
        //                    catch { }
        //                    if (selectedDate.HasValue)
        //                    {
        //                        attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
        //                            attribute, selectedDate.Value.ToString("D"));
        //                    }
        //                }
        //                break;
        //            case AttributeControlType.FileUpload:
        //                {
        //                    Guid downloadGuid;
        //                    Guid.TryParse(form[controlId], out downloadGuid);
        //                    var download = _downloadService.GetDownloadByGuid(downloadGuid);
        //                    if (download != null)
        //                    {
        //                        attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
        //                                   attribute, download.DownloadGuid.ToString());
        //                    }
        //                }
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    //save checkout attributes
        //    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.CheckoutAttributes, attributesXml, _storeContext.CurrentStore.Id);
        //}

        ///// <summary>
        ///// Parse product rental dates on the product details page
        ///// </summary>
        ///// <param name="product">Product</param>
        ///// <param name="form">Form</param>
        ///// <param name="startDate">Start date</param>
        ///// <param name="endDate">End date</param>
        //[NonAction]
        //protected virtual void ParseRentalDates(Product product, NameValueCollection form,
        //    out DateTime? startDate, out DateTime? endDate)
        //{
        //    startDate = null;
        //    endDate = null;

        //    string startControlId = string.Format("rental_start_date_{0}", product.Id);
        //    string endControlId = string.Format("rental_end_date_{0}", product.Id);
        //    var ctrlStartDate = form[startControlId];
        //    var ctrlEndDate = form[endControlId];
        //    try
        //    {
        //        //currenly we support only this format (as in the \Views\Product\_RentalInfo.cshtml file)
        //        const string datePickerFormat = "MM/dd/yyyy";
        //        startDate = DateTime.ParseExact(ctrlStartDate, datePickerFormat, CultureInfo.InvariantCulture);
        //        endDate = DateTime.ParseExact(ctrlEndDate, datePickerFormat, CultureInfo.InvariantCulture);
        //    }
        //    catch
        //    {
        //    }
        //}


        


        //#endregion

        //#region Action Method

        //[Route("api/AddProductToCart/{productId}/{shoppingCartTypeId}")]
        //[HttpPost]
        //public IActionResult AddProductToCart_Details(int productId, int shoppingCartTypeId, List<KeyValueApi> formValues)
        //{
        //    var form = new NameValueCollection();
        //    foreach (var values in formValues)
        //    {
        //        form.Add(values.Key, values.Value);
        //    }
        //    var errrorList = new List<string>();
        //    var result = new AddProductToCartResponseModel();
        //    var product = _productService.GetProductById(productId);
        //    if (product == null)
        //    {
        //        errrorList.Add("Product Not Found");
        //        result.ErrorList = errrorList;
        //        result.Success = false;
        //        result.StatusCode = (int)ErrorType.NotOk;
        //    }

        //    //we can add only simple products
        //    if (product.ProductType != ProductType.SimpleProduct)
        //    {
        //        errrorList.Add("Only simple products could be added to the cart");
        //        result.ErrorList = errrorList;
        //        result.Success = false;
        //        result.StatusCode = (int)ErrorType.NotOk;
        //    }

        //    #region Update existing shopping cart item?
        //    int updatecartitemid = 0;

        //    foreach (string formKey in form.AllKeys)
        //        if (formKey.Equals(string.Format("addtocart_{0}.UpdatedShoppingCartItemId", productId), StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            int.TryParse(form[formKey], out updatecartitemid);
        //            break;
        //        }
        //    ShoppingCartItem updatecartitem = null;
        //    if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
        //    {
        //        var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //            .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //            .LimitPerStore(_storeContext.CurrentStore.Id)
        //            .ToList();
        //        updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
        //        //not found?
        //        if (updatecartitem == null)
        //        {
        //            errrorList.Add("No shopping cart item found to update");
        //            result.ErrorList = errrorList;
        //            result.Success = false;
        //            result.StatusCode = (int)ErrorType.NotOk;
        //        }
        //        //is it this product?
        //        if (product.Id != updatecartitem.ProductId)
        //        {
        //            errrorList.Add("This product does not match a passed shopping cart item identifier");
        //            result.ErrorList = errrorList;
        //            result.Success = false;
        //            result.StatusCode = (int)ErrorType.NotOk;
        //        }
        //    }
        //    #endregion

        //    #region Customer entered price
        //    decimal customerEnteredPriceConverted = decimal.Zero;
        //    if (product.CustomerEntersPrice)
        //    {
        //        foreach (string formKey in form.AllKeys)
        //        {
        //            if (formKey.Equals(string.Format("addtocart_{0}.CustomerEnteredPrice", productId), StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                decimal customerEnteredPrice;
        //                if (decimal.TryParse(form[formKey], out customerEnteredPrice))
        //                    customerEnteredPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(customerEnteredPrice, _workContext.WorkingCurrency);
        //                break;
        //            }
        //        }
        //    }
        //    #endregion

        //    #region Quantity

        //    int quantity = 1;

        //    foreach (string formKey in form.AllKeys)
        //        if (formKey.Equals(string.Format("addtocart_{0}.EnteredQuantity", productId), StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            int.TryParse(form[formKey], out quantity);
        //            break;
        //        }

        //    #endregion

        //    //product and gift card attributes
        //    string attributes = ParseProductAttributes(product, form);

        //    //rental attributes
        //    DateTime? rentalStartDate = null;
        //    DateTime? rentalEndDate = null;
        //    if (product.IsRental)
        //    {
        //        ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
        //    }

        //    //save item
        //    var addToCartWarnings = new List<string>();
        //    var cartType = (ShoppingCartType)shoppingCartTypeId;
        //    if (updatecartitem == null)
        //    {
        //        //add to the cart
        //        addToCartWarnings.AddRange(_shoppingCartService.AddToCart(_workContext.CurrentCustomer,
        //            product, cartType, _storeContext.CurrentStore.Id,
        //            attributes, customerEnteredPriceConverted,
        //            rentalStartDate, rentalEndDate, quantity, true));
        //    }
        //    else
        //    {
        //        var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //            .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //            .LimitPerStore(_storeContext.CurrentStore.Id)
        //            .ToList();
        //        var otherCartItemWithSameParameters = _shoppingCartService.FindShoppingCartItemInTheCart(
        //            cart, cartType, product, attributes, customerEnteredPriceConverted,
        //            rentalStartDate, rentalEndDate);
        //        if (otherCartItemWithSameParameters != null &&
        //            otherCartItemWithSameParameters.Id == updatecartitem.Id)
        //        {
        //            //ensure it's other shopping cart cart item
        //            otherCartItemWithSameParameters = null;
        //        }
        //        //update existing item
        //        addToCartWarnings.AddRange(_shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
        //            updatecartitem.Id, attributes, customerEnteredPriceConverted,
        //            rentalStartDate, rentalEndDate, quantity, true));
        //        if (otherCartItemWithSameParameters != null && addToCartWarnings.Count == 0)
        //        {
        //            //delete the same shopping cart item (the other one)
        //            _shoppingCartService.DeleteShoppingCartItem(otherCartItemWithSameParameters);
        //        }
        //    }

        //    #region Return result

        //    if (addToCartWarnings.Count > 0)
        //    {
        //        result.ErrorList = addToCartWarnings;
        //        result.Success = false;
        //        result.StatusCode = (int)ErrorType.NotOk;
        //    }
        //    else
        //    {
        //        //added to the cart/wishlist
        //        switch (cartType)
        //        {
        //            case ShoppingCartType.Wishlist:
        //                {
        //                    //activity log
        //                    //_customerActivityService.InsertActivity("PublicStore.AddToWishlist", _localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name);

        //                    if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct)
        //                    {
        //                        result.ForceRedirect = true;
        //                    }

        //                    result.Success = true;
        //                    break;
        //                }
        //            case ShoppingCartType.ShoppingCart:
        //            default:
        //                {
        //                    //activity log
        //                    // _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", _localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name);
        //                    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //                        .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //                        .LimitPerStore(_storeContext.CurrentStore.Id)
        //                        .ToList();
        //                    result.Count = _workContext.CurrentCustomer.ShoppingCartItems
        //                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //                    .LimitPerStore(_storeContext.CurrentStore.Id)
        //                    .ToList()
        //                    .GetTotalProducts();
        //                    if (_shoppingCartSettings.DisplayCartAfterAddingProduct)
        //                    {
        //                        result.ForceRedirect = true;
        //                        result.Success = true;
        //                    }

        //                    result.Success = true;
        //                    break;
        //                }
        //        }
        //    }
        //    return Ok(result);

        //    #endregion
        //}

        ////handle product attribute selection event. this way we return new price, overridden gtin/sku/mpn
        ////currently we use this method on the product details pages
        //[Route("api/ProductDetailsPagePrice/{productId}")]
        //[HttpPost]
        //public IActionResult ProductDetails_AttributeChange(int productId, List<KeyValueApi> formValues)
        //{
        //    var form = new NameValueCollection();
        //    foreach (var values in formValues)
        //    {
        //        form.Add(values.Key, values.Value);
        //    }
        //    var result = new ProductDetailPriceResponseModel();
        //    var errrorList = new List<string>();
        //    var product = _productService.GetProductById(productId);
        //    if (product == null)
        //    {
        //        errrorList.Add("Product Not Found");
        //        result.ErrorList = errrorList;
        //        result.StatusCode = (int)ErrorType.NotOk;
        //        return Ok(result);
        //    }

        //    string attributeXml = ParseProductAttributes(product, form);

        //    //rental attributes
        //    DateTime? rentalStartDate = null;
        //    DateTime? rentalEndDate = null;
        //    if (product.IsRental)
        //    {
        //        ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
        //    }

        //    result.Sku = product.FormatSku(attributeXml, _productAttributeParser);
        //    result.Mpn = product.FormatMpn(attributeXml, _productAttributeParser);
        //    result.Gtin = product.FormatGtin(attributeXml, _productAttributeParser);


        //    string price = "";
        //    if (!product.CustomerEntersPrice)
        //    {
        //        //we do not calculate price of "customer enters price" option is enabled
        //        // Discount scDiscount;
        //        List<DiscountForCaching> scDiscounts; // change 3.8
        //        decimal discountAmount;
        //        decimal finalPrice = _priceCalculationService.GetUnitPrice(product,
        //            _workContext.CurrentCustomer,
        //            ShoppingCartType.ShoppingCart,
        //            1, attributeXml, 0,
        //            rentalStartDate, rentalEndDate,
        //            true, out discountAmount, out scDiscounts);
        //        decimal taxRate;
        //        decimal finalPriceWithDiscountBase = _taxService.GetProductPrice(product, finalPrice, out taxRate);
        //        decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);
        //        price = _priceFormatter.FormatPrice(finalPriceWithDiscount);
        //    }
        //    result.Price = price;
        //    return Ok(result);

        //}

        //[Route("api/ShoppingCart")]
        //[HttpGet]
        //public IActionResult Cart()
        //{
        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();
        //    var model = new ShoppingCartResponseModel();
            
        //    _shoppingCartModelFactoryApi.PrepareShoppingCartModel(model, cart);
        //    model.Count = cart.GetTotalProducts();

        //    model.OrderTotalResponseModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);
        //    return Ok(model);
        //}

        //[Route("api/ShoppingCart/applycheckoutattribute")]
        //[HttpPost]
        //public IActionResult ApplyCheckoutAttribute(List<KeyValueApi> formValues)
        //{
        //    var form = new NameValueCollection();
        //    foreach (var values in formValues)
        //    {
        //        form.Add(values.Key, values.Value);
        //    }

        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();
        //    ParseAndSaveCheckoutAttributes(cart, form);

        //    var model = new GeneralResponseModel<bool>()
        //    {
        //        Data = true
        //    };
        //    return Ok(model);

        //}

        //[Route("api/ShoppingCart/UpdateCart")]
        //[HttpPost]
        //public IActionResult UpdateCart(List<KeyValueApi> formValues)
        //{
        //    var form = new NameValueCollection();
        //    foreach (var values in formValues)
        //    {
        //        form.Add(values.Key, values.Value);
        //    }

        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();

        //    var allIdsToRemove = form["removefromcart"] != null ? form["removefromcart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();

        //    //current warnings <cart item identifier, warnings>
        //    var innerWarnings = new Dictionary<int, IList<string>>();
        //    foreach (var sci in cart)
        //    {
        //        bool remove = allIdsToRemove.Contains(sci.Id);
        //        if (remove)
        //            _shoppingCartService.DeleteShoppingCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);
        //        else
        //        {
        //            foreach (string formKey in form.AllKeys)
        //                if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
        //                {
        //                    int newQuantity;
        //                    if (int.TryParse(form[formKey], out newQuantity))
        //                    {
        //                        var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
        //                            sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
        //                            sci.RentalStartDateUtc, sci.RentalEndDateUtc,
        //                            newQuantity, true);
        //                        innerWarnings.Add(sci.Id, currSciWarnings);
        //                    }
        //                    break;
        //                }
        //        }
        //    }

        //    //updated cart
        //    cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();
        //    var model = new ShoppingCartResponseModel();
        //    _shoppingCartModelFactoryApi.PrepareShoppingCartModel(model, cart);
        //    //update current warnings
        //    model.ErrorList = new List<string>();
        //    foreach (var kvp in innerWarnings)
        //    {
        //        //kvp = <cart item identifier, warnings>
        //        var sciId = kvp.Key;
        //        var warnings = kvp.Value;
        //        //find model
        //        var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
        //        if (sciModel != null)
        //            foreach (var w in warnings)
        //                if (!model.ErrorList.Contains(w))
        //                    model.ErrorList.Add(w);


        //    }
        //    if (model.ErrorList.Count > 0)
        //    {
        //        model.StatusCode = (int)ErrorType.NotOk;
        //    }
        //    model.Count = cart.GetTotalProducts();
        //    model.OrderTotalResponseModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);
        //    return Ok(model);
        //}

        //[Route("api/ShoppingCart/ApplyGiftCard")]
        //[HttpPost]
        //public IActionResult ApplyGiftCard(SingleValue value)
        //{
        //    string giftcardcouponcode = value.Value;
        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();

        //    var model = new CoupontypeResponse();
        //    model.OrderTotalResponseModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);
        //    var errorList = new List<string>();
        //    string error = string.Empty;

        //    if (!cart.IsRecurring())
        //    {
        //        if (!String.IsNullOrWhiteSpace(giftcardcouponcode))
        //        {
        //            var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: giftcardcouponcode).FirstOrDefault();
        //            bool isGiftCardValid = giftCard != null && giftCard.IsGiftCardValid();
        //            if (isGiftCardValid)
        //            {
        //                _workContext.CurrentCustomer.ApplyGiftCardCouponCode(giftcardcouponcode);
        //                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
        //                //model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.Applied");
        //                model.Data = true;
        //            }
        //            else
        //            {
        //                error = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");

        //            }
        //        }
        //        else
        //        {
        //            error = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");

        //        }
        //    }
        //    else
        //    {
        //        error = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.DontWorkWithAutoshipProducts");

        //    }
        //    if (!string.IsNullOrEmpty(error))
        //    {
        //        errorList.Add(error);
        //        model.ErrorList = errorList;
        //        model.Data = false;
        //        model.StatusCode = (int)ErrorType.NotOk;
        //    }
        //    // PrepareShoppingCartModel(model, cart);
        //    return Ok(model);
        //}

        //[Route("api/ShoppingCart/RemoveGiftCard")]
        //[HttpPost]
        //public IActionResult RemoveGiftCardCode(SingleValue value)
        //{
        //    var model = new CoupontypeResponse();
        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();
        //    model.OrderTotalResponseModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);
        //    //get gift card identifier
        //    int giftCardId = Convert.ToInt32(value.Value);
        //    var gc = _giftCardService.GetGiftCardById(giftCardId);
        //    if (gc != null)
        //    {
        //        _workContext.CurrentCustomer.RemoveGiftCardCouponCode(gc.GiftCardCouponCode);
        //        _customerService.UpdateCustomer(_workContext.CurrentCustomer);
        //        model.Data = true;
        //    }
        //    else
        //    {
        //        model.StatusCode = (int)ErrorType.NotOk;
        //        model.Data = false;
        //    }

        //    return Ok(model);
        //}

        //[Route("api/ShoppingCart/ApplyDiscountCoupon")]
        //[HttpPost]
        //public IActionResult ApplyDiscountCoupon(SingleValue value)
        //{
        //    string discountcouponcode = value.Value;
        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();

        //    var model = new CoupontypeResponse();
            

        //    List<string> errorList = new List<string>();

        //    if (!String.IsNullOrWhiteSpace(discountcouponcode))
        //    {
        //        var discounts = _discountService.GetAllDiscountsForCaching(couponCode: discountcouponcode, showHidden: true)
        //            .Where(d => d.RequiresCouponCode)
        //            .ToList();
        //        if (discounts.Any())
        //        {
        //            var userErrors = new List<string>();
        //            var anyValidDiscount = discounts.Any(discount =>
        //            {
        //                var validationResult = _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer, new[] { discountcouponcode });
        //                userErrors.AddRange(validationResult.Errors);

        //                return validationResult.IsValid;
        //            });
        //            if (anyValidDiscount)
        //            {
        //                //valid
        //                _workContext.CurrentCustomer.ApplyDiscountCouponCode(discountcouponcode);
        //                //model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.Applied");
        //                model.Data = true;
        //            }
        //            else
        //            {
        //                if (userErrors.Any())
        //                {
        //                    //some user error
        //                    errorList = userErrors;
        //                    model.Data = false;
        //                }
        //                else
        //                {
        //                    //general error text
        //                    errorList.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
        //                    model.Data = false;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //discount cannot be found
        //            errorList.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
        //            model.Data = false;
        //        }
        //    }
        //    else
        //    {
        //        errorList.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
        //        model.Data = false;
        //    }

        //    if (errorList.Any())
        //    {
        //        model.ErrorList = errorList;
        //        model.Data = false;
        //        model.StatusCode = (int)ErrorType.NotOk;
        //    }
        //    model.OrderTotalResponseModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);
        //    return Ok(model);
        //}

        //[Route("api/ShoppingCart/RemoveDiscountCoupon")]
        //[HttpGet]
        //public IActionResult ExistingDiscounts()
        //{
        //    var model = _shoppingCartModelFactoryApi.PrepareExistingDiscountsModel();

        //    return Ok(model);
        //}

        //[Route("api/ShoppingCart/RemoveDiscountCoupon")]
        //[HttpPost]
        //public IActionResult RemoveDiscountCoupon(SingleValue value)
        //{
        //    //get discount identifier            
        //    int discountId = Convert.ToInt32(value.Value);
        //    var discount = _discountService.GetDiscountById(discountId);
        //    if (discount != null)
        //        _workContext.CurrentCustomer.RemoveDiscountCouponCode(discount.CouponCode);

        //    var model = new CoupontypeResponse();
        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();

        //    model.OrderTotalResponseModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);
        //    model.Data = true;
        //    return Ok(model);
        //}

        //[Route("api/ShoppingCart/OrderTotal")]
        //[HttpGet]
        //public IActionResult OrderTotals()
        //{
        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();
        //    var model = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);
        //    return Ok(model);
        //}

        //[Route("api/shoppingCart/wishlist")]
        //[HttpGet]
        //public IActionResult Wishlist(Guid? customerGuid = null)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
        //        return Challenge();

        //    Customer customer = customerGuid.HasValue ?
        //        _customerService.GetCustomerByGuid(customerGuid.Value)
        //        : _workContext.CurrentCustomer;
        //    if (customer == null)
        //        return Challenge();
        //    var cart = customer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();
        //    var model = new WishlistResponseModel();
        //    _shoppingCartModelFactoryApi.PrepareWishlistModel(model, cart, !customerGuid.HasValue);
        //    return Ok(model);
        //}

        //[Route("api/ShoppingCart/UpdateWishlist")]
        //[HttpPost]
        //public IActionResult UpdateWishlist(List<KeyValueApi> formValues)
        //{
        //    var form = new NameValueCollection();
        //    foreach (var values in formValues)
        //    {
        //        form.Add(values.Key, values.Value);
        //    }
        //    if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
        //        return Challenge();

        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();

        //    var allIdsToRemove = form["removefromcart"] != null
        //        ? form["removefromcart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        //        .Select(int.Parse)
        //        .ToList()
        //        : new List<int>();

        //    //current warnings <cart item identifier, warnings>
        //    var innerWarnings = new Dictionary<int, IList<string>>();
        //    foreach (var sci in cart)
        //    {
        //        bool remove = allIdsToRemove.Contains(sci.Id);
        //        if (remove)
        //            _shoppingCartService.DeleteShoppingCartItem(sci);
        //        else
        //        {
        //            foreach (string formKey in form.AllKeys)
        //                if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
        //                {
        //                    int newQuantity;
        //                    if (int.TryParse(form[formKey], out newQuantity))
        //                    {
        //                        var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
        //                            sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
        //                            sci.RentalStartDateUtc, sci.RentalEndDateUtc,
        //                            newQuantity, true);
        //                        innerWarnings.Add(sci.Id, currSciWarnings);
        //                    }
        //                    break;
        //                }
        //        }
        //    }

        //    //updated wishlist
        //    cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();
        //    var model = new WishlistResponseModel();
        //    _shoppingCartModelFactoryApi.PrepareWishlistModel(model, cart);
        //    //update current warnings
        //    foreach (var kvp in innerWarnings)
        //    {
        //        //kvp = <cart item identifier, warnings>
        //        var sciId = kvp.Key;
        //        var warnings = kvp.Value;
        //        //find model
        //        var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
        //        if (sciModel != null)
        //            foreach (var w in warnings)
        //                if (!model.ErrorList.Contains(w))
        //                    model.ErrorList.Add(w);


        //    }
        //    if (model.ErrorList.Count > 0)
        //    {
        //        model.StatusCode = (int)ErrorType.NotOk;
        //    }
        //    return Ok(model);
        //}

        //[Route("api/ShoppingCart/AddItemsToCartFromWishlist")]
        //[HttpPost]
        //public IActionResult AddItemsToCartFromWishlist(List<KeyValueApi> formValues, Guid? customerGuid = null)
        //{
        //    Customer customer = customerGuid.HasValue ?
        //       _customerService.GetCustomerByGuid(customerGuid.Value)
        //       : _workContext.CurrentCustomer;
        //    var form = new NameValueCollection();
        //    foreach (var values in formValues)
        //    {
        //        form.Add(values.Key, values.Value);
        //    }
        //    if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
        //        return Challenge();

        //    var response = new WishlistResponseModel();

        //    if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
        //        return Challenge();

        //    var pageCustomer = customerGuid.HasValue
        //        ? _customerService.GetCustomerByGuid(customerGuid.Value)
        //        : _workContext.CurrentCustomer;
        //    if (pageCustomer == null)
        //        return Challenge();

        //    var pageCart = pageCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();

        //    // var numberOfAddedItems = 0;
        //    var allIdsToAdd = form["addtocart"] != null
        //        ? form["addtocart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        //        .Select(int.Parse)
        //        .ToList()
        //        : new List<int>();
        //    foreach (var sci in pageCart)
        //    {
        //        if (allIdsToAdd.Contains(sci.Id))
        //        {
        //            var warnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
        //                sci.Product, ShoppingCartType.ShoppingCart,
        //                _storeContext.CurrentStore.Id,
        //                sci.AttributesXml, sci.CustomerEnteredPrice,
        //                sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, true);

        //            if (_shoppingCartSettings.MoveItemsFromWishlistToCart && //settings enabled
        //                !customerGuid.HasValue && //own wishlist
        //                warnings.Count == 0) //no warnings ( already in the cart)
        //            {
        //                //let's remove the item from wishlist
        //                _shoppingCartService.DeleteShoppingCartItem(sci);
        //            }
        //            response.ErrorList.AddRange(warnings);
        //        }
        //    }
        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //         .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //         .ToList();
        //    response.Count = cart.GetTotalProducts();
        //    var wishlistCart = customer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();
        //    _shoppingCartModelFactoryApi.PrepareWishlistModel(response, wishlistCart, !customerGuid.HasValue);

        //    if (response.ErrorList.Count > 0)
        //    {
        //        response.StatusCode = (int)ErrorType.NotOk;
        //    }
        //    return Ok(response);
        //}

        //[Route("api/shoppingcart/checkoutorderinformation")]
        //[HttpGet]
        //public IActionResult CheckoutOrderInformation()
        //{
        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //       .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //      .LimitPerStore(_storeContext.CurrentStore.Id)
        //       .ToList();

        //    var model = new CheckoutInformationResponseModel();
        //    _shoppingCartModelFactoryApi.PrepareShoppingCartModel(model.ShoppingCartModel, cart, prepareAndDisplayOrderReviewData: true);
        //    model.OrderTotalModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);

        //    return Ok(model);
        //}
        //#endregion
    }
}

using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.BulkOrder.Services
{
    public class Xtn_ShoppingCartService : ShoppingCartService
    {
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        public Xtn_ShoppingCartService(IRepository<ShoppingCartItem> sciRepository
            , IWorkContext workContext
            , IStoreContext storeContext
            , ICurrencyService currencyService
            , IProductService productService
            , ILocalizationService localizationService
            , IProductAttributeParser productAttributeParser
            , ICheckoutAttributeService checkoutAttributeService
            , ICheckoutAttributeParser checkoutAttributeParser
            , IPriceFormatter priceFormatter
            , ICustomerService customerService
            , OrderSettings orderSettings
            , ShoppingCartSettings shoppingCartSettings
            , IEventPublisher eventPublisher
            , IPermissionService permissionService
            , IAclService aclService
            , IDateRangeService dateRangeService
            , IStoreMappingService storeMappingService
            , IGenericAttributeService genericAttributeService
            , IProductAttributeService productAttributeService
            , IDateTimeHelper dateTimeHelper) : base(sciRepository
                , workContext
                , storeContext
                , currencyService
                , productService
                , localizationService
                , productAttributeParser
                , checkoutAttributeService
                , checkoutAttributeParser
                , priceFormatter
                , customerService
                , orderSettings
                , shoppingCartSettings
                , eventPublisher
                , permissionService
                , aclService
                , dateRangeService
                , storeMappingService
                , genericAttributeService
                , productAttributeService
                , dateTimeHelper)
        {
            _localizationService = localizationService;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _shoppingCartSettings = shoppingCartSettings;
        }

        /// <summary>
        /// Add a product to shopping cart
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="product">Product</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">The price enter by a customer</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredProductsIfEnabled">Automatically add required products if enabled</param>
        /// <returns>Warnings</returns>
        public override IList<string> AddToCart(Customer customer, Product product,
            ShoppingCartType shoppingCartType, int storeId, string attributesXml = null,
            decimal customerEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool automaticallyAddRequiredProductsIfEnabled = true)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var warnings = new List<string>();
            //if (shoppingCartType == ShoppingCartType.ShoppingCart && !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart, customer))
            //{
            //    warnings.Add("Shopping cart is disabled");
            //    return warnings;
            //}
            //if (shoppingCartType == ShoppingCartType.Wishlist && !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist, customer))
            //{
            //    warnings.Add("Wishlist is disabled");
            //    return warnings;
            //}
            if (customer.IsSearchEngineAccount())
            {
                warnings.Add("Search engine can't add to cart");
                return warnings;
            }

            if (quantity <= 0)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));
                return warnings;
            }

            //reset checkout info
           // _customerService.ResetCheckoutData(customer, storeId);

            var cart = customer.ShoppingCartItems
                //.Where(sci => sci.ShoppingCartType == shoppingCartType)
                .LimitPerStore(storeId)
                .ToList();

            var shoppingCartItem = FindShoppingCartItemInTheCart(cart,
                shoppingCartType, product, attributesXml, customerEnteredPrice,
                rentalStartDate, rentalEndDate);

            if (shoppingCartItem != null)
            {
                //update existing shopping cart item
                var newQuantity = shoppingCartItem.Quantity + quantity;
                warnings.AddRange(GetShoppingCartItemWarnings(customer, shoppingCartType, product,
                    storeId, attributesXml,
                    customerEnteredPrice, rentalStartDate, rentalEndDate,
                    newQuantity, automaticallyAddRequiredProductsIfEnabled));

                if (!warnings.Any())
                {
                    shoppingCartItem.AttributesXml = attributesXml;
                    shoppingCartItem.Quantity = newQuantity;
                    shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;
                    _customerService.UpdateCustomer(customer);

                    //event notification
                    _eventPublisher.EntityUpdated(shoppingCartItem);
                }
            }
            else
            {
                //new shopping cart item
                warnings.AddRange(GetShoppingCartItemWarnings(customer, shoppingCartType, product,
                    storeId, attributesXml, customerEnteredPrice,
                    rentalStartDate, rentalEndDate,
                    quantity, automaticallyAddRequiredProductsIfEnabled));
                if (!warnings.Any())
                {
                    if (cart.Count >= _shoppingCartSettings.MaximumShoppingCartItems)
                    {
                        warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumShoppingCartItems"), _shoppingCartSettings.MaximumShoppingCartItems));
                        return warnings;
                    }
                    //maximum items validation
                    //switch (shoppingCartType)
                    //{
                    //    case ShoppingCartType.ShoppingCart:
                    //        {
                    //            if (cart.Count >= _shoppingCartSettings.MaximumShoppingCartItems)
                    //            {
                    //                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumShoppingCartItems"), _shoppingCartSettings.MaximumShoppingCartItems));
                    //                return warnings;
                    //            }
                    //        }
                    //        break;
                    //    case ShoppingCartType.Wishlist:
                    //        {
                    //            if (cart.Count >= _shoppingCartSettings.MaximumWishlistItems)
                    //            {
                    //                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumWishlistItems"), _shoppingCartSettings.MaximumWishlistItems));
                    //                return warnings;
                    //            }
                    //        }
                    //        break;
                    //    default:
                    //        break;
                    //}

                    var now = DateTime.UtcNow;
                    shoppingCartItem = new ShoppingCartItem
                    {
                        ShoppingCartType = shoppingCartType,
                        StoreId = storeId,
                        Product = product,
                        AttributesXml = attributesXml,
                        CustomerEnteredPrice = customerEnteredPrice,
                        Quantity = quantity,
                        RentalStartDateUtc = rentalStartDate,
                        RentalEndDateUtc = rentalEndDate,
                        CreatedOnUtc = now,
                        UpdatedOnUtc = now
                    };
                    customer.ShoppingCartItems.Add(shoppingCartItem);
                    _customerService.UpdateCustomer(customer);


                    //updated "HasShoppingCartItems" property used for performance optimization
                    customer.HasShoppingCartItems = customer.ShoppingCartItems.Any();
                    _customerService.UpdateCustomer(customer);

                    //event notification
                   // _eventPublisher.EntityInserted(shoppingCartItem);
                }
            }

            return warnings;
        }
        public override ShoppingCartItem FindShoppingCartItemInTheCart(IList<ShoppingCartItem> shoppingCart,
            ShoppingCartType shoppingCartType,
            Product product,
            string attributesXml = "",
            decimal customerEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null,
            DateTime? rentalEndDate = null)
        {
            if (shoppingCart == null)
                throw new ArgumentNullException(nameof(shoppingCart));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            foreach (var sci in shoppingCart.Where(a => a.ShoppingCartType == shoppingCartType))
            {
                if (sci.ProductId == product.Id)
                {
                    //attributes
                    var attributesEqual = (sci.AttributesXml.Trim() == attributesXml.Trim());// _productAttributeParser.AreProductAttributesEqual(sci.AttributesXml, attributesXml, false, false);

                    ////gift cards
                    //var giftCardInfoSame = true;
                    //if (sci.Product.IsGiftCard)
                    //{
                    //    _productAttributeParser.GetGiftCardAttribute(attributesXml, out string giftCardRecipientName1, out string _, out string giftCardSenderName1, out string _, out string _);

                    //    _productAttributeParser.GetGiftCardAttribute(sci.AttributesXml, out string giftCardRecipientName2, out string _, out string giftCardSenderName2, out string _, out string _);

                    //    if (giftCardRecipientName1.ToLowerInvariant() != giftCardRecipientName2.ToLowerInvariant() ||
                    //        giftCardSenderName1.ToLowerInvariant() != giftCardSenderName2.ToLowerInvariant())
                    //        giftCardInfoSame = false;
                    //}

                    ////price is the same (for products which require customers to enter a price)
                    //var customerEnteredPricesEqual = true;
                    //if (sci.Product.CustomerEntersPrice)
                    //    //TODO should we use RoundingHelper.RoundPrice here?
                    //    customerEnteredPricesEqual = Math.Round(sci.CustomerEnteredPrice, 2) == Math.Round(customerEnteredPrice, 2);

                    ////rental products
                    //var rentalInfoEqual = true;
                    //if (sci.Product.IsRental)
                    //{
                    //    rentalInfoEqual = sci.RentalStartDateUtc == rentalStartDate && sci.RentalEndDateUtc == rentalEndDate;
                    //}

                    //found?
                    if (attributesEqual )//&& giftCardInfoSame && customerEnteredPricesEqual && rentalInfoEqual)
                        return sci;
                }
            }

            return null;
        }
    }
}

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Orders.MultiShipping.Models.ShoppingCart;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.ShoppingCart;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Orders.MultiShipping.Components
{
    [ViewComponent(Name = "Mus_OrderSummaryShipment")]
    public class Mus_OrderSummaryShipmentViewComponent : NopViewComponent
    {
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public Mus_OrderSummaryShipmentViewComponent(IShoppingCartModelFactory shoppingCartModelFactory,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            this._shoppingCartModelFactory = shoppingCartModelFactory;
            this._storeContext = storeContext;
            this._workContext = workContext;
        }

        public IViewComponentResult Invoke(bool? prepareAndDisplayOrderReviewData, ShoppingCartModel overriddenModel)
        {
            //use already prepared (shared) model
            if (overriddenModel != null)
                return View(overriddenModel);

            //if not passed, then create a new model
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var model = new ShoppingCartModel();
            model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart,
                isEditable: false,
                prepareAndDisplayOrderReviewData: prepareAndDisplayOrderReviewData.GetValueOrDefault());
            var ScmFactory = new ShoppingCartModelFactories();
            Mus_ShoppingCartModel newModel = new Mus_ShoppingCartModel()
            {
                AnonymousCheckoutAllowed = false,
                ButtonPaymentMethodViewComponentNames = model.ButtonPaymentMethodViewComponentNames,
                CheckoutAttributes = ScmFactory.prepareCheckoutAttr(model.CheckoutAttributes),
                CustomProperties = model.CustomProperties,
                CurrentCustomerIsGuest = _workContext.CurrentCustomer.IsGuest(),
                DiscountBox = ScmFactory.PrepareDiscountBox(model.DiscountBox),
                DisplayTaxShippingInfo = model.DisplayTaxShippingInfo,
                GiftCardBox = ScmFactory.PrepareGiftCardBox(model.GiftCardBox),
                HideCheckoutButton = model.HideCheckoutButton,
                IsEditable = model.IsEditable,
                Items = ScmFactory.PrepareItems(model.Items),
                MinOrderSubtotalWarning = model.MinOrderSubtotalWarning,
                OnePageCheckoutEnabled = model.OnePageCheckoutEnabled,
                OrderReviewData = ScmFactory.prepareOrderReviewData(model.OrderReviewData),
                ShowProductImages = model.ShowProductImages,
                ShowSku = model.ShowSku,
                ShowVendorName = true,
                TermsOfServiceOnOrderConfirmPage = model.TermsOfServiceOnOrderConfirmPage,
                TermsOfServiceOnShoppingCartPage = model.TermsOfServiceOnShoppingCartPage,
                TermsOfServicePopup = model.TermsOfServicePopup,
                Warnings = model.Warnings
            };
            //
            return View("~/Plugins/Orders.MultiShipping/Views/Components/OrderSummaryShipment/Default.cshtml",newModel);
        }
    }
}

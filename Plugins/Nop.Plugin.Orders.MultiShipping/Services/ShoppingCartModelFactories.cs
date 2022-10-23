using Nop.Plugin.Orders.MultiShipping.Models.ShoppingCart;
using Nop.Web.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public class ShoppingCartModelFactories
    {
        #region FactoriesMethod
        public Mus_ShoppingCartModel.OrderReviewDataModel prepareOrderReviewData(ShoppingCartModel.OrderReviewDataModel orderReviewData)
        {

            var ord = new Mus_ShoppingCartModel.OrderReviewDataModel();
            if (orderReviewData == null)
                return ord;
            ord.BillingAddress = orderReviewData.BillingAddress;
            ord.CustomProperties = orderReviewData.CustomProperties;
            ord.CustomValues = orderReviewData.CustomValues;
            ord.Display = orderReviewData.Display;
            ord.IsShippable = orderReviewData.IsShippable;
            ord.PaymentMethod = orderReviewData.PaymentMethod;
            ord.PickupAddress = orderReviewData.PickupAddress;
            ord.SelectedPickUpInStore = orderReviewData.SelectedPickUpInStore;
            ord.ShippingAddress = orderReviewData.ShippingAddress;
            ord.ShippingMethod = orderReviewData.ShippingMethod;
            return ord;
        }

        public IList<Mus_ShoppingCartModel.ShoppingCartItemModel> PrepareItems(IList<ShoppingCartModel.ShoppingCartItemModel> items)
        {
            var Lst_ScI = new List<Mus_ShoppingCartModel.ShoppingCartItemModel>();
            if (items == null)
                return Lst_ScI;
            foreach (var item in items)
            {
                Lst_ScI.Add(new Mus_ShoppingCartModel.ShoppingCartItemModel()
                {
                    AllowedQuantities = item.AllowedQuantities,
                    AllowItemEditing = item.AllowItemEditing,
                    AttributeInfo = item.AttributeInfo,
                    AvailableEndDateTimeUtc = "",
                    AvailableStartDateTimeUtc = "",
                    CustomProperties = item.CustomProperties,
                    DisableRemoval = item.DisableRemoval,
                    Discount = item.Discount,
                    FreeQty = null,
                    Id = item.Id,
                    MaximumDiscountedQty = item.MaximumDiscountedQty,
                    PackProductSeName = "",
                    Picture = item.Picture,
                    PreOrderAvailabilityStartDateTimeUtc = "",
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductSeName = item.ProductSeName,
                    Quantity = item.Quantity,
                    RecurringInfo = item.RecurringInfo,
                    RentalInfo = item.RentalInfo,
                    Sku = item.Sku,
                    SubTotal = item.SubTotal,
                    UnitPrice = item.UnitPrice,
                    VendorName = "",
                    Warnings = item.Warnings,
                    Weight = 0
                });
            }
            return Lst_ScI;
        }

        public Mus_ShoppingCartModel.GiftCardBoxModel PrepareGiftCardBox(ShoppingCartModel.GiftCardBoxModel giftCardBox)
        {
            var Gcb = new Mus_ShoppingCartModel.GiftCardBoxModel();
            if (giftCardBox == null)
                return Gcb;
            Gcb.CustomProperties = giftCardBox.CustomProperties;
            Gcb.Display = giftCardBox.Display;
            Gcb.IsApplied = giftCardBox.IsApplied;
            Gcb.Message = giftCardBox.Message;
            return Gcb;
        }

        public Mus_ShoppingCartModel.DiscountBoxModel PrepareDiscountBox(ShoppingCartModel.DiscountBoxModel discountBox)
        {
            var Dbx = new Mus_ShoppingCartModel.DiscountBoxModel();
            if (discountBox == null)
                return Dbx;
            Dbx.AppliedDiscountsWithCodes = PrepareDiscountWithCodes(discountBox.AppliedDiscountsWithCodes);
            Dbx.CustomProperties = discountBox.CustomProperties;
            Dbx.Display = discountBox.Display;
            Dbx.IsApplied = discountBox.IsApplied;
            Dbx.Messages = discountBox.Messages;
            return Dbx;
        }

        public List<Mus_ShoppingCartModel.DiscountBoxModel.DiscountInfoModel> PrepareDiscountWithCodes(List<ShoppingCartModel.DiscountBoxModel.DiscountInfoModel> appliedDiscountsWithCodes)
        {
            var Dbxm = new List<Mus_ShoppingCartModel.DiscountBoxModel.DiscountInfoModel>();
            if (appliedDiscountsWithCodes == null)
                return Dbxm;
            foreach (var item in appliedDiscountsWithCodes)
            {
                Dbxm.Add(new Mus_ShoppingCartModel.DiscountBoxModel.DiscountInfoModel()
                {
                    CouponCode = item.CouponCode,
                    CustomProperties = item.CustomProperties,
                    Id = item.Id
                });
            }
            return Dbxm;
        }

        public IList<Mus_ShoppingCartModel.CheckoutAttributeModel> prepareCheckoutAttr(IList<ShoppingCartModel.CheckoutAttributeModel> checkoutAttributes)
        {
            IList<Mus_ShoppingCartModel.CheckoutAttributeModel> Lst_checkoutattr = new List<Mus_ShoppingCartModel.CheckoutAttributeModel>();
            if (checkoutAttributes == null)
                return Lst_checkoutattr;
            foreach (var item in checkoutAttributes)
            {
                Lst_checkoutattr.Add(new Mus_ShoppingCartModel.CheckoutAttributeModel()
                {
                    AllowedFileExtensions = item.AllowedFileExtensions,
                    AttributeControlType = item.AttributeControlType,
                    CustomProperties = item.CustomProperties,
                    DefaultValue = item.DefaultValue,
                    Id = item.Id,
                    IsRequired = item.IsRequired,
                    Name = item.Name,
                    SelectedDay = item.SelectedDay,
                    SelectedMonth = item.SelectedMonth,
                    SelectedYear = item.SelectedYear,
                    TextPrompt = item.TextPrompt,
                    Values = prepareCheckoutAttrValue(item.Values)
                });
            }
            return Lst_checkoutattr;
        }

        public IList<Mus_ShoppingCartModel.CheckoutAttributeValueModel> prepareCheckoutAttrValue(IList<ShoppingCartModel.CheckoutAttributeValueModel> values)
        {
            IList<Mus_ShoppingCartModel.CheckoutAttributeValueModel> Lst_CheckoutAttrVal = new List<Mus_ShoppingCartModel.CheckoutAttributeValueModel>();
            if (values == null)
                return Lst_CheckoutAttrVal;
            foreach (var item in values)
            {
                Lst_CheckoutAttrVal.Add(new Mus_ShoppingCartModel.CheckoutAttributeValueModel()
                {
                    ColorSquaresRgb = item.ColorSquaresRgb,
                    CustomProperties = item.CustomProperties,
                    Id = item.Id,
                    IsPreSelected = item.IsPreSelected,
                    Name = item.Name,
                    PriceAdjustment = item.PriceAdjustment
                });
            }
            return Lst_CheckoutAttrVal;
        }
        #endregion
    }
}

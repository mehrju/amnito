using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Html;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using static Nop.plugin.Orders.ExtendedShipment.Services.AgentAmountRuleService;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class ExtendedPdfService : PdfService
    {
        #region Field
        private readonly IAddressService _addressService;
        private readonly IPictureService _pictureService;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IOrderService _orderService;
        private readonly ILanguageService _languageService;
        private readonly PdfSettings _pdfSettings;
        private readonly IWorkContext _workContext;
        private readonly AddressSettings _addressSettings;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ISettingService _settingService;
        private readonly TaxSettings _taxSettings;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly CatalogSettings _catalogSettings;
        private readonly IPaymentService _paymentService;
        private readonly IWebHelper _webHelper;
        private readonly ITopicService _topicService;
        private readonly IDbContext _dbContext;
        private readonly ICollectorService _collectorService;
        private readonly IRelatedOrders_Service _relatedOrders_Service;
        private readonly IRewardPointCashoutService _rewardPointCashoutService;
        private readonly IAgentAmountRuleService _agentAmountRuleService;
        #endregion

        #region ctor

        public ExtendedPdfService(ILocalizationService localizationService
            , IAddressService addressService
            , ILanguageService languageService
            , IWorkContext workContext
            , IOrderService orderService
            , IPaymentService paymentService
            , IDateTimeHelper dateTimeHelper
            , IPriceFormatter priceFormatter
            , ICurrencyService currencyService
            , IMeasureService measureService
            , IPictureService pictureService
            , IProductService productService
            , IProductAttributeParser productAttributeParser
            , IStoreService storeService
            , IStoreContext storeContext
            , ISettingService settingService
            , IAddressAttributeFormatter addressAttributeFormatter
            , CatalogSettings catalogSettings
            , CurrencySettings currencySettings
            , MeasureSettings measureSettings
            , PdfSettings pdfSettings
            , TaxSettings taxSettings
            , AddressSettings addressSettings
            , IWebHelper webHelper
            , IExtendedShipmentService extendedShipmentService
            , ITopicService topicService
            , IDbContext dbContext
            , ICollectorService collectorService
            , IRewardPointCashoutService rewardPointCashoutService

            , IRelatedOrders_Service relatedOrders_Service
            , IAgentAmountRuleService agentAmountRuleService
            ) : base(localizationService
            , languageService
            , workContext
            , orderService
            , paymentService
            , dateTimeHelper
            , priceFormatter
            , currencyService
            , measureService
            , pictureService
            , productService
            , productAttributeParser
            , storeService
            , storeContext
            , settingService
            , addressAttributeFormatter
            , catalogSettings
            , currencySettings
            , measureSettings
            , pdfSettings
            , taxSettings
            , addressSettings)
        {
            _agentAmountRuleService = agentAmountRuleService;
            _relatedOrders_Service = relatedOrders_Service;
            _dbContext = dbContext;
            _topicService = topicService;
            this._rewardPointCashoutService = rewardPointCashoutService;
            this._pictureService = pictureService;
            this._storeService = storeService;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._extendedShipmentService = extendedShipmentService;
            this._orderService = orderService;
            this._languageService = languageService;
            this._pdfSettings = pdfSettings;
            this._workContext = workContext;
            this._addressSettings = addressSettings;
            this._productAttributeParser = productAttributeParser;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._settingService = settingService;
            this._taxSettings = taxSettings;
            this._priceFormatter = priceFormatter;
            this._currencyService = currencyService;
            this._catalogSettings = catalogSettings;
            this._paymentService = paymentService;
            _webHelper = webHelper;
            this._addressService = addressService;
            this._collectorService = collectorService;
        }

        #endregion

        public override void PrintOrdersToPdf(Stream stream, IList<Order> orders, int languageId = 0, int vendorId = 0)
        {
            try
            {
                if (stream == null)
                    throw new ArgumentNullException(nameof(stream));

                if (orders == null)
                    throw new ArgumentNullException(nameof(orders));
                if (orders.Any(p => p.OrderItems.Any(q => q.ProductId == 10435)))
                {
                    Print_Valet(orders, stream, false);
                    return;
                }
                if (orders.Any(p => p.OrderItems.Any(n => n.ProductId == 10277)))
                {
                    Print_Valet(orders, stream, true);
                    return;
                }
                if (orders.Any(p => p.OrderItems.Any(n => n.ProductId == 10430)))
                {
                    base.PrintOrdersToPdf(stream, orders, languageId, vendorId);
                    return;
                }
                if ((orders.Any(p => p.PaymentMethodSystemName == "Payments.CashOnDelivery")
                    || orders.Any(p => p.OrderItems.Any(n => new int[] { 10436, 10437 }.Contains(n.ProductId)))))// پس کرایه ماهکس
                {
                    Print_COD_OrdersToPdf(orders, stream);
                    return;
                }
                else
                {
                    Stream tempStram = new MemoryStream();
                    var pageSize = PageSize.A4;

                    if (_pdfSettings.LetterPageSizeEnabled)
                    {
                        pageSize = PageSize.LETTER;
                    }

                    var doc = new Document(pageSize);
                    var pdfWriter = PdfWriter.GetInstance(doc, tempStram);
                    pdfWriter.CloseStream = false;
                    doc.Open();

                    //fonts
                    var titleFont = GetFont("Vazir-FD.TTF");
                    titleFont.SetStyle(Font.BOLD);
                    titleFont.Color = BaseColor.BLACK;
                    var font = GetFont("Vazir-FD.TTF");
                    font.Size = 10;
                    var attributesFont = GetFont("Vazir-FD.TTF");
                    attributesFont.SetStyle(Font.ITALIC);

                    var ordCount = orders.Count + orders.Sum(p => p.OrderItems.Sum(n => n.Quantity));
                    var ordNum = 0;

                    Shipment shipment = null;
                    Address ShippingAddress = null;
                    foreach (var order in orders)
                    {
                        bool hasCollectorInState = order.BillingAddress.StateProvince != null ? _collectorService.HasCollectorInState(order.BillingAddress.StateProvinceId.Value) : false;
                        int itemDiscount = 0;
                        int ServiceId = order.OrderItems.First().Product.ProductCategories.First().CategoryId;
                        var catInfo = _extendedShipmentService.GetCategoryInfo(ServiceId);
                        if (order.OrderDiscount > decimal.Zero)
                        {
                            itemDiscount = Convert.ToInt32(order.OrderDiscount / (order.OrderItems.Sum(p => p.Quantity)));
                        }
                        bool IsMultiShipment = _extendedShipmentService.IsMultiShippment(order);
                        int HagheMaghar = 0;
                        int _posthagheMaghar = 0;
                        foreach (var orderItem in order.OrderItems)
                        {
                            HagheMaghar = _extendedShipmentService.getHagheMaghar(orderItem.Id, order.BillingAddress.CountryId.Value, ServiceId, out _posthagheMaghar);
                            if (HagheMaghar > 0)
                                break;
                        }
                        if (HagheMaghar > 0)
                        {
                            if (!catInfo.IsPrivatepost)
                            {
                                HagheMaghar -= _posthagheMaghar;
                                _posthagheMaghar = 0;
                            }
                            else
                                _posthagheMaghar = 0;
                            HagheMaghar = HagheMaghar / order.OrderItems.Sum(p => p.Quantity);
                        }
                        foreach (var orderItem in order.OrderItems)
                        {
                            for (int i = 0; i < orderItem.Quantity; i++)
                            {
                                if (IsMultiShipment)
                                {
                                    var MultiShipmentData = _extendedShipmentService.getShipmentFromMultiShipment(orderItem, i);
                                    if (MultiShipmentData == null)
                                    {
                                        _extendedShipmentService.Log("اطلاعات مربوط به حمل و نقل ایتم سفارش " + orderItem.Id + " یافت نشد ", "");
                                        continue;
                                    }
                                    shipment = MultiShipmentData.shipment;
                                    ShippingAddress = _addressService.GetAddressById(MultiShipmentData.ShipmentAddressId);
                                }
                                else
                                {
                                    ShippingAddress = order.ShippingAddress;
                                    if (orderItem.Order.Shipments != null)
                                    {
                                        if (orderItem.Order.Shipments.Any())
                                        {
                                            shipment = orderItem.Order.Shipments.Where(p => p.ShipmentItems.Any(n => n.OrderItemId == orderItem.Id))
                                                .OrderBy(p => p.Id).Skip(i).Take(1).FirstOrDefault();
                                        }
                                    }
                                }

                                //by default _pdfSettings contains settings for the current active store
                                //and we need PdfSettings for the store which was used to place an order
                                //so let's load it based on a store of the current order
                                int StoreIdForSetting = 0;
                                if (new int[] { 654, 662, 655, 667, 670, 725, 726, 727 }.Contains(ServiceId))
                                    StoreIdForSetting = 3;
                                else
                                    StoreIdForSetting = _storeContext.CurrentStore.Id;
                                var pdfSettingsByStore = _settingService.LoadSetting<PdfSettings>(StoreIdForSetting);

                                var lang = _languageService.GetLanguageById(languageId == 0 ? order.CustomerLanguageId : languageId);
                                if (lang == null || !lang.Published)
                                    lang = _workContext.WorkingLanguage;

                                //header
                                PrintHeader(pdfSettingsByStore, lang, order, font, titleFont, doc, shipment, orderItem);
                                var bill = _extendedShipmentService.orderItemTotal(orderItem);
                                if (order.StoreId == 3 || order.StoreId == 5)
                                {
                                    //addresses
                                    PrintAddresses(vendorId, lang, titleFont, orderItem, font, doc, shipment, ShippingAddress, catInfo.CategoryId);
                                    //products
                                    PrintPostPrice(vendorId, lang, titleFont, doc, orderItem, font, attributesFont, bill, i, true, _posthagheMaghar);

                                    PrintEngiPrice(vendorId, lang, titleFont, doc, orderItem, font, attributesFont, bill, HagheMaghar, itemDiscount, true);
                                }
                                else
                                {
                                    PrintAddresses(vendorId, lang, titleFont, order, font, doc);
                                    PrintProducts(vendorId, lang, titleFont, doc, order, font, attributesFont);
                                }

                                //checkout attributes
                                PrintCheckoutAttributes(vendorId, order, doc, lang, font);
                                if (order.StoreId == 3 || order.StoreId == 5)
                                    //totals
                                    PrintTotals(vendorId, lang, orderItem, font, titleFont, doc, bill, shipment, itemDiscount, HagheMaghar, _posthagheMaghar
                                        , shipment, ServiceId);
                                else
                                    PrintTotals(vendorId, lang, order, font, titleFont, doc);
                                _posthagheMaghar = 0;
                                //order notes
                                PrintOrderNotes(pdfSettingsByStore, order, lang, titleFont, doc, font);

                                //footer
                                PrintFooter(pdfSettingsByStore, pdfWriter, pageSize, lang, font);

                                ordNum++;
                                if (ordNum < ordCount)
                                {
                                    doc.NewPage();
                                }
                            }
                        }
                    }
                    doc.Close();
                    #region AddPdfFileToFactor;

                    if (!_workContext.CurrentCustomer.IsInCustomerRole("CollerctorAgent"))
                    {
                        var pdfFileStream = ReadPdfFromFile(pageSize);
                        PdfCopyFields copy = new PdfCopyFields(stream);
                        tempStram.Position = 0;
                        MemoryStream firstPdf = new MemoryStream(((MemoryStream)tempStram).ToArray());
                        copy.AddDocument(new PdfReader(firstPdf));

                        //pdfFileStream.Position = 0;
                        MemoryStream secoundtPdf = new MemoryStream(((MemoryStream)pdfFileStream).ToArray());
                        copy.AddDocument(new PdfReader(secoundtPdf));


                        firstPdf.Dispose();
                        secoundtPdf.Dispose();
                        pdfFileStream.Dispose();
                        copy.Close();
                    }
                    else
                    {
                        var pdfFileStream = ReadPdfFromFile(pageSize);
                        PdfCopyFields copy = new PdfCopyFields(stream);
                        tempStram.Position = 0;
                        MemoryStream firstPdf = new MemoryStream(((MemoryStream)tempStram).ToArray());
                        copy.AddDocument(new PdfReader(firstPdf));

                        //pdfFileStream.Position = 0;
                        //MemoryStream secoundtPdf = new MemoryStream(((MemoryStream)pdfFileStream).ToArray());
                        //copy.AddDocument(new PdfReader(secoundtPdf));


                        firstPdf.Dispose();
                        // secoundtPdf.Dispose();
                        pdfFileStream.Dispose();
                        copy.Close();
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _extendedShipmentService.Log("خطا در زمان چاپ فاکتور",
                    ex.Message + (ex.InnerException != null ? ex.InnerException.Message : "") + "--> خط : " + getline(ex));
            }
        }
        public MemoryStream ReadPdfFromFile(Rectangle pageSize)
        {
            MemoryStream ms = new MemoryStream();
            var Newdoc = new Document(pageSize);
            PdfCopy writer = new PdfCopy(Newdoc, ms);
            Newdoc.Open();
            string pdfAddress = "";
            if (_storeContext.CurrentStore.Id == 5)
                pdfAddress = CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Content/") + "NewRole.pdf";
            else
                pdfAddress = CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Content/") + "postbarRule.pdf";
            PdfReader reader = new PdfReader(pdfAddress);
            writer.AddDocument(reader);
            reader.Close();
            writer.Close();
            Newdoc.Close();
            return ms;
        }
        public int getline(Exception ex)
        {
            // Get stack trace for the exception with source file information
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            return frame.GetFileLineNumber();
        }

        public void Print_COD_OrdersToPdf(IList<Order> orders, Stream stream)
        {

            //var t = new StiReport();
            //t.Load(@"D:\Projects\Web_APP\COD.mrt");
            //t.Dictionary.BusinessObjects.Clear();
            //t.RegBusinessObject("CODList", "CODList", new CODModel());
            //t.Dictionary.SynchronizeBusinessObjects();
            //t.Save(@"D:\Projects\Web_APP\Nope Clear file\PostbarNop\Presentation\Nop.Web\Plugins\Orders.ExtendedShipment\Report\COD.mrt");


            //var t1 = new StiReport();
            //t1.Dictionary.BusinessObjects.Clear();
            //t1.RegBusinessObject("CODList", "CODList", new CODModel());
            //t1.Dictionary.SynchronizeBusinessObjects();
            //t1.Save(@"D:\Projects\Web_APP\COD_DTS.mrt");

            List<CODModel> Lst_model = new List<CODModel>();
            List<CODAttributesModel> Lst_attributemodel = new List<CODAttributesModel>();

            int serviceId = 0;

            foreach (var order in orders)
            {
                int Haghemaghar = _extendedShipmentService.getInsertedHagheMaghar(order);
                Haghemaghar += Convert.ToInt32(Haghemaghar * 9 / 100);
                int _haghemagharForshipment = Convert.ToInt32(Haghemaghar / order.OrderItems.Sum(p => p.Quantity));

                int itemDiscount = 0;
                if (order.OrderDiscount > decimal.Zero)
                {
                    itemDiscount = Convert.ToInt32(order.OrderDiscount / (order.OrderItems.Sum(p => p.Quantity)));
                }

                bool IsmultiShipment = _extendedShipmentService.IsMultiShippment(order);
                int agentPercent = 0;
                bool isSafeBuy = _extendedShipmentService.IsSafeBuy(order.Id);
                var orderitems = getOrderitems(order.Id);
                if (orderitems.Any())
                    Lst_attributemodel.AddRange(orderitems);
                int divedTotalDiscount = 0;
                if (order.OrderDiscount > 0)
                {
                    int discount = (int)(order.OrderDiscount - order.OrderDiscount * 9 / 100);
                    divedTotalDiscount = (discount / order.Shipments.Count());
                }
                foreach (var orderItem in order.OrderItems)
                {
                    var category = orderItem.Product.ProductCategories.First().Category;
                    serviceId = category.Id;
                    //if (order.Customer.IsInCustomerRole("mini-Administrators"))
                    //{
                    PrivatePostDiscount discount = _agentAmountRuleService.GetPrivatePostDiscount(order.CustomerId, serviceId);
                    if (discount != null)
                        agentPercent = discount.DisCountAmount;
                    //}

                    bool _isFreePost = _extendedShipmentService.GetFreePost(orderItem);
                    var store = _storeService.GetStoreById(orderItem.Order.StoreId) ?? _storeContext.CurrentStore;
                    int weight = _extendedShipmentService.GetItemWeightFromAttr(orderItem.Id);
                    int approximateValue = _extendedShipmentService.getApproximateValue(orderItem.Id);
                    int shipmentDiscount = 0;

                    if (!order.Customer.IsInCustomerRole("mini-Administrators"))
                    {
                        PrivatePostDiscount privatePostDiscount;
                        var inlineAgentSaleAMount = _agentAmountRuleService.getInlineCustomerDiscountForShipment(orderItem, out privatePostDiscount);
                        if (inlineAgentSaleAMount != null && inlineAgentSaleAMount.Price > 0)
                        {
                            shipmentDiscount = inlineAgentSaleAMount.Price;
                        }
                    }
                    shipmentDiscount += divedTotalDiscount;
                    for (int i = 0; i < orderItem.Quantity; i++)
                    {
                        Shipment shipment = null;
                        Address shippingAddress = null;
                        string CountryCode = "";
                        string StateCode = "";
                        if (IsmultiShipment)
                        {
                            var multiShipmentData = _extendedShipmentService.getShipmentFromMultiShipment(orderItem, i);
                            if (multiShipmentData == null)
                            {
                                _extendedShipmentService.Log("اطلاعات مربوط به حمل و نقل ایتم سفارش " + orderItem.Id + " یافت نشد ", "");
                                continue;
                            }
                            shipment = multiShipmentData.shipment;
                            shippingAddress = _addressService.GetAddressById(multiShipmentData.ShipmentAddressId);
                            CountryCode = _extendedShipmentService.GetGatwayPriceCountryCode(shippingAddress.CountryId.Value).ToString();

                        }
                        else
                        {
                            shippingAddress = order.ShippingAddress;
                            if (orderItem.Order.Shipments != null)
                            {
                                if (orderItem.Order.Shipments.Any())
                                {
                                    shipment = orderItem.Order.Shipments.Where(p => p.ShipmentItems.Any(n => n.OrderItemId == orderItem.Id))
                                        .OrderBy(p => p.Id).Skip(i).Take(1).FirstOrDefault();
                                }
                            }
                        }
                        //if (string.IsNullOrEmpty(shipment.TrackingNumber))
                        //{
                        //    continue;
                        //}
                        int HazineErsalWithAddtionalFee = 0;
                        int HazineErsalWithAddtionalFee_Tax = 0;
                        int ProductPrice = 0;
                        int kalaPrice = _extendedShipmentService.getGoodsPrice(orderItem);
                        int _postPrice = 0;
                        int _EngPrice = 0;
                        if (new int[] { 705, 706, 711, 713, 715, 731 }.Contains(serviceId))
                        {
                            var prices = _extendedShipmentService.getEngAndPostPrice(orderItem);
                            if (prices == null)
                            {
                                throw new Exception($"قیمت مربوط به آیتم شماره {orderItem.Id} از سرویس مربوطه دریافت نشده");
                            }
                            HazineErsalWithAddtionalFee = prices.IncomePrice;
                            _postPrice = prices.IncomePrice;
                            //int hagheSabt = _extendedShipmentService.getHagheSabt(orderItem.Id);
                            //int hagheMaghar = _extendedShipmentService.getInsertedHagheMaghar(orderItem.Id);
                            ProductPrice = prices.EngPrice + prices.AttrPrice;
                            ProductPrice += ((ProductPrice * 9) / 100);
                            _EngPrice = ProductPrice + _haghemagharForshipment;
                            ProductPrice += kalaPrice;
                            HazineErsalWithAddtionalFee_Tax = 0;
                        }
                        else
                        {
                            bool isCod = !(new int[] { 722, 723 }.Contains(serviceId));
                            string error = "";
                            var mark = _extendedShipmentService.GetOrderRegistrationMethod(order);
                            int DealerId = (mark == OrderRegistrationMethod.Ap) ? 1 : (mark == OrderRegistrationMethod.bidok ? 4 : 0);


                            CodValues codValues = getCodValues(shipment.Id);
                            if (codValues == null || codValues.CodCost == 0 || codValues.CodBmValue == 0)
                            {
                                common.Log($"قیمت سفارش پس کرایه {shipment.OrderId} به درستی محاسبه نشده و در چاپ فاکتور مشکل به وجود آورده", "");
                                common.InsertOrderNote($"قیمت سفارش پس کرایه {shipment.OrderId} به درستی محاسبه نشده و در چاپ فاکتور مشکل به وجود آورده", shipment.OrderId);
                                return;
                            }
                            HazineErsalWithAddtionalFee = codValues.CodCost;
                            ProductPrice = codValues.CodBmValue;
                            HazineErsalWithAddtionalFee_Tax = (codValues.CodCost * 9) / 100;
                            _postPrice = codValues.CodCost + HazineErsalWithAddtionalFee_Tax;
                            _EngPrice = (codValues.CodBmValue - kalaPrice);
                            if (isSafeBuy && !isCod && !string.IsNullOrEmpty(shipment.TrackingNumber))
                            {
                                ProductPrice = (codValues.CodBmValue - approximateValue);
                            }
                            if (_isFreePost)
                            {
                                _EngPrice = getEngPriceForFreePost(shipment.Id);
                            }
                            if (!_isFreePost && !isCod && itemDiscount > 0)
                            {
                                ProductPrice = ProductPrice - itemDiscount;
                            }
                        }


                        CODModel codModel = new CODModel();
                        bool IsCod = !new int[] { 722, 723 }.Contains(serviceId);
                        codModel.CountryCode = (string.IsNullOrEmpty(CountryCode) ? 0 : int.Parse(CountryCode));
                        codModel.payType = (!IsCod ? "پرداخت شده(خرید با کارت بانکی)" : "پرداخت در محل");

                        codModel.WebSiteUrl = _webHelper.GetStoreLocation();
                        codModel.SupportPhoneNo = "";
                        codModel.DonnerBranch = orderItem.Order.BillingAddress.Country.Name + "-" + orderItem.Order.BillingAddress.StateProvince.Name;
                        codModel.OrderId = orderItem.Order.Id;
                        codModel.RowNum = orderItem.Id;

                        if (shipment != null)
                        {
                            codModel.BarcodeImage = _extendedShipmentService.getBarocdeImage(shipment);
                            codModel.barcodeNo = shipment.TrackingNumber;
                            var SendDate = shipment.CreatedOnUtc.ToLocalTime();
                            codModel.sendToPostDate = ("تاریخ ارجا" + ":" + MiladyToShamsi(SendDate
                                , true));
                        }

                        codModel.StoreName = orderItem.Order.BillingAddress.FirstName + " " + orderItem.Order.BillingAddress.LastName;
                        codModel.StorePhoneNo = orderItem.Order.BillingAddress.PhoneNumber;
                        codModel.StoreAddress = orderItem.Order.BillingAddress.Address1;
                        codModel.StoreEmail = orderItem.Order.BillingAddress.Email;
                        codModel.StoreUrl = store.Url;
                        codModel.StoreAnswerTime = "10 " + "-" + " 22";
                        codModel.StorePostCode = orderItem.Order.BillingAddress.ZipPostalCode;
                        codModel.Weight = _extendedShipmentService.GetItemWeightFromAttr(orderItem);// Convert.ToInt32(orderItem.ItemWeight * 1000);
                        codModel.Source = orderItem.Order.BillingAddress.Country.Name + '-' + (new int[] { 4, 579, 580, 581, 582, 583, 584, 585 }.Contains(orderItem.Order.BillingAddress.StateProvince.Id) ? "شهر تهران"
                            : orderItem.Order.BillingAddress.StateProvince.Name);
                        codModel.PostTypeName = category.Name.Replace("پس کرایه", "");
                        codModel.Destination = shippingAddress.Country.Name + '-'
                            + (new int[] { 4, 579, 580, 581, 582, 583, 584, 585 }.Contains(shippingAddress.StateProvinceId.Value) ? "شهر تهران"
                            : shippingAddress.StateProvince.Name);

                        codModel.OrderDate = MiladyToShamsi2(orderItem.Order.CreatedOnUtc.ToLocalTime());
                        codModel.PayeTypeName = "پرداخت در محل";
                        codModel.ReadyConfirmDateTime = shipment != null ? MiladyToShamsi2(shipment.CreatedOnUtc.ToLocalTime(), true) : "";
                        codModel.ProductName = "خدمات فنی مهندسی و کالا"; //orderItem.Product.Name;
                        codModel.ProductPrice = ProductPrice;
                        codModel.PostPrice = HazineErsalWithAddtionalFee;
                        codModel.TaxValue = HazineErsalWithAddtionalFee_Tax;// Convert.ToInt32(((HazineErsal + HazineKala) * bill.Tax) / 100);
                        codModel.DiscountValue = itemDiscount.ToString();
                        codModel.ReciverFullName = shippingAddress.FirstName + " " + shippingAddress.LastName;
                        codModel.ReciverPhoneNo = shippingAddress.PhoneNumber;
                        codModel.ReciverPostCode = shippingAddress.ZipPostalCode;
                        codModel.ReciverAddress = shippingAddress.Address1;
                        codModel.Eng_Kala_Price = (_isFreePost ? ProductPrice : ((HazineErsalWithAddtionalFee + ProductPrice) + codModel.TaxValue) - itemDiscount);
                        codModel.E_NemadNo = (_extendedShipmentService.getOrderItemContent(orderItem) ?? "");
                        codModel.CodBmValue = _EngPrice;
                        codModel.CodCost = _postPrice;
                        codModel.isCod = IsCod;
                        codModel.IsSafeBuy = isSafeBuy;
                        codModel.ShipmentId = shipment?.Id ?? 0;

                        // if (!IsCod)
                        {
                            Lst_attributemodel.Add(new CODAttributesModel { Name = "جمع آوری", Value = _haghemagharForshipment.ToString(), ShipmentId = shipment.Id });
                            int attrPrice = Lst_attributemodel.Where(p => p.ShipmentId == shipment.Id && !p.Name.Contains("وجهی")).Sum(p => int.Parse(p.Value));

                            Lst_attributemodel.Add(new CODAttributesModel { Name = "پستی", Value = (_postPrice).ToString(), ShipmentId = shipment.Id });
                            if (IsCod)
                                attrPrice = attrPrice - kalaPrice;
                            int engPrice = 0;
                            if (!_isFreePost)
                                engPrice = _EngPrice - attrPrice;
                            else
                                engPrice = _EngPrice;
                            if (isSafeBuy && !string.IsNullOrEmpty(shipment.TrackingNumber))
                            {
                                engPrice = engPrice - approximateValue;
                            }
                            int CodTranPrice = 0;
                            CodTranPrice = getCodPriceForFreePost(shipment.Id);
                            Lst_attributemodel.Add(new CODAttributesModel { Name = "COD خدمات", Value = CodTranPrice.ToString(), ShipmentId = shipment.Id });
                            Lst_attributemodel.Add(new CODAttributesModel { Name = "فنی", Value = (engPrice - CodTranPrice).ToString(), ShipmentId = shipment.Id });
                            if (_isFreePost)
                            {
                                var kala = Lst_attributemodel.Where(p => p.Name.Contains("کالا")).FirstOrDefault();
                                if (kala != null)
                                    Lst_attributemodel.Remove(kala);
                            }
                            if (isSafeBuy)
                            {
                                Lst_attributemodel.Add(new CODAttributesModel { Name = "کالا", Value = approximateValue.ToString(), ShipmentId = shipment.Id });
                            }
                            int UserPayValue = 0;
                            if (!_isFreePost)
                                UserPayValue = _postPrice + engPrice + attrPrice + kalaPrice + (!IsCod && isSafeBuy ? approximateValue : 0);
                            else
                                UserPayValue = Convert.ToInt32(_postPrice + (engPrice + attrPrice + ((engPrice + attrPrice) * 0.09)));
                            int PostEngPrice = _postPrice + engPrice;
                            if (shipmentDiscount > 0)
                            {
                                Lst_attributemodel.Add(new CODAttributesModel { Name = "تخفیف", Value = (Convert.ToInt32(shipmentDiscount + (shipmentDiscount * 0.09))).ToString(), ShipmentId = shipment.Id });
                                UserPayValue = UserPayValue - Convert.ToInt32(shipmentDiscount + (shipmentDiscount * 0.09));
                            }

                            Lst_attributemodel.Add(new CODAttributesModel { Name = "کل", Value = UserPayValue.ToString(), ShipmentId = shipment.Id });
                        }
                        if (!IsCod)
                        {
                            if ((codModel.CodBmValue + approximateValue) < 50000)
                            {
                                approximateValue += (50000 - (codModel.CodBmValue + approximateValue));
                            }
                            codModel.approximateValue = (IsCod ? 0 : approximateValue);
                        }
                        else
                        {
                            if (kalaPrice == 0 && new int[] { 670, 667 }.Contains(serviceId))
                                codModel.approximateValue = codModel.ProductPrice;
                            else
                                codModel.approximateValue = approximateValue;
                        }
                        codModel.AgentPercent = agentPercent;
                        //deserialize and add to lstattributemodel

                        //   Lst_attributemodel.Add();

                        Lst_model.Add(codModel);
                    }
                }
            }
            Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHm4lqN+V3/fvJECXRrqdoEPZqYyFXh3g3K9oCDFvgzMl4c9KudQQwMJ6nO6w7rHz59BhwYgDE0QzKtjY2WxEejTNewbNDXY492M2mDsK1Hb4t6MoFYGbSoID0gow3VC5cFhvcOKVoagiHq6/4iqFc3RS7nZQBp95+WB6N1fR//H7OBlvfuBKldvC1pJh6pTW/7HRdOclErt/EGwivx9SpuDNabWWBfIZJBdJsvKjpoIsDQGibWk8Y8F9V1mCLf88FurWwEZ9WzXePbJn+wfabHM+a7pqDAXhsaW33UEW6vQ3kgLrVjbcHWdhtYbp5j6ZeIMLBCW2LXiCZpzy3N3XqjlroV/s7KRZGeZMvRrLWhcM8YJ6sBTMUyQrF/Fk3d5f0WbAf7+opIA5rhxY+qgwWcE42S+HOOcMgb2IWtj5ycC/GXZ4o9qtk2WWJzCC+ygckXK7iI5pzhzScGmLCBrFnjyE5EP65FViVDSCcNl6hUFw7wwRtcQq5pnOu6wHRXsSpwNPufWaRqE4gd7hbrQoPqki4BLOYgGOQjZ3kJdc7MG47nVT62z1ScLsjH73q4yhABt8h1wmJl0LanKnE6EGkvZPCrDBBf0sZp12sJrxK8FGsPUKrBNZ5f4VuYkdzWS1ed2m5MZNHkDDd3LncYkQnLPh/MYAsnc2ud715njwiM62xIWYk9lIrmCWJU3JVcexRDS3/l7Po9UU5gx2xJKUW1tWOXKQ9GqQZYtn/rl804VMC1/tWr4X2XU1otulny5P8YkZ7BTG6zmkcW6raROxv7xkSUwE0LK2FmIpeBr29Zxn0QUYBtC09J+wVD5y00f/1jMU+k1jQmi1n2tpWgy6CxAHPPrQFhk0FsGXEDg82IELCKncbqkfqgT0QyV262YL2uxrcbcJ1Wrzf4llEX90hMa3oZK5wdQLrCBkXIlXverhGL3I09fR6lcXDMj8A3vrQrhr4bwYcWTH0hyJCPVLYIVxAFLwgV8wk700QOo3z32sSWKnyMHVx3brFB8I7rOPaW2Sc8rBxc1E9EvNcqiTwRd9QA3lIdR2Hpc4jyn5dx8PT6GTNGXNKhoTZDLrGMtYqXw4rVOXfkE8+opF4+/H602ockCI/IY7C65+sxooNgSg+9LpzImUt6GdtrvF8HrvJ0rUW6ZiAmGaQmqdrHfwC3K+QKU4UEWuLU1GPG+PKMrYEbtT/Hei7NPA5sOqHqbzaMsW7zj+enlhuEUeMdlxqfQs1QtTERrzX2HXv/JZ43rASH4ycuGftLR2v3AOHhtBCoYOCszhOiHYGmH0I9lwi8HDRTXjkuoRU1Zxmz9rSIQy35/aypV339OS7p4VydO5LibEiwugzaNHXCZalEPShhcWzTAq6+uCb+jA9ob+Za4yICRdIjgot/E5ZQNZl2VEhFmWR7pYSB7MB24z0ysVf3igAc50/3Lr4d7mQ7SgmtKFoejGOjvb75YOq/pEJGxCo1xIS+jhZklyLuiwmtyDe7Xmpn33fip22qx2iF7FYW71AyuT3tTPn2a7Jc2wXlPCAxB6dqx0eeX4fWA3+RIosUTxTyA7a/SM95pwN9FKYFPZ8TcTqDIq0Ntc57IdX5u1To=";
            Stimulsoft.Base.StiFontCollection.AddFontFile(CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Content/vazir") + "/Vazir.TTF");
            var report = new StiReport();
            report.RegBusinessObject("CODList", "CODList", Lst_model);
            report.RegBusinessObject("AttributesList", "Attributes", Lst_attributemodel.Where(p => p.Value != "0").ToList());

            if (new int[] { 667, 670, 722, 723 }.Contains(serviceId))
            {
                Picture AvatarPicture = null;
                var orderItem = orders.First().OrderItems.First();
                if (isAvatarValid(orderItem))
                {
                    var AvatarPictureId = orderItem.Order.Customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId);
                    if (AvatarPictureId > 0)
                    {
                        AvatarPicture = _pictureService.GetPictureById(AvatarPictureId);

                    }
                }
                report.Load(CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Report/") + "COD.mrt");
                if (AvatarPicture != null)
                {
                    var AvatarFilePath = _pictureService.GetThumbLocalPath(AvatarPicture, 0, false);
                    System.Drawing.Image myImage = System.Drawing.Image.FromFile(AvatarFilePath);
                    if (report.Dictionary.Variables != null)
                        report.Dictionary.Variables["Logo"].ValueObject = myImage;// (byte[])imageConvertor.ConvertTo(myImage, typeof(byte[]));
                }

            }
            else
            {
                string ImageName = "";
                if (new int[] { 705, 706 }.Contains(serviceId))
                {
                    ImageName = "DTS.png";
                }
                else if (serviceId == 711)
                {
                    ImageName = "TPG.png";
                }
                else if (new int[] { 731 }.Contains(serviceId))
                {
                    ImageName = "Mahex.png";
                }
                else if (new int[] { 733 }.Contains(serviceId))
                {
                    ImageName = "Kalaresan.png";
                }
                else if (new int[] { 667, 670, 722, 723 }.Contains(serviceId))
                {
                    ImageName = "POSTBAR.jpeg";
                }
                else if (new int[] { 715, 713 }.Contains(serviceId))
                {
                    ImageName = "CHAPAR.PNG";
                }
                System.Drawing.Image myImage = System.Drawing.Image.FromFile(CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Images/" + ImageName));
                // var imageConvertor = new System.Drawing.ImageConverter();
                report.Load(CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Report/") + "COD_DTS.mrt");
                if (report.Dictionary.Variables != null)
                    report.Dictionary.Variables["Logo"].ValueObject = myImage;// (byte[])imageConvertor.ConvertTo(myImage, typeof(byte[]));
            }

            report.Compile();
            string postBarRule = "";// getPostBarRule();
            if (!string.IsNullOrEmpty(postBarRule))
            {
                postBarRule = postBarRule.Replace("dir=\"rtl\"", "").Replace("style=\"text-align: right;\"", "")
                    .Replace("style=\"color: #ff6600;\"", "");
                postBarRule = $"<h1>قوانین {_storeContext.CurrentStore.Name}</h1><br>" + postBarRule;
            }
            report["PostBarRule"] = postBarRule ?? "";
            report.Render();

            var settingsPdf = new StiPdfExportSettings();
            var servicePdf = new StiPdfExportService();
            settingsPdf.ImageQuality = 75;
            settingsPdf.ImageResolution = 100;
            //settingsPdf.StandardPdfFonts = true;
            settingsPdf.EmbeddedFonts = true;
            settingsPdf.UseUnicode = true;
            MemoryStream tempStram = new MemoryStream();
            servicePdf.ExportTo(report, tempStram, settingsPdf);

            #region AddPdfFileToFactor;
            //if (_storeContext.CurrentStore.Id == 5)
            //{
            var pdfFileStream = ReadPdfFromFile(PageSize.A4);
            PdfCopyFields copy = new PdfCopyFields(stream);
            MemoryStream firstPdf = new MemoryStream(((MemoryStream)tempStram).ToArray());
            copy.AddDocument(new PdfReader(firstPdf));

            //pdfFileStream.Position = 0;
            MemoryStream secoundtPdf = new MemoryStream(((MemoryStream)pdfFileStream).ToArray());
            copy.AddDocument(new PdfReader(secoundtPdf));


            firstPdf.Dispose();
            secoundtPdf.Dispose();
            tempStram.Dispose();
            pdfFileStream.Dispose();
            copy.Close();
            //}
            #endregion

        }

        public int getEngPriceForFreePost(int shipmentId)
        {
            string query = $@"SELECT
	                        TCCD.EngPrice
                        FROM
	                        dbo.Tb_CodCalculationDetailes AS TCCD
                        WHERE
	                        TCCD.ShipmentId  = {shipmentId}";
            return _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
        }
        public int getCodPriceForFreePost(int shipmentId)
        {
            string query = $@"SELECT
	                            TCCD.CodTranPrice
                            FROM
	                            dbo.Tb_CodCalculationDetailes AS TCCD
                            WHERE
	                            TCCD.ShipmentId  = {shipmentId}";
            return _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
        }

        private List<CODAttributesModel> getOrderitems(int orderId)
        {
            string query = $@"SELECT
	                               	CASE WHEN TOIAV.PropertyAttrName  LIKE N'%ثبت مرسوله%' THEN N'ثبت الکترونیک'
								When TOIAV.PropertyAttrName LIKE N'%ارزش افزوده%' THEN N'خدمات نماینده'
								When TOIAV.PropertyAttrName LIKE N'%هزینه توزیع%' THEN N'هزینه توزیع'
								When TOIAV.PropertyAttrName = N'هزینه جمع آوری هر مرسوله' THEN N'جمع آوری جزء'
								When TOIAV.PropertyAttrName = N'حق ثب انبوه' THEN N' ثبت انبوه'
								When TOIAV.PropertyAttrName LIKE N'%وجهی%' THEN N'کالا'
								When TOIAV.PropertyAttrName LIKE N'%پرینتر%' THEN N'پرینتر'
								When TOIAV.PropertyAttrName LIKE N'%پیامک%' THEN N'پیامک'
								ELSE TOIAV.PropertyAttrName END Name,
	                                CAST((CASE WHEN TOIAV.PropertyAttrName  LIKE N'%ثبت مرسوله%' 
								    OR TOIAV.PropertyAttrName LIKE N'%ارزش افزوده%' 
                                    OR TOIAV.PropertyAttrName LIKE N'%هزینه توزیع%'
								    OR TOIAV.PropertyAttrName = N'هزینه جمع آوری هر مرسوله'
								    OR TOIAV.PropertyAttrName = N'حق ثب انبوه'
								    OR TOIAV.PropertyAttrName LIKE N'%وجهی%'
								    THEN dbo.GetOnlyNumbers(ISNULL(TOIAV.PropertyAttrValueText,'0')) ELSE TOIAV.PropertyAttrValuePrice END) AS NVARCHAR(20)) Value,
	                                S.Id ShipmentId
                                INTO #tb1
                                FROM
	                                dbo.[Order] AS O
	                                INNER JOIN dbo.Shipment AS S ON S.OrderId = O.Id
	                                INNER JOIN dbo.ShipmentItem AS SI ON SI.ShipmentId = S.Id
	                                INNER JOIN dbo.OrderItem AS OI ON Si.OrderItemId = oi.Id
	                                INNER JOIN dbo.Tb_OrderItemAttributeValue AS TOIAV ON oi.Id = TOIAV.OrderItemId
                                WHERE
	                                O.Id= {orderId}
	                                AND ((TOIAV.PropertyAttrValuePrice IS NOT NULL AND  TOIAV.PropertyAttrValuePrice > 0) 
			                                OR TOIAV.PropertyAttrName LIKE N'%ثبت مرسوله%' 
			                                OR TOIAV.PropertyAttrName LIKE N'%ارزش افزوده%'
                                            OR TOIAV.PropertyAttrName LIKE N'%هزینه توزیع%'
			                                OR TOIAV.PropertyAttrName = N'هزینه جمع آوری هر مرسوله'
			                                OR TOIAV.PropertyAttrName = N'حق ثب انبوه'
			                                OR TOIAV.PropertyAttrName LIKE N'%وجهی%')
			                                AND TOIAV.PropertyAttrName NOT LIKE N'%وزن بسته%'
                                SELECT
								    T.Name
								    , CAST(CASE WHEN T.Name LIKE N'%کالا%' THEN T.Value ELSE CAST(t.Value AS INT)+ CAST(t.Value AS INT)*9/100 END AS NVARCHAR(20)) Value
								    , T.ShipmentId
							    FROM
								    #tb1 AS T";
            return _dbContext.SqlQuery<CODAttributesModel>(query, new object[0]).ToList();
        }
        public void Print_Valet(IList<Order> orders, Stream stream, bool isValet)
        {
            string _title = "";
            if (isValet)
            {
                _title = "رسید افزايش موجودی کیف پول";
            }
            else
            {
                _title = "رسید خرید حق اشتراک سالیانه پس کرایه";
            }

            foreach (var order in orders)
            {
                int serviceId = 0;
                int itemDiscount = 0;
                if (order.OrderDiscount > decimal.Zero)
                {
                    itemDiscount = Convert.ToInt32(order.OrderDiscount / (order.OrderItems.Sum(p => p.Quantity)));
                }

                bool IsmultiShipment = _extendedShipmentService.IsMultiShippment(order);
                foreach (var orderItem in order.OrderItems)
                {
                    serviceId = orderItem.Product.ProductCategories.First().CategoryId;
                    var store = _storeService.GetStoreById(orderItem.Order.StoreId) ?? _storeContext.CurrentStore;
                    for (int i = 0; i < orderItem.Quantity; i++)
                    {

                        var pm = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
                        string paymentMethodName = pm != null ? pm.PluginDescriptor.FriendlyName : order.PaymentMethodSystemName;
                        Valet _Valet = new Valet();
                        var orderStore = _storeService.GetStoreById(orderItem.Order.StoreId);

                        _Valet.StorName = orderStore.Name;
                        _Valet.StorePhone = order.StoreId == 5 ? "021-91300250" : "+989422020249";
                        _Valet.StoreAddress = orderStore.Url;
                        _Valet.StoreEmail = order.StoreId == 5 ? "info@postex.ir" : "info@postbar.ir";
                        _Valet.InvoiceNumber = order.Id.ToString();
                        _Valet.DateInvoce = MiladyToShamsi2(orderItem.Order.CreatedOnUtc.ToLocalTime());
                        _Valet.Username = order.Customer.Username;
                        _Valet.Fname = string.IsNullOrEmpty(order.Customer.GetFullName()) ? (order.BillingAddress != null ? (order.BillingAddress.FirstName ?? "" + " " + order.BillingAddress.LastName) :
                            order.Customer.Username) : order.Customer.Username;
                        _Valet.Lname = "";
                        _Valet.NameDargah = paymentMethodName;
                        _Valet.Mablagh = order.OrderTotal.ToString("N0") + " ريال";
                        _Valet.Note = "";
                        _Valet.NoteBank = ((order.OrderStatus == OrderStatus.Complete || order.OrderStatus == OrderStatus.Processing) ? "تراکنش با موفقیت انجام شد" :
                            (order.OrderStatus == OrderStatus.Pending ? "سفارش در حالت معلق می باشد" : "سفارش کنسل شده"));

                        Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHm4lqN+V3/fvJECXRrqdoEPZqYyFXh3g3K9oCDFvgzMl4c9KudQQwMJ6nO6w7rHz59BhwYgDE0QzKtjY2WxEejTNewbNDXY492M2mDsK1Hb4t6MoFYGbSoID0gow3VC5cFhvcOKVoagiHq6/4iqFc3RS7nZQBp95+WB6N1fR//H7OBlvfuBKldvC1pJh6pTW/7HRdOclErt/EGwivx9SpuDNabWWBfIZJBdJsvKjpoIsDQGibWk8Y8F9V1mCLf88FurWwEZ9WzXePbJn+wfabHM+a7pqDAXhsaW33UEW6vQ3kgLrVjbcHWdhtYbp5j6ZeIMLBCW2LXiCZpzy3N3XqjlroV/s7KRZGeZMvRrLWhcM8YJ6sBTMUyQrF/Fk3d5f0WbAf7+opIA5rhxY+qgwWcE42S+HOOcMgb2IWtj5ycC/GXZ4o9qtk2WWJzCC+ygckXK7iI5pzhzScGmLCBrFnjyE5EP65FViVDSCcNl6hUFw7wwRtcQq5pnOu6wHRXsSpwNPufWaRqE4gd7hbrQoPqki4BLOYgGOQjZ3kJdc7MG47nVT62z1ScLsjH73q4yhABt8h1wmJl0LanKnE6EGkvZPCrDBBf0sZp12sJrxK8FGsPUKrBNZ5f4VuYkdzWS1ed2m5MZNHkDDd3LncYkQnLPh/MYAsnc2ud715njwiM62xIWYk9lIrmCWJU3JVcexRDS3/l7Po9UU5gx2xJKUW1tWOXKQ9GqQZYtn/rl804VMC1/tWr4X2XU1otulny5P8YkZ7BTG6zmkcW6raROxv7xkSUwE0LK2FmIpeBr29Zxn0QUYBtC09J+wVD5y00f/1jMU+k1jQmi1n2tpWgy6CxAHPPrQFhk0FsGXEDg82IELCKncbqkfqgT0QyV262YL2uxrcbcJ1Wrzf4llEX90hMa3oZK5wdQLrCBkXIlXverhGL3I09fR6lcXDMj8A3vrQrhr4bwYcWTH0hyJCPVLYIVxAFLwgV8wk700QOo3z32sSWKnyMHVx3brFB8I7rOPaW2Sc8rBxc1E9EvNcqiTwRd9QA3lIdR2Hpc4jyn5dx8PT6GTNGXNKhoTZDLrGMtYqXw4rVOXfkE8+opF4+/H602ockCI/IY7C65+sxooNgSg+9LpzImUt6GdtrvF8HrvJ0rUW6ZiAmGaQmqdrHfwC3K+QKU4UEWuLU1GPG+PKMrYEbtT/Hei7NPA5sOqHqbzaMsW7zj+enlhuEUeMdlxqfQs1QtTERrzX2HXv/JZ43rASH4ycuGftLR2v3AOHhtBCoYOCszhOiHYGmH0I9lwi8HDRTXjkuoRU1Zxmz9rSIQy35/aypV339OS7p4VydO5LibEiwugzaNHXCZalEPShhcWzTAq6+uCb+jA9ob+Za4yICRdIjgot/E5ZQNZl2VEhFmWR7pYSB7MB24z0ysVf3igAc50/3Lr4d7mQ7SgmtKFoejGOjvb75YOq/pEJGxCo1xIS+jhZklyLuiwmtyDe7Xmpn33fip22qx2iF7FYW71AyuT3tTPn2a7Jc2wXlPCAxB6dqx0eeX4fWA3+RIosUTxTyA7a/SM95pwN9FKYFPZ8TcTqDIq0Ntc57IdX5u1To=";
                        Stimulsoft.Base.StiFontCollection.AddFontFile(CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Content/vazir") + "/Vazir.TTF");
                        var report = new StiReport();
                        report.Load(CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Report/") + "Valet.mrt");
                        report.RegBusinessObject("Valet", _Valet);
                        string logoUrl = "";
                        if (order.StoreId == 3)
                        {
                            logoUrl = CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Images/" + "POSTBAR.jpeg");
                        }
                        else
                        {
                            logoUrl = CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Images/" + "POSTEX.jpeg");
                        }
                        System.Drawing.Image myImage = System.Drawing.Image.FromFile(logoUrl);
                        report.Dictionary.Variables["ImageStore"].ValueObject = myImage;
                        report.Dictionary.Variables["title"].Value = _title;
                        report.Compile();
                        report.Render();
                        var settingsPdf = new StiPdfExportSettings();
                        var servicePdf = new StiPdfExportService();
                        //settingsPdf.ImageQuality = 75;
                        //settingsPdf.ImageResolution = 100;
                        settingsPdf.EmbeddedFonts = true;
                        settingsPdf.UseUnicode = true;
                        servicePdf.ExportTo(report, stream, settingsPdf);
                    }
                }
            }
        }

        #region Cod Value
        private CodValues getCodValues(int shipmentId)
        {
            string query = $@"SELECT
                                ISNULL(SA.CodCost,0) CodCost,
                                ISNULL(SA.CodBmValue,0) CodBmValue,
                                ISNULL(dbo.GetOnlyNumbers(dbo.AttributeProductCost_COD(OI.Id)),0)  GoodsValue,
                                CAST(((ISNUll(dbo.GetOnlyNumbers(dbo.AttributeProductCost_COD(OI.Id)),0) - ISNULL(SA.CodBmValue,0))* -1) + ISNULL(SA.CodCost,0) AS INT) EngCost
                            FROM
	                            dbo.Shipment AS S
	                            INNER JOIN dbo.ShipmentItem AS SI ON SI.ShipmentId = S.Id
	                            INNER JOIN dbo.OrderItem AS OI ON Si.OrderItemId = OI.Id
	                            INNER JOIN dbo.ShipmentAppointment AS SA ON SA.ShipmentId = S.Id
                            WHERE
	                            S.id= {shipmentId}";
            return _dbContext.SqlQuery<CodValues>(query, new object[0]).SingleOrDefault();

        }
        public class CodValues
        {
            public int CodCost { get; set; }
            public int CodBmValue { get; set; }
            public int GoodsValue { get; set; }
            public int EngCost { get; set; }
        }
        #endregion

        protected void PrintAddresses(int vendorId, Language lang, Font titleFont, OrderItem orderItem, Font font, Document doc
            , Shipment shipment, Address shippingAddress, int serviceId)
        {
            var addressTable = new PdfPTable(2) { RunDirection = GetDirection(lang) };
            addressTable.DefaultCell.Border = Rectangle.ALIGN_JUSTIFIED_ALL;
            addressTable.WidthPercentage = 100f;
            addressTable.SetWidths(new[] { 50, 50 });

            //billing info
            var msgTaable = new PdfPTable(1) { RunDirection = GetDirection(lang) };
            string IsCodOrNo = "پستچی محترم، مبلغ مرسوله تسویه شده"
                + Environment.NewLine
                + Environment.NewLine
                + "لطفا از گرفتن هرگونه وجه خوداری نمایید"
                + Environment.NewLine
                + Environment.NewLine;
            if (orderItem.Product.Id == 10445)
                IsCodOrNo = "";
            msgTaable.AddCell(new Paragraph(
                IsCodOrNo + "*********************گیرنده محترم**********************"
                + Environment.NewLine
                + "تحویل بسته از جانب شما به منزله تایید صحت و سلامت مرسوله "
                + Environment.NewLine
                + "شماست، قبل از تحویل از سلامت مرسوله مطمئن شوید"
                , titleFont));
            addressTable.AddCell(msgTaable);
            PrintBillingInfo(vendorId, lang, titleFont, orderItem.Order, font, addressTable);

            //shipping info
            PrintShippingInfo(lang, orderItem.Order, titleFont, font, addressTable, shippingAddress, serviceId);

            if (shipment != null)
            {
                var barcodeTable = PrintBarcode(shipment, lang, titleFont, font);
                if (barcodeTable != null)
                    addressTable.AddCell(barcodeTable);
                else
                {
                    addressTable.AddCell(new Paragraph("  "));
                }
            }
            else
            {
                addressTable.AddCell(new Paragraph("  "));
            }

            doc.Add(addressTable);
            doc.Add(new Paragraph(" "));
        }
        protected override void PrintBillingInfo(int vendorId, Language lang, Font titleFont, Order order, Font font, PdfPTable addressTable)
        {
            const string indent = "   ";
            var billingAddress = new PdfPTable(1) { RunDirection = GetDirection(lang) };
            billingAddress.DefaultCell.Border = Rectangle.NO_BORDER;

            billingAddress.AddCell(new Paragraph("فرستنده", titleFont));

            if (_addressSettings.CompanyEnabled && !string.IsNullOrEmpty(order.BillingAddress.Company))
                billingAddress.AddCell(GetParagraph("PDFInvoice.Company", indent, lang, font, order.BillingAddress.Company));

            billingAddress.AddCell(GetParagraph("PDFInvoice.Name", indent, lang, font, order.BillingAddress.FirstName + " " + order.BillingAddress.LastName));
            if (_addressSettings.PhoneEnabled)
                billingAddress.AddCell(GetParagraph("PDFInvoice.Phone", indent, lang, font, order.BillingAddress.PhoneNumber));

            if (_addressSettings.FaxEnabled && !string.IsNullOrEmpty(order.BillingAddress.FaxNumber))
                billingAddress.AddCell(GetParagraph("PDFInvoice.Fax", indent, lang, font, order.BillingAddress.FaxNumber));
            string address = "";
            if (_addressSettings.CountryEnabled && order.BillingAddress.Country != null)
                address = indent + order.BillingAddress.Country.GetLocalized(x => x.Name, lang.Id);
            //billingAddress.AddCell(new Paragraph(indent + order.BillingAddress.Country.GetLocalized(x => x.Name, lang.Id),
            //    font));

            if (_addressSettings.CityEnabled || _addressSettings.StateProvinceEnabled)
                address +=
                    $"{indent}{(order.BillingAddress.StateProvince != null ? order.BillingAddress.StateProvince.GetLocalized(x => x.Name, lang.Id) : "")} ,"
                    + $"{order.BillingAddress.City ?? ""}";
            billingAddress.AddCell(new Paragraph(
                        address,
                        font));

            if (_addressSettings.StreetAddressEnabled)
                billingAddress.AddCell(GetParagraph("PDFInvoice.Address", indent, lang, font, order.BillingAddress.Address1));
            if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(order.BillingAddress.Address2))
                billingAddress.AddCell(GetParagraph("PDFInvoice.Address2", indent, lang, font, order.BillingAddress.Address2));

            if (_addressSettings.ZipPostalCodeEnabled)
                billingAddress.AddCell(new Paragraph($"{indent}{order.BillingAddress.ZipPostalCode}", font));

            //VAT number
            if (!string.IsNullOrEmpty(order.VatNumber))
                billingAddress.AddCell(GetParagraph("PDFInvoice.VATNumber", indent, lang, font, order.VatNumber));

            //custom attributes
            var customBillingAddressAttributes =
                _addressAttributeFormatter.FormatAttributes(order.BillingAddress.CustomAttributes);
            if (!string.IsNullOrEmpty(customBillingAddressAttributes))
            {
                //TODO: we should add padding to each line (in case if we have several custom address attributes)
                billingAddress.AddCell(
                    new Paragraph(indent + HtmlHelper.ConvertHtmlToPlainText(customBillingAddressAttributes, true, true), font));
            }

            //vendors payment details
            if (vendorId == 0)
            {
                //payment method


                //custom values
                var customValues = order.DeserializeCustomValues();
                if (customValues != null)
                {
                    foreach (var item in customValues)
                    {
                        billingAddress.AddCell(new Paragraph(" "));
                        billingAddress.AddCell(new Paragraph(indent + item.Key + ": " + item.Value, font));
                        billingAddress.AddCell(new Paragraph());
                    }
                }
            }
            addressTable.AddCell(billingAddress);
        }
        protected void PrintShippingInfo(Language lang, Order order, Font titleFont, Font font, PdfPTable addressTable, Address shippingAdd, int serviceId)
        {
            var shippingAddress = new PdfPTable(1)
            {
                RunDirection = GetDirection(lang)
            };
            shippingAddress.DefaultCell.Border = Rectangle.NO_BORDER;

            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                //cell = new PdfPCell();
                //cell.Border = Rectangle.NO_BORDER;
                const string indent = "   ";

                if (!order.PickUpInStore)
                {
                    if (shippingAdd == null)
                        throw new NopException($"Shipping is required, but address is not available. Order ID = {order.Id}");

                    shippingAddress.AddCell(GetParagraph("PDFInvoice.ShippingInformation", lang, titleFont));
                    if (!string.IsNullOrEmpty(shippingAdd.Company))
                        shippingAddress.AddCell(GetParagraph("PDFInvoice.Company", indent, lang, font, shippingAdd.Company));
                    if (serviceId == 733)
                        shippingAddress.AddCell(GetParagraph("PDFInvoice.Name", indent, lang, font, "امنیتو" + " - " + shippingAdd.FirstName + " " + shippingAdd.LastName));
                    else
                        shippingAddress.AddCell(GetParagraph("PDFInvoice.Name", indent, lang, font, shippingAdd.FirstName + " " + shippingAdd.LastName));
                    if (_addressSettings.PhoneEnabled)
                        shippingAddress.AddCell(GetParagraph("PDFInvoice.Phone", indent, lang, font, shippingAdd.PhoneNumber));
                    if (_addressSettings.FaxEnabled && !string.IsNullOrEmpty(shippingAdd.FaxNumber))
                        shippingAddress.AddCell(GetParagraph("PDFInvoice.Fax", indent, lang, font, shippingAdd.FaxNumber));
                    string address = "";
                    if (_addressSettings.CountryEnabled && shippingAdd.Country != null)
                        address = indent + shippingAdd.Country.GetLocalized(x => x.Name, lang.Id);
                    //shippingAddress.AddCell(
                    //    new Paragraph(indent + shippingAdd.Country.GetLocalized(x => x.Name, lang.Id), font));

                    string StateName = shippingAdd.StateProvince != null ?
                        (new int[] { 4, 581, 582, 583, 584, 585, 580, 579 }.Contains(shippingAdd.StateProvinceId.Value) ?
                        "شهر تهران" : shippingAdd.StateProvince.GetLocalized(x => x.Name, lang.Id)) : "";
                    if (_addressSettings.CityEnabled || _addressSettings.StateProvinceEnabled)
                        address += $"{indent}{(StateName)} ,"
                        + $"{shippingAdd.City ?? ""}";
                    shippingAddress.AddCell(new Paragraph(address, font));

                    if (_addressSettings.StreetAddressEnabled)
                        shippingAddress.AddCell(GetParagraph("PDFInvoice.Address", indent, lang, font, shippingAdd.Address1));
                    if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(shippingAdd.Address2))
                        shippingAddress.AddCell(GetParagraph("PDFInvoice.Address2", indent, lang, font, shippingAdd.Address2));

                    if (_addressSettings.ZipPostalCodeEnabled)
                        shippingAddress.AddCell(new Paragraph($"{indent}{shippingAdd.ZipPostalCode}", font));
                    //custom attributes
                    var customShippingAddressAttributes =
                        _addressAttributeFormatter.FormatAttributes(shippingAdd.CustomAttributes);
                    if (!string.IsNullOrEmpty(customShippingAddressAttributes))
                    {
                        //TODO: we should add padding to each line (in case if we have several custom address attributes)
                        shippingAddress.AddCell(new Paragraph(
                            indent + HtmlHelper.ConvertHtmlToPlainText(customShippingAddressAttributes, true, true), font));
                    }
                    shippingAddress.AddCell(new Paragraph(" "));
                }
                else if (order.PickupAddress != null)
                {
                    shippingAddress.AddCell(GetParagraph("PDFInvoice.Pickup", lang, titleFont));
                    if (!string.IsNullOrEmpty(order.PickupAddress.Address1))
                        shippingAddress.AddCell(new Paragraph(
                            $"{indent}{string.Format(_localizationService.GetResource("PDFInvoice.Address", lang.Id), order.PickupAddress.Address1)}",
                            font));
                    if (!string.IsNullOrEmpty(order.PickupAddress.City))
                        shippingAddress.AddCell(new Paragraph($"{indent}{order.PickupAddress.City}", font));
                    if (order.PickupAddress.Country != null)
                        shippingAddress.AddCell(
                            new Paragraph($"{indent}{order.PickupAddress.Country.GetLocalized(x => x.Name, lang.Id)}", font));
                    if (!string.IsNullOrEmpty(order.PickupAddress.ZipPostalCode))
                        shippingAddress.AddCell(new Paragraph($"{indent}{order.PickupAddress.ZipPostalCode}", font));
                    shippingAddress.AddCell(new Paragraph(" "));
                }
                //shippingAddress.AddCell(GetParagraph("PDFInvoice.ShippingMethod", indent, lang, font, order.ShippingMethod));
                //shippingAddress.AddCell(new Paragraph());

                //var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
                //var paymentMethodStr = paymentMethod != null
                //    ? paymentMethod.GetLocalizedFriendlyName(_localizationService, lang.Id)
                //    : order.PaymentMethodSystemName;
                //if (!string.IsNullOrEmpty(paymentMethodStr))
                //{
                //    shippingAddress.AddCell(new Paragraph(" "));
                //    shippingAddress.AddCell(GetParagraph("PDFInvoice.PaymentMethod", indent, lang, font, paymentMethodStr));
                //    shippingAddress.AddCell(new Paragraph());
                //}

                addressTable.AddCell(shippingAddress);
            }
            else
            {
                shippingAddress.AddCell(new Paragraph());
                addressTable.AddCell(shippingAddress);
            }
        }
        protected void PrintTotals(int vendorId, Language lang, OrderItem orderItem, Font font, Font titleFont, Document doc, OrderBill bill,
            Shipment shipment1, int itemDiscount, int HagheMaghar, int _posthagheMaghar, Shipment shipment, int _serviceId)
        {
            //vendors cannot see totals
            if (vendorId != 0)
                return;

            //subtotal
            var totalsTable = new PdfPTable(1)
            {
                RunDirection = GetDirection(lang),
                WidthPercentage = 50f
            };
            totalsTable.DefaultCell.Border = Rectangle.NO_BORDER;
            totalsTable.HorizontalAlignment = Element.ALIGN_CENTER;
            //var shipment1 = orderItem.ship .Shipments.FirstOrDefault();
            if (shipment1 != null)
            {
                string weight = "وزن: " + (_extendedShipmentService.GetItemWeightFromAttr(orderItem)) + " گرم ";
                weight += " " + ("-" + _extendedShipmentService.getOrderItemContent(orderItem) ?? "");

                var Dimensions = _extendedShipmentService.getDimensions(orderItem.Id);
                if (Dimensions.Item1 != 0 && !orderItem.Order.Customer.IsInCustomerRole("Collector"))//نماینده جمع آور از این قاعده مستثنی شده
                {
                    weight += " ابعاد : " + ">>>>>" + (Dimensions.Item1.ToString() + "*" + Dimensions.Item2.ToString() + "*" + Dimensions.Item3.ToString()) + "<<<<<";
                }
                var p = GetPdfCell(weight, font);
                p.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                p.Border = Rectangle.NO_BORDER;
                if (!string.IsNullOrEmpty(shipment.AdminComment) && new int[] { 712, 713, 714, 715 }.Contains(_serviceId))
                    p.HorizontalAlignment = Element.ALIGN_CENTER;
                else
                    p.HorizontalAlignment = Element.ALIGN_RIGHT;
                totalsTable.AddCell(p);
            }

            if (!string.IsNullOrEmpty(shipment.AdminComment) && new int[] { 712, 713, 714, 715 }.Contains(_serviceId))
            {
                var barcodeTable = PrintChaparSecoundBarcode(shipment, lang, titleFont, font);
                barcodeTable.HorizontalAlignment = Element.ALIGN_CENTER;
                totalsTable.AddCell(barcodeTable);
            }

            int TotalPrice = 0;
            int HagheSabt = _extendedShipmentService.getHagheSabt(orderItem.Id);
            int ValueAddedByAgnt = 0;
            if (orderItem.Order.Customer.IsInCustomerRole("mini-Administrators"))
            {
                ValueAddedByAgnt = _extendedShipmentService.GetValueAddedbyAgent(orderItem);
            }
            var mark = _extendedShipmentService.GetOrderRegistrationMethod(orderItem.Order);
            var isPostService = _extendedShipmentService.IsPostService(orderItem.Product.ProductCategories.First().CategoryId);
            if (!isPostService || mark == OrderRegistrationMethod.Ap || mark == OrderRegistrationMethod.bidok)
            {

                var prices = _extendedShipmentService.getEngAndPostPrice(orderItem);
                if (prices == null)
                {
                    if (!_extendedShipmentService.CheckHasValidPrice(orderItem.Order))
                    {
                        throw new Exception($"قیمت مربوط به آیتم شماره {orderItem.Id} از سرویس مربوطه دریافت نشده");
                    }
                    else
                    {
                        prices = _extendedShipmentService.getEngAndPostPrice(orderItem);
                    }
                }
                int engPrice = prices.EngPrice;
                if (!isPostService)
                {
                    int foreignPrices = 0;
                    if (_extendedShipmentService.IsOrderForeign(orderItem.Order))
                    {
                        var RelatedOrder = _relatedOrders_Service.GetTb_RelatedOrders_By_ParentOrderId(orderItem.OrderId);
                        if (RelatedOrder != null)
                        {
                            var number = orderItem.Order.OrderItems.Sum(p => p.Quantity);
                            if (number > 0)
                            {
                                foreignPrices = (RelatedOrder.ChildOrderPrice / number);
                            }
                        }
                    }
                    TotalPrice = (engPrice + prices.AttrPrice + HagheMaghar + _posthagheMaghar);
                    TotalPrice += (TotalPrice * 9) / 100;
                    TotalPrice += prices.IncomePrice + foreignPrices;
                }
                else
                {
                    TotalPrice = (prices.IncomePrice + engPrice + prices.AttrPrice + HagheMaghar + _posthagheMaghar + ValueAddedByAgnt);
                    TotalPrice += (TotalPrice * 9) / 100;
                }
            }
            else
            {
                TotalPrice = (int)(_extendedShipmentService.GetItemPriceFromAttr(orderItem)
                                 + bill.AttributePrice.Sum(p => p.value));
                TotalPrice += (HagheMaghar + _posthagheMaghar + HagheSabt + ValueAddedByAgnt);
                TotalPrice += (TotalPrice * 9) / 100;
            }
            TotalPrice -= itemDiscount;

            bool IsMahexCod = (orderItem.Product.Id == 10445);
            int CodGoodPrice = _extendedShipmentService.getGoodsPrice(orderItem);
            if (IsMahexCod && CodGoodPrice > 0)
            {
                TotalPrice += CodGoodPrice;
            }
            //order total
            //var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(TotalPrice, orderItem.Order.CurrencyRate);
            //var orderTotalStr = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, orderItem.Order.CustomerCurrencyCode, false, lang);

            var pTotal = GetPdfCell($"{_localizationService.GetResource("PDFInvoice.OrderTotal", lang.Id)} {TotalPrice.ToString("N0") + " ريال"}" + "/جهت استحضار", titleFont);
            if (!string.IsNullOrEmpty(shipment.AdminComment) && new int[] { 712, 713, 714, 715 }.Contains(_serviceId))
                pTotal.HorizontalAlignment = Element.ALIGN_CENTER;
            else
                pTotal.HorizontalAlignment = Element.ALIGN_RIGHT;
            pTotal.Border = Rectangle.NO_BORDER;
            pTotal.Phrase.Add(new Phrase(Environment.NewLine));
            totalsTable.AddCell(pTotal);


            var bigFont = GetFont("Vazir-FD.TTF");
            bigFont.SetStyle(Font.BOLD);
            bigFont.Color = BaseColor.BLACK;
            bigFont.Size = 16;
            var catInfo = _extendedShipmentService.GetCategoryInfo(orderItem.Product);
            string text = "سرویس :" + catInfo.CategoryName;

            //if (new int[] { }.Contains(catInfo.CategoryId))
            //{
            //text += "\r\n" + "مرسوله دارای بیمه می باشد، به صورت خاص ارسال شود،پستچی می بایست تا زمان بازدید مرسوله توسط گیرنده حضور داشته باشد ";
            //}
            //var ServiceType = GetPdfCell(text, bigFont);
            /// ServiceType.HorizontalAlignment = Element.ALIGN_RIGHT;
            //ServiceType.Border = Rectangle.NO_BORDER;
            //ServiceType.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            //totalsTable.AddCell(ServiceType);

            doc.Add(totalsTable);
        }
        protected void PrintHeader(PdfSettings pdfSettingsByStore, Language lang, Order order, Font font, Font titleFont
            , Document doc, Shipment shipment, OrderItem orderItem)
        {
            // base.PrintHeader(pdfSettingsByStore, lang, order, font, titleFont, doc);
            //logo
            var logoPicture = _pictureService.GetPictureById(pdfSettingsByStore.LogoPictureId);
            var logoExists = logoPicture != null;
            Nop.Core.Domain.Media.Picture AvatarPicture = null;
            string ImageUrl = "";
            int serviceId = orderItem.Product.ProductCategories.First().CategoryId;

            if (order.StoreId == 5)
            {
                string ImageName = "";
                if (new int[] { 699, 703 }.Contains(serviceId))
                {
                    ImageName = "DTS.png";
                }
                else if (new int[] { 709, 710 }.Contains(serviceId))
                {
                    ImageName = "TPG.png";
                }
                else if (new int[] { 714, 712 }.Contains(serviceId))
                {
                    ImageName = "CHAPAR.png";
                }
                else if (new int[] { 708, 707 }.Contains(serviceId))
                {
                    ImageName = "PDE.png";
                }
                else if (new int[] { 702 }.Contains(serviceId))
                {
                    ImageName = "YARBOX.png";
                }
                else if (new int[] { 701 }.Contains(serviceId))
                {
                    ImageName = "UBAAR.jpeg";
                }
                else if (new int[] { 730, 731 }.Contains(serviceId))
                {
                    ImageName = "Mahex.png";
                }
                else if (new int[] { 733 }.Contains(serviceId))
                {
                    ImageName = "Kalaresan.png";
                }
                if (ImageName != "")
                    ImageUrl = CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Images/" + ImageName);
            }
            if (isAvatarValid(orderItem))
            {
                var AvatarPictureId = order.Customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId);
                if (AvatarPictureId > 0)
                {
                    AvatarPicture = _pictureService.GetPictureById(AvatarPictureId);

                }
            }
            int headerTable_ColNum = 1;
            if (AvatarPicture != null)
                headerTable_ColNum++;
            if (logoExists)
                headerTable_ColNum++;
            if (ImageUrl != "")
                headerTable_ColNum++;
            //header
            var headerTable = new PdfPTable(headerTable_ColNum)
            {
                RunDirection = GetDirection(lang)
            };
            headerTable.DefaultCell.Border = Rectangle.NO_BORDER;
            //var serviceId = order.OrderItems.First().Product.ProductCategories.First().CategoryId;
            int storeId = 3;
            if (!_extendedShipmentService.IsPostService(serviceId))
                storeId = order.StoreId;
            //store info
            var store = _storeService.GetStoreById(storeId) ?? _storeContext.CurrentStore;
            var anchor = new Anchor(store.Url.Trim('/'), font)
            {
                Reference = store.Url
            };

            var cellHeader = GetPdfCell("شماره درخواست : " + order.CustomOrderNumber, titleFont);
            if (shipment != null)
                cellHeader.Phrase.Add(new Paragraph(("شماره محموله : " + shipment.Id.ToString())));
            cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
            //cellHeader.Phrase.Add(new Phrase(anchor));
            //cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
            //cellHeader.Phrase.Add(GetParagraph("PDFInvoice.OrderDate", lang, font, MiladyToShamsi(order.CreatedOnUtc.ToLocalTime())));
            var SendDate = shipment != null ? shipment.CreatedOnUtc.ToLocalTime() : (DateTime?)null;
            cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
            cellHeader.Phrase.Add(new Paragraph(("تاریخ ارجاع " + ":" + (SendDate == null ? "" : MiladyToShamsi2(SendDate.Value, true)))));
            cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
            cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
            cellHeader.Border = Rectangle.NO_BORDER;
            bool IsMahexCod = (orderItem.Product.Id == 10445);
            int CodGoodPrice = _extendedShipmentService.getGoodsPrice(orderItem);
            if (IsMahexCod && CodGoodPrice > 0)
            {
                cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
                cellHeader.Phrase.Add(new Paragraph(">>>>> پرداخت در محل گیرنده <<<<<"));
            }


            headerTable.AddCell(cellHeader);
            if (headerTable_ColNum == 1)
                headerTable.SetWidths(new[] { 1f });
            else if (headerTable_ColNum == 2)
                headerTable.SetWidths(lang.Rtl ? new[] { 0.2f, 0.8f } : new[] { 0.8f, 0.2f });
            else if (headerTable_ColNum == 3)
                headerTable.SetWidths(new[] { 0.3f, 0.4f, 0.3f });
            else if (headerTable_ColNum == 4)
                headerTable.SetWidths(new[] { 0.2f, 0.2f, 0.2f, 0.3f });

            headerTable.WidthPercentage = 100f;

            if (ImageUrl != "")
            {
                var ServiceLogImage = Image.GetInstance(ImageUrl);
                ServiceLogImage.Alignment = 1;// GetAlignment(lang, true);
                ServiceLogImage.ScaleToFit(80f, 80f);

                var cellLogo = new PdfPCell { Border = Rectangle.NO_BORDER };
                cellLogo.AddElement(ServiceLogImage);
                headerTable.AddCell(cellLogo);
            }
            if (AvatarPicture != null)
            {
                var AvatarFilePath = _pictureService.GetThumbLocalPath(AvatarPicture, 0, false);
                var Avatar = Image.GetInstance(AvatarFilePath);
                Avatar.Alignment = 1;// GetAlignment(lang, true);
                Avatar.ScaleToFit(80f, 80f);

                var cellLogo = new PdfPCell { Border = Rectangle.NO_BORDER };
                cellLogo.AddElement(Avatar);
                headerTable.AddCell(cellLogo);
            }
            //logo               
            if (logoExists)
            {

                var logoFilePath = _pictureService.GetThumbLocalPath(logoPicture, 0, false);
                var logo = Image.GetInstance(logoFilePath);
                logo.Alignment = GetAlignment(lang, true);
                logo.ScaleToFit(150f, 150f);

                var cellLogo = new PdfPCell { Border = Rectangle.NO_BORDER };
                cellLogo.AddElement(logo);
                headerTable.AddCell(cellLogo);
            }
            doc.Add(headerTable);
        }

        public bool isAvatarValid(OrderItem orderItem)
        {

            bool hasValidAvatar = _extendedShipmentService.hasValidAvatar(orderItem.Order.CustomerId).GetValueOrDefault(false);
            if (hasValidAvatar)
            {
                var hasRequestPrintAvatar = _extendedShipmentService.HasRequestPrintAvatar(orderItem);
                return (hasValidAvatar && hasRequestPrintAvatar);
            }
            return hasValidAvatar;
        }

        public override void PrintPackagingSlipsToPdf(Stream stream, IList<Shipment> shipments, int languageId = 0)
        {
            IList<Order> Lst_Order = shipments.Select(p => p.Order).ToList();
            if (Lst_Order.Any(p => p.PaymentMethodSystemName == "Payments.CashOnDelivery"))
                Print_COD_OrdersToPdf(Lst_Order, stream);
            else
                this.PrintOrdersToPdf(stream, Lst_Order, languageId);
        }
        private PdfPTable PrintBarcode(Shipment shipment, Language lang, Font titleFont, Font font)
        {
            byte[] barcodeImg = null;
            barcodeImg = _extendedShipmentService.getBarocdeImage(shipment);
            if (barcodeImg == null)
                return null;
            var barcodeTable = new PdfPTable(1);
            if (lang.Rtl)
                barcodeTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
            else
                barcodeTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            barcodeTable.DefaultCell.Border = Rectangle.NO_BORDER;
            barcodeTable.WidthPercentage = 40f;
            var logo = Image.GetInstance(barcodeImg); //Image.GetInstance(logoFilePath);
            logo.Alignment = GetAlignment(lang, true);
            logo.ScaleToFit(100f, 40f);
            logo.Border = Rectangle.NO_BORDER;
            barcodeTable.AddCell(logo);
            var myFont = GetFont("Vazir-FD.TTF");
            myFont.Size = 10;
            myFont.Color = LabColor.BLUE;
            PdfPCell tracknumCell = new PdfPCell(new Paragraph(shipment.TrackingNumber + Environment.NewLine));
            tracknumCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tracknumCell.Border = Rectangle.NO_BORDER;
            //tracknumCell.Width = 100f;
            barcodeTable.Rows.Add(new PdfPRow(new PdfPCell[] { tracknumCell }));

            Paragraph p = new Paragraph("http://postbar.ir/rahgiry", myFont);
            PdfPCell tracknumCel3 = new PdfPCell(p);
            tracknumCel3.HorizontalAlignment = Element.ALIGN_CENTER;
            tracknumCel3.Border = Rectangle.NO_BORDER;
            //tracknumCell.Width = 100f;
            barcodeTable.Rows.Add(new PdfPRow(new PdfPCell[] { tracknumCel3 }));
            string AgentName = "";
            if (shipment.Order.Customer.IsInCustomerRole("mini-Administrators"))
            {
                AgentName = shipment.Order.Customer.GetFullName();
            }
            PdfPCell sign = new PdfPCell(new Paragraph("نام و نام خانوادگی:"
                + Environment.NewLine
                + Environment.NewLine
                + "امضاء مشتری:"
                + Environment.NewLine
                + Environment.NewLine
                + Environment.NewLine
                + AgentName
                , titleFont))
            {
                RunDirection = PdfWriter.RUN_DIRECTION_RTL
            };
            sign.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
            barcodeTable.AddCell(sign);
            return barcodeTable;
        }
        private PdfPTable PrintChaparSecoundBarcode(Shipment shipment, Language lang, Font titleFont, Font font)
        {
            byte[] barcodeImg = null;
            barcodeImg = _extendedShipmentService.getBarocdeImage(shipment.AdminComment);
            if (barcodeImg == null)
                return null;
            var barcodeTable = new PdfPTable(1);
            if (lang.Rtl)
                barcodeTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
            else
                barcodeTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            barcodeTable.DefaultCell.Border = Rectangle.NO_BORDER;
            barcodeTable.WidthPercentage = 10;
            var logo = Image.GetInstance(barcodeImg); //Image.GetInstance(logoFilePath);
            logo.Alignment = GetAlignment(lang, true);
            logo.ScaleToFit(50f, 20f);
            logo.Border = Rectangle.NO_BORDER;
            barcodeTable.AddCell(logo);
            var myFont = GetFont("Vazir-FD.TTF");
            myFont.Size = 8;
            myFont.Color = LabColor.BLUE;
            PdfPCell tracknumCell = new PdfPCell(new Paragraph(shipment.TrackingNumber + Environment.NewLine));
            tracknumCell.HorizontalAlignment = Element.ALIGN_CENTER;
            tracknumCell.Border = Rectangle.NO_BORDER;
            //tracknumCell.Width = 50f;
            barcodeTable.Rows.Add(new PdfPRow(new PdfPCell[] { tracknumCell }));

            return barcodeTable;
        }
        private void PrintEngiPrice(int vendorId, Language lang, Font titleFont, Document doc, OrderItem orderitem
            , Font font, Font attributesFont, OrderBill bill, int HagheMaghar, int ItemDiscount, bool igoneBime = false)
        {
            // doc.Add(new Paragraph(" "));
            int foreignPrices = 0;
            if (_extendedShipmentService.IsOrderForeign(orderitem.Order))
            {
                var RelatedOrder = _relatedOrders_Service.GetTb_RelatedOrders_By_ParentOrderId(orderitem.OrderId);
                if (RelatedOrder != null)
                {
                    var number = orderitem.Order.OrderItems.Sum(p => p.Quantity);
                    if (number > 0)
                    {
                        foreignPrices = (RelatedOrder.ChildOrderPrice / number);
                    }
                }
            }
            doc.Add(new Paragraph(" "));
            decimal tax = orderitem.Order.TaxRatesDictionary.First().Key;
            var productsHeader = new PdfPTable(1)
            {
                RunDirection = GetDirection(lang),
                WidthPercentage = 100f
            };
            var cellProducts = new PdfPCell(new Phrase(" خدمات فنی و پشتیبانی         شماره سفارش : " + orderitem.OrderId, titleFont));
            cellProducts.Border = Rectangle.NO_BORDER;
            productsHeader.AddCell(cellProducts);
            doc.Add(productsHeader);
            doc.Add(new Paragraph(" "));

            var productsTable = new PdfPTable(4)
            {
                RunDirection = GetDirection(lang),
                WidthPercentage = 100f
            };

            if (lang.Rtl)
            {
                productsTable.SetWidths(new[] { 20, 10, 60, 10 });
            }
            else
            {
                productsTable.SetWidths(new[] { 10, 60, 10, 20 });
            }
            int rowNum = 1;
            #region header
            //product name
            var cellProductItem = new PdfPCell(new Phrase("ردیف", font));
            cellProductItem.BackgroundColor = BaseColor.LIGHT_GRAY;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            cellProductItem = new PdfPCell(new Phrase("شرح", font));
            cellProductItem.BackgroundColor = BaseColor.LIGHT_GRAY;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            cellProductItem = new PdfPCell(new Phrase("تعداد", font));
            cellProductItem.BackgroundColor = BaseColor.LIGHT_GRAY;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            cellProductItem = new PdfPCell(new Phrase("قیمت", font));
            cellProductItem.BackgroundColor = BaseColor.LIGHT_GRAY;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);
            #endregion

            #region EngPrice
            //row number

            cellProductItem = GetPdfCell(rowNum.ToString(), font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);
            //eng name
            //int ApproximateValue = _extendedShipmentService.getApproximateValue(orderitem.Id);
            string content = $"خدمات فنی مهندسی ، بازاریابی و پوششی";
            //if (bime > 0)
            //    content += $"(پوشش {bime} ریال و ارزش {ApproximateValue} ريال)";
            cellProductItem = GetPdfCell(content, font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);


            //eng qty
            int qty = 1;
            cellProductItem = GetPdfCell(qty.ToString(), font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);
            var mark = _extendedShipmentService.GetOrderRegistrationMethod(orderitem.Order);

            //eng price
            var TotalCost = _extendedShipmentService.GetItemCostFromAttr(orderitem);
            //+bill.AttributeCost.Sum(p => p.value);
            var TotalValue = _extendedShipmentService.GetItemPriceFromAttr(orderitem);
            //+ bill.AttributePrice.Sum(p => p.value);
            //int bime = 0;
            //if (bill.AttributePrice.Any(p => p.name.Contains("بیمه") && p.value > 0))
            //{
            //    bime = Convert.ToInt32(bill.AttributePrice.FirstOrDefault(p => p.name.Contains("بیمه")).value);
            //}
            decimal engPrice = 0;
            var isPostService = _extendedShipmentService.IsPostService(orderitem.Product.ProductCategories.First().CategoryId);
            if (!isPostService || mark == OrderRegistrationMethod.Ap || mark == OrderRegistrationMethod.bidok)
            {
                var prices = _extendedShipmentService.getEngAndPostPrice(orderitem);
                if (prices == null)
                {
                    if (!_extendedShipmentService.CheckHasValidPrice(orderitem.Order))
                    {
                        throw new Exception($"قیمت مربوط به آیتم شماره {orderitem.Id} از سرویس مربوطه دریافت نشده");
                    }
                    else
                    {
                        prices = _extendedShipmentService.getEngAndPostPrice(orderitem);
                    }

                }
                engPrice = prices.EngPrice + foreignPrices;


            }
            else
            {
                engPrice = (TotalValue - TotalCost);// + HagheMaghar + bime;
            }
            engPrice = engPrice + HagheMaghar;//فعلا برای پست باید ما به التفاوت حق مقر اصلی با حق مقر فعلی پست به خدمات فنی مهندسی اضافه شد
            //var engPriceInCustomerCurrency =
            //        _currencyService.ConvertCurrency(engPrice, orderitem.Order.CurrencyRate);
            //string EngPrice = _priceFormatter.FormatPrice(engPriceInCustomerCurrency, true,
            //    orderitem.Order.CustomerCurrencyCode, lang, false);
            cellProductItem = GetPdfCell(engPrice.ToString("N0") + " ريال", font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);
            #endregion

            #region print

            int print = 0;
            if (bill.AttributePrice.Any(p => p.name.Contains("پرینتر") && p.value > 0))
            {
                print = Convert.ToInt32(bill.AttributePrice.FirstOrDefault(p => p.name.Contains("پرینتر")).value);
                //row number
                cellProductItem = GetPdfCell((rowNum++).ToString(), font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                ////eng name
                cellProductItem = GetPdfCell("درخواست چاپ ", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                ////eng qty
                cellProductItem = GetPdfCell("1", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //////HagheMaghar Tax
                //var printInCustomerCurrency =
                //    _currencyService.ConvertCurrency(bill.AttributePrice.FirstOrDefault(p => p.name.Contains("پرینتر")).value, orderitem.Order.CurrencyRate);

                //string printPrice = _priceFormatter.FormatPrice(printInCustomerCurrency, true,
                //    orderitem.Order.CustomerCurrencyCode, lang, false);

                cellProductItem = GetPdfCell(print.ToString("N0") + " ريال", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);
            }
            #endregion

            #region کارتون و لفاف بندی

            int _cartonValue = 0;
            int LafafValue = 0;
            if (bill.AttributePrice.Any(p => p.name.Contains("لفاف") && p.value > 0))
            {
                int cartonLafafValue = Convert.ToInt32(bill.AttributePrice.FirstOrDefault(p => p.name.Contains("لفاف")).value);

                LafafValue = Convert.ToInt32(bill.AttributeCost.FirstOrDefault(p => p.name.Contains("لفاف")).value);
                _cartonValue = cartonLafafValue - LafafValue;
                //row number
                cellProductItem = GetPdfCell((++rowNum).ToString(), font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                ////eng name
                cellProductItem = GetPdfCell("بسته بندی", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                ////eng qty
                cellProductItem = GetPdfCell("1", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                ////HagheMaghar Tax
                //var kartonInCustomerCurrency =
                //    _currencyService.ConvertCurrency(bill.AttributePrice.FirstOrDefault(p => p.name.Contains("لفاف")).value, orderitem.Order.CurrencyRate);

                //string kartonPrice = _priceFormatter.FormatPrice(kartonInCustomerCurrency, true,
                //    orderitem.Order.CustomerCurrencyCode, lang, false);


                cellProductItem = GetPdfCell(_cartonValue.ToString("N0") + " ريال", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);
            }
            //if(LafafValue > 0)
            //{
            //    cellProductItem = GetPdfCell((++rowNum).ToString(), font);
            //    cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            //    productsTable.AddCell(cellProductItem);

            //    ////eng name
            //    cellProductItem = GetPdfCell("بسته بندی", font);
            //    cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            //    productsTable.AddCell(cellProductItem);

            //    ////eng qty
            //    cellProductItem = GetPdfCell("1", font);
            //    cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            //    productsTable.AddCell(cellProductItem);

            //    ////HagheMaghar Tax
            //    //var LafafInCustomerCurrency =
            //    //    _currencyService.ConvertCurrency(LafafValue, orderitem.Order.CurrencyRate);

            //    //string LafafPrice = _priceFormatter.FormatPrice(LafafInCustomerCurrency, true,
            //    //    orderitem.Order.CustomerCurrencyCode, lang, false);


            //    cellProductItem = GetPdfCell(LafafValue.ToString("N0")+" ريال", font);
            //    cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            //    productsTable.AddCell(cellProductItem);
            //}
            #endregion

            #region Bime

            int bime = 0;
            if (bill.AttributePrice.Any(p => p.name.Contains("بیمه") && p.value > 0))
            {
                bime = Convert.ToInt32(bill.AttributePrice.FirstOrDefault(p => p.name.Contains("بیمه")).value);
                //row number
                cellProductItem = GetPdfCell((rowNum++).ToString(), font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng name
                cellProductItem = GetPdfCell("تضمین غرامت کالا", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng qty
                cellProductItem = GetPdfCell("1", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //HagheMaghar Tax
                //var bimeInCustomerCurrency =
                //    _currencyService.ConvertCurrency(bime, orderitem.Order.CurrencyRate);

                //string bimePrice = _priceFormatter.FormatPrice(bimeInCustomerCurrency, true,
                //    orderitem.Order.CustomerCurrencyCode, lang, false);

                cellProductItem = GetPdfCell(bime.ToString("N0") + " ريال", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);
            }
            #endregion

            int sms = 0;
            if (bill.AttributePrice.Any(p => p.name.Contains("اطلاع رسانی پیامکی") && p.value > 0))
            {
                sms = Convert.ToInt32(bill.AttributePrice.FirstOrDefault(p => p.name.Contains("اطلاع رسانی پیامکی")).value);
                //row number
                cellProductItem = GetPdfCell((rowNum++).ToString(), font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng name
                cellProductItem = GetPdfCell("اطلاع رسانی پیامکی", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng qty
                cellProductItem = GetPdfCell("1", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //HagheMaghar Tax
                //var SmsInCustomerCurrency =
                //    _currencyService.ConvertCurrency(sms, orderitem.Order.CurrencyRate);

                //string SmsPrice = _priceFormatter.FormatPrice(SmsInCustomerCurrency, true,
                //    orderitem.Order.CustomerCurrencyCode, lang, false);

                cellProductItem = GetPdfCell(sms.ToString("N0") + " ريال", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);
            }

            int distributionPrice = _extendedShipmentService.GetdistributionValue(orderitem);
            if (distributionPrice > 0)
            {
                //row number
                cellProductItem = GetPdfCell((rowNum++).ToString(), font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng name
                cellProductItem = GetPdfCell("هزینه توزیع", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng qty
                cellProductItem = GetPdfCell("1", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //HagheMaghar Tax
                //var SmsInCustomerCurrency =
                //    _currencyService.ConvertCurrency(sms, orderitem.Order.CurrencyRate);

                //string SmsPrice = _priceFormatter.FormatPrice(SmsInCustomerCurrency, true,
                //    orderitem.Order.CustomerCurrencyCode, lang, false);

                cellProductItem = GetPdfCell(distributionPrice.ToString("N0") + " ريال", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);
            }

            #region HagheSabt
            int HagheSabt = 0;
            HagheSabt = _extendedShipmentService.getHagheSabt(orderitem.Id);
            if (HagheSabt > 0)
            {
                //row number
                cellProductItem = GetPdfCell((rowNum++).ToString(), font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng name
                cellProductItem = GetPdfCell("ثبت الکترونیک", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng qty
                cellProductItem = GetPdfCell("1", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //HagheMaghar Tax
                //var HagheSabtInCustomerCurrency =
                //    _currencyService.ConvertCurrency(HagheSabt, orderitem.Order.CurrencyRate);

                //string HagheSabtPrice = _priceFormatter.FormatPrice(HagheSabtInCustomerCurrency, true,
                //    orderitem.Order.CustomerCurrencyCode, lang, false);

                cellProductItem = GetPdfCell(HagheSabt.ToString("N0") + " ريال", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);
            }
            #endregion

            #region PrintLogo
            int requestPrintLogo = 0;
            requestPrintLogo = _extendedShipmentService.RequestPrintLogoPrice(orderitem);
            if (requestPrintLogo > 0)
            {
                //row number
                cellProductItem = GetPdfCell((rowNum++).ToString(), font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng name
                cellProductItem = GetPdfCell("چاپ نشان تجاری", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng qty
                cellProductItem = GetPdfCell("1", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //HagheMaghar Tax
                //var requestPrintLogotInCustomerCurrency =
                //    _currencyService.ConvertCurrency(requestPrintLogo, orderitem.Order.CurrencyRate);

                //string ز = _priceFormatter.FormatPrice(requestPrintLogotInCustomerCurrency, true,
                //    orderitem.Order.CustomerCurrencyCode, lang, false);

                cellProductItem = GetPdfCell(requestPrintLogo.ToString("N0") + " ريال", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);
            }
            #endregion

            //اضافه کردن مبلغ خدمات نمایندگی
            #region ValueAddedByAgnt
            int ValueAddedByAgnt = 0;
            if (orderitem.Order.Customer.IsInCustomerRole("mini-Administrators"))
            {
                ValueAddedByAgnt = _extendedShipmentService.GetValueAddedbyAgent(orderitem);

            }
            if (ValueAddedByAgnt > 0)
            {
                //row number
                cellProductItem = GetPdfCell((rowNum++).ToString(), font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng name
                cellProductItem = GetPdfCell("خدمات نمایندگی", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng qty
                cellProductItem = GetPdfCell("1", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //HagheMaghar Tax
                //var AgaInCustomerCurrency =
                //    _currencyService.ConvertCurrency(ValueAddedByAgnt, orderitem.Order.CurrencyRate);

                //string AgaPrice = _priceFormatter.FormatPrice(AgaInCustomerCurrency, true,
                //    orderitem.Order.CustomerCurrencyCode, lang, false);

                cellProductItem = GetPdfCell(ValueAddedByAgnt.ToString("N0") + " ريال", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);
            }
            #endregion

            bool IsMahexCod = (orderitem.Product.Id == 10445);
            int CodGoodPrice = _extendedShipmentService.getGoodsPrice(orderitem);
            if (IsMahexCod && CodGoodPrice > 0)
            {
                cellProductItem = GetPdfCell((rowNum++).ToString(), font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng name
                cellProductItem = GetPdfCell($"مبلغ کالا", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng qty
                cellProductItem = GetPdfCell("1", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);


                cellProductItem = GetPdfCell(CodGoodPrice.ToString("N0") + " ريال", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);
            }

            int TotalEngPirce = (int)((engPrice + print + _cartonValue + bime + sms + HagheSabt + requestPrintLogo + ValueAddedByAgnt + distributionPrice));
            int TotalEngPirceTax = (TotalEngPirce * 9) / 100;
            TotalEngPirce -= ItemDiscount;
            TotalEngPirce += CodGoodPrice;
            #region Eng Tax
            //row number
            cellProductItem = GetPdfCell((++rowNum).ToString(), font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);

            //eng name
            cellProductItem = GetPdfCell("مالیات بر ارزش افزوده", font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);

            //eng qty
            cellProductItem = GetPdfCell("1", font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);

            //eng Tax
            decimal Engtax = TotalEngPirceTax;
            //var taxInCustomerCurrency =
            //        _currencyService.ConvertCurrency(Engtax, orderitem.Order.CurrencyRate);
            //string taxPrice = _priceFormatter.FormatPrice(taxInCustomerCurrency, true,
            //    orderitem.Order.CustomerCurrencyCode, lang, false);

            cellProductItem = GetPdfCell(Engtax.ToString("N0") + " ريال", font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);
            #endregion

            #region Discount
            if (ItemDiscount > 0)
            {
                //row number
                cellProductItem = GetPdfCell((++rowNum).ToString(), font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng name
                cellProductItem = GetPdfCell("تخفیف", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng qty
                cellProductItem = GetPdfCell("1", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //HagheMaghar Tax
                //var DiscountInCustomerCurrency =
                //    _currencyService.ConvertCurrency(ItemDiscount, orderitem.Order.CurrencyRate);

                //string DiscountPrice = _priceFormatter.FormatPrice(DiscountInCustomerCurrency, true,
                //    orderitem.Order.CustomerCurrencyCode, lang, false);

                cellProductItem = GetPdfCell(ItemDiscount.ToString("N0") + " ريال", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);
            }
            #endregion

            #region Eng Total
            //row number
            cellProductItem = GetPdfCell("", font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);

            //eng name
            cellProductItem = GetPdfCell("", font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);

            //eng qty
            cellProductItem = GetPdfCell("جمع کل", titleFont);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);

            //eng Tax

            //var TotalInCustomerCurrency =
            //        _currencyService.ConvertCurrency(TotalEngPirce + TotalEngPirceTax, orderitem.Order.CurrencyRate);
            //string TotalPrice = _priceFormatter.FormatPrice(TotalInCustomerCurrency, true,
            //    orderitem.Order.CustomerCurrencyCode, lang, true);

            cellProductItem = GetPdfCell((TotalEngPirce + TotalEngPirceTax).ToString("N0") + " ريال", font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);
            #endregion

            doc.Add(productsTable);
        }



        private void PrintPostPrice(int vendorId, Language lang, Font titleFont, Document doc, OrderItem orderItem, Font font
            , Font attributesFont, OrderBill bill, int orderItemNum, bool igoneBime = false, int _posthagheMaghar = 0)
        {

            var productsHeader = new PdfPTable(1)
            {
                RunDirection = GetDirection(lang),
                WidthPercentage = 100f
            };
            var cellProducts = new PdfPCell(new Phrase(" خدمات پستی         شماره سفارش : " + orderItem.OrderId, titleFont));
            cellProducts.Border = Rectangle.NO_BORDER;
            productsHeader.AddCell(cellProducts);
            doc.Add(productsHeader);
            doc.Add(new Paragraph(" "));

            var productsTable = new PdfPTable(4)
            {
                RunDirection = GetDirection(lang),
                WidthPercentage = 100f
            };

            if (lang.Rtl)
            {
                productsTable.SetWidths(new[] { 20, 10, 60, 10 });
            }
            else
            {
                productsTable.SetWidths(new[] { 10, 60, 10, 20 });
            }

            #region header
            //product name
            var cellProductItem = new PdfPCell(new Phrase("ردیف", font));
            cellProductItem.BackgroundColor = BaseColor.LIGHT_GRAY;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            cellProductItem = new PdfPCell(new Phrase("شرح", font));
            cellProductItem.BackgroundColor = BaseColor.LIGHT_GRAY;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            cellProductItem = new PdfPCell(new Phrase("تعداد", font));
            cellProductItem.BackgroundColor = BaseColor.LIGHT_GRAY;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            cellProductItem = new PdfPCell(new Phrase("قیمت", font));
            cellProductItem.BackgroundColor = BaseColor.LIGHT_GRAY;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);
            #endregion

            #region Post Price
            //row number
            int rowNum = 1;
            cellProductItem = GetPdfCell(rowNum.ToString(), font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);
            //eng name
            string wheightName = _extendedShipmentService.getOrderItemWehghtName(orderItem, true);
            string producName = orderItem.Product.GetLocalized(x => x.Name, lang.Id) + " " + wheightName;
            cellProductItem = GetPdfCell(producName, font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);
            //eng qty
            int qty = 1;//order.OrderItems.Sum(n => n.Quantity);
            cellProductItem = GetPdfCell(qty.ToString(), font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);
            int postPrice = 0;
            //Post price
            bool exceptPostTaxPrice = false;
            if (new int[] { 699, 700, 701, 702, 703, 704, 705, 706, 707, 708, 709, 710, 715, 714, 713, 712, 711, 730, 731, 733, 718 }.Contains(orderItem.Product.ProductCategories.First().CategoryId))
            {
                var prices = _extendedShipmentService.getEngAndPostPrice(orderItem);
                if (prices == null)
                {
                    throw new Exception($"قیمت مربوط به آیتم شماره {orderItem.Id} از سرویس مربوطه دریافت نشده");
                }
                postPrice = prices.IncomePrice;
                exceptPostTaxPrice = true;
            }
            else
            {
                postPrice = _extendedShipmentService.GetItemCostFromAttr(orderItem);
            }
            //var engPriceInCustomerCurrency =
            //        _currencyService.ConvertCurrency(postPrice, orderItem.Order.CurrencyRate);
            //string _PostPrice = _priceFormatter.FormatPrice(engPriceInCustomerCurrency, false,
            //    orderItem.Order.CustomerCurrencyCode, lang, !exceptPostTaxPrice);

            cellProductItem = GetPdfCell(postPrice.ToString("N0") + " ريال", font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);
            #endregion

            if (bill.AttributeCost != null)
            {
                foreach (var item in bill.AttributeCost)
                {
                    if (igoneBime && item.name.Contains("بیمه"))
                        continue;
                    //if (item.name.Contains("لفاف"))
                    //    continue;
                    if (item.name.Contains("ثبت مرسوله"))
                        continue;
                    if (item.name.Contains("پرینت"))
                        continue;
                    if (item.name.Contains("نشان"))
                        continue;
                    if (item.name.Contains("پیامک"))
                        continue;
                    rowNum++;
                    cellProductItem = GetPdfCell(rowNum.ToString(), font);
                    cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                    productsTable.AddCell(cellProductItem);

                    //eng name
                    string AttrName = "";
                    if (item.name.Contains("غرامت پست"))
                        AttrName = "تعهد غرامت اجباری سرویس های ثبتی(بیمه عمومی)";
                    else
                        AttrName = item.name;
                    cellProductItem = GetPdfCell(AttrName, font);
                    cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                    productsTable.AddCell(cellProductItem);

                    //eng qty
                    cellProductItem = GetPdfCell(qty, font);
                    cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                    productsTable.AddCell(cellProductItem);

                    //bime
                    //var TaxInCustomerCurrency =
                    //    _currencyService.ConvertCurrency(item.value, orderItem.Order.CurrencyRate);
                    //string TaxPrice = _priceFormatter.FormatPrice(TaxInCustomerCurrency, false,
                    //    orderItem.Order.CustomerCurrencyCode, lang, false);

                    cellProductItem = GetPdfCell(item.value.ToString("N0") + " ريال", font);
                    cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                    productsTable.AddCell(cellProductItem);
                }
            }
            #region HagheMaghar
            if (_posthagheMaghar > 0)
            {
                //row number
                cellProductItem = GetPdfCell((rowNum++).ToString(), font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng name
                cellProductItem = GetPdfCell("حق مقر", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng qty
                cellProductItem = GetPdfCell("1", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //HagheMaghar Tax
                //var HagheMagharInCustomerCurrency =
                //    _currencyService.ConvertCurrency(_posthagheMaghar, orderItem.Order.CurrencyRate);

                //string HagheMagharPrice = _priceFormatter.FormatPrice(HagheMagharInCustomerCurrency, false,
                //    orderItem.Order.CustomerCurrencyCode, lang, false);

                cellProductItem = GetPdfCell(_posthagheMaghar.ToString("N0") + " ريال", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);
            }
            #endregion
            decimal AttCost = 0;
            if (bill.AttributeCost != null)
                if (bill.AttributeCost.Any())
                    if (igoneBime)
                    {
                        AttCost = bill.AttributeCost.Where(p => !p.name.Contains("بیمه") //&& !p.name.Contains("لفاف")
                        && !p.name.Contains("ثبت مرسوله") && !p.name.Contains("پرینت")).Sum(p => p.value);
                    }
                    else
                    {
                        AttCost = bill.AttributeCost.Sum(p => p.value);
                    }

            int TotalCost = 0;
            if (exceptPostTaxPrice)
            {
                AttCost += ((AttCost + _posthagheMaghar) * 9) / 100;
                TotalCost = (int)(postPrice + AttCost + _posthagheMaghar);
            }
            else
            {
                postPrice += (int)AttCost;
                int PostTax = (int)((postPrice + _posthagheMaghar) * 9) / 100;
                TotalCost += postPrice + _posthagheMaghar + PostTax;


                #region Eng Tax
                //row number
                cellProductItem = GetPdfCell((++rowNum).ToString(), font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng name
                cellProductItem = GetPdfCell("مالیات بر ارزش افزوده", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng qty
                cellProductItem = GetPdfCell("1", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //eng Tax
                decimal Engtax = PostTax;
                //var taxInCustomerCurrency =
                //        _currencyService.ConvertCurrency(Engtax, orderItem.Order.CurrencyRate);
                //string taxPrice = _priceFormatter.FormatPrice(taxInCustomerCurrency, false,
                //    orderItem.Order.CustomerCurrencyCode, lang, false);

                cellProductItem = GetPdfCell(Engtax.ToString("N0") + " ريال", font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);
                #endregion
            }

            #region post Total
            //row number
            cellProductItem = GetPdfCell("", font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);

            //eng name
            cellProductItem = GetPdfCell("", font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);

            //eng qty
            cellProductItem = GetPdfCell("جمع کل", titleFont);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);

            //eng Tax
            //var TotalInCustomerCurrency =
            //        _currencyService.ConvertCurrency(TotalCost, orderItem.Order.CurrencyRate);
            //string TotalPrice = _priceFormatter.FormatPrice(TotalInCustomerCurrency, false,
            //    orderItem.Order.CustomerCurrencyCode, lang, true);

            cellProductItem = GetPdfCell(TotalCost.ToString("N0") + " ريال", font);
            cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
            productsTable.AddCell(cellProductItem);
            #endregion

            doc.Add(productsTable);
        }

        private string MiladyToShamsi(DateTime dt, bool AddTime = false)
        {
            PersianCalendar pa = new PersianCalendar();
            string PaData = pa.GetDayOfMonth(dt).ToString("00") + "/" + pa.GetMonth(dt).ToString("00") + "/" + pa.GetYear(dt);
            if (AddTime)
                PaData += " " + dt.ToShortTimeString();
            return PaData;
        }
        private string MiladyToShamsi2(DateTime dt, bool AddTime = false)
        {
            PersianCalendar pa = new PersianCalendar();
            string PaData = pa.GetYear(dt) + "/" + pa.GetMonth(dt).ToString("00") + "/" + pa.GetDayOfMonth(dt).ToString("00");
            if (AddTime)
                PaData += " " + dt.ToShortTimeString();
            return PaData;
        }
        private Image LoadGatwayImage(Language lang)
        {
            var imagePath = CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Images/") + "gatewayLogo.jpg";
            byte[] img = File.ReadAllBytes(imagePath);
            var logo = Image.GetInstance(img);
            logo.Alignment = GetAlignment(lang, true);
            logo.ScaleToFit(50f, 50f);
            return logo;
        }

        private string getPostBarRule()
        {
            try
            {
                int id = 67;
                var topic = _topicService.GetTopicById(id);
                if (topic == null)
                {
                    topic = _topicService.GetTopicBySystemName("پست بار - قوانین");
                    if (topic == null)
                        topic = _topicService.GetTopicBySystemName("قوانین پست بار");
                    if (topic == null)
                        return "";
                }

                return topic.Body;
            }
            catch (Exception ex)
            {
                _extendedShipmentService.Log("خطا در زمان دریافت قوانین پست بار",
                    ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                return "";
            }

        }
    }
    public class ApiOrderItemPrice
    {
        public int IncomePrice { get; set; }
        public int EngPrice { get; set; }
        public int AttrPrice { get; set; }
    }
    public class LablePrint
    {
        public string ServiceName { get; set; }
        public string OfficeName { get; set; }
        public string BoxType { get; set; }
        public string SenderState { get; set; }
        public string ReceiverState { get; set; }
        public int Weight { get; set; }
        public int ShipmentId { get; set; }
        public int orderId { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string PostTotalPrice { get; set; }
        public byte[] BarcodeImage { get; set; }
        public string barcodeNo { get; set; }
    }
    public class LablePrintEngList
    {
        public string Receiver { get; set; }
        public string ApiOrderRefrence { get; set; }
        public string Sender { get; set; }
        public string TotalEngPrice { get; set; }
        public int ShipmentId { get; set; }
        public int orderId { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Stimulsoft.Report;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    public class ExtendedAdminOrderController : Nop.Web.Areas.Admin.Controllers.OrderController
    {

        #region feild
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IShipmentTrackingService _shipmentTrackingService; 
        #endregion

        #region ctor
        public ExtendedAdminOrderController(
            IDbContext dbContext,
            IShipmentTrackingService shipmentTrackingService,
           IOrderService orderService,
          IOrderReportService orderReportService,
          IOrderProcessingService orderProcessingService,
          IReturnRequestService returnRequestService,
          IPriceCalculationService priceCalculationService,
          ITaxService taxService,
          IDateTimeHelper dateTimeHelper,
          IPriceFormatter priceFormatter,
          IDiscountService discountService,
          ILocalizationService localizationService,
          IWorkContext workContext,
          ICurrencyService currencyService,
          IEncryptionService encryptionService,
          IPaymentService paymentService,
          IMeasureService measureService,
          IPdfService pdfService,
          IAddressService addressService,
          ICountryService countryService,
          IStateProvinceService stateProvinceService,
          IProductService productService,
          IExportManager exportManager,
          IPermissionService permissionService,
          IWorkflowMessageService workflowMessageService,
          ICategoryService categoryService,
          IManufacturerService manufacturerService,
          IProductAttributeService productAttributeService,
          IProductAttributeParser productAttributeParser,
          IProductAttributeFormatter productAttributeFormatter,
          IShoppingCartService shoppingCartService,
          IGiftCardService giftCardService,
          IDownloadService downloadService,
          IShipmentService shipmentService,
          IShippingService shippingService,
          IStoreService storeService,
          IVendorService vendorService,
          IAddressAttributeParser addressAttributeParser,
          IAddressAttributeService addressAttributeService,
          IAddressAttributeFormatter addressAttributeFormatter,
          IAffiliateService affiliateService,
          IPictureService pictureService,
          ICustomerActivityService customerActivityService,
          IStaticCacheManager cacheManager,
          OrderSettings orderSettings,
          CurrencySettings currencySettings,
          TaxSettings taxSettings,
          MeasureSettings measureSettings,
          AddressSettings addressSettings,
          ShippingSettings shippingSettings) : base(orderService,
          orderReportService,
          orderProcessingService,
          returnRequestService,
          priceCalculationService,
          taxService,
          dateTimeHelper,
          priceFormatter,
          discountService,
          localizationService,
          workContext,
          currencyService,
          encryptionService,
          paymentService,
          measureService,
          pdfService,
          addressService,
          countryService,
          stateProvinceService,
          productService,
          exportManager,
          permissionService,
          workflowMessageService,
          categoryService,
          manufacturerService,
          productAttributeService,
          productAttributeParser,
          productAttributeFormatter,
          shoppingCartService,
          giftCardService,
          downloadService,
          shipmentService,
          shippingService,
          storeService,
          vendorService,
          addressAttributeParser,
          addressAttributeService,
          addressAttributeFormatter,
          affiliateService,
          pictureService,
          customerActivityService,
          cacheManager,
          orderSettings,
          currencySettings,
          taxSettings,
          measureSettings,
          addressSettings,
          shippingSettings)
        {
            _permissionService = permissionService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
            _shipmentTrackingService = shipmentTrackingService;
        }
        #endregion
     
        public override IActionResult LoadOrderStatistics(string period)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return Content("");

            //a vendor doesn't have access to this report
            if (_workContext.CurrentVendor != null)
                return Content("");

            var result = new List<object>();

            var nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            var timeZone = _dateTimeHelper.CurrentTimeZone;

            var culture = new CultureInfo(_workContext.WorkingLanguage.LanguageCulture);

            switch (period)
            {
                case "year":
                    //year statistics
                    var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                    var searchYearDateUser = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                    if (!timeZone.IsInvalidTime(searchYearDateUser))
                    {
                        for (var i = 0; i <= 12; i++)
                        {
                            result.Add(new
                            {
                                date = searchYearDateUser.Date.ToString("Y", culture),
                                value = _shipmentTrackingService.GetShipmentStatistic(
                                     _dateTimeHelper.ConvertToUtcTime(searchYearDateUser, timeZone),
                                   _dateTimeHelper.ConvertToUtcTime(searchYearDateUser.AddMonths(1), timeZone)
                                    ).ToString()
                            });

                            searchYearDateUser = searchYearDateUser.AddMonths(1);
                        }
                    }
                    break;

                case "month":
                    //month statistics
                    var monthAgoDt = nowDt.AddDays(-30);
                    var searchMonthDateUser = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                    if (!timeZone.IsInvalidTime(searchMonthDateUser))
                    {
                        for (var i = 0; i <= 30; i++)
                        {
                            result.Add(new
                            {
                                date = searchMonthDateUser.Date.ToString("M", culture),
                                value = _shipmentTrackingService.GetShipmentStatistic(
                                    _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser, timeZone),
                                    _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser.AddDays(1), timeZone)
                                 ).ToString()
                            });

                            searchMonthDateUser = searchMonthDateUser.AddDays(1);
                        }
                    }
                    break;

                case "week":
                default:
                    //week statistics
                    var weekAgoDt = nowDt.AddDays(-7);
                    var searchWeekDateUser = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                    if (!timeZone.IsInvalidTime(searchWeekDateUser))
                    {
                        for (var i = 0; i <= 7; i++)
                        {
                            result.Add(new
                            {
                                date = searchWeekDateUser.Date.ToString("d dddd", culture),
                                value = _shipmentTrackingService.GetShipmentStatistic(
                                    _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser, timeZone),
                                    _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser.AddDays(1), timeZone)).ToString()
                            });

                            searchWeekDateUser = searchWeekDateUser.AddDays(1);
                        }
                    }
                    break;
            }

            return Json(result);
        }

    }
}

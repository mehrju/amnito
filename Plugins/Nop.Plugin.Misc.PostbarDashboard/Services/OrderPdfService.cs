using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Services
{
    public class OrderPdfService : IOrderPdfService
    {
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;

        public OrderPdfService(IStoreContext storeContext,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICountryService countryService,
            IStateProvinceService stateProvinceService)
        {
            _storeContext = storeContext;
            _localizationService = localizationService;
            _workContext = workContext;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
        }

        public void GetOrderBarcodePdf(IList<OrderTrackingBarcode> orders, OrderSearchCondition searchCondition, Stream stream)
        {
            Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHm4lqN+V3/fvJECXRrqdoEPZqYyFXh3g3K9oCDFvgzMl4c9KudQQwMJ6nO6w7rHz59BhwYgDE0QzKtjY2WxEejTNewbNDXY492M2mDsK1Hb4t6MoFYGbSoID0gow3VC5cFhvcOKVoagiHq6/4iqFc3RS7nZQBp95+WB6N1fR//H7OBlvfuBKldvC1pJh6pTW/7HRdOclErt/EGwivx9SpuDNabWWBfIZJBdJsvKjpoIsDQGibWk8Y8F9V1mCLf88FurWwEZ9WzXePbJn+wfabHM+a7pqDAXhsaW33UEW6vQ3kgLrVjbcHWdhtYbp5j6ZeIMLBCW2LXiCZpzy3N3XqjlroV/s7KRZGeZMvRrLWhcM8YJ6sBTMUyQrF/Fk3d5f0WbAf7+opIA5rhxY+qgwWcE42S+HOOcMgb2IWtj5ycC/GXZ4o9qtk2WWJzCC+ygckXK7iI5pzhzScGmLCBrFnjyE5EP65FViVDSCcNl6hUFw7wwRtcQq5pnOu6wHRXsSpwNPufWaRqE4gd7hbrQoPqki4BLOYgGOQjZ3kJdc7MG47nVT62z1ScLsjH73q4yhABt8h1wmJl0LanKnE6EGkvZPCrDBBf0sZp12sJrxK8FGsPUKrBNZ5f4VuYkdzWS1ed2m5MZNHkDDd3LncYkQnLPh/MYAsnc2ud715njwiM62xIWYk9lIrmCWJU3JVcexRDS3/l7Po9UU5gx2xJKUW1tWOXKQ9GqQZYtn/rl804VMC1/tWr4X2XU1otulny5P8YkZ7BTG6zmkcW6raROxv7xkSUwE0LK2FmIpeBr29Zxn0QUYBtC09J+wVD5y00f/1jMU+k1jQmi1n2tpWgy6CxAHPPrQFhk0FsGXEDg82IELCKncbqkfqgT0QyV262YL2uxrcbcJ1Wrzf4llEX90hMa3oZK5wdQLrCBkXIlXverhGL3I09fR6lcXDMj8A3vrQrhr4bwYcWTH0hyJCPVLYIVxAFLwgV8wk700QOo3z32sSWKnyMHVx3brFB8I7rOPaW2Sc8rBxc1E9EvNcqiTwRd9QA3lIdR2Hpc4jyn5dx8PT6GTNGXNKhoTZDLrGMtYqXw4rVOXfkE8+opF4+/H602ockCI/IY7C65+sxooNgSg+9LpzImUt6GdtrvF8HrvJ0rUW6ZiAmGaQmqdrHfwC3K+QKU4UEWuLU1GPG+PKMrYEbtT/Hei7NPA5sOqHqbzaMsW7zj+enlhuEUeMdlxqfQs1QtTERrzX2HXv/JZ43rASH4ycuGftLR2v3AOHhtBCoYOCszhOiHYGmH0I9lwi8HDRTXjkuoRU1Zxmz9rSIQy35/aypV339OS7p4VydO5LibEiwugzaNHXCZalEPShhcWzTAq6+uCb+jA9ob+Za4yICRdIjgot/E5ZQNZl2VEhFmWR7pYSB7MB24z0ysVf3igAc50/3Lr4d7mQ7SgmtKFoejGOjvb75YOq/pEJGxCo1xIS+jhZklyLuiwmtyDe7Xmpn33fip22qx2iF7FYW71AyuT3tTPn2a7Jc2wXlPCAxB6dqx0eeX4fWA3+RIosUTxTyA7a/SM95pwN9FKYFPZ8TcTqDIq0Ntc57IdX5u1To=";
            Stimulsoft.Base.StiFontCollection.AddFontFile(CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Content") + "/B Mitra.TTF");
            var report = new StiReport();
            report.Load(CommonHelper.MapPath("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Reports/") + "OrdersBarCodeReport.mrt");
            report.Compile();
            report.RegBusinessObject("OrdersBarCodeList", "OrdersBarCodeList", orders);
            report["Now"] = DateTime.Now.ToPersianDate();
            report["SiteAddress"] = _storeContext.CurrentStore.Url;
            report["CompanyName"] = _storeContext.CurrentStore.Name;

            string conditions = CreateConditionText(searchCondition);
            report["SearchConditions"] = conditions;
            report.Render();
            StiPdfExportService service = new StiPdfExportService();
            service.ExportPdf(report, stream);
            stream.Seek(0, SeekOrigin.Begin);
        }
        public void GetOrderFactorPdf(IList<DashboardFactorRequestMRTModel> orders, Stream stream)
        {

            Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHm4lqN+V3/fvJECXRrqdoEPZqYyFXh3g3K9oCDFvgzMl4c9KudQQwMJ6nO6w7rHz59BhwYgDE0QzKtjY2WxEejTNewbNDXY492M2mDsK1Hb4t6MoFYGbSoID0gow3VC5cFhvcOKVoagiHq6/4iqFc3RS7nZQBp95+WB6N1fR//H7OBlvfuBKldvC1pJh6pTW/7HRdOclErt/EGwivx9SpuDNabWWBfIZJBdJsvKjpoIsDQGibWk8Y8F9V1mCLf88FurWwEZ9WzXePbJn+wfabHM+a7pqDAXhsaW33UEW6vQ3kgLrVjbcHWdhtYbp5j6ZeIMLBCW2LXiCZpzy3N3XqjlroV/s7KRZGeZMvRrLWhcM8YJ6sBTMUyQrF/Fk3d5f0WbAf7+opIA5rhxY+qgwWcE42S+HOOcMgb2IWtj5ycC/GXZ4o9qtk2WWJzCC+ygckXK7iI5pzhzScGmLCBrFnjyE5EP65FViVDSCcNl6hUFw7wwRtcQq5pnOu6wHRXsSpwNPufWaRqE4gd7hbrQoPqki4BLOYgGOQjZ3kJdc7MG47nVT62z1ScLsjH73q4yhABt8h1wmJl0LanKnE6EGkvZPCrDBBf0sZp12sJrxK8FGsPUKrBNZ5f4VuYkdzWS1ed2m5MZNHkDDd3LncYkQnLPh/MYAsnc2ud715njwiM62xIWYk9lIrmCWJU3JVcexRDS3/l7Po9UU5gx2xJKUW1tWOXKQ9GqQZYtn/rl804VMC1/tWr4X2XU1otulny5P8YkZ7BTG6zmkcW6raROxv7xkSUwE0LK2FmIpeBr29Zxn0QUYBtC09J+wVD5y00f/1jMU+k1jQmi1n2tpWgy6CxAHPPrQFhk0FsGXEDg82IELCKncbqkfqgT0QyV262YL2uxrcbcJ1Wrzf4llEX90hMa3oZK5wdQLrCBkXIlXverhGL3I09fR6lcXDMj8A3vrQrhr4bwYcWTH0hyJCPVLYIVxAFLwgV8wk700QOo3z32sSWKnyMHVx3brFB8I7rOPaW2Sc8rBxc1E9EvNcqiTwRd9QA3lIdR2Hpc4jyn5dx8PT6GTNGXNKhoTZDLrGMtYqXw4rVOXfkE8+opF4+/H602ockCI/IY7C65+sxooNgSg+9LpzImUt6GdtrvF8HrvJ0rUW6ZiAmGaQmqdrHfwC3K+QKU4UEWuLU1GPG+PKMrYEbtT/Hei7NPA5sOqHqbzaMsW7zj+enlhuEUeMdlxqfQs1QtTERrzX2HXv/JZ43rASH4ycuGftLR2v3AOHhtBCoYOCszhOiHYGmH0I9lwi8HDRTXjkuoRU1Zxmz9rSIQy35/aypV339OS7p4VydO5LibEiwugzaNHXCZalEPShhcWzTAq6+uCb+jA9ob+Za4yICRdIjgot/E5ZQNZl2VEhFmWR7pYSB7MB24z0ysVf3igAc50/3Lr4d7mQ7SgmtKFoejGOjvb75YOq/pEJGxCo1xIS+jhZklyLuiwmtyDe7Xmpn33fip22qx2iF7FYW71AyuT3tTPn2a7Jc2wXlPCAxB6dqx0eeX4fWA3+RIosUTxTyA7a/SM95pwN9FKYFPZ8TcTqDIq0Ntc57IdX5u1To=";
            Stimulsoft.Base.StiFontCollection.AddFontFile(CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Content") + "/B Mitra.TTF");
            var report = new StiReport();
            report.Load(CommonHelper.MapPath("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Reports/") + "officialFactor.mrt");
            report.Compile();
            report.RegBusinessObject("DataList", "Data", orders);
            //report["Now"] = DateTime.Now.ToPersianDate();
            //report["SiteAddress"] = _storeContext.CurrentStore.Url;
            //report["CompanyName"] = _storeContext.CurrentStore.Name;

            //string conditions = CreateConditionText(searchCondition);
            //report["SearchConditions"] = conditions;
            report.Render();
            StiPdfExportService service = new StiPdfExportService();
            service.ExportPdf(report, stream);
            stream.Seek(0, SeekOrigin.Begin);
        }

        private string CreateConditionText(OrderSearchCondition searchCondition)
        {
            string conditions = "";
            if (searchCondition.FromDate.HasValue)
            {
                conditions += $"از تاریخ : {searchCondition.FromDate.Value.ToPersianDate()} - ";
            }
            if (searchCondition.ToDate.HasValue)
            {
                conditions += $"تا تاریخ : {searchCondition.ToDate.Value.ToPersianDate()} - ";
            }
            if (!string.IsNullOrEmpty(searchCondition.OrderSerialFrom))
            {
                conditions += $"از شماره سفارش : {searchCondition.OrderSerialFrom} - ";
            }
            if (!string.IsNullOrEmpty(searchCondition.OrderSerialTo))
            {
                conditions += $"تا شماره سفارش : {searchCondition.OrderSerialTo} - ";
            }
            if (!string.IsNullOrEmpty(searchCondition.RecieverName))
            {
                conditions += $"گیرنده نامه : {searchCondition.RecieverName} - ";
            }
            if (searchCondition.OrderStatus != 0)
            {
                conditions += $"وضعیت سفارش : { ((OrderStatus)searchCondition.OrderStatus).GetLocalizedEnum(_localizationService, _workContext)} - ";
            }
            if (searchCondition.PayStatus != 0)
            {
                conditions += $"وضعیت پرداخت : {((PaymentStatus)searchCondition.PayStatus).GetLocalizedEnum(_localizationService, _workContext)} - ";
            }
            if (searchCondition.RecieverProvinceId != 0)
            {
                var provinceName = _countryService.GetCountryById(searchCondition.RecieverProvinceId.GetValueOrDefault(0))?.Name;
                conditions += $"استان گیرنده : {provinceName} - ";
            }
            if (searchCondition.RecieverCityId != 0)
            {
                var cityName = _stateProvinceService.GetStateProvinceById(searchCondition.RecieverCityId.GetValueOrDefault(0))?.Name;
                conditions += $"شهر گیرنده : {cityName} - ";
            }
            if (searchCondition.SenderProvinceId != 0)
            {
                var provinceName = _countryService.GetCountryById(searchCondition.SenderProvinceId.GetValueOrDefault(0))?.Name;
                conditions += $"استان فرستنده : {provinceName} - ";
            }
            if (searchCondition.SenderCityId != 0)
            {
                var cityName = _stateProvinceService.GetStateProvinceById(searchCondition.SenderCityId.GetValueOrDefault(0))?.Name;
                conditions += $"شهر فرستنده : {cityName} - ";
            }
            if (!string.IsNullOrEmpty(conditions))
            {
                conditions = conditions.Substring(0, conditions.Length - 2);
                //conditions = conditions.Insert(0, "فیلتر های گزارش : ");
            }

            return conditions;
        }
    }
}

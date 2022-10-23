using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.Ticket.Domain;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FrotelServiceLibrary;
using System.Text;
using System.Threading.Tasks;
using FrotelServiceLibrary.Input;
using System.Globalization;
using Nop.Core.Data;
using Nop.Services.Directory;
using Nop.plugin.Orders.ExtendedShipment.Services.Messages;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Plugin.Misc.PostbarDashboard.Domain;

namespace Nop.Plugin.Misc.PostbarDashboard.Services
{
    public class CODRequestService : ICODRequestService
    {
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IExtnOrderProcessingService _extnOrderProcessingService;
        private readonly IRepository<Tbl_RequestCODCustomer> _repository_Tbl_RequestCODCustomer;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IPostexEmailSender _postexEmailSender;
        private readonly IRepository<Tbl_Ticket> _repository_Tbl_Ticket;
        private readonly IRepository<Tbl_Ticket_Detail> _repository_Tbl_Ticket_Detail;
        private readonly ILogger _logger;
        private readonly IRepository<Order> _orderRepository;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IRepository<Nop.plugin.Orders.ExtendedShipment.Models.StateCodemodel> _repositoryStateCode;
        private readonly IRepository<Nop.plugin.Orders.ExtendedShipment.Models.CountryCodeModel> _repositoryCountryCode;
        private readonly IRepository<Tbl_CODRequestLog> _repositoryCodRequestLog;

        public CODRequestService(IWorkContext workContext,
            ICustomerService customerService,
            IShoppingCartService shoppingCartService,
            IProductService productService,
            IStoreContext storeContext,
            IExtnOrderProcessingService extnOrderProcessingService,
            IRepository<Tbl_RequestCODCustomer> repository_Tbl_RequestCODCustomer,
            IStateProvinceService stateProvinceService,
            IPostexEmailSender postexEmailSender,
            IRepository<Tbl_Ticket> repository_Tbl_Ticket,
            IRepository<Tbl_Ticket_Detail> repository_Tbl_Ticket_Detail,
            ILogger logger,
            IRepository<Order> orderRepository,
            IEmailAccountService emailAccountService,
            IRepository<Nop.plugin.Orders.ExtendedShipment.Models.StateCodemodel> repositoryStateCode,
            IRepository<plugin.Orders.ExtendedShipment.Models.CountryCodeModel> repositoryCountryCode,
            IRepository<Tbl_CODRequestLog> repositoryCodRequestLog)
        {
            _workContext = workContext;
            _customerService = customerService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _storeContext = storeContext;
            _extnOrderProcessingService = extnOrderProcessingService;
            _repository_Tbl_RequestCODCustomer = repository_Tbl_RequestCODCustomer;
            _stateProvinceService = stateProvinceService;
            _postexEmailSender = postexEmailSender;
            _repository_Tbl_Ticket = repository_Tbl_Ticket;
            _repository_Tbl_Ticket_Detail = repository_Tbl_Ticket_Detail;
            _logger = logger;
            _orderRepository = orderRepository;
            _emailAccountService = emailAccountService;
            _repositoryStateCode = repositoryStateCode;
            _repositoryCountryCode = repositoryCountryCode;
            _repositoryCodRequestLog = repositoryCodRequestLog;
        }

        public void GetCODRequestPdf(Stream stream, string cityName, string customerFullName, string customerSenderAddress, string customerPostalCode, string code)
        {
            Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHm4lqN+V3/fvJECXRrqdoEPZqYyFXh3g3K9oCDFvgzMl4c9KudQQwMJ6nO6w7rHz59BhwYgDE0QzKtjY2WxEejTNewbNDXY492M2mDsK1Hb4t6MoFYGbSoID0gow3VC5cFhvcOKVoagiHq6/4iqFc3RS7nZQBp95+WB6N1fR//H7OBlvfuBKldvC1pJh6pTW/7HRdOclErt/EGwivx9SpuDNabWWBfIZJBdJsvKjpoIsDQGibWk8Y8F9V1mCLf88FurWwEZ9WzXePbJn+wfabHM+a7pqDAXhsaW33UEW6vQ3kgLrVjbcHWdhtYbp5j6ZeIMLBCW2LXiCZpzy3N3XqjlroV/s7KRZGeZMvRrLWhcM8YJ6sBTMUyQrF/Fk3d5f0WbAf7+opIA5rhxY+qgwWcE42S+HOOcMgb2IWtj5ycC/GXZ4o9qtk2WWJzCC+ygckXK7iI5pzhzScGmLCBrFnjyE5EP65FViVDSCcNl6hUFw7wwRtcQq5pnOu6wHRXsSpwNPufWaRqE4gd7hbrQoPqki4BLOYgGOQjZ3kJdc7MG47nVT62z1ScLsjH73q4yhABt8h1wmJl0LanKnE6EGkvZPCrDBBf0sZp12sJrxK8FGsPUKrBNZ5f4VuYkdzWS1ed2m5MZNHkDDd3LncYkQnLPh/MYAsnc2ud715njwiM62xIWYk9lIrmCWJU3JVcexRDS3/l7Po9UU5gx2xJKUW1tWOXKQ9GqQZYtn/rl804VMC1/tWr4X2XU1otulny5P8YkZ7BTG6zmkcW6raROxv7xkSUwE0LK2FmIpeBr29Zxn0QUYBtC09J+wVD5y00f/1jMU+k1jQmi1n2tpWgy6CxAHPPrQFhk0FsGXEDg82IELCKncbqkfqgT0QyV262YL2uxrcbcJ1Wrzf4llEX90hMa3oZK5wdQLrCBkXIlXverhGL3I09fR6lcXDMj8A3vrQrhr4bwYcWTH0hyJCPVLYIVxAFLwgV8wk700QOo3z32sSWKnyMHVx3brFB8I7rOPaW2Sc8rBxc1E9EvNcqiTwRd9QA3lIdR2Hpc4jyn5dx8PT6GTNGXNKhoTZDLrGMtYqXw4rVOXfkE8+opF4+/H602ockCI/IY7C65+sxooNgSg+9LpzImUt6GdtrvF8HrvJ0rUW6ZiAmGaQmqdrHfwC3K+QKU4UEWuLU1GPG+PKMrYEbtT/Hei7NPA5sOqHqbzaMsW7zj+enlhuEUeMdlxqfQs1QtTERrzX2HXv/JZ43rASH4ycuGftLR2v3AOHhtBCoYOCszhOiHYGmH0I9lwi8HDRTXjkuoRU1Zxmz9rSIQy35/aypV339OS7p4VydO5LibEiwugzaNHXCZalEPShhcWzTAq6+uCb+jA9ob+Za4yICRdIjgot/E5ZQNZl2VEhFmWR7pYSB7MB24z0ysVf3igAc50/3Lr4d7mQ7SgmtKFoejGOjvb75YOq/pEJGxCo1xIS+jhZklyLuiwmtyDe7Xmpn33fip22qx2iF7FYW71AyuT3tTPn2a7Jc2wXlPCAxB6dqx0eeX4fWA3+RIosUTxTyA7a/SM95pwN9FKYFPZ8TcTqDIq0Ntc57IdX5u1To=";
            //Stimulsoft.Base.StiFontCollection.AddFontFile(CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Content") + "/B Mitra.TTF");
            var report = new StiReport();
            report.Load(CommonHelper.MapPath("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Reports/") + "GetwayRegisterPdf.mrt");
            report.Compile();
            report["CityName"] = cityName;
            report["CustomerFullName"] = customerFullName;
            report["CustomerSenderAddress"] = customerSenderAddress;
            report["CustomerPostalCode"] = customerPostalCode;
            string date = DateTime.Now.ToPersianDate();
            report["Date"] = date;
            report["MailNo"] = $"{date.Substring(0, 7).Replace("/", "")}{code}";
            report.Render();
            var servicePdf = new StiPdfExportService();
            servicePdf.ExportPdf(report, stream);
        }

        public void GetCODRequestExcel(Stream stream, string customerFullName,
            string customerAddress, string customerTel,
            string customerMobile, string customerPostalCode)
        {
            Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHm4lqN+V3/fvJECXRrqdoEPZqYyFXh3g3K9oCDFvgzMl4c9KudQQwMJ6nO6w7rHz59BhwYgDE0QzKtjY2WxEejTNewbNDXY492M2mDsK1Hb4t6MoFYGbSoID0gow3VC5cFhvcOKVoagiHq6/4iqFc3RS7nZQBp95+WB6N1fR//H7OBlvfuBKldvC1pJh6pTW/7HRdOclErt/EGwivx9SpuDNabWWBfIZJBdJsvKjpoIsDQGibWk8Y8F9V1mCLf88FurWwEZ9WzXePbJn+wfabHM+a7pqDAXhsaW33UEW6vQ3kgLrVjbcHWdhtYbp5j6ZeIMLBCW2LXiCZpzy3N3XqjlroV/s7KRZGeZMvRrLWhcM8YJ6sBTMUyQrF/Fk3d5f0WbAf7+opIA5rhxY+qgwWcE42S+HOOcMgb2IWtj5ycC/GXZ4o9qtk2WWJzCC+ygckXK7iI5pzhzScGmLCBrFnjyE5EP65FViVDSCcNl6hUFw7wwRtcQq5pnOu6wHRXsSpwNPufWaRqE4gd7hbrQoPqki4BLOYgGOQjZ3kJdc7MG47nVT62z1ScLsjH73q4yhABt8h1wmJl0LanKnE6EGkvZPCrDBBf0sZp12sJrxK8FGsPUKrBNZ5f4VuYkdzWS1ed2m5MZNHkDDd3LncYkQnLPh/MYAsnc2ud715njwiM62xIWYk9lIrmCWJU3JVcexRDS3/l7Po9UU5gx2xJKUW1tWOXKQ9GqQZYtn/rl804VMC1/tWr4X2XU1otulny5P8YkZ7BTG6zmkcW6raROxv7xkSUwE0LK2FmIpeBr29Zxn0QUYBtC09J+wVD5y00f/1jMU+k1jQmi1n2tpWgy6CxAHPPrQFhk0FsGXEDg82IELCKncbqkfqgT0QyV262YL2uxrcbcJ1Wrzf4llEX90hMa3oZK5wdQLrCBkXIlXverhGL3I09fR6lcXDMj8A3vrQrhr4bwYcWTH0hyJCPVLYIVxAFLwgV8wk700QOo3z32sSWKnyMHVx3brFB8I7rOPaW2Sc8rBxc1E9EvNcqiTwRd9QA3lIdR2Hpc4jyn5dx8PT6GTNGXNKhoTZDLrGMtYqXw4rVOXfkE8+opF4+/H602ockCI/IY7C65+sxooNgSg+9LpzImUt6GdtrvF8HrvJ0rUW6ZiAmGaQmqdrHfwC3K+QKU4UEWuLU1GPG+PKMrYEbtT/Hei7NPA5sOqHqbzaMsW7zj+enlhuEUeMdlxqfQs1QtTERrzX2HXv/JZ43rASH4ycuGftLR2v3AOHhtBCoYOCszhOiHYGmH0I9lwi8HDRTXjkuoRU1Zxmz9rSIQy35/aypV339OS7p4VydO5LibEiwugzaNHXCZalEPShhcWzTAq6+uCb+jA9ob+Za4yICRdIjgot/E5ZQNZl2VEhFmWR7pYSB7MB24z0ysVf3igAc50/3Lr4d7mQ7SgmtKFoejGOjvb75YOq/pEJGxCo1xIS+jhZklyLuiwmtyDe7Xmpn33fip22qx2iF7FYW71AyuT3tTPn2a7Jc2wXlPCAxB6dqx0eeX4fWA3+RIosUTxTyA7a/SM95pwN9FKYFPZ8TcTqDIq0Ntc57IdX5u1To=";
            Stimulsoft.Base.StiFontCollection.AddFontFile(CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Content") + "/B Mitra.TTF");
            var report = new StiReport();
            report.Load(CommonHelper.MapPath("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Reports/") + "gatewayRegisterExcel.mrt");
            report.Compile();
            report["CustomerFullName"] = customerFullName;
            report["CustomerAddress"] = customerAddress;
            report["CustomerTel"] = customerTel;
            report["CustomerMobile"] = customerMobile;
            report["CustomerPostalCode"] = customerPostalCode;
            report.Render();
            StiExcelExportService service = new StiExcelExportService();
            service.ExportExcel(report, stream);
        }
        //public void GetCODRequestPdf(Stream stream, string cityName, string customerFullName, string customerSenderAddress, string customerPostalCode, string code)
        //{
        //    Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHm4lqN+V3/fvJECXRrqdoEPZqYyFXh3g3K9oCDFvgzMl4c9KudQQwMJ6nO6w7rHz59BhwYgDE0QzKtjY2WxEejTNewbNDXY492M2mDsK1Hb4t6MoFYGbSoID0gow3VC5cFhvcOKVoagiHq6/4iqFc3RS7nZQBp95+WB6N1fR//H7OBlvfuBKldvC1pJh6pTW/7HRdOclErt/EGwivx9SpuDNabWWBfIZJBdJsvKjpoIsDQGibWk8Y8F9V1mCLf88FurWwEZ9WzXePbJn+wfabHM+a7pqDAXhsaW33UEW6vQ3kgLrVjbcHWdhtYbp5j6ZeIMLBCW2LXiCZpzy3N3XqjlroV/s7KRZGeZMvRrLWhcM8YJ6sBTMUyQrF/Fk3d5f0WbAf7+opIA5rhxY+qgwWcE42S+HOOcMgb2IWtj5ycC/GXZ4o9qtk2WWJzCC+ygckXK7iI5pzhzScGmLCBrFnjyE5EP65FViVDSCcNl6hUFw7wwRtcQq5pnOu6wHRXsSpwNPufWaRqE4gd7hbrQoPqki4BLOYgGOQjZ3kJdc7MG47nVT62z1ScLsjH73q4yhABt8h1wmJl0LanKnE6EGkvZPCrDBBf0sZp12sJrxK8FGsPUKrBNZ5f4VuYkdzWS1ed2m5MZNHkDDd3LncYkQnLPh/MYAsnc2ud715njwiM62xIWYk9lIrmCWJU3JVcexRDS3/l7Po9UU5gx2xJKUW1tWOXKQ9GqQZYtn/rl804VMC1/tWr4X2XU1otulny5P8YkZ7BTG6zmkcW6raROxv7xkSUwE0LK2FmIpeBr29Zxn0QUYBtC09J+wVD5y00f/1jMU+k1jQmi1n2tpWgy6CxAHPPrQFhk0FsGXEDg82IELCKncbqkfqgT0QyV262YL2uxrcbcJ1Wrzf4llEX90hMa3oZK5wdQLrCBkXIlXverhGL3I09fR6lcXDMj8A3vrQrhr4bwYcWTH0hyJCPVLYIVxAFLwgV8wk700QOo3z32sSWKnyMHVx3brFB8I7rOPaW2Sc8rBxc1E9EvNcqiTwRd9QA3lIdR2Hpc4jyn5dx8PT6GTNGXNKhoTZDLrGMtYqXw4rVOXfkE8+opF4+/H602ockCI/IY7C65+sxooNgSg+9LpzImUt6GdtrvF8HrvJ0rUW6ZiAmGaQmqdrHfwC3K+QKU4UEWuLU1GPG+PKMrYEbtT/Hei7NPA5sOqHqbzaMsW7zj+enlhuEUeMdlxqfQs1QtTERrzX2HXv/JZ43rASH4ycuGftLR2v3AOHhtBCoYOCszhOiHYGmH0I9lwi8HDRTXjkuoRU1Zxmz9rSIQy35/aypV339OS7p4VydO5LibEiwugzaNHXCZalEPShhcWzTAq6+uCb+jA9ob+Za4yICRdIjgot/E5ZQNZl2VEhFmWR7pYSB7MB24z0ysVf3igAc50/3Lr4d7mQ7SgmtKFoejGOjvb75YOq/pEJGxCo1xIS+jhZklyLuiwmtyDe7Xmpn33fip22qx2iF7FYW71AyuT3tTPn2a7Jc2wXlPCAxB6dqx0eeX4fWA3+RIosUTxTyA7a/SM95pwN9FKYFPZ8TcTqDIq0Ntc57IdX5u1To=";
        //    //Stimulsoft.Base.StiFontCollection.AddFontFile(CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Content") + "/B Mitra.TTF");
        //    var report = new StiReport();
        //    report.Load(CommonHelper.MapPath("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Reports/") + "GetwayRegisterPdf.mrt");
        //    report.Compile();
        //    report["CityName"] = cityName;
        //    report["CustomerFullName"] = customerFullName;
        //    report["CustomerSenderAddress"] = customerSenderAddress;
        //    report["CustomerPostalCode"] = customerPostalCode;
        //    string date = DateTime.Now.ToPersianDate();
        //    report["Date"] = date;
        //    report["MailNo"] = $"{date.Substring(0, 7).Replace("/", "")}{code}";
        //    report.Render();
        //    var servicePdf = new StiPdfExportService();
        //    servicePdf.ExportPdf(report, stream);
        //}
        public PlaceOrderResult ProcessCODRequestOrder(Tbl_RequestCODCustomer newRequestCOD, string paymentMethod)
        {
            if (_workContext.CurrentCustomer.BillingAddress == null)
            {
                _workContext.CurrentCustomer.BillingAddress = new Core.Domain.Common.Address()
                {
                    Address1 = newRequestCOD.Address,
                    Email = newRequestCOD.Username
                };
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
            }
            if (_workContext.CurrentCustomer.ShoppingCartItems.Any())
            {
                while (_workContext.CurrentCustomer.ShoppingCartItems.Any())
                {
                    _workContext.CurrentCustomer.ShoppingCartItems.ToList().ForEach(sci => _shoppingCartService.DeleteShoppingCartItem(sci, false));
                }
            }

            var product = _productService.GetProductById(10435);
            if (product == null)
            {
                return new PlaceOrderResult()
                {
                    Errors = new List<string>() { "در حال حاضر امکان ثبت درخواست وجود ندارد" },
                    PlacedOrder = null
                };
            }

            var warnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer, product, ShoppingCartType.ShoppingCart,
                         _storeContext.CurrentStore.Id, null,
                         0, automaticallyAddRequiredProductsIfEnabled: false);
            if (warnings.Any())
            {
                return new PlaceOrderResult()
                {
                    Errors = warnings,
                    PlacedOrder = null
                };
            }

            var ppr = new ProcessPaymentRequest
            {
                CustomerId = _workContext.CurrentCustomer.Id,
                StoreId = _storeContext.CurrentStore.Id,
                PaymentMethodSystemName = paymentMethod
            };

            return _extnOrderProcessingService.PlaceOrderCarton(ppr);
        }

        public void CODRequestPaid(Tbl_RequestCODCustomer requestCOD, bool insertTicket = true)
        {
            //Tbl_RequestCODCustomer newRequestCOD = _repository_Tbl_RequestCODCustomer.Table.FirstOrDefault(p=>p.OrderId == orderId);
            //if (newRequestCOD == null)
            //{
            //    return;
            //}
            bool robotSuccessfull = true;
            bool emailSent = true;
            string emailException = "";
            string robotException = "";
            var log = _repositoryCodRequestLog.Table.FirstOrDefault(p => p.CODRequestId == requestCOD.Id);

            try
            {
                var stateCode = _repositoryCountryCode.Table.FirstOrDefault(p => p.CountryId == requestCOD.StateId);
                var cityCode = _repositoryStateCode.Table.FirstOrDefault(p => p.stateId == requestCOD.CityId);
                int city1, state;
                //طبق گفته آقای وزیری گیت وی پست برای مناطق پستی تهران جای کد استان و شهرستان عوض میشه
                if (cityCode.StateCode.StartsWith("10") && stateCode.CountryCode == "1")
                {
                    city1 = Convert.ToInt32(stateCode.CountryCode);
                    state = Convert.ToInt32(cityCode.StateCode);
                }
                else
                {
                    city1 = Convert.ToInt32(cityCode.StateCode);
                    state = Convert.ToInt32(stateCode.CountryCode);
                }
                var city = _stateProvinceService.GetStateProvinceById(requestCOD.CityId);
                var cityName = city?.Name;

                #region Register In gateway  With Robot
                using (PostGatewayServiceManager postGatewayServiceManager = new PostGatewayServiceManager())
                {
                    var loginResult = postGatewayServiceManager.Login(new LoginInput
                    {
                        UserName = "postbar",
                        Password = "2a1234@A!@#$"
                    }).GetAwaiter().GetResult();
                    //string year = DateTime.Now.Year.ToString();
                    //string month = DateTime.Now.Month.ToString("00");
                    //string day = DateTime.Now.Day.ToString("00");

                    var now = DateTime.Now;
                    var pcDate = new PersianCalendar();

                    var newshopoutput = postGatewayServiceManager.NewShop(new NewShopInput
                    {
                        Site = "postbar.ir",
                        ManagerName = requestCOD.Fname + " " + requestCOD.Lname,
                        Name = $"{cityName ?? ""}-{requestCOD.Fname} {requestCOD.Lname}",
                        Email = "info@postbar.ir",
                        Mobile = requestCOD.Username,
                        Tel = requestCOD.Username,
                        PostalCode = requestCOD.CodePosti,
                        State = state,
                        City = city1,
                        Address = requestCOD.Address,
                        NationalCode = requestCOD.NatinolCode,
                        UserName = requestCOD.Username,
                        FishDateDay = pcDate.GetDayOfMonth(now).ToString(),
                        FishDateMonth = pcDate.GetMonth(now).ToString(),
                        FishDateYear = pcDate.GetYear(now).ToString()
                    }, loginResult.Cookies).GetAwaiter().GetResult();
                    if (!newshopoutput.Successfull)
                    {
                        robotSuccessfull = false;
                        //_repository_Tbl_RequestCODCustomer.Delete(requestCOD);
                        //return;
                    }
                    else
                    {
                        if (log != null)
                        {
                            log.RobotSucceed = true;
                            _repositoryCodRequestLog.Update(log);
                        }
                    }
                }
                #endregion

                #region Email
                var pdfStream = new MemoryStream();
                var excelStream = new MemoryStream();


                GetCODRequestExcel(excelStream,
                    requestCOD.Fname + " " + requestCOD.Lname,
                    requestCOD.Address,
                    requestCOD.Username,
                    requestCOD.Username,
                    requestCOD.CodePosti);

                GetCODRequestPdf(pdfStream,
                    city?.Name,
                    requestCOD.Fname + " " + requestCOD.Lname,
                    requestCOD.Address,
                    requestCOD.CodePosti,
                    requestCOD.Id.ToString());

                pdfStream.Seek(0, SeekOrigin.Begin);
                excelStream.Seek(0, SeekOrigin.Begin);

                var emailAccount = _emailAccountService.GetEmailAccountById(1);
                try
                {
                    _postexEmailSender.SendEmail(new Core.Domain.Messages.EmailAccount()
                    {
                        Email = emailAccount.Email,
                        EnableSsl = emailAccount.EnableSsl,
                        Host = emailAccount.Host,
                        Password = emailAccount.Password,
                        Port = emailAccount.Port,
                        UseDefaultCredentials = emailAccount.UseDefaultCredentials,
                        Username = emailAccount.Username
                    }, "پست بار", "", emailAccount.Email, "",
                    "ecommerce@post.ir",
                    //"info@postbar.ir",
                    "post",
                    bcc: new List<string> { emailAccount.Email },
                    attachmentFileNames: new List<string>
                    {
                        $"گیت وی-{requestCOD.Fname} {requestCOD.Lname}-{requestCOD.Id}.pdf",
                        $"گیت وی-{requestCOD.Fname} {requestCOD.Lname}-{requestCOD.Id}.xls"
                    }.ToArray(),
                    attachments: new List<Stream>()
                    {
                        pdfStream,
                        excelStream
                    });
                    if (log != null)
                    {
                        log.EmailSucceed = true;
                    }
                }
                catch (Exception ex)
                {
                    emailSent = false;
                    emailException = ex.ToString();
                    _logger.Error("خطایی در زمان ارسال ایمیل به گیت وی اتفاق افتاده", ex);
                    if (log != null)
                    {
                        log.Message += Environment.NewLine + "email " + Environment.NewLine + ex.ToString();
                    }
                }
                finally
                {
                    if (log != null)
                    {
                        _repositoryCodRequestLog.Update(log);
                    }

                }
                #endregion

            }
            catch (Exception ex)
            {
                robotSuccessfull = false;
                robotException = ex.ToString();
                if (log != null)
                {
                    log.Message += Environment.NewLine + "robot " + Environment.NewLine + ex.ToString();
                    _repositoryCodRequestLog.Update(log);
                }
                throw;
            }


            if (insertTicket)
            {
                #region  new ticket
                var currentTicket = _repository_Tbl_Ticket.Table.Where(p => p.IdCustomer == _workContext.CurrentCustomer.Id
                         && p.IdCategoryTicket == 2 && p.OrderCode == 0 && p.Issue.Contains("COD")).OrderByDescending(n => n.Id).FirstOrDefault();
                if (currentTicket != null || currentTicket.Status != 3)
                {
                    currentTicket.Status = 3;
                    currentTicket.DateUpdate = DateTime.Now;
                    currentTicket.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repository_Tbl_Ticket.Update(currentTicket);
                }
                string issue = $"درخواست فعال سازی حساب پس کرایهCOD";
                string ticketText = "";
                if (!robotSuccessfull)
                {
                    ticketText = $"{Environment.NewLine}ثبت نام اتوماتیک گیت وی انجام نشد لطفا نسبت به ثبت نام دستی اقدام نمایید{Environment.NewLine}";
                    if (string.IsNullOrEmpty(robotException))
                    {
                        ticketText += robotException + Environment.NewLine;
                    }
                }
                if (!emailSent)
                {
                    ticketText += $"ایمیل گیت وی ارسال نشد{Environment.NewLine}{emailException}";
                }
                Ticket.Domain.Tbl_Ticket newticket = new Ticket.Domain.Tbl_Ticket();
                newticket.DateInsert = DateTime.Now;
                newticket.DepartmentId = 2;
                newticket.IdCategoryTicket = 2;
                newticket.ProrityId = 4;
                newticket.IdCustomer = _workContext.CurrentCustomer.Id;
                newticket.IsActive = true;
                newticket.Issue = issue;
                newticket.OrderCode = 0;
                newticket.StoreId = _storeContext.CurrentStore.Id;
                newticket.TrackingCode = null;
                newticket.ShowCustomer = false;
                _repository_Tbl_Ticket.Insert(newticket);
                #endregion

                #region  add Deateil
                Ticket.Domain.Tbl_Ticket_Detail temp = new Ticket.Domain.Tbl_Ticket_Detail();
                temp.DateInsert = DateTime.Now;
                string link = String.Format("<a href=\"http://postex.ir/Admin/CODCustomer/{0}\">Click here</a>", requestCOD.Id.ToString());// "http://postex.ir/Admin/CODCustomer/"+ newRequestCOD.Id.ToString()
                temp.Description = $"درخواست فعال سازی حساب پس کرایهCOD لینک نمایش درخواست ها   {link}    کاربر گرامی نیازی به پاسخ به این تیکت نیست، درخواست را پیگیری بفرمایید، سامانه امنیتو{Environment.NewLine}{ticketText}";
                temp.IdTicket = newticket.Id;
                temp.Type = false;
                temp.UrlFile1 = null;
                temp.UrlFile2 = null;
                temp.UrlFile3 = null;
                _repository_Tbl_Ticket_Detail.Insert(temp);



                #endregion

                #region update id ticket in tbl Cod
                requestCOD.IdTicket = newticket.Id;
                _repository_Tbl_RequestCODCustomer.Update(requestCOD);
                #endregion
            }

        }

        public bool CustomerHasActiveCOD(int customerId)
        {
            var orders = _orderRepository.Table.Where(p => p.CustomerId == customerId && p.OrderItems.Any(q => q.ProductId == 10435) && p.OrderStatusId == (int)OrderStatus.Complete).ToList();
            if (!orders.Any())
            {
                return false;
            }
            var date = DateTime.Now.AddYears(-1);
            if (orders.All(p => p.CreatedOnUtc.Date < date.Date))
                return false;
            return true;
        }

        /// <summary>
        /// ثبت کامل COD 
        /// این تابع تیکت ایجاد نمیکند
        /// </summary>
        /// <param name="codModel"></param>
        /// <returns></returns>
        public FullCODRequestResultModel FullCODRequest(AddRequestCODModel codModel)
        {

            if (CustomerHasActiveCOD(codModel.CustomerId))
            {
                return new FullCODRequestResultModel()
                {
                    Success = false,
                    Message = "حساب کاربری پس کرایه شما در حال حاضر فعال می باشد"
                };
            }
            var userName = _customerService.GetCustomerById(codModel.CustomerId)?.Username;
            if (string.IsNullOrEmpty(userName))
            {
                return new FullCODRequestResultModel()
                {
                    Success = false,
                    Message = "شناسه کاربر اشتباه است"
                };
            }
            string country, state;

            #region Validate Data
            string error;
            var ReciverCountryCOde = _repositoryCountryCode.Table.FirstOrDefault(p => p.CountryId == codModel.Country);
            if (ReciverCountryCOde == null)
            {
                error = "کد استان گیرنده مربوط به  " + $"{ReciverCountryCOde}" + " یافت نشد";
                //Log(error, "");
            }
            if (string.IsNullOrEmpty(ReciverCountryCOde.CountryCode))
            {
                error = "کد استان گیرنده مربوط به  " + $"{ReciverCountryCOde.CountryCode}" + " یافت نشد";
                //Log(error, "");
            }
            var RecivercityCode = _repositoryStateCode.Table.FirstOrDefault(p => p.stateId == codModel.State);
            if (RecivercityCode == null)
            {
                error = "کد شهر گیرنده مربوط به  " + $"{RecivercityCode}" + " یافت نشد";
                //Log(error, "");
            }
            if (string.IsNullOrEmpty(RecivercityCode.StateCode))
            {
                error = "کد شهر گیرنده مربوط به  " + $"{RecivercityCode.StateCode}" + " یافت نشد";
                //Log(error, "");
            }
            //طبق گفته آقای وزیری گیت وی پست برای مناطق پستی تهران جای کد استان و شهرستان عوض میشه
            if (RecivercityCode.StateCode.StartsWith("10") && ReciverCountryCOde.CountryCode == "1")
            {
                state = ReciverCountryCOde.CountryCode;
                country = RecivercityCode.StateCode;
            }
            else
            {
                state = RecivercityCode.StateCode;
                country = ReciverCountryCOde.CountryCode;
            }

            #endregion

            #region  new RequestCOD
            var tempRequest = _repository_Tbl_RequestCODCustomer.Table.Where(p => p.IdCustomer == codModel.CustomerId
                && p.Status == 0).ToList();
            if (tempRequest.Any())
            {
                foreach (var item in tempRequest)
                {
                    item.Status = 2;
                    item.DateUpdate = DateTime.Now;
                    item.StaffIdLastAnswer = codModel.CustomerId;
                    _repository_Tbl_RequestCODCustomer.Update(item);
                }

            }
            Ticket.Domain.Tbl_RequestCODCustomer newRequestCOD = new Ticket.Domain.Tbl_RequestCODCustomer();
            newRequestCOD.Fname = codModel.Fname;
            newRequestCOD.Lname = codModel.Lname;
            newRequestCOD.NatinolCode = codModel.NationalCode;
            newRequestCOD.Shaba = codModel.AccountIBAN;
            newRequestCOD.Address = codModel.Address;
            newRequestCOD.StateId = codModel.Country;
            newRequestCOD.CityId = codModel.State;

            newRequestCOD.IdCustomer = codModel.CustomerId;
            newRequestCOD.IsActive = true;
            newRequestCOD.StoreId = _storeContext.CurrentStore.Id;
            newRequestCOD.DateInsert = DateTime.Now;
            newRequestCOD.Status = 0;
            newRequestCOD.Username = userName;
            newRequestCOD.CodePosti = codModel.PostalCode;
            _repository_Tbl_RequestCODCustomer.Insert(newRequestCOD);


            _repositoryCodRequestLog.Insert(new Tbl_CODRequestLog()
            {
                CODRequestId = newRequestCOD.Id
            });
            #endregion


            CODRequestPaid(newRequestCOD, false);

            return new FullCODRequestResultModel()
            {
                CODRequestId = newRequestCOD.Id,
                Success = true
            };

        }


        public List<FullCODRequestResultModel> BulkFullCODRequest(List<AddRequestCODModel> codModelList)
        {
            if (codModelList == null)
            {
                throw new ArgumentNullException(nameof(codModelList));
            }

            List<FullCODRequestResultModel> resultList = new List<FullCODRequestResultModel>();
            foreach (var codModel in codModelList)
            {
                try
                {
                    var result = FullCODRequest(codModel);
                    if (result.Success)
                    {
                        resultList.Add(new FullCODRequestResultModel()
                        {
                            CODRequestId = result.CODRequestId,
                            Success = true
                        });
                    }
                    else
                    {
                        resultList.Add(new FullCODRequestResultModel()
                        {
                            CODRequestId = result.CODRequestId,
                            Message = result.Message,
                            Success = false
                        });
                    }

                }
                catch (Exception ex)
                {
                    resultList.Add(new FullCODRequestResultModel()
                    {
                        Message = "خطا در هنگام ثبت " + ex.ToString(),
                        Success = false
                    });
                }
            }

            return resultList;

        }
    }

}

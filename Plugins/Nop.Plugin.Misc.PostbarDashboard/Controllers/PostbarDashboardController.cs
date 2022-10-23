using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Extensions;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.plugin.Orders.ExtendedShipment.Services.Messages;
using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Plugin.Misc.PostbarDashboard.Services;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using Nop.Plugin.Misc.Ticket.Domain;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport.Help;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Web.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Security.AntiXss;
using System.Web;

namespace Nop.Plugin.Misc.PostbarDashboard.Controllers
{
    public partial class PostbarDashboardController : BasePublicController
    {
        #region Fields
        private readonly IRepository<Tbl_CheckAvatarCustomer> _repositoryTbl_CheckAvatarCustomer;
        private readonly IRepository<Tbl_ViewVideoCustomer> _repositoryTbl_ViewVideoCustomer;
        private readonly IRepository<Tbl_CategoryTicket> _repositoryTbl_CategoryTicket;
        private readonly IRepository<Tbl_Product_PatternPricing> _repositoryTbl_Product_PatternPricing;
        private readonly IRepository<Tbl_AffiliateToCustomer> _repositoryTbl_AffiliateToCustomer;
        private readonly IRepository<Ticket.Domain.Tbl_RequestCODCustomer> _repositoryTbl_RequestCODCustomer;

        private readonly IRepository<Tbl_LogSMS> _repositoryTbl_LogSMS;
        private readonly IUserStatesService _userStatesService;
        private readonly IDbContext _dbContext;

        private readonly ILanguageService _languageService;
        private readonly IProductService _productService;
        private readonly INewCheckout _newCheckout;
        private readonly ICustomerService _customerServices;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IRewardPointServices _rewardPointServices;
        private readonly IOrderServices _orderServices;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPaymentService _paymentService;
        private readonly IRepository<Ticket.Domain.Tbl_Ticket> _repositoryTbl_Ticket;
        private readonly IRepository<Ticket.Domain.Tbl_Ticket_Detail> _repositoryTbl_TicketDetail;
        private readonly IRepository<Ticket.Domain.Tbl_Ticket_Department> _repositoryTbl_Department;
        private readonly IRepository<Ticket.Domain.Tbl_Ticket_Priority> _repositoryTbl_Priority;
        private readonly IRepository<Ticket.Domain.Tbl_FAQ> _repositoryTbl_FAQ;
        private readonly IRepository<Ticket.Domain.Tbl_FAQCategory> _repositoryTbl_FAQCategory;
        private readonly IRepository<Ticket.Domain.Tbl_Damages> _repositoryTbl_Damages;
        private readonly IRepository<Ticket.Domain.Tbl_Damages_Detail> _repositoryTbl_DamagesDetail;
        private readonly IShipmentService _shipmentService;
        private readonly IShipmentTrackingService _shipmentTrackingService;
        private readonly INotificationService _notificationService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IShipmentTrackingService _ShipmentTrackingService;
        private readonly ITrackingUbaarOrder_Service _trackingUbaarOrder_Service;
        private readonly IRepository<Nop.plugin.Orders.ExtendedShipment.Models.StateCodemodel> _repositoryStateCode;
        private readonly IRepository<Nop.plugin.Orders.ExtendedShipment.Models.CountryCodeModel> _repositoryCountryCode;
        private readonly IOrderExcelService _orderExcelService;
        private readonly IPostexEmailSender _postexEmailSender;
        private readonly ICODRequestService _codRequestService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IOrderPdfService _orderPdfService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IServiceTypeService _serviceTypeService;
        private readonly IDashboardService _dashboardService;
        private readonly ICodService _codService;
        private readonly IWebHelper _webHelper;
        private readonly ICartonService _cartonPriceService;
        private readonly IRepository<Tbl_CustomerDepositCode> _repository_TblCustomerDepositCode;
        private readonly IRewardPointCashoutService _rewardPointCashoutService;
        #endregion

        #region Ctor

        public PostbarDashboardController(
            ILanguageService languageService,
            IProductService productService,
            IRepository<Tbl_Product_PatternPricing> repositoryTbl_Product_PatternPricing,
            IRepository<Tbl_CategoryTicket> repositoryTbl_CategoryTicket,
            IRepository<Tbl_AffiliateToCustomer> repositoryTbl_AffiliateToCustomer,
            IRepository<Tbl_LogSMS> repositoryTbl_LogSMS,
            IRepository<Ticket.Domain.Tbl_RequestCODCustomer> repositoryTbl_RequestCODCustomer,
            ICustomerService customerServices,
            ILogger logger,
            IOrderService orderService,
            IOrderServices orderServices,
            IRewardPointService rewardPointService,
            IRewardPointServices rewardPointServices,
            IWorkContext workContext,
            IStoreContext storeContext,
            CustomerSettings customerSettings,
            ICustomerRegistrationService customerRegistrationService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            MediaSettings mediaSettings,
            IPriceFormatter priceFormatter,
            IPaymentService paymentService,
            INewCheckout newCheckout,
            IRepository<Tbl_CheckAvatarCustomer> repositoryTbl_CheckAvatarCustomer,
            IRepository<Tbl_ViewVideoCustomer> repositoryTbl_ViewVideoCustomer,
            IRepository<Ticket.Domain.Tbl_Ticket> repositoryTbl_Ticket,
            IRepository<Ticket.Domain.Tbl_Ticket_Detail> repositoryTbl_TicketDetail,
            IRepository<Ticket.Domain.Tbl_Ticket_Department> repositoryTbl_Department,
            IRepository<Ticket.Domain.Tbl_Ticket_Priority> repositoryTbl_Priority,
            IRepository<Ticket.Domain.Tbl_FAQ> repositoryTbl_FAQ,
            IRepository<Ticket.Domain.Tbl_FAQCategory> repositoryTbl_FAQCategory,
            IHostingEnvironment hostingEnvironment,
            IRepository<Ticket.Domain.Tbl_Damages> repositoryTbl_Damages,
            IRepository<Ticket.Domain.Tbl_Damages_Detail> repositoryTbl_DamagesDetail,
            IShipmentService shipmentService,
            INotificationService notificationService,
            IShipmentTrackingService shipmentTrackingService,
            IUserStatesService userStatesService,
            IDbContext dbContext,
            ICodService codService,
            IShipmentTrackingService ShipmentTrackingService,
            ITrackingUbaarOrder_Service trackingUbaarOrder_Service,
            IRepository<Nop.plugin.Orders.ExtendedShipment.Models.StateCodemodel> repositoryStateCode,
            IRepository<Nop.plugin.Orders.ExtendedShipment.Models.CountryCodeModel> repositoryCountryCode,
            IOrderExcelService orderExcelService,
            IPostexEmailSender postexEmailSender,
            ICODRequestService cODRequestService,
            IStateProvinceService stateProvinceService,
            IOrderPdfService orderPdfService,
            IExtendedShipmentService extendedShipmentService,
            IServiceTypeService serviceTypeService,
            IWebHelper webHelper,
            ICartonService cartonPriceService,
            IRepository<Tbl_CustomerDepositCode> repository_TblCustomerDepositCode,
            IRewardPointCashoutService rewardPointCashoutService,
            IDashboardService dashboardService
            )
        {
            _languageService = languageService;
            _productService = productService;
            _repositoryTbl_Product_PatternPricing = repositoryTbl_Product_PatternPricing;
            _repositoryTbl_CategoryTicket = repositoryTbl_CategoryTicket;
            _repositoryTbl_AffiliateToCustomer = repositoryTbl_AffiliateToCustomer;
            _newCheckout = newCheckout;
            this._dashboardService = dashboardService;
            this._logger = logger;
            this._customerServices = customerServices;
            this._orderService = orderService;
            this._rewardPointService = rewardPointService;
            this._rewardPointServices = rewardPointServices;
            this._orderServices = orderServices;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._customerSettings = customerSettings;
            this._customerRegistrationService = customerRegistrationService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._pictureService = pictureService;
            this._mediaSettings = mediaSettings;
            this._priceFormatter = priceFormatter;
            this._paymentService = paymentService;
            this._codService = codService;
            _repositoryTbl_CheckAvatarCustomer = repositoryTbl_CheckAvatarCustomer;
            _repositoryTbl_ViewVideoCustomer = repositoryTbl_ViewVideoCustomer;
            _repositoryTbl_Ticket = repositoryTbl_Ticket;
            _repositoryTbl_TicketDetail = repositoryTbl_TicketDetail;
            _repositoryTbl_Department = repositoryTbl_Department;
            _repositoryTbl_Priority = repositoryTbl_Priority;
            _repositoryTbl_FAQ = repositoryTbl_FAQ;
            _repositoryTbl_FAQCategory = repositoryTbl_FAQCategory;
            _hostingEnvironment = hostingEnvironment;
            _repositoryTbl_Damages = repositoryTbl_Damages;
            _repositoryTbl_DamagesDetail = repositoryTbl_DamagesDetail;
            _shipmentService = shipmentService;
            _notificationService = notificationService;
            _shipmentTrackingService = shipmentTrackingService;
            _repositoryTbl_LogSMS = repositoryTbl_LogSMS;
            _userStatesService = userStatesService;
            _dbContext = dbContext;
            _ShipmentTrackingService = ShipmentTrackingService;
            _repositoryTbl_RequestCODCustomer = repositoryTbl_RequestCODCustomer;
            _trackingUbaarOrder_Service = trackingUbaarOrder_Service;
            _repositoryStateCode = repositoryStateCode;
            _repositoryCountryCode = repositoryCountryCode;
            this._orderExcelService = orderExcelService;
            _postexEmailSender = postexEmailSender;
            _codRequestService = cODRequestService;
            _stateProvinceService = stateProvinceService;
            _orderPdfService = orderPdfService;
            _extendedShipmentService = extendedShipmentService;
            _serviceTypeService = serviceTypeService;
            _webHelper = webHelper;
            _cartonPriceService = cartonPriceService;
            _repository_TblCustomerDepositCode = repository_TblCustomerDepositCode;
            _rewardPointCashoutService = rewardPointCashoutService;
            _rewardPointCashoutService = rewardPointCashoutService;
        }

        #endregion


        public virtual IActionResult Dashboard()
        {
            var model = new DashboardModel();
            model.TotalOrdersCount = _orderServices.GetCustomerOrderCount(customerId: _workContext.CurrentCustomer.Id);
            model.LastOrder = _orderServices.GetCustomerLastOrder(customerId: _workContext.CurrentCustomer.Id);
            model.CountTicket = _repositoryTbl_Ticket.Table.Where(p => p.IdCustomer == _workContext.CurrentCustomer.Id && p.IsActive).ToList().Count;
            model.RewardPointsBalanced = _rewardPointService.GetRewardPointsBalance(
                customerId: _workContext.CurrentCustomer.Id,
                storeId: _storeContext.CurrentStore.Id);
            model.DepositCode = _repository_TblCustomerDepositCode.Table.FirstOrDefault(p => p.CustomerId == _workContext.CurrentCustomer.Id)?.DepositCode;


            var url = _pictureService.GetPictureUrl(
              _workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
              _mediaSettings.AvatarPictureSize,
              false);
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Dashboard.cshtml", model);
        }

        public virtual IActionResult Dashboard2()
        {
            if (!_workContext.CurrentCustomer.IsInCustomerRole("Registered"))
            {
                return RedirectToRoute("Login", new { returnUrl = "/Dashboard" });
            }
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Dashboard2.cshtml", (object)"");
        }
        public virtual IActionResult SavePersonalInfo(string name, string lastName, string AfficiantId)
        {
            string message = _dashboardService.execSp<string>(new { name, lastName, AfficiantId, Operation = "SaveInfo", CustomerId = _workContext.CurrentCustomer.Id }).FirstOrDefault();
            return Json(new { message });
        }
        [HttpPost]
        public virtual IActionResult ChangePassword(ChangePasswordModel model)
        {
            var customer = _workContext.CurrentCustomer;
            if (!customer.IsRegistered())
                return Challenge();


            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(model.CustomerId, true, _customerSettings.DefaultPasswordFormat, model.Password, model.OldPassword);
                var changePasswordResult = _dashboardService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    return Json(new
                    {
                        success = true
                    });
                }

                //errors
                foreach (var error in changePasswordResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        public virtual IActionResult ChargePrice(int? priceIndex, int? priceToCharge)
        {
            string message = _dashboardService.execSp<string>(new { priceIndex, priceToCharge, Operation = "ChargePrice", CustomerId = _workContext.CurrentCustomer.Id }).FirstOrDefault();
            return Json(new { message });
        }
        public virtual IActionResult TransferPrice(int priceToTransfer, string mobile)
        {
            try
            {

                if (mobile == null || priceToTransfer <= 0)
                {
                    return Json(new { message = "لطفا شماره تلفن مشتری و مبلغ مورد نظر را وارد نمایید" });
                }
                var balance = _rewardPointService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, 5);
                if (balance < priceToTransfer)
                {
                    return Json(new { message = "موجودی کافی نیست" });
                }
                var targetCustomer = _customerServices.GetCustomerByUsername(mobile);
                if (targetCustomer == null)
                {
                    return Json(new { message = "مشتری یافت نشد" });
                }
                _rewardPointService.AddRewardPointsHistoryEntry(_workContext.CurrentCustomer, -priceToTransfer, 5, "انتقال اعتبار به مشتری " + targetCustomer.Username);
                _rewardPointService.AddRewardPointsHistoryEntry(targetCustomer, priceToTransfer, 5, "دریافت اعتبار از مشتری " + _workContext.CurrentCustomer.Username);

                //string message = _dashboardService.execSp<string>(new { priceIndex, priceToTransfer, mobile, Operation = "TransferPrice", CustomerId = _workContext.CurrentCustomer.Id }).FirstOrDefault();
                return Json(new { message = "انتقال اعتبار با موفقیت انجام شد" });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }
        public virtual IActionResult SaveDashboardSettings(DashboardSettingInfoResult model)
        {
            try
            {
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, "ReciveSmsPersuit", model.ReciveSmsPersuit);
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, "ReciveOrderEmail", model.ReciveOrderEmail);
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, "AccessToPrinter", model.AccessToPrinter);
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, "AccessToPackage", model.AccessToPackage);
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, "UseLogo", model.UseLogo);

                return Json(new { message = "اطلاعات ذخیره شد" });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult GridCustommerAddresses(DataSourceRequest command, string address)
        {
            string outjson;
            var DashboardAddress = _dashboardService.execSp<DashboardAddressResult>(new { address, command.Page, command.PageSize, Operation = "GetAddressList", CustomerId = _workContext.CurrentCustomer.Id }, out outjson);

            var tt = new { TotalCount = (int)0 };
            tt = JsonConvert.DeserializeAnonymousType(outjson, tt);

            var gridModel = new DataSourceResult
            {
                Data = DashboardAddress.ToArray(),
                Total = tt.TotalCount
            };
            return Json(gridModel);
        }
        [HttpPost]
        public IActionResult SaveFactors(DashboardFactorRequestModel model)
        {

            model.Operation = "SaveFactorRequest";
            model.CustomerId = _workContext.CurrentCustomer.Id;
            //model.DateFrom2 = model.DateFrom2.FromPersianToGregorianDate().ToString();
            //model.DateTo2 = model.DateTo2.FromPersianToGregorianDate().ToString();
            string insertedId = "";
            string message = _dashboardService.execSp<string>(model, out insertedId).FirstOrDefault();

            try
            {
                if (message.Trim() == "اطلاعات با موفقیت ذخیره شد")
                {
                    Ticket.Domain.Tbl_Ticket newticket = new Ticket.Domain.Tbl_Ticket();
                    newticket.DateInsert = DateTime.Now;
                    newticket.DepartmentId = 3;
                    newticket.IdCategoryTicket = 3;
                    newticket.ProrityId = 1;
                    newticket.IdCustomer = _workContext.CurrentCustomer.Id;
                    newticket.IsActive = true;
                    newticket.Issue = "درخواست فاکتور سفارش";
                    newticket.OrderCode = 0;
                    newticket.StoreId = _storeContext.CurrentStore.Id;
                    newticket.TrackingCode = null;
                    newticket.ShowCustomer = false;
                    _repositoryTbl_Ticket.Insert(newticket);

                    #region  add Deateil
                    Ticket.Domain.Tbl_Ticket_Detail temp = new Ticket.Domain.Tbl_Ticket_Detail();
                    temp.DateInsert = DateTime.Now;
                    temp.Description = "درخواست فاکتور سفارش " + "لینک نمایش درخواست   " + "https://postex.ir/admin/ExtendedCustomer/RequestFactor?RequestFactorId=" + insertedId.ToString();
                    temp.IdTicket = newticket.Id;
                    temp.Type = false;
                    temp.UrlFile1 = null;
                    temp.UrlFile2 = null;
                    temp.UrlFile3 = null;
                    _repositoryTbl_TicketDetail.Insert(temp);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }

            return Json(new { message });
        }
        [HttpPost]
        public IActionResult GridFactors(DataSourceRequest command, string datefrom, string dateto)
        {
            string outjson;
            var DashboardFactors = _dashboardService.execSp<DashboardFactorRequestResult>(new { datefrom, dateto, command.Page, command.PageSize, Operation = "GetFactorList", CustomerId = _workContext.CurrentCustomer.Id }, out outjson);

            var tt = new { TotalCount = (int)0 };
            tt = JsonConvert.DeserializeAnonymousType(outjson, tt);

            var gridModel = new DataSourceResult
            {
                Data = DashboardFactors.ToArray(),
                Total = tt.TotalCount
            };
            return Json(gridModel);
        }

        [HttpPost]
        public IActionResult GridCustommerMessages(DataSourceRequest command, string message)
        {
            string outjson;
            var DashboardMessages = _dashboardService.execSp<DashboardMessagesResult>(new { message, command.Page, command.PageSize, Operation = "GetMessageList", CustomerId = _workContext.CurrentCustomer.Id }, out outjson);

            var tt = new { TotalCount = (int)0 };
            tt = JsonConvert.DeserializeAnonymousType(outjson, tt);

            var gridModel = new DataSourceResult
            {
                Data = DashboardMessages.ToArray(),
                Total = tt.TotalCount
            };
            return Json(gridModel);
        }
        [HttpPost]
        public IActionResult ReadCustomerMessage(int messageId, int state)
        {
            var msg = _dashboardService.execSp<DashboardMessagesResult>(new { messageId, state, Operation = "UpdateMessageState", CustomerId = _workContext.CurrentCustomer.Id }).FirstOrDefault();
            return Json(msg);
        }
        [HttpPost]
        public IActionResult GridCustommerOrders()
        {
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Dashboard2_OrderPartial.cshtml");
        }
        [HttpPost]
        public IActionResult createYourAffiliateId(string customerId)
        {
            try
            {
                string message = _dashboardService.execSp<string>(new { Operation = "CreateAffiliateId", CustomerId = customerId }).FirstOrDefault();
                return Json(new { message = "کد معرفی  ایجاد شد", success = true });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = false });
            }
        }
        public virtual IActionResult CustomerChargeWalletHistory()
        {
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/CustomerWalletChargeHistory.cshtml");
        }

        [HttpPost]
        public IActionResult SaveCahoutSetting(int? automaticCashoutMethod)
        {
            try
            {
                string spMessage = _rewardPointCashoutService.SaveSettingCashout(JsonConvert.SerializeObject(new { automaticCashoutMethod }), _workContext.CurrentCustomer.Id);
                return Json(new { failed = false, message = spMessage });
            }
            catch (Exception ex)
            {
                return Json(new { failed = true, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CheckDate(string d1, string d2)
        {
            if ((d2.FromPersianToGregorianDate() - d1.FromPersianToGregorianDate()).TotalDays > 30)
            {
                return Json(new
                {
                    message = "تعداد روزهای بازه ی تاریخ نباید از 30 روز تجاوز کند"
                });
            }
            return Json(new { message = "" });
        }
        public IActionResult ExcelCustommerWalletHistory(DataSourceRequest command, RewardPointsCashoutInputModel model, string strPaid)
        {
            int co = 0;
            if (!string.IsNullOrEmpty(strPaid)) co = strPaid.Split(',').Where(x => x.Trim().Length > 0).Count();
            if (co == 0 || co == 2)
            {
                model.paid = null;
            }
            else
            {
                if (strPaid.Split(',').Any(x => x.ToLower() == "true")) model.paid = true;
                if (strPaid.Split(',').Any(x => x.ToLower() == "false")) model.paid = false;
            }

            int count = 0;
            var rewardPoints = _rewardPointCashoutService.GetCustomerWalletHistory(model.dateFrom, model.dateTo, _workContext.CurrentCustomer.Id, model.chargeWalletType, model.paid, 0, 0, out count);

            var properties = new[]
            {
                new PropertyByName<ChargeWalletHistoryResult>("شماره فاکتور", p => p.Id),
                new PropertyByName<ChargeWalletHistoryResult>("شماره درخواست", p => p.orderId),
                new PropertyByName<ChargeWalletHistoryResult>("نوع تراکنش", p => p.ChargeWalletTypeName),
                new PropertyByName<ChargeWalletHistoryResult>("مبلغ", p => p.Point),
                new PropertyByName<ChargeWalletHistoryResult>("تاریخ", p => p.xShamsiDate),
                new PropertyByName<ChargeWalletHistoryResult>("تاریخ تسویه", p => p.xShamsiPayDate),
                new PropertyByName<ChargeWalletHistoryResult>("تاریخ درخواست تسویه", p => p.xShamsiRequestPayDate),
                new PropertyByName<ChargeWalletHistoryResult>("توضیحات", p => p.Description),
            };

            var bts = UtilityExtensions.ExportToXlsx(properties, rewardPoints);

            Thread.Sleep(1000);
            var stream = new MemoryStream(bts);

            return File(stream, MimeTypes.TextXlsx, $"ChargeWalletHistory_{Guid.NewGuid().ToString()}.xlsx");
        }
        [HttpPost]
        public IActionResult GridCustommerWalletHistory(DataSourceRequest command, RewardPointsCashoutInputModel model, string strPaid)
        {
            if (model.dateFrom == null || model.dateTo == null)
            {
                return Json(new DataSourceResult
                {
                    Data = new ChargeWalletHistoryResult[] { },
                    Total = 0,
                    ExtraData = ""
                });
            }
            int co = 0;
            if (!string.IsNullOrEmpty(strPaid)) co = strPaid.Split(',').Where(x => x.Trim().Length > 0).Count();
            if (co == 0 || co == 2)
            {
                model.paid = null;
            }
            else
            {
                if (strPaid.Split(',').Any(x => x.ToLower() == "true")) model.paid = true;
                if (strPaid.Split(',').Any(x => x.ToLower() == "false")) model.paid = false;
            }

            int count = 0;
            var rewardPoints = _rewardPointCashoutService.GetCustomerWalletHistory(model.dateFrom, model.dateTo, _workContext.CurrentCustomer.Id, model.chargeWalletType, model.paid, command.PageSize, command.Page - 1, out count);

            var gridModel = new DataSourceResult
            {
                Data = rewardPoints.ToArray(),
                Total = count
            };
            return Json(gridModel);
        }

        [HttpPost]
        public IActionResult PayRequest(string ChargeWalletIds, bool? CashoutTypeId, string ShebaToInsert, RewardPointsCashoutInputModel model, string strPaid)
        {
            try
            {
                if (CashoutTypeId == null)
                    return Json(new { failed = false, message = "لطفا روش تسویه حساب را انتخاب کنید" });

                if (model.dateFrom != null)
                {
                    ChargeWalletIds = "";
                    int count;

                    int co = 0;
                    if (!string.IsNullOrEmpty(strPaid)) co = strPaid.Split(',').Where(x => x.Trim().Length > 0).Count();
                    if (co == 0 || co == 2)
                    {
                        model.paid = null;
                    }
                    else
                    {
                        if (strPaid.Split(',').Any(x => x.ToLower() == "true")) model.paid = true;
                        if (strPaid.Split(',').Any(x => x.ToLower() == "false")) model.paid = false;
                    }

                    var searchres = _rewardPointCashoutService.GetCustomerWalletHistory(model.dateFrom, model.dateTo, _workContext.CurrentCustomer.Id, model.chargeWalletType, model.paid, 0, 0, out count);
                    ChargeWalletIds = string.Join(",", searchres.Select(x => x.Id));
                }
                string spMessage = _rewardPointCashoutService.RequestRewardPointCashout(ChargeWalletIds.TrimEnd(','), CashoutTypeId, ShebaToInsert, _workContext.CurrentCustomer.Id);
                return Json(new { failed = false, message = spMessage });
            }
            catch (Exception ex)
            {
                return Json(new { failed = true, message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult GetchargeWalletTypes()
        {
            return Json(_rewardPointCashoutService.getchargeWalletTypes());
        }
        public virtual IActionResult Wallet()
        {
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Wallet.cshtml");
        }

        public virtual IActionResult WalletPaged(int pageIndex, int pageSize, bool? firstTime)
        {
            var rewards = _rewardPointService.GetRewardPointsHistory(
                customerId: _workContext.CurrentCustomer.Id,
                pageIndex: pageIndex,
                pageSize: pageSize).ToList();
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.firstTime = firstTime;
            ViewBag.RowsCount = _rewardPointServices.GetRewardPointsCount(customerId: _workContext.CurrentCustomer.Id);

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/_WalletPaged.cshtml", rewards);
        }

        public virtual IActionResult Orders()
        {
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Orders.cshtml");
        }

        public virtual IActionResult OrderBillDetailExcel(string searchConditions)// Is Ok
        {
            var search = new OrderSearchCondition();
            if (!string.IsNullOrEmpty(searchConditions))
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                search = JsonConvert.DeserializeObject<OrderSearchCondition>(searchConditions, settings);
            }

            var orders = _orderServices.SearchOrderBillDetail(
                customerId: _workContext.CurrentCustomer.Id,
                pageIndex: 0,
                pageSize: 3000,
                OrderIdFrom: string.IsNullOrEmpty(search.OrderSerialFrom) ? 0 : Convert.ToInt32(search.OrderSerialFrom),
                OrderIdTo: string.IsNullOrEmpty(search.OrderSerialTo) ? 0 : Convert.ToInt32(search.OrderSerialTo),
                ReciverName: search.RecieverName,
                ReciverCountryId: search.RecieverProvinceId.GetValueOrDefault(0),
                ReciverStateProvinceId: search.RecieverCityId.GetValueOrDefault(0),
                SenderCountryId: search.SenderProvinceId.GetValueOrDefault(0),
                SenderStateProvinceId: search.SenderCityId.GetValueOrDefault(0),
                createdFromUtc: search.FromDate.HasValue ? search.FromDate.Value.Date : search.FromDate,
                createdToUtc: search.ToDate.HasValue ? search.ToDate.Value.Date.AddHours(23).AddMinutes(59) : search.ToDate,
                OrderStatus: search.OrderStatus,
                PaymentStatus: search.PayStatus,
                ServiceTypes: search.ServiceTypes == 0 ? null : new List<int> { search.ServiceTypes }
                ).ToList();



            var stream = new MemoryStream();
            _orderExcelService.GetOrderBillDetailExcel(orders, search, stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/vnd.ms-excel", $"OrderBillDetailReport_{Guid.NewGuid().ToString()}.xls");
        }


        public virtual IActionResult OrdersPagedExcel(string searchConditions)
        {

            var search = new OrderSearchCondition();
            if (!string.IsNullOrEmpty(searchConditions))
            {
                search = JsonConvert.DeserializeObject<OrderSearchCondition>(searchConditions);
            }
            search.CustomerId = _workContext.CurrentCustomer.Id;
            search.StoreId = 5;

            var orderModel = _dashboardService.execSp<OrderModel>("Dashboard_Sp_OrdersPagedExcel", search);

            var stream = new MemoryStream();
            _orderExcelService.GetOrdersExcel(orderModel, search, stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/vnd.ms-excel", $"OrderReport_{Guid.NewGuid().ToString()}.xls");
        }

        public IActionResult DownloadWalletRecord()
        {
            string query = $@"SELECT TOP(1000)
	                            RPH.Id ,
                                RPH.Points ,
                                RPH.PointsBalance ,
                                RPH.Message ,
                               dbo.GregorianToJalali(dbo.Fn_ConvertToLocalDate(RPH.CreatedOnUtc),'yyyy/MM/dd HH:mm:ss')  CreateDate
                            FROM 
	                            dbo.RewardPointsHistory AS RPH
                            WHERE
	                            RPH.CustomerId = {_workContext.CurrentCustomer.Id}
                            ORDER BY RPH.CreatedOnUtc DESC ";
            var data = _dbContext.SqlQuery<WalletRecord>(query, new object[0]).ToList();
            var stream = new MemoryStream();
            _orderExcelService.GetWalletExcel(data, stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/vnd.ms-excel", $"Walletrecord_{Guid.NewGuid().ToString()}.xls");
        }
        public virtual IActionResult DownloadFactor(int requestFactorId)
        {
            string outjson;
            var DashboardFactors = _dashboardService.execSp<DashboardFactorRequestMRTModel>(new { requestFactorId, Page = 1, PageSize = 100000, Operation = "GetFactorReportList", CustomerId = _workContext.CurrentCustomer.Id }, out outjson);

            var stream = new MemoryStream();
            _orderPdfService.GetOrderFactorPdf(DashboardFactors, stream);
            return File(stream, "application/vnd.ms-excel", $"OrderFactor_{Guid.NewGuid().ToString()}.pdf");
        }
        public IActionResult SaveFactor(int requestFactorId, string year,bool overwrite)
        {
            string outjson;
            if (overwrite || !System.IO.File.Exists("C:\\FTP\\OfficalFactor\\" + year + "\\" + $"OrderFactor_{requestFactorId.ToString()}.pdf"))
            {
                var DashboardFactors = _dashboardService.execSp<DashboardFactorRequestMRTModel>(new { requestFactorId, Page = 1, PageSize = 100000, Operation = "GetFactorReportList", CustomerId = _workContext.CurrentCustomer.Id }, out outjson);

                var stream = new MemoryStream();
                _orderPdfService.GetOrderFactorPdf(DashboardFactors, stream);
                using (var fileStream = System.IO.File.Create("C:\\FTP\\OfficalFactor\\" + year + "\\" + $"OrderFactor_{requestFactorId.ToString()}.pdf"))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
            }
            return Ok();
        }
        public virtual IActionResult ChangeOrderState(int orderId, int state)
        {
            try
            {
                if (!_workContext.CurrentCustomer.IsInCustomerRole("SelfCollector"))
                    Json(new { success = false, message = "متاسفانه شما دسترسی به این قسمت ندارید" });
                string msg;
                var TrackingNumbers = _dashboardService.execSp<string>(new { orderId = orderId, Operation = "ChangeOrderState", CustomerId = _workContext.CurrentCustomer.Id }, out msg);
                if (!string.IsNullOrEmpty(msg))
                {
                    return Json(new { success = false, message = msg });
                }
                int gatewayState = (state == 1 ? 2 : 1);
                foreach (var item in TrackingNumbers)
                {
                    string res;
                    var ret = _codService.ChangeStatus(gatewayState, item, out res);
                    if (!ret) return Json(new { success = false, message = $"Error While Change State, Tracking Number = {item + Environment.NewLine + res}" });
                }
                return Json(new { success = true, message = "آماده ارسال به پست" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public virtual IActionResult OrdersBarcodeReport(string searchConditions)
        {
            var search = new OrderSearchCondition();
            if (!string.IsNullOrEmpty(searchConditions))
            {
                search = JsonConvert.DeserializeObject<OrderSearchCondition>(searchConditions);
            }
            if (!search.FromDate.HasValue && !search.ToDate.HasValue)
            {
                search.FromDate = DateTime.Now.AddDays(-7);
                search.ToDate = DateTime.Now;
            }
            else if (!search.FromDate.HasValue && search.ToDate.HasValue)
            {
                search.FromDate = search.ToDate.Value.AddDays(-7);
            }
            else if (search.FromDate.HasValue && !search.ToDate.HasValue)
            {
                search.ToDate = search.FromDate.Value.AddDays(7);
            }
            else if (search.FromDate.HasValue && search.ToDate.HasValue && (search.ToDate.Value - search.FromDate.Value).TotalDays > 8)
            {
                search.ToDate = search.FromDate.Value.AddDays(7);
            }

            var orders = _orderServices.SearchOrderBarcode(
                //storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id,
                OrderStatus: search.OrderStatus,
                pageIndex: 0,
                pageSize: 3000,
                OrderIdFrom: string.IsNullOrEmpty(search.OrderSerialFrom) ? 0 : Convert.ToInt32(search.OrderSerialFrom),
                OrderIdTo: string.IsNullOrEmpty(search.OrderSerialTo) ? 0 : Convert.ToInt32(search.OrderSerialTo),
                ReciverName: search.RecieverName,
                ReciverCountryId: search.RecieverProvinceId.GetValueOrDefault(0),
                ReciverStateProvinceId: search.RecieverCityId.GetValueOrDefault(0),
                SenderCountryId: search.SenderProvinceId.GetValueOrDefault(0),
                SenderStateProvinceId: search.SenderCityId.GetValueOrDefault(0),
                createdFromUtc: search.FromDate.HasValue ? search.FromDate.Value.Date : search.FromDate,
                createdToUtc: search.ToDate.HasValue ? search.ToDate.Value.Date.AddHours(23).AddMinutes(59) : search.ToDate,
                ServiceTypes: search.ServiceTypes == 0 ? null : new List<int> { search.ServiceTypes }
                ).ToList();

            orders.ForEach(x =>
            {
                x.BarCodeImage = _extendedShipmentService.getBarocdeImage(new Core.Domain.Shipping.Shipment() { TrackingNumber = x.TrackingNumber });
            });

            var stream = new MemoryStream();
            _orderPdfService.GetOrderBarcodePdf(orders, search, stream);
            return File(stream, MimeTypes.TextXlsx, $"OrderBarCodeReport_{Guid.NewGuid().ToString()}.pdf");
        }

        public virtual IActionResult WalletIncomeDetailsMiniAdmin(string searchConditions)
        {
            var search = new WalletIncomeMiniAdminInput();
            if (!string.IsNullOrEmpty(searchConditions))
            {
                search = JsonConvert.DeserializeObject<WalletIncomeMiniAdminInput>(searchConditions);
                search.Username = _workContext.CurrentCustomer.Username;
            }

            var result = _rewardPointCashoutService.execSpNormal<WalletIncomeDetailsMiniAdminResult>("Sp_WalletIncomeDetailsMiniAdmin", search);
            var stream = new MemoryStream();
            _orderExcelService.WalletIncomeDetailsMiniAdminExcel(result, search, stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, MimeTypes.TextXlsx, $"WalletIncomeDetailsMiniAdminReport_{Guid.NewGuid().ToString()}.xlsx");
        }
        public virtual IActionResult WalletIncomeForServicesMiniAdmin(string searchConditions)
        {
            var search = new WalletIncomeMiniAdminInput();
            if (!string.IsNullOrEmpty(searchConditions))
            {
                search = JsonConvert.DeserializeObject<WalletIncomeMiniAdminInput>(searchConditions);
                search.Username = _workContext.CurrentCustomer.Username;
            }

            var result = _rewardPointCashoutService.execSpNormal<WalletIncomeForServicesMiniAdminResult>("Sp_WalletIncomeForServicesMiniAdmin", search);
            var stream = new MemoryStream();
            _orderExcelService.WalletIncomeForServicesMiniAdminExcel(result, search, stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, MimeTypes.TextXlsx, $"WalletIncomeForServicesMiniAdminReport_{Guid.NewGuid().ToString()}.xlsx");
        }

        public virtual IActionResult ReportWithTrackingCodeMiniAdmin(string searchConditions)
        {
            var search = new WalletIncomeMiniAdminInput();
            if (!string.IsNullOrEmpty(searchConditions))
            {
                search = JsonConvert.DeserializeObject<WalletIncomeMiniAdminInput>(searchConditions);
                search.Username = _workContext.CurrentCustomer.Username;
            }

            var result = _rewardPointCashoutService.execSpNormal<WalletIncomeForServicesMiniAdminResult>("Sp_WalletIncomeForServicesMiniAdmin", search);
            var stream = new MemoryStream();
            _orderExcelService.WalletIncomeForServicesMiniAdminExcel(result, search, stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, MimeTypes.TextXlsx, $"WalletIncomeForServicesMiniAdminReport_{Guid.NewGuid().ToString()}.xlsx");
        }
        public virtual IActionResult OrdersBarcodeReportExcel(string searchConditions)
        {
            var search = new OrderSearchCondition();
            if (!string.IsNullOrEmpty(searchConditions))
            {
                search = JsonConvert.DeserializeObject<OrderSearchCondition>(searchConditions);
            }
            if (!search.FromDate.HasValue && !search.ToDate.HasValue)
            {
                search.FromDate = DateTime.Now.AddDays(-7);
                search.ToDate = DateTime.Now;
            }
            else if (!search.FromDate.HasValue && search.ToDate.HasValue)
            {
                search.FromDate = search.ToDate.Value.AddDays(-7);
            }
            else if (search.FromDate.HasValue && !search.ToDate.HasValue)
            {
                search.ToDate = search.FromDate.Value.AddDays(7);
            }
            else if (search.FromDate.HasValue && search.ToDate.HasValue && (search.ToDate.Value - search.FromDate.Value).TotalDays > 8)
            {
                search.ToDate = search.FromDate.Value.AddDays(7);
            }

            var orders = _orderServices.SearchOrderBarcode(
                //storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id,
                OrderStatus: search.OrderStatus,
                pageIndex: 0,
                pageSize: 3000,
                OrderIdFrom: string.IsNullOrEmpty(search.OrderSerialFrom) ? 0 : Convert.ToInt32(search.OrderSerialFrom),
                OrderIdTo: string.IsNullOrEmpty(search.OrderSerialTo) ? 0 : Convert.ToInt32(search.OrderSerialTo),
                ReciverName: search.RecieverName,
                ReciverCountryId: search.RecieverProvinceId.GetValueOrDefault(0),
                ReciverStateProvinceId: search.RecieverCityId.GetValueOrDefault(0),
                SenderCountryId: search.SenderProvinceId.GetValueOrDefault(0),
                SenderStateProvinceId: search.SenderCityId.GetValueOrDefault(0),
                createdFromUtc: search.FromDate.HasValue ? search.FromDate.Value.Date : search.FromDate,
                createdToUtc: search.ToDate.HasValue ? search.ToDate.Value.Date.AddHours(23).AddMinutes(59) : search.ToDate,
                ServiceTypes: search.ServiceTypes == 0 ? null : new List<int> { search.ServiceTypes }
                ).ToList();

            orders.ForEach(x =>
            {
                x.BarCodeImage = _extendedShipmentService.getBarocdeImage(new Core.Domain.Shipping.Shipment() { TrackingNumber = x.TrackingNumber });
            });

            var stream = new MemoryStream();
            _orderExcelService.GetOrderBarcodeExcel(orders, search, stream);
            return File(stream, MimeTypes.TextXlsx, $"OrderBarCodeReport_{Guid.NewGuid().ToString()}.xls");
        }


        public IActionResult GetServiceTypes()
        {
            return Ok(_serviceTypeService.GetServiceTypesByCurrentStoreId());
        }

        //[HttpGet("{pageIndex}/{pageSize}/{searchConditions?}")]
        public virtual IActionResult OrdersPaged(int pageIndex, int pageSize, string searchConditions)
        {
            var search = new OrderSearchCondition();
            if (!string.IsNullOrEmpty(searchConditions))
            {
                try
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    search = JsonConvert.DeserializeObject<OrderSearchCondition>(searchConditions, settings);
                }
                catch (Exception ex)
                {
                    LogException(ex);
                }
            }
            var orders = _orderServices.SearchOrders(
                 pageIndex: pageIndex,
                pageSize: pageSize,
                customerId: _workContext.CurrentCustomer.Id,
                SenderStateProvinceId: search.SenderCityId.GetValueOrDefault(0),
                SenderCountryId: search.SenderProvinceId.GetValueOrDefault(0),
                ReciverCountryId: search.RecieverProvinceId.GetValueOrDefault(0),
                ReciverStateProvinceId: search.RecieverCityId.GetValueOrDefault(0),
                ReciverName: search.RecieverName,
                OrderIdFrom: string.IsNullOrEmpty(search.OrderSerialFrom) ? 0 : Convert.ToInt32(search.OrderSerialFrom),
                OrderIdTo: string.IsNullOrEmpty(search.OrderSerialTo) ? 0 : Convert.ToInt32(search.OrderSerialTo),
                PaymentStatus: search.PayStatus,
                OrderStatus: search.OrderStatus,
                createdFromUtc: search.FromDate.HasValue ? search.FromDate.Value.Date : search.FromDate,
                createdToUtc: search.ToDate.HasValue ? search.ToDate.Value.Date.AddHours(23).AddMinutes(59) : search.ToDate,
                ServiceTypes: search.ServiceTypes == 0 ? null : new List<int> { search.ServiceTypes }
                );
            var orderModel = orders.Select(x => new OrderModel
            {
                OrderId = x.OrderId,
                CategoryName = x.CategoryName,
                CategoryId = x.CategoryId,
                OrderDate = x.OrderDate,
                OrderTotal = x.OrderTotal,
                OrderStatus = ((OrderStatus)x.OrderStatusId).GetLocalizedEnum(_localizationService, _workContext),
                PaymentStatus = ((PaymentStatus)x.PaymentStatusId).GetLocalizedEnum(_localizationService, _workContext),
                OrderStatusId = x.OrderStatusId,
                PaymentStatusId = x.PaymentStatusId,
                strBillingAddress = x.strBillingAddress,
                ShipmentsState = x.ShipmentsState
            }).ToList();

            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;

            //_orderServices.GetCustomerOrderCount(customerId: _workContext.CurrentCustomer.Id);
            if (orderModel.Any())
                return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/_OrdersPaged.cshtml", orderModel);
            return Content("سفارشی یافت نشد");
        }

        public virtual IActionResult CustomerInfo()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();
            var customer = _workContext.CurrentCustomer;

            var model = new CustomerInfModel()
            {
                CustomerInfoModel = new CustomerInfoModel
                {
                    FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName),
                    LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName),
                    Email = customer.Email,
                    Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                    AvatarUrl = _pictureService.GetPictureUrl(
                                    _workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                                    _mediaSettings.AvatarPictureSize,
                                    false),
                }
                ,
                ChangePasswordModel = new Web.Models.Customer.ChangePasswordModel()
            };

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/CustomerInfo.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult CustomerInfo(CustomerInfoModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();

            var customer = _workContext.CurrentCustomer;

            //email
            if (!customer.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
            {
                //change email
                var requireValidation = _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
                _customerRegistrationService.SetEmail(customer, model.Email.Trim(), requireValidation);
            }
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);

            return Content("");
        }

        public virtual IActionResult ChangePass()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/ChangePass.cshtml");
        }

        [HttpPost]
        public virtual IActionResult ChangePass(Web.Models.Customer.ChangePasswordModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();

            var customer = _workContext.CurrentCustomer;

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(customer.Email,
                    true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = _customerRegistrationService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    model.Result = _localizationService.GetResource("Account.ChangePassword.Success");
                    return Content("");

                }

                //errors
                string errs = "";
                foreach (var error in changePasswordResult.Errors)
                {
                    errs += error + "\n";
                }
                return Content(errs);
            }

            //If we got this far, something failed, redisplay form
            string errors = "";
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var item in allErrors)
            {
                errors += item.ErrorMessage + "\n";
            }
            return Content(errors);
        }

        //
        //public virtual IActionResult Avatar()
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        return Challenge();

        //    if (!_customerSettings.AllowCustomersToUploadAvatars)
        //        return RedirectToRoute("CustomerInfo");

        //    var model = new Web.Models.Customer.CustomerAvatarModel
        //    {
        //        AvatarUrl = _pictureService.GetPictureUrl(
        //            _workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
        //            _mediaSettings.AvatarPictureSize,
        //            false)
        //    };

        //    return View(model);
        //}

        [HttpPost]
        public virtual IActionResult Avatar()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var customer = _workContext.CurrentCustomer;

            var files = Request.Form.Files;
            try
            {
                var customerAvatar = _pictureService.GetPictureById(customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
                if (files != null && files.Count() > 0 && !string.IsNullOrEmpty(files.First().FileName))
                {
                    var avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
                    if (files.First().Length > avatarMaxSize)
                        throw new NopException(string.Format(_localizationService.GetResource("Account.Avatar.MaximumUploadedFileSize"), avatarMaxSize));

                    var customerPictureBinary = files.First().GetPictureBits();
                    if (customerAvatar != null)
                        customerAvatar = _pictureService.UpdatePicture(customerAvatar.Id, customerPictureBinary, files.First().ContentType, null);
                    else
                        customerAvatar = _pictureService.InsertPicture(customerPictureBinary, files.First().ContentType, null);
                }

                var customerAvatarId = 0;
                if (customerAvatar != null)
                    customerAvatarId = customerAvatar.Id;

                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AvatarPictureId, customerAvatarId);

                var url = _pictureService.GetPictureUrl(
                    customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                    _mediaSettings.AvatarPictureSize,
                    false);

                #region ADD to table Avatar for Check
                var tempRequest = _repositoryTbl_CheckAvatarCustomer.Table.Where(p => p.CustomerId == _workContext.CurrentCustomer.Id
                 && p.StateVerify == 0).ToList();
                if (tempRequest.Any())
                {
                    foreach (var item in tempRequest)
                    {
                        item.StateVerify = 1;
                        item.DateVerify = DateTime.Now;
                        item.IdUserVerify = _workContext.CurrentCustomer.Id;
                        _repositoryTbl_CheckAvatarCustomer.Update(item);
                    }

                }
                Tbl_CheckAvatarCustomer Temp = new Tbl_CheckAvatarCustomer();
                Temp.CustomerAvatarId = customerAvatarId;
                Temp.CustomerId = customer.Id;
                Temp.DateInsert = DateTime.Now;
                Temp.StateVerify = 0;
                _repositoryTbl_CheckAvatarCustomer.Insert(Temp);
                #endregion
                #region Ticket
                #region  new ticket
                var currentTicket = _repositoryTbl_Ticket.Table.Where(p => p.IdCustomer == _workContext.CurrentCustomer.Id
                      && p.IdCategoryTicket == 2 && p.OrderCode == 0 && p.Issue.Contains("آواتار")).OrderByDescending(n => n.Id).FirstOrDefault();
                if (currentTicket != null && currentTicket.Status != 3)
                {
                    currentTicket.Status = 3;
                    currentTicket.DateUpdate = DateTime.Now;
                    currentTicket.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Ticket.Update(currentTicket);
                }
                Ticket.Domain.Tbl_Ticket newticket = new Ticket.Domain.Tbl_Ticket();
                newticket.DateInsert = DateTime.Now;
                newticket.DepartmentId = 2;
                newticket.IdCategoryTicket = 2;
                newticket.ProrityId = 4;
                newticket.IdCustomer = _workContext.CurrentCustomer.Id;
                newticket.IsActive = true;
                newticket.Issue = "درخواست بررسی آواتار";
                newticket.OrderCode = 0;
                newticket.StoreId = _storeContext.CurrentStore.Id;
                newticket.TrackingCode = null;
                newticket.ShowCustomer = false;
                _repositoryTbl_Ticket.Insert(newticket);
                #endregion
                #region  add Deateil
                Ticket.Domain.Tbl_Ticket_Detail temp = new Ticket.Domain.Tbl_Ticket_Detail();
                temp.DateInsert = DateTime.Now;
                temp.Description = "درخواست بررسی آواتار " + "لینک نمایش درخواست   " + "http://postex.ir/Admin/AvatarCustomer/" + Temp.Id.ToString() + "    کاربر گرامی نیازی به پاسخ به این تیکت نیست،  درخواست را پیگیری بفرمایید، سامانه امنیتو";
                temp.IdTicket = newticket.Id;
                temp.Type = false;
                temp.UrlFile1 = null;
                temp.UrlFile2 = null;
                temp.UrlFile3 = null;
                _repositoryTbl_TicketDetail.Insert(temp);

                #endregion


                #endregion
                #region update id ticket in tbl avatar
                temp.IdTicket = newticket.Id;
                _repositoryTbl_CheckAvatarCustomer.Update(Temp);
                #endregion
                return Json(new { Success = true, Message = url });

            }
            catch (Exception exc)
            {
                return Json(new { Success = false, Message = exc.Message });
            }
        }

        public virtual IActionResult PostalAddress()
        {
            var model = new PostalAddress();
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/PostalAddress.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult AddPostalAddress(PostalAddress model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Json(new { success = false, error = "ابتدا وارد سیستم شوید" });

            var customer = _workContext.CurrentCustomer;

            if (ModelState.IsValid)
            {
                var address = new Address
                {
                    Address1 = model.Address1,
                    CountryId = model.CountryId,
                    StateProvinceId = model.StateProvinceId,
                    PhoneNumber = model.PhoneNumber,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Company = model.Company,
                    Email = model.Email
                };
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                var _NewAddress = _newCheckout.ProcessAddress(address, _workContext.CurrentCustomer, "billingAddress");
                if (_NewAddress.Id > 0)
                {
                    if (model.Lat.HasValue && model.Lon.HasValue)
                    {
                        _extendedShipmentService.InsertAddressLocation(address.Id, model.Lat.Value, model.Lon.Value);
                    }
                    return Json(new { success = true, error = "" });
                }
                return Json(new { success = false, error = "خطا در زمان ثبت آدرس" });
            }

            //If we got this far, something failed, redisplay form
            string errors = "";
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var item in allErrors)
            {
                errors += item.ErrorMessage + "\n";
            }
            return Json(new { success = false, error = errors });
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpGet]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel
            {
            };

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Configure.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost]//, AdminAuthorize, ChildActionOnly]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            //save settings
            //_phoneLoginSettings.Enabled = model.Enabled;
            //_phoneLoginSettings.LineNumber = model.LineNumber;
            //_phoneLoginSettings.ApiKey = model.ApiKey;
            // _settingService.SaveSetting(_phoneLoginSettings);

            // SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            // _settingService.ClearCache();

            return Configure();
        }

        public virtual IActionResult support()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/support.cshtml");
        }

        public virtual IActionResult supportPaged(int pageIndex = 0, int pageSize = 200)
        {
            var supports = _repositoryTbl_Ticket.Table.Where(p => p.IdCustomer == _workContext.CurrentCustomer.Id && p.ShowCustomer == true).OrderByDescending(p => p.Id).ToList();

            var SupportModel = supports.Select(x => new SupportModel
            {
                SupportId = x.Id,
                Issue = x.Issue,
                Date = x.DateInsert.ToString(),
                Status = x.Status == 0 ? "در صف انتظار" : x.Status == 1 ? "در حال بررسی" : x.Status == 2 ? "پاسخ داده شده" : x.Status == 2 ? "پایان یافته" : "",
                NameDep = _repositoryTbl_Department.GetById(x.DepartmentId).Name,
                OrderId = x.OrderCode.ToString(),
                TrackingCode = x.TrackingCode != null ? x.TrackingCode : "-"
            }).ToList();

            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.RowsCount = _orderServices.GetCustomerOrderCount(customerId: _workContext.CurrentCustomer.Id);

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/_supportPaged.cshtml", SupportModel);
        }

        public virtual IActionResult AddSupport()
        {
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/AddSupport.cshtml");
        }

        [HttpPost]
        public IActionResult CreateTicket(AddTicketModel param)//, HttpPostedFileBase[] _files
        {

            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
            #region check params

            if (param.DepartmentId == 0)
            {
                return Json(new { success = true, Status = 110 });

            }
            if (param.ProrityId == 0)
            {
                return Json(new { success = true, Status = 111 });
            }
            if (string.IsNullOrEmpty(param.Issue))
            {
                return Json(new { success = true, Status = 112 });
            }
            if (param.OrderCode > 0)
            {
                var order = _orderService.GetOrderById(param.OrderCode);
                if (order == null)
                {
                    return Json(new { success = true, Status = 113 });

                }
            }

            #endregion

            #region  new ticket
            Ticket.Domain.Tbl_Ticket newticket = new Ticket.Domain.Tbl_Ticket();
            newticket.DateInsert = DateTime.Now;
            newticket.DepartmentId = param.DepartmentId;
            newticket.IdCategoryTicket = param.IdCategoryTicket;
            newticket.ProrityId = param.ProrityId;
            newticket.IdCustomer = _workContext.CurrentCustomer.Id;
            newticket.IsActive = true;
            newticket.Issue = param.Issue;
            newticket.OrderCode = param.OrderCode;
            newticket.StoreId = _storeContext.CurrentStore.Id;
            newticket.TrackingCode = param.TrackingCode;
            newticket.ShowCustomer = true;
            newticket.TypeTicket = param.TypeTicket == 1 ? true : false;
            _repositoryTbl_Ticket.Insert(newticket);
            #endregion

            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "TicketFiles");
            var filename = "";
            List<string> listfilename = new List<string>();

            //======== new
            var files = Request.Form.Files;
            try
            {

                if (files != null && files.Count() > 0)
                {
                    foreach (var item in files)
                    {
                        if (item != null)
                        {
                            if (item.Length > 3145728)
                            {
                                TempData["error"] = "حجم فایل انتخاب شده بیش از 3 مگا بایت میباشد";
                                return Redirect("/Dashboard/AddRequestCOD");
                                //throw new NopException("حجم فایل انتخاب شده بیش از 3 مگا بایت میباشد");
                            }
                            else
                            {
                                var number = new Random();
                                string oldfilename = item.FileName;
                                string format = "";
                                var extension = Path.GetExtension(oldfilename).ToLower();
                                if (extension == ".jpg" || extension == ".png" || extension == ".jpeg" || extension == ".tiff" || extension == ".pdf")
                                {
                                    format = extension;
                                }
                                else
                                {
                                    TempData["error"] = "فرمت فایل ارسالی صحیح نمی باشد";
                                    return Redirect("/Dashboard/AddRequestCOD");
                                    //return BadRequest(new { message = "فرمت فایل ارسالی صحیح نمی باشد" });
                                }
                                filename = newticket.Id.ToString() + "_" + number.Next(1, 999999999).ToString() + format;
                                listfilename.Add(filename);
                                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\TicketFiles")))
                                {
                                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\TicketFiles"));
                                }
                                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\TicketFiles", filename);
                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    item.CopyTo(fileStream);
                                }
                            }
                        }
                    }


                }
            }
            catch
            {

            }
            //========
            //=================== old
            //foreach (HttpPostedFileBase file in _files)
            //{
            //    //Checking file is available to save.  
            //    if (file != null)
            //    {
            //        if (file.ContentLength > 3145728)
            //        {
            //            new System.Web.Mvc.HttpStatusCodeResult(532);
            //        }
            //        var number = new Random();
            //        filename = newticket.Id.ToString() + "_" + number.Next(1, 999999999).ToString();
            //        listfilename.Add(filename);
            //        var path = Path.Combine(uploads, filename);
            //        file.SaveAs(path);
            //    }
            //}
            //===========
            #region  add Deateil
            Ticket.Domain.Tbl_Ticket_Detail temp = new Ticket.Domain.Tbl_Ticket_Detail();
            temp.DateInsert = DateTime.Now;
            //temp.Description = Regex.Replace(param.Description, "<.*?>", String.Empty);
            temp.Description = AntiXssEncoder.HtmlEncode(param.Description, true);
            temp.IdTicket = newticket.Id;
            temp.Type = false;
            if (listfilename.Count >= 1)
            {
                temp.UrlFile1 = listfilename[0];
            }
            if (listfilename.Count >= 2)
            {
                temp.UrlFile2 = listfilename[1];
            }
            if (listfilename.Count >= 3)
            {
                temp.UrlFile3 = listfilename[2];
            }

            _repositoryTbl_TicketDetail.Insert(temp);

            #endregion

            return Json(new { success = true });

        }

        [HttpPost]
        public IActionResult AddDetailTicket(AddDetailTicket param)//, , HttpPostedFileBase[] _files
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
                                   //try
                                   //{
            Ticket.Domain.Tbl_Ticket ticket = _repositoryTbl_Ticket.GetById(param.Id);



            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "TicketFiles");
            var filename = "";
            List<string> listfilename = new List<string>();
            //======== new
            var files = Request.Form.Files;

            if (files != null && files.Count() > 0)
            {
                foreach (var item in files)
                {
                    if (item != null)
                    {
                        if (item.Length > 3145728)
                        {
                            TempData["error"] = "حجم فایل انتخاب شده بیش از 3 مگا بایت میباشد";
                            return Redirect("/Dashboard/AddRequestCOD");
                            //throw new NopException("حجم فایل انتخاب شده بیش از 3 مگا بایت میباشد");
                        }
                        else
                        {
                            var number = new Random();
                            string oldfilename = item.FileName;
                            string format = "";
                            var extension = Path.GetExtension(oldfilename).ToLower();
                            if (extension == ".jpg" || extension == ".png" || extension == ".jpeg" || extension == ".tiff" || extension == ".pdf")
                            {
                                format = extension;
                            }
                            else
                            {
                                TempData["error"] = "فرمت فایل ارسالی صحیح نمی باشد";
                                return Redirect("/Dashboard/AddRequestCOD");
                                //return BadRequest(new { message = "فرمت فایل ارسالی صحیح نمی باشد" });
                            }
                            filename = ticket.Id.ToString() + "_" + number.Next(1, 999999999).ToString() + format;
                            listfilename.Add(filename);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\TicketFiles", filename);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                item.CopyTo(fileStream);
                            }
                        }
                    }
                }


            }

            //========
            //foreach (HttpPostedFileBase file in _files)
            //{
            //    //Checking file is available to save.  
            //    if (file != null)
            //    {
            //        if (file.ContentLength > 3145728)
            //        {
            //            return Json(new { error = true, data = 532 });
            //        }
            //        var number = new Random();
            //        filename = ticket.Id.ToString() + "_" + number.Next(1, 999999999).ToString();
            //        listfilename.Add(filename);
            //        var path = Path.Combine(uploads, filename);
            //        file.SaveAs(path);
            //    }
            //}
            ////======================
            #region  add Deateil
            Ticket.Domain.Tbl_Ticket_Detail temp = new Ticket.Domain.Tbl_Ticket_Detail();
            temp.DateInsert = DateTime.Now;
            //temp.Description = Regex.Replace(param.Description, "<.*?>", String.Empty);
            temp.Description = AntiXssEncoder.HtmlEncode(param.Description, true);
            temp.IdTicket = ticket.Id;
            temp.Type = false;
            if (listfilename.Count >= 1)
            {
                temp.UrlFile1 = listfilename[0];
            }
            if (listfilename.Count >= 2)
            {
                temp.UrlFile2 = listfilename[1];
            }
            if (listfilename.Count >= 3)
            {
                temp.UrlFile3 = listfilename[2];
            }

            _repositoryTbl_TicketDetail.Insert(temp);

            #endregion

            #region update ticket
            ticket.Status = 0;
            _repositoryTbl_Ticket.Update(ticket);
            #endregion
            return Json(new { success = true });

            //}
            //catch
            //{
            //    return Json(new { error = true });
            //}



        }

        [HttpGet]
        public IActionResult GetListProirity()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
            var listProitity = _repositoryTbl_Priority.Table.Where(p => p.IsActive == true).Select(p => new
            {
                Value = p.Id,
                Text = p.Name
            }).ToList();
            return Json(listProitity);

        }
        [HttpGet]
        public IActionResult GetListDepartment()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
            var listDeps = _repositoryTbl_Department.Table.Where(p => p.IsActive == true).Select(p => new
            {
                Value = p.Id,
                Text = p.Name
            }).ToList();
            return Json(listDeps);

        }

        public virtual IActionResult DetailSupport(int id)
        {
            vmDetailTicket model = new vmDetailTicket();
            model.Tbl_Ticket = _repositoryTbl_Ticket.GetById(id);
            if (model.Tbl_Ticket == null)
            {
                return RedirectToAction("support");
            }
            var currentCustomer = _workContext.CurrentCustomer;
            if (model.Tbl_Ticket.IdCustomer != currentCustomer.Id)
            {
                return RedirectToAction("support");
            }
            model.NameCustomer = currentCustomer.GetFullName();
            model.NameCategory = _repositoryTbl_CategoryTicket.GetById(model.Tbl_Ticket.IdCategoryTicket).NameCategoryTicket;
            model.NameDep = _repositoryTbl_Department.GetById(model.Tbl_Ticket.DepartmentId).Name.ToString();
            model.Proirity = _repositoryTbl_Priority.GetById(model.Tbl_Ticket.ProrityId).Name.ToString();
            model.Status = model.Tbl_Ticket.Status == 0 ? "در صف انتظار" : model.Tbl_Ticket.Status == 1 ? "در حال بررسی" : model.Tbl_Ticket.Status == 2 ? "پاسخ داده شده" : "پایان یافته";
            List<Tbl_Ticket_Detail> lll = _repositoryTbl_TicketDetail.Table.Where(p => p.IdTicket == model.Tbl_Ticket.Id).OrderByDescending(p => p.Id).ToList();
            model.vmTicket_Detail = new List<vmTicket_Detail>();
            foreach (var item in lll)
            {
                vmTicket_Detail temp = new vmTicket_Detail();
                //item.Description = item.Description.Replace("\n", "<br>");
                item.Description = HttpUtility.HtmlDecode(item.Description);
                temp.NameStaff = item.StaffId != null ? _customerServices.GetCustomerById(item.StaffId.GetValueOrDefault()).GetFullName() : "";
                temp.List_Detail = item;
                model.vmTicket_Detail.Add(temp);

            }
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/DetailSupport.cshtml", model);
        }

        [HttpPost]
        public ActionResult ReadFilePdf(string url)
        {
            ViewBag.UrlFil = url;
            return PartialView("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/ReadFilePdf.cshtml");
        }

        public virtual IActionResult FAQ()
        {
            vm_FAQ model = new vm_FAQ();
            model.ListFAQ = new List<listFAQ>();
            List<Tbl_FAQCategory> tempcategory = _repositoryTbl_FAQCategory.Table.Where(p => p.IsActive == true).ToList();
            if (tempcategory.Count > 0)
            {
                foreach (var item in tempcategory)
                {
                    listFAQ x = new listFAQ();
                    x.NameCategory = item.Name;
                    x.Listfaq = _repositoryTbl_FAQ.Table.Where(p => p.IsActive == true && p.IdCategory == item.Id).ToList();
                    model.ListFAQ.Add(x);
                }
            }
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/ListFAQ.cshtml", model);
        }

        public IActionResult Services()
        {
            vm_Services model = new vm_Services();
            model.ListServices = new List<ItemService>();
            try
            {
                List<Tbl_Product_PatternPricing> tbl_productpp = _repositoryTbl_Product_PatternPricing.Table.Where(p => p.IsActive).GroupBy(p => p.IdProduct).Select(g => g.OrderBy(c => c.Id).FirstOrDefault()).ToList();
                if (tbl_productpp.Count > 0)
                {
                    foreach (var item in tbl_productpp)
                    {
                        ItemService temp = new ItemService();
                        var product = _productService.GetProductById(item.IdProduct);
                        temp.Price = product.Price;
                        temp.Name = product.Name;
                        temp.Discription = product.GetLocalized(x => x.ShortDescription, 1, false, false);
                        var defaultProductPicture = _pictureService.GetPicturesByProductId(item.IdProduct, 1).FirstOrDefault();
                        temp.UrlImage = _pictureService.GetPictureUrl(defaultProductPicture, 75, true);
                        temp.UrlPage = product.GetSeName();
                        temp.UrlPardukht = "#";
                        model.ListServices.Add(temp);
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Services.cshtml", model);
        }

        public IActionResult GetStateViewVideoCustomer()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
            var Customer = _repositoryTbl_ViewVideoCustomer.Table.Where(p => p.IsActive == true && p.CustomerId == _workContext.CurrentCustomer.Id).FirstOrDefault();
            if (Customer != null)
            {
                return Json(new { success = true, state = 1 });
            }
            else
            {
                return Json(new { success = true, state = 0 });

            }

        }


        [HttpPost]
        public IActionResult GetCategoryTicket(int id)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
            var listCats = _repositoryTbl_CategoryTicket.Table.Where(p => p.IsActive == true && p.DepartmentId == id).Select(p => new
            {
                Value = p.Id,
                Text = p.NameCategoryTicket
            }).ToList();
            return Json(listCats);

        }

        [HttpPost]
        public ActionResult ViewVideo()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
            var Customer = _repositoryTbl_ViewVideoCustomer.Table.Where(p => p.CustomerId == _workContext.CurrentCustomer.Id).FirstOrDefault();
            if (Customer != null)
            {
                if (Customer.IsActive == false)
                {
                    //update
                    Customer.DateView = DateTime.Now;
                    Customer.IsActive = true;
                    _repositoryTbl_ViewVideoCustomer.Update(Customer);
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = true });
                }
            }
            else
            {
                //new 
                Tbl_ViewVideoCustomer temp = new Tbl_ViewVideoCustomer();
                temp.CustomerId = _workContext.CurrentCustomer.Id;
                temp.IsActive = true;
                temp.DateView = DateTime.Now;
                temp.IPCustomer = _workContext.CurrentCustomer.LastIpAddress;
                _repositoryTbl_ViewVideoCustomer.Insert(temp);
                return Json(new { success = true });

            }
        }

        public virtual IActionResult Customersubset()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Customersubset.cshtml");
        }

        public virtual IActionResult CustomersubsetPaged(int pageIndex, int pageSize)
        {
            List<CustomerSubsetModel> SupportModel = new List<CustomerSubsetModel>();
            try
            {
                var aff = _repositoryTbl_AffiliateToCustomer.Table.Where(p => p.CustomerId == _workContext.CurrentCustomer.Id).FirstOrDefault();
                if (aff != null)
                {
                    List<Customer> customers = _customerServices.GetAllCustomers(affiliateId: aff.AffiliateId).ToList();
                    if (customers.Count > 0)
                    {
                        foreach (var item in customers)
                        {
                            CustomerSubsetModel q = new CustomerSubsetModel();
                            q.fullname = item.GetFullName();
                            q.mobile = item.Username;
                            q.DateRegister = item.CreatedOnUtc.ToString();
                            q.Total = "0";
                            SupportModel.Add(q);
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.RowsCount = SupportModel.Count;

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/_CustomersubsetPaged.cshtml", SupportModel);
        }

        #region Damages

        public virtual IActionResult Damages()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Damages.cshtml");
        }

        public virtual IActionResult DamagesPaged(int pageIndex, int pageSize)
        {
            var Damages = _repositoryTbl_Damages.Table.Where(p => p.IdCustomer == _workContext.CurrentCustomer.Id).OrderByDescending(p => p.Id).ToList();

            var DamagesModel = Damages.Select(x => new DamagesModel
            {
                DamagesId = x.Id,
                Date = x.DateInsert.ToString(),
                Status = x.Status == 0 ? "در صف انتظار" : x.Status == 1 ? "در حال بررسی" : x.Status == 2 ? "عدم رعایت قانون 24" : x.Status == 3 ? "عدم تایید و پایان" : x.Status == 4 ? " تایید،پرداخت غرامت و پایان" : "-",
                TrackingCode = x.TrackingCode != null ? x.TrackingCode : "-",
                NameGoods = x.NameGoods,
                Berand = x.Berand,
                Price = x.Price.ToString()
            }).ToList();

            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.RowsCount = _orderServices.GetCustomerOrderCount(customerId: _workContext.CurrentCustomer.Id);

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/_DamagesPaged.cshtml", DamagesModel);
        }

        [HttpPost]
        public IActionResult CreateDamages(AddDamagesModel param)
        {

            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login

            #region checkparam
            var shipment = _shipmentService.GetAllShipments(trackingNumber: param.TrackingCode).FirstOrDefault();

            if (shipment == null)
            {
                return Json(new { error = true, Status = 110 });

            }
            var order = _orderService.GetOrderById(shipment.OrderId);
            if (order.CustomerId != _workContext.CurrentCustomer.Id)
            {
                return Json(new { error = true, Status = 111 });
            }
            #endregion

            #region  new Damages
            Ticket.Domain.Tbl_Damages newDamages = new Ticket.Domain.Tbl_Damages();
            newDamages.Price = param.Price;
            newDamages.NameGoods = param.NameGoods;
            newDamages.Berand = param.NameBerand;
            newDamages.Shaba = param.Shaba;
            newDamages.IdCustomer = _workContext.CurrentCustomer.Id;
            newDamages.IsActive = true;
            newDamages.StoreId = _storeContext.CurrentStore.Id;
            newDamages.DateInsert = DateTime.Now;
            newDamages.TrackingCode = param.TrackingCode;
            newDamages.Stock = param.type;
            newDamages.Status = 0;
            _repositoryTbl_Damages.Insert(newDamages);
            #endregion




            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "DamagesFiles");
            var filename = "";
            List<string> listfilename = new List<string>();
            var files = Request.Form.Files;
            try
            {

                if (files != null && files.Count() > 0)
                {
                    foreach (var item in files)
                    {
                        if (item != null)
                        {
                            if (files.First().Length > 3145728)
                            {
                                TempData["error"] = "حجم فایل انتخاب شده بیش از 3 مگا بایت میباشد";
                                return Redirect("/Dashboard/AddRequestCOD");
                                //throw new NopException("حجم فایل انتخاب شده بیش از 3 مگا بایت میباشد");
                            }
                            else
                            {
                                var number = new Random();
                                string oldfilename = item.FileName;
                                string format = "";
                                var extension = Path.GetExtension(oldfilename).ToLower();
                                if (extension == ".jpg" || extension == ".png" || extension == ".jpeg" || extension == ".tiff" || extension == ".pdf")
                                {
                                    format = extension;
                                }
                                else
                                {
                                    TempData["error"] = "فرمت فایل ارسالی صحیح نمی باشد";
                                    return Redirect("/Dashboard/AddRequestCOD");
                                    //return BadRequest(new { message = "فرمت فایل ارسالی صحیح نمی باشد" });
                                }
                                filename = newDamages.Id.ToString() + "_" + number.Next(1, 999999999).ToString() + format;
                                listfilename.Add(filename);
                                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\DamagesFiles")))
                                {
                                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\DamagesFiles"));
                                }
                                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\DamagesFiles", filename);
                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    item.CopyTo(fileStream);
                                }
                            }
                        }
                    }


                }
            }
            catch
            {

            }
            #region  add Deateil
            Ticket.Domain.Tbl_Damages_Detail temp = new Ticket.Domain.Tbl_Damages_Detail();
            temp.DateInsert = DateTime.Now;
            //temp.Description = Regex.Replace(param.Description, "<.*?>", String.Empty); ;
            temp.Description = AntiXssEncoder.HtmlEncode(param.Description, true);
            temp.IdDamages = newDamages.Id;
            temp.Type = false;
            if (listfilename.Count >= 1)
            {
                temp.UrlFile1 = listfilename[0];
            }
            if (listfilename.Count >= 2)
            {
                temp.UrlFile2 = listfilename[1];
            }
            if (listfilename.Count >= 3)
            {
                temp.UrlFile3 = listfilename[2];
            }
            if (listfilename.Count >= 4)
            {
                temp.UrlFile4 = listfilename[3];
            }
            if (listfilename.Count >= 5)
            {
                temp.UrlFileCardMeli = listfilename[4];
            }
            _repositoryTbl_DamagesDetail.Insert(temp);

            #endregion

            #region check delivery
            if (shipment.DeliveryDateUtc != null)
            {
                DateTime t = DateTime.Now.ToUniversalTime();
                TimeSpan span = t.Subtract(shipment.DeliveryDateUtc.GetValueOrDefault());
                string msg = "";
                if ((span.Days == 1 && span.Hours > 0) || (span.Days > 1))
                {
                    // قانون 24
                    newDamages.Status = 3;
                    _repositoryTbl_Damages.Update(newDamages);
                    //ارسال پیامک به مشتری
                    msg = "کاربر گرامی ، درخواست خسارت به شماره: " + newDamages.Id.ToString() + " قانون 24 را رعایت نکرده و مورد قبول نیست، سامانه امنیتو";

                }
                else
                {
                    msg = "کاربر گرامی ، درخواست خسارت به شماره: " + newDamages.Id.ToString() + "با موفقیت ثبت شد و توسط کارشناسان در حال بررسی میباشد، با تشکر سامانه امنیتو";
                }
                var sended = _notificationService._sendSms(_workContext.CurrentCustomer.Username, msg);
                #region log sms
                Tbl_LogSMS log1 = new Tbl_LogSMS();
                log1.Type = 3;
                log1.Mobile = _workContext.CurrentCustomer.Username;
                log1.StoreId = _storeContext.CurrentStore.Id;
                log1.TextMessage = msg;
                log1.Status = sended == true ? 1 : 0;
                log1.DateSend = DateTime.Now;
                _repositoryTbl_LogSMS.Insert(log1);
                #endregion
            }
            else
            {
                ///چک کردن سایر api ها
                int CategoryId = GetIdCategoryByTrackingNumber(param.TrackingCode);
                if (CategoryId > 0)
                {
                    string er;
                    try
                    {
                        var result = _ShipmentTrackingService.getLastShipmentTracking(trackingNumber: param.TrackingCode, orderId: 0, mobileNo: "", CustomerId: 0, IdServiceProvider: CategoryId, strError: out er);
                        if (result.Count > 0)
                        {
                            #region if

                            if (_extendedShipmentService.IsPostService(CategoryId))
                            {
                                //post
                                if (result.FirstOrDefault().LastSate.Contains("مرسوله تحویل گیرنده گردیده است"))
                                {

                                }
                            }
                            else if ((new int[] { 703, 699, 705, 706 }).Contains(CategoryId))
                            {
                                //dts
                                if (result.FirstOrDefault().LastSate.Contains("OK (Consignment delivered in good condition.)"))
                                {

                                }
                            }
                            else if (CategoryId == 708)
                            {
                                //pde demestic
                            }
                            else if (CategoryId == 707)// PDE INTERNATIONAL)
                            {
                            }
                            else if (new int[] { 709, 710 }.Contains(CategoryId)) // TPG
                            {
                                //
                                if (result.FirstOrDefault().LastSate.Contains("تحویل شده"))
                                {

                                }
                            }
                            else if (CategoryId == 701 && CategoryId >= 100000) // اوبار
                            {

                                if (result.FirstOrDefault().LastSate.Contains("delivered"))
                                {

                                }
                            }
                            else if (CategoryId == 702) // یارباکس
                            {
                            }
                            else if ((new int[] { 712, 713, 714, 715 }).Contains(CategoryId))//چاپار
                            {
                            }
                            else if (CategoryId == 719)
                            {
                                //chapar

                                if (result.FirstOrDefault().LastSate.Contains("OK (Consignment delivered in good condition.)"))
                                {

                                }
                            }
                            else if (CategoryId == 717)
                            {
                                //snappbox
                                //
                                if (result.FirstOrDefault().LastSate.Contains("DELIVERED"))
                                {

                                }
                            }
                            #endregion
                        }
                        else
                        {

                        }
                    }
                    catch
                    {
                        return Json(new { error = true, Status = 111 });
                    }


                }


            }
            #endregion
            #region sendsmstostaff
            int userId = _userStatesService.GetIdUser(shipment.Order.BillingAddress.StateProvinceId.GetValueOrDefault());
            if (userId > 0)
            {
                var staff = _customerServices.GetCustomerById(userId);
                string msg1 = "کاربر گرامی ، درخواست خسارت به شماره: " + newDamages.Id.ToString() + " در سامانه برای شما ثبت شده است، لطفا پیگیری بفرمایید";
                var sended1 = _notificationService._sendSms(_workContext.CurrentCustomer.Username, msg1);
                #region log sms
                Tbl_LogSMS log = new Tbl_LogSMS();
                log.Type = 4;
                log.Mobile = _workContext.CurrentCustomer.Username;
                log.StoreId = _storeContext.CurrentStore.Id;
                log.TextMessage = msg1;
                log.Status = sended1 == true ? 1 : 0;
                log.DateSend = DateTime.Now;
                _repositoryTbl_LogSMS.Insert(log);
                #endregion
            }


            #endregion
            #region set staff to Damages
            newDamages.StaffIdAccept = userId;
            if (newDamages.Status != 3)
            {

                newDamages.Status = 1;
            }
            _repositoryTbl_Damages.Update(newDamages);
            #endregion
            #region Ticket
            #region  new ticket
            Ticket.Domain.Tbl_Ticket newticket = new Ticket.Domain.Tbl_Ticket();
            newticket.DateInsert = DateTime.Now;
            newticket.DepartmentId = 2;
            newticket.IdCategoryTicket = 2;
            newticket.ProrityId = 4;
            newticket.IdCustomer = _workContext.CurrentCustomer.Id;
            newticket.IsActive = true;
            newticket.Issue = "درخواست بررسی خسارت";
            newticket.OrderCode = 0;
            newticket.StoreId = _storeContext.CurrentStore.Id;
            newticket.TrackingCode = null;
            newticket.ShowCustomer = false;
            _repositoryTbl_Ticket.Insert(newticket);
            #endregion
            #region  add Deateil
            Ticket.Domain.Tbl_Ticket_Detail temp1 = new Ticket.Domain.Tbl_Ticket_Detail();
            temp1.DateInsert = DateTime.Now;
            temp1.Description = "درخواست بررسی خسارت " + "لینک نمایش درخواست   " + "http://postex.ir/Admin/DamagesCustomer/" + newDamages.Id.ToString() + "    کاربر گرامی نیازی به پاسخ به این تیکت نیست،  درخواست را پیگیری بفرمایید، سامانه امنیتو";
            temp1.IdTicket = newticket.Id;
            temp1.Type = false;
            temp1.UrlFile1 = null;
            temp1.UrlFile2 = null;
            temp1.UrlFile3 = null;
            _repositoryTbl_TicketDetail.Insert(temp1);

            #endregion


            #endregion
            #region update id ticket in tbl damages
            newDamages.IdTicket = newticket.Id;
            _repositoryTbl_Damages.Update(newDamages);
            #endregion
            return Json(new { success = true });

        }

        public virtual IActionResult AddDamages()
        {
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/AddDamages.cshtml");
        }


        public virtual IActionResult DetailDamages(int id)
        {
            vmDetailDamages model = new vmDetailDamages();
            model.Tbl_Damages = _repositoryTbl_Damages.GetById(id);
            model.TypeGoods = model.Tbl_Damages.Stock == true ? "کالای دست دوم" : "کالای نو";
            model.NameCustomer = _workContext.CurrentCustomer.GetFullName();
            model.Status = model.Tbl_Damages.Status == 0 ? "در صف انتظار" : model.Tbl_Damages.Status == 1 ? "در حال بررسی" : model.Tbl_Damages.Status == 2 ? "پاسخ داده شده" : model.Tbl_Damages.Status == 3 ? "عدم رعایت قانون 24" : model.Tbl_Damages.Status == 4 ? "عدم تایید و پایان" : model.Tbl_Damages.Status == 5 ? " تایید،پرداخت غرامت و پایان" : "-";
            List<Tbl_Damages_Detail> lll = _repositoryTbl_DamagesDetail.Table.Where(p => p.IdDamages == model.Tbl_Damages.Id).OrderByDescending(p => p.Id).ToList();
            model.vmDamages_Detail = new List<vmDamages_Detail>();
            foreach (var item in lll)
            {
                vmDamages_Detail temp = new vmDamages_Detail();
                temp.NameStaff = item.StaffId != null ? _customerServices.GetCustomerById(item.StaffId.GetValueOrDefault()).GetFullName() : "";
                temp.List_Detail = item;
                model.vmDamages_Detail.Add(temp);

            }
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/DetailDamages.cshtml", model);
        }


        [HttpPost]
        public IActionResult AddDetailDamages(AddDetailDamages param)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
            try
            {
                Ticket.Domain.Tbl_Damages Damages = _repositoryTbl_Damages.GetById(param.Id);

                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "DamagesFiles");
                var filename = "";
                List<string> listfilename = new List<string>();
                //======== new
                var files = Request.Form.Files;
                try
                {

                    if (files != null && files.Count() > 0)
                    {
                        foreach (var item in files)
                        {
                            if (item != null)
                            {
                                if (files.First().Length > 3145728)
                                {
                                    TempData["error"] = "حجم فایل انتخاب شده بیش از 3 مگا بایت میباشد";
                                    return Redirect("/Dashboard/AddRequestCOD");
                                    //throw new NopException("حجم فایل انتخاب شده بیش از 3 مگا بایت میباشد");
                                }
                                else
                                {
                                    var number = new Random();
                                    string oldfilename = item.FileName;
                                    string format = "";
                                    var extension = Path.GetExtension(oldfilename).ToLower();
                                    if (extension == ".jpg" || extension == ".png" || extension == ".jpeg" || extension == ".tiff" || extension == ".pdf")
                                    {
                                        format = extension;
                                    }
                                    else
                                    {
                                        TempData["error"] = "فرمت فایل ارسالی صحیح نمی باشد";
                                        return Redirect("/Dashboard/AddRequestCOD");
                                        //return BadRequest(new { message = "فرمت فایل ارسالی صحیح نمی باشد" });
                                    }
                                    filename = Damages.Id.ToString() + "_" + number.Next(1, 999999999).ToString() + format;
                                    listfilename.Add(filename);
                                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\DamagesFiles", filename);
                                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                                    {
                                        item.CopyTo(fileStream);
                                    }
                                }
                            }
                        }


                    }
                }
                catch
                {

                }

                #region  add Deateil
                Ticket.Domain.Tbl_Damages_Detail temp = new Ticket.Domain.Tbl_Damages_Detail();
                temp.DateInsert = DateTime.Now;
                temp.Description = AntiXssEncoder.HtmlEncode(param.Description, true);
                //Regex.Replace(param.Description, "<.*?>", String.Empty); ;
                temp.IdDamages = Damages.Id;
                temp.Type = false;
                if (listfilename.Count >= 1)
                {
                    temp.UrlFile1 = listfilename[0];
                }
                if (listfilename.Count >= 2)
                {
                    temp.UrlFile2 = listfilename[1];
                }
                if (listfilename.Count >= 3)
                {
                    temp.UrlFile3 = listfilename[2];
                }
                if (listfilename.Count >= 4)
                {
                    temp.UrlFile4 = listfilename[3];
                }
                if (listfilename.Count >= 5)
                {
                    temp.UrlFileCardMeli = listfilename[4];
                }
                _repositoryTbl_DamagesDetail.Insert(temp);

                #endregion

                #region update Damages
                Damages.Status = 0;
                _repositoryTbl_Damages.Update(Damages);
                #endregion

                #region send sms to staff
                if (Damages.StaffIdAccept > 0)
                {
                    int z = Damages.StaffIdAccept.GetValueOrDefault();
                    var staff = _customerServices.GetCustomerById(z);
                    string msg1 = "کاربر گرامی ، پاسخی در درخواست خسارت به شماره: " + Damages.Id.ToString() + " در سامانه برای شما ثبت شده است، لطفا پیگیری بفرمایید";
                    var sended1 = _notificationService._sendSms(staff.Username, msg1);
                    #region log sms
                    Tbl_LogSMS log = new Tbl_LogSMS();
                    log.Type = 4;
                    log.Mobile = _workContext.CurrentCustomer.Username;
                    log.StoreId = _storeContext.CurrentStore.Id;
                    log.TextMessage = msg1;
                    log.Status = sended1 == true ? 1 : 0;
                    log.DateSend = DateTime.Now;
                    _repositoryTbl_LogSMS.Insert(log);
                    #endregion
                }

                #endregion
                return Json(new { success = true });

            }
            catch
            {
                return Json(new { error = true });
            }



        }
        #endregion


        public int GetIdCategoryByTrackingNumber(string trackingnumber)
        {
            string query = $@"SELECT
	                                PCM.CategoryId
                                 FROM
                                 	dbo.[Order] AS O
                                 	INNER JOIN dbo.Shipment AS S ON S.OrderId = O.Id
                                 	INNER JOIN dbo.ShipmentItem AS SI ON SI.ShipmentId = S.Id
                                 	INNER JOIN dbo.OrderItem AS OI ON OI.OrderId = O.Id
                                 	INNER JOIN dbo.Product AS P ON P.Id = OI.ProductId
                                 	INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.ProductId = P.Id
                                 	INNER JOIN dbo.Category AS C ON C.Id = PCM.CategoryId
                                 WHERE
                                 	S.TrackingNumber=@TrackingNumber";
            SqlParameter P_CategoryId = new SqlParameter()
            {
                ParameterName = "TrackingNumber",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)trackingnumber
            };
            SqlParameter[] prms = new SqlParameter[]{
                P_CategoryId
            };
            return _dbContext.SqlQuery<int>(query, prms).FirstOrDefault();
        }



        #region RequestCOD
        public virtual IActionResult AddRequestCOD()
        {
            //تست
            //_cartonPriceService.CalculateSize9Price(55, 55, 55);
            var PaymentMethod = _newCheckout.GetPaymentMethodsForCODRequest();

            if (_codRequestService.CustomerHasActiveCOD(_workContext.CurrentCustomer.Id))
            {
                TempData["error"] = "حساب کاربری پس کرایه شما در حال حاضر فعال می باشد";
            }

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/AddRequestCOD.cshtml", PaymentMethod);
        }
        public virtual IActionResult RequestCOD()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/RequestCOD.cshtml");
        }
        public virtual IActionResult RequestCODPaged(int pageIndex, int pageSize)
        {
            var RequestCOD = _repositoryTbl_RequestCODCustomer.Table.Where(p => p.IdCustomer == _workContext.CurrentCustomer.Id).OrderByDescending(p => p.Id).ToList();

            var RequestCODModel = RequestCOD.Select(x => new RequestCODModel
            {
                RequestCODId = x.Id,
                Date = x.DateInsert.ToString(),
                Status = x.Status == 0 ? "در صف انتظار" : x.Status == 1 ? "در حال بررسی" : x.Status == 2 ? "عدم تایید" : x.Status == 3 ? "تایید" : "-",
                Fname = x.Fname,
                Lname = x.Lname,
                NatinolCode = x.NatinolCode,
                BusinessType = x.BusinessType,
                Address = x.Address,
                Shaba = x.Shaba,
                UrlFile = x.UrlFile,
                UserName = x.Username,
                CodePosti = x.CodePosti
            }).ToList();

            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.RowsCount = _orderServices.GetCustomerOrderCount(customerId: _workContext.CurrentCustomer.Id);

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/_RequestCODPaged.cshtml", RequestCODModel);
        }
        [HttpPost]
        public IActionResult CreateRequestCOD(AddRequestCODModel param)
        {
            try
            {
                if (!_workContext.CurrentCustomer.IsRegistered())
                    return Challenge();//login
                List<string> erorrs = new List<string>();
                //if (_codRequestService.CustomerHasActiveCOD(_workContext.CurrentCustomer.Id))
                //{
                //    TempData["error"] = "حساب کاربری پس کرایه شما در حال حاضر فعال می باشد";
                //    return Redirect("/Dashboard#list-flatbed");
                //}
                var userName = _workContext.CurrentCustomer.Username;
                string country, state;

                #region Validate Data
                string error;
                var ReciverCountryCOde = _repositoryCountryCode.Table.FirstOrDefault(p => p.CountryId == param.Country);
                if (ReciverCountryCOde == null)
                {
                    error = "کد استان مربوط به  " + $"{ReciverCountryCOde}" + " یافت نشد";
                    erorrs.Add(error);
                    Log(error, "");
                }
                if (string.IsNullOrEmpty(ReciverCountryCOde.CountryCode))
                {
                    error = "کد استان مربوط به  " + $"{ReciverCountryCOde.CountryCode}" + " یافت نشد";
                    erorrs.Add(error);
                    Log(error, "");
                }
                var RecivercityCode = _repositoryStateCode.Table.FirstOrDefault(p => p.stateId == param.State);
                if (RecivercityCode == null)
                {
                    error = "کد شهر مربوط به  " + $"{RecivercityCode}" + " یافت نشد";
                    erorrs.Add(error);
                    Log(error, "");
                }
                if (string.IsNullOrEmpty(RecivercityCode.StateCode))
                {
                    error = "کد شهر مربوط به  " + $"{RecivercityCode.StateCode}" + " یافت نشد";
                    erorrs.Add(error);
                    Log(error, "");
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
                var _date = DateTime.Now.AddDays(-365);
                var tempRequest = _repositoryTbl_RequestCODCustomer.Table.Where(p => p.IdCustomer == _workContext.CurrentCustomer.Id
                    && p.DateInsert >= _date).ToList();
                if (tempRequest.Any())
                {
                    int LastStatus = tempRequest.OrderByDescending(p => p.DateInsert).First().Status;
                    int _reqId = tempRequest.OrderByDescending(p => p.DateInsert).First().Id;
                    if (LastStatus != 2)
                    {
                        string StatusText = LastStatus == 0 ? "درصف انتظار" : (LastStatus == 1 ? "در حال بررسی" : (LastStatus == 3 ? "تایید شده" : ""));
                        error = $"شما قبلا درخواستی را ثبت کردید که وضعیت آن {StatusText} می باشد." + " شماره درخواست: " + _reqId;
                        erorrs.Add(error);
                    }
                }
                if (erorrs.Any())
                {
                    TempData["error"] = string.Join(Environment.NewLine, erorrs);
                    return Redirect("/Dashboard#list-flatbed");
                }
                Ticket.Domain.Tbl_RequestCODCustomer newRequestCOD = null;
                newRequestCOD = new Ticket.Domain.Tbl_RequestCODCustomer();
                newRequestCOD.Fname = param.Fname;
                newRequestCOD.Lname = param.Lname;
                newRequestCOD.NatinolCode = param.NationalCode.ToEnDigitString();
                newRequestCOD.ManagerNationalIDSerial = param.ManagerNationalIDSerial;
                newRequestCOD.ManagerBirthDate = param.ManagerBirthDate;
                newRequestCOD.Email = param.Email;
                newRequestCOD.AccountNumber = param.AccountNumber.ToEnDigitString();
                newRequestCOD.AccountBranchName = param.AccountBranchName;

                newRequestCOD.Shaba = param.AccountIBAN;
                newRequestCOD.BusinessType = "";
                newRequestCOD.Address = param.Address;
                newRequestCOD.StateId = param.Country;
                newRequestCOD.CityId = param.State;

                newRequestCOD.IdCustomer = _workContext.CurrentCustomer.Id;
                newRequestCOD.IsActive = true;
                newRequestCOD.StoreId = _storeContext.CurrentStore.Id;
                newRequestCOD.DateInsert = DateTime.Now;
                newRequestCOD.Status = 0;
                newRequestCOD.Username = userName;
                newRequestCOD.CodePosti = param.PostalCode.ToEnDigitString();
                newRequestCOD.Mobile = param.Mobile.ToEnDigitString();
                newRequestCOD.Phone = param.Phone.ToEnDigitString();
                _repositoryTbl_RequestCODCustomer.Insert(newRequestCOD);
                #endregion


                #region InsertOrder
                HttpContext.Session.Set("isFromApp", param.isFromApp);
                PlaceOrderResult result = _codRequestService.ProcessCODRequestOrder(newRequestCOD, param.PaymentMethod);
                if (result.Success)
                {
                    error = null;
                    var order = result.PlacedOrder;
                    newRequestCOD.OrderId = order.Id;
                    _repositoryTbl_RequestCODCustomer.Update(newRequestCOD);
                    //if (order.OrderStatus == OrderStatus.Cancelled)
                    //{
                    //    TempData["error"] = "• سفارش شما کنسل شده و امکان پرداخت برای آن وجود ندارد";
                    //    return Redirect("Dashboard/AddRequestCOD");
                    //}

                    //if (order.PaymentStatus == Core.Domain.Payments.PaymentStatus.Paid)
                    //{
                    //    TempData["error"] = "ثبت با موفقیت انجام شد";
                    //    return Redirect("Dashboard/AddRequestCOD");
                    //}
                    order.PaymentMethodSystemName = param.PaymentMethod;
                    _orderService.UpdateOrder(order);
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = order
                    };
                    _paymentService.PostProcessPayment(postProcessPaymentRequest);
                    if (param.isFromApp)
                    {
                        common.Log("ارسال به صفحه میانی اپ", HttpContext.Session.Get<string>("redirectUrl"));
                        return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/OpenExternalBrowser.cshtml", HttpContext.Session.Get<string>("redirectUrl"));
                    }
                    if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                    {
                        return Content("Redirected");
                    }

                    return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                }
                else
                {
                    TempData["error"] = string.Join(Environment.NewLine, result.Errors);
                    return Redirect("/Dashboard#list-flatbed");
                }


                #endregion


            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { success = false });
            }
        }
        #endregion

        public virtual IActionResult CustomerConfirmOrderUbaar(int OrderItem)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();
            var orderitem = _orderService.GetOrderItemById(OrderItem);
            Order Current_Order = _orderService.GetOrderById(orderitem.OrderId);
            var model = new vm_customerconfirmorder();
            model.NameCustomer = _workContext.CurrentCustomer.GetFullName();
            model.OrderId = Current_Order.Id.ToString();
            model.Price = Current_Order.OrderTotal.ToString();
            _trackingUbaarOrder_Service.Insert(Current_Order.Id, orderitem.Id, 2, "مورد تایید مشتری", 0, 0);
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/CustomerConfirmOrderUbaar.cshtml", model);
        }

        public void Log(string header, string Message)
        {
            //var logger = (DefaultLogger)EngineContext.Current.Resolve<ILogger>();
            //logger.InsertLog(LogLevel.Information, header, Message, null);
        }

        public IActionResult GetOrdersSum()
        {
            var data = _orderServices.GetOrdersSumByCustomer(_workContext.CurrentCustomer.Id);
            return Ok(new
            {
                OrderTotal = data.OrderTotal.HasValue ? data.OrderTotal.Value.ToString("#,###") : "0" + " ریال",
                DiscountSum = data.DiscountSum.HasValue ? data.DiscountSum.Value.ToString("#,###") : "0" + " ریال"
            });
        }


        public IActionResult CustomerSetting()
        {
            CustomerSetting c = new CustomerSetting();
            c.ShowDiscountOnPrintPdf = _workContext.CurrentCustomer.GetAttribute<bool>(nameof(c.ShowDiscountOnPrintPdf));
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/CustomerSetting.cshtml", c);
        }

        [HttpPost]
        public IActionResult CustomerSetting([FromForm] CustomerSetting customerSetting)
        {
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, nameof(customerSetting.ShowDiscountOnPrintPdf), customerSetting.ShowDiscountOnPrintPdf);
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/CustomerSetting.cshtml", customerSetting);
        }

    }
}

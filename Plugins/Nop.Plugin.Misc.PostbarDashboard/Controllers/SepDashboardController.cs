using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Plugin.Misc.PostbarDashboard.Services;


using Nop.Plugin.Misc.ShippingSolutions.Domain;
using Nop.Plugin.Misc.Ticket.Domain;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Web.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nop.Plugin.Misc.PostbarDashboard.Controllers
{
    public partial class SepDashboardController : BasePublicController
    {
        #region Fields
        private readonly IRepository<Tbl_CheckAvatarCustomer> _repositoryTbl_CheckAvatarCustomer;
        private readonly IRepository<Tbl_ViewVideoCustomer> _repositoryTbl_ViewVideoCustomer;
        private readonly IRepository<Tbl_CategoryTicket> _repositoryTbl_CategoryTicket;
        private readonly IRepository<Tbl_Product_PatternPricing> _repositoryTbl_Product_PatternPricing;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IRepository<Ticket.Domain.Tbl_RequestCODCustomer> _repositoryTbl_RequestCODCustomer;

        private readonly ILanguageService _languageService;
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
        private readonly IProductService _productService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IRepository<Tbl_CustomerDepositCode> _repository_TblCustomerDepositCode;
        private readonly ICODRequestService _codRequestService;
        private readonly IRepository<Nop.plugin.Orders.ExtendedShipment.Models.CountryCodeModel> _repositoryCountryCode;
        private readonly IRepository<Nop.plugin.Orders.ExtendedShipment.Models.StateCodemodel> _repositoryStateCode;
        private readonly IWebHelper _webHelper;
        #endregion

        #region Ctor

        public SepDashboardController(
            IDateTimeHelper dateTimeHelper,
            IOrderProcessingService orderProcessingService,
            ILanguageService languageService,
            IProductService productService,
            IRepository<Tbl_Product_PatternPricing> repositoryTbl_Product_PatternPricing,
            IRepository<Tbl_CategoryTicket> repositoryTbl_CategoryTicket,
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
            IWebHelper webHelper,
            IRepository<Tbl_CheckAvatarCustomer> repositoryTbl_CheckAvatarCustomer,
            IRepository<Tbl_ViewVideoCustomer> repositoryTbl_ViewVideoCustomer,
            IRepository<Ticket.Domain.Tbl_Ticket> repositoryTbl_Ticket,
            IRepository<Ticket.Domain.Tbl_Ticket_Detail> repositoryTbl_TicketDetail,
            IRepository<Ticket.Domain.Tbl_Ticket_Department> repositoryTbl_Department,
            IRepository<Ticket.Domain.Tbl_Ticket_Priority> repositoryTbl_Priority,
            IRepository<Ticket.Domain.Tbl_FAQ> repositoryTbl_FAQ,
            IRepository<Nop.plugin.Orders.ExtendedShipment.Models.StateCodemodel> repositoryStateCode,
            IRepository<Nop.plugin.Orders.ExtendedShipment.Models.CountryCodeModel> repositoryCountryCode,
            IRepository<Ticket.Domain.Tbl_FAQCategory> repositoryTbl_FAQCategory,
            IHostingEnvironment hostingEnvironment,
            IRepository<Tbl_CustomerDepositCode> repository_TblCustomerDepositCode,
            IRepository<Ticket.Domain.Tbl_RequestCODCustomer> repositoryTbl_RequestCODCustomer,
            ICODRequestService cODRequestService
            )
        {
            _dateTimeHelper = dateTimeHelper;
            _orderProcessingService = orderProcessingService;
            _languageService = languageService;
            _productService = productService;
            _repositoryTbl_Product_PatternPricing = repositoryTbl_Product_PatternPricing;
            _repositoryTbl_CategoryTicket = repositoryTbl_CategoryTicket;
            _newCheckout = newCheckout;
            this._logger = logger;
            this._customerServices = customerServices;
            this._orderService = orderService;
            _repositoryCountryCode = repositoryCountryCode;
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
            _repositoryTbl_RequestCODCustomer = repositoryTbl_RequestCODCustomer;
            _repositoryStateCode = repositoryStateCode;
            _codRequestService = cODRequestService;
            _repositoryTbl_CheckAvatarCustomer = repositoryTbl_CheckAvatarCustomer;
            _repositoryTbl_ViewVideoCustomer = repositoryTbl_ViewVideoCustomer;
            _repositoryTbl_Ticket = repositoryTbl_Ticket;
            _repositoryTbl_TicketDetail = repositoryTbl_TicketDetail;
            _repositoryTbl_Department = repositoryTbl_Department;
            _repositoryTbl_Priority = repositoryTbl_Priority;
            _repositoryTbl_FAQ = repositoryTbl_FAQ;
            _repositoryTbl_FAQCategory = repositoryTbl_FAQCategory;
            _hostingEnvironment = hostingEnvironment;
            _repository_TblCustomerDepositCode = repository_TblCustomerDepositCode;
            _webHelper = webHelper;
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

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/Dashboard.cshtml", model);
        }


        public virtual IActionResult Wallet()
        {
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/Wallet.cshtml");
        }


        public virtual IActionResult WalletPaged(int pageIndex, int pageSize)
        {
            var rewards = _rewardPointService.GetRewardPointsHistory(
                customerId: _workContext.CurrentCustomer.Id,
                pageIndex: pageIndex,
                pageSize: pageSize).ToList();
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.RowsCount = _rewardPointServices.GetRewardPointsCount(customerId: _workContext.CurrentCustomer.Id);

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/_WalletPaged.cshtml", rewards);
        }


        public virtual IActionResult Orders()
        {
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/Orders.cshtml");
        }


        public virtual IActionResult OrdersPaged(int pageIndex, int pageSize)
        {
            var orders = _orderService.SearchOrders(
                //storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id,
                pageIndex: pageIndex,
                pageSize: pageSize).ToList();

            var orderModel = orders.Where(p => !p.OrderItems.Any(n => n.ProductId == 10277)).Select(x => new OrderModel
            {
                OrderId = x.Id,
                CategoryName = x.OrderItems.FirstOrDefault()?.Product.ProductCategories.FirstOrDefault().Category.Name,
                CategoryId = x.OrderItems.FirstOrDefault()?.Product.ProductCategories.FirstOrDefault().Category.Id,
                OrderDate = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc).ToString(),
                OrderTotal = Convert.ToInt32(x.OrderTotal),
                OrderStatus = x.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                OrderStatusId = x.OrderStatusId,
                PaymentStatusId = x.PaymentStatusId,
                strBillingAddress = x.BillingAddress != null ? x.BillingAddress.SumAddress() : ""
            }).ToList();

            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.RowsCount = _orderServices.GetCustomerOrderCount(customerId: _workContext.CurrentCustomer.Id);

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/_OrdersPaged.cshtml", orderModel);
        }
        [HttpPost]
        public IActionResult GetOrderDetails(int orderId)
        {
            return null;
        }
        public virtual IActionResult CustomerInfo()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();
            var customer = _workContext.CurrentCustomer;
            string avatarUrl = _pictureService.GetPictureUrl(
                                    _workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                                    _mediaSettings.AvatarPictureSize,
                                    false);
            if (!string.IsNullOrEmpty(avatarUrl))
            {
                avatarUrl = avatarUrl.Replace("http://localhost:55390", "..");
                avatarUrl = avatarUrl.Replace("https://postex.ir", "..");
                avatarUrl = avatarUrl.Replace("http://postex.ir", "..");
            }
            var model = new CustomerInfModel()
            {
                CustomerInfoModel = new CustomerInfoModel
                {
                    FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName),
                    LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName),
                    Email = customer.Email,
                    Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                    AvatarUrl = avatarUrl
                }
                ,
                ChangePasswordModel = new Web.Models.Customer.ChangePasswordModel()
            };

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/CustomerInfo.cshtml", model);
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

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/ChangePass.cshtml");
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
            try
            {
                if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var customer = _workContext.CurrentCustomer;

            var files = Request.Form.Files;
           
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
                if (!string.IsNullOrEmpty(url))
                {
                    url = url.Replace("http://localhost:55390", "..");
                    url = url.Replace("https://postex.ir", "..");
                    url = url.Replace("http://postex.ir", "..");
                }
                #region ADD to table Avatar for Check
                Tbl_CheckAvatarCustomer Temp = new Tbl_CheckAvatarCustomer();
                Temp.CustomerAvatarId = customerAvatarId;
                Temp.CustomerId = customer.Id;
                Temp.DateInsert = DateTime.Now;
                Temp.StateVerify = 0;
                _repositoryTbl_CheckAvatarCustomer.Insert(Temp);
                #endregion

                return Json(new { errorCode = 0});

            }
            catch (Exception exc)
            {
                LogException(exc);
                return Json(new { errorCode = -2});
            }
        }


        public virtual IActionResult PostalAddress()
        {
            var model = new PostalAddress();
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/PostalAddress.cshtml", model);
        }

        [HttpPost]
        public virtual IActionResult PostalAddress(PostalAddress model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();

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
                    Company = model.Company
                };
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                // _newCheckout.ProcessAddress(address, _workContext.CurrentCustomer, "");
                return Content("");
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

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpGet]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel
            {
            };

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/Configure.cshtml", model);
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
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/support.cshtml");
        }

        public virtual IActionResult supportPaged(int pageIndex, int pageSize)
        {
            var supports = _repositoryTbl_Ticket.Table.Where(p => p.IdCustomer == _workContext.CurrentCustomer.Id).ToList();

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

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/_supportPaged.cshtml", SupportModel);
        }



        public virtual IActionResult AddSupport()
        {
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/AddSupport.cshtml");
        }

        [HttpPost]
        public IActionResult CreateTicket(AddTicketModel param)//, HttpPostedFileBase[] _files
        {

            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
            #region check params

            if (param.DepartmentId == 0)
            {
                return Json(new { error = true, Status = 110 });

            }
            if (param.ProrityId == 0)
            {
                return Json(new { error = true, Status = 111 });
            }
            if (string.IsNullOrEmpty(param.Issue))
            {
                return Json(new { error = true, Status = 112 });
            }
            if (string.IsNullOrEmpty(param.TrackingCode) && param.OrderCode == 0)
            {
                return Json(new { error = true, Status = 112 });
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
                            if (files.First().Length > 3145728)
                            {

                                throw new NopException("حجم فایل انتخاب شده بیش از 3 مگا بایت میباشد");
                            }
                            else
                            {
                                var number = new Random();
                                string oldfilename = item.FileName;
                                string format = "";
                                if (oldfilename.Contains(".jpg"))
                                {
                                    format = ".jpg";
                                }
                                else
                                {
                                    format = ".pdf";
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
            temp.Description = param.Description;
            temp.IdTicket = newticket.Id;
            temp.Type = false;
            if (listfilename.Count == 1)
            {
                temp.UrlFile1 = listfilename[0];
            }
            if (listfilename.Count == 2)
            {
                temp.UrlFile2 = listfilename[1];
            }
            if (listfilename.Count == 3)
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
            try
            {
                Ticket.Domain.Tbl_Ticket ticket = _repositoryTbl_Ticket.GetById(param.Id);



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
                                if (files.First().Length > 3145728)
                                {

                                    throw new NopException("حجم فایل انتخاب شده بیش از 3 مگا بایت میباشد");
                                }
                                else
                                {
                                    var number = new Random();
                                    string oldfilename = item.FileName;
                                    string format = "";
                                    if (oldfilename.Contains(".jpg"))
                                    {
                                        format = ".jpg";
                                    }
                                    else
                                    {
                                        format = ".pdf";
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
                }
                catch
                {

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
                temp.Description = param.Description;
                temp.IdTicket = ticket.Id;
                temp.Type = false;
                if (listfilename.Count == 1)
                {
                    temp.UrlFile1 = listfilename[0];
                }
                if (listfilename.Count == 2)
                {
                    temp.UrlFile2 = listfilename[1];
                }
                if (listfilename.Count == 3)
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

            }
            catch
            {
                return Json(new { error = true });
            }



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
            model.NameCategory = _repositoryTbl_CategoryTicket.GetById(model.Tbl_Ticket.IdCategoryTicket).NameCategoryTicket;
            model.NameCustomer = _workContext.CurrentCustomer.GetFullName();
            model.NameDep = _repositoryTbl_Department.GetById(model.Tbl_Ticket.DepartmentId).Name.ToString();
            model.Proirity = _repositoryTbl_Priority.GetById(model.Tbl_Ticket.ProrityId).Name.ToString();
            model.Status = model.Tbl_Ticket.Status == 0 ? "در صف انتظار" : model.Tbl_Ticket.Status == 1 ? "در حال بررسی" : model.Tbl_Ticket.Status == 2 ? "پاسخ داده شده" : "پایان یافته";
            List<Tbl_Ticket_Detail> lll = _repositoryTbl_TicketDetail.Table.Where(p => p.IdTicket == model.Tbl_Ticket.Id).OrderByDescending(p => p.Id).ToList();
            model.vmTicket_Detail = new List<vmTicket_Detail>();
            foreach (var item in lll)
            {
                vmTicket_Detail temp = new vmTicket_Detail();
                temp.NameStaff = item.StaffId != null ? _customerServices.GetCustomerById(item.StaffId.GetValueOrDefault()).GetFullName() : "";
                temp.List_Detail = item;
                model.vmTicket_Detail.Add(temp);

            }
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/DetailSupport.cshtml", model);
        }


        [HttpPost]
        public ActionResult ReadFilePdf(string url)
        {
            ViewBag.UrlFil = url;
            return PartialView("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/ReadFilePdf.cshtml");
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
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/ListFAQ.cshtml", model);
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

            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/Services.cshtml", model);
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
        [HttpPost]
        public IActionResult cancelOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null)
            {
                return Json(new { message = "سفارش مورد نظر یافت نشد", success = false });
            }
            if (order.Shipments.Any())
            {
                if (order.Shipments.Any(p => p.ShippedDateUtc != null))
                {
                    return Json(new { message = "سفارش شما وارد فرایند ارسال شده و امکان کنسل کردن آن وجود ندارد", success = false });
                }
            }
            _orderProcessingService.CancelOrder(order, true);
            return Json(new { message = "سفارش شما جهت کنسل شدن ارجاع شد . نتیجه به صورت پیامک ارسال خواهد شد", success = true });
        }

        public virtual IActionResult AddRequestCOD()
        {
            return View("~/Plugins/Nop.Plugin.Misc.PostbarDashboard/Views/Sep/AddRequestCOD.cshtml");
        }
        [HttpPost]
        public IActionResult CreateRequestCOD(AddRequestCODModel param)
        {
            try
            {
                if (!_workContext.CurrentCustomer.IsRegistered())
                    return Challenge();//login
                if (_codRequestService.CustomerHasActiveCOD(_workContext.CurrentCustomer.Id))
                {
                    TempData["error"] = "حساب کاربری پس کرایه شما در حال حاضر فعال می باشد";
                    return Redirect("Dashboard/AddRequestCOD");
                }
                var userName = _workContext.CurrentCustomer.Username;
                string country, state;

                #region Validate Data
                string error;
                var ReciverCountryCOde = _repositoryCountryCode.Table.FirstOrDefault(p => p.CountryId == param.Country);
                if (ReciverCountryCOde == null)
                {
                    error = "کد استان گیرنده مربوط به  " + $"{ReciverCountryCOde}" + " یافت نشد";
                    Log(error, "");
                }
                if (string.IsNullOrEmpty(ReciverCountryCOde.CountryCode))
                {
                    error = "کد استان گیرنده مربوط به  " + $"{ReciverCountryCOde.CountryCode}" + " یافت نشد";
                    Log(error, "");
                }
                var RecivercityCode = _repositoryStateCode.Table.FirstOrDefault(p => p.stateId == param.State);
                if (RecivercityCode == null)
                {
                    error = "کد شهر گیرنده مربوط به  " + $"{RecivercityCode}" + " یافت نشد";
                    Log(error, "");
                }
                if (string.IsNullOrEmpty(RecivercityCode.StateCode))
                {
                    error = "کد شهر گیرنده مربوط به  " + $"{RecivercityCode.StateCode}" + " یافت نشد";
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
                var tempRequest = _repositoryTbl_RequestCODCustomer.Table.Where(p => p.IdCustomer == _workContext.CurrentCustomer.Id
                    && p.Status == 0).ToList();
                if (tempRequest.Any())
                {
                    foreach (var item in tempRequest)
                    {
                        item.Status = 2;
                        item.DateUpdate = DateTime.Now;
                        item.StaffIdLastAnswer = _workContext.CurrentCustomer.Id;
                        _repositoryTbl_RequestCODCustomer.Update(item);
                    }

                }
                Ticket.Domain.Tbl_RequestCODCustomer newRequestCOD = new Ticket.Domain.Tbl_RequestCODCustomer();
                newRequestCOD.Fname = param.Fname;
                newRequestCOD.Lname = param.Lname;
                newRequestCOD.NatinolCode = param.NationalCode;
                newRequestCOD.Shaba = param.AccountIBAN;
                newRequestCOD.Address = param.Address;
                newRequestCOD.StateId = param.Country;
                newRequestCOD.CityId = param.State;
                newRequestCOD.IdCustomer = _workContext.CurrentCustomer.Id;
                newRequestCOD.IsActive = true;
                newRequestCOD.StoreId = _storeContext.CurrentStore.Id;
                newRequestCOD.DateInsert = DateTime.Now;
                newRequestCOD.Status = 0;
                newRequestCOD.Username = userName;
                newRequestCOD.CodePosti = param.PostalCode;
                _repositoryTbl_RequestCODCustomer.Insert(newRequestCOD);

                #endregion


                #region InsertOrder
                param.PaymentMethod = "NopFarsi.Payments.SepShaparak";
                PlaceOrderResult result = _codRequestService.ProcessCODRequestOrder(newRequestCOD, param.PaymentMethod);
                if (result.Success)
                {
                    error = null;
                    var order = result.PlacedOrder;
                    newRequestCOD.OrderId = order.Id;
                    _repositoryTbl_RequestCODCustomer.Update(newRequestCOD);

                    order.PaymentMethodSystemName = param.PaymentMethod;
                    _orderService.UpdateOrder(order);
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = order
                    };
                    _paymentService.PostProcessPayment(postProcessPaymentRequest);

                    if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                    {
                        return Content("Redirected");
                    }

                    return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                }
                else
                {
                    TempData["error"] = string.Join(Environment.NewLine, result.Errors);
                    return Redirect("Dashboard/AddRequestCOD");
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { success = false });
            }
        }
        public void Log(string header, string Message)
        {
            //var logger = (DefaultLogger)EngineContext.Current.Resolve<ILogger>();
            //logger.InsertLog(LogLevel.Information, header, Message, null);
        }
    }
}

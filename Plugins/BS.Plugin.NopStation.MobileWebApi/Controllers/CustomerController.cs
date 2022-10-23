using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Factories;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Models.App.Params;
using BS.Plugin.NopStation.MobileWebApi.Models.App.Results;
using BS.Plugin.NopStation.MobileWebApi.Services;
using BS.Plugin.NopStation.MobileWebApi.Validator;
using BS.Plugin.Orders.ExtendedShipment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
//using System.Web.Mvc;
using Nop.Core.Domain.Tax;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Misc.PhoneLogin.Services;
using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Plugin.Misc.PostbarDashboard.Services;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
//using Nop.Web.Models.Customer;
//using Nop.Web.Models.Common;
using Nop.Services.Tax;
using Nop.Services.Topics;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FrotelServiceLibrary;
using FrotelServiceLibrary.Input;
using FrotelServiceLibrary.Output;
using static Nop.plugin.Orders.ExtendedShipment.Services.AgentAmountRuleService;
using static Nop.plugin.Orders.ExtendedShipment.Services.ExtendedShipmentService;
using System.Globalization;
using BS.Plugin.NopStation.MobileWebApi.Models;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class APICustomerController : BasePublicController
    {
        #region Field
        private readonly IAgentAmountRuleService _agentAmountRuleService;
        private readonly IOptimeApiService _optimeApiService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICustomerServices _customerServices;
        private readonly IDashboardService _dashboardService;
        private readonly ICustomerModelFactoryApi _customerModelFactoryApi;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly TaxSettings _taxSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IDbContext _dbContext;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ITaxService _taxService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IAddressService _addressService;
        private readonly INotificationService _notificationService;
        private readonly IApService _apService;
        private readonly ICODRequestService _cODRequestService;
        private readonly ITopicService _topicService;
        private readonly IRepository<Tbl_CheckAvatarCustomer> _repositoryTbl_CheckAvatarCustomer;
        private readonly ISekehService _sekeService;
        private readonly IPdfService _pdfService;
        private readonly ICustomerServiceApi _customerServiceApi;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IRepository<OrderItem> _repositoryOrderItem;

        #endregion

        #region Ctor
        public APICustomerController(
            IAgentAmountRuleService agentAmountRuleService,
            IOptimeApiService optimeApiService,
            IRepository<Tbl_CheckAvatarCustomer> repositoryTbl_CheckAvatarCustomer,
            ICODRequestService cODRequestService,
            IOrderProcessingService orderProcessingService,
            ICustomerServices customerServices,
            IDashboardService dashboardService,
            IApService apService,
            ICustomerModelFactoryApi customerModelFactoryApi,
            CustomerSettings customerSettings,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            DateTimeSettings dateTimeSettings,
            TaxSettings taxSettings,
            LocalizationSettings localizationSettings,
            MediaSettings mediaSettings,
            IOrderService orderService,
            IShoppingCartService shoppingCartService,
            IWorkContext workContext,
            IStoreContext storeContext,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IStateProvinceService stateProvinceService,
            ICustomerAttributeService customerAttributeService,
            ICustomerAttributeParser customerAttributeParser,
            IGenericAttributeService genericAttributeService,
            IAuthenticationService authenticationService,
            ITaxService taxService,
            IWorkflowMessageService workflowMessageService,
            IAddressService addressService,
            INotificationService notificationService,
            IPictureService pictureService,
            IDbContext dbContext,
            ITopicService topicService,
            ISekehService sekeService,
            ICustomerServiceApi customerServiceApi,
             IPdfService pdfService,
             IExtendedShipmentService extendedShipmentService,
             IRepository<OrderItem> repositoryOrderItem
        )
        {
            _agentAmountRuleService = agentAmountRuleService;
            _repositoryOrderItem = repositoryOrderItem;
            _extendedShipmentService = extendedShipmentService;
            _pdfService = pdfService;
            _customerServiceApi = customerServiceApi;
            _optimeApiService = optimeApiService;
            _topicService = topicService;
            _repositoryTbl_CheckAvatarCustomer = repositoryTbl_CheckAvatarCustomer;
            _orderProcessingService = orderProcessingService;
            _customerServices = customerServices;
            _dashboardService = dashboardService;
            _pictureService = pictureService;
            _dbContext = dbContext;
            _apService = apService;
            _cODRequestService = cODRequestService;
            this._customerModelFactoryApi = customerModelFactoryApi;
            this._customerSettings = customerSettings;
            this._customerRegistrationService = customerRegistrationService;
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._dateTimeSettings = dateTimeSettings;
            this._taxSettings = taxSettings;
            this._localizationSettings = localizationSettings;
            this._mediaSettings = mediaSettings;
            this._orderService = orderService;
            this._shoppingCartService = shoppingCartService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._stateProvinceService = stateProvinceService;
            this._customerAttributeService = customerAttributeService;
            this._customerAttributeParser = customerAttributeParser;
            this._genericAttributeService = genericAttributeService;
            this._authenticationService = authenticationService;
            this._taxService = taxService;
            this._workflowMessageService = workflowMessageService;
            this._addressService = addressService;
            _notificationService = notificationService;
            _sekeService = sekeService;
            _customerServiceApi = customerServiceApi;
        }
        #endregion

        #region Utility

        protected virtual string ParseCustomCustomerAttributes(NameValueCollection form)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
            var attributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in attributes)
            {
                string controlId = string.Format("customer_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.FileUpload:
                    //not supported customer attributes
                    default:
                        break;
                }
            }

            return attributesXml;
        }

        protected void AddressBind(Address model, Address destination, bool trimFields = true)
        {


            if (trimFields)
            {
                if (model.FirstName != null)
                    model.FirstName = model.FirstName.Trim();
                if (model.LastName != null)
                    model.LastName = model.LastName.Trim();
                if (model.Email != null)
                    model.Email = model.Email.Trim();
                if (model.Company != null)
                    model.Company = model.Company.Trim();
                if (model.City != null)
                    model.City = model.City.Trim();
                if (model.Address1 != null)
                    model.Address1 = model.Address1.Trim();
                if (model.Address2 != null)
                    model.Address2 = model.Address2.Trim();
                if (model.ZipPostalCode != null)
                    model.ZipPostalCode = model.ZipPostalCode.Trim();
                if (model.PhoneNumber != null)
                    model.PhoneNumber = model.PhoneNumber.Trim();
                if (model.FaxNumber != null)
                    model.FaxNumber = model.FaxNumber.Trim();
            }

            destination.FirstName = model.FirstName;
            destination.LastName = model.LastName;
            destination.Email = model.Email;
            destination.Company = model.Company;
            destination.CountryId = model.CountryId;
            destination.StateProvinceId = model.StateProvinceId;
            destination.City = model.City;
            destination.Address1 = model.Address1;
            destination.Address2 = model.Address2;
            destination.ZipPostalCode = model.ZipPostalCode;
            destination.PhoneNumber = model.PhoneNumber;
            destination.FaxNumber = model.FaxNumber;


        }

        #endregion
        [Route("api/AgentList")]
        public IActionResult getAgentList()
        {
            try
            {
                string query = $@"SELECT DISTINCT
	                                C.Id CollectorCustomerId,
	                                C.Username CollectorUsername,
	                                TCI.FullName CollectorFullName,
	                                C2.Id RepresentativeCustomerId,
	                                C2.Username RepresentativeUsername,
	                                TCI2.FullName RepresentativeFullName,
	                                sc.stateId PostexStateId ,
	                                Sc.StateCode PostStateCode
                                FROM	
	                                dbo.Customer AS C
	                                INNER JOIN dbo.Tbl_AgentNearpostNode AS TANN ON C.id = TANN.AgentCustomerId
	                                INNER JOIN dbo.Customer AS C2 ON TANN.RepresentativeCustomerId = C2.Id
	                                INNER JOIN dbo.StateProvince AS SP ON TANN.NearSateId = Sp.Id
	                                INNER JOIN dbo.StateCode AS SC ON Sp.Id= sc.stateId
	                                LEFT JOIN dbo.Tb_CustomerInfo AS TCI2 ON C2.Id = TCI2.CustomerId
	                                LEFT JOIN dbo.Tb_CustomerInfo AS TCI  ON C.Id = Tci.CustomerId
                                WHERE
	                                c.Active = 1";
                var _data = _dbContext.SqlQuery<AgentModel>(query, new object[0]).ToList();
                return Json(new { success = true, data = _data });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { success = false, data = (List<AgentModel>)null });
            }
        }
         [Route("api/AgentNavy")]
        public IActionResult getAgentNavy(int CustomerId)
        {
            try
            {
                string query = $@"SELECT
	                TCV.Name CollectoName,
	                TCV.Phone CollectorPhoneNumber,
	                CASE WHEN TCV.VehicleTypeEnum =0 THEN N'موتور' WHEN TCV.VehicleTypeEnum =1 THEN N'خودرو شخصی' WHEN TCV.VehicleTypeEnum =2 THEN N'وانت' WHEN TCV.VehicleTypeEnum =3 THEN N'کامیون' ELSE N'نامشخص' END 
		                VehicleType
                FROM
	                dbo.Tbl_CustomerVehicle AS TCV
                WHERE
	                 TCV.IsActive = 1
	                 AND TCV.CustomerId = {CustomerId}";
                var _data = _dbContext.SqlQuery<AgentNavyModel>(query, new object[0]).ToList();
                return Json(new { success = true, data = _data });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { success = false, data = (List<AgentModel>)null });
            }
        }
        #region Login
        [Route("api/JLogin")]
        [HttpPost]
        public IActionResult Json_Login([FromBody] LoginQueryModel model)
        {
            return _Login(model);
        }
        [Route("api/login")]
        [HttpPost]
        public IActionResult Login(LoginQueryModel model)
        {
            return _Login(model);
        }
        public IActionResult _Login(LoginQueryModel model)
        {
            // LogException(new Exception("android app" + "==>" + Newtonsoft.Json.JsonConvert.SerializeObject(model)));
            var customerLoginModel = new LogInPostResponseModel();
            customerLoginModel.StatusCode = (int)ErrorType.NotOk;
            ValidationExtension.LoginValidator(ModelState, model, _localizationService, _customerSettings);
            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);

                switch (loginResult)
                {

                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled ? _customerService.GetCustomerByUsername(model.Username) : _customerService.GetCustomerByEmail(model.Email);
                            customerLoginModel = _customerModelFactoryApi.PrepareCustomerLoginModel(customerLoginModel, customer);
                            customerLoginModel.StatusCode = (int)ErrorType.Ok;
                            //migrate shopping cart
                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);
                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);
                            //string deviceId = GetDeviceIdFromHeader();
                            //var device = _deviceService.GetDeviceByDeviceToken(deviceId);
                            //if (device != null)
                            //{
                            //    device.CustomerId = customer.Id;
                            //    device.IsRegistered = customer.IsRegistered();
                            //    _deviceService.UpdateDevice(device);
                            //}
                            break;

                        }
                    case CustomerLoginResults.CustomerNotExist:
                        customerLoginModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist")
                        };
                        break;
                    case CustomerLoginResults.Deleted:

                        customerLoginModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials.Deleted")
                        };
                        break;
                    case CustomerLoginResults.NotActive:

                        customerLoginModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials.NotActive")
                        };
                        break;
                    case CustomerLoginResults.NotRegistered:

                        customerLoginModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered")
                        };
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:

                        customerLoginModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials")
                        };
                        break;
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        customerLoginModel.ErrorList.Add(error.ErrorMessage);
                    }
                }
            }
            //If we got this far, something failed, redisplay form


            return Ok(customerLoginModel);
        }

        [Route("api/PnlLogin")]
        [HttpPost]
        public IActionResult ebazar_Login(string mobile)
        {
            LoginQueryModel model = new LoginQueryModel();
            var customerLoginModel = new LogInPostResponseModel();
            customerLoginModel.StatusCode = (int)ErrorType.NotOk;
            model.Username = _sekeService.VerifyToken(mobile);//decode
            if (string.IsNullOrEmpty(model.Username) || model.Username.Length != 11 || !model.Username.StartsWith("09"))
            {
                customerLoginModel.ErrorList = new List<string>
                        {
                            "نام کاربری وارد شده نا معتبر می باشد"
                        };
                return Ok(customerLoginModel);
            }
            var customer = _customerService.GetCustomerByUsername(model.Username); ;
            if (customer == null)
            {
                customer = _sekeService.Register(model.Username, out string msg, 0, false, false);
            }
            if (customer == null)
            {
                customerLoginModel.ErrorList = new List<string>
                        {
                            "در حال حاضر امکان ورود به سامانه برای شما وجود ندارد لطفا با پشتیبانی فنی تماس بگیرید"
                        };
                return Ok(customerLoginModel);
            }

            customerLoginModel = _customerModelFactoryApi.PrepareCustomerLoginModel(customerLoginModel, customer);
            customerLoginModel.StatusCode = (int)ErrorType.Ok;
            return Ok(customerLoginModel);
        }


        [Route("api/IsTokenExpire")]
        [HttpPost]
        public IActionResult IsTokenExpire(string Token)
        {
            var secretKey = Constant.SecretKey;
            try
            {
                var payload = JWT.JsonWebToken.DecodeToObject(Token, secretKey) as IDictionary<string, object>;
                return Json(new { IsValid = true });
            }
            catch
            {
                return Json(new { IsValid = false });
            }
        }
        //public IActionResult RequestValidationCode(string mobile,string sign)
        //{

        //}
        //[Route("api/IsTokenExpire")]
        //public IActionResult IsValidValidationCode(string mobile,string Code)
        //{

        //}
        #endregion

        [HttpPost]
        [Route("api/customer/GetAffilateDiscount")]
        public IActionResult GetAffilateDiscount(int OrderId, int CustomerId)
        {
            try
            {
                Nop.Core.Domain.Orders.Order O = null;
                if (OrderId > 0)
                    O = _orderService.GetOrderById(OrderId);
                if (O == null)
                    return Json(new { discountValue = 0 });
                int SumDiscountValue = 0;
                foreach (var OI in O.OrderItems)
                {
                    PrivatePostDiscount privatePostDiscount = new PrivatePostDiscount();
                    var discount = _agentAmountRuleService.getInlineDsicountByCustomer(OI, CustomerId, out privatePostDiscount);
                    if (discount != null && discount.Price > 0)
                        SumDiscountValue += discount.Price;
                    else if (privatePostDiscount != null && privatePostDiscount.DisCountAmount > 0 && !privatePostDiscount.IsPercent)
                        SumDiscountValue += privatePostDiscount.DisCountAmount;
                }
                return Json(new { discountValue = SumDiscountValue });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { discountValue = 0 });
            }
        }
        [HttpPost]
        [Route("api/customer/register2")]
        public IActionResult Register2(RegisterQueryModel model)
        {

            var customer = _workContext.CurrentCustomer;
            var form = model.FormValue.ToNameValueCollection();
            var response = new RegisterResponseModel();
            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            ValidationExtension.RegisterValidator(ModelState, model, _localizationService, _stateProvinceService, _customerSettings);
            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }

                bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new CustomerRegistrationRequest(customer,
                   ((string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email)) ? model.Username + "@postbar.ir" : model.Email),
                   _customerSettings.UsernamesEnabled ? model.Username : model.Email,
                   model.Password,
                   _customerSettings.DefaultPasswordFormat,
                   _storeContext.CurrentStore.Id,
                   isApproved);
                var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                if (registrationResult.Success)
                {
                    customer.CustomerGuid = new Guid();
                    _customerService.UpdateCustomer(customer);
                    //properties
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.TimeZoneId);
                    }
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumber, model.VatNumber);

                        string vatName;
                        string vatAddress;
                        var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName, out vatAddress);
                        _genericAttributeService.SaveAttribute(customer,
                            SystemCustomerAttributeNames.VatNumberStatusId,
                            (int)vatNumberStatus);
                        //send VAT number admin notification
                        if (!String.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                            _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);

                    }

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                    _genericAttributeService.SaveAttribute(customer, "CodeMeli", model.CodeMeli);

                    if (_customerSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, dateOfBirth);
                    }
                    if (_customerSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company, model.Company);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress, model.StreetAddress);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress2, model.StreetAddress2);
                    if (_customerSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ZipPostalCode, model.ZipPostalCode);
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.City, model.City);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId, model.CountryId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId, model.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);

                    //newsletter
                    if (_customerSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(model.Email, _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                            }
                            //else
                            //{
                            //When registering, not checking the newsletter check box should not take an existing email address off of the subscription list.
                            //_newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                            //}
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = model.Email,
                                    Active = true,
                                    StoreId = _storeContext.CurrentStore.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    //save customer attributes
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributesXml);

                    //login customer now
                    if (isApproved)
                        _authenticationService.SignIn(customer, true);

                    //associated with external account (if possible)
                    //TryAssociateAccountWithExternalAccount(customer);

                    //insert default address (if possible)
                    var defaultAddress = new Address
                    {
                        FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName),
                        LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName),
                        Email = customer.Email,
                        Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company),
                        CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId) > 0 ?
                            (int?)customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId) : null,
                        StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId) > 0 ?
                            (int?)customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId) : null,
                        City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City),
                        Address1 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress),
                        Address2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2),
                        ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode),
                        PhoneNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                        FaxNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax),
                        CreatedOnUtc = customer.CreatedOnUtc
                    };
                    if (this._addressService.IsAddressValid(defaultAddress))
                    {
                        //some validation
                        if (defaultAddress.CountryId == 0)
                            defaultAddress.CountryId = null;
                        if (defaultAddress.StateProvinceId == 0)
                            defaultAddress.StateProvinceId = null;
                        //set default address
                        customer.Addresses.Add(defaultAddress);
                        customer.BillingAddress = defaultAddress;
                        customer.ShippingAddress = defaultAddress;
                        _customerService.UpdateCustomer(customer);
                    }

                    //notifications
                    if (_customerSettings.NotifyNewCustomerRegistration)
                        _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer, _localizationSettings.DefaultAdminLanguageId);

                    switch (_customerSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //email validation message
                                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
                                _workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);
                                response.SuccessMessage = _localizationService.GetResource("Account.Register.Result.EmailValidation");
                                break;
                                //result
                                //return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                response.SuccessMessage = _localizationService.GetResource("Account.Register.Result.AdminApproval");
                                break;
                                //return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });
                            }
                        case UserRegistrationType.Standard:
                        default:
                            {

                                //send customer welcome message
                                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);
                                response.SuccessMessage = _localizationService.GetResource("Account.Register.Result.Standard");
                                break;
                                //var redirectUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                                //if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                //    redirectUrl = _webHelper.ModifyQueryString(redirectUrl, "returnurl=" + HttpUtility.UrlEncode(returnUrl), null);
                                //return Redirect(redirectUrl);
                            }
                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                {
                    response.StatusCode = (int)ErrorType.NotOk;
                    response.ErrorList.Add(error);
                }

            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        response.ErrorList.Add(error.ErrorMessage);
                    }
                }
                response.StatusCode = (int)ErrorType.NotOk;
            }
            //If we got this far, something failed, redisplay form

            return Ok(response);
        }
        [HttpPost]
        [Route("api/customer/MobReg")]
        public IActionResult RegisterByMoible(string mobile)
        {
            if (string.IsNullOrEmpty(mobile) || mobile.Length < 11 || !mobile.StartsWith("09"))
            {
                return Json(new { resultMessage = "شماره موبایل وارد شده نامعتبر می باشد" });
            }
            string message = "";
            var customer = _apService.Register(mobile, out message, 1149, true);
            return Json(new { resultMessage = message });
        }


        [HttpPost]
        [Route("api/customer/register")]
        public object Register(mRegister _model)
        {
            try
            {
                common.Log("وردی ثبت نام Api", Newtonsoft.Json.JsonConvert.SerializeObject(_model));
                #region CheckParam
                if (string.IsNullOrEmpty(_model.Mobile) && !string.IsNullOrEmpty(_model.Username))
                {
                    _model.Mobile = _model.Username;
                }
                if (!string.IsNullOrEmpty(_model.Mobile) && string.IsNullOrEmpty(_model.Username))
                {
                    _model.Username = _model.Mobile;
                }
                if (string.IsNullOrEmpty(_model.CodeMeli))
                {
                    mResponseList.error_invalid_param.message_fa = "کد ملی خالی میباشد";
                    return mResponseList.error_invalid_param;
                }
                if (string.IsNullOrEmpty(_model.Mobile))
                {
                    mResponseList.error_invalid_param.message_fa = "موبایل خالی میباشد";
                    return mResponseList.error_invalid_param;
                }
                if (string.IsNullOrEmpty(_model.Password))
                {
                    mResponseList.error_invalid_param.message_fa = "پسورد خالی میباشد";
                    return mResponseList.error_invalid_param;
                }
                if (!CheckCodeMeli.IsValidNationalCode(_model.CodeMeli))
                {
                    mResponseList.error_invalid_param.message_fa = "کد ملی ارسال شده نامعتبر است";
                    return mResponseList.error_invalid_param;
                }
                if (!_model.Mobile.StartsWith("09") || _model.Mobile.Length != 11)
                {
                    mResponseList.error_invalid_param.message_fa = "شماره موبایل ارسال شده نامعتبر است";
                    return mResponseList.error_invalid_param;
                }
                #endregion

                RegisterQueryModel model = new RegisterQueryModel();
                var response = new RegisterResponseModel();
                model.CodeMeli = _model.CodeMeli;
                model.Username = _model.Mobile;
                model.Password = _model.Password;
                model.ConfirmPassword = _model.Password;
                model.Email = model.Username + (_storeContext.CurrentStore.Id == 5 ? "@postex.ir" : "@Postbar.ir");
                if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                {
                    return mResponseList.error_Register_Disable;
                }
                //_workContext.CurrentCustomer.IsRegistered()
                Customer duplicate = _customerService.GetCustomerByUsername(_model.Mobile);
                if (duplicate != null)
                {
                    return mResponseList.error_Register_Duplicate;
                }
                _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
                var customer = _workContext.CurrentCustomer;
                customer.RegisteredInStoreId = _storeContext.CurrentStore.Id;
                var attribute = _customerAttributeService.GetCustomerAttributeById(8);
                string customerAttributesXml = "";
                customerAttributesXml = _customerAttributeParser.AddCustomerAttribute("",
                                         attribute, _model.CodeMeli.ToString());


                ValidationExtension.RegisterValidator(ModelState, model, _localizationService, _stateProvinceService, _customerSettings);
                if (ModelState.IsValid)
                {
                    if (_customerSettings.UsernamesEnabled && model.Username != null)
                    {
                        model.Username = model.Username.Trim();
                    }
                    bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
                    customer.IsSystemAccount = false;
                    var registrationRequest = new CustomerRegistrationRequest(customer,
                    model.Email,
                    _customerSettings.UsernamesEnabled ? model.Username : model.Email,
                    model.Password,
                    _customerSettings.DefaultPasswordFormat,
                    _storeContext.CurrentStore.Id,
                    isApproved);
                    var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                    if (registrationResult.Success)
                    {
                        customer.CustomerGuid = new Guid();
                        _customerService.UpdateCustomer(customer);
                        _genericAttributeService.SaveAttribute(customer, "CodeMeli", model.CodeMeli);
                        //save customer attributes
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributesXml);
                        return mResponseList.success;

                    }
                    else
                    {
                        string RegisterResult = string.Join(",", registrationResult.Errors);
                        mResponseList.error_Execption.message_fa = "ثبت نام کاربر با موفقیت انجام نشد";
                        common.Log("خطا در زمان ثبت نام api", RegisterResult);
                        return mResponseList.error_Execption;
                    }
                }
                else
                {
                    return mResponseList.error_invalid_param;
                }

            }
            catch (Exception ee)
            {
                LogException(ee);
                mResponseList.error_Execption.data = ee.Message;
                return mResponseList.error_Execption;
            }
        }

        [HttpPost]
        [Route("api/customer/UploadAvatar")]
        public IActionResult UploadAvatar(int cusotmerId)
        {
            try
            {

                if (cusotmerId == 0)
                {
                    if (_workContext.CurrentCustomer == null || !_workContext.CurrentCustomer.IsRegistered() || _workContext.CurrentCustomer.IsGuest())
                    {
                        Nop.plugin.Orders.ExtendedShipment.Services.common.Log("امکان آپلود عکس برای کاربران اعتبار سنجی نشده وجودن دارد", "");
                        return Json(new { errorCode = -2, message = "در حال حاضر امکان اعتبار سنجی حساب شما وجود ندارد.لطفا از محیط برنامه خارج  شوید و مجددا تلاش کنید" });
                    }
                }
                var customer = _customerService.GetCustomerById(cusotmerId);
                if (customer == null || !customer.IsRegistered() || customer.IsGuest())
                {
                    Nop.plugin.Orders.ExtendedShipment.Services.common.Log("امکان آپلود عکس برای کاربران اعتبار سنجی نشده وجودن دارد", "");
                    return Json(new { errorCode = -2, message = "در حال حاضر امکان اعتبار سنجی حساب شما وجود ندارد.لطفا از محیط برنامه خارج  شوید و مجددا تلاش کنید" });
                }
                var files = Request.Form.Files;
                string ContentType = "";
                //if (files.First().ContentType == "*/*")
                {
                    string extension = Path.GetExtension(files.First().FileName).ToLower();
                    if (extension == ".jpe" || extension == ".jpeg" || extension == ".jpg")
                        ContentType = "image/jpeg";
                    else if (extension == ".bmp")
                        ContentType = "image/bmp";
                }
                if (ContentType == "")
                {
                    Nop.plugin.Orders.ExtendedShipment.Services.common.Log("پسوند فایل بارگذاری شده نامعتبر می باشد", "");
                    return Json(new { errorCode = -2, message = "پسوند فایل بارگذاری شده نامعتبر می باشد" });
                }
                var customerAvatar = _pictureService.GetPictureById(customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
                if (files != null && files.Count() > 0 && !string.IsNullOrEmpty(files.First().FileName))
                {
                    var avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
                    if (files.First().Length > avatarMaxSize)
                    {
                        Nop.plugin.Orders.ExtendedShipment.Services.common.Log("Ap File Size Over Limit" + "-" + cusotmerId.ToString(), "سایز عکس بیش از حد مجاز می باشد");
                        return Json(new { errorCode = -2, message = "سایز عکس بیش از حد مجاز می باشد" });
                    }
                    Nop.plugin.Orders.ExtendedShipment.Services.common.Log("Ap  logo File Type:" + ContentType, "");
                    var customerPictureBinary = files.First().GetPictureBits();
                    if (customerAvatar != null)
                        customerAvatar = _pictureService.UpdatePicture(customerAvatar.Id, customerPictureBinary, ContentType, null);
                    else
                        customerAvatar = _pictureService.InsertPicture(customerPictureBinary, ContentType, null);
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
                Tbl_CheckAvatarCustomer Temp = new Tbl_CheckAvatarCustomer();
                Temp.CustomerAvatarId = customerAvatarId;
                Temp.CustomerId = customer.Id;
                Temp.DateInsert = DateTime.Now;
                Temp.StateVerify = 0;
                _repositoryTbl_CheckAvatarCustomer.Insert(Temp);
                #endregion
                return Json(new { errorCode = 0 });
            }
            catch (Exception exc)
            {
                LogException(exc);
                return Json(new { errorCode = -2 });
            }
        }

        [HttpPost]
        [Route("api/customer/SaveCodRequest")]
        public IActionResult UploadCodLogo(AddRequestCODModel param)
        {
            try
            {
                if (param.CustomerId == 0)
                {
                    if (_workContext.CurrentCustomer == null || !_workContext.CurrentCustomer.IsRegistered() || _workContext.CurrentCustomer.IsGuest())
                    {
                        Nop.plugin.Orders.ExtendedShipment.Services.common.Log("امکان ثبت درخواست اکانت پس کرایه برای کاربران اعتبار سنجی نشده وجودن دارد", "");
                        return Json(new { errorCode = -2, message = "در حال حاضر امکان اعتبار سنجی حساب شما وجود ندارد.لطفا از محیط برنامه خارج  شوید و مجددا تلاش کنید" });
                    }
                }
                var customer = _customerService.GetCustomerById(param.CustomerId);
                if (customer == null || !customer.IsRegistered() || customer.IsGuest())
                {
                    Nop.plugin.Orders.ExtendedShipment.Services.common.Log("امکان ثبت درخواست اکانت پس کرایه برای کاربران اعتبار سنجی نشده وجودن دارد", "");
                    return Json(new { errorCode = -2, message = "در حال حاضر امکان اعتبار سنجی حساب شما وجود ندارد.لطفا از محیط برنامه خارج  شوید و مجددا تلاش کنید" });
                }
                var files = Request.Form.Files;
                string ContentType = "";
                string extension = Path.GetExtension(files.First().FileName).ToLower();
                if (files.First().ContentType == "*/*")
                {

                    if (extension == ".jpe" || extension == ".jpeg" || extension == ".jpg")
                        ContentType = "image/jpeg";
                    else if (extension == ".bmp")
                        ContentType = "image/bmp";
                }
                if (ContentType == "")
                {
                    Nop.plugin.Orders.ExtendedShipment.Services.common.Log("پسوند فایل بارگذاری شده نامعتبر می باشد", "");
                    return Json(new { errorCode = -2, message = "پسوند فایل بارگذاری شده نامعتبر می باشد" });
                }

                int RequestId = _dashboardService.InsertRequestCOD(param);
                if (RequestId == 0)
                {
                    Nop.plugin.Orders.ExtendedShipment.Services.common.Log("امکان ثبت درخواست اکانت پس کرایه برای کاربران اعتبار سنجی نشده وجودن دارد", "");
                    return Json(new { errorCode = -2, message = "در زمان" });
                }

                var number = new Random();
                string oldfilename = files.First().FileName;

                string filename = RequestId.ToString() + "_" + number.Next(1, 999999999).ToString() + extension;
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\RequestCODFiles")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\RequestCODFiles"));
                }
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\RequestCODFiles", filename);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    files.First().CopyTo(fileStream);
                }

                return Json(new { errorCode = 0 });
            }
            catch (Exception exc)
            {
                LogException(exc);
                return Json(new { errorCode = -2 });
            }
        }

        [HttpPost]
        [Route("api/customer/GetActivationCode")]
        public IActionResult GetActivationCode(string userName)
        {
            var sentCode = new Random().Next(10000, 100000).ToString();
            string SotreName = "کد تایید پستِکس";
            _notificationService.sendSms(sentCode + " : " + SotreName, userName);
            return Json(new { activationCode = sentCode });
        }

        [Route("api/customer/RestPassword")]
        public IActionResult restPassword(string userName)
        {
            if (string.IsNullOrEmpty(userName) || userName.Length != 11 || !CheckNumber(userName))
            {
                return Json(new { message = "نام کاربری وارد شده نامعتبر می باشد", success = false });
            }
            var customer = _customerService.GetCustomerByUsername(userName);
            if (customer == null)
            {
                return Json(new { message = "کاربری مورد نظر یافت نشد", success = false });
            }

            if (customer != null && customer.Active && !customer.Deleted)
            {
                var newPass = RandomString();

                string msg = "";
                if (_customerServices.ChangePassword(customer, newPass, out msg))
                {
                    string SotreName = _storeContext.CurrentStore.Id == 5 ? "پستِکس" : "پست بار";
                    _notificationService.sendSms(newPass + " : " + SotreName, userName);
                    Nop.plugin.Orders.ExtendedShipment.Services.common.Log(userName + ":" + newPass, "");
                    return Json(new { message = "رمز عبور جدید ارسال شد", success = true });
                }
                else
                {
                    return Json(new { message = "خطا در زمان تغییر رمز عبور", success = true });
                }
            }
            return Json(new { message = "امکان تغییر رمز عبور وجود ندارد", success = true });
        }

        [Route("api/customer/BulkCODRegistration")]
        [ServiceFilter(typeof(LocalFilterAttribute))]
        public IActionResult BulkCODRegistration()
        {
            var customers = _dbContext.SqlQuery<AddRequestCODModel>("EXEC dbo.Sp_GetCustomersForCODRequest").ToList();
            _cODRequestService.BulkFullCODRequest(customers);
            return Ok();
        }

        private string RandomString()
        {
            Random rand = new Random();
            const string pool = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var chars = Enumerable.Range(0, 5)
                .Select(x => pool[rand.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }
        public bool CheckNumber(string strPhoneNumber)
        {
            var MatchPhoneNumberPattern = "(0|\\+98)?([ ]|-|[()]){0,2}9[1|2|3|4]([ ]|-|[()]){0,3}(?:[0-9]([ ]|-|[()]){0,2}){8}";
            return strPhoneNumber != null && Regex.IsMatch(strPhoneNumber, MatchPhoneNumberPattern);
        }
        public class MarkAsPaidOrder
        {
            public string user { get; set; }
            public string password { get; set; }
            public int OrderId { get; set; }
        }
        public class Cprice
        {
            public string user { get; set; }
            public string password { get; set; }
            public int CustomerId { get; set; }
            public string ReciverCountryCode { get; set; }
            public string ReciverCityCode { get; set; }
            public int ItemWeight { get; set; }
            public string postType { get; set; }
            public int SenderStateId { get; set; }
            public int OrderItemId { get; set; }
            public int _haghemagharForshipment { get; set; }
            public int _approximateValue { get; set; }
        }
        [Route("api/Order/MarkP")]
        [HttpPost]
        public IActionResult MarkAsPaid(MarkAsPaidOrder model)
        {
            try
            {
                if (!(model.user == "postbar" && model.password == "b1us2^b!Nu"))
                {
                    return Json(new { resultCode = -1, message = "Invalid UserName and password" });
                }
                var order = _orderService.GetOrderById(model.OrderId);
                if (order == null)
                {
                    return Json(new { resultCode = -1, message = "Order not Found" });
                }
                if (!_orderProcessingService.CanMarkOrderAsPaid(order))
                {
                    return Json(new { resultCode = -1, message = "this order can't mark as paid" });
                }
                _orderProcessingService.MarkOrderAsPaid(order);
                return Json(new { resultCode = 0, message = "order mark as paid" });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { resultCode = -1, message = "Error happen.see log for more info" });
            }
        }
        [Route("api/Order/CPrice")]
        [HttpPost]
        public IActionResult GetCodPrices(Cprice model)
        {
            try
            {
                if (!(model.user == "postbar" && model.password == "b1us2^b!Nu"))
                {
                    var reuslt = new CodGetPriceRersult()
                    {
                        Success = false,
                        ErrorMessage = "Invalid UserName and password",
                        AgentUserName = ""
                        ,
                        approximateValue = 0,
                        CodPostPrice = new int[] { 0, 0 },
                        HaizneKala = 0
                    };
                    return Json(reuslt);
                }
                var customer = _customerService.GetCustomerById(model.CustomerId);
                var OrderItem = _repositoryOrderItem.GetById(model.OrderItemId);
                var Result = _extendedShipmentService.CalcGatewayPrice(customer, model.ReciverCountryCode, model.ReciverCityCode, false, model.ItemWeight, model.postType, model.SenderStateId
                    , OrderItem, model._haghemagharForshipment, model._approximateValue, null);
                return Json(Result);
            }
            catch (Exception ex)
            {
                LogException(ex);
                var reuslt = new CodGetPriceRersult()
                {
                    Success = false,
                    ErrorMessage = "Error happen.see log for more info",
                    AgentUserName = ""
                       ,
                    approximateValue = 0,
                    CodPostPrice = new int[] { 0, 0 },
                    HaizneKala = 0
                };
                return Json(reuslt);
            }
        }
        [Route("api/getRules")]
        public string getRuleFromFile()
        {
            var topic = _topicService.GetTopicById(145);
            if (topic == null)
                return "";
            return topic.Body;
            //string rules = "";
            //string path = CommonHelper.MapPath("~/Plugins/Orders.ExtendedShipment/Content/") + "iban.docx";
            //using (var document = WordprocessingDocument.Open(path, false))
            //{
            //    var main = document.MainDocumentPart;
            //    var fonts = main.FontTablePart;
            //    var styles = main.StyleDefinitionsPart;
            //    var effects = main.StylesWithEffectsPart;
            //    var doc = main.Document;
            //    var body = doc.Body;
            //    rules = body.InnerText.ToString();
            //}
            //return rules;
        }

        [Route("api/SekehGetToken")]
        [HttpPost]
        public IActionResult SekeTokenGenerator([FromBody] SekehInputModel inputModel)
        {
            try
            {
                common.Log("SekeTokenGenerator", Newtonsoft.Json.JsonConvert.SerializeObject(inputModel));
                string Mobile = inputModel.MobileNumber;
                if (string.IsNullOrEmpty(Mobile) || Mobile.Length < 11 || !Mobile.StartsWith("09"))
                {
                    return Json(new { success = false, message = "شماره موبایل وارد شده معتبر نیست" });
                }

                if (String.IsNullOrEmpty(inputModel.Password) || !_sekeService.Authenticate(inputModel.Password))
                {
                    return Json(new { success = false, message = "پسورد وارد شده معتبر نیست" });
                }
                Customer customer = _customerService.GetCustomerByUsername(Mobile);

                if (_sekeService.IsValidCustomer(customer))
                {
                    return Json(_sekeService.GetToken(inputModel));
                }
                else
                {
                    _sekeService.Register(Mobile, out string msg);
                    return Json(_sekeService.GetToken(inputModel));
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new SekehOutputModel());
            }
        }

        [Route("Sekeh/_Startup")]
        [HttpGet]
        public IActionResult SekeVerifyToken([FromQuery] string Token)
        {
            string Username = _sekeService.VerifyToken(Token);
            if (Username != null)
            {
                HttpContext.Session.SetString("ComeFrom", "Sekke");

                Customer customer = _customerService.GetCustomerByUsername(Username);
                if (customer != null)
                {
                    _authenticationService.SignIn(customer, true);
                    _genericAttributeService.InsertAttribute(new GenericAttribute()
                    {
                        Key = "InputSource",
                        KeyGroup = "Customer",
                        StoreId = 5,
                        Value = "seke",
                        EntityId = customer.Id
                    });
                    return RedirectToRoute("_ShipitoHome", new { input = "sekeh" });
                }
            }
            return RedirectToRoute("_ShipitoHome", new { input = "sekeh" });
        }

        [Route("api/optimeCheck")]
        [HttpGet]
        public IActionResult GetOptimizedRout(string pass)
        {
            if (pass != "bXVnz6M!8g!XNSqvsiXMX")
                return new EmptyResult();
            _optimeApiService.CheckForOptimizedTask();
            return Json(new { susccess = true });
        }
        [Route("api/InternalCancelOrder")]
        [HttpGet]
        public void InternalCancelOrder(int orderId, string pass)
        {
            try
            {
                if (pass != "bXVnz6M!8g!XNSqvsiXMX")
                    return;
                var order = _orderService.GetOrderById(orderId);
                if (order == null)
                    return;
                if (order.OrderStatus == Nop.Core.Domain.Orders.OrderStatus.Cancelled)
                    return;
                common.InsertOrderNote("سفارش توسط ای پی ای بانک اطلاعاتی کنسل شد", order.Id);
                _orderProcessingService.CancelOrder(order, true);

            }
            catch (Exception ex)
            {
                common.Log("webhook", "SnapboxBikerCancel");
                common.LogException(ex);
            }
        }
        [Route("api/ComplateOrder")]
        [HttpGet]
        public void ComplateOrder(int orderId, string pass)
        {
            try
            {
                if (pass != "bXVnz6M!8g!XNSqvsiXMX")
                    return;
                var order = _orderService.GetOrderById(orderId);
                if (order == null)
                    return;
                if (order.OrderStatus == Nop.Core.Domain.Orders.OrderStatus.Complete)
                    return;
                order.OrderStatusId = 30;
                _orderService.UpdateOrder(order);
            }
            catch (Exception ex)
            {
                common.Log("webhook", "SnapboxBikerCancel");
                common.LogException(ex);
            }
        }
        [Route("api/GetFactorsk")]
        [HttpGet]
        public IActionResult GetFactors(string Ids, string pass)
        {
            if (pass != "bXVnz6M!8g!XNSqvsiXMX")
                return Content("Access is denid");
            var orders = new List<Order>();
            if (Ids != null)
            {
                var ids = Ids
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(_orderService.GetOrdersByIds(ids));
            }
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintOrdersToPdf(stream, orders, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, orders.First().Id.ToString() + ".pdf");
        }
        [HttpGet]
        public IActionResult AgentTokenVerifier([FromQuery] string Token)
        {
            return Json(_customerServiceApi.VerifyToken(Token));
        }

        [Route("api/SendForOptimization")]
        [HttpGet]
        public IActionResult SendForOptimization()
        {
            _optimeApiService.SendForOptimizeRout();
            return Ok();
        }

        [Route("api/RegisterShop")]
        [HttpPost]
        public IActionResult RegisterShopInGateway(RegisterShopModel requestCOD)
        {
            try
            {
                using (PostGatewayServiceManager postGatewayServiceManager = new FrotelServiceLibrary.PostGatewayServiceManager())
                {
                    var loginResult = postGatewayServiceManager.Login(new FrotelServiceLibrary.Input.LoginInput
                    {
                        UserName = "postbar",
                        Password = "2a1234@A!@#$"
                    }).GetAwaiter().GetResult();

                    var now = DateTime.Now;
                    var pcDate = new PersianCalendar();

                    var newshopoutput = postGatewayServiceManager.NewShop(new NewShopInput
                    {
                        Site = "postbar.ir",
                        ManagerName = requestCOD.Fname + " " + requestCOD.Lname,
                        Name = $"requestCOD.{requestCOD.cityName ?? ""}-{requestCOD.Fname} {requestCOD.Lname}",
                        Email = "info@postex.ir",
                        Mobile = requestCOD.Username,
                        Tel = requestCOD.Username,
                        PostalCode = requestCOD.CodePosti,
                        State = requestCOD.state,
                        City = requestCOD.city1,
                        Address = requestCOD.Address,
                        NationalCode = requestCOD.NatinolCode,
                        UserName = requestCOD.Username,
                        FishDateDay = pcDate.GetDayOfMonth(now).ToString(),
                        FishDateMonth = pcDate.GetMonth(now).ToString(),
                        FishDateYear = pcDate.GetYear(now).ToString()
                    }, loginResult.Cookies).GetAwaiter().GetResult();
                    if (!newshopoutput.Successfull)
                    {
                        return Json(new { success = false, message = newshopoutput.Message });
                    }
                    else
                    {
                        return Json(new { success = true });
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { success = false, message = "خطای سیستمی" });
            }
        }
    }
    public class RegisterShopModel
    {
        public int ID { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string cityName { get; set; }
        public string Username { get; set; }
        public string CodePosti { get; set; }
        public int state { get; set; }
        public int city1 { get; set; }
        public string Address { get; set; }
        public string NatinolCode { get; set; }
    }
}
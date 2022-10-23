using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Misc.PhoneLogin.Factories;
using Nop.Plugin.Misc.PhoneLogin.Models;
using Nop.Plugin.Misc.PhoneLogin.Services;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;

using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Nop.Plugin.Misc.PhoneLogin.Controllers
{
	public partial class PhoneLoginController : BasePublicController
	{
		#region Fields
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ISecurityService _securityService;
		private readonly IAddressModelFactory _addressModelFactory;
		private readonly IPhoneLoginModelFactory _customerModelFactory;
		private readonly ICustomAuthenticationService _authenticationService;
		private readonly DateTimeSettings _dateTimeSettings;
		private readonly TaxSettings _taxSettings;
		private readonly ILocalizationService _localizationService;
		private readonly IWorkContext _workContext;
		private readonly IStoreContext _storeContext;
		private readonly ICustomerService _customerService;
		private readonly ICustomerAttributeParser _customerAttributeParser;
		private readonly ICustomerAttributeService _customerAttributeService;
		private readonly IGenericAttributeService _genericAttributeService;
		private readonly ICustomerRegistrationService _customerRegistrationService;
		private readonly ITaxService _taxService;
		private readonly CustomerSettings _customerSettings;
		private readonly AddressSettings _addressSettings;
		private readonly ForumSettings _forumSettings;
		private readonly IAddressService _addressService;
		private readonly ICountryService _countryService;
		private readonly IOrderService _orderService;
		private readonly IPictureService _pictureService;
		private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
		private readonly IShoppingCartService _shoppingCartService;
		private readonly IExternalAuthenticationService _externalAuthenticationService;
		private readonly IWebHelper _webHelper;
		private readonly ICustomerActivityService _customerActivityService;
		private readonly IAddressAttributeParser _addressAttributeParser;
		private readonly IAddressAttributeService _addressAttributeService;
		private readonly IEventPublisher _eventPublisher;
		private readonly MediaSettings _mediaSettings;
		private readonly IWorkflowMessageService _workflowMessageService;
		private readonly LocalizationSettings _localizationSettings;
		private readonly CaptchaSettings _captchaSettings;
		private readonly StoreInformationSettings _storeInformationSettings;
		private readonly PhoneLoginSettings _phoneLoginSettings;
		private readonly ISettingService _settingService;
		private readonly ICustomerServices _customerServices;
		private readonly INotificationService _notificationService;
		private readonly ILogger _logger;
		private readonly IDbContext _dbContext;
		private readonly ICacheManager _cacheManager;

		#endregion

		#region Ctor

		public PhoneLoginController(ISecurityService securityService,
			IHttpContextAccessor httpContextAccessor,
			IAddressModelFactory addressModelFactory,
			IPhoneLoginModelFactory customerModelFactory,
			ICustomAuthenticationService authenticationService,
			DateTimeSettings dateTimeSettings,
			TaxSettings taxSettings,
			ILocalizationService localizationService,
			IWorkContext workContext,
			IStoreContext storeContext,
			ICustomerService customerService,
			ICustomerAttributeParser customerAttributeParser,
			ICustomerAttributeService customerAttributeService,
			IGenericAttributeService genericAttributeService,
			ICustomerRegistrationService customerRegistrationService,
			ITaxService taxService,
			CustomerSettings customerSettings,
			AddressSettings addressSettings,
			ForumSettings forumSettings,
			IAddressService addressService,
			ICountryService countryService,
			IOrderService orderService,
			IPictureService pictureService,
			INewsLetterSubscriptionService newsLetterSubscriptionService,
			IShoppingCartService shoppingCartService,
			IExternalAuthenticationService externalAuthenticationService,
			IWebHelper webHelper,
			ICustomerActivityService customerActivityService,
			IAddressAttributeParser addressAttributeParser,
			IAddressAttributeService addressAttributeService,
			IEventPublisher eventPublisher,
			MediaSettings mediaSettings,
			IWorkflowMessageService workflowMessageService,
			LocalizationSettings localizationSettings,
			CaptchaSettings captchaSettings,
			StoreInformationSettings storeInformationSettings,
			PhoneLoginSettings phoneLoginSettings,
			ISettingService settingService,
			ICustomerServices customerServices,
			ILogger logger,
			INotificationService notificationService,
			IDbContext dbContext,
			ICacheManager cacheManager
			)
		{
			_dbContext = dbContext;
			_cacheManager = cacheManager;
			_httpContextAccessor = httpContextAccessor;
			_notificationService = notificationService;
			_securityService = securityService;
			this._addressModelFactory = addressModelFactory;
			this._customerModelFactory = customerModelFactory;
			this._authenticationService = authenticationService;
			this._dateTimeSettings = dateTimeSettings;
			this._taxSettings = taxSettings;
			this._localizationService = localizationService;
			this._workContext = workContext;
			this._storeContext = storeContext;
			this._customerService = customerService;
			this._customerAttributeParser = customerAttributeParser;
			this._customerAttributeService = customerAttributeService;
			this._genericAttributeService = genericAttributeService;
			this._customerRegistrationService = customerRegistrationService;
			this._taxService = taxService;
			this._customerSettings = customerSettings;
			this._addressSettings = addressSettings;
			this._forumSettings = forumSettings;
			this._addressService = addressService;
			this._countryService = countryService;
			this._orderService = orderService;
			this._pictureService = pictureService;
			this._newsLetterSubscriptionService = newsLetterSubscriptionService;
			this._shoppingCartService = shoppingCartService;
			this._externalAuthenticationService = externalAuthenticationService;
			this._webHelper = webHelper;
			this._customerActivityService = customerActivityService;
			this._addressAttributeParser = addressAttributeParser;
			this._addressAttributeService = addressAttributeService;
			this._eventPublisher = eventPublisher;
			this._mediaSettings = mediaSettings;
			this._workflowMessageService = workflowMessageService;
			this._localizationSettings = localizationSettings;
			this._captchaSettings = captchaSettings;
			this._storeInformationSettings = storeInformationSettings;
			_phoneLoginSettings = phoneLoginSettings;
			this._settingService = settingService;
			this._logger = logger;
			this._customerServices = customerServices;
		}

		#endregion

		#region Login / logout


		//available even when a store is closed
		[CheckAccessClosedStore(true)]
		//available even when navigation is not allowed
		[CheckAccessPublicStore(true)]
		public virtual IActionResult Login(bool? checkoutAsGuest)
		{
			var model = _customerModelFactory.PrepareLoginModel(checkoutAsGuest);
			string host = _httpContextAccessor.HttpContext.Request.Host.Host.ToLower();
			if (host.Contains("postbar") || host.Contains("postbaar"))
				return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/Login.cshtml", model);
			else if (host.Contains("postkhone") || host.Contains("shipito"))
				return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/ptx_Login.cshtml", model);
			return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/ptx_Login.cshtml", model);
		}
		[HttpPost]
		[ValidateCaptcha]
		//available even when a store is closed
		[CheckAccessClosedStore(true)]
		//available even when navigation is not allowed
		[CheckAccessPublicStore(true)]
		public virtual IActionResult Login(Models.LoginModel model, string returnUrl, bool captchaValid)
		{
			//validate CAPTCHA
			if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
			{
				ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
			}

			if (ModelState.IsValid)
			{
				if (_workContext.CurrentCustomer.IsRegistered())
				{
					//Already registered customer. 
					_authenticationService.SignOut();

					//raise logged out event       
					_eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

					//Save a new record
					//   _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
				}

				foreach (var cookie in Request.Cookies.Keys)
				{
					Response.Cookies.Delete(cookie);
				}

				if (_customerSettings.UsernamesEnabled && model.Username != null)
				{
					model.Username = model.Username.Trim();
				}
				var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
				switch (loginResult)
				{
					case CustomerLoginResults.Successful:
						{

							var customer = _customerSettings.UsernamesEnabled
								? _customerService.GetCustomerByUsername(model.Username)
								: _customerService.GetCustomerByEmail(model.Email);
							if (_storeContext.CurrentStore.Id == 5 && IsPostMember(customer.Id))
							{
								_authenticationService.SignOut();
								ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
								break;
							}
							//if (_storeContext.CurrentStore.Id == 5 && IsPostMember(customer.Id))
							//{
							//    _authenticationService.SignOut();
							//    return Redirect("https://postbar.ir/Login");
							//}
							if (_storeContext.CurrentStore.Id == 3
								&& !customer.IsInCustomerRole("Administrators")
								&& CanForceToPostkhone(customer.Id)
								&& !customer.IsInCustomerRole("mini-Administrators")
								&& !customer.IsInCustomerRole("Collector")
								&& !IsPostMember(customer.Id))
							{
								_authenticationService.SignOut();

								return Redirect(_webHelper.GetStoreLocation() + "/Login");
							}

							//migrate shopping cart
							_shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

							//sign in new customer

							string SupperAdminCode = HttpContext.Request.Form["SupperAdminCode"];
							if (!string.IsNullOrEmpty(SupperAdminCode))

							{
								if (!_securityService.IsValidSecurityCode(model.Username, _securityService.GetHashString(SupperAdminCode)))
								{
									_authenticationService.SignOut();
									ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
									break;
								}
								if (SupperAdminCode.Length <= 20)
								{
									//_cacheManager.Set("secCod_" + model.Username, _securityService.GetHashString(SupperAdminCode),0);
									//HttpContext.Session.SetString("secCod", _securityService.GetHashString(SupperAdminCode));
									_authenticationService.SignIn(customer, model.RememberMe, _securityService.GetHashString(SupperAdminCode));

								}

							}
							else
							{
								_authenticationService.SignIn(customer, model.RememberMe);
							}
							//raise event       
							_eventPublisher.Publish(new CustomerLoggedinEvent(customer));
							var attr = _genericAttributeService.GetAttributesForEntity(customer.Id, "Customer");
							if (attr != null && attr.Any())
							{
								var Lst_attr = attr.Where(p => p.Key == "InputSource").ToList();
								_genericAttributeService.DeleteAttributes(Lst_attr);
							}
							//activity log
							_customerActivityService.InsertActivity(customer, "PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"));
							_httpContextAccessor.HttpContext.Session.SetComplexData("MenuAccessList", null);
							if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
								return RedirectToRoute("HomePage");

							return Redirect(returnUrl);
						}
					case CustomerLoginResults.CustomerNotExist:
						ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
						break;
					case CustomerLoginResults.Deleted:
						ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
						break;
					case CustomerLoginResults.NotActive:
						ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
						break;
					case CustomerLoginResults.NotRegistered:
						ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
						break;
					case CustomerLoginResults.LockedOut:
						ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.LockedOut"));
						break;
					case CustomerLoginResults.WrongPassword:
					default:
						ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
						break;
				}
			}

			//If we got this far, something failed, redisplay form
			model = _customerModelFactory.PrepareLoginModel(model.CheckoutAsGuest);
			if (_storeContext.CurrentStore.Id == 5)
				return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/ptx_Login.cshtml", model);
			else
				return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/Login.cshtml", model);
		}

		#endregion

		[AuthorizeAdmin]
		[Area(AreaNames.Admin)]
		[HttpGet]
		public ActionResult Configure()
		{
			var model = new ConfigurationModel
			{
				Enabled = _phoneLoginSettings.Enabled,
				LineNumber = _phoneLoginSettings.LineNumber,
				ApiKey = _phoneLoginSettings.ApiKey
			};

			return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/Configure.cshtml", model);
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
			_phoneLoginSettings.Enabled = model.Enabled;
			_phoneLoginSettings.LineNumber = model.LineNumber;
			_phoneLoginSettings.ApiKey = model.ApiKey;
			_settingService.SaveSetting(_phoneLoginSettings);

			SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

			_settingService.ClearCache();

			return Configure();
		}

		//test

		#region Register


		//available even when navigation is not allowed
		[CheckAccessPublicStore(true)]
		public virtual IActionResult Register()
		{
			//check whether registration is allowed
			if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
				return RedirectToRoute("RegisterResult", new { resultId = (int)Core.Domain.Customers.UserRegistrationType.Disabled });
			var model = new Nop.Plugin.Misc.PhoneLogin.Models.RegisterModel();
			model = _customerModelFactory.PrepareRegisterModel(model, false, setDefaultValues: true);

			string host = _httpContextAccessor.HttpContext.Request.Host.Host.ToLower();
			if (host.Contains("postbar") || host.Contains("postbaar"))
				return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/Register.cshtml", model);
			else if (_httpContextAccessor.HttpContext.Request.Host.Host.ToLower().Contains("shipito"))
				return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/ptx_Register.cshtml", model);
			return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/ptx_Register.cshtml", model);
		}

		[HttpPost]
		[ValidateCaptcha]
		[ValidateHoneypot]
		[PublicAntiForgery]
		//available even when navigation is not allowed
		[CheckAccessPublicStore(true)]
		public virtual IActionResult Register(Nop.Plugin.Misc.PhoneLogin.Models.RegisterModel model, string returnUrl, bool captchaValid, string activationCode)
		{
			try
			{
				Log("register", "-1");
				if (!RetriveActivationCode(activationCode, model.Username))
				{
					ModelState.AddModelError("", "کد تایید وارد شده نامعتبر می باشد");
				}
				if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
				{
					ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
				}
				model.ConfirmPassword = model.Password;
				model.FirstName = "";
				model.LastName = "";

				model.Email = model.Username + (_storeContext.CurrentStore.Id == 5 ? "@postex.ir" : "@Postbar.ir");
				//model.Email = model.Username + "@Postex.ir";
				//model.Email = model.Username + "@Shipito6t.ir";
				HttpContext.Session.Remove("sentCode");
				Log("register", "-2");
				//check whether registration is allowed
				if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
					return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });
				Log("register", "-3");
				if (_workContext.CurrentCustomer.IsRegistered())
				{
					//Already registered customer. 
					_authenticationService.SignOut();

					//raise logged out event       
					_eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

					//Save a new record
					_workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
				}
				Log("register", "-4");
				var customer = _workContext.CurrentCustomer;
				customer.RegisteredInStoreId = _storeContext.CurrentStore.Id;
				string customerAttributesXml = "";
				//custom customer attributes
				var attribute = _customerAttributeService.GetCustomerAttributeById(8);
				customerAttributesXml = _customerAttributeParser.AddCustomerAttribute("",
										 attribute, model.NationalCode.ToString());

				if (model.CodeTpye != null)
				{
					var attribute2 = _customerAttributeService.GetCustomerAttributeById(11);
					customerAttributesXml = _customerAttributeParser.AddCustomerAttribute(customerAttributesXml,
											 attribute2, model.CodeTpye.ToString());
				}
				Log("register", "-5");
				//var customerAttributesXml = ParseCustomCustomerAttributes(model.Form);
				//var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
				//foreach (var error in customerAttributeWarnings)
				//{
				//    ModelState.AddModelError("", error);
				//}

				////validate CAPTCHA

				Log("register", "-6");
				common.Log("ModelState is " + ModelState.IsValid.ToString(), "");
				if (ModelState.IsValid)
				{
					if (_customerSettings.UsernamesEnabled && model.Username != null)
					{
						model.Username = model.Username.Trim();
					}
					Log("register", "1");
					var isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
					var registrationRequest = new CustomerRegistrationRequest(customer,
						model.Email,
						_customerSettings.UsernamesEnabled ? model.Username : model.Email,
						model.Password,
						_customerSettings.DefaultPasswordFormat,
						_storeContext.CurrentStore.Id,
						isApproved);
					Log("register", "2");
					var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
					if (registrationResult.Success)
					{
						Log("register", "3");
						//properties
						if (_dateTimeSettings.AllowCustomersToSetTimeZone)
						{
							_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.TimeZoneId);
						}
						//VAT number
						if (_taxSettings.EuVatEnabled)
						{
							_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumber, model.VatNumber);

							var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out string _, out string vatAddress);
							_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumberStatusId, (int)vatNumberStatus);
							//send VAT number admin notification
							if (!string.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
								_workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
						}
						Log("register", "4");
						//form fields
						if (_customerSettings.GenderEnabled)
							_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
						_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
						_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
						if (_customerSettings.DateOfBirthEnabled)
						{
							var dateOfBirth = model.ParseDateOfBirth();
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
							_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId,
								model.StateProvinceId);
						if (_customerSettings.PhoneEnabled)
							_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
						if (_customerSettings.FaxEnabled)
							_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);
						Log("register", "5");
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
						Log("register", "6");
						//login customer now
						if (isApproved)
							_authenticationService.SignIn(customer, true);
						Log("register", "7");
						//insert default address (if possible)
						var defaultAddress = new Address
						{
							FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName),
							LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName),
							Email = customer.Email,
							Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company),
							CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId) > 0
								? (int?)customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId)
								: null,
							StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId) > 0
								? (int?)customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId)
								: null,
							City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City),
							Address1 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress),
							Address2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2),
							ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode),
							PhoneNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
							FaxNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax),
							CreatedOnUtc = customer.CreatedOnUtc
						};
						Log("register", "8");
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
						Log("register", "9");
						//notifications
						if (_customerSettings.NotifyNewCustomerRegistration)
							_workflowMessageService.SendCustomerRegisteredNotificationMessage(customer,
								_localizationSettings.DefaultAdminLanguageId);
						Log("register", "10");
						//raise event       
						_eventPublisher.Publish(new CustomerRegisteredEvent(customer));
						if (_storeContext.CurrentStore.Id == 5)
						{
							_notificationService._sendSms(customer.Username, $@"کاربر گرامی 
خدمات پست‌درون شهری، بین‌شهری و بین‌المللی را با تخفیف 2 الی 15 درصد تجربه کن.
www.postex.ir");
						}
						else
						{
							_notificationService._sendSms(customer.Username, $@"کاربر گرامی
خدمات پست‌درون شهری، بین‌شهری و بین‌المللی را با تخفیف 2 الی 15 درصد تجربه کن.
www.postbar.ir");
						}
						Log("register", "11");
						switch (_customerSettings.UserRegistrationType)
						{
							case UserRegistrationType.EmailValidation:
								{
									//email validation message
									_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
									_workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);

									//result
									return RedirectToRoute("RegisterResult",
										new { resultId = (int)UserRegistrationType.EmailValidation });
								}
							case UserRegistrationType.AdminApproval:
								{
									return RedirectToRoute("RegisterResult",
										new { resultId = (int)UserRegistrationType.AdminApproval });
								}
							case UserRegistrationType.Standard:
								{
									Log("register", "12");
									//send customer welcome message
									_workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

									return RedirectToRoute("HomePage");
								}
							default:
								{
									return RedirectToRoute("HomePage");
								}
						}
					}
					Log("register", "13");
					//errors
					foreach (var error in registrationResult.Errors)
					{

						if (error.Contains("ست الکترونیک مشخص شده در حال حاضر وجود دارد"))
							ModelState.AddModelError("شماره موبایل وارد شده نامعتبر است و یا قبلا استفاده شده", error);
						else
							ModelState.AddModelError("", error);
					}
					Log("register", "14");
				}
				var t = string.Join("\r\n", ModelState.Values.Where(p => p.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid).SelectMany(p => p.Errors.Select(k => k.ErrorMessage)).ToList());
				ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
				//If we got this far, something failed, redisplay form
				model = _customerModelFactory.PrepareRegisterModel(model, true, customerAttributesXml);
				model.ActivationCode = "";
				Log("register", "15");

			}
			catch (Exception ex)
			{
				LogException(ex);
			}
			if (_storeContext.CurrentStore.Id == 5)
				return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/ptx_Register.cshtml", model);
			else
				return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/Register.cshtml", model);
		}

		[PublicAntiForgery]
		[HttpGet]
		public IActionResult ResendActivationCode(string phone, string CountryCodeNum)
		{
			if (CountryCodeNum == "+98")
			{
				if (!Utility.CheckNumber(phone))
					return Json(new { success = false, message = "شماره موبایل وارد شده نامعتبر می باشد" });
			}
			var customer = _customerService.GetCustomerByUsername(phone);
			if (customer != null)
			{
				return Json(new { success = false, message = "این کاربری قبلا در سامانه ثبت شده" });
			}
			string error = "";
			string ActivationCode = _securityService.SetActivationCode(phone, out error);
			if (string.IsNullOrEmpty(ActivationCode))
			{
				return Json(new { success = false, message = error });
			}
			string SotreName = _storeContext.CurrentStore.Id == 5 ? "کد تایید پستِکس" : "کد تایید پست بار";
			_notificationService.sendSms(ActivationCode + " : " + SotreName, phone);
			return Json(new { success = true, message = "پیام کد تایید ارسال شد", interval = 60 });
		}
		[PublicAntiForgery]
		[HttpGet]
		public bool RetriveActivationCode(string activatationCode, string Username)
		{
			if (string.IsNullOrEmpty(activatationCode))
				return false;
			if (activatationCode.Length != 6)
				return false;
			return _securityService.ValidateActivationCode(Username, activatationCode);
		}

		#endregion

		[HttpGet]

		//available even when navigation is not allowed
		[CheckAccessPublicStore(true)]
		public virtual IActionResult PasswordRecovery()
		{
			var model = new Models.PasswordRecoveryModel();
			string host = _httpContextAccessor.HttpContext.Request.Host.Host.ToLower();
			if (host.Contains("postbar") || host.Contains("postbaar"))
				return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/PasswordRecovery.cshtml", model);
			else if (_httpContextAccessor.HttpContext.Request.Host.Host.ToLower().Contains("postex"))
				return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/Sh_PasswordRecovery.cshtml", model);
			return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/Sh_PasswordRecovery.cshtml", model);
		}

		[HttpPost]
		[PublicAntiForgery]
		[ValidateCaptcha]
		//[FormValueRequired("send-mobile-number")]
		//available even when navigation is not allowed
		[CheckAccessPublicStore(true)]
		public virtual ActionResult PasswordRecovery(Models.PasswordRecoveryModel model, bool captchaValid)
		{
			string host = _httpContextAccessor.HttpContext.Request.Host.Host.ToLower();
			if (!captchaValid)
			{
				model.Result = _captchaSettings.GetWrongCaptchaMessage(_localizationService);
				if (host.Contains("postbar") || host.Contains("postbaar"))
					return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/PasswordRecovery.cshtml", model);
				else
					return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/Sh_PasswordRecovery.cshtml", model);

			}
			if (!ModelState.IsValid)
			{
				if (host.Contains("postbar") || host.Contains("postbaar"))
					return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/PasswordRecovery.cshtml", model);
				else
					return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/Sh_PasswordRecovery.cshtml", model);

			}
			if (!Utility.CheckNumber(model.MobileNumber))
			{
				model.Result = _localizationService.GetResource("Plugins.Misc.PhoneLogin.Password.MobileNumber.Wrong");
			}
			var customer = _customerService.GetCustomerByUsername(model.MobileNumber);
			if (customer != null && customer.Active && !customer.Deleted)
			{
				var newPass = RandomString();
				string msg = "";
				if (_customerServices.ChangePassword(customer, newPass, out msg))
				{
					string SotreName = _storeContext.CurrentStore.Id == 5 ? "پستِکس" : "پست بار";
					_notificationService.sendSms(newPass + " : " + SotreName, model.MobileNumber);

					model.Result = _localizationService.GetResource("Plugins.Misc.PhoneLogin.Password.Sent");
					var loginModel = _customerModelFactory.PrepareLoginModel(false);
					Log(model.MobileNumber + ":" + newPass, "");
					if (host.Contains("postbar") || host.Contains("postbaar"))
						return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/Login.cshtml", loginModel);
					else
						return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/ptx_Login.cshtml", loginModel);


				}
				else
				{
					model.Result = "بازنشانی رمز عبور با مشکل مواجه شد" + " \r\n " + msg;
					var loginModel = _customerModelFactory.PrepareLoginModel(false);
					if (host.Contains("postbar") || host.Contains("postbaar"))
						return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/PasswordRecovery.cshtml", model);
					else
						return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/Sh_PasswordRecovery.cshtml", model);
				}
			}
			else
			{
				model.Result = _localizationService.GetResource("Plugins.Misc.PhoneLogin.MobileNumber.NotFound");
			}

			if (host.Contains("postbar") || host.Contains("postbaar"))
				return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/PasswordRecovery.cshtml", model);
			else
				return View("~/Plugins/Nop.Plugin.Misc.PhoneLogin/Views/Sh_PasswordRecovery.cshtml", model);
		}
		private string RandomString()
		{
			Random rand = new Random();
			const string pool = "1234567890";
			var chars = Enumerable.Range(0, 4)
				.Select(x => pool[rand.Next(0, pool.Length)]);
			return new string(chars.ToArray());
		}
		protected virtual string ParseCustomCustomerAttributes(IFormCollection form)
		{
			if (form == null)
				throw new ArgumentNullException(nameof(form));

			var attributesXml = "";
			var attributes = _customerAttributeService.GetAllCustomerAttributes();
			foreach (var attribute in attributes)
			{
				var controlId = $"customer_attribute_{attribute.Id}";
				switch (attribute.AttributeControlType)
				{
					case AttributeControlType.DropdownList:
					case AttributeControlType.RadioList:
						{
							var ctrlAttributes = form[controlId];
							if (!StringValues.IsNullOrEmpty(ctrlAttributes))
							{
								var selectedAttributeId = int.Parse(ctrlAttributes);
								if (selectedAttributeId > 0)
									attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
										attribute, selectedAttributeId.ToString());
							}
						}
						break;
					case AttributeControlType.Checkboxes:
						{
							var cblAttributes = form[controlId];
							if (!StringValues.IsNullOrEmpty(cblAttributes))
							{
								foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
								)
								{
									var selectedAttributeId = int.Parse(item);
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
							if (!StringValues.IsNullOrEmpty(ctrlAttributes))
							{
								var enteredText = ctrlAttributes.ToString().Trim();
								attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
									attribute, enteredText);
							}
						}
						break;
					case AttributeControlType.Datepicker:
					case AttributeControlType.ColorSquares:
					case AttributeControlType.ImageSquares:
					case AttributeControlType.FileUpload:
					//not supported customer attributes
					default:
						break;
				}
			}

			return attributesXml;
		}

		public void Log(string header, string Message)
		{
			var logger =
				(DefaultLogger)EngineContext.Current
					.Resolve<ILogger>();
			logger.InsertLog(LogLevel.Information, header, Message, null);
		}

		private bool CanForceToPostkhone(int CustomerId)
		{
			string query = $@"DECLARE @count INT = 0
                                SELECT
	                                @count = COUNT(1)
                                FROM
	                                dbo.[Order] AS O
	                                INNER JOIN dbo.Shipment AS S ON S.OrderId = O.Id
                                WHERE
	                                O.CustomerId = {CustomerId}
	                                AND O.OrderStatusId IN (20,30)
	                                AND S.TrackingNumber IS NOT NULL 

                                SET @count =ISNULL(@count,0)

                                IF @count >= 5
	                                SELECT CAST(1 AS BIT)
                                ELSE
	                                SELECT CAST(0 AS BIT)";
			return _dbContext.SqlQuery<bool?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(false);
		}

		private bool IsPostMember(int CustomerId)
		{
			string query = $@"IF EXISTS(	
		                        SELECT TOP(1)
			                           CCRM.Customer_Id
		                        FROM
			                        dbo.Customer_CustomerRole_Mapping AS CCRM
		                        WHERE
			                        CCRM.CustomerRole_Id IN (10,22)
			                        AND CCRM.Customer_Id = {CustomerId})
		                        SELECT CAST(1 AS BIT) IsPostMember
	                        ELSE
		                        SELECT CAST(0 AS BIT) IsPostMember";
			return _dbContext.SqlQuery<bool?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(false);
		}
	}
}

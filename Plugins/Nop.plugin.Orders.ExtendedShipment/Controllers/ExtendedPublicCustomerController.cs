using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Tax;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
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
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Customer;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    public class ExtendedPublicCustomerController : Web.Controllers.CustomerController
    {
        #region field
        private readonly CaptchaSettings _captchaSettings;
        private readonly ILocalizationService _localizationService;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomAuthenticationService _CustomeauthenticationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ISecurityService _securityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region ctor
        public ExtendedPublicCustomerController(
         IHttpContextAccessor httpContextAccessor,
         ISecurityService securityService,
         IAddressModelFactory addressModelFactory,
         ICustomerModelFactory customerModelFactory,
         IAuthenticationService authenticationService,
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
         ICustomAuthenticationService CustomeauthenticationService,
         ICacheManager cacheManager)
         : base(addressModelFactory,
         customerModelFactory,
         authenticationService,
         dateTimeSettings,
         taxSettings,
         localizationService,
         workContext,
         storeContext,
         customerService,
         customerAttributeParser,
         customerAttributeService,
         genericAttributeService,
         customerRegistrationService,
         taxService,
         customerSettings,
         addressSettings,
         forumSettings,
         addressService,
         countryService,
         orderService,
         pictureService,
         newsLetterSubscriptionService,
         shoppingCartService,
         externalAuthenticationService,
         webHelper,
         customerActivityService,
         addressAttributeParser,
         addressAttributeService,
         eventPublisher,
         mediaSettings,
         workflowMessageService,
         localizationSettings,
         captchaSettings,
         storeInformationSettings)
        {
            _CustomeauthenticationService = CustomeauthenticationService;
            _httpContextAccessor = httpContextAccessor;
            _captchaSettings = captchaSettings;
            _localizationService = localizationService;
            _customerSettings = customerSettings;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
            _authenticationService = authenticationService;
            _eventPublisher = eventPublisher;
            _customerModelFactory = customerModelFactory;
            _customerActivityService = customerActivityService;
            _securityService = securityService;
            _genericAttributeService = genericAttributeService;
            _storeInformationSettings = storeInformationSettings;
            _cacheManager = cacheManager;
        }
        #endregion

        [HttpPost]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public override IActionResult Login(LoginModel model, string returnUrl, bool captchaValid)
        {

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

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
                            var customer = _customerSettings.UsernamesEnabled
                                ? _customerService.GetCustomerByUsername(model.Username)
                                : _customerService.GetCustomerByEmail(model.Email);

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
                                    _CustomeauthenticationService.SignIn(customer, model.RememberMe, _securityService.GetHashString(SupperAdminCode));
                                    //HttpContext.Session.SetString("secCod", _securityService.GetHashString(SupperAdminCode));
                                    //_cacheManager.Set("secCod_" + _workContext.CurrentCustomer?.Username, _securityService.GetHashString(SupperAdminCode), 0);
                                }
                            }
                            else
                                _authenticationService.SignIn(customer, model.RememberMe);

                            //raise event       
                            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                            //activity log
                            _customerActivityService.InsertActivity(customer, "PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"));

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
            return View(model);
        }

        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public override IActionResult Logout()
        {
            _httpContextAccessor.HttpContext.Session.SetComplexData("MenuAccessList", null);
            if (_workContext.OriginalCustomerIfImpersonated != null)
            {
                //activity log
                _customerActivityService.InsertActivity(_workContext.OriginalCustomerIfImpersonated,
                    "Impersonation.Finished",
                    _localizationService.GetResource("ActivityLog.Impersonation.Finished.StoreOwner"),
                    _workContext.CurrentCustomer.Email, _workContext.CurrentCustomer.Id);
                _customerActivityService.InsertActivity("Impersonation.Finished",
                    _localizationService.GetResource("ActivityLog.Impersonation.Finished.Customer"),
                    _workContext.OriginalCustomerIfImpersonated.Email, _workContext.OriginalCustomerIfImpersonated.Id);

                //logout impersonated customer
                _genericAttributeService.SaveAttribute<int?>(_workContext.OriginalCustomerIfImpersonated,
                    SystemCustomerAttributeNames.ImpersonatedCustomerId, null);

                //redirect back to customer details page (admin area)
                return this.RedirectToAction("Edit", "Customer",
                    new { id = _workContext.CurrentCustomer.Id, area = AreaNames.Admin });

            }

            //activity log
            _customerActivityService.InsertActivity("PublicStore.Logout", _localizationService.GetResource("ActivityLog.PublicStore.Logout"));
            //_cacheManager.Remove("secCod" + _workContext.CurrentCustomer?.Username);
            //HttpContext.Session.SetString("secCod", "");
            //HttpContext.Session.Remove("secCod");
            //standard logout 
            _authenticationService.SignOut();

            //raise logged out event       
            _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

            //EU Cookie
            if (_storeInformationSettings.DisplayEuCookieLawWarning)
            {
                //the cookie law message should not pop up immediately after logout.
                //otherwise, the user will have to click it again...
                //and thus next visitor will not click it... so violation for that cookie law..
                //the only good solution in this case is to store a temporary variable
                //indicating that the EU cookie popup window should not be displayed on the next page open (after logout redirection to homepage)
                //but it'll be displayed for further page loads
                TempData["nop.IgnoreEuCookieLawWarning"] = true;
            }

            return RedirectToRoute("HomePage");
        }
    }
}

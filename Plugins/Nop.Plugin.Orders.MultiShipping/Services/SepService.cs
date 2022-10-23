using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Services.Authentication;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Models.Customer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public class SepService : ISepService
    {
        #region fileds
        private readonly INotificationService _notificationService;
        private readonly INewCheckout _newCheckout;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly IPermissionService _permissionService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly ISettingService _settingService;
        private readonly Random _rand;
        private readonly ICustomerService _customerService;
        private readonly IAuthenticationService _authenticationService;
        private readonly CustomerSettings _customerSettings;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ISecurityService _securityService;
        private readonly IDbContext _dbContext;
        #endregion

        #region ctor
        public SepService(IEventPublisher eventPublisher
          , IDbContext dbContext
         , IWorkflowMessageService workflowMessageService
         , LocalizationSettings localizationSettings
         , IExtendedShipmentService extendedShipmentService
         , ICustomerActivityService customerActivityService
         , IOrderProcessingService orderProcessingService
         , ILocalizationService localizationService
         , IOrderService orderService
         , IPermissionService permissionService
         , INewCheckout newCheckout
         , IWorkContext workContext
         , OrderSettings orderSettings
         , ISettingService settingService
         , ICustomerService customerService
         , IAuthenticationService authenticationService
         , CustomerSettings customerSettings
         , IStoreContext storeContext
         , ICustomerRegistrationService customerRegistrationService
         , INewsLetterSubscriptionService newsLetterSubscriptionService
         , ISecurityService securityService
         , INotificationService notificationService
         )
        {
            _dbContext = dbContext;
            _securityService = securityService;
            _eventPublisher = eventPublisher;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _customerRegistrationService = customerRegistrationService;
            _storeContext = storeContext;
            _customerSettings = customerSettings;
            _authenticationService = authenticationService;
            _customerService = customerService;
            _rand = new Random();
            _settingService = settingService;
            _extendedShipmentService = extendedShipmentService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _permissionService = permissionService;
            _workContext = workContext;
            _newCheckout = newCheckout;
            _orderSettings = orderSettings;
            _notificationService = notificationService;
        }
        #endregion

        public Customer AuthenticatSepUser(out string msg, string mobile)
        {
            Customer customer = null;
            try
            {
                customer = _customerService.GetCustomerByUsername(mobile);
                if (customer != null)
                {
                    _authenticationService.SignIn(customer, true);
                    msg = "اعتبار سنجی با موفقیت انجام شد";
                    if (!customer.IsRegistered())
                    {
                        customer = Register(mobile, out msg, sendSms: false);
                        if (msg.Contains("Current customer is already registered"))
                        {
                            if (customer == null)
                                customer = _customerService.GetCustomerByUsername(mobile);
                            if (customer != null)
                            {
                                _authenticationService.SignIn(customer, true);
                                msg = "";
                            }
                        }
                    }
                    return customer;
                }
                else
                {
                    customer = Register(mobile, out msg, sendSms: false);
                    if (msg.Contains("Current customer is already registered"))
                    {
                        if (customer == null)
                            customer = _customerService.GetCustomerByUsername(mobile);
                        if (customer != null)
                        {
                            _authenticationService.SignIn(customer, true);
                            msg = "";
                        }
                    }
                    return customer;
                }
            }
            catch (Exception ex)
            {
                msg = "بروز خطای نامشخص. مجددا سعی کنید";
                _newCheckout.LogException(ex);
                return null;
            }
        }
        public Customer AuthenticatItSazUser(out string msg, string mobile)
        {
            Customer customer = null;
            try
            {
                customer = _customerService.GetCustomerByUsername(mobile);
                if (customer != null)
                {
                    _authenticationService.SignIn(customer, true);
                    msg = "اعتبار سنجی با موفقیت انجام شد";
                    if (!customer.IsRegistered())
                    {
                        customer = Register(mobile, out msg, sendSms: false);
                        if (msg.Contains("Current customer is already registered"))
                        {
                            if (customer == null)
                                customer = _customerService.GetCustomerByUsername(mobile);
                            if (customer != null)
                            {
                                _authenticationService.SignIn(customer, true);
                                msg = "";
                            }
                        }
                    }
                    return customer;
                }
                else
                {
                    customer = Register(mobile, out msg, sendSms: true, affilateId: 1223);
                    if (msg.Contains("Current customer is already registered"))
                    {
                        if (customer == null)
                            customer = _customerService.GetCustomerByUsername(mobile);
                        if (customer != null)
                        {
                            _authenticationService.SignIn(customer, true);
                            msg = "";
                        }
                    }
                    return customer;
                }
            }
            catch (Exception ex)
            {
                msg = "بروز خطای نامشخص. مجددا سعی کنید";
                _newCheckout.LogException(ex);
                return null;
            }
        }
        public Customer Register(string mobileNo, out string msg, int affilateId = 0, bool PasswordSameUserName = false, bool sendSms = false)
        {
            RegisterModel model = new RegisterModel();
            model.Username = mobileNo;
            string pass = "";
            if (PasswordSameUserName)
                pass = mobileNo;
            else
                pass = RandomString();
            model.Password = pass;
            model.ConfirmPassword = pass;
            model.FirstName = "";
            model.LastName = "";
            model.Email = model.Username + "@postex.ir";
            //model.Email = model.Username + "@Postex.ir";
            //model.Email = model.Username + "@Shipito6t.ir";

            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
            {
                msg = "درحال حاضر امکان استفاده از این سرویس و جود ندارد";
                return null;
            }
            var customer = _workContext.CurrentCustomer;
            customer.RegisteredInStoreId = _storeContext.CurrentStore.Id;

            if (_customerSettings.UsernamesEnabled && model.Username != null)
            {
                model.Username = model.Username.Trim();
            }

            var isApproved = true;// _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
            var registrationRequest = new CustomerRegistrationRequest(customer,
                model.Email,
                model.Username,
                model.Password,
                _customerSettings.DefaultPasswordFormat,
                _storeContext.CurrentStore.Id,
                isApproved);
            var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
            if (registrationResult.Success)
            {

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

                //login customer now
                if (isApproved)
                    _authenticationService.SignIn(customer, true);

                //notifications
                if (_customerSettings.NotifyNewCustomerRegistration)
                    _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer,
                        _localizationSettings.DefaultAdminLanguageId);

                //raise event       
                _eventPublisher.Publish(new CustomerRegisteredEvent(customer));
                //send customer welcome message
                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);
                if (affilateId > 0)
                {
                    customer.AffiliateId = affilateId;
                    _customerService.UpdateCustomer(customer);
                    if (affilateId == 1149)
                    {
                        var customerRole = _customerService.GetCustomerRoleBySystemName("TwoStepOrder");
                        // var customerRole1 = _customerService.GetCustomerRoleBySystemName("Collector");
                        _customerService.InsertCustomerRole(customerRole);
                        // _customerService.InsertCustomerRole(customerRole1);
                    }
                }
                msg = "";
                if (sendSms)
                {
                    string _msg = "سامانه پستی آنلاین پُستِکس" + " \r\n" +
                     "نام کاربری" + " : " + customer.Username + " \r\n" +
                     "رمز عبور" + " : " + pass + " \r\n" +
                     "https://postex.ir/login";
                    _notificationService._sendSms(customer.Username, _msg);
                }
                return customer;

            }
            msg = "";
            //errors
            foreach (var error in registrationResult.Errors)
            {
                msg += error;
            }
            return null;

        }


        public Customer RegisterForNotCurrentCustomer(string mobileNo, out string msg, int affilateId = 0, bool PasswordSameUserName = false, bool sendSms = false)
        {
            RegisterModel model = new RegisterModel();
            model.Username = mobileNo;
            string pass = "";
            if (PasswordSameUserName)
                pass = mobileNo;
            else
                pass = RandomString();
            model.Password = pass;
            model.ConfirmPassword = pass;
            model.FirstName = "";
            model.LastName = "";
            model.Email = model.Username + "@postex.ir";
            //model.Email = model.Username + "@Postex.ir";
            //model.Email = model.Username + "@Shipito6t.ir";

            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
            {
                msg = "درحال حاضر امکان استفاده از این سرویس و جود ندارد";
                return null;
            }
            var customer = new Customer();
            customer.RegisteredInStoreId = _storeContext.CurrentStore.Id;
            customer.CreatedOnUtc = DateTime.Now;
            customer.LastActivityDateUtc = customer.CreatedOnUtc;

            if (_customerSettings.UsernamesEnabled && model.Username != null)
            {
                model.Username = model.Username.Trim();
            }

            var isApproved = true;// _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
            var registrationRequest = new CustomerRegistrationRequest(customer,
                model.Email,
                model.Username,
                model.Password,
                _customerSettings.DefaultPasswordFormat,
                _storeContext.CurrentStore.Id,
                isApproved);
            var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
            if (registrationResult.Success)
            {

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

                //login customer now
                if (isApproved)
                    _authenticationService.SignIn(customer, true);

                //notifications
                if (_customerSettings.NotifyNewCustomerRegistration)
                    _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer,
                        _localizationSettings.DefaultAdminLanguageId);

                //raise event       
                _eventPublisher.Publish(new CustomerRegisteredEvent(customer));
                //send customer welcome message
                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);
                if (affilateId > 0)
                {
                    customer.AffiliateId = affilateId;
                    _customerService.UpdateCustomer(customer);
                    if (affilateId == 1149)
                    {
                        var customerRole = _customerService.GetCustomerRoleBySystemName("TwoStepOrder");
                        // var customerRole1 = _customerService.GetCustomerRoleBySystemName("Collector");
                        _customerService.InsertCustomerRole(customerRole);
                        // _customerService.InsertCustomerRole(customerRole1);
                    }
                }
                msg = "";
                if (sendSms)
                {
                    string _msg = "سامانه پستی آنلاین پُستِکس" + " \r\n" +
                     "نام کاربری" + " : " + customer.Username + " \r\n" +
                     "رمز عبور" + " : " + pass + " \r\n" +
                     "https://postex.ir/login";
                    _notificationService._sendSms(customer.Username, _msg);
                }
                return customer;

            }
            msg = "";
            //errors
            foreach (var error in registrationResult.Errors)
            {
                msg += error;
            }
            return null;

        }


        private string RandomString()
        {
            Random rand = new Random();
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, 8)
                .Select(x => pool[rand.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }
        public bool IsValidCustomer(Customer customer)
        {
            if (customer == null || !customer.IsRegistered() || customer.IsGuest() || customer.Username == null)
            {
                _newCheckout.Log("کاربر وارد شده از آپ", "مهمان:" + customer.IsGuest() + "& ثبت نام شده:" + customer.IsRegistered());
                return false;
            }
            return true;
        }

    }
}

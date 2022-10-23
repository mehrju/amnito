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
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public class ApService : IApService
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
        public ApService(IEventPublisher eventPublisher
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

        public Customer AuthenticatApUser(out string msg, string authToken)
        {
            Customer customer = null;
            try
            {
                string ApiKey = "j5ic5p3nhpua9aq4big5jl76ga";//_settingService.GetSettingByKey<string>("asanpardakhtsettings.key", "", 3);
                HostRequestData2Model _hostRequest = new HostRequestData2Model()
                {
                    hi = 1199,
                    htran = _rand.Next(1, 90000000),//باید عوض شود
                    htime = _newCheckout.getTimeSpan(),
                    hop = 998,
                    hkey = ApiKey,
                    atkn = authToken
                };
                string hostRequest = JsonConvert.SerializeObject(_hostRequest).Trim();
                string signedData = "1#1#" + _securityService.SignData(hostRequest);
                var data = new { hreq = hostRequest, hsign = signedData, ver = "1.1.0" };
                string JsonData = JsonConvert.SerializeObject(data).Trim();
                _newCheckout.Log("اطلاعات ارسالی به آپ", JsonData);
                string result = "";
                //string url = "https://91.232.66.65:7002/sdk/auth/1199/1";
                string url = "https://apms.asanpardakht.net/sdk/auth/1199/1";
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "text/plain; charset=utf-8";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var json = JsonData;
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                using (var response = httpWebRequest.GetResponse() as HttpWebResponse)
                {
                    if (httpWebRequest.HaveResponse && response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            result = reader.ReadToEnd();
                        }
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            HostResponceBoundel respData = JsonConvert.DeserializeObject<HostResponceBoundel>(result);
                            HostResponseModel t = JsonConvert.DeserializeObject<HostResponseModel>(respData.hresp);
                            _newCheckout.Log("پاسخ آپ در احراز هویت", result);
                            if (t.st == 0)
                            {
                                string mobile = t.mo;
                                if (!string.IsNullOrEmpty(mobile))
                                {
                                    customer = _customerService.GetCustomerByUsername(mobile);
                                    if (customer != null)
                                    {
                                        _authenticationService.SignIn(customer, true);
                                        msg = "اعتبار سنجی با موفقیت انجام شد";
                                        if (!customer.IsRegistered())
                                        {
                                            return Register(mobile, out msg);
                                        }
                                        return customer;
                                    }
                                    else
                                    {
                                        return Register(mobile, out msg);
                                    }
                                }
                                else
                                {
                                    _newCheckout.Log("شماره موبایل کاربر آپ دریافت نشد", t.st.ToString());
                                    msg = "در حال حاضر اعتبار سنجی شما در این سرویس امکان پذیر نمی باشد. مجددا سعی کنید";
                                    return null;
                                }

                            }
                            else
                            {
                                _newCheckout.Log("کد خطا در زمان ورود کاربر از اپلیکیشن آپ", t.ToString());
                                msg = "در حال حاضر اعتبار سنجی شما در این سرویس امکان پذیر نمی باشد. مجددا سعی کنید";
                                return null;
                                //کد خطا برگردانده نشده
                            }
                        }
                        else if (((int)response.StatusCode).ToString().StartsWith("40"))
                        {
                            msg = "اطلاعات ارسالی به سرور نامعتبر می باشد. مجددا سعی کنید";
                            return null;
                        }
                        else if (((int)response.StatusCode).ToString().StartsWith("50"))
                        {
                            msg = "بروز خطای نامشخص. مجددا سعی کنید";
                            return null;
                        }
                    }

                    msg = "بروز خطای نامشخص. مجددا سعی کنید";
                    return null;
                }
            }
            catch (Exception ex)
            {
                msg = "بروز خطای نامشخص. مجددا سعی کنید";
                _newCheckout.LogException(ex);
                return null;
            }
        }
        public Customer AuthHyperJet(string mobile, out string msg, int affilateId = 0)
        {
            msg = "";
            if (_workContext.CurrentCustomer.IsRegistered())
            {
                //Already registered customer. 
                _authenticationService.SignOut();

                //raise logged out event       
                _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

                //Save a new record
                _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
            }
            var customer = _customerService.GetCustomerByUsername(mobile);
            if (customer != null)
            {

                if (!customer.IsRegistered())
                {
                    msg = "ثبت نام با موفقیت انجام شد";
                    return Register(mobile, out msg, affilateId, sendSms: true);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return Register(mobile, out msg, affilateId, sendSms: true);
            }
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
        public Customer RegisterUnknown(string mobileNo, out string msg, int affilateId = 0, bool PasswordSameUserName = false, bool sendSms = false)
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
            var customer = _customerService.InsertGuestCustomer();
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

        private string RandomString()
        {
            Random rand = new Random();
            const string pool = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var chars = Enumerable.Range(0, 8)
                .Select(x => pool[rand.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }

        public _paymentRequest CreatePaymentRequest(int orderId, out string error)
        {
            try
            {
                var order = _orderService.GetOrderById(orderId);
                int orderTotal = Convert.ToInt32(order.OrderTotal);
                //if (order.CustomerId == 273739)
                //    orderTotal = 10000;
                int ApStock = GetTashimValue(orderId);
                int myStock = orderTotal - ApStock;
                string ApiKey = "j5ic5p3nhpua9aq4big5jl76ga";//_settingService.GetSettingByKey<string>("asanpardakhtsettings.key", "", 3);
                HostRequestData2Model _hostRequest = new HostRequestData2Model()
                {
                    hi = 1199,
                    htran = _rand.Next(1, 90000000),//باید عوض شود
                    htime = _newCheckout.getTimeSpan(),
                    hop = 209,
                    hkey = ApiKey,
                    ao = orderTotal,
                    merch = "3946774",
                    //iban = $"IR650560956102002669518001:{myStock},IR410560087381001465272001:{ApStock}",//,IR410560087381001465272001:{0}",//iban1:amoun1,iban2:amount2#specifiedIBan    مربوط به تسهیم
                    iban = $"IR410560087381001465272001:{orderTotal}",//,IR410560087381001465272001:{0}",//iban1:amoun1,iban2:amount2#specifiedIBan    مربوط به تسهیم
                    pid = "" //شناسه پرداخت
                };
                var hostRequest = _hostRequest;// 
                string Str_hostRequest = JsonConvert.SerializeObject(_hostRequest).Trim();
                string signedData = "1#1#" + _securityService.SignData(Str_hostRequest);
                var HostRequestData = new _HostRequestData()
                {
                    hsign = signedData,
                    hreq = Str_hostRequest,
                    ver = "1.1.0"
                };
                _paymentRequest paymentRequest = new _paymentRequest()
                {
                    Amount = orderTotal,
                    Cardnumber = "",
                    DescriptionEn = "For postal and Transportation services with order number " + order.Id,
                    DescriptionFa = "بابت خدمات پستی و حمل و نقل با شماره سفارش " + order.Id,
                    HostRequestData = HostRequestData,
                    PaymentId = orderId,
                    ServerData = "",
                    TitleEn = "Payment for Postal and Transportation services",
                    TitleFa = "پرداخت خدمات پستی و حمل و نقل"
                };
                error = "";
                return paymentRequest;
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                error = "در حال حاضر امکان پرداخت وجود ندارد، با واحد پشتیبانی تماس بگیرید";
                return null;
            }
        }
        public _paymentRequest CreatePaymentRequestForCatoon(int orderId, out string error)
        {
            try
            {
                var order = _orderService.GetOrderById(orderId);
                int orderTotal = Convert.ToInt32(order.OrderTotal);
                //if (order.CustomerId == 273739)
                //    orderTotal = 10000;
                string ApiKey = "j5ic5p3nhpua9aq4big5jl76ga";//_settingService.GetSettingByKey<string>("asanpardakhtsettings.key", "", 3);
                HostRequestData2Model _hostRequest = new HostRequestData2Model()
                {
                    hi = 1199,
                    htran = _rand.Next(1, 90000000),//باید عوض شود
                    htime = _newCheckout.getTimeSpan(),
                    hop = 209,
                    hkey = ApiKey,
                    ao = orderTotal,
                    merch = "3946774",
                    //iban = $"IR650560956102002669518001:{myStock},IR410560087381001465272001:{ApStock}",//,IR410560087381001465272001:{0}",//iban1:amoun1,iban2:amount2#specifiedIBan    مربوط به تسهیم
                    iban = $"IR410560087381001465272001:{orderTotal}",//,IR410560087381001465272001:{0}",//iban1:amoun1,iban2:amount2#specifiedIBan    مربوط به تسهیم
                    pid = "" //شناسه پرداخت
                };
                var hostRequest = _hostRequest;// 
                string Str_hostRequest = JsonConvert.SerializeObject(_hostRequest).Trim();
                string signedData = "1#1#" + _securityService.SignData(Str_hostRequest);
                var HostRequestData = new _HostRequestData()
                {
                    hsign = signedData,
                    hreq = Str_hostRequest,
                    ver = "1.1.0"
                };
                _paymentRequest paymentRequest = new _paymentRequest()
                {
                    Amount = orderTotal,
                    Cardnumber = "",
                    DescriptionEn = "For cartons and postal envelopes with order number " + order.Id,
                    DescriptionFa = "بابت خرید کارتن و لفاف پستی با شماره سفارش " + order.Id,
                    HostRequestData = HostRequestData,
                    PaymentId = orderId,
                    ServerData = "",
                    TitleEn = "Payment for cartons and postal envelopes",
                    TitleFa = "پرداخت کارتن و لفاف پستی"
                };
                error = "";
                return paymentRequest;
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                error = "در حال حاضر امکان پرداخت وجود ندارد، با واحد پشتیبانی تماس بگیرید";
                return null;
            }
        }
        public bool VarifyPayment(ApHostResponce model, out string error)
        {
            Order order = null;
            if (model == null)
            {
                error = "نتیجه پرداخت نامشخص. با پشتیبانی تماس بگیرید";
                return false;
            }
            if (model.paymentId > 0)
            {
                order = _orderService.GetOrderById(model.paymentId);
                if (order == null)
                {
                    error = "نتیجه سفارش نامشخص. با پشتیبانی تماس بگیرید";
                    return false;
                }
            }
            if (!(model._host_response != null && model._host_response.st == 0))
            {
                error = getMessageFromStatusCode(model._host_response != null ? model._host_response.st : model.status_code);
                _newCheckout.InsertOrderNote(error, order.Id);
                return false;
            }
            string ApiKey = "j5ic5p3nhpua9aq4big5jl76ga";//_settingService.GetSettingByKey<string>("asanpardakhtsettings.key", "", 3);
                                                         // string url = "https://91.232.66.65:7002/sdk/w10/1199/1";
            string url = "https://apms.asanpardakht.net/sdk/w10/1199/1";

            HostRequestData2Model _hostRequest = new HostRequestData2Model()
            {
                hi = 1199,
                htran = model._host_response.htran.Value,//باید عوض شود
                htime = model._host_response.htime,
                hop = 2001,
                hkey = ApiKey,
                ao = model._host_response.ao,
                stime = _newCheckout.getTimeSpan(),
                utran = long.Parse(model.unique_tran_id),
                stkn = model._host_response.stkn
            };
            string hostRequest = JsonConvert.SerializeObject(_hostRequest).Trim();
            string signedData = "1#1#" + _securityService.SignData(hostRequest);
            var data = new { hreq = hostRequest, hsign = signedData, ver = "1.1.0" };
            string JsonData = JsonConvert.SerializeObject(data).Trim();
            _newCheckout.Log("verify اطلاعات ارسالی به آپ جهت", JsonData);
            string result = "";
            System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "text/plain; charset=utf-8";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = JsonData;
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            using (var response = httpWebRequest.GetResponse() as HttpWebResponse)
            {
                if (httpWebRequest.HaveResponse && response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        HostResponceBoundel respData = JsonConvert.DeserializeObject<HostResponceBoundel>(result);
                        HostResponseModel t = JsonConvert.DeserializeObject<HostResponseModel>(respData.hresp);
                        _newCheckout.Log("verify پاسخ آپ در", result);
                        if (t.st == 0)
                        {
                            string SettlError = "";
                            bool settelReuslt = SettlePayment(t, out SettlError);
                            error = "";
                            return true;
                        }
                        else
                        {
                            string reversError = "";
                            var reversresult = ReversePayment(t, out reversError);
                            error = getMessageFromStatusCode(t.st);
                            _newCheckout.InsertOrderNote(error, order.Id);
                            _newCheckout.Log(error, respData.hresp ?? "");
                            return false;
                            //کد خطا برگردانده نشده
                        }
                    }
                    else if (((int)response.StatusCode).ToString().StartsWith("40") || ((int)response.StatusCode).ToString().StartsWith("50"))
                    {
                        error = "تایید پرداخت انجام نشد، با پشتیبانی تماس بگیرید";
                        _newCheckout.InsertOrderNote("Responce code in verfiy Ap Order payment is:" + ((int)response.StatusCode).ToString(), order.Id);
                        return false;
                    }

                }
                error = "تایید پرداخت انجام نشد، با پشتیبانی تماس بگیرید";
                _newCheckout.InsertOrderNote("نامعتبر میباشد verify پاسخ دریافتی از سرور در درخواست ", order.Id);
                return false;
            }
        }
        public int GetTashimValue(int OrderId)
        {
            string Query = $@"SELECT TOP(1)
	                            TCPOI.TashimValue
                            FROM
	                            dbo.Tb_CalcPriceOrderItem AS TCPOI
	                            INNER JOIN dbo.OrderItem AS OI ON OI.Id = TCPOI.OrderItemId
                            WHERE
	                            OI.OrderId = {OrderId}";
            return _dbContext.SqlQuery<int?>(Query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
        }


        public bool SettlePayment(HostResponseModel model, out string SettlError)
        {
            SettlError = "";
            return true;
        }
        public bool ReversePayment(HostResponseModel model, out string reversError)
        {
            reversError = "";
            return true;
        }
        private string getMessageFromStatusCode(int errorCode)
        {
            return ApError.GetErrorMsg(errorCode.ToString());
        }

        public bool AuthenticateMyIrancell(string token)
        {
            string phoneNumber = getPhoneNumberFromMyIrancell(token);
            common.Log("شماره تشخیص داده شده ایرانسل", phoneNumber);
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                if (phoneNumber.StartsWith("98"))
                    phoneNumber = phoneNumber.Replace("98", "0");
                else if (phoneNumber.StartsWith("+98"))
                    phoneNumber = phoneNumber.Replace("+98", "0");
                else if (phoneNumber.StartsWith("0098"))
                    phoneNumber = phoneNumber.Replace("0098", "0");
                else if (!phoneNumber.StartsWith("0"))
                    phoneNumber = "0" + phoneNumber;
                var customer = _customerService.GetCustomerByUsername(phoneNumber);
                string Message = "";
                customer = _customerService.GetCustomerByUsername(phoneNumber);
                if (customer != null)
                {
                    _authenticationService.SignIn(customer, true);
                    Message = "اعتبار سنجی با موفقیت انجام شد";
                    if (!customer.IsRegistered())
                    {
                        Register(phoneNumber, out Message);
                    }
                    return true;
                }
                else
                {
                    Register(phoneNumber, out Message,affilateId: 1549);
                    return true;
                }

            }
            return false;

        }
        private string getPhoneNumberFromMyIrancell(string _token)
        {
            common.Log("توکن دریافتی از مای ایرانسل", _token);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient("https://Services.postex.ir/api");
            //var client = new RestClient("http://localhost:5000/api");
            var request = new RestRequest("/IrancellValidation", Method.GET);
            request.AddParameter("token", _token);
            request.AddParameter("Hash", "0");
            IrancellValidationResult result = new IrancellValidationResult();
            try
            {
                var reposnce = client.Execute(request);
                if (reposnce.StatusCode == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrEmpty(reposnce.Content))
                    {
                        result = Newtonsoft.Json.JsonConvert.DeserializeObject<IrancellValidationResult>(reposnce.Content);
                        if (result == null || !result.success)
                        {
                            common.Log("خطا در زمان دریافت شماره موبایل از ایرانسل", result == null ? "" : result.errorMessage);
                            return "";
                        }
                        return result.mobile;
                    }else
                    {
                        common.Log("بدنه بازگشتی از سرویس اعتبار سنجی ایرانسل خالی است", "246");
                        return "";
                    }
                }
                else
                {
                    common.Log("کد استاتوس برگشتی از سرویس ایرانسل 246", reposnce.StatusCode.ToString());
                    return "";
                }
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                return "";
            }
        }
    }
    public class IrancellValidationResult
    {
        public string mobile { get; set; }
        public bool success { get; set; }
        public string errorMessage { get; set; }
    }
    public class _paymentRequest
    {
        public int Amount { get; set; }
        public string Cardnumber { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionFa { get; set; }
        public _HostRequestData HostRequestData { get; set; }
        public int PaymentChanelID { get { return 2; } }
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        public int PaymentId { get; set; }
        /// <summary>
        /// امکان پرداخت از طریق امتیاز ها وجود داشته باشد یا خیر
        /// </summary>
        public bool PointEnabled { get { return false; } }
        public string ServerData { get; set; }
        public string TitleEn { get; set; }
        public string TitleFa { get; set; }
    }
    public class _HostRequestData
    {
        public string hsign { get; set; }
        public string ver { get; set; }
        public string hreq { get; set; }
    }
    public static class ApError
    {
        private static readonly Dictionary<int, string> _errorDict = new Dictionary<int, string>
        {
            {0, "درخواست با موفقیت انجام شد."},
            {2020, "انصراف کاربر"},
            {2021, " اتمام فرصت کاربر جهت انجام تراکنش )Timeout "},
            {2023, @"خطا در رمزگشایی اطالعات ذخیره شده در SDK .Token Security ارسالی از اپلیکیشن میزبان
اشتباه است. در صورت تغییر حساب کاربری یا تغییر Token Security برنامه میزبان میبایست
درخواست (102 (register ارسال کند"},
            {1102, @"یاز به ثبت برنامه )برنامه میزبان می بایست درخواست (102 (register ارسال کند.("},
            {1103, "برنامه یا موبایل غیر فعال شده است."},
            {1001, @"عدم دریافت پاسخ در SDK) 1001 (و یا نامشخص شدن پاسخ )1201)
در درخواست خرید نتیجه تراکنش ناموفق بوده و پذیرنده باید اصالحیه ارسال کند.
در درخواست های Reverse/Settle در صورت دریافت این کد، درخواست می بایست تکرار
شود."},
            {1201, @"عدم دریافت پاسخ در SDK) 1001 (و یا نامشخص شدن پاسخ )1201)
در درخواست خرید نتیجه تراکنش ناموفق بوده و پذیرنده باید اصالحیه ارسال کند.
در درخواست های Reverse/Settle در صورت دریافت این کد، درخواست می بایست تکرار
شود."},
            {1002, "ناموفق )عمومی("},
            {1098, " * تراکنش پذیرنده تکراری است. "},
            {1099, "اختالف زمان ارسالی پذیرنده در درخواست با زمان فعلی سرور بیش از حد مجاز است."},
            {1100, "درخواست پذیرنده معتبر نمی باشد."},
            {1127, "دستگاه الزامات امنیتی مورد نیاز برای نصب یا استفاده از SDK را دارا نیست"},
            {1128, "دستگاه الزامات غیر امنیتی مورد نیاز برای نصب یا استفاده از SDK را دارا نیست"},
            {1135, "کد پذیرنده معتبر نیست."},
            {1105, @"نسخه SDK قدیمی بوده و به روز رسانی آن الزامی است. در این شرایط در صورتیکه میزبان نسخه
دیگری حاوی SDK به روز شده دارد به کاربر پیغام می دهد برای استفاده از این درگاه پرداخت
برنامه خود را به روز کند. در غیر اینصورت امکان پرداخت با SDK تا به روز رسانی بعدی برنامه
وجود نخواهد داشت."},
            {1200, " * ناموفق – )علت نامشخص("},
            {2101, "تراکنش اصلی یافت نشد."},
            {2102, "تراکنش قبلا Verify شده است."},
            {2103, "تراکنش قبلا Settle شده است."},
            {2104, "تراکنش قبلا Reverse شده است"},
            {2105, "تراکنش اصلی ناموفق می باشد"},
            {2106, " تراکنش قبلا Verify نشده است."},
            {1190, " توکن ارسالی معتبر نمی باشد"},
            {1191, "وکن ارسالی منقضی شده است."},

        };

        public static string GetErrorMsg(string key)
        {

            string value;
            _errorDict.TryGetValue(int.Parse(key), out value);
            return string.IsNullOrEmpty(value) ? "" : value;
        }

    }
}

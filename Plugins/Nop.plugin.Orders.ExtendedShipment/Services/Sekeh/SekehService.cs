using Microsoft.IdentityModel.Tokens;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Authentication;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Messages;
using Nop.Web.Models.Customer;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class SekehService : ISekehService
    {
        private readonly INotificationService _notificationService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IAuthenticationService _authenticationService;
        private readonly CustomerSettings _customerSettings;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IEventPublisher _eventPublisher;
        private readonly string Key = "yuEkf87!vyT8%o#hlV9RA";
        private readonly string Issuer = "https://postex.ir";
        private readonly string Audience = "http://myaudience.com";

        public SekehService(INotificationService notificationService,
                            IWorkContext workContext,
                            ICustomerService customerService,
                            IAuthenticationService authenticationService,
                            CustomerSettings customerSettings,
                            IStoreContext storeContext,
                            ICustomerRegistrationService customerRegistrationService,
                            INewsLetterSubscriptionService newsLetterSubscriptionService,
                            LocalizationSettings localizationSettings,
                            IWorkflowMessageService workflowMessageService,
                            IEventPublisher eventPublisher)
        {
            _notificationService = notificationService;
            _workContext = workContext;
            _customerService = customerService;
            _authenticationService = authenticationService;
            _customerSettings = customerSettings;
            _storeContext = storeContext;
            _customerRegistrationService = customerRegistrationService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _localizationSettings = localizationSettings;
            _workflowMessageService = workflowMessageService;
            _eventPublisher = eventPublisher;
        }

        public bool Authenticate(string HashPassword)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var password = Convert.ToBase64String(sha1.ComputeHash(Encoding.ASCII.GetBytes(Key)));
            if (HashPassword.Equals(password))
            {
                return true;
            }
            return false;
        }
        public SekehOutputModel GetToken(SekehInputModel inputModel)
        {
            var SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, inputModel.MobileNumber),
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = Issuer,
                Audience = Audience,
                SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var outputModel = new SekehOutputModel
            {
                Token = tokenHandler.WriteToken(token)
            };
            return outputModel;
        }
        public string EncryptData(string MobileNumber)
        {
            var SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, MobileNumber),
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = Issuer,
                Audience = Audience,
                SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string VerifyToken(string Token)
        {
            string Username;
            var SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                    IssuerSigningKey = SecurityKey
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                Username = jwtToken.Claims.First(x => x.Type == "nameid").Value;
            }
            catch (Exception ex)
            {
                return null;
            }
            return Username;
        }

        public bool IsValidCustomer(Customer customer)
        {
            if (customer == null || !customer.IsRegistered() || customer.IsGuest() || customer.Username == null)
                return false;
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

        private string RandomString()
        {
            Random rand = new Random();
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, 8)
                .Select(x => pool[rand.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }
    }
}

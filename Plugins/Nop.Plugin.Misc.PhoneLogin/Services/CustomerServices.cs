using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Security;
using System;

namespace Nop.Plugin.Misc.PhoneLogin.Services
{
    public class CustomerServices : ICustomerServices
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly IEncryptionService _encryptionService;
        private readonly ILocalizationService _localizationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly CustomerSettings _customerSettings;
        private readonly IDbContext _dbContext;


        public CustomerServices(ICustomerService customerService,
           IEncryptionService encryptionService,
           ILocalizationService localizationService,
           IEventPublisher eventPublisher,
           CustomerSettings customerSettings,
           ICustomerRegistrationService customerRegistrationService,
           IDbContext dbContext)
        {
            _customerRegistrationService = customerRegistrationService;
            this._customerService = customerService;
            this._encryptionService = encryptionService;
            this._localizationService = localizationService;
            this._eventPublisher = eventPublisher;
            this._customerSettings = customerSettings;
            _dbContext = dbContext;
        }

        public bool ChangePassword(Customer customer, string newPassword, out string msg)
        {
            try
            {
                //at this point request is valid
                var customerPassword = new CustomerPassword
                {
                    Customer = customer,
                    PasswordFormat = _customerSettings.DefaultPasswordFormat,
                    CreatedOnUtc = DateTime.UtcNow
                };
                switch (_customerSettings.DefaultPasswordFormat)
                {
                    case PasswordFormat.Clear:
                        customerPassword.Password = newPassword;
                        break;
                    case PasswordFormat.Encrypted:
                        customerPassword.Password = _encryptionService.EncryptText(newPassword);
                        break;
                    case PasswordFormat.Hashed:
                        {
                            var saltKey = _encryptionService.CreateSaltKey(5);
                            customerPassword.PasswordSalt = saltKey;
                            customerPassword.Password = _encryptionService.CreatePasswordHash(newPassword, saltKey, _customerSettings.HashedPasswordFormat);
                        }
                        break;
                }
                _customerService.InsertCustomerPassword(customerPassword);

                //publish event
                _eventPublisher.Publish(new CustomerPasswordChangedEvent(customerPassword));
                msg = "پسورد با موفقیت تغییر کرد";
                return true;
            }
            catch (Exception ex)
            {
                msg = "خطا در زمان بازیابی و تغییر  پسورد. مجددا سعی کنید";
                return false;
            }
        }
       
    }
}

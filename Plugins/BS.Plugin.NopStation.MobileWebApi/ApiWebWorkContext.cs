using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Nop.Core.Domain.Vendors;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using Nop.Services.Vendors;
using Nop.Web.Framework;

namespace BS.Plugin.NopStation.MobileWebApi
{
    public class ApiWebWorkContext : WebWorkContext
    {
        private Customer _cachedCustomer;
        private Customer _originalCustomerIfImpersonated;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerService _customerService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomerServiceApi _customerServiceApi;


        private readonly CurrencySettings _currencySettings;

        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILanguageService _languageService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;

        private readonly IVendorService _vendorService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly TaxSettings _taxSettings;


       

        public ApiWebWorkContext(IHttpContextAccessor httpContextAccessor,
            ICustomerService customerService,
            IVendorService vendorService,
            IStoreContext storeContext,
            IAuthenticationService authenticationService,
            ILanguageService languageService,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            TaxSettings taxSettings,
            CurrencySettings currencySettings,
            LocalizationSettings localizationSettings,
            IUserAgentHelper userAgentHelper,
            IStoreMappingService storeMappingService,
            ICustomerServiceApi customerServiceApi, 
            CurrencySettings currencySettings1, 
            ICurrencyService currencyService1, 
            IGenericAttributeService genericAttributeService1, 
            ILanguageService languageService1, 
            IStoreContext storeContext1, 
            IStoreMappingService storeMappingService1,
            IVendorService vendorService1, 
            LocalizationSettings localizationSettings1, 
            TaxSettings taxSettings1
          )
            : base(
                    httpContextAccessor,
                    currencySettings,
                   authenticationService,
                   currencyService,
                   customerService,
                   genericAttributeService,
                   languageService,
                   storeContext,
                   storeMappingService,
                   userAgentHelper,
                   vendorService,
                  localizationSettings,
                  taxSettings)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._customerService = customerService;
            this._userAgentHelper = userAgentHelper;
            this._authenticationService = authenticationService;
            this._customerServiceApi = customerServiceApi;
            _currencySettings = currencySettings1;
            _currencyService = currencyService1;
            _genericAttributeService = genericAttributeService1;
            _languageService = languageService1;
            _storeContext = storeContext1;
            _storeMappingService = storeMappingService1;
            _vendorService = vendorService1;
            _localizationSettings = localizationSettings1;
            _taxSettings = taxSettings1;
       
        }


        public Customer GetCustomerFromToken()
        {
            //IEnumerable<string> headerValues;
            try
            {
                int Id = 0;
                var secretKey = Constant.SecretKey;
                //  var keyFound = _httpContextAccessor.HttpContext.Request.Headers.GetValues(Constant.TokenName);
                
                    
                 _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(Constant.TokenName, out var keyFound);
                var token = keyFound.FirstOrDefault();
                var load = JWT.JsonWebToken.DecodeToObject(token, secretKey) as IDictionary<string, object>;
                if (load != null)
                {
                    Id = Convert.ToInt32(load[Constant.CustomerIdName]);
                    return _customerService.GetCustomerById(Id);
                }

            }
            catch
            {
                return null;
            }
            return null;
        }

        public Customer GetCustomerFromDeviceId()
        {
            //IEnumerable<string> headerValues;
            int Id = 0;
            var secretKey = Constant.SecretKey;
            // var keyFound = _httpContextAccessor.HttpContext.Request.Headers.GetValues(Constant.DeviceIdName);
            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(Constant.DeviceIdName, out var keyFound);
            var deviceId = keyFound.FirstOrDefault();
            var getGuidForDevice = HelperExtension.GetGuid(deviceId);
            Customer customer = _customerService.GetCustomerByGuid(getGuidForDevice);
            if (customer != null)
            {
                return customer;
            }
            else
            {
                customer = _customerServiceApi.InsertGuestCustomerByMobile(deviceId);
                return customer;
            }

            return null;
        }
        public override Customer CurrentCustomer
        {
            get
            {
                if (_cachedCustomer != null)
                    return _cachedCustomer;

                Customer customer = null;
                //check whether request is made by a background (schedule) task
                if (_httpContextAccessor.HttpContext == null ||
                    _httpContextAccessor.HttpContext.Request.Path.Equals(new PathString($"/{TaskManager.ScheduleTaskPath}"), StringComparison.InvariantCultureIgnoreCase))
                {
                    //in this case return built-in customer record for background task
                    customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.BackgroundTask);
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //check whether request is made by a search engine, in this case return built-in customer record for search engines
                    if (_userAgentHelper.IsSearchEngine())
                        customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.SearchEngine);
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //try to get registered user
                    customer = _authenticationService.GetAuthenticatedCustomer();
                }


                //load mobile customer
               

                if (_httpContextAccessor.HttpContext?.Request.GetDisplayUrl() != null && _httpContextAccessor.HttpContext.Request.GetDisplayUrl().Contains("/api/"))// _httpContextAccessor.HttpContext.Request.GetDisplayUrl().StartsWith("/api/"))
                {
                    //check whether request is made by a background task
                    //in this case return built-in customer record for background task
                    if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(Constant.TokenName) )
                    {
                        customer = GetCustomerFromToken();
                        if (customer != null)
                        {
                            _cachedCustomer = customer;
                            return customer;
                        }
                    }
                    else if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(Constant.DeviceIdName))
                    {
                        customer = GetCustomerFromDeviceId();
                        if (customer != null)
                        {
                            _cachedCustomer = customer;
                            return customer;
                        }
                    }
                }



                if (customer != null && !customer.Deleted && customer.Active && !customer.RequireReLogin)
                {
                    //get impersonate user if required
                    var impersonatedCustomerId = customer.GetAttribute<int?>(SystemCustomerAttributeNames.ImpersonatedCustomerId);
                    if (impersonatedCustomerId.HasValue && impersonatedCustomerId.Value > 0)
                    {
                        var impersonatedCustomer = _customerService.GetCustomerById(impersonatedCustomerId.Value);
                        if (impersonatedCustomer != null && !impersonatedCustomer.Deleted && impersonatedCustomer.Active && !impersonatedCustomer.RequireReLogin)
                        {
                            //set impersonated customer
                            _originalCustomerIfImpersonated = customer;
                            customer = impersonatedCustomer;
                        }
                    }
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //get guest customer
                    var customerCookie = GetCustomerCookie();
                    if (!string.IsNullOrEmpty(customerCookie))
                    {
                        if (Guid.TryParse(customerCookie, out Guid customerGuid))
                        {
                            //get customer from cookie (should not be registered)
                            var customerByCookie = _customerService.GetCustomerByGuid(customerGuid);
                            if (customerByCookie != null && !customerByCookie.IsRegistered())
                                customer = customerByCookie;
                        }
                    }
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //create guest if not exists
                    customer = _customerService.InsertGuestCustomer();
                }

                if (!customer.Deleted && customer.Active && !customer.RequireReLogin)
                {
                    //set customer cookie
                    SetCustomerCookie(customer.CustomerGuid);

                    //cache the found customer
                    _cachedCustomer = customer;
                }



                return _cachedCustomer;
            }
            set
            {
                SetCustomerCookie(value.CustomerGuid);
                _cachedCustomer = value;
            }
        }

        public override Customer OriginalCustomerIfImpersonated
        {
            get
            {
                return _originalCustomerIfImpersonated;
            }
        }
    }
}

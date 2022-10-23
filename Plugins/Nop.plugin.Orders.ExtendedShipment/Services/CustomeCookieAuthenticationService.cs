
using Nop.Core.Domain.Customers;
using Nop.Services.Authentication;
using Nop.Services.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class CustomeCookieAuthenticationService : ICustomAuthenticationService
    {
        #region Fields

        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Customer _cachedCustomer;

        #endregion

        #region Ctor
        public CustomeCookieAuthenticationService(CustomerSettings customerSettings, ICustomerService customerService, IHttpContextAccessor httpContextAccessor)
        {
            this._customerSettings = customerSettings;
            this._customerService = customerService;
            this._httpContextAccessor = httpContextAccessor;
        }
        #endregion
        public async void SignIn(Customer customer, bool isPersistent, string SuperAdminCode = null)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //create claims for customer's username and email
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(customer.Username))
                claims.Add(new Claim(ClaimTypes.Name, customer.Username, ClaimValueTypes.String, NopCookieAuthenticationDefaults.ClaimsIssuer));

            if (!string.IsNullOrEmpty(customer.Email))
                claims.Add(new Claim(ClaimTypes.Email, customer.Email, ClaimValueTypes.Email, NopCookieAuthenticationDefaults.ClaimsIssuer));
            if (!string.IsNullOrEmpty(SuperAdminCode))
                claims.Add(new Claim(ClaimTypes.UserData, "IsSupperAdmin|" + SuperAdminCode, ClaimValueTypes.String, NopCookieAuthenticationDefaults.ClaimsIssuer));
            //create principal for the current authentication scheme
            var userIdentity = new ClaimsIdentity(claims, NopCookieAuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            //set value indicating whether session is persisted and the time at which the authentication was issued
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                IssuedUtc = DateTime.UtcNow
            };

            //sign in
            await _httpContextAccessor.HttpContext.SignInAsync(NopCookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, authenticationProperties);

            //cache authenticated customer
            _cachedCustomer = customer;
        }
        /// <summary>
        /// Sign out
        /// </summary>
        public async void SignOut()
        {
            //reset cached customer
            _cachedCustomer = null;

            //and sign out from the current authentication scheme
            await _httpContextAccessor.HttpContext.SignOutAsync(NopCookieAuthenticationDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Get authenticated customer
        /// </summary>
        /// <returns>Customer</returns>
        public Customer GetAuthenticatedCustomer()
        {
            //whether there is a cached customer
            if (_cachedCustomer != null)
                return _cachedCustomer;

            //try to get authenticated user identity
            var authenticateResult = _httpContextAccessor.HttpContext.AuthenticateAsync(NopCookieAuthenticationDefaults.AuthenticationScheme).Result;
            if (!authenticateResult.Succeeded)
                return null;

            Customer customer = null;
            if (_customerSettings.UsernamesEnabled)
            {
                //try to get customer by username
                var usernameClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name
                    && claim.Issuer.Equals(NopCookieAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
                if (usernameClaim != null)
                    customer = _customerService.GetCustomerByUsername(usernameClaim.Value);
            }
            else
            {
                //try to get customer by email
                var emailClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email
                    && claim.Issuer.Equals(NopCookieAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
                if (emailClaim != null)
                    customer = _customerService.GetCustomerByEmail(emailClaim.Value);
            }

            //whether the found customer is available
            if (customer == null || !customer.Active || customer.RequireReLogin || customer.Deleted || !customer.IsRegistered())
                return null;

            //cache authenticated customer
            _cachedCustomer = customer;

            return _cachedCustomer;
        }
        public bool IsSupperAdmin()
        {
            return _httpContextAccessor.HttpContext.User.Claims.Any(p => p.Type == ClaimTypes.UserData && p.ValueType == ClaimValueTypes.String && p.Value.StartsWith("IsSupperAdmin"));
        }
        public string getSupperAdminCode()
        {
            try
            {
                var t = ((ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity).Claims;
                if (!IsSupperAdmin())
                    return null;
                var claim = _httpContextAccessor.HttpContext.User.Claims.Where(p => p.Type == ClaimTypes.UserData && p.ValueType == ClaimValueTypes.String && p.Value.StartsWith("IsSupperAdmin")).FirstOrDefault();
                if (claim != null && !string.IsNullOrEmpty(claim.Value))
                    return claim.Value.Split('|')[1];
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}

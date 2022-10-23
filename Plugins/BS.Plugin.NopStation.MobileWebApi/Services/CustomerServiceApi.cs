using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public class CustomerServiceApi : ICustomerServiceApi
    {
        private const string CUSTOMERROLES_BY_SYSTEMNAME_KEY = "Nop.customerrole.systemname-{0}";
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly ICacheManager _cacheManager;
        private readonly string Key = "%T97S70SwR$hCg66Kd0Z&";
        private readonly string Issuer = "https://postex.ir";
        private readonly string Audience = "http://myaudience.com";
        public CustomerServiceApi(IRepository<Customer> customerRepository,
            IRepository<CustomerRole> customerRoleRepository,
            ICacheManager cacheManager)
        {
            this._customerRepository = customerRepository;
            this._customerRoleRepository = customerRoleRepository;
            this._cacheManager = cacheManager;
        }

        public virtual CustomerRole GetCustomerRoleBySystemName(string systemName)
        {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;

            string key = string.Format(CUSTOMERROLES_BY_SYSTEMNAME_KEY, systemName);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _customerRoleRepository.Table
                            orderby cr.Id
                            where cr.SystemName == systemName
                            select cr;
                var customerRole = query.FirstOrDefault();
                return customerRole;
            });
        }

        public Customer InsertGuestCustomerByMobile(string deviceId)
        {

            var customer = new Customer
            {
                CustomerGuid = HelperExtension.GetGuid(deviceId),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow
            };

            //add to 'Guests' role
            var guestRole = GetCustomerRoleBySystemName(SystemCustomerRoleNames.Guests);
            if (guestRole == null)
                throw new NopException("'Guests' role could not be loaded");
            customer.CustomerRoles.Add(guestRole);

            _customerRepository.Insert(customer);

            return customer;

        }

        public Customer GetCustomerByVendorId(int vendorId)
        {
            var query = from cr in _customerRepository.Table
                        orderby cr.Id
                        where cr.VendorId == vendorId
                        select cr;
            var customer = new Customer();
            if (query.Any())
            {
                customer = query.FirstOrDefault();
            }
            return customer;
        }

        public bool IsValidCustomer(Customer customer)
        {
            if (customer == null || !customer.IsRegistered() || customer.IsGuest() || customer.Username == null)
                return false;
            return true;
        }

        public string GetToken(string Username)
        {
            var SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, Username),
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
    }
}
